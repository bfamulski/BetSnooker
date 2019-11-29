using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BetSnooker.HttpHelper
{
    public interface IAsyncRestClient
    {
        Task<Response<T>> Get<T>(string uri);
        
        Task<Response<T>> Send<T>(HttpRequestMessage request);

        Uri BaseAddress { get; set; }
    }

    /// Send a HTTP request and retrieve the response.
    /// <example>
    /// var restClient = new AsyncRestClient("http://someserverurl");
    /// var response = restClient.Get<MyResponseObject>("api/myObject/all");
    /// </example>
    /// AsyncRestClient uses the standard .NET HttpClient under the hood.
    public class AsyncRestClient : IAsyncRestClient, IDisposable
    {
        private readonly HttpClient _client;

        public AsyncRestClient()
        {
            _client = new HttpClient(ProvideClientHandler());
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        }

        public Uri BaseAddress
        {
            get => _client.BaseAddress;
            set => _client.BaseAddress = value;
        }

        public async Task<Response<T>> Get<T>(string uri)
        {
            return await Send<T>(new HttpRequestMessage(HttpMethod.Get, uri));
        }

        public async Task<Response<T>> Send<T>(HttpRequestMessage request)
        {
            HttpResponseMessage response;

            try
            {
                response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                return new Response<T>(false, HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                return new Response<T>(false, FormatErrorMessage(ex));
            }

            try
            {
                if (response == null)
                {
                    return new Response<T>(false, "Response is <NULL>.", HttpStatusCode.NoContent);
                }

                if (response.IsSuccessStatusCode)
                {
                    T result = default;
                    if (response.Content?.Headers?.ContentType?.MediaType == "application/octet-stream")
                    {
                        var resultStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        using (var memStream = new MemoryStream())
                        {
                            resultStream.CopyTo(memStream);
                            if (typeof(T) == typeof(byte[]))
                            {
                                result = (T)(memStream.ToArray() as object);
                            }
                        }
                    }
                    else
                    {
                        result = DeserializeJsonFromStream<T>(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    }

                    return new Response<T>(true, response.StatusCode, result);
                }
                else
                {
                    var errorMessage = await StreamToStringAsync(await response.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    return new Response<T>(false, errorMessage, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return new Response<T>(false, FormatErrorMessage(ex));
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _client.Dispose();
        }

        private T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
            {
                return default;
            }

            using (var streamReader = new StreamReader(stream))
            using (var textReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer(); // { DateTimeZoneHandling = _dateTimeZoneHandling };
                return serializer.Deserialize<T>(textReader);
            }
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            if (stream == null)
            {
                return null;
            }

            using (var streamReader = new StreamReader(stream))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        private HttpClientHandler ProvideClientHandler()
        {
            return new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseDefaultCredentials = false
            };
        }

        private static string FormatErrorMessage(Exception exception)
        {
            if (exception.InnerException != null)
            {
                return exception.Message + " " + exception.InnerException.Message;
            }

            return exception.Message;
        }
    }
}
