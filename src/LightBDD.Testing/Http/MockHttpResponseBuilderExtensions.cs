using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public static class MockHttpResponseBuilderExtensions
    {
        public static IMockHttpHandlerConfigurator Respond(this IMockHttpResponseBuilder builder, Action<ITestableHttpRequest, IMockHttpResponse> respond)
        {
            return builder.RespondAsync((req, rsp) => { respond(req, rsp); return Task.CompletedTask; });
        }

        public static IMockHttpHandlerConfigurator Respond(this IMockHttpResponseBuilder builder, Action<IMockHttpResponse> respond)
        {
            return builder.RespondAsync((req, rsp) => { respond(rsp); return Task.CompletedTask; });
        }

        public static IMockHttpHandlerConfigurator RespondAsync(this IMockHttpResponseBuilder builder, Func<IMockHttpResponse, Task> respond)
        {
            return builder.RespondAsync((req, rsp) => respond(rsp));
        }

        public static IMockHttpHandlerConfigurator RespondStatusCode(this IMockHttpResponseBuilder builder, HttpStatusCode code)
        {
            return builder.Respond((req, rsp) => rsp.SetStatusCode(code));
        }

        public static IMockHttpHandlerConfigurator RespondStringContent(this IMockHttpResponseBuilder builder, HttpStatusCode code, string content, Encoding encoding, string contentType)
        {
            return builder.Respond((req, rsp) => rsp.SetStatusCode(code).SetStringContent(content, encoding, contentType));
        }

        public static IMockHttpHandlerConfigurator RespondJsonContent(this IMockHttpResponseBuilder builder, HttpStatusCode code, object content, JsonSerializerSettings settings = null)
        {
            return builder.Respond((req, rsp) => rsp.SetStatusCode(code).SetJsonContent(content, settings));
        }
    }
}