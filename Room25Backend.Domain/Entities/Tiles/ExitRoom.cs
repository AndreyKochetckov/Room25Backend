namespace Room25Backend.Domain.Entities.Tiles;

public class ExitRoom : Tile
{
    private static List<(int, int)> ExitRoomCoords = [(0, 0), (0, 1), (1, 0), (0, 3), (0, 4), (1, 4), (3, 0), (4, 0), (4, 1), (4, 3), (3, 4), (4, 4)];

    public static (int, int) GetRoomCoords()
    {
        return ExitRoomCoords[new Random().Next(ExitRoomCoords.Count) - 1];
    }

    public override bool Action(GameInfo gameInfo, Player player, string[] action, int destinationX, int destinationY)
    {
        switch (action[0])
        {
            case "move":
                {
                    if (destinationX >= 0 && destinationX <= 4 && destinationY >= 0 && destinationY <= 4)
                    {
                        if (gameInfo.Field[destinationX, destinationY].IsAccessable)
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

            case "push":
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

        var keyRoom = gameInfo.Field.Cast<Tile>().ToList().Single(t => t is KeyRoom) as KeyRoom;

        if (keyRoom.IsActivated)
        {
            var (erX, erY) = (0, 0);
            for (int i = 0; i < gameInfo.Field.GetLength(0); i++)
            {
                for (int j = 0; j < gameInfo.Field.GetLength(1); j++)
                {
                    if (gameInfo.Field[i, j] is ExitRoom) (erX, erY) = (i, j);
                }
            }

            bool isAllPlayersHere = true;
            foreach (var p in gameInfo.Players)
            {
                if ((p.X, p.Y) != (erX, erY))
                {
                    isAllPlayersHere = false;
                    break;
                }
            }

            if (isAllPlayersHere)
            {
                gameInfo.GameStatus = GameStatus.Victory;
            }
        }
    }

    public override void Update(GameInfo gameInfo)
    {
        
    }
}
