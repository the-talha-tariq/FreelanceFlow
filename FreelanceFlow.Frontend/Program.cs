var builder = WebApplication.CreateBuilder(args);

// NOTE: Scaffolding only (Step 2). Razor components and auth-state wiring
// are added in the frontend build step. The Frontend has no project
// reference to the Backend — all communication happens through this typed
// HttpClient, calling the Backend's REST API over HTTP.

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("FreelanceFlowApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "FreelanceFlow.Frontend" }));

app.Run();
