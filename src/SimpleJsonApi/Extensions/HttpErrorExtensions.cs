using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Extensions
{
    internal static class HttpErrorExtensions
    {
        public static IEnumerable<Error> ToErrors(this HttpError httpError)
        {
            while (httpError != null)
            {
                var error = new Error { Title = httpError.Message };

                if (!string.IsNullOrEmpty(httpError.ExceptionMessage))
                {
                    var detail = new StringBuilder(httpError.ExceptionMessage);
                    if (httpError.StackTrace != null)
                    {
                        detail.AppendLine();
                        detail.Append(httpError.StackTrace);
                    }

                    error.Detail = detail.ToString();
                }

                yield return error;

                httpError = httpError.InnerException;
            }
        }
    }
}
