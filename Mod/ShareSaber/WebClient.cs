using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ShareSaber
{
    internal class WebResponse
    {
        public readonly HttpStatusCode StatusCode;
        public readonly string ReasonPhrase;
        public readonly HttpResponseHeaders Headers;
        public readonly HttpRequestMessage RequestMessage;
        public readonly bool IsSuccessStatusCode;

        private readonly byte[] _content;

        internal WebResponse(HttpResponseMessage resp, byte[] content)
        {
            StatusCode = resp.StatusCode;
            ReasonPhrase = resp.ReasonPhrase;
            Headers = resp.Headers;
            RequestMessage = resp.RequestMessage;
            IsSuccessStatusCode = resp.IsSuccessStatusCode;

            _content = content;
        }

        public byte[] ContentToBytes() => _content;
        public string ContentToString() => Encoding.UTF8.GetString(_content);
        public T ContentToJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(ContentToString());
        }
        public JObject ConvertToJObject()
        {
            return JObject.Parse(ContentToString());
        }
    }

    internal class WebClient
    {
        private readonly HttpClient _client;

        internal WebClient()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.UserAgent.TryParseAdd($"ShareSaber/{Plugin.Version}");
        }

        ~WebClient()
        {
            Dispose();
        }

        internal void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }

        internal async Task<WebResponse> SendAsync(HttpMethod methodType, string url, CancellationToken token, object postData = null, AuthenticationHeaderValue authHeader = null, IProgress<double> progress = null)
        {
            // create new request messsage
            var req = new HttpRequestMessage(methodType, url);

            // add authorization header
            req.Headers.Authorization = authHeader;

            // add json content, if provided
            if (methodType == HttpMethod.Post && postData != null)
            {
                req.Content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
            }

            // send request
            var resp = await _client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);

            if (token.IsCancellationRequested) throw new TaskCanceledException();

            using (var memoryStream = new MemoryStream())
            using (var stream = await resp.Content.ReadAsStreamAsync())
            {
                var buffer = new byte[8192];
                var bytesRead = 0; ;

                long? contentLength = resp.Content.Headers.ContentLength;
                var totalRead = 0;

                // send report
                progress?.Report(0);

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    if (token.IsCancellationRequested) throw new TaskCanceledException();

                    if (contentLength != null)
                    {
                        progress?.Report((double)totalRead / (double)contentLength);
                    }

                    await memoryStream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;
                }

                progress?.Report(1);
                byte[] bytes = memoryStream.ToArray();

                return new WebResponse(resp, bytes);
            }
        }
    }
}
