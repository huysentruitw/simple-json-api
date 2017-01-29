using System;

namespace SimpleJsonApi.Extensions
{
    internal static class UriExtensions
    {
        public static Uri GetAbsoluteBaseUri(this Uri uri)
            => new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped));
    }
}
