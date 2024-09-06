using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Room25Backend.Application.Interfaces;
using Room25Backend.Domain.Entities;
using Room25Backend.Domain.Entities.Tiles;
using System.Text.Json;

namespace Room25Backend.Infrastructure.Services.Session;

public class SessionService(IDistributedCache cache) : ISessionService
{
    private readonly IDistributedCache _cache = cache;
    private readonly JsonSerializerSettings _serializeSettings = new() { TypeNameHandling = TypeNameHandling.Auto };

    public async Task<Guid> CreateSession()
    {
        GameInfo gameInfo = new()
        {
            Id = Guid.NewGuid(),
            GameMode = GameMode.Cooperation,
        };

        string gameInfoJSON = JsonConvert.SerializeObject(gameInfo, _serializeSettings);
        await _cache.SetStringAsync($"Session:{gameInfo.Id}", gameInfoJSON, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(60)
        });

        return gameInfo.Id;
    }

    public async Task<GameInfo> GetSession(Guid sessionId)
    {
        var gameInfoJSON = await _cache.GetStringAsync($"Session:{sessionId}");

        if (gameInfoJSON != null)
        {
            var gameInfo = JsonConvert.DeserializeObject<GameInfo>(gameInfoJSON, _serializeSettings);
            if (gameInfo != null)
            {
                return gameInfo;
            }
            else
            {
                throw new Exception($"Не удалось десериализовать объект сессии!");
            }
        }
        else
        {
            throw new Exception($"Не найдена сессия с id {sessionId}!");
        }
    }

    public async Task<Guid> UpdateSession(GameInfo gameInfo)
    {
        var gameInfoJSON = await _cache.GetStringAsync($"Session:{gameInfo.Id}");

        if (gameInfoJSON != null)
        {
            var gameInfoOld = JsonConvert.DeserializeObject<GameInfo>(gameInfoJSON, _serializeSettings);
            if (gameInfoOld != null)
            {
                gameInfoJSON = JsonConvert.SerializeObject(gameInfo, _serializeSettings);
                await _cache.SetStringAsync($"Session:{gameInfo.Id}", gameInfoJSON);

                return gameInfo.Id;
            }
            else
            {
                throw new Exception($"Не удалось десериализовать объект сессии!");
            }
        }
        else
        {
            throw new Exception($"Не найдена сессия с id {gameInfo.Id}");
        }
    }

    public async Task<bool> StartSession(Guid sessionId)
    {
        var gameInfoJSON = await _cache.GetStringAsync($"Session:{sessionId}");

        if (gameInfoJSON != null)
        {
            var gameInfo = JsonConvert.DeserializeObject<GameInfo>(gameInfoJSON, _serializeSettings);
            if (gameInfo != null)
            {
                if (!gameInfo.IsStarted)
                {
                    gameInfo.IsStarted = true;
                    SessionMaster.InitializeField(gameInfo);
                    gameInfo.CurrentPlayer = gameInfo.Players[0];
                    gameInfo.RemainingTurns = SessionMaster.GetTurnCount(gameInfo);

                    gameInfoJSON = JsonConvert.SerializeObject(gameInfo, _serializeSettings);
                    await _cache.SetStringAsync($"Session:{sessionId}", gameInfoJSON);

                    return true;
                }
            }
        }

        return false;
    }
}
