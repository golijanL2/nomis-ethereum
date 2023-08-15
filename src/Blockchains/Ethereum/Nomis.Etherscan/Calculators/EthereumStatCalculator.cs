// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumStatCalculator.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Numerics;

using Nomis.Aave.Interfaces.Calculators;
using Nomis.Aave.Interfaces.Responses;
using Nomis.Blockchain.Abstractions.Calculators;
using Nomis.Blockchain.Abstractions.Contracts;
using Nomis.Blockchain.Abstractions.Stats;
using Nomis.Chainanalysis.Interfaces.Calculators;
using Nomis.Chainanalysis.Interfaces.Models;
using Nomis.CyberConnect.Interfaces.Calculators;
using Nomis.CyberConnect.Interfaces.Models;
using Nomis.CyberConnect.Interfaces.Responses;
using Nomis.Dex.Abstractions.Calculators;
using Nomis.Dex.Abstractions.Contracts;
using Nomis.Etherscan.Interfaces.Extensions;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Greysafe.Interfaces.Calculators;
using Nomis.Greysafe.Interfaces.Models;
using Nomis.HapiExplorer.Interfaces.Calculators;
using Nomis.HapiExplorer.Interfaces.Responses;
using Nomis.Snapshot.Interfaces.Calculators;
using Nomis.Snapshot.Interfaces.Models;
using Nomis.Snapshot.Interfaces.Responses;
using Nomis.Tally.Interfaces.Calculators;
using Nomis.Tally.Interfaces.Models;
using Nomis.Utils.Contracts;
using Nomis.Utils.Contracts.Calculators;
using Nomis.Utils.Extensions;

