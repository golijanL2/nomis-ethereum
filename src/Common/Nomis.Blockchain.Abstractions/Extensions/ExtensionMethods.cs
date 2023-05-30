// ------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text;
using System.Text.Json;

using Ipfs.CoreApi;
using Nomis.IPFS.Interfaces;
using Nomis.IPFS.Interfaces.Requests;
using Nomis.SoulboundTokenService.Interfaces.Contracts;
using Nomis.SoulboundTokenService.Interfaces.Models;
using Nomis.SoulboundTokenService.Interfaces.Requests;
using Nomis.Utils.Contracts.Requests;
using Nomis.Utils.Extensions;
using Nomis.Utils.Wrapper;

namespace Nomis.Blockchain.Abstractions.Extensions
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get the token metadata IPFS URL.
        /// </summary>
        /// <typeparam name="TWalletRequest">The wallet request type.</typeparam>
        /// <param name="soulboundTokenService"><see cref="ISoulboundTokenService"/>.</param>
        /// <param name="ipfsService"><see cref="IIPFSService"/>.</param>
        /// <param name="request"><see cref="WalletStatsRequest"/>.</param>
        /// <param name="chainId">Blockchain id.</param>
        /// <param name="chainName">Blockchain name.</param>
        /// <param name="score">The wallet score.</param>
        /// <param name="additionalTraits">Additional traits.</param>
        /// <returns>Returns the token metadata IPFS URL.</returns>
        public static async Task<Result<string?>> TokenMetadataAsync<TWalletRequest>(
            this ISoulboundTokenService soulboundTokenService,
            IIPFSService ipfsService,
            TWalletRequest request,
            ulong chainId,
            string? chainName,
            double score,
            IList<SoulboundTokenTrait>? additionalTraits = default)
            where TWalletRequest : WalletStatsRequest
        {
            if (!request.PrepareToMint)
            {
                return await Result<string?>.FailAsync($"Can't get token metadata: {nameof(request.PrepareToMint)} parameter is false.").ConfigureAwait(false);
            }

            try
            {
                var tokenImageResult = await soulboundTokenService.GetSoulboundTokenImageAsync(new SoulboundTokenImageRequest
                {
                    Address = request.Address,
                    Score = (byte)(score * 100),
                    Type = request.CalculationModel.ToString(),
                    Size = 512,
                    ChainId = chainId
                }).ConfigureAwait(false);
                if (tokenImageResult.Succeeded)
                {
                    using var tokenImageStream = new MemoryStream(tokenImageResult.Data.Image!);
                    var uploadImageResult = await ipfsService.UploadFileAsync(new IPFSUploadFileRequest
                    {
                        FileContent = tokenImageStream,
                        FileName = $"{request.Address}_{chainId}_{request.CalculationModel.ToString()}_{request.ScoreType.ToString()}.png",
                        Options = new AddFileOptions
                        {
                            Pin = true
                        }
                    }).ConfigureAwait(false);
                    if (uploadImageResult.Succeeded)
                    {
                        var metadataResult = await soulboundTokenService.GetSoulboundTokenMetadataAsync(new SoulboundTokenMetadataRequest
                        {
                            Image = string.Format(ipfsService.Settings.IpfsGatewayUrlTemplate!, uploadImageResult.Data),
                            Attributes = new List<SoulboundTokenTrait>
                            {
                                new()
                                {
                                    TraitType = "Blockchain",
                                    Value = chainName
                                },
                                new()
                                {
                                    DisplayType = "number",
                                    TraitType = "Chain id",
                                    Value = chainId
                                },
                                new()
                                {
                                    DisplayType = "boost_percentage",
                                    TraitType = "Score",
                                    Value = score * 100
                                },
                                new()
                                {
                                    DisplayType = "number",
                                    TraitType = "Calculation model",
                                    Value = request.CalculationModel.ToString()
                                },
                                new()
                                {
                                    DisplayType = "number",
                                    TraitType = "Score type",
                                    Value = request.ScoreType.ToString()
                                },
                                new()
                                {
                                    DisplayType = "date",
                                    TraitType = "Timestamp",
                                    Value = DateTime.UtcNow.ConvertToTimestamp()
                                }
                            }.Union(additionalTraits ?? new List<SoulboundTokenTrait>()).ToList()
                        }).ConfigureAwait(false);
                        if (metadataResult.Succeeded)
                        {
                            using var tokenMetadataStream = new MemoryStream(JsonSerializer.Serialize(metadataResult.Data).ToByteArray(Encoding.UTF8));
                            var uploadMetadataResult = await ipfsService.UploadFileAsync(new IPFSUploadFileRequest
                            {
                                FileContent = tokenMetadataStream,
                                FileName = $"{request.Address}_{chainId}_{request.CalculationModel.ToString()}_{request.ScoreType.ToString()}.json",
                                Options = new AddFileOptions
                                {
                                    Pin = true
                                }
                            }).ConfigureAwait(false);
                            if (uploadMetadataResult.Succeeded)
                            {
                                return await Result<string?>.SuccessAsync(string.Format(ipfsService.Settings.IpfsGatewayUrlTemplate!, uploadMetadataResult.Data), "Successfully got token metadata URL.").ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return await Result<string?>.FailAsync(e.Message).ConfigureAwait(false);
            }

            return await Result<string?>.FailAsync("Cant get token metadata.").ConfigureAwait(false);
        }
    }
}