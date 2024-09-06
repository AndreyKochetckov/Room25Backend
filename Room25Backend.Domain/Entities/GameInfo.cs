using Room25Backend.Domain.Entities.Characters;
using Room25Backend.Domain.Entities.Tiles;

namespace Room25Backend.Domain.Entities;

public class GameInfo : BaseEntity
{
    public Tile[,]? Field { get; set; }
    public List<Player> Players { get; set; } = [];
    public Player CurrentPlayer { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public GameMode GameMode { get; set; }
    public GameStatus GameStatus { get; set; }
    public bool IsStarted { get; set; } = false;
    public int RemainingTurns { get; set; }
}

public enum GameMode
{
    Cooperation = 1,
    Suspicion
}

public enum GameStatus
{
    Game,
    Defeat,
    Victory
}