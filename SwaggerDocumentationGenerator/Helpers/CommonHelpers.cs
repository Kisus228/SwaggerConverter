using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SwaggerDocumentationGenerator.Temp;

namespace SwaggerDocumentationGenerator.Helpers
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
