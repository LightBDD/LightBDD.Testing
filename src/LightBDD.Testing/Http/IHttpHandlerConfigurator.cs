namespace LightBDD.Testing.Http
{
    public interface IHttpHandlerConfigurator
    {
        MockHttpServerConfigurator Apply();
    }
}