using Eco.EM.Framework.Logging;
using Eco.Shared.Properties;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Eco.EM.Framework.WebHook;

namespace Eco.EM.Framework.Networking
{
    public static class Network
    {
        private static readonly HttpClient httpClient = new();
        private readonly static string jsonMediaType = "application/json";


        public static async Task<string> GetRequest(string URL)
        {
            try
            {
                httpClient.BaseAddress = new Uri(URL);
                var result = httpClient.GetStringAsync(httpClient.BaseAddress);
                return result.Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static async Task<string> PostRequestAsync(string URL, Dictionary<string, string> Parameters)
        {
            try
            {
                httpClient.BaseAddress = new Uri(URL);

                var PostData = string.Empty;

                foreach (var param in Parameters)
                {
                    if (!string.IsNullOrWhiteSpace(PostData))
                    {
                        PostData += "&";
                    }

                    PostData += $"{param.Key}={param.Value}";
                }

                StringContent strContent = new(PostData, Encoding.UTF8, jsonMediaType);
                HttpResponseMessage responseMessage = await httpClient.PostAsync(httpClient.BaseAddress, strContent).ConfigureAwait(false);
                return await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static async Task<string> PostRequest(string URL, string PostData)
        {
            try
            {
                httpClient.BaseAddress = new Uri(URL);
                LoggingUtils.Debug(PostData);

                StringContent strContent = new StringContent(PostData, Encoding.UTF8, jsonMediaType);
                HttpResponseMessage responseMessage = await httpClient.PostAsync(httpClient.BaseAddress, strContent).ConfigureAwait(false);
                return await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string PostRequestResponse(string URL, Dictionary<string, string> Parameters)
        {
            try
            {
                if (Parameters != null)
                    foreach (var parameter in Parameters)
                    {
                        URL += (URL.Contains("?")) ? "&" : "?";
                        URL += $"{parameter.Key}={parameter.Value}";
                    }

                using WebClient client = new();
                return client.UploadString(URL, "POST", string.Empty);
            }
            catch (Exception e)
            {
                return URL + " " + e.Message;
            }
        }
    }
}
