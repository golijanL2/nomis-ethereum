// ------------------------------------------------------------------------------------------------------
// <copyright file="ISoulboundTokenService.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.SoulboundTokenService.Interfaces.Models;
using Nomis.SoulboundTokenService.Interfaces.Requests;
using Nomis.Utils.Wrapper;

namespace Nomis.SoulboundTokenService.Interfaces.Contracts
{
    /// <summary>
    /// Soulbound token service.
    /// </summary>
    public interface ISoulboundTokenService
    {
        /// <summary>
        /// Get the soulbound token signature.
        /// </summary>
        /// <param name="request">The soulbound token request.</param>
        /// <returns>Return the soulbound token signature.</returns>
        public Result<SoulboundTokenSignature> GetSoulboundTokenSignature(
            SoulboundTokenRequest request);

        /// <summary>
        /// Get the soulbound token image.
        /// </summary>
        /// <param name="request">The soulbound token image request.</param>
        /// <returns>Return the soulbound token image.</returns>
        public Task<Result<SoulboundTokenImage>> GetSoulboundTokenImageAsync(
            SoulboundTokenImageRequest request);

        /// <summary>
        /// Get the soulbound token metadata.
        /// </summary>
        /// <param name="request">The soulbound token metadata request.</param>
        /// <returns>Return the soulbound token metadata.</returns>
        public Task<Result<SoulboundTokenMetadata>> GetSoulboundTokenMetadataAsync(
            SoulboundTokenMetadataRequest request);

        /// <summary>
        /// Get scoring calculation models.
        /// </summary>
        /// <returns>Returns scoring calculation models.</returns>
        public Task<Result<IList<ScoringCalculationModelData>>> GetScoringCalculationModelsAsync();
    }
}