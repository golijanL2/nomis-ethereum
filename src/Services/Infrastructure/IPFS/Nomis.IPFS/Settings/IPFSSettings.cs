// ------------------------------------------------------------------------------------------------------
// <copyright file="IPFSSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.IPFS.Enums;
using Nomis.IPFS.Interfaces.Settings;
using Nomis.Utils.Contracts.Common;

// ReSharper disable InconsistentNaming
namespace Nomis.IPFS.Settings
{
    /// <summary>
    /// IPFS settings.
    /// </summary>
    public class IPFSSettings :
        IIPFSSettings,
        ISettings
    {
        /// <inheritdoc cref="IPFSProvider"/>
        public IPFSProvider Provider { get; set; } = IPFSProvider.LocalNode;

        /// <summary>
        /// API key.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// API base URL.
        /// </summary>
        public string? ApiBaseUrl { get; set; }

        /// <inheritdoc />
        public string? IpfsGatewayUrlTemplate { get; set; }
    }
}