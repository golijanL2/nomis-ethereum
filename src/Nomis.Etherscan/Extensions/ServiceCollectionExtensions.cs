using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Interfaces.Settings;
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
        /// <param name="configuration"><see cref="IConfiguration"/>.</param>
        /// <returns>Returns <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddEtherscanService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSettings<EtherscanSettings>(configuration);

            return services
                .AddTransientInfrastructureService<IEtherscanService, EtherscanService>();
        }
    }
}