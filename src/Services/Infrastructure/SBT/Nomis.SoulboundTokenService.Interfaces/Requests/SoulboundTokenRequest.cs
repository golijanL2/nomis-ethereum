// ------------------------------------------------------------------------------------------------------
// <copyright file="SoulboundTokenRequest.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.SoulboundTokenService.Interfaces.Contracts;
using Nomis.Utils.Enums;

namespace Nomis.SoulboundTokenService.Interfaces.Requests
{
    /// <summary>
    /// Soulbound token request.
    /// </summary>
    public class SoulboundTokenRequest
    {
        /// <summary>
        /// To address.
        /// </summary>
        /// <example>0x0000000000000000000000000000000000000000</example>
        public string? To { get; set; }

        /// <summary>
        /// The score type.
        /// </summary>
        /// <example>0</example>
        public ScoreType ScoreType { get; set; }

        /// <summary>
        /// Score value.
        /// </summary>
        /// <example>1414</example>
        public ushort Score { get; set; }

        /// <summary>
        /// Scoring calculation model.
        /// </summary>
        /// <example>0</example>
        public ScoringCalculationModel CalculationModel { get; set; }

        /// <summary>
        /// Nonce.
        /// </summary>
        /// <example>0</example>
        public ulong Nonce { get; set; }

        /// <summary>
        /// Blockchain id in which the score will be minted.
        /// </summary>
        /// <example>56</example>
        public ulong MintChainId { get; set; }

        /// <summary>
        /// Soulbound token common data.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public SoulboundTokenCommonData? SBTCommonData { get; set; }

        /// <summary>
        /// Time to the verifying deadline.
        /// </summary>
        /// <example>0</example>
        public ulong Deadline { get; set; }

        /// <summary>
        /// Token metadata IPFS URL.
        /// </summary>
        /// <example>null</example>
        public string? MetadataUrl { get; set; }

        /// <summary>
        /// Blockchain id in which the score was calculated.
        /// </summary>
        /// <example>56</example>
        public ulong? ScoreChainId { get; set; }
    }
}