using CinemaNotifier.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<TelegramService>();
builder.Services.AddSingleton<MovieService>();
builder.Services.AddHostedService<Worker>();

// Basit bir Kestrel web server açıyoruz
builder.Services.AddSingleton<IHostedService, MinimalApiService>();

var host = builder.Build();
host.Run();
