﻿using MAUIAndroidReleaseTool.Services;
using Microsoft.Extensions.Logging;

namespace MAUIAndroidReleaseTool;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
		builder.Services.AddMasaBlazor();
		builder.Services.AddSingleton<IStaticWebAssets, StaticWebAssets>();
		builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<ISystemService, SystemService>();

        return builder.Build();
	}
}
