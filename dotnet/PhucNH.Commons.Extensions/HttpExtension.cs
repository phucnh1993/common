using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace PhucNH.Commons.Extensions
{
    /// <summary>
    /// Extension for call API.
    /// </summary>
    public static class HttpExtension
    {
        /// <summary>
        /// Method check a dictionary parameters is empty.
        /// </summary>
        /// <param name="dictionary">Dictionary want to check.</param>
        /// <returns>Result checked.</returns>
        public static bool IsNotEmpty(
            this Dictionary<string, string> dictionary)
        {
            return (dictionary != null && dictionary.Any());
        }

        /// <summary>
        /// Build uri of request API.
        /// </summary>
        /// <param name="url">URL of request.</param>
        /// <param name="queries">Queries of request.</param>
        /// <returns>A uri.</returns>
        public static async Task<Uri> BuildUriAsync(
            this string url,
            Dictionary<string, string> queries = null)
        {
            return await Task.Run(() =>
            {
                var uri = new Uri(url);
                if (queries.IsNotEmpty())
                {
                    var listQuery = new List<string>();
                    foreach (var query in queries)
                    {
                        listQuery.Add(
                            $"{HttpUtility.UrlEncode(query.Key)}={HttpUtility.UrlEncode(query.Value)}");
                    }
                    uri = new Uri($"{url}?{string.Join("&", listQuery)}");
                }
                return uri;
            });
        }

        /// <summary>
        /// Build content of request API.
        /// </summary>
        /// <param name="body">Body of request.</param>
        /// <param name="formDatas">Form data of request.</param>
        /// <typeparam name="TBody">DAta type of body.</typeparam>
        /// <returns>A multipart content.</returns>
        public static async Task<MultipartContent> BuildContentAsync<TBody>(
            TBody body = default(TBody),
            Dictionary<string, string> formDatas = null)
        {
            return await Task.Run(() =>
            {
                var content = new MultipartContent();
                if (formDatas.IsNotEmpty())
                {
                    var keyPairs = new List<KeyValuePair<string, string>>();
                    foreach (var formData in formDatas)
                    {
                        keyPairs.Add(
                            new KeyValuePair<string, string>(
                                formData.Key,
                                formData.Value));
                    }
                    content.Add(new FormUrlEncodedContent(keyPairs));
                }
                if (body != null)
                {
                    content.Add(JsonContent.Create(body));
                }
                return content;
            });
        }

        /// <summary>
        /// Call API by info.
        /// </summary>
        /// <param name="client">Http client for call API.</param>
        /// <param name="method">Method of API.</param>
        /// <param name="url">Url of API.</param>
        /// <param name="headers">Header parameters of API.</param>
        /// <param name="queries">Queriy parameters of API.</param>
        /// <param name="formDatas">Form data parameters of API.</param>
        /// <param name="body">Body of API.</param>
        /// <typeparam name="TResult">Data type of result from API.</typeparam>
        /// <typeparam name="TBody">Data type of body.</typeparam>
        /// <returns>A object result.</returns>
        public static async Task<(HttpStatusCode, TResult)> CallApiAsync<TResult, TBody>(
            this HttpClient client,
            HttpMethod method,
            string url,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queries = null,
            Dictionary<string, string> formDatas = null,
            TBody body = default(TBody)) where TResult : class
        {
            TResult result = Activator.CreateInstance<TResult>();
            try
            {
                var uri = await url.BuildUriAsync(queries);
                using (var request = new HttpRequestMessage(method, uri))
                {
                    if (headers.IsNotEmpty())
                    {
                        foreach (var header in headers)
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                    }
                    request.Content = await BuildContentAsync(body, formDatas);
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseStream = await response.Content.ReadAsStreamAsync();
                        if (responseStream != null)
                        {
                            result = await JsonSerializer.DeserializeAsync<TResult>(responseStream) ??
                                Activator.CreateInstance<TResult>();
                            return (response.StatusCode, result);
                        }
                    }
                    return (response.StatusCode, result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(nameof(CallApiAsync), ex);
            }
        }
    }
}