using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public static class HttpResponseBuilderExtensions
    {
        public static IHttpHandlerConfigurator RespondStatusCode(this IHttpResponseBuilder builder, HttpStatusCode code)
        {
            return builder.Respond((req, rsp) => rsp.SetStatusCode(code));
        }

        public static IHttpHandlerConfigurator RespondStringContent(this IHttpResponseBuilder builder, HttpStatusCode code, string content, Encoding encoding, string contentType)
        {
            return builder.Respond((req, rsp) => rsp.SetStatusCode(code).SetStringContent(content, encoding, contentType));
        }

        public static IHttpHandlerConfigurator RespondJsonContent(this IHttpResponseBuilder builder, HttpStatusCode code, object content, JsonSerializerSettings settings = null)
        {
            return builder.Respond((req, rsp) => rsp.SetStatusCode(code).SetJsonContent(content, settings));
        }
    }
}