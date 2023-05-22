using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Kontur.Elba.Mobile.Domain;
using Kontur.Elba.SwaggerDocumentationGenerator.Helpers;
using Kontur.Elba.Web.Application.PublicInterface.Mobile.Infrastructure;
using Kontur.Elba.Web.Infrastructure.Mobile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kontur.Elba.SwaggerDocumentationGenerator
{
	public class SwaggerDocumentationGenerator
	{
		public static void Main(string[] args)
		{
			//todo e_pajl запускать автоматически, складывать файл по вменяемому пути
			File.WriteAllText("mobile_elba_api.json", Generate());
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

		private static string Generate()
		{
			InitializeDocumentation();
			const int apiVersion = 1;
			var controllers = GetControllers<NativeMobileElbaController, NativeMobileOrganizationLoggedInControllerBase>();

			var paths = PathsGenerator.GetPaths(controllers, apiVersion);
			DefinitionsGenerator.FillByParameterAndReturnTypes(controllers);

			var root = new JObject
					   {
						   { "openapi", "3.0.0" },
						   {
							   "info", new JObject
									   {
										   { "title", "Мобильное API Эльбы для нативного приложения" },
										   { "version", $"v{apiVersion}" }
									   }
						   },
						   {"paths", paths},
						   {"components", new JObject
										  {
											  {"schemas", GetSchemas()}
										  }}
					   };
			return JsonConvert.SerializeObject(root, CommonHelpers.JsonSerializerSettings);
		}

		private static void InitializeDocumentation()
		{
			XmlDocumentationReader.LoadXmlDocumentation(typeof(NativeMobileMarketingContent).Assembly); //todo e_pajl что-нибудь другое взять
		}

		private static Type[] GetControllers<TBaseController, TFromAssembly>()
		{
			return Assembly.GetAssembly(typeof(TFromAssembly)).GetTypes()
						   .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && c.IsSubclassOf(typeof(TBaseController)))
						   .ToArray();
		}
	}
}
