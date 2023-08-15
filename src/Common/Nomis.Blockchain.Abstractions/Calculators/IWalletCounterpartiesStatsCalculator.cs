// ------------------------------------------------------------------------------------------------------
// <copyright file="IWalletCounterpartiesStatsCalculator.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts;
using Nomis.Blockchain.Abstractions.Stats;
using Nomis.Utils.Contracts;
using Nomis.Utils.Contracts.Calculators;

namespace Nomis.Blockchain.Abstractions.Calculators
{
    /// <summary>
    /// Blockchain wallet counterparties stats calculator.
    /// </summary>
    /// <typeparam name="TWalletStats">The wallet stats type.</typeparam>
    /// <typeparam name="TTransactionIntervalData">The transaction interval data type.</typeparam>
    // ReSharper disable once InconsistentNaming
    public interface IWalletCounterpartiesStatsCalculator<TWalletStats, TTransactionIntervalData> :
        IWalletStatsCalculator<TWalletStats, TTransactionIntervalData>
        where TWalletStats : class, IWalletCounterpartiesStats, new()
        where TTransactionIntervalData : class, ITransactionIntervalData, new()
    {
        /// <inheritdoc cref="IWalletCounterpartiesStats.CounterpartiesData"/>
        public IEnumerable<ExtendedCounterpartyData>? CounterpartiesData { get; }

        /// <summary>
        /// Get blockchain wallet counterparties stats.
        /// </summary>
        public new IWalletCounterpartiesStats Stats()
        {
            return new TWalletStats
            {
                CounterpartiesData = CounterpartiesData
            };
        }

        /// <summary>
        /// Blockchain wallet counterparties stats filler.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        internal new Func<TWalletStats, TWalletStats> StatsFiller()
        {
            return stats => Stats().FillStatsTo(stats);
        }
    }
}