// ------------------------------------------------------------------------------------------------------
// <copyright file="ApiCommonSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.Api.Common.Settings
{
    /// <summary>
    /// API common settings.
    /// </summary>
    public class ApiCommonSettings :
        ISettings
    {
        /// <summary>
        /// Use Swagger caching.
        /// </summary>
        public bool UseSwaggerCaching { get; init; }
    }
}