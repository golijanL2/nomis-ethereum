// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using EthScanNet.Lib;
using EthScanNet.Lib.Models.EScan;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Contracts.Services;
using Nethereum.Contracts.Standards.ENS;
using Nethereum.JsonRpc.Client;
using Nethereum.Util;
using Nethereum.Web3;
using Nomis.Aave.Interfaces;
using Nomis.Aave.Interfaces.Contracts;
using Nomis.Aave.Interfaces.Enums;
using Nomis.Aave.Interfaces.Responses;
using Nomis.Blockchain.Abstractions;
using Nomis.Blockchain.Abstractions.Contracts;
using Nomis.Blockchain.Abstractions.Enums;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.Chainanalysis.Interfaces;
using Nomis.Chainanalysis.Interfaces.Contracts;
using Nomis.Chainanalysis.Interfaces.Extensions;
using Nomis.CyberConnect.Interfaces;
using Nomis.CyberConnect.Interfaces.Contracts;
using Nomis.CyberConnect.Interfaces.Extensions;
using Nomis.DefiLlama.Interfaces;
using Nomis.Dex.Abstractions.Contracts;
using Nomis.Dex.Abstractions.Enums;
using Nomis.DexProviderService.Interfaces;
using Nomis.DexProviderService.Interfaces.Contracts;
using Nomis.DexProviderService.Interfaces.Extensions;
using Nomis.DexProviderService.Interfaces.Requests;
using Nomis.Domain.Scoring.Entities;
using Nomis.Etherscan.Calculators;
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Interfaces.Extensions;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Etherscan.Interfaces.Requests;
using Nomis.Etherscan.Interfaces.Responses;
using Nomis.Etherscan.Settings;
using Nomis.Greysafe.Interfaces;
using Nomis.Greysafe.Interfaces.Contracts;
using Nomis.Greysafe.Interfaces.Extensions;
using Nomis.HapiExplorer.Interfaces;
using Nomis.HapiExplorer.Interfaces.Contracts;
using Nomis.HapiExplorer.Interfaces.Responses;
using Nomis.IPFS.Interfaces;
using Nomis.PolygonId.Interfaces;
using Nomis.PolygonId.Interfaces.Contracts;
using Nomis.ScoringService.Interfaces;
using Nomis.Snapshot.Interfaces;
using Nomis.Snapshot.Interfaces.Contracts;
using Nomis.Snapshot.Interfaces.Extensions;
using Nomis.SoulboundTokenService.Interfaces;
using Nomis.SoulboundTokenService.Interfaces.Extensions;
using Nomis.Tally.Interfaces;
using Nomis.Tally.Interfaces.Contracts;
using Nomis.Tally.Interfaces.Extensions;
using Nomis.Utils.Contracts;
using Nomis.Utils.Contracts.Requests;
using Nomis.Utils.Contracts.Services;
using Nomis.Utils.Contracts.Stats;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Extensions;
using Nomis.Utils.Wrapper;

