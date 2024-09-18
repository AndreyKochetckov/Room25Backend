namespace Room25Backend.Domain.Entities.Tiles;

public abstract class Tile
{
    public bool IsVisible { get; set; } = false;
    public bool IsAccessable { get; set; } = true;
    public int X { get; set; }
    public int Y { get; set; }

    /// <summary>
    /// Действие во время захода персонажа в комнату
    /// </summary>
    /// <param name="gameInfo">Состояние игры</param>
    /// <param name="player">Игрок</param>
    public abstract void Enter(GameInfo gameInfo, Player player);

    /// <summary>
    /// Действие игрока внутри комнаты
    /// </summary>
    /// <param name="gameInfo">Состояние игры</param>
    public abstract bool Action(GameInfo gameInfo, Player player, string[] action, int destinationX, int destinationY);

    /// <summary>
    /// Фоновая обработка
    /// </summary>
    /// <param name="gameInfo">Состояние игры</param>
    public abstract void Update(GameInfo gameInfo);
}
