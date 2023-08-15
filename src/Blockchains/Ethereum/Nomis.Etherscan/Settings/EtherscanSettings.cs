// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Blockchain.Abstractions.Contracts;
using Nomis.Blockchain.Abstractions.Enums;
using Nomis.Utils.Enums;

namespace Nomis.Etherscan.Settings
{
    /// <summary>
    /// Etherscan settings.
    /// </summary>
    internal class EtherscanSettings :
        IBlockchainSettings,
        IRateLimitSettings,
        IGetFromCacheStatsSettings,
        IHttpClientLoggingSettings,
        IUseHistoricalMedianBalanceUSDSettings,
        IFilterCounterpartiesByCalculationModelSettings
    {
        /// <summary>
        /// API keys for Etherscan.
        /// </summary>
        public IList<string> ApiKeys { get; init; } = new List<string>();

        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.etherscan.io/getting-started/endpoint-urls"/>
        /// </remarks>
        public string? ApiBaseUrl { get; init; }

        /// <inheritdoc/>
        public uint MaxApiCallsPerSecond { get; init; }

        /// <inheritdoc/>
        public ThreadLocal<DateTime> LastApiCall { get; } = new();

        /// <summary>
        /// Blockchain provider URL.
        /// </summary>
        public string? BlockchainProviderUrl { get; init; }

        /// <inheritdoc cref="BlockchainDescriptor"/>
        public IDictionary<BlockchainKind, BlockchainDescriptor> BlockchainDescriptors { get; init; } = new Dictionary<BlockchainKind, BlockchainDescriptor>();

        /// <inheritdoc/>
        public bool GetFromCacheStatsIsEnabled { get; init; }

        /// <inheritdoc/>
        public TimeSpan GetFromCacheStatsTimeLimit { get; init; }

        /// <inheritdoc/>
        public bool UseHttpClientLogging { get; init; }

        /// <inheritdoc/>
        public IDictionary<ScoringCalculationModel, bool> UseHistoricalMedianBalanceUSD { get; init; } = new Dictionary<ScoringCalculationModel, bool>();

        /// <inheritdoc/>
        public decimal MedianBalancePrecision { get; init; }

        /// <inheritdoc/>
        public TimeSpan? MedianBalanceStartFrom { get; init; }

        /// <inheritdoc/>
        public int? MedianBalanceLastCount { get; init; }

        /// <inheritdoc/>
        public IDictionary<ScoringCalculationModel, List<CounterpartyData>> CounterpartiesFilterData { get; init; } =
            new Dictionary<ScoringCalculationModel, List<CounterpartyData>>();
    }
}