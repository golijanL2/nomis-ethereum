namespace Nomis.Web.Client.Common.Helpers
{
    /// <summary>
    /// Html helpers.
    /// </summary>
    public static class HtmlHelpers
    {
        /// <summary>
        /// TODO.
        /// </summary>
        /// <param name="value">TODO.</param>
        /// <param name="num">TODO.</param>
        /// <returns>TODO.</returns>
        public static string Round(this decimal value, int num = 2)
        {
            return value.ToString("F" + num);
        }

        /// <summary>
        /// TODO.
        /// </summary>
        /// <param name="value">TODO.</param>
        /// <param name="num">TODO.</param>
        /// <returns>TODO.</returns>
        public static string Round(this double value, int num = 2)
        {
            return value.ToString("F" + num);
        }
    }
}