namespace Nomis.Etherscan.Calculators
{
    /// <summary>
    /// Ethereum wallet stats calculator.
    /// </summary>
    internal sealed class EthereumStatCalculator :
        IWalletCommonStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletBalanceStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletTransactionStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletTokenStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletContractStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletSnapshotStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletTallyStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletGreysafeStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletChainanalysisStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletCyberConnectStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletNftStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletAaveStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletDexTokenSwapPairsStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>,
        IWalletHapiStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>
    {
        private readonly decimal _tokenUSDPrice;
        private readonly string _address;
        private readonly decimal _balance;
        private readonly IEnumerable<EtherscanAccountNormalTransaction> _transactions;
        private readonly IEnumerable<EtherscanAccountInternalTransaction> _internalTransactions;
        private readonly IEnumerable<INFTTokenTransfer> _tokenTransfers;
        private readonly IEnumerable<EtherscanAccountERC20TokenEvent> _erc20TokenTransfers;
        private readonly IEnumerable<TokenDataBalance>? _tokenDataBalances;
        private readonly IEnumerable<DexTokenSwapPairsData> _dexTokenSwapPairs;
        private readonly IEnumerable<GreysafeReport>? _greysafeReports;
        private readonly IEnumerable<ChainanalysisReport>? _chainanalysisReports;
        private readonly IEnumerable<CyberConnectLikeData>? _cyberConnectLikes;
        private readonly IEnumerable<CyberConnectEssenceData>? _cyberConnectEssences;
        private readonly IEnumerable<CyberConnectSubscribingProfileData>? _cyberConnectSubscribings;

        /// <inheritdoc />
        public int WalletAge => _transactions.Any()
            ? IWalletStatsCalculator.GetWalletAge(_transactions.Select(x => x.TimeStamp!.ToDateTime()))
            : 1;

        /// <inheritdoc />
        public IList<EthereumTransactionIntervalData> TurnoverIntervals
        {
            get
            {
                var turnoverIntervalsDataList =
                    _transactions.Select(x => new TurnoverIntervalsData(
                        x.TimeStamp!.ToDateTime(),
                        BigInteger.TryParse(x.Value, out var value) ? value : 0,
                        x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true));
                return IWalletStatsCalculator<EthereumTransactionIntervalData>
                    .GetTurnoverIntervals(_tokenUSDPrice, turnoverIntervalsDataList, _transactions.Any() ? _transactions.Min(x => x.TimeStamp!.ToDateTime()) : DateTime.MinValue).ToList();
            }
        }

        /// <inheritdoc />
        public decimal NativeBalance => _balance.ToEth();

        /// <inheritdoc />
        public decimal HistoricalMedianBalanceUSD { get; } = 0;

        /// <inheritdoc />
        public decimal NativeBalanceUSD { get; }

        /// <inheritdoc />
        public decimal BalanceChangeInLastMonth =>
            IWalletStatsCalculator<EthereumTransactionIntervalData>.GetBalanceChangeInLastMonth(TurnoverIntervals);

        /// <inheritdoc />
        public decimal BalanceChangeInLastYear =>
            IWalletStatsCalculator<EthereumTransactionIntervalData>.GetBalanceChangeInLastYear(TurnoverIntervals);

        /// <inheritdoc />
        public decimal WalletTurnover =>
            _transactions.Sum(x => decimal.TryParse(x.Value, out decimal value) ? value.ToEth() : 0);

        /// <inheritdoc />
        public decimal WalletTurnoverUSD => NativeBalance != 0 ? WalletTurnover * NativeBalanceUSD / NativeBalance : 0;

        /// <inheritdoc />
        public IEnumerable<TokenDataBalance>? TokenBalances => _tokenDataBalances?.Any() == true ? _tokenDataBalances : null;

        /// <inheritdoc />
        public int TokensHolding => _erc20TokenTransfers.Select(x => x.TokenSymbol).Distinct().Count();

        /// <inheritdoc />
        public int DeployedContracts =>
            _transactions.Count(x => !string.IsNullOrWhiteSpace(x.ContractAddress));

        /// <inheritdoc />
        public IEnumerable<SnapshotProposal>? SnapshotProposals { get; }

        /// <inheritdoc />
        public IEnumerable<SnapshotVote>? SnapshotVotes { get; }

        /// <inheritdoc />
        public TallyAccount? TallyAccount { get; }

        /// <inheritdoc />
        public IEnumerable<GreysafeReport>? GreysafeReports =>
            _greysafeReports?.Any() == true ? _greysafeReports : null;

        /// <inheritdoc />
        public IEnumerable<ChainanalysisReport>? ChainanalysisReports =>
            _chainanalysisReports?.Any() == true ? _chainanalysisReports : null;

        /// <inheritdoc />
        public CyberConnectProfileData? CyberConnectProfile { get; }

        /// <inheritdoc />
        public IEnumerable<CyberConnectLikeData>? CyberConnectLikes => _cyberConnectLikes?.Any() == true ? _cyberConnectLikes : null;

        /// <inheritdoc />
        public IEnumerable<CyberConnectEssenceData>? CyberConnectEssences => _cyberConnectEssences?.Any() == true ? _cyberConnectEssences : null;

        /// <inheritdoc />
        public IEnumerable<CyberConnectSubscribingProfileData>? CyberConnectSubscribings => _cyberConnectSubscribings?.Any() == true ? _cyberConnectSubscribings : null;

        /// <inheritdoc />
        public AaveUserAccountDataResponse? AaveData { get; }

        /// <inheritdoc />
        public IEnumerable<DexTokenSwapPairsData>? DexTokensSwapPairs => _dexTokenSwapPairs.Any() ? _dexTokenSwapPairs : null;

        /// <inheritdoc />
        public HapiProxyRiskScoreResponse? HapiRiskScore { get; }

        public EthereumStatCalculator(
            string address,
            decimal balance,
            decimal usdBalance,
            decimal medianUsdBalance,
            IEnumerable<EtherscanAccountNormalTransaction> transactions,
            IEnumerable<EtherscanAccountInternalTransaction> internalTransactions,
            IEnumerable<INFTTokenTransfer> tokenTransfers,
            IEnumerable<EtherscanAccountERC20TokenEvent> erc20TokenTransfers,
            SnapshotData? snapshotData,
            TallyAccount? tallyAccount,
            HapiProxyRiskScoreResponse? hapiRiskScore,
            IEnumerable<TokenDataBalance>? tokenDataBalances,
            IEnumerable<DexTokenSwapPairsData> dexTokenSwapPairs,
            AaveUserAccountDataResponse? aaveUserAccountData,
            IEnumerable<GreysafeReport>? greysafeReports,
            IEnumerable<ChainanalysisReport>? chainanalysisReports,
            CyberConnectData? cyberConnectData)
        {
            _tokenUSDPrice = balance > 0 ? usdBalance / balance.ToEth() : 0;
            _address = address;
            _balance = balance;
            NativeBalanceUSD = usdBalance;
            HistoricalMedianBalanceUSD = medianUsdBalance;
            _transactions = transactions;
            _internalTransactions = internalTransactions;
            _tokenTransfers = tokenTransfers;
            _erc20TokenTransfers = erc20TokenTransfers;
            _tokenDataBalances = tokenDataBalances;
            SnapshotVotes = snapshotData?.Votes;
            SnapshotProposals = snapshotData?.Proposals;
            TallyAccount = tallyAccount;
            HapiRiskScore = hapiRiskScore;
            _dexTokenSwapPairs = dexTokenSwapPairs;
            AaveData = aaveUserAccountData;
            _greysafeReports = greysafeReports;
            _chainanalysisReports = chainanalysisReports;
            _cyberConnectLikes = cyberConnectData?.Likes;
            _cyberConnectEssences = cyberConnectData?.Essences;
            _cyberConnectSubscribings = cyberConnectData?.Subscribings;
            CyberConnectProfile = cyberConnectData?.Profile;
        }

        /// <inheritdoc />
        public EthereumWalletStats Stats()
        {
            return (this as IWalletStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>).ApplyCalculators();
        }

        /// <inheritdoc />
        IWalletTransactionStats IWalletTransactionStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>.Stats()
        {
            if (!_transactions.Any())
            {
                return new EthereumWalletStats
                {
                    NoData = true
                };
            }

            var intervals = IWalletStatsCalculator
                .GetTransactionsIntervals(_transactions.Select(x => x.TimeStamp!.ToDateTime())).ToList();
            if (intervals.Count == 0)
            {
                return new EthereumWalletStats
                {
                    NoData = true
                };
            }

            var monthAgo = DateTime.Now.AddMonths(-1);
            var yearAgo = DateTime.Now.AddYears(-1);

            return new EthereumWalletStats
            {
                TotalTransactions = _transactions.Count(),
                TotalRejectedTransactions = _transactions.Count(t => string.Equals(t.IsError, "1", StringComparison.OrdinalIgnoreCase)),
                MinTransactionTime = intervals.Min(),
                MaxTransactionTime = intervals.Max(),
                AverageTransactionTime = intervals.Average(),
                LastMonthTransactions = _transactions.Count(x => x.TimeStamp!.ToDateTime() > monthAgo),
                LastYearTransactions = _transactions.Count(x => x.TimeStamp!.ToDateTime() > yearAgo),
                TimeSinceTheLastTransaction = (int)((DateTime.UtcNow - _transactions.OrderBy(x => x.TimeStamp).Last().TimeStamp!.ToDateTime()).TotalDays / 30)
            };
        }

        /// <inheritdoc />
        IWalletNftStats IWalletNftStatsCalculator<EthereumWalletStats, EthereumTransactionIntervalData>.Stats()
        {
            var soldTokens = _tokenTransfers.Where(x => x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true).ToList();
            var soldSum = IWalletStatsCalculator
                .TokensSum(soldTokens.Select(x => x.Hash!), _internalTransactions.Select(x => (x.Hash!, BigInteger.TryParse(x.Value, out var amount) ? amount : 0)));

            var soldTokensIds = soldTokens.Select(x => x.GetTokenUid());
            var buyTokens = _tokenTransfers.Where(x => x.To?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && soldTokensIds.Contains(x.GetTokenUid()));
            var buySum = IWalletStatsCalculator
                .TokensSum(buyTokens.Select(x => x.Hash!), _internalTransactions.Select(x => (x.Hash!, BigInteger.TryParse(x.Value, out var amount) ? amount : 0)));

            var buyNotSoldTokens = _tokenTransfers.Where(x => x.To?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && !soldTokensIds.Contains(x.GetTokenUid()));
            var buyNotSoldSum = IWalletStatsCalculator
                .TokensSum(buyNotSoldTokens.Select(x => x.Hash!), _internalTransactions.Select(x => (x.Hash!, BigInteger.TryParse(x.Value, out var amount) ? amount : 0)));

            int holdingTokens = _tokenTransfers.Count() - soldTokens.Count;
            decimal nftWorth = buySum == 0 ? 0 : soldSum.ToNative() / buySum.ToNative() * buyNotSoldSum.ToNative();

            return new EthereumWalletStats
            {
                NftHolding = holdingTokens,
                NftTrading = (soldSum - buySum).ToNative(),
                NftWorth = nftWorth
            };
        }
    }
}