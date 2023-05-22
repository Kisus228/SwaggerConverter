using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Kontur.Elba.SwaggerDocumentationGenerator
{
	public static class XmlDocumentationReader
	{
		private static readonly HashSet<Assembly> loadedAssemblies = new();
		private static readonly Dictionary<string, string> loadedXmlDocumentation = new();

		public static void LoadXmlDocumentation(Assembly assembly)
		{
			if (loadedAssemblies.Contains(assembly))
				return;

			var directoryPath = GetDirectoryPath(assembly);

			var xmlFilePath = Path.Combine(directoryPath, $"{assembly.GetName().Name}.xml");
			if (File.Exists(xmlFilePath)) {
				LoadXmlDocumentation(File.ReadAllText(xmlFilePath));
				loadedAssemblies.Add(assembly);
			}
		}

		private static string GetDirectoryPath(Assembly assembly)
		{
			var codeBase = assembly.CodeBase;
			var path = Uri.UnescapeDataString(new UriBuilder(codeBase).Path);
			return Path.GetDirectoryName(path)!;
		}

		private static void LoadXmlDocumentation(string xmlDocumentation)
		{
			using XmlReader xmlReader = XmlReader.Create(new StringReader(xmlDocumentation));
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "member")
				{
					var rawName = xmlReader["name"] ?? string.Empty;
					loadedXmlDocumentation[rawName] = xmlReader.ReadInnerXml();
				}
			}
		}
	}
}
