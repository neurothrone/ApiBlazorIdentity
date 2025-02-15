using ApiBlazorIdentity.Shared.Services;
using ApiBlazorIdentity.Wasm.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddKeyedScoped<HttpClient>(
    "FromBlazorWasmToWebAPI",
    (_, _) =>
        new HttpClient
        {
            BaseAddress = new Uri(
                builder.Configuration["WebAPIBaseAddress"] ??
                throw new Exception("WebAPIBaseAddress is missing.")
            )
        }
);
builder.Services.AddKeyedScoped<HttpClient>(
    "FromBlazorWasmToBlazorServerAPI",
    (_, _) =>
        new HttpClient
        {
            BaseAddress = new Uri(
                builder.Configuration["BlazorServerAPIBaseAddress"] ??
                throw new Exception("BlazorServerAPIBaseAddress is missing.")
            )
        }
);

builder.Services.AddScoped<ISkillService, WasmSkillService>();

await builder.Build().RunAsync();