// ------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Net;

using Microsoft.Extensions.Logging;
using Nomis.Utils.Contracts.Requests;
using Nomis.Utils.Exceptions;
using Nomis.Utils.Wrapper;

namespace Nomis.ReferralService.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get wallet own referral code.
        /// </summary>
        /// <typeparam name="TWalletRequest">The wallet request type.</typeparam>
        /// <param name="referralService"><see cref="IReferralService"/>.</param>
        /// <param name="request"><see cref="WalletStatsRequest"/>.</param>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>Returns wallet own referral code.</returns>
        public static async Task<Result<string?>> GetOwnReferralCodeAsync<TWalletRequest>(
            this IReferralService referralService,
            TWalletRequest request,
            ILogger logger,
            CancellationToken cancellationToken = default)
            where TWalletRequest : WalletStatsRequest
        {
            var messages = new List<string>();

            string? ownReferralCode = null;
            var referralWalletResult = await referralService.GetOrCreateReferralWalletAsync(request.Address, cancellationToken).ConfigureAwait(false);
            messages.AddRange(referralWalletResult.Messages);
            if (referralWalletResult.Succeeded)
            {
                ownReferralCode = referralWalletResult.Data.ReferralCode;
            }

            if (!string.IsNullOrWhiteSpace(request.ReferrerCode) && !request.ReferrerCode.Equals("undefined", StringComparison.InvariantCultureIgnoreCase))
            {
                if (request.ReferrerCode.Equals(ownReferralCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.LogWarning("Try to add for wallet {Address} the own {ReferralCode} referral code.", request.Address, request.ReferrerCode);
                    throw new CustomException("You can’t use your own referral code.", statusCode: HttpStatusCode.BadRequest);
                }

                var referralWalletByCodeResult = await referralService.GetReferralWalletByReferralCodeAsync(request.ReferrerCode, cancellationToken).ConfigureAwait(false);
                messages.AddRange(referralWalletByCodeResult.Messages);
                if (referralWalletByCodeResult.Succeeded)
                {
                    var addReferralResult = await referralService.AddReferralAsync(request.Address, referralWalletByCodeResult.Data.WalletAddress, cancellationToken).ConfigureAwait(false);
                    messages.AddRange(addReferralResult.Messages);
                    logger.LogInformation("Added referral {Address} for the {ReferralCode} referral code.", request.Address, request.ReferrerCode);
                }
                else
                {
                    logger.LogWarning("Failed to add referral {Address} for the {ReferralCode} referral code.", request.Address, request.ReferrerCode);
                    request.ReferrerCode = null;
                }
            }

            if (string.IsNullOrWhiteSpace(request.ReferrerCode))
            {
                var referrerCodeResult = await referralService.GetReferrerCodeByReferralWalletAsync(request.Address, cancellationToken).ConfigureAwait(false);
                messages.AddRange(referrerCodeResult.Messages);
                if (referrerCodeResult.Succeeded)
                {
                    request.ReferrerCode = referrerCodeResult.Data;
                }
            }

            return await Result<string?>.SuccessAsync(ownReferralCode, messages).ConfigureAwait(false);
        }
    }
}