// ------------------------------------------------------------------------------------------------------
// <copyright file="SoulboundTokenSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.SoulboundTokenService.Models;
using Nomis.Utils.Contracts.Common;
using Nomis.Utils.Enums;

namespace Nomis.SoulboundTokenService.Settings
{
    /// <summary>
    /// Soulbound token settings.
    /// </summary>
    public class SoulboundTokenSettings :
        ISettings
    {
        /// <summary>
        /// Token image API base URL.
        /// </summary>
        public string? TokenImageApiBaseUrl { get; set; }

        /// <summary>
        /// Metadata token name.
        /// </summary>
        public string? MetadataTokenName { get; set; }

        /// <summary>
        /// Metadata token description.
        /// </summary>
        public string? MetadataTokenDescription { get; set; }

        /// <summary>
        /// Metadata token external URL.
        /// </summary>
        public string? MetadataTokenExternalUrl { get; set; }

        /// <summary>
        /// Token data by score type.
        /// </summary>
        public IDictionary<ScoreType, IDictionary<ulong, SoulboundTokenData>> TokenData { get; set; } = new Dictionary<ScoreType, IDictionary<ulong, SoulboundTokenData>>();
    }
}