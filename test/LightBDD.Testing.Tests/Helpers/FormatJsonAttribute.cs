using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using Newtonsoft.Json;

namespace LightBDD.Testing.Tests.Helpers
{
    internal class FormatJsonAttribute : ParameterFormatterAttribute
    {
        public override string FormatValue(object value, IValueFormattingService formattingService)
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}