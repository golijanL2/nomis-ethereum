// ------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedCounterpartyData.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming
namespace Nomis.Blockchain.Abstractions.Contracts
{
    /// <summary>
    /// Extended counterparty data.
    /// </summary>
    public sealed class ExtendedCounterpartyData :
        CounterpartyData
    {
        /// <summary>
        /// Initialize <see cref="ExtendedCounterpartyData"/>.
        /// </summary>
        /// <param name="counterpartyData"><see cref="CounterpartyData"/>.</param>
        public ExtendedCounterpartyData(
            CounterpartyData counterpartyData)
        {
            UseCounterparty = counterpartyData.UseCounterparty;
            Methods = counterpartyData.Methods;
            IsONFT = counterpartyData.IsONFT;
            Name = counterpartyData.Name;
            ContractName = counterpartyData.ContractName;
            Description = counterpartyData.Description;
            ContractAddress = counterpartyData.ContractAddress;
        }

        /// <inheritdoc />
        [JsonIgnore]
        public override bool UseCounterparty { get; init; }

        /// <inheritdoc />
        [JsonIgnore]
        public override IList<string> Methods { get; init; }

        /// <summary>
        /// The counterparty transactions.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CounterpartyTransactions { get; init; }

        /// <summary>
        /// The counterparty transfers.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CounterpartyTransfers { get; init; }

        /// <summary>
        /// Counterparty transaction hashes.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IList<string>? CounterpartyTransactionHashes { get; init; }

        /// <summary>
        /// Counterparty transfer balances.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IList<TokenDataBalance>? CounterpartyTransferBalances { get; init; }

        /// <summary>
        /// Counterparty turnover in USD.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? CounterpartyTurnoverUSD { get; init; }

        /// <summary>
        /// NFT token transfers data.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IList<INFTTokenTransfer>? CounterpartyNFTTransfers { get; init; }
    }
}