// ------------------------------------------------------------------------------------------------------
// <copyright file="IRateLimitSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

namespace Nomis.Blockchain.Abstractions.Contracts
{
    /// <summary>
    /// Rate limit settings.
    /// </summary>
    public interface IRateLimitSettings
    {
        /// <summary>
        /// Max API calls per second.
        /// </summary>
        public uint MaxApiCallsPerSecond { get; init; }

        /// <summary>
        /// Last API call date and time.
        /// </summary>
        public ThreadLocal<DateTime> LastApiCall { get; }
    }
}