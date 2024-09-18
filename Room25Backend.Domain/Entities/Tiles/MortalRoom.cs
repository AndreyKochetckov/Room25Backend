namespace Room25Backend.Domain.Entities.Tiles;

public class MortalRoom : Tile
{

    public override bool Action(GameInfo gameInfo, Player player, string[] action, int destinationX, int destinationY)
    {
        return false;
    }

    public override void Enter(GameInfo gameInfo, Player player)
    {
        IsVisible = true;
        player.Character.IsAlive = false;
    }

    public override void Update(GameInfo gameInfo)
    {
        
    }
}
