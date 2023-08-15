// ------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Nomis">
// Copyright (c) Nomis, 2023. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nomis.Blockchain.Abstractions.Extensions;
using Nomis.DexProviderService.Interfaces;
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Settings;
using Nomis.Utils.Contracts;
using Nomis.Utils.Extensions;

namespace Nomis.Etherscan.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Etherscan service.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <returns>Returns <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddEtherscanService(
            this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            services.CheckServiceDependencies(typeof(EtherscanService), typeof(IDexProviderService));
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            services.AddSettings<EtherscanSettings>(configuration);
            var settings = configuration.GetSettings<EtherscanSettings>();
            services
                .AddSingleton<IValuePool<EtherscanService, string>>(_ => new ValuePool<EtherscanService, string>(settings.ApiKeys));
            services
                .AddHttpClient<EtherscanClient>(client =>
                {
                    client.BaseAddress = new(settings.ApiBaseUrl ?? "https://api.etherscan.io/");
                })
                .AddTraceLogHandler(_ => Task.FromResult(settings.UseHttpClientLogging));
            return services
                .AddTransient<IEtherscanClient, EtherscanClient>(provider =>
                {
                    var apiKeysPool = provider.GetRequiredService<IValuePool<EtherscanService, string>>();
                    var logger = provider.GetRequiredService<ILogger<EtherscanClient>>();
                    var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(EtherscanClient));
                    return new EtherscanClient(settings, apiKeysPool, client, logger);
                })
                .AddTransientInfrastructureService<IEthereumScoringService, EtherscanService>();
        }
    }
}