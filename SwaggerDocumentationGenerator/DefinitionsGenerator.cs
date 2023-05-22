using System.Reflection;
using Namotion.Reflection;
using SwaggerDocumentationGenerator.Data;
using SwaggerDocumentationGenerator.Helpers;
using SwaggerDocumentationGenerator.Temp;

namespace SwaggerDocumentationGenerator
{
	internal static class DefinitionsGenerator
	{
		public static void FillByParameterAndReturnTypes(IEnumerable<Type> controllers)
		{
			var parameterAndReturnTypes = GetParameterAndReturnTypes(controllers);
			var types = TypeHelpers.GetPropertyTypes(parameterAndReturnTypes)
								   .Distinct(x => x.GetName());
			foreach (var type in types)
			{
				TypeCollection.AddIfNeeded(type);
			}
		}

		public static TypeDescription GetDefinition(ParameterInfo[] parameters)
		{
			return new TypeDescription
				   {
					   Required = parameters.Select(x => x.ToContextualParameter())
											.Where(x => !x.IsNullableType)
											.Select(x => x.Name.Decapitalize())
											.ToArray(),
					   Properties = parameters.ToDictionary(x => x.Name, x => SchemaGenerator.GetSchema(x.ParameterType).FillDescription(x)),
				   };
		}

		private static IEnumerable<Type> GetParameterAndReturnTypes(IEnumerable<Type> controllers)
		{
			foreach (var controller in controllers)
			{
				var methods = controller.GetMethods();
				foreach (var methodInfo in methods)
				{
					foreach (var parameter in methodInfo.GetParameters())
						yield return parameter.ParameterType;

					if (methodInfo.ReturnType != typeof(void))
						yield return methodInfo.ReturnType;
				}
			}
		}

		public static TypeDescription GetDefinition(Type type)
		{
			if (type.IsDictionary())
			{
				return new TypeDescription
					   {
						   Type = DataTypeNames.Object,
						   Required = new[] { DataTypeNames.Key, DataTypeNames.Value }, //todo e_pajl смотреть на нуллябельность ключа и значения
						   Properties = SchemaGenerator.GetDictionaryProperties(type),
						   Description = type.GetSummary()
					   };
			}

			if (type.IsEnum)
			{
				return new TypeDescription
					   {
						   Type = DataTypeNames.String,
						   Enum = TypeHelpers.GetEnumMembers(type),
						   Description = TypeHelpers.GetEnumMembersDescription(type)
					   };
			}

			if (type.IsEnumerable())
			{
				return GetDefinition(type.UnwrapEnumerableType());
			}

			return new TypeDescription
				   {
					   Type = GetDataTypeName(type),
					   Required = GetRequiredProperties(type).ToArray(),
					   Properties = GetProperties(type),
					   Description = type.GetSummary(),
				   };
		}


		private static Dictionary<string,Schema> GetProperties(Type type)
		{
			return type.GetProperties()
					   .ToDictionary(propertyInfo => propertyInfo.Name.Decapitalize(),
									 propertyInfo => SchemaGenerator.GetSchema(propertyInfo.PropertyType.UnwrapNullableType()).FillDescription(propertyInfo));
		}

		private static string GetDataTypeName(Type type)
		{
			if (type.IsEnumerable())
				return DataTypeNames.Array;

			return type.IsSystemType()
					   ? type.GetName()
					   : DataTypeNames.Object;
		}

		private static IEnumerable<string> GetRequiredProperties(Type type)
		{
			return type.GetProperties()
					   .Where(x => x.ToContextualProperty().Nullability != Nullability.Nullable)
					   .Select(x => x.Name.Decapitalize());
		}
	}
}
