// ------------------------------------------------------------------------------------------------------
// <copyright file="DeFiShieldData.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.DeFi.Interfaces.Models
{
    /// <summary>
    /// De.Fi shield data.
    /// </summary>
    public class DeFiShieldData
    {
        /// <summary>
        /// Id.
        /// </summary>
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        /// <summary>
        /// Address.
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Logo.
        /// </summary>
        [JsonPropertyName("logo")]
        public string? Logo { get; set; }

        /// <summary>
        /// In progress.
        /// </summary>
        [JsonPropertyName("inProgress")]
        public bool InProgress { get; set; }

        /// <summary>
        /// Whitelisted.
        /// </summary>
        [JsonPropertyName("whitelisted")]
        public bool Whitelisted { get; set; }

        /// <summary>
        /// The dictionary of tags.
        /// </summary>
        [JsonPropertyName("tags")]
        public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The token data.
        /// </summary>
        [JsonPropertyName("token")]
        public DeFiTokenData? Token { get; set; }

        /// <summary>
        /// The collection of issues.
        /// </summary>
        [JsonPropertyName("issues")]
        public IList<DeFiIssueData> Issues { get; set; } = new List<DeFiIssueData>();
    }
}