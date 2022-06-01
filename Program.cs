var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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
    branch.UseMiddleware<Platform.QueryStringMiddleware>();

    branch.Use(async (HttpContext context, Func<Task> next) =>
    {
        await context.Response.WriteAsync("Branch middleware");
    });
});

app.UseMiddleware<Platform.QueryStringMiddleware>();


app.Run();
