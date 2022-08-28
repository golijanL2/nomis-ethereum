using Nomis.Web.Client.Common.Settings;

namespace Nomis.Web.Client.Ethereum.Extensions
{
    /// <summary>
    /// <see cref="IConfigurationBuilder"/> extension methods.
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        private const string ConfigsDirectory = "Configs";

        /// <summary>
        /// Add configuration from json.
        /// </summary>
        /// <param name="manager"><see cref="IConfigurationBuilder"/>.</param>
        /// <returns>Returns <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddJsonConfigs(this IConfigurationBuilder manager)
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            return manager
                .AddJsonFile(Path.Combine(ConfigsDirectory, $"{nameof(WebApiSettings).ToLower()}.json"), false, true)
                .AddJsonFile(Path.Combine(ConfigsDirectory, $"{nameof(WebApiSettings).ToLower()}.{env}.json"), true, true);
        }
    }
}