using BlazorDatasheet.Extensions;
using BlazorDatasheet.Services;
using CostAnalizerApp;
using CostAnalizerApp.Api;
using MudBlazor.Services;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;
using WebClient.Components;
using WebClient.Components.UIServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Ensure server logs are visible on the console / debug output.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddBlazorDatasheet();
builder.Services.AddMudServices();
builder.Services.AddScoped<SheetFocusMangerService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<CostAnalizerApplication>(x => Factory.GetApplication());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Show detailed exception page in development so server-side errors are visible.
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
