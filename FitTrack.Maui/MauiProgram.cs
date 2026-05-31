using FitTrack.Maui.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace FitTrack.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		AppDomain.CurrentDomain.UnhandledException += (_, args) =>
		{
			if (args.ExceptionObject is Exception ex)
				App.WriteErrorLog("UnhandledException", ex);
		};

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddAuthorizationCore();
		builder.Services.AddCascadingAuthenticationState();

		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddSingleton<ApiClient>();
		builder.Services.AddScoped<FitTrackAuthStateProvider>();
		builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
			sp.GetRequiredService<FitTrackAuthStateProvider>());

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