namespace Nomis.Etherscan
{
    /// <inheritdoc cref="IEthereumScoringService"/>
    internal sealed class EtherscanService :
        BlockchainDescriptor,
        IEthereumScoringService,
        ITransientService
    {
        private readonly IScoringService _scoringService;
        private readonly IEvmSoulboundTokenService _soulboundTokenService;
        private readonly ISnapshotService _snapshotService;
        private readonly ITallyService _tallyService;
        private readonly IHapiExplorerService _hapiExplorerService;
        private readonly IDexProviderService _dexProviderService;
        private readonly IDefiLlamaService _defiLlamaService;
        private readonly IAaveService _aaveService;
        private readonly IGreysafeService _greysafeService;
        private readonly IChainanalysisService _chainanalysisService;
        private readonly ICyberConnectService _cyberConnectService;
        private readonly IIPFSService _ipfsService;
        private readonly IPolygonIdService _polygonIdService;
        private readonly EtherscanSettings _settings;
        private readonly Web3 _nethereumClient;
        private readonly HttpClient _coinbaseClient;
        private readonly EScanClient _client;

        /// <summary>
        /// Initialize <see cref="EtherscanService"/>.
        /// </summary>
        /// <param name="settings"><see cref="EtherscanSettings"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        /// <param name="soulboundTokenService"><see cref="IEvmSoulboundTokenService"/>.</param>
        /// <param name="snapshotService"><see cref="ISnapshotService"/>.</param>
        /// <param name="tallyService"><see cref="ITallyService"/>.</param>
        /// <param name="hapiExplorerService"><see cref="IHapiExplorerService"/>.</param>
        /// <param name="dexProviderService"><see cref="IDexProviderService"/>.</param>
        /// <param name="defiLlamaService"><see cref="IDefiLlamaService"/>.</param>
        /// <param name="aaveService"><see cref="IAaveService"/>.</param>
        /// <param name="greysafeService"><see cref="IGreysafeService"/>.</param>
        /// <param name="chainanalysisService"><see cref="IChainanalysisService"/>.</param>
        /// <param name="cyberConnectService"><see cref="ICyberConnectService"/>.</param>
        /// <param name="ipfsService"><see cref="IIPFSService"/>.</param>
        /// <param name="polygonIdService"><see cref="IPolygonIdService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public EtherscanService(
            IOptions<EtherscanSettings> settings,
            IScoringService scoringService,
            IEvmSoulboundTokenService soulboundTokenService,
            ISnapshotService snapshotService,
            ITallyService tallyService,
            IHapiExplorerService hapiExplorerService,
            IDexProviderService dexProviderService,
            IDefiLlamaService defiLlamaService,
            IAaveService aaveService,
            IGreysafeService greysafeService,
            IChainanalysisService chainanalysisService,
            ICyberConnectService cyberConnectService,
            IIPFSService ipfsService,
            IPolygonIdService polygonIdService,
            ILogger<EtherscanService> logger)
            : base(settings.Value.BlockchainDescriptors.TryGetValue(BlockchainKind.Mainnet, out var value) ? value : settings.Value.BlockchainDescriptors[BlockchainKind.Testnet])
        {
            _scoringService = scoringService;
            _soulboundTokenService = soulboundTokenService;
            _snapshotService = snapshotService;
            _tallyService = tallyService;
            _hapiExplorerService = hapiExplorerService;
            _dexProviderService = dexProviderService;
            _defiLlamaService = defiLlamaService;
            _aaveService = aaveService;
            _greysafeService = greysafeService;
            _chainanalysisService = chainanalysisService;
            _cyberConnectService = cyberConnectService;
            _ipfsService = ipfsService;
            _polygonIdService = polygonIdService;
            Logger = logger;
            _settings = settings.Value;
            _client = new(EScanNetwork.MainNet, settings.Value.ApiKey);

            _nethereumClient = new(settings.Value.BlockchainProviderUrl)
            {
                TransactionManager =
                {
                    DefaultGasPrice = new(0x4c4b40),
                    DefaultGas = new(0x4c4b40)
                }
            };

            _coinbaseClient = new()
            {
                BaseAddress = new("https://api.coinbase.com/")
            };
        }

        /// <inheritdoc/>
        public ILogger Logger { get; }

