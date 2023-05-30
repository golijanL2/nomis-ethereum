// ------------------------------------------------------------------------------------------------------
// <copyright file="SoulboundTokenCommonData.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

namespace Nomis.SoulboundTokenService.Interfaces.Contracts
{
    /// <summary>
    /// Soulbound token common data.
    /// </summary>
    public class SoulboundTokenCommonData
    {
        /// <summary>
        /// Soulbound token contract address.
        /// </summary>
        /// <example>0x0000000000000000000000000000000000000000</example>
        public string? ContractAddress { get; set; }

        /// <summary>
        /// Token name.
        /// </summary>
        /// <remarks>
        /// "NMSS" value by default.
        /// </remarks>
        /// <example>NMSS</example>
        public string? TokenName { get; set; } = "NMSS";

        /// <summary>
        /// Token version.
        /// </summary>
        /// <remarks>
        /// "0.1" value by default.
        /// </remarks>
        /// <example>0.1</example>
        public string? Version { get; set; } = "0.1";
    }
}