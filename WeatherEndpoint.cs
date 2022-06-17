using Platform.Services;

namespace Platform;

public class WeatherEndpoint
{
    //private IResponseFormatter formatter;

    //public WeatherEndpoint(IResponseFormatter formatter)
    //{
    //    this.formatter = formatter;
    //}
    
    public async Task Endpoint(HttpContext context, IResponseFormatter formatter)
    {
        await formatter.Format(context, "Endpoint class: it is cloudy in Calgary");
    }
}