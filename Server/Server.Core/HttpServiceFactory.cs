using System;
using System.Linq;
using System.Reflection;

namespace Server.Core
{
    public class HttpServiceFactory
    {
        private readonly IHttpServiceProcessor _defaultService;
        public HttpServiceFactory(IHttpServiceProcessor defaultService)
        {
            _defaultService = defaultService;
        }

        public IHttpServiceProcessor GetService(string canProcess, Assembly currentAssembly, string currentNameSpace,
            ServerProperties serverProperties)
        {
            var typelist = GetTypesInNamespace(currentAssembly, currentNameSpace);
            return
                typelist.Where(t => t.GetInterface("IHttpServiceProcessor", true) != null)
                    .Select(
                        t =>
                            (IHttpServiceProcessor)
                                Activator.CreateInstance(currentAssembly.ToString(),
                                    currentNameSpace + "." + t.Name).Unwrap())
                    .FirstOrDefault(service => service.CanProcessRequest(canProcess, serverProperties)) ?? _defaultService;
        }

        private Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
                assembly.GetTypes()
                    .Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                    .ToArray();
        }
    }
}