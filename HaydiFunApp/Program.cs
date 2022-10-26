using DataLibrary;
using HaydiFunApp;
using HaydiFunApp.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using SixLabors.ImageSharp.Web.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("C:\\AspNetConfig\\HaydiFun.json",
                       optional: true,
                       reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddImageSharp();
builder.Services.AddSingleton<IDataAccess, FBDataAccess>();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddSingleton<IPubs, Pubs>();

builder.Services.AddSingleton<EtkHub>();
builder.Services.AddSingleton<ChatHub>();
builder.Services.AddSingleton<UsrHub>();
builder.Services.AddSingleton<DataHub>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddMudServices(config =>
{
  config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

  config.SnackbarConfiguration.PreventDuplicates = false;
  config.SnackbarConfiguration.NewestOnTop = false;
  config.SnackbarConfiguration.ShowCloseIcon = true;
  config.SnackbarConfiguration.VisibleStateDuration = 10000;
  config.SnackbarConfiguration.HideTransitionDuration = 500;
  config.SnackbarConfiguration.ShowTransitionDuration = 500;
  config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});


builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
}

app.UseImageSharp();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Services.GetService<UsrHub>().LoadAllUsr();
var someService = app.Services.GetService<EtkHub>();
someService.LoadAllEtk();

app.Run();
