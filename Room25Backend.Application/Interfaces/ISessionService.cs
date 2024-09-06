using Room25Backend.Domain.Entities;

namespace Room25Backend.Application.Interfaces;

public interface ISessionService
{
    Task<GameInfo> GetSession(Guid sessionId);
    Task<Guid> CreateSession();
    Task<Guid> UpdateSession(GameInfo gameInfo);
    Task<bool> StartSession(Guid sessionId);
}
