// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanService.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Numerics;
using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;
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
using Nomis.Blockchain.Abstractions.Settings;
using Nomis.Blockscout.Interfaces;
using Nomis.CacheProviderService.Interfaces;
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
using Nomis.ReferralService.Interfaces;
using Nomis.ReferralService.Interfaces.Extensions;
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
        private readonly BlacklistSettings _blacklistSettings;
        private readonly IEtherscanClient _client;
        private readonly IScoringService _scoringService;
        private readonly IReferralService _referralService;
        private readonly IEvmScoreSoulboundTokenService _soulboundTokenService;
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
        private readonly ICacheProviderService _cacheProviderService;
        private readonly IBlockscoutApiService _blockscoutApiService;
        private readonly EtherscanSettings _settings;
        private readonly Web3 _nethereumClient;
        private readonly HttpClient _coinbaseClient;

        /// <summary>
        /// Initialize <see cref="EtherscanService"/>.
        /// </summary>
        /// <param name="blacklistSettings"><see cref="BlacklistSettings"/>.</param>
        /// <param name="settings"><see cref="EtherscanSettings"/>.</param>
        /// <param name="client"><see cref="IEtherscanClient"/>.</param>
        /// <param name="scoringService"><see cref="IScoringService"/>.</param>
        /// <param name="referralService"><see cref="IReferralService"/>.</param>
        /// <param name="soulboundTokenService"><see cref="IEvmScoreSoulboundTokenService"/>.</param>
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
        /// <param name="cacheProviderService"><see cref="ICacheProviderService"/>.</param>
        /// <param name="blockscoutApiService"><see cref="IBlockscoutApiService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public EtherscanService(
            IOptions<BlacklistSettings> blacklistSettings,
            IOptions<EtherscanSettings> settings,
            IEtherscanClient client,
            IScoringService scoringService,
            IReferralService referralService,
            IEvmScoreSoulboundTokenService soulboundTokenService,
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
            ICacheProviderService cacheProviderService,
            IBlockscoutApiService blockscoutApiService,
            ILogger<EtherscanService> logger)
#pragma warning disable S3358
            : base(settings.Value.BlockchainDescriptors.TryGetValue(BlockchainKind.Mainnet, out var value) ? value : settings.Value.BlockchainDescriptors.TryGetValue(BlockchainKind.Testnet, out var testnetValue) ? testnetValue : null)
