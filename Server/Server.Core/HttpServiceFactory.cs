using System;
using System.Collections.Generic;
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

        public IHttpServiceProcessor GetService(string canProcess,
            List<string> nameSpaces, List<Assembly> assemblies,
            ServerProperties serverProperties)
        {
            foreach (var processingService in assemblies.SelectMany(currentAssembly => (from currentNameSpace in nameSpaces
                                                                                        let typelist = GetTypesInNamespace(currentAssembly, currentNameSpace)
                                                                                        select typelist.Where(t => t.GetInterface("IHttpServiceProcessor", true)
                                                                                                                   != null)
                                                                                            .Select(
                                                                                                t =>
                                                                                                    (IHttpServiceProcessor)
                                                                                                        Activator.CreateInstance(currentAssembly.ToString(),
                                                                                                            currentNameSpace + "." + t.Name).Unwrap())
                                                                                            .FirstOrDefault(service => service.CanProcessRequest(canProcess, serverProperties))
                into processingService
                                                                                        where processingService != null
                                                                                        select processingService)))
            {
                return processingService;
            }
            return _defaultService;
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