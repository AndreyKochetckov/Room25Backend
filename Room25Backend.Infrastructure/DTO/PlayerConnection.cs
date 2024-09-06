using System.Net.WebSockets;

namespace Room25Backend.Infrastructure.DTO;

public class PlayerConnection
{
    public WebSocket Socket { get; set; }
    public string Name { get; set; }
    public Guid GameId { get; set; }
}
