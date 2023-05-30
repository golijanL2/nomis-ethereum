// ------------------------------------------------------------------------------------------------------
// <copyright file="IWalletTokenBalancesStats.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts;
using Nomis.Utils.Contracts.Stats;
using Nomis.Utils.Enums;

// ReSharper disable InconsistentNaming
namespace Nomis.Blockchain.Abstractions.Stats
{
    /// <summary>
    /// Wallet hold token balances stats.
    /// </summary>
    public interface IWalletTokenBalancesStats :
        IWalletStats
    {
        /// <summary>
        /// Set wallet hold token balances stats.
        /// </summary>
        /// <typeparam name="TWalletStats">The wallet stats type.</typeparam>
        /// <param name="stats">The wallet stats.</param>
        /// <returns>Returns wallet stats with initialized properties.</returns>
        public new TWalletStats FillStatsTo<TWalletStats>(TWalletStats stats)
            where TWalletStats : class, IWalletTokenBalancesStats
        {
            stats.TokenBalances = TokenBalances;
            return stats;
        }

        /// <summary>
        /// Hold token balances.
        /// </summary>
        public IEnumerable<TokenDataBalance>? TokenBalances { get; set; }

        /// <summary>
        /// Wallet hold tokens total balance (USD).
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public decimal HoldTokensBalanceUSD { get; }

        /// <summary>
        /// Calculate wallet DEX token balances stats score.
        /// </summary>
        /// <param name="chainId">Blockchain id.</param>
        /// <param name="calculationModel">Scoring calculation model.</param>
        /// <returns>Returns DEX token balances stats score.</returns>
        public new double CalculateScore(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            double result = HoldTokensBalanceScore(chainId, HoldTokensBalanceUSD, calculationModel) / 100 * HoldTokensBalancePercents(chainId, calculationModel);

            return result;
        }

        private static double HoldTokensBalancePercents(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                    return 20.41 / 100;
                case ScoringCalculationModel.XDEFI:
                    return 14.49 / 100;
                case ScoringCalculationModel.Halo:
                    return 17.23 / 100;
                case ScoringCalculationModel.CommonV2:
                    return 26.05 / 100;
                case ScoringCalculationModel.CommonV1:
                default:
                    return 26.88 / 100;
            }
        }

        private static double HoldTokensBalanceScore(
            ulong chainId,
            decimal balanceUSD,
            ScoringCalculationModel calculationModel)
        {
            if (balanceUSD == 0)
            {
                return 0;
            }

            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                case ScoringCalculationModel.CommonV2:
                    return balanceUSD switch
                    {
                        <= 20 => 8.84,
                        <= 200 => 28.44,
                        <= 1000 => 51.35,
                        <= 10000 => 92.21,
                        _ => 100
                    };
                case ScoringCalculationModel.CommonV1:
                default:
                    return 0;
            }
        }
    }
}