﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumWalletScore.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.Blockchain.Abstractions;
using Nomis.Blockchain.Abstractions.Contracts;
using Nomis.PolygonId.Interfaces.Contracts;
using Nomis.Utils.Enums;

namespace Nomis.Etherscan.Interfaces.Models
{
    /// <summary>
    /// Ethereum wallet score.
    /// </summary>
    public class EthereumWalletScore :
        IWalletScore<EthereumWalletStats, EthereumTransactionIntervalData>
    {
        /// <summary>
        /// Wallet address.
        /// </summary>
        public string? Address { get; init; }

        /// <summary>
        /// Nomis Score in range of [0; 1].
        /// </summary>
        public double Score { get; init; }

        /// <summary>
        /// Score type.
        /// </summary>
        public ScoreType ScoreType => ScoreType.Finance;

        /// <summary>
        /// Mint data.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MintData? MintData { get; init; }

        /// <summary>
        /// DID data.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DIDData? DIDData { get; init; }

        /// <summary>
        /// Additional stat data used in score calculations.
        /// </summary>
        public EthereumWalletStats? Stats { get; init; }
    }
}