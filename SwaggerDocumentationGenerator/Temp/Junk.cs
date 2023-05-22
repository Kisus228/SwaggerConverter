namespace SwaggerDocumentationGenerator.Temp;

public class NativeMobileElbaController
{
}

public class NativeMobileOrganizationLoggedInControllerBase
{
}

public class NativeMobileMarketingContent
{
}

[AttributeUsage(AttributeTargets.Class)]
public class ApiMetaAttribute: Attribute
{
    public ApiMetaAttribute(string externalRootPath, string[] internalRootPath, string internalControllerPrefix, int minimalVersion, ClientType clientType)
    {
        ExternalRootPath = externalRootPath;
        InternalRootPath = internalRootPath;
        InternalControllerPrefix = internalControllerPrefix;
        MinimalVersion = minimalVersion;
        ClientType = clientType;
    }

    public string ExternalRootPath { get; set; }
    public string[] InternalRootPath { get; set; }
    public string InternalControllerPrefix { get; set; }
    public int MinimalVersion { get; set; }
    public ClientType ClientType { get; set; }
}

public enum ClientType
{
    Http = 0,
    OldAndroid = 1,
    IOS = 11,
    Android = 22,
    NativeMobile = 33,
}

[AttributeUsage(AttributeTargets.Class)]
public class NativeMobileDocumentationAttribute: Attribute
{
    public NativeMobileDocumentationAttribute(string tag)
    {
        Tag = tag;
    }

    public string Tag { get; }
}