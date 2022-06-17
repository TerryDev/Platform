using Platform.Services;

namespace Platform;

public class WeatherMiddleware
{
    private RequestDelegate next;
    private IResponseFormatter formatter;

    public WeatherMiddleware(RequestDelegate next, IResponseFormatter formatter)
    {
        this.next = next;
        this.formatter = formatter;
    }
    
    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path == "/middleware/class")
        {
            await formatter.Format(context, "Middleware class: it is raining in Edmonton");
        }
        else
        {
            await next(context);
        }
    }
}