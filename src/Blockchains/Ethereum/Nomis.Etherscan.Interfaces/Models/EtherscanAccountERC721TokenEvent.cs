﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanAccountERC721TokenEvent.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Blockchain.Abstractions.Contracts;

// ReSharper disable InconsistentNaming
namespace Nomis.Etherscan.Interfaces.Models
{
    /// <summary>
    /// Etherscan account ERC-721 token transfer event.
    /// </summary>
    public class EtherscanAccountERC721TokenEvent :
        INFTTokenTransfer,
        IEtherscanTransfer
    {
        /// <summary>
        /// Block number.
        /// </summary>
        [JsonPropertyName("blockNumber")]
        public string? BlockNumber { get; set; }

        /// <summary>
        /// Time stamp.
        /// </summary>
        [JsonPropertyName("timeStamp")]
        public string? TimeStamp { get; set; }

        /// <summary>
        /// Hash.
        /// </summary>
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        /// <summary>
        /// From address.
        /// </summary>
        [JsonPropertyName("from")]
        public string? From { get; set; }

        /// <summary>
        /// Contract address.
        /// </summary>
        [JsonPropertyName("contractAddress")]
        public string? ContractAddress { get; set; }

        /// <summary>
        /// To address.
        /// </summary>
        [JsonPropertyName("to")]
        public string? To { get; set; }

        /// <summary>
        /// Token identifier.
        /// </summary>
        [JsonPropertyName("TokenID")]
        public string? TokenId { get; set; }

        /// <summary>
        /// Token name.
        /// </summary>
        [JsonPropertyName("tokenName")]
        public string? TokenName { get; set; }

        /// <summary>
        /// Token symbol.
        /// </summary>
        [JsonPropertyName("tokenSymbol")]
        public string? TokenSymbol { get; set; }

        /// <summary>
        /// Token decimal.
        /// </summary>
        [JsonPropertyName("tokenDecimal")]
        public string? TokenDecimal { get; set; }

        /// <summary>
        /// Confirmations.
        /// </summary>
        [JsonPropertyName("confirmations")]
        public string? Confirmations { get; set; }
    }
}