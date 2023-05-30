// ------------------------------------------------------------------------------------------------------
// <copyright file="DeFiIssueData.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.DeFi.Interfaces.Models
{
    /// <summary>
    /// De.Fi issue data.
    /// </summary>
    public class DeFiIssueData
    {
        /// <summary>
        /// Id.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Registry id.
        /// </summary>
        [JsonPropertyName("registryId")]
        public string? RegistryId { get; set; }

        /// <summary>
        /// Impact.
        /// </summary>
        [JsonPropertyName("impact")]
        public string? Impact { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Category.
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }
    }
}