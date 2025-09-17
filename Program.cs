using Movie_management_system.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Register the handler
builder.Services.AddTransient<AuthHeaderHandler>();

// Register named HttpClient and add the handler to its pipeline
builder.Services.AddHttpClient("ApiClient", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config["ApiBaseUrl"]); 
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// MVC setup (no global Jwt filter)
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

var app = builder.Build();
// using Rotativa.AspNetCore;
Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
