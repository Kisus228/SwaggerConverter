using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kontur.Elba.Core.Utilities.Helpers;
using Kontur.Elba.Web.Infrastructure.Common.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Helpers
{
	internal static class CommonHelpers
	{
		public const string ContentTypeJson = "application/json";
		public static string GetDefinitionsRef(Type type) => $"#/components/schemas/{type.Name.Decapitalize()}";
		public static string GetComponentsRef(string controllerName, string methodName) => $"#/components/schemas/{GetSchemasPath(controllerName, methodName)}";

		public static string GetSchemasPath(string controllerName, string methodName)
		{
			return $"{controllerName.Decapitalize()}.{methodName.Decapitalize()}";
		}

		public static IEnumerable<MethodInfo> GetMethods(Type type, int version)
		{
			var methodsWithVersion = ElbaRouter.GetMethodsWithVersion(type);
			return ElbaRouter.GetByVersion(methodsWithVersion, version)
							 .ToArray();
		}

		public static readonly JsonSerializerSettings JsonSerializerSettings = new()
																   {
																	   Formatting = Formatting.Indented,
																	   ContractResolver = new DefaultContractResolver
																		   {
																			   NamingStrategy = new CamelCaseNamingStrategy(),
																		   }
																   };
		public static readonly JsonSerializer Serializer = JsonSerializer.Create(JsonSerializerSettings);
	}
}
