namespace Platform;

public class Capital
{
    private RequestDelegate? next;

    public Capital()
    {
    }

    public Capital(RequestDelegate next)
    {
        this.next = next;
    }

    public static async Task Endpoint(HttpContext context)
    {
        string? capital = null;
        string? country = context.Request.RouteValues["country"] as string;
        switch ((country ?? "").ToLower())
        {
            case "uk":
                capital = "London";
                break;
            case "france":
                capital = "Paris";
                break;
            case "monaco":
                context.Response.Redirect($"/population/country/{country}");
                return;
        }

        if (capital != null)
        {
            await context.Response.WriteAsync($"{capital} is the capital of {country}");
            
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
    }

    public async Task Invoke(HttpContext context)
    {
        string[] parts = context.Request.Path.ToString().Split("/", StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 2 && parts[0] == "capital")
        {
            string? capital = null;
            string country = parts[1];
            switch (country.ToLower())
            {
                case "uk":
                    capital = "London";
                    break;
                case "france":
                    capital = "Paris";
                    break;
                case "monaco":
                    context.Response.Redirect($"/population/country/{country}");
                    return;
            }

            if (capital != null)
            {
                await context.Response.WriteAsync($"{capital} is the capital of {country}");
                return;
            }
        }

        if (next != null)
        {
            await next(context);
        }
    }
}