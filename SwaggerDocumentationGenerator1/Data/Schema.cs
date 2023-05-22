using System;
using System.Collections.Generic;
using System.Reflection;
using Kontur.Elba.Core.Utilities.Helpers;
using Kontur.Elba.SwaggerDocumentationGenerator.Helpers;
using Newtonsoft.Json;

namespace Kontur.Elba.SwaggerDocumentationGenerator.Data
{
	public class Schema
	{
		//todo e_pajl нельзя одновременно заполнять и ref и description, по крайней мере для responses. description игнорируется. чтобы нет, нужно завернуть схему в allOf
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "$ref")]
		public string? Ref { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Type { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Format { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public Schema? Items { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public AdditionalPropertiesDescription? AdditionalProperties { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Description { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public object? Example { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string[]? Enum { get; set; }

		public static Schema ForPrimitive(string typeName, string? format) =>
			new()
			{
				Type = typeName,
				Format = format,
			};

		public static Schema ForEnumerable(Schema schema)
			=> new()
			   {
				   Type = DataTypeNames.Array,
				   Items = schema,
			   };

		public static Schema ForDictionary(Dictionary<string, Schema> additionalProperties) =>
			new()
			{
				Type = DataTypeNames.Object,
				AdditionalProperties = new AdditionalPropertiesDescription
									   {
										   Type = DataTypeNames.Object,
										   Properties = additionalProperties,
									   },
			};

		public static Schema ForFileResult() =>
			new()
			{
				Type = DataTypeNames.String,
				Format = "binary"
			};

		public static Schema ForReusableType(Type type)
			=> new()
			   {
				   Ref = CommonHelpers.GetDefinitionsRef(type), //todo e_pajl если названия типов повторяются, будет беда
			   };

		public static Schema ForRequestBodyRef(string controllerName, string methodName)
			=> new()
			   {
				   Ref = CommonHelpers.GetComponentsRef(controllerName, methodName)
			   };

		public static Schema ForEnum(Type type)
			=> new()
			   {
				   Type = DataTypeNames.String,
				   Enum = TypeHelpers.GetEnumMembers(type),
				   Description = TypeHelpers.GetEnumMembersDescription(type)
			   };
	}

	public static class SchemaHelpers
	{
		public static Schema FillDescription(this Schema schema, Type type)
		{
			schema.Description ??= type.GetSummary();
			schema.Example = type.GetExample();
			return schema;
		}

		public static Schema FillDescription(this Schema schema, PropertyInfo propertyInfo)
		{
			schema.Description ??= propertyInfo.GetSummary();
			schema.Example = propertyInfo.GetExample();
			return schema;
		}

		public static Schema FillDescription(this Schema schema, ParameterInfo parameterInfo)
		{
			schema.Description ??= parameterInfo.GetSummary();
			schema.Example = parameterInfo.GetExample();
			return schema;
		}
	}
}
