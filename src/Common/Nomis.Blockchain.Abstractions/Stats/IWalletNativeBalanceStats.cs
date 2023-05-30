// ------------------------------------------------------------------------------------------------------
// <copyright file="IWalletNativeBalanceStats.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Stats;
using Nomis.Utils.Enums;

// ReSharper disable InconsistentNaming
namespace Nomis.Blockchain.Abstractions.Stats
{
    /// <summary>
    /// Wallet native token balance stats.
    /// </summary>
    public interface IWalletNativeBalanceStats :
        IWalletStats
    {
        /// <summary>
        /// Set wallet native token balance stats.
        /// </summary>
        /// <typeparam name="TWalletStats">The wallet stats type.</typeparam>
        /// <param name="stats">The wallet stats.</param>
        /// <returns>Returns wallet stats with initialized properties.</returns>
        public new TWalletStats FillStatsTo<TWalletStats>(TWalletStats stats)
            where TWalletStats : class, IWalletNativeBalanceStats
        {
            stats.NativeBalance = NativeBalance;
            stats.NativeBalanceUSD = NativeBalanceUSD;
            stats.BalanceChangeInLastMonth = BalanceChangeInLastMonth;
            stats.BalanceChangeInLastYear = BalanceChangeInLastYear;
            stats.WalletTurnover = WalletTurnover;
            stats.WalletTurnoverUSD = WalletTurnoverUSD;
            return stats;
        }

        /// <summary>
        /// Native token symbol.
        /// </summary>
        public string NativeToken { get; }

        /// <summary>
        /// Wallet balance (Native token).
        /// </summary>
        public decimal NativeBalance { get; set; }

        /// <summary>
        /// Wallet balance (Native token in USD).
        /// </summary>
        public decimal NativeBalanceUSD { get; set; }

        /// <summary>
        /// The balance change value in the last month (Native token).
        /// </summary>
        public decimal BalanceChangeInLastMonth { get; set; }

        /// <summary>
        /// The balance change value in the last year (Native token).
        /// </summary>
        public decimal BalanceChangeInLastYear { get; set; }

        /// <summary>
        /// The movement of funds on the wallet (Native token).
        /// </summary>
        public decimal WalletTurnover { get; set; }

        /// <summary>
        /// The movement of funds on the wallet (Native token in USD).
        /// </summary>
        public decimal WalletTurnoverUSD { get; set; }

        /// <summary>
        /// Calculate wallet native token balance stats score.
        /// </summary>
        /// <param name="chainId">Blockchain id.</param>
        /// <param name="calculationModel">Scoring calculation model.</param>
        /// <returns>Returns wallet native token balance stats score.</returns>
        public new double CalculateScore(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            double result = BalanceScore(chainId, NativeBalance, NativeBalanceUSD, calculationModel) / 100 * BalancePercents(chainId, calculationModel);
            result += WalletTurnoverScore(chainId, WalletTurnover, calculationModel) / 100 * WalletTurnoverPercents(chainId, calculationModel);

            return result;
        }

        private static double BalancePercents(
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

        private static double WalletTurnoverPercents(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                    return 23.2 / 100;
                case ScoringCalculationModel.XDEFI:
                    return 7.67 / 100;
                case ScoringCalculationModel.Halo:
                    return 9.0 / 100;
                case ScoringCalculationModel.CommonV2:
                    return 4.43 / 100;
                case ScoringCalculationModel.CommonV1:
                default:
                    return 16.31 / 100;
            }
        }

        private static double BalanceScore(
            ulong chainId,
            decimal balance,
            decimal balanceUSD,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                case ScoringCalculationModel.CommonV2:
                    if (balanceUSD > 0)
                    {
                        return balanceUSD switch
                        {
                            <= 20 => 8.84,
                            <= 200 => 28.44,
                            <= 1000 => 51.35,
                            <= 10000 => 92.21,
                            _ => 100
                        };
                    }

                    return 0;
                case ScoringCalculationModel.CommonV1:
                default:
                    return balance switch
                    {
                        <= 0.2m => 7.7,
                        <= 0.4m => 22.23,
                        <= 0.7m => 23.05,
                        <= 1m => 65.98,
                        _ => 100
                    };
            }
        }

        private static double WalletTurnoverScore(
            ulong chainId,
            decimal turnover,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                case ScoringCalculationModel.CommonV2:
                    return turnover switch
                    {
                        <= 1 => 8.62,
                        <= 10 => 36.15,
                        <= 20 => 64.44,
                        <= 50 => 92.21,
                        _ => 100
                    };
                case ScoringCalculationModel.CommonV1:
                default:
                    return turnover switch
                    {
                        <= 10 => 7.62,
                        <= 50 => 14.67,
                        <= 100 => 27.82,
                        <= 1000 => 55.38,
                        _ => 100
                    };
            }
        }
    }
}