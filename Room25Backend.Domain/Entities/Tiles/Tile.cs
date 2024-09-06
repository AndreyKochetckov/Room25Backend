namespace Room25Backend.Domain.Entities.Tiles;

public abstract class Tile
{
    public bool IsVisible { get; set; }
    public abstract void Action(GameInfo gameInfo, Player player);
}
