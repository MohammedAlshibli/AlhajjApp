using Microsoft.Extensions.Localization;
using Pligrimage.Resources;
using System.Reflection;

namespace Pligrimage.Utilities
{
    public class CultureLocalizer
    {
        private readonly IStringLocalizer _localizedizer;

        public CultureLocalizer(IStringLocalizerFactory factory)
        {
            var type = typeof(ApplicationResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizedizer = factory.Create("ApplicationResource", assemblyName.Name);
        }

        public LocalizedString Text(string key , params string[] arguments)
        {
            return arguments == null
                ? _localizedizer[key]
                : _localizedizer[key,arguments];
                
        }
    }
}
