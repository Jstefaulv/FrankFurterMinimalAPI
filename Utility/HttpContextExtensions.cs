using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FrankfurterTest.Utility
{
    public static class HttpContextExtensions
    {
        public async static Task InsertParamsPag<T> (this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext is null) { throw new ArgumentNullException(nameof(httpContext)); }
            double cant = await queryable.CountAsync();
            httpContext.Response.Headers.Append("TotalRecords", cant.ToString());
        }

    }
}
