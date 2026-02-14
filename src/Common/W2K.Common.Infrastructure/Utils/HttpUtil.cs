using System.Net.Http.Headers;
using W2K.Common.Utils;

namespace W2K.Common.Infrastructure.Utils;

public static class HttpUtil
{
    public static FormUrlEncodedContent? ToFormData(this object obj)
    {
        var formData = obj.ToKeyValue();
        return formData is null ? null : new FormUrlEncodedContent(formData);
    }

    public static FormUrlEncodedContent ToFormData(this Dictionary<string, string> dict)
    {
        return new FormUrlEncodedContent(dict);
    }

    public static HttpContent ToJsonHttpContent(this object content)
    {
        if (content is not null)
        {
            var ms = JsonUtil.SerializeJsonIntoStream(content);
            _ = ms.Seek(0, SeekOrigin.Begin);
            var httpContent = new StreamContent(ms);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpContent;
        }
        return new StringContent(string.Empty);
    }
}
