using System.Net;

namespace LightBDD.Testing.Http
{
    public static class HttpResponseBuilderExtensions
    {
        public static IHttpHandlerConfigurator RespondStatusCode(this IHttpResponseBuilder builder,HttpStatusCode code)
        {
            return builder.Respond((req, rsp) => rsp.SetStatusCode(code));
        }
    }
}