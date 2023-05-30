// ------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.SoulboundTokenService.Interfaces.Contracts;
using Nomis.SoulboundTokenService.Interfaces.Models;
using Nomis.Utils.Contracts.Requests;
using Nomis.Utils.Enums;
using Nomis.Utils.Wrapper;

namespace Nomis.SoulboundTokenService.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get the signed data signature.
        /// </summary>
        /// <typeparam name="TWalletRequest">The wallet request type.</typeparam>
        /// <param name="service"><see cref="ISoulboundTokenService"/>.</param>
        /// <param name="request"><see cref="WalletStatsRequest"/>.</param>
        /// <param name="mintedScore">The wallet minted score.</param>
        /// <param name="mintChainId">Blockchain id in which the score will be minted.</param>
        /// <param name="sbtData">The soulbound token common data.</param>
        /// <param name="metadataUrl">Token metadata IPFS URL.</param>
        /// <param name="scoreChainId">Blockchain id in which the score was calculated.</param>
        /// <param name="criteria">Criteria to get signature.</param>
        /// <returns>Returns the signed data signature.</returns>
        public static async Task<Result<SoulboundTokenSignature>> SignatureAsync<TWalletRequest>(
            this ISoulboundTokenService service,
            TWalletRequest request,
            ushort mintedScore,
            ulong? mintChainId,
            IDictionary<ScoreType, SoulboundTokenCommonData>? sbtData,
            string? metadataUrl = null,
            ulong? scoreChainId = null,
            params bool?[] criteria)
            where TWalletRequest : WalletStatsRequest
        {
            if (!request.PrepareToMint)
            {
                return await Result<SoulboundTokenSignature>.FailAsync(
                    new SoulboundTokenSignature
                    {
                        Signature = null
                    }, $"Can't get token signature: {nameof(request.PrepareToMint)} parameter is false.").ConfigureAwait(false);
            }

            var signatureResult = await Result<SoulboundTokenSignature>.FailAsync(
                new SoulboundTokenSignature
                {
                    Signature = null
                }, "Get token signature: Can't get signature without Risk adjusting score or empty blockchain id.").ConfigureAwait(false);
            if (request.PrepareToMint && mintChainId != null && (!criteria.Any() || criteria.All(c => c == true)))
            {
                signatureResult = service.GetSoulboundTokenSignature(new()
                {
                    Score = mintedScore,
                    ScoreType = request.ScoreType,
                    CalculationModel = request.CalculationModel,
                    To = request.Address,
                    Nonce = request.Nonce,
                    MintChainId = (ulong)mintChainId,
                    SBTCommonData = sbtData?.TryGetValue(request.ScoreType, out var value) is true ? value : null,
                    Deadline = request.Deadline,
                    MetadataUrl = metadataUrl,
                    ScoreChainId = scoreChainId
                });
            }

            return signatureResult;
        }
    }
}