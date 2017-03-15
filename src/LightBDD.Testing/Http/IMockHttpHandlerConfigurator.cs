using System;

namespace LightBDD.Testing.Http
{
    public interface IMockHttpHandlerConfigurator
    {
        MockHttpServerConfigurator Apply();
        IMockHttpHandlerConfigurator ExpireAfterCallNumber(int maxCallNumber);
        IMockHttpHandlerConfigurator ExpireAfterTime(TimeSpan maxTime);
    }
}