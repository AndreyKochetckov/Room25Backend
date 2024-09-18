namespace Room25Backend.Domain.Entities.Tiles;

public class PivotingRoom : Tile
{
    public bool IsVertical { get; set; }

    public override bool Action(GameInfo gameInfo, Player player, string[] action, int destinationX, int destinationY)
    {
        switch (action[0])
        {
            case "move":
                {
                    if (destinationX >= 0 && destinationX <= 4 && destinationY >= 0 && destinationY <= 4)
                    {
                        if (IsVertical)
                        {
                            if (gameInfo.Field[destinationX, destinationY].IsAccessable && Y != destinationY)
                            {
                                var p = gameInfo.Players.Single(p => p.Name == player.Name);
                                p.X = destinationX;
                                p.Y = destinationY;

                                gameInfo.Field[p.X, p.Y].Enter(gameInfo, p);
                            }
                            else return false;
                        }
                        else
                        {
                            if (gameInfo.Field[destinationX, destinationY].IsAccessable && X != destinationX)
                            {
                                var p = gameInfo.Players.Single(p => p.Name == player.Name);
                                p.X = destinationX;
                                p.Y = destinationY;

                                gameInfo.Field[p.X, p.Y].Enter(gameInfo, p);
                            }
                            else return false;
                        }
                        
                    }
                    else return false;
                }
                break;

            case "look":
                {
                    if (IsVertical)
                    {
                        if (destinationX >= 0 && destinationX <= 4 && destinationY >= 0 && destinationY <= 4 && Y != destinationY)
                        {
                            gameInfo.Field[destinationX, destinationY].IsVisible = true;
                        }
                        else return false;
                    }
                    else
                    {
                        if (destinationX >= 0 && destinationX <= 4 && destinationY >= 0 && destinationY <= 4 && X != destinationX)
                        {
                            gameInfo.Field[destinationX, destinationY].IsVisible = true;
                        }
                        else return false;
                    }
                }
                break;

            case "push":
                {
                    if (IsVertical)
                    {
                        if (destinationX >= 0 && destinationX <= 4 && destinationY >= 0 && destinationY <= 4 && Y != destinationY)
                        {
                            gameInfo.Field[destinationX, destinationY].IsVisible = true;
                        }
                        else return false;
                    }
                    else
                    {
                        if (destinationX >= 0 && destinationX <= 4 && destinationY >= 0 && destinationY <= 4 && X != destinationX)
                        {
                            gameInfo.Field[destinationX, destinationY].IsVisible = true;
                        }
                        else return false;
                    }
                }
                break;

            default: return false;
        }

        return true;
    }

    public override void Enter(GameInfo gameInfo, Player player)
    {
        IsVisible = true;
        IsVertical = !IsVertical;
    }

    public override void Update(GameInfo gameInfo)
    {
        
    }
}
