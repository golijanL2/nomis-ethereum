// ------------------------------------------------------------------------------------------------------
// <copyright file="MintData.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Enums;

namespace Nomis.Blockchain.Abstractions.Contracts
{
    /// <summary>
    /// Mint data.
    /// </summary>
    public sealed class MintData
    {
        /// <summary>
        /// Initialize <see cref="MintData"/>.
        /// </summary>
        /// <param name="signature">Soulbound token signature.</param>
        /// <param name="mintedScore">Nomis minted Score in range of [0; 10000].</param>
        /// <param name="calculationModel"><see cref="ScoringCalculationModel"/>.</param>
        /// <param name="deadline">Verifying deadline block timestamp.</param>
        /// <param name="metadataUrl">Token metadata IPFS URL.</param>
        /// <param name="chainId">The blockchain id in which the score was calculated.</param>
        /// <param name="mintedChain">The blockchain descriptor in which the score will be minted.</param>
        public MintData(
            string? signature,
            ushort mintedScore,
            ScoringCalculationModel calculationModel,
            ulong deadline,
            string? metadataUrl,
            ulong chainId,
            IBlockchainDescriptor mintedChain)
        {
            Signature = signature;
            MintedScore = mintedScore;
            CalculationModel = calculationModel;
            Deadline = deadline;
            MetadataUrl = metadataUrl;
            ChainId = chainId;
            MintedChain = mintedChain;
        }

        /// <summary>
        /// Soulbound token signature.
        /// </summary>
        public string? Signature { get; init; }

        /// <summary>
        /// Nomis minted Score in range of [0; 10000].
        /// </summary>
        public ushort MintedScore { get; init; }

        /// <inheritdoc cref="ScoringCalculationModel"/>
        public ScoringCalculationModel CalculationModel { get; init; }

        /// <summary>
        /// Verifying deadline block timestamp.
        /// </summary>
        public ulong Deadline { get; init; }

        /// <summary>
        /// Token metadata IPFS URL.
        /// </summary>
        public string? MetadataUrl { get; init; }

        /// <summary>
        /// The blockchain id in which the score was calculated.
        /// </summary>
        public ulong ChainId { get; init; }

        /// <summary>
        /// The blockchain descriptor in which the score will be minted.
        /// </summary>
        public IBlockchainDescriptor MintedChain { get; init; }
    }
}