﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="IWalletTransactionStats.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Stats;
using Nomis.Utils.Enums;

namespace Nomis.Blockchain.Abstractions.Stats
{
    /// <summary>
    /// Wallet transaction stats.
    /// </summary>
    public interface IWalletTransactionStats :
        IWalletStats
    {
        /// <summary>
        /// Set wallet transaction stats.
        /// </summary>
        /// <typeparam name="TWalletStats">The wallet stats type.</typeparam>
        /// <param name="stats">The wallet stats.</param>
        /// <returns>Returns wallet stats with initialized properties.</returns>
        public new TWalletStats FillStatsTo<TWalletStats>(TWalletStats stats)
            where TWalletStats : class, IWalletTransactionStats
        {
            stats.TotalTransactions = TotalTransactions;
            stats.TotalRejectedTransactions = TotalRejectedTransactions;
            stats.AverageTransactionTime = AverageTransactionTime;
            stats.MaxTransactionTime = MaxTransactionTime;
            stats.MinTransactionTime = MinTransactionTime;
            stats.TimeFromLastTransaction = TimeFromLastTransaction;
            stats.LastMonthTransactions = LastMonthTransactions;
            stats.LastYearTransactions = LastYearTransactions;
            return stats;
        }

        /// <summary>
        /// Total transactions on wallet (number).
        /// </summary>
        public int TotalTransactions { get; set; }

        /// <summary>
        /// Total rejected transactions on wallet (number).
        /// </summary>
        public int TotalRejectedTransactions { get; set; }

        /// <summary>
        /// Average time interval between transactions (hours).
        /// </summary>
        public double AverageTransactionTime { get; set; }

        /// <summary>
        /// Maximum time interval between transactions (hours).
        /// </summary>
        public double MaxTransactionTime { get; set; }

        /// <summary>
        /// Minimal time interval between transactions (hours).
        /// </summary>
        public double MinTransactionTime { get; set; }

        /// <summary>
        /// Time since last transaction (months).
        /// </summary>
        public int TimeFromLastTransaction { get; set; }

        /// <summary>
        /// Average transaction per months (number).
        /// </summary>
        public double TransactionsPerMonth { get; }

        /// <summary>
        /// Last month transactions (number).
        /// </summary>
        public int LastMonthTransactions { get; set; }

        /// <summary>
        /// Last year transactions on wallet (number).
        /// </summary>
        public int LastYearTransactions { get; set; }

        /// <summary>
        /// Calculate wallet transaction stats score.
        /// </summary>
        /// <param name="chainId">Blockchain id.</param>
        /// <param name="calculationModel">Scoring calculation model.</param>
        /// <returns>Returns wallet transaction stats score.</returns>
        public new double CalculateScore(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            double result = TotalTransactionsScore(chainId, TotalTransactions, calculationModel) / 100 * TotalTransactionsPercents(chainId, calculationModel);

            double lastMonth = 0.0;
            lastMonth += TransactionsPerMonthScore(chainId, TransactionsPerMonth, calculationModel) / 100 * TransactionsPerMonthPercents(chainId, calculationModel);
            lastMonth += TransactionsLastMonthScore(chainId, LastMonthTransactions, calculationModel) / 100 * TransactionsLastMonthPercents(chainId, calculationModel);
            result += lastMonth * LastMonthPercents(chainId, calculationModel);

            return result;
        }

        private static double TotalTransactionsPercents(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                    return 23.2 / 100;
                case ScoringCalculationModel.XDEFI:
                    return 24.01 / 100;
                case ScoringCalculationModel.Halo:
                    return 19.22 / 100;
                case ScoringCalculationModel.CommonV2:
                    return 3.72 / 100;
                case ScoringCalculationModel.CommonV1:
                default:
                    return 9.99 / 100;
            }
        }

        private static double TransactionsPerMonthPercents(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                    return 60.81 / 100;
                case ScoringCalculationModel.CommonV2:
                    return 65.56 / 100;
                case ScoringCalculationModel.CommonV1:
                default:
                    return 18.00 / 100;
            }
        }

        private static double TransactionsLastMonthPercents(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                    return 39.19 / 100;
                case ScoringCalculationModel.CommonV2:
                    return 34.44 / 100;
                case ScoringCalculationModel.CommonV1:
                default:
                    return 10.34 / 100;
            }
        }

        private static double LastMonthPercents(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                    return 4.99 / 100;
                case ScoringCalculationModel.XDEFI:
                    return 5.35 / 100;
                case ScoringCalculationModel.Halo:
                    return 2.14 / 100;
                case ScoringCalculationModel.CommonV2:
                    return 2.86 / 100;
                case ScoringCalculationModel.CommonV1:
                default:
                    return 5.7 / 100;
            }
        }

        private static double TotalTransactionsScore(
            ulong chainId,
            int transactions,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                    return transactions switch
                    {
                        <= 1 => 8.76,
                        <= 10 => 15.34,
                        <= 100 => 71.03,
                        <= 1000 => 93.49,
                        _ => 100
                    };
                case ScoringCalculationModel.CommonV2:
                    return transactions switch
                    {
                        <= 1 => 9.73,
                        <= 10 => 14.34,
                        <= 100 => 65.98,
                        <= 1000 => 96.96,
                        _ => 100
                    };
                case ScoringCalculationModel.CommonV1:
                default:
                    return transactions switch
                    {
                        <= 1 => 7.62,
                        <= 10 => 14.67,
                        <= 100 => 27.82,
                        <= 1000 => 54.93,
                        _ => 100
                    };
            }
        }

        private static double TransactionsPerMonthScore(
            ulong chainId,
            double value,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                case ScoringCalculationModel.CommonV2:
                    return value switch
                    {
                        <= 1 => 7.44,
                        <= 2 => 17.21,
                        <= 5 => 39.71,
                        <= 10 => 78.6,
                        _ => 100
                    };
                case ScoringCalculationModel.CommonV1:
                default:
                    return value switch
                    {
                        <= 0.5D => 4.59,
                        <= 1 => 10.62,
                        <= 2 => 24.49,
                        <= 5 => 55.38,
                        _ => 100
                    };
            }
        }

        private static double TransactionsLastMonthScore(
            ulong chainId,
            int value,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                case ScoringCalculationModel.CommonV2:
                    return value switch
                    {
                        <= 1 => 11.89,
                        <= 5 => 38.22,
                        <= 10 => 88.01,
                        _ => 100
                    };
                case ScoringCalculationModel.CommonV1:
                default:
                    return value switch
                    {
                        <= 1 => 4.59,
                        <= 10 => 10.62,
                        <= 50 => 24.49,
                        <= 100 => 55.38,
                        _ => 100
                    };
            }
        }
    }
}