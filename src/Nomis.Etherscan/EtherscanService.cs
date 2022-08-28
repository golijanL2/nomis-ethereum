using System.Net;

using EthScanNet.Lib;
using EthScanNet.Lib.Models.EScan;
using Microsoft.Extensions.Options;
using Nethereum.ENS;
using Nethereum.Util;
using Nethereum.Web3;
using Nomis.Blockchain.Abstractions.Enums;
using Nomis.Etherscan.Calculators;
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Etherscan.Interfaces.Settings;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.Etherscan
{
    /// <inheritdoc cref="IEtherscanService"/>
    internal sealed class EtherscanService :
        IEtherscanService,
        ITransientService
    {
        private readonly EtherscanSettings _settings;

        /// <summary>
        /// Initialize <see cref="EtherscanService"/>.
        /// </summary>
        /// <param name="settings"><see cref="EtherscanSettings"/>.</param>
        public EtherscanService(
            IOptions<EtherscanSettings> settings)
        {
            _settings = settings.Value;
            var network = settings.Value.Network switch
            {
                BlockchainNetwork.Ethereum => EScanNetwork.MainNet,
                _ => throw new InvalidOperationException($"Invalid {nameof(settings.Value.Network)}")
            };
            Client = new(network, settings.Value.ApiKey);
        }

        /// <inheritdoc/>
        public EScanClient Client { get; }

        /// <inheritdoc/>
        public async Task<Result<EthereumWalletScore>> GetWalletStatsAsync(string address)
        {
            if (address.EndsWith(".eth", StringComparison.CurrentCultureIgnoreCase))
            {
                var web3 = new Web3(_settings.BlockchainProviderUrl);
                address = await new ENSService(web3).ResolveAddressAsync(address);
            }

            if (!new AddressUtil().IsValidAddressLength(address) || !new AddressUtil().IsValidEthereumAddressHexFormat(address))
            {
                throw new CustomException("Invalid address", statusCode: HttpStatusCode.BadRequest);
            }

            var ethAddress = new EScanAddress(address);
            var balanceWei = (await Client.Accounts.GetBalanceAsync(ethAddress)).Balance;
            var transactions = (await Client.Accounts.GetNormalTransactionsAsync(ethAddress)).Transactions;
            var internalTransactions = (await Client.Accounts.GetInternalTransactionsAsync(ethAddress)).Transactions;
            var tokens = (await Client.Accounts.GetTokenEvents(ethAddress)).TokenTransferEvents;
            var erc20Tokens = (await Client.Accounts.GetERC20TokenEvents(ethAddress)).ERC20TokenTransferEvents;

            var walletStats = new EthereumStatCalculator(
                    address,
                    balanceWei,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens)
                .GetStats();

            return await Result<EthereumWalletScore>.SuccessAsync(new()
            {
                Stats = walletStats,
                Score = walletStats.GetScore()
            }, "Got ethereum wallet score.");
        }
    }
}