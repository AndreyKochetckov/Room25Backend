using Newtonsoft.Json;
using Room25Backend.Application.Interfaces;
using Room25Backend.Domain.Entities;
using Room25Backend.Domain.Entities.Characters;
using Room25Backend.Infrastructure.DTO;
using Room25Backend.Infrastructure.Services;
using Room25Backend.Infrastructure.Services.Session;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration); // Подключаем все DI сервисы

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseWebSockets();

var connections = new List<PlayerConnection>();
app.Map("/ws", async (HttpContext context, ISessionService sessionService, IServiceScopeFactory serviceScopeFactory) =>
{
    try
    {
        if (!context.WebSockets.IsWebSocketRequest) throw new Exception();

        string name = context.Request.Query["name"].ToString();
        Guid gameId = Guid.Parse(context.Request.Query["game_id"].ToString());

        using var ws = await context.WebSockets.AcceptWebSocketAsync() ?? throw new Exception();
        var playerConnection = new PlayerConnection()
        {
            Socket = ws,
            GameId = gameId,
            Name = name,
        };

        // Добавляем соединение в пул, если достаточно места в лобби и нет повторяющихся ников, иначе закрываем соединение
        if (!connections.Where(c => c.GameId == gameId && c.Name == name).Any() && connections.Where(c => c.GameId == gameId).Count() <= 4)
        {
            connections.Add(playerConnection);
        }
        else
        {
            await playerConnection.Socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Нет мест или игрок с таким ником уже существует в лобби.", CancellationToken.None);
            return;
        }
        
        await ReceiveMessage(playerConnection, serviceScopeFactory);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

async Task ReceiveMessage(PlayerConnection playerConnection, IServiceScopeFactory serviceScopeFactory)
{
    var buffer = new byte[1024 * 4];
    while (playerConnection.Socket.State == WebSocketState.Open)
    {
        var result = await playerConnection.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var request = System.Text.Json.JsonSerializer.Deserialize<PlayerRequest>(message);
            if (request != null)
            {
                var sessionService = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ISessionService>();
                if (sessionService != null)
                {
                    var gameInfo = await sessionService.GetSession(playerConnection.GameId);
                    if (gameInfo != null)
                    {
                        string responseJSON = string.Empty;
                        if (gameInfo.IsStarted) //Если партия уже идет, обрабатываем команды
                        {
                            responseJSON = SessionMaster.Turn(gameInfo, request, playerConnection);
                        }
                        else // Если нет, то выбираем персонажа
                        {
                            if (gameInfo.Players.Select(p => p.Name).Contains(request.Name))
                            {
                                gameInfo.Players.Single(p => p.Name == request.Name).Character = new Character() { Name = request.CharacterName };
                            }
                            else
                            {
                                gameInfo.Players.Add(new Player()
                                {
                                    Name = request.Name,
                                    Character = new Character() { Name = request.CharacterName },
                                });
                            }
                        }

                        await sessionService.UpdateSession(gameInfo);
                        await Broadcast(responseJSON, gameInfo.Id);
                    }
                }
                await Broadcast(message, playerConnection.GameId);
            }
        }
        else if (result.MessageType == WebSocketMessageType.Close || playerConnection.Socket.State == WebSocketState.Aborted)
        {
            var sessionService = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ISessionService>();
            if (sessionService != null)
            {
                var gameInfo = await sessionService.GetSession(playerConnection.GameId);
                if (gameInfo != null)
                {
                    var player = gameInfo.Players.FirstOrDefault(p => p.Name == playerConnection.Name);
                    if (player != null)
                    {
                        gameInfo.Players.Remove(player);
                    }
                }
                await sessionService.UpdateSession(gameInfo);
            }
            connections.Remove(playerConnection);
            await playerConnection.Socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}

async Task Broadcast(string message, Guid gameId)
{
    var bytes = Encoding.UTF8.GetBytes(message);
    foreach (var socket in connections.Where(c => c.GameId == gameId).Select(c => c.Socket))
    {
        if (socket.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

app.Run();
