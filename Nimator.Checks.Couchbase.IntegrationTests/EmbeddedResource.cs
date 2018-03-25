using System.IO;
using System.Reflection;

namespace Nimator.Checks.Couchbase.IntegrationTests
{
    internal class EmbeddedResource
    {
        public static string Read(string name)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{assemblyName}.{name}"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}