namespace Room25Backend.Domain.Entities.Tiles;

public class KeyRoom : Tile
{
    public bool IsActivated { get; set; } = false;

    public override void Action(GameInfo gameInfo, Player player)
    {
        IsActivated = true;
    }
}
