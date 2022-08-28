using Nomis.Utils.Contracts.Common;

namespace Nomis.Web.Client.Common.Settings
{
    /// <summary>
    /// Web API settings.
    /// </summary>
    public class WebApiSettings :
        ISettings
    {
        /// <summary>
        /// Nomis API base URL.
        /// </summary>
        public string? ApiBaseUrl { get; set; }
    }
}