// ------------------------------------------------------------------------------------------------------
// <copyright file="IWalletCounterpartiesStats.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts;
using Nomis.Utils.Contracts.Stats;
using Nomis.Utils.Enums;

namespace Nomis.Blockchain.Abstractions.Stats
{
    /// <summary>
    /// Wallet counterparties stats.
    /// </summary>
    public interface IWalletCounterpartiesStats :
        IWalletStats
    {
        /// <summary>
        /// Set wallet counterparties stats.
        /// </summary>
        /// <typeparam name="TWalletStats">The wallet stats type.</typeparam>
        /// <param name="stats">The wallet stats.</param>
        /// <returns>Returns wallet stats with initialized properties.</returns>
        public new TWalletStats FillStatsTo<TWalletStats>(TWalletStats stats)
            where TWalletStats : class, IWalletCounterpartiesStats
        {
            stats.CounterpartiesData = CounterpartiesData;
            return stats;
        }

        /// <summary>
        /// Counterparties data.
        /// </summary>
        public IEnumerable<ExtendedCounterpartyData>? CounterpartiesData { get; set; }

        // TODO - add more properties

        /// <summary>
        /// Calculate wallet token balance stats score.
        /// </summary>
        /// <param name="chainId">Blockchain id.</param>
        /// <param name="calculationModel">Scoring calculation model.</param>
        /// <returns>Returns wallet token balance stats score.</returns>
        public new double CalculateScore(
            ulong chainId,
            ScoringCalculationModel calculationModel)
        {
            // TODO - add calculation
            return 0;
        }
    }
}