using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace BetSnooker.HttpHelper
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public static class HttpRequestBuilder
    {
        public static HttpRequestMessage Create(string uri = "/", HttpVerb httpVerb = HttpVerb.GET)
        {
            return new HttpRequestMessage(httpVerb.GetHttpMethod(), uri);
        }

        public static HttpRequestMessageQueryParamBuilder WithQueryParams(this HttpRequestMessage request)
        {
            return new HttpRequestMessageQueryParamBuilder { Request = request, QueryParams = new Dictionary<string, string>() };
        }

        public static HttpRequestMessageQueryParamBuilder Add(this HttpRequestMessageQueryParamBuilder queryParamBuilder, string paramName, string paramValue)
        {
            queryParamBuilder.QueryParams.Add(paramName, paramValue);
            return queryParamBuilder;
        }

        public static HttpRequestMessage BuildQuery(this HttpRequestMessageQueryParamBuilder queryParamBuilder)
        {
            if (queryParamBuilder.QueryParams.Keys.Any())
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(queryParamBuilder.Request.RequestUri.OriginalString);
                stringBuilder.Append("?");
                foreach (var pair in queryParamBuilder.QueryParams)
                {
                    stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}&", pair.Key, pair.Value);
                }

                stringBuilder.Remove(stringBuilder.Length - 1, 1); // we always have & at the end when we have parameters
                queryParamBuilder.Request.RequestUri = new Uri(stringBuilder.ToString(), UriKind.Relative);
            }

            return queryParamBuilder.Request;
        }

        private static HttpMethod GetHttpMethod(this HttpVerb verb)
        {
            switch (verb)
            {
                case HttpVerb.GET: return HttpMethod.Get;
                case HttpVerb.POST: return HttpMethod.Post;
                case HttpVerb.PUT: return HttpMethod.Put;
                case HttpVerb.DELETE: return HttpMethod.Delete;
                default: throw new NotSupportedException("[HttpRequestBuilder.GetHttpMethod]");
            }
        }
    }

    public class HttpRequestMessageQueryParamBuilder
    {
        public HttpRequestMessage Request { get; set; }
        
        public Dictionary<string, string> QueryParams { get; set; }
    }
}
