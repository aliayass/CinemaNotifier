using CinemaNotifier.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<TelegramService>();
builder.Services.AddSingleton<MovieService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