        /// <inheritdoc/>
        public async Task<T?> CallReadFunctionAsync<T>(EthereumCallReadFunctionRequest request)
        {
            if (!new AddressUtil().IsValidAddressLength(request.ContractAddress))
            {
                throw new CustomException("Invalid contract address", statusCode: HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.Abi))
            {
                throw new CustomException("ABI must be set", statusCode: HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(request.FunctionName))
            {
                throw new CustomException("Function name must be set", statusCode: HttpStatusCode.BadRequest);
            }

            var contract = _nethereumClient.Eth.GetContract(request.Abi, request.ContractAddress);
            var function = contract.GetFunction(request.FunctionName);

            var result = await function.CallAsync<T>(request.Parameters).ConfigureAwait(false);
            return result ?? default;
        }

        /// <inheritdoc/>
        public async Task<Result<TWalletScore>> GetWalletStatsAsync<TWalletStatsRequest, TWalletScore, TWalletStats, TTransactionIntervalData>(
            TWalletStatsRequest request,
            CancellationToken cancellationToken = default)
            where TWalletStatsRequest : WalletStatsRequest
            where TWalletScore : IWalletScore<TWalletStats, TTransactionIntervalData>, new()
            where TWalletStats : class, IWalletCommonStats<TTransactionIntervalData>, new()
            where TTransactionIntervalData : class, ITransactionIntervalData
        {
            if (request.Address.EndsWith(".eth", StringComparison.CurrentCultureIgnoreCase))
            {
                request.Address = await new ENSService(new EthApiContractService(new RpcClient(new Uri(_settings.BlockchainProviderUrl ?? string.Empty)))).ResolveAddressAsync(request.Address).ConfigureAwait(false);
            }

            if (!new AddressUtil().IsValidAddressLength(request.Address) || !new AddressUtil().IsValidEthereumAddressHexFormat(request.Address))
            {
                throw new InvalidAddressException(request.Address);
            }

            var mintBlockchain = _dexProviderService.MintChain(request);

            var ethAddress = new EScanAddress(request.Address);
            var balanceWei = (await _client.Accounts.GetBalanceAsync(ethAddress).ConfigureAwait(false)).Balance;
            decimal balanceUsd = await GetUsdBalanceAsync(balanceWei.ToEth()).ConfigureAwait(false);
            var transactions = (await _client.Accounts.GetNormalTransactionsAsync(ethAddress).ConfigureAwait(false)).Transactions;
            var internalTransactions = (await _client.Accounts.GetInternalTransactionsAsync(ethAddress).ConfigureAwait(false)).Transactions;
            var tokens = (await _client.Accounts.GetTokenEvents(ethAddress).ConfigureAwait(false)).TokenTransferEvents;
            var erc20Tokens = (await _client.Accounts.GetERC20TokenEvents(ethAddress).ConfigureAwait(false)).ERC20TokenTransferEvents;

            #region HAPI protocol

            HapiProxyRiskScoreResponse? hapiRiskScore = null;
            if ((request as IWalletHapiProtocolRequest)?.GetHapiProtocolData == true)
            {
                try
                {
                    hapiRiskScore = (await _hapiExplorerService.GetWalletRiskScoreAsync("ethereum", request.Address).ConfigureAwait(false)).Data;
                }
                catch (NoDataException)
                {
                    // ignored
                }
            }

            #endregion HAPI protocol

            #region Greysafe scam reports

            var greysafeReportsResponse = await _greysafeService.ReportsAsync(request as IWalletGreysafeRequest).ConfigureAwait(false);

            #endregion Greysafe scam reports

            #region Chainanalysis sanctions reports

            var chainanalysisReportsResponse = await _chainanalysisService.ReportsAsync(request as IWalletChainanalysisRequest).ConfigureAwait(false);

            #endregion Chainanalysis sanctions reports

            #region Snapshot protocol

            var snapshotData = await _snapshotService.DataAsync(request as IWalletSnapshotProtocolRequest, ChainId).ConfigureAwait(false);

            #endregion Snapshot protocol

            #region Tally protocol

            var tallyAccountData = await _tallyService.AccountDataAsync(request as IWalletTallyProtocolRequest, ChainId).ConfigureAwait(false);

            #endregion Tally protocol

            #region CyberConnect protocol

            var cyberConnectData = await _cyberConnectService.DataAsync(request as IWalletCyberConnectProtocolRequest, ChainId).ConfigureAwait(false);

            #endregion CyberConnect protocol

            #region Tokens data

            var tokenDataBalances = new List<TokenDataBalance>();
            if ((request as IWalletTokensSwapPairsRequest)?.GetTokensSwapPairs == true
                || (request as IWalletTokensBalancesRequest)?.GetHoldTokensBalances == true)
            {
                foreach (string? erc20TokenContractId in erc20Tokens.Select(x => x.ContractAddress).Distinct())
                {
                    await Task.Delay(250, cancellationToken).ConfigureAwait(false);
                    var tokenBalance = (await _client.Accounts.GetTokenBalanceForAddress(ethAddress, new(erc20TokenContractId)).ConfigureAwait(false)).Balance;
                    if (tokenBalance > 0)
                    {
                        tokenDataBalances.Add(new TokenDataBalance
                        {
                            Balance = tokenBalance,
                            Id = erc20TokenContractId
                        });
                    }
                }
            }

            #endregion Tokens data

            #region Tokens balances (DefiLlama)

            var dexTokensData = new List<DexTokenData>();
            if (request is IWalletTokensBalancesRequest balancesRequest)
            {
                var tokenPrices = await _defiLlamaService.TokensPriceAsync(
                    tokenDataBalances.Select(t => $"{PlatformIds?[BlockchainPlatform.DefiLLama]}:{t.Id}").ToList(), balancesRequest.SearchWidthInHours).ConfigureAwait(false);
                foreach (var tokenDataBalance in tokenDataBalances)
                {
                    if (tokenPrices?.TokensPrices.ContainsKey($"{PlatformIds?[BlockchainPlatform.DefiLLama]}:{tokenDataBalance.Id}") == true)
                    {
                        var tokenPrice = tokenPrices.TokensPrices[$"{PlatformIds?[BlockchainPlatform.DefiLLama]}:{tokenDataBalance.Id}"];
                        tokenDataBalance.Confidence = tokenPrice.Confidence;
                        tokenDataBalance.LastPriceDateTime = tokenPrice.LastPriceDateTime;
                        tokenDataBalance.Price = tokenPrice.Price;
                        tokenDataBalance.Decimals ??= tokenPrice.Decimals?.ToString();
                        tokenDataBalance.Symbol ??= tokenPrice.Symbol;
                    }
                }

                if (balancesRequest.UseTokenLists)
                {
                    var dexTokensDataResult = await _dexProviderService.TokensDataAsync(new TokensDataRequest
                    {
                        Blockchain = (Chain)ChainId,
                        IncludeUniversalTokenLists = balancesRequest.IncludeUniversalTokenLists,
                        FromCache = true
                    }).ConfigureAwait(false);
                    dexTokensData = dexTokensDataResult.Data;

                    foreach (var tokenDataBalance in tokenDataBalances)
                    {
                        var dexTokenData = dexTokensDataResult.Data.FirstOrDefault(t => t.Id?.Equals(tokenDataBalance.Id, StringComparison.OrdinalIgnoreCase) == true);
                        tokenDataBalance.LogoUri ??= dexTokenData?.LogoUri;
                        tokenDataBalance.Decimals ??= dexTokenData?.Decimals;
                        tokenDataBalance.Symbol ??= dexTokenData?.Symbol;
                        tokenDataBalance.Name ??= dexTokenData?.Name;
                    }
                }

                tokenDataBalances = tokenDataBalances
                    .OrderByDescending(b => b.TotalAmountPrice)
                    .ThenByDescending(b => b.Amount)
                    .ThenBy(b => b.Symbol)
                    .ToList();
            }

            #endregion Tokens balances

            #region Swap pairs from DEXes

            var dexTokenSwapPairs = new List<DexTokenSwapPairsData>();
            if ((request as IWalletTokensSwapPairsRequest)?.GetTokensSwapPairs == true && tokenDataBalances.Any())
            {
                var swapPairsResult = await _dexProviderService.BlockchainSwapPairsAsync(new()
                {
                    Blockchain = (Chain)ChainId,
                    First = (request as IWalletTokensSwapPairsRequest)?.FirstSwapPairs ?? 100,
                    Skip = (request as IWalletTokensSwapPairsRequest)?.Skip ?? 0,
                    FromCache = false
                }).ConfigureAwait(false);
                if (swapPairsResult.Succeeded)
                {
                    dexTokenSwapPairs.AddRange(tokenDataBalances.Select(t =>
                        DexTokenSwapPairsData.ForSwapPairs(t.Id!, t.Balance, swapPairsResult.Data, dexTokensData)));
                    dexTokenSwapPairs.RemoveAll(p => !p.TokenSwapPairs.Any());
                }
            }

            #endregion Swap pairs from DEXes

            #region Aave protocol

            AaveUserAccountDataResponse? aaveAccountDataResponse = null;
            if ((request as IWalletAaveProtocolRequest)?.GetAaveProtocolData == true)
            {
                aaveAccountDataResponse = (await _aaveService.GetAaveUserAccountDataAsync(AaveChain.Ethereum, request.Address).ConfigureAwait(false)).Data;
            }

            #endregion Aave protocol

            var walletStats = new EthereumStatCalculator(
                    request.Address,
                    balanceWei,
                    balanceUsd,
                    transactions,
                    internalTransactions,
                    tokens,
                    erc20Tokens,
                    snapshotData,
                    tallyAccountData,
                    hapiRiskScore,
                    tokenDataBalances,
                    dexTokenSwapPairs,
                    aaveAccountDataResponse,
                    greysafeReportsResponse?.Reports,
                    chainanalysisReportsResponse?.Identifications,
                    cyberConnectData)
                .Stats() as TWalletStats;

            double score = walletStats!.CalculateScore<TWalletStats, TTransactionIntervalData>(ChainId, request.CalculationModel);
            var scoringData = new ScoringData(request.Address, request.Address, request.CalculationModel, JsonSerializer.Serialize(request), ChainId, score, JsonSerializer.Serialize(walletStats));
            await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken).ConfigureAwait(false);

            var metadataResult = await _soulboundTokenService.TokenMetadataAsync(_ipfsService, request, ChainId, ChainName, score).ConfigureAwait(false);

            // getting signature
            ushort mintedScore = (ushort)(score * 10000);
            var signatureResult = await _soulboundTokenService
                .SignatureAsync(request, mintedScore, mintBlockchain?.ChainId ?? request.GetChainId(_settings), mintBlockchain?.SBTData ?? request.GetSBTData(_settings), metadataResult.Data, ChainId, (request as IWalletGreysafeRequest)?.GetGreysafeData, (request as IWalletChainanalysisRequest)?.GetChainanalysisData, (request as IWalletHapiProtocolRequest)?.GetHapiProtocolData).ConfigureAwait(false);
            var messages = signatureResult.Messages;
            messages.Add($"Got {ChainName} wallet {request.ScoreType.ToString()} score.");

            #region DID

            var didDataResult = await _polygonIdService.CreateClaimAndGetQrAsync<EthereumWalletStatsRequest, EthereumWalletStats, EthereumTransactionIntervalData>((request as EthereumWalletStatsRequest) !, mintedScore, (walletStats as EthereumWalletStats) !, DateTime.UtcNow.AddYears(5).ConvertToTimestamp(), ChainId, cancellationToken).ConfigureAwait(false);
            messages.Add(didDataResult.Messages.FirstOrDefault() ?? string.Empty);

            #endregion DID

            return await Result<TWalletScore>.SuccessAsync(
                new()
                {
                    Address = request.Address,
                    Stats = walletStats,
                    Score = score,
                    MintData = request.PrepareToMint ? new MintData(signatureResult.Data.Signature, mintedScore, request.CalculationModel, request.Deadline, metadataResult.Data, ChainId, mintBlockchain ?? this) : null,
                    DIDData = didDataResult.Data
                }, messages).ConfigureAwait(false);
        }

        private async Task<decimal> GetUsdBalanceAsync(decimal balance)
        {
            var response = await _coinbaseClient.GetAsync("/v2/prices/ETH-USD/spot").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<CoinbaseSpotPriceResponse>().ConfigureAwait(false) ?? throw new CustomException("Can't get USD balance.");

            if (decimal.TryParse(data.Data?.Amount, NumberStyles.AllowDecimalPoint, new NumberFormatInfo() { CurrencyDecimalSeparator = "." }, out decimal decimalAmount))
            {
                return balance * decimalAmount;
            }

            return 0;
        }
    }
}