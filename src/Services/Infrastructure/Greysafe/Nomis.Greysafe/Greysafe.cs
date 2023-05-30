﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="Greysafe.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Nomis.Greysafe.Extensions;
using Nomis.Greysafe.Interfaces;
using Nomis.Utils.Contracts.Services;

namespace Nomis.Greysafe
{
    /// <summary>
    /// Greysafe service registrar.
    /// </summary>
    public sealed class Greysafe :
        IGreysafeServiceRegistrar
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterService(
            IServiceCollection services)
        {
            return services
                .AddGreysafeService();
        }

        /// <inheritdoc/>
        public IInfrastructureService GetService(
            IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IGreysafeService>();
        }
    }
}