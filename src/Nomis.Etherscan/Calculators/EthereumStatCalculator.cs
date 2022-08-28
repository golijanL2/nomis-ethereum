using System.Numerics;

using EthScanNet.Lib.Models.ApiResponses.Accounts.Models;
using Nomis.Etherscan.Extensions;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Utils.Extensions;

namespace Nomis.Etherscan.Calculators
{
    /// <summary>
    /// Ethereum wallet stats calculator.
    /// </summary>
    internal sealed class EthereumStatCalculator
    {
        private readonly string _address;
        private readonly BigInteger _balance;
        private readonly IEnumerable<EScanTransaction> _transactions;
        private readonly IEnumerable<EScanTransaction> _internalTransactions;
        private readonly IEnumerable<EScanTokenTransferEvent> _tokenTransfers;
        private readonly IEnumerable<EScanTokenTransferEvent> _ecr20TokenTransfers;

        public EthereumStatCalculator(
            string address,
            BigInteger balance,
            IEnumerable<EScanTransaction> transactions,
            IEnumerable<EScanTransaction> internalTransactions,
            IEnumerable<EScanTokenTransferEvent> tokenTransfers,
            IEnumerable<EScanTokenTransferEvent> ecr20TokenTransfers)
        {
            _address = address;
            _balance = balance;
            _transactions = transactions;
            _internalTransactions = internalTransactions;
            _tokenTransfers = tokenTransfers;
            _ecr20TokenTransfers = ecr20TokenTransfers;
        }

        private int GetWalletAge()
        {
            var firstTransaction = _transactions.First();
            return (int)((DateTime.UtcNow - firstTransaction.TimeStamp.ToDateTime()).TotalDays / 30);
        }

        private IEnumerable<double> GetTransactionsIntervals()
        {
            var result = new List<double>();
            DateTime? lasDateTime = null;
            foreach (var transaction in _transactions)
            {

                var transactionDate = transaction.TimeStamp.ToDateTime();
                if (!lasDateTime.HasValue)
                {
                    lasDateTime = transactionDate;
                    continue;
                }

                var interval = (transactionDate - lasDateTime.Value).TotalHours;
                result.Add(interval);
            }

            return result;
        }

        private BigInteger GetTokensSum(IEnumerable<EScanTokenTransferEvent> tokenList)
        {
            var transactions = tokenList.Select(x => x.Hash).ToHashSet();
            var result = new BigInteger();
            foreach (var st in _internalTransactions.Where(x => transactions.Contains(x.Hash)))
            {
                result += st.Value;
            }

            return result;
        }

        public EthereumWalletStats GetStats()
        {
            if (!_transactions.Any())
            {
                return new()
                {
                    NoData = true
                };
            }

            var intervals = GetTransactionsIntervals().ToList();
            if (!intervals.Any())
            {
                return new()
                {
                    NoData = true
                };
            }

            var monthAgo = DateTime.Now.AddMonths(-1);

            var soldTokens = _tokenTransfers.Where(x => x.From?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true).ToList();
            var soldSum = GetTokensSum(soldTokens);

            var soldTokensIds = soldTokens.Select(x => x.GetTokenUid());
            var buyTokens = _tokenTransfers.Where(x => x.To?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && soldTokensIds.Contains(x.GetTokenUid()));
            var buySum = GetTokensSum(buyTokens);

            var buyNotSoldTokens = _tokenTransfers.Where(x => x.To?.Equals(_address, StringComparison.InvariantCultureIgnoreCase) == true && !soldTokensIds.Contains(x.GetTokenUid()));
            var buyNotSoldSum = GetTokensSum(buyNotSoldTokens);

            var holdingTokens = _tokenTransfers.Count() - soldTokens.Count;
            var nftWorth = buySum == 0 ? 0 : (decimal)soldSum / (decimal)buySum * (decimal)buyNotSoldSum;
            var contractsCreated = _transactions.Count(x => !string.IsNullOrWhiteSpace(x.ContractAddress));
            var totalTokens = _ecr20TokenTransfers.Select(x => x.TokenSymbol).Distinct();

            return new()
            {
                Balance = _balance.ToEth(),
                WalletAge = GetWalletAge(),
                TotalTransactions = _transactions.Count(),
                MinTransactionTime = intervals.Min(),
                MaxTransactionTime = intervals.Max(),
                AverageTransactionTime = intervals.Average(),
                WalletTurnover = _transactions.Sum(x => (decimal)x.Value).ToEth(),
                LastMonthTransactions = _transactions.Count(x => x.TimeStamp.ToDateTime() > monthAgo),
                TimeFromLastTransaction = (int)((DateTime.UtcNow - _transactions.Last().TimeStamp.ToDateTime()).TotalDays / 30),
                NftHolding = holdingTokens,
                NftTrading = (soldSum - buySum).ToEth(),
                NftWorth = nftWorth.ToEth(),
                DeployedContracts = contractsCreated,
                TokensHolding = totalTokens.Count()
            };
        }
    }
}