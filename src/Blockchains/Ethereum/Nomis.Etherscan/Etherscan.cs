// ------------------------------------------------------------------------------------------------------
// <copyright file="Etherscan.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Etherscan.Extensions;
using Nomis.Etherscan.Interfaces;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Etherscan
{
    /// <summary>
    /// Etherscan service registrar.
    /// </summary>
    public sealed class Etherscan :
        IEthereumServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddEtherscanService();
        }

        /// <inheritdoc/>
        public IInfrastructureService GetService(
            IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IEthereumScoringService>();
        }
    }
}