﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="EtherscanAccountInternalTransaction.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Etherscan.Interfaces.Models
{
    /// <summary>
    /// Etherscan account internal transaction.
    /// </summary>
    public class EtherscanAccountInternalTransaction :
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
        /// To address.
        /// </summary>
        [JsonPropertyName("to")]
        public string? To { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        /// <summary>
        /// Type.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Is error.
        /// </summary>
        [JsonPropertyName("isError")]
        public string? IsError { get; set; }

        /// <summary>
        /// Error code.
        /// </summary>
        [JsonPropertyName("errCode")]
        public string? ErrCode { get; set; }
    }
}