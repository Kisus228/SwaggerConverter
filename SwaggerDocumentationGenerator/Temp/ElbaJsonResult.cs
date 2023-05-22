using System.Net;
using Newtonsoft.Json;

namespace SwaggerDocumentationGenerator.Temp;

public class ElbaJsonResult//: ActionResult
{
    public object Data { get; }
    private readonly JsonSerializer serializer;

    public ElbaJsonResult(object data, JsonSerializer? serializer = null)
    {
        Data = data;
        //this.serializer = serializer ?? JsonSerializer.WithCustomNewtonSerializer(x => x.IncludeConstants());
    }

    /*public override void ExecuteResult(ControllerContext context)
    {
        var type = GetDataType();
        var statusCode = GetStatusCode(type);

        if (statusCode.HasValue)
        {
            context.HttpContext.Response.TrySkipIisCustomErrors = true;
            context.HttpContext.Response.StatusCode = (int)statusCode;
        }

        context.HttpContext.Response.ContentType = MimeType.json;
        var outputStream = context.HttpContext.Response.OutputStream;

        using (var m = new MemoryStream())
        {
            serializer.Serialize(m, type, Data);
            m.Seek(0, SeekOrigin.Begin);
            //AddAdditionalHeaders(context, m);
            m.WriteTo(outputStream);
        }
    }*/

    public static HttpStatusCode? GetStatusCode(Type type)
    {
        //return GetStatusCodeAttribute(type)?.Code;
        return HttpStatusCode.Accepted;
    }

    /*private static JsonStatusCodeAttribute? GetStatusCodeAttribute(Type type)
    {
        return type.GetCustomAttributeOrNull<JsonStatusCodeAttribute>()
               // ReSharper disable once ConstantNullCoalescingCondition
               ?? type.GetInterfaces()
                      .Select(x => x.GetCustomAttributeOrNull<JsonStatusCodeAttribute>())
                      .FirstOrDefault(x => x != null);
    }*/

    /*private Type GetDataType()
    {
        if (Data is IPublicApiResult apiResult)
            return apiResult.Value.GetType();
        return Data?.GetType() ?? typeof(object);
    }*/

    /*protected virtual void AddAdditionalHeaders(ControllerContext context, MemoryStream memoryStream)
    {
    }*/
}