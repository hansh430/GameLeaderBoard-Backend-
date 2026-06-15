using System.Reflection.Metadata.Ecma335;
using GameLeaderBoard.Middleware;

namespace GameLeaderBoard.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
