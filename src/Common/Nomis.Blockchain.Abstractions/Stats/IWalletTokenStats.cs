﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="IWalletTokenStats.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Stats;
using Nomis.Utils.Enums;

namespace Nomis.Blockchain.Abstractions.Stats
{
    /// <summary>
    /// Wallet token stats.
    /// </summary>
    public interface IWalletTokenStats :
        IWalletStats
    {
        /// <summary>
        /// Set wallet token stats.
        /// </summary>
        /// <typeparam name="TWalletStats">The wallet stats type.</typeparam>
        /// <param name="stats">The wallet stats.</param>
        /// <returns>Returns wallet stats with initialized properties.</returns>
        public new TWalletStats FillStatsTo<TWalletStats>(TWalletStats stats)
            where TWalletStats : class, IWalletTokenStats
        {
            stats.TokensHolding = TokensHolding;
            return stats;
        }

        /// <summary>
        /// Value of all holding tokens (number).
        /// </summary>
        public int TokensHolding { get; set; }

        /// <summary>
        /// Calculate wallet token stats score.
        /// </summary>
        /// <param name="chainId">Blockchain id.</param>
        /// <param name="calculationModel">Scoring calculation model.</param>
        /// <returns>Returns wallet token stats score.</returns>
        public new double CalculateScore(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            double result = TokensHoldingScore(chainId, TokensHolding, calculationModel) / 100 * TokensHoldingPercents(chainId, calculationModel);

            return result;
        }

        private static double TokensHoldingPercents(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                    return 3.23 / 100;
                case ScoringCalculationModel.XDEFI:
                    return 17.86 / 100;
                case ScoringCalculationModel.Halo:
                    return 7.4 / 100;
                case ScoringCalculationModel.CommonV2:
                    return 14.07 / 100;
                case ScoringCalculationModel.CommonV1:
                default:
                    return 3.86 / 100;
            }
        }

        private static double TokensHoldingScore(
            ulong chainId,
            int tokens,
            ScoringCalculationModel calculationModel)
        {
            switch (calculationModel)
            {
                case ScoringCalculationModel.Symbiosis:
                case ScoringCalculationModel.XDEFI:
                case ScoringCalculationModel.Halo:
                case ScoringCalculationModel.CommonV2:
                    return tokens switch
                    {
                        <= 1 => 10.91,
                        <= 3 => 48.96,
                        <= 7 => 59.57,
                        <= 10 => 78.6,
                        _ => 100
                    };
                case ScoringCalculationModel.CommonV1:
                default:
                    return tokens switch
                    {
                        <= 1 => 7.7,
                        <= 5 => 14.86,
                        <= 10 => 34.49,
                        <= 100 => 65.98,
                        _ => 100
                    };
            }
        }
    }
}