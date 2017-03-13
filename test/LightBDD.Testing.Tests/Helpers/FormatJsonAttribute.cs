using System.Globalization;
using LightBDD.Core.Formatting.Parameters;
using Newtonsoft.Json;

namespace LightBDD.Testing.Tests.Helpers
{
    internal class FormatJsonAttribute : ParameterFormatterAttribute
    {
        public override string Format(CultureInfo culture, object parameter)
        {
            return JsonConvert.SerializeObject(parameter);
        }
    }
}