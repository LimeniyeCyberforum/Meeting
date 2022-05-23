using GrpsServer.Model;
using GrpsServer.Persistence;
using GrpsServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<IChatLogRepository, ChatRepository>();
builder.Services.AddSingleton<IUsersCameraCaptureRepository, UsersCameraCaptureRepository>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<UsersCameraCaptureService>();


var app = builder.Build();
app.UseGrpcWeb();
app.MapGrpcService<MeetingService>().EnableGrpcWeb();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
