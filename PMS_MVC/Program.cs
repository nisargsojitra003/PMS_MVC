using Microsoft.AspNetCore.Authentication.Cookies;
using PMS_MVC.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string? baseUrl = builder.Configuration.GetSection("ApiSettings:BaseUrl").Value;
builder.Services.AddHttpClient("MyApiClient", client =>
{
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<NotificationMessages>();
builder.Services.AddScoped<APIUrls>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.LoginPath = "/login/login";
        options.AccessDeniedPath = "/login/accessdenied";
        options.SlidingExpiration = true;
    }

    );
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/dashboard/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//following condition use for clear cache and when we clicked in back than not open content.
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Dashboard"))
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");
    }

    if (context.Request.Path.StartsWithSegments("/Product"))
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");
    }

    if (context.Request.Path.StartsWithSegments("/Category"))
    {
        context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("Expires", "0");
    }

    await next.Invoke();
});

app.Use(async (ctx, next) =>
{
    await next();

    if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
    {
        //Re-execute the request so the user gets the error page
        string originalPath = ctx.Request.Path.Value;
        ctx.Items["originalPath"] = originalPath;
        ctx.Request.Path = "/dashboard/notfound";
        await next();
    }
});


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
