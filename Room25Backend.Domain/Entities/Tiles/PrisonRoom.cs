namespace Room25Backend.Domain.Entities.Tiles;

public class PrisonRoom : Tile
{

    public override bool Action(GameInfo gameInfo, Player player, string[] action, int destinationX, int destinationY)
    {
        switch (action[0])
        {
            case "move":
                {
                    if (destinationX >= 0 && destinationX <= 4 && destinationY >= 0 && destinationY <= 4)
                    {
                        // Если тюрьма примыкает к центральной комнате или в направлении стоит другой персонаж
                        if ((gameInfo.Field[destinationX, destinationY] is CentralRoom || gameInfo.Players.Any(p => p.X == destinationX && p.Y == destinationY))
                            && gameInfo.Field[destinationX, destinationY].IsAccessable)
                        {
                            var p = gameInfo.Players.Single(p => p.Name == player.Name);
                            p.X = destinationX;
                            p.Y = destinationY;

                            gameInfo.Field[p.X, p.Y].Enter(gameInfo, p);
                        }
                        else return false;
                    }
                    else return false;
                }
                break;

            case "look":
                {
                    if (destinationX >= 0 && destinationX <= 4 && destinationY >= 0 && destinationY <= 4)
                    {
                        gameInfo.Field[destinationX, destinationY].IsVisible = true;
                    }
                    else return false;
                }
                break;

            default: return false;
        }

        return true;
    }

    public override void Enter(GameInfo gameInfo, Player player)
    {
        IsVisible = true;
    }

    public override void Update(GameInfo gameInfo)
    {
        
    }
}
