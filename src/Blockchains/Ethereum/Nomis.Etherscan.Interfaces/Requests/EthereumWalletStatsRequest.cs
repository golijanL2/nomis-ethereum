﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumWalletStatsRequest.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Nomis.Aave.Interfaces.Contracts;
using Nomis.Blockchain.Abstractions.Requests;
using Nomis.Dex.Abstractions.Contracts;
using Nomis.HapiExplorer.Interfaces.Contracts;

namespace Nomis.Etherscan.Interfaces.Requests
{
    /// <summary>
    /// Request for getting the wallet stats for Ethereum.
    /// </summary>
    public sealed class EthereumWalletStatsRequest :
        BaseEvmWalletStatsRequest,
        IWalletTokensSwapPairsRequest,
        IWalletHapiProtocolRequest,
        IWalletAaveProtocolRequest
    {
        /// <inheritdoc />
        /// <example>false</example>
        [FromQuery]
        public bool GetTokensSwapPairs { get; set; } = false;

        /// <inheritdoc />
        /// <example>1000</example>
        [FromQuery]
        [Range(typeof(int), "1", "1000")]
        public int FirstSwapPairs { get; set; } = 1000;

        /// <inheritdoc />
        /// <example>0</example>
        [FromQuery]
        [Range(typeof(int), "0", "2147483647")]
        public int Skip { get; set; } = 0;

        /// <inheritdoc />
        /// <example>true</example>
        [FromQuery]
        public bool GetHapiProtocolData { get; set; } = true;

        /// <inheritdoc />
        /// <example>false</example>
        [FromQuery]
        public bool GetAaveProtocolData { get; set; } = false;
    }
}