// ------------------------------------------------------------------------------------------------------
// <copyright file="SoulboundTokenImage.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

namespace Nomis.SoulboundTokenService.Interfaces.Models
{
    /// <summary>
    /// Soulbound token image.
    /// </summary>
    public class SoulboundTokenImage
    {
        /// <summary>
        /// The image.
        /// </summary>
        public byte[]? Image { get; set; }

        /// <summary>
        /// The image type.
        /// </summary>
        public string? ImageType { get; set; }
    }
}