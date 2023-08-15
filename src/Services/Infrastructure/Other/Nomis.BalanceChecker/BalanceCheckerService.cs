// ------------------------------------------------------------------------------------------------------
// <copyright file="BalanceCheckerService.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Util;
using Nethereum.Web3;
using Nomis.BalanceChecker.Contracts;
using Nomis.BalanceChecker.Interfaces;
using Nomis.BalanceChecker.Interfaces.Contracts;
using Nomis.BalanceChecker.Interfaces.Enums;
using Nomis.BalanceChecker.Interfaces.Requests;
using Nomis.BalanceChecker.Settings;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.BalanceChecker
{
    /// <inheritdoc cref="IBalanceCheckerService"/>
    internal sealed class BalanceCheckerService :
        IBalanceCheckerService,
        ISingletonService
    {
        private readonly ILogger<BalanceCheckerService> _logger;
        private readonly BalanceCheckerSettings _settings;
        private readonly Dictionary<BalanceCheckerChain, Web3> _nethereumClients;

        /// <summary>
        /// Initialize <see cref="BalanceCheckerService"/>.
        /// </summary>
        /// <param name="settings"><see cref="BalanceCheckerSettings"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public BalanceCheckerService(
            IOptions<BalanceCheckerSettings> settings,
            ILogger<BalanceCheckerService> logger)
        {
            _logger = logger;
            _settings = settings.Value;
            _nethereumClients = new Dictionary<BalanceCheckerChain, Web3>();
            foreach (var chain in Enum.GetValues<BalanceCheckerChain>())
            {
                _nethereumClients.Add(chain, new Web3(_settings.DataFeeds?.Find(a => a.Blockchain == chain)?.RpcUrl ?? "http://localhost:8545")
                {
                    TransactionManager =
                    {
                        DefaultGasPrice = new(0x4c4b40),
                        DefaultGas = new(0x4c4b40)
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task<Result<IEnumerable<BalanceCheckerTokenInfo>>> TokenBalancesAsync(
            TokenBalancesRequest request)
        {
            if (!new AddressUtil().IsValidAddressLength(request.Owner))
            {
                throw new InvalidAddressException(request.Owner!);
            }

            if (request.TokenAddresses.Any(x => !new AddressUtil().IsValidEthereumAddressHexFormat(x)))
            {
                return await Result<IEnumerable<BalanceCheckerTokenInfo>>.FailAsync("There is an invalid token address in the request.").ConfigureAwait(false);
            }

            var contractsData = _settings.DataFeeds.Find(a => a.Blockchain == request.Blockchain);
            if (!new AddressUtil().IsValidAddressLength(contractsData?.ContractAddress))
            {
                return await Result<IEnumerable<BalanceCheckerTokenInfo>>.FailAsync("Invalid contract address.").ConfigureAwait(false);
            }

            if (string.IsNullOrWhiteSpace(contractsData?.ContractAbi))
            {
                return await Result<IEnumerable<BalanceCheckerTokenInfo>>.FailAsync("ABI must be set.").ConfigureAwait(false);
            }

            var nethereumClient = _nethereumClients[request.Blockchain];
            var contract = nethereumClient.Eth.GetContract(contractsData.ContractAbi, contractsData.ContractAddress);
            var function = contract.GetFunction(contractsData.MethodName);

            var tokenInfos = new List<BalanceCheckerTokenInfo>();
            int skip = 0;
            int batchLimit = contractsData.BatchLimit;
            string[] tokenAddresses = request.TokenAddresses.Take(contractsData.BatchLimit).Skip(skip).ToArray();
            while (tokenAddresses.Any())
            {
                try
                {
                    var result = await function.CallDeserializingToObjectAsync<BalanceCheckerTokensInfo>(request.Owner, tokenAddresses).ConfigureAwait(false);
                    tokenInfos.AddRange(result.TokenInfos);
                    skip += batchLimit;
                    tokenAddresses = request.TokenAddresses.Take(batchLimit + skip).Skip(skip).ToArray();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "There is an error when calling smart-contract for {Blockchain}", request.Blockchain);
                    foreach (string tokenAddress in tokenAddresses)
                    {
                        try
                        {
                            await Task.Delay(100).ConfigureAwait(false);
                            var result = await function.CallDeserializingToObjectAsync<BalanceCheckerTokensInfo>(request.Owner, new List<string> { tokenAddress }.ToArray()).ConfigureAwait(false);
                            tokenInfos.AddRange(result.TokenInfos);
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning(e, "There is an error when calling smart-contract for {Blockchain} with address {Address} and wallet {Wallet}. Calculated {TokensCount}", request.Blockchain, tokenAddress, request.Owner, tokenInfos.Count);
                        }
                    }

                    skip += batchLimit;
                    tokenAddresses = request.TokenAddresses.Take(batchLimit + skip).Skip(skip).ToArray();
                }
            }

            return await Result<IEnumerable<BalanceCheckerTokenInfo>>.SuccessAsync(tokenInfos, "Got token balances by given wallet address and blockchain.").ConfigureAwait(false);
        }
    }
}