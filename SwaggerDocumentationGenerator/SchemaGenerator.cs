using Namotion.Reflection;
using SwaggerDocumentationGenerator.Data;
using SwaggerDocumentationGenerator.Helpers;

namespace SwaggerDocumentationGenerator
{
	public class SchemaGenerator
	{
		public static Dictionary<string, Schema> GetDictionaryProperties(Type type)
		{
			var (keyType, valueType) = type.UnwrapDictionaryTypes();
			TypeCollection.AddIfNeeded(keyType);
			TypeCollection.AddIfNeeded(valueType);
			return new Dictionary<string, Schema>
				   {
					   [DataTypeNames.Key] = GetSchema(keyType),
					   [DataTypeNames.Value] = GetSchema(valueType)
				   };
		}

		public static Schema GetSchema(Type type)
		{
			if (type.ToContextualType().IsNullableType)
				return GetSchema(type.UnwrapNullableType());

			if (type.IsEnumerable())
				return Schema.ForEnumerable(GetSchema(type.UnwrapEnumerableType()));

			if (type.IsDictionary())
				return Schema.ForDictionary(GetDictionaryProperties(type));

			if (type.ToContextualType().IsNullableType && type.IsSystemType())
			{
				return new Schema
					   {
						   Type = type.GenericTypeArguments.Single().Name
					   };
			}

			if (type.IsEnum)
				return Schema.ForEnum(type);

			var forPrimitiveType = DataTypeNames.GetForPrimitiveType(type);
			if (forPrimitiveType.HasValue)
				return Schema.ForPrimitive(forPrimitiveType.Value.typeName, forPrimitiveType.Value.format);

			return Schema.ForReusableType(type);
		}
	}
}
