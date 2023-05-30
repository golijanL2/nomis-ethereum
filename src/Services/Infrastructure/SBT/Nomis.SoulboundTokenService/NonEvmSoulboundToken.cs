// ------------------------------------------------------------------------------------------------------
// <copyright file="NonEvmSoulboundToken.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.SoulboundTokenService.Extensions;
using Nomis.SoulboundTokenService.Interfaces;
using Nomis.Utils.Contracts.Services;

namespace Nomis.SoulboundTokenService
{
    /// <summary>
    /// Soulbound token registrar.
    /// </summary>
    /// <remarks>
    /// Is not EVM-compatible.
    /// </remarks>
    public sealed class NonEvmSoulboundToken :
        INonEvmSoulboundTokenServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddNonEvmSoulboundTokenService();
        }

        /// <inheritdoc/>
        public IInfrastructureService GetService(
            IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<INonEvmSoulboundTokenService>();
        }
    }
}