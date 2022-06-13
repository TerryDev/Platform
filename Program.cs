using Microsoft.Extensions.Options;
using Platform;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MessageOptions>(options =>
{
    options.CityName = "Edmonton";
    options.CountryName = "Canada";
});

var app = builder.Build();

//app.UseMiddleware<Population>();
//app.UseMiddleware<Capital>();

app.MapGet("{first}/{second}/{third}", async context =>
{
    await context.Response.WriteAsync("Request was routed:\n");
    foreach (var kvp in context.Request.RouteValues)
    {
        await context.Response.WriteAsync($"{kvp.Key}: {kvp.Value}\n");
    }
});
app.MapGet("bruh/{item}", Population.NewEndpoint);

app.MapGet("capital/{country}", Capital.Endpoint);
app.MapGet("population/{city}", Population.Endpoint);

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("routing", async context =>
    {
        await context.Response.WriteAsync("Request was routed");
    });
    endpoints.MapGet("capital/uk", new Capital().Invoke);
    endpoints.MapGet("population/paris", new Population().Invoke);  
});

app.Run(async (context) =>
{
    await context.Response.WriteAsync("Terminal Middleware reached");
});

//app.MapGet("/location", async (HttpContext context, IOptions<MessageOptions> msgOpts) =>
//{
//    Platform.MessageOptions opts = msgOpts.Value;
//    await context.Response.WriteAsync($"{opts.CityName}, {opts.CountryName}");
//});
app.UseMiddleware<LocationMiddleware>();

app.MapGet("/", () => "Hello World!");

app.Use(async (context, next) =>
{
    await next();
    await context.Response.WriteAsync($"\nStatus Code: {context.Response.StatusCode}");
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/short")
    {
        await context.Response.WriteAsync($"Request short circuited");
    }
    else
    {
        await next();
    }
});

app.Use(async (ctx, req) =>
{
    if (ctx.Request.Method == HttpMethods.Get && ctx.Request.Query["custom"] == "true")
    {
        ctx.Response.ContentType = "text/plain";
        await ctx.Response.WriteAsync("Custom Middleware \n");
    }
    await req();
});

((IApplicationBuilder) app).Map("/branch", branch =>
{
    branch.Run(new Platform.QueryStringMiddleware().Invoke);
});

app.UseMiddleware<Platform.QueryStringMiddleware>();


app.Run();
