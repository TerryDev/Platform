using Microsoft.Extensions.Options;
using Platform;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MessageOptions>(options =>
{
    options.CityName = "Edmonton";
    options.CountryName = "Canada";
});

var app = builder.Build();

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
