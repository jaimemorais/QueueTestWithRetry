using QueueWithRetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

builder.Services.AddSingleton<QueueWithRetryProcessor>();

var app = builder.Build();

app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();
