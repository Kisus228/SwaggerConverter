using SwaggerDocumentationGenerator.Data;
using SwaggerDocumentationGenerator.Helpers;

namespace SwaggerDocumentationGenerator
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
