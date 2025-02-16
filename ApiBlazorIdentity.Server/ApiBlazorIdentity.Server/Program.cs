using ApiBlazorIdentity.Server.Components;
using ApiBlazorIdentity.Server.Components.Account;
using ApiBlazorIdentity.Server.Data;
using ApiBlazorIdentity.Server.Services;
using ApiBlazorIdentity.Shared.Models;
using ApiBlazorIdentity.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
// builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddHttpClient("FromBlazorServerToWebAPI", config =>
{
    config.BaseAddress = new Uri(
        builder.Configuration["WebAPIBaseAddress"] ??
        throw new Exception("WebAPIBaseAddress is missing.")
    );
});
builder.Services.AddHttpClient("BlazorServer", config =>
{
    config.BaseAddress = new Uri(
        builder.Configuration["BlazorServerAPIBaseAddress"] ??
        throw new Exception("BlazorServerAPIBaseAddress is missing.")
    );
});

builder.Services.AddScoped<ISkillService, ServerSkillService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ApiBlazorIdentity.Wasm._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapGet("/blazor/skills", () =>
{
    List<Skill> skills =
    [
        new() { Id = 1, Name = "Sarcasm" },
        new() { Id = 2, Name = "Irony" },
        new() { Id = 3, Name = "Satire" },
        new() { Id = 4, Name = "Parody" },
        new() { Id = 5, Name = "Wit" },
    ];
    return Results.Ok(skills);
});

app.Run();