#pragma warning restore S3358
        {
            _blacklistSettings = blacklistSettings.Value;
            _client = client;
            _scoringService = scoringService;
            _referralService = referralService;
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
            _cacheProviderService = cacheProviderService;
            _blockscoutApiService = blockscoutApiService;
            Logger = logger;
            _settings = settings.Value;

            _nethereumClient = new(settings.Value.BlockchainProviderUrl ?? "https://rpc.ankr.com/eth")
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

            #region Blacklist

            if (_blacklistSettings.UseBlacklist)
            {
                var blacklist = new List<string>();
                foreach (var blacklistItem in _blacklistSettings.Blacklist)
                {
                    blacklist.AddRange(blacklistItem.Value);
                }

                if (blacklist.Contains(request.Address.ToLower()))
                {
                    throw new CustomException("The specified wallet address cannot be scored.", statusCode: HttpStatusCode.BadRequest);
                }
            }

            #endregion Blacklist

            var messages = new List<string>();

            #region Referral

            var ownReferralCodeResult = await _referralService.GetOwnReferralCodeAsync(request, Logger, cancellationToken).ConfigureAwait(false);
            messages.AddRange(ownReferralCodeResult.Messages);
            string? ownReferralCode = ownReferralCodeResult.Data;

            #endregion Referral

            var mintBlockchain = _dexProviderService.MintChain(request, ChainId);

            TWalletStats? walletStats = null;
            bool calculateNewScore = false;
            if (_settings.GetFromCacheStatsIsEnabled)
            {
                walletStats = await _cacheProviderService.GetFromCacheAsync<EthereumWalletStats>($"{request.Address}_{ChainId}_{(int)request.CalculationModel}_{(int)request.ScoreType}").ConfigureAwait(false) as TWalletStats;
            }

            if (walletStats == null)
            {
                calculateNewScore = true;
                string? balanceWei = (await _client.GetBalanceAsync(request.Address).ConfigureAwait(false)).Balance;
                decimal usdBalance = await GetUsdBalanceAsync(balanceWei!.ToEth()).ConfigureAwait(false);
                var tokenTransfers = new List<INFTTokenTransfer>();
                var transactions = (await _client.GetTransactionsAsync<EtherscanAccountNormalTransactions, EtherscanAccountNormalTransaction>(request.Address).ConfigureAwait(false)).ToList();
                if (!transactions.Any())
                {
                    return await Result<TWalletScore>.FailAsync(
                        new()
                        {
                            Address = request.Address,
                            Stats = new TWalletStats
                            {
                                NoData = true
                            },
                            Score = 0
                        }, new List<string> { "There is no transactions for this wallet." }).ConfigureAwait(false);
                }

                var erc20Tokens = (await _client.GetTransactionsAsync<EtherscanAccountERC20TokenEvents, EtherscanAccountERC20TokenEvent>(request.Address).ConfigureAwait(false)).ToList();
                var internalTransactions = (await _client.GetTransactionsAsync<EtherscanAccountInternalTransactions, EtherscanAccountInternalTransaction>(request.Address).ConfigureAwait(false)).ToList();
                var erc721Tokens = (await _client.GetTransactionsAsync<EtherscanAccountERC721TokenEvents, EtherscanAccountERC721TokenEvent>(request.Address).ConfigureAwait(false)).ToList();

                // var erc1155Tokens = (await _client.GetTransactionsAsync<EtherscanAccountERC1155TokenEvents, EtherscanAccountERC1155TokenEvent>(request.Address).ConfigureAwait(false)).ToList();
                // tokenTransfers.AddRange(erc1155Tokens);
                tokenTransfers.AddRange(erc721Tokens);

                #region Counterparties

                if (_settings is IFilterCounterpartiesByCalculationModelSettings counterpartiesSettings &&
                    counterpartiesSettings.CounterpartiesFilterData.TryGetValue(request.CalculationModel, out var counterpartiesData))
                {
                    var counterpartiesContracts = counterpartiesData.Where(x => x.UseCounterparty).Select(x => x.ContractAddress.ToLowerInvariant()).ToList();
                    transactions = transactions.Where(x => counterpartiesContracts.Contains(x.ContractAddress?.ToLowerInvariant() ?? string.Empty) || counterpartiesContracts.Contains(x.To?.ToLowerInvariant() ?? string.Empty)).ToList();
                    internalTransactions = internalTransactions.Where(x => counterpartiesContracts.Contains(x.To?.ToLowerInvariant() ?? string.Empty)).ToList();
                    erc20Tokens = erc20Tokens.Where(x => counterpartiesContracts.Contains(x.ContractAddress?.ToLowerInvariant() ?? string.Empty) || counterpartiesContracts.Contains(x.To?.ToLowerInvariant() ?? string.Empty)).ToList();
                }

                #endregion Counterparties

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
                        await _settings.WaitForRequestRateLimit().ConfigureAwait(false);
                        var tokenBalance = (await _client.GetTokenBalanceAsync(request.Address, erc20TokenContractId!).ConfigureAwait(false)).Balance?.ToBigInteger();
                        if (tokenBalance > 0)
                        {
                            tokenDataBalances.Add(new TokenDataBalance
                            {
                                ChainId = ChainId,
                                Balance = (BigInteger)tokenBalance,
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
                            var dexTokenData = dexTokensDataResult.Data.Find(t => t.Id?.Equals(tokenDataBalance.Id, StringComparison.OrdinalIgnoreCase) == true);
                            tokenDataBalance.LogoUri ??= dexTokenData?.LogoUri;
                            tokenDataBalance.Decimals ??= dexTokenData?.Decimals;
                            tokenDataBalance.Symbol ??= dexTokenData?.Symbol;
                            tokenDataBalance.Name ??= dexTokenData?.Name;
                        }
                    }

                    tokenDataBalances = tokenDataBalances
                        .Where(b => b.TotalAmountPrice > 0)
                        .OrderByDescending(b => b.TotalAmountPrice)
                        .ThenByDescending(b => b.Balance)
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

                #region Median USD balance

                decimal medianUsdBalance = await _scoringService.MedianBalanceUsdAsync<EthereumWalletStats>(request.Address, ChainId, request.CalculationModel, _settings, usdBalance + (tokenDataBalances.Any() ? tokenDataBalances : null)?.Sum(b => b.TotalAmountPrice) ?? 0, cancellationToken).ConfigureAwait(false);

                #endregion Median USD balance

                walletStats = new EthereumStatCalculator(
                        request.Address,
                        decimal.TryParse(balanceWei, out decimal weiBalance) ? weiBalance : 0,
                        usdBalance,
                        medianUsdBalance,
                        transactions,
                        internalTransactions,
                        tokenTransfers,
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

                await _cacheProviderService.SetCacheAsync($"{request.Address}_{ChainId}_{(int)request.CalculationModel}_{(int)request.ScoreType}", walletStats!, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _settings.GetFromCacheStatsTimeLimit
                }).ConfigureAwait(false);
            }

            double score = walletStats!.CalculateScore<TWalletStats, TTransactionIntervalData>(ChainId, request.CalculationModel);

            if (calculateNewScore)
            {
                var scoringData = new ScoringData(request.Address, request.Address, request.CalculationModel, JsonSerializer.Serialize(request), ChainId, score, JsonSerializer.Serialize(walletStats));
                await _scoringService.SaveScoringDataToDatabaseAsync(scoringData, cancellationToken).ConfigureAwait(false);
            }

            var metadataResult = await _soulboundTokenService.TokenMetadataAsync(_ipfsService, _cacheProviderService, request, ChainId, ChainName, score).ConfigureAwait(false);

            // getting signature
            ushort mintedScore = (ushort)(score * 10000);
            var signatureResult = await _soulboundTokenService
                .SignatureAsync(request, mintedScore, mintBlockchain?.ChainId ?? request.GetChainId(_settings), mintBlockchain?.SBTData ?? request.GetSBTData(_settings), metadataResult.Data, ChainId, ownReferralCode ?? "anon", request.ReferrerCode ?? "nomis", (request as IWalletGreysafeRequest)?.GetGreysafeData, (request as IWalletChainanalysisRequest)?.GetChainanalysisData, (request as IWalletHapiProtocolRequest)?.GetHapiProtocolData).ConfigureAwait(false);

            messages.AddRange(signatureResult.Messages);
            messages.Add(calculateNewScore ? $"Got {ChainName} wallet {request.ScoreType.ToString()} score." : $"Got {ChainName} wallet {request.ScoreType.ToString()} score created before.");

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
                    MintData = request.PrepareToMint ? new MintData(signatureResult.Data.Signature, mintedScore, request.CalculationModel, request.Deadline, metadataResult.Data, ChainId, mintBlockchain ?? this, ownReferralCode ?? "anon", request.ReferrerCode ?? "nomis") : null,
                    DIDData = didDataResult.Data,
                    ReferralCode = ownReferralCode,
                    ReferrerCode = request.ReferrerCode
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