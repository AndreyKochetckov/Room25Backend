namespace Room25Backend.Domain.Entities.Tiles;

public class VortexRoom : Tile
{

    public override bool Action(GameInfo gameInfo, Player player, string[] action, int destinationX, int destinationY)
    {
        return false;
    }

    public override void Enter(GameInfo gameInfo, Player player)
    {
        IsVisible = true;
        var centralRoom = gameInfo.Field.Cast<Tile>().ToList().Single(t => t is CentralRoom) as CentralRoom;
        (player.X, player.Y) = (centralRoom.X, centralRoom.Y);
        centralRoom.Enter(gameInfo, player);
    }

    public override void Update(GameInfo gameInfo)
    {
        
    }
}
