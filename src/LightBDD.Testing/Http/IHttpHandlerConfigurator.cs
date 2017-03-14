using System;

namespace LightBDD.Testing.Http
{
    public interface IHttpHandlerConfigurator
    {
        MockHttpServerConfigurator Apply();
        IHttpHandlerConfigurator ExpireAfterCallNumber(int maxCallNumber);
        IHttpHandlerConfigurator ExpireAfterTime(TimeSpan maxTime);
    }
}