using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context) 
    {
        try 
        {
            await _next(context);
        }
        catch(DbUpdateException ex) 
        {
            context.Response.ContentType = "application/json";
            if (ex.InnerException is SqlException innerException)
            {
                switch (innerException.Number)
                {
                    case 2627: // unique constraint violation
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        await context.Response.WriteAsync("unique constraint violation");
                        break;

                    case 515: // cannot insert null
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("cannot insert null");
                        break;
                    case 547: // foreign key constraint violation
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        await context.Response.WriteAsync("foreign key constraint violation");
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync("An Error Occurred : " + ex.Message);
                        break;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An Error Occurred : " + ex.Message);
            }
        }
        catch(Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("An Error Occurred : " + ex.Message);
        }
    }
}
