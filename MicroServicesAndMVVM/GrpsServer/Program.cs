using GrpsServer.Infrastructure;
using GrpsServer.Model;
using GrpsServer.Persistence;
using GrpsServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<Logger>();
builder.Services.AddSingleton<IChatLogRepository, ChatRepository>();
builder.Services.AddSingleton<ChatService>();


var app = builder.Build();
app.MapGrpcService<MeetingService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
