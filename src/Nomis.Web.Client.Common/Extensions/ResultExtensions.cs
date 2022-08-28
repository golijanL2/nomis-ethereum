using System.Text.Json;
using System.Text.Json.Serialization;

using Nomis.Utils.Wrapper;

namespace Nomis.Web.Client.Common.Extensions
{
    /// <summary>
    /// Методы расширения для <see cref="HttpResponseMessage"/>.
    /// </summary>
    /// <remarks>
    /// Для получения различных результатов выполнения операций.
    /// </remarks>
    public static class ResultExtensions
    {
        /// <summary>
        /// To <see cref="IResult{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">The return data type.</typeparam>
        /// <param name="response"><see cref="HttpResponseMessage"/>.</param>
        /// <returns>Returns <see cref="IResult{TResult}"/>.</returns>
        public static async Task<IResult<TResult>> ToResultAsync<TResult>(this HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Result<TResult>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject!;
        }

        /// <summary>
        /// To <see cref="IResult"/>.
        /// </summary>
        /// <param name="response"><see cref="HttpResponseMessage"/>.</param>
        /// <returns>Returns <see cref="IResult"/>.</returns>
        public static async Task<IResult> ToResultAsync(this HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject!;
        }

        /// <summary>
        /// To <see cref="PaginatedResult{T}"/>.
        /// </summary>
        /// <typeparam name="TResult">The return data type.</typeparam>
        /// <param name="response"><see cref="HttpResponseMessage"/>.</param>
        /// <returns>Возвращает <see cref="PaginatedResult{T}"/>.</returns>
        public static async Task<PaginatedResult<TResult>> ToPaginatedResultAsync<TResult>(this HttpResponseMessage response)
        {
            string responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<PaginatedResult<TResult>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseObject!;
        }
    }
}