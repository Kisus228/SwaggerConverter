using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwaggerDocumentationGenerator.Helpers;
using SwaggerDocumentationGenerator.Options;
using SwaggerDocumentationGenerator.Temp;

namespace SwaggerDocumentationGenerator
{
	public class SwaggerDocumentationGenerator
	{
		public static void GenerateDocumentation(DocumentationGeneratorOptions options)
		{
			File.WriteAllText(options.OutputPath, Generate(options));
		}

		private static JObject GetSchemas()
		{
			var schemas = new JObject();
			foreach (var description in TypeCollection.GetAll())
			{
				schemas.Add(description.Key, JObject.FromObject(description.Value, CommonHelpers.Serializer));
			}

			return schemas;
		}

		private static string Generate(DocumentationGeneratorOptions options)
		{
			var controllers = options.ApiControllers;

			DefinitionsGenerator.FillByParameterAndReturnTypes(controllers);
			var paths = PathsGenerator.GetPaths(controllers, options.ApiVersion);
			
			var root = new JObject
			           {
				           { "openapi", "3.0.0" },
				           {
					           "servers", new JArray(options.ServerUrls.Select(url => new JObject
					                                                                  {
						                                                                  { "url", $"{url}" }
					                                                                  }))
				           },
				           {
					           "info", new JObject
					                   {
						                   { "title", "API веб-приложения" },
						                   { "version", $"v{options.ApiVersion}" }
					                   }
				           },
				           { "paths", paths },
				           {
					           "components", new JObject
					                         {
						                         { "schemas", GetSchemas() }
					                         }
				           }
			           };
			return JsonConvert.SerializeObject(root, CommonHelpers.JsonSerializerSettings);
		}

		private static Type[] GetControllers<TBaseController, TFromAssembly>()
		{
			return Assembly.GetAssembly(typeof(TFromAssembly))!.GetTypes()
						   .Where(c => c is { IsClass: true, IsAbstract: false, IsPublic: true } && c.IsSubclassOf(typeof(TBaseController)))
						   .ToArray();
		}
	}
}
