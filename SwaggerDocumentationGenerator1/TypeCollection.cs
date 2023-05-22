using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.Elba.SwaggerDocumentationGenerator.Data;
using Kontur.Elba.SwaggerDocumentationGenerator.Helpers;
using FileResult = Kontur.Elba.Web.Infrastructure.Common.Mvc.ActionResults.FileResult;

namespace Kontur.Elba.SwaggerDocumentationGenerator
{
	internal static class TypeCollection
	{
		private static Dictionary<string, TypeDescription> Descriptions { get; set; } = new();

		public static Dictionary<string, TypeDescription> GetAll()
		{
			var descriptions =  Descriptions.ToDictionary(x => x.Key, x => x.Value);
			Descriptions = new Dictionary<string, TypeDescription>();
			return descriptions;
		}

		public static void AddIfNeeded(Type type)
		{
			var typeName = type.GetName();
			if (Descriptions.ContainsKey(typeName))
				return;
			if (type.IsSystemType())
				return;
			if (type.IsFileResultType())
				return;
			if (type.IsEnum)
				return;
			Descriptions[typeName] = DefinitionsGenerator.GetDefinition(type);
		}

		public static void AddIfNotExists(string path, TypeDescription typeDescription)
		{
			if(Descriptions.ContainsKey(path))
				return;
			Descriptions[path] = typeDescription;
		}
	}
}
