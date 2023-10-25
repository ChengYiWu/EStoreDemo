using Infrastructure;

namespace WebAPI.Infrastructure;

public class TransactionMiddleware
{
    private readonly RequestDelegate _next;

    public TransactionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, EStoreContext dbContext)
    {
        string httpVerb = httpContext.Request.Method.ToUpper();

        if (httpVerb == "POST" || httpVerb == "PUT" || httpVerb == "DELETE")
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                await _next(httpContext);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        else
        {
            await _next(httpContext);
        }
    }
}
