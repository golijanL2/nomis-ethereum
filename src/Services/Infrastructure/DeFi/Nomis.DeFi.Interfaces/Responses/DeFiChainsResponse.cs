// ------------------------------------------------------------------------------------------------------
// <copyright file="DeFiChainsResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.DeFi.Interfaces.Models;

namespace Nomis.DeFi.Interfaces.Responses
{
    /// <summary>
    /// De.Fi chains response.
    /// </summary>
    public class DeFiChainsResponse
    {
        /// <summary>
        /// The collection of chains.
        /// </summary>
        [JsonPropertyName("chains")]
        public IList<DeFiChainData> Chains { get; set; } = new List<DeFiChainData>();
    }
}