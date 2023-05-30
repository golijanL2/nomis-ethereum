// ------------------------------------------------------------------------------------------------------
// <copyright file="DeFiShieldsResponse.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Nomis.DeFi.Interfaces.Models;

namespace Nomis.DeFi.Interfaces.Responses
{
    /// <summary>
    /// De.Fi shields response.
    /// </summary>
    public class DeFiShieldsResponse
    {
        /// <summary>
        /// The collection of shields.
        /// </summary>
        [JsonPropertyName("shields")]
        public IList<DeFiShieldData> Shields { get; set; } = new List<DeFiShieldData>();
    }
}