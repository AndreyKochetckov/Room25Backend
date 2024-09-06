using Room25Backend.Domain.Entities.Tiles;
using Room25Backend.Domain.Entities;

namespace Room25Backend.Infrastructure.DTO;

public class PlayerResponse
{
    public Tile[,]? Field { get; set; }
    public List<Player> Players { get; set; }
    public Player CurrentPlayer { get; set; }
    public GameMode GameMode { get; set; }
    public bool IsStarted { get; set; }
    public int RemainingTurns { get; set; }
}
