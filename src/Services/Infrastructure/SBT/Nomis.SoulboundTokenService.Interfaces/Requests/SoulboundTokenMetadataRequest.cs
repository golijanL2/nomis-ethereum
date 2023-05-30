// ------------------------------------------------------------------------------------------------------
// <copyright file="SoulboundTokenMetadataRequest.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.SoulboundTokenService.Interfaces.Models;

namespace Nomis.SoulboundTokenService.Interfaces.Requests
{
    /// <summary>
    /// Soulbound token metadata request.
    /// </summary>
    public class SoulboundTokenMetadataRequest
    {
        /// <summary>
        /// Image.
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// Attributes.
        /// </summary>
        public IList<SoulboundTokenTrait> Attributes { get; set; } = new List<SoulboundTokenTrait>();
    }
}