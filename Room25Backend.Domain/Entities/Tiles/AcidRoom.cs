﻿namespace Room25Backend.Domain.Entities.Tiles;

public class AcidRoom : Tile
{
    public string FirstPlayerName { get; set; }

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

        if (string.IsNullOrEmpty(FirstPlayerName))
        {
            FirstPlayerName = player.Name;
            (X, Y) = (player.X, player.Y);
        }
        else
        {
            var firstPlayer = gameInfo.Players.Single(p => p.Name == FirstPlayerName);
            if (FirstPlayerName != player.Name && firstPlayer.X == X && firstPlayer.Y == Y)
            {
                firstPlayer.Character.IsAlive = false;
                FirstPlayerName = string.Empty;
            }
        }
    }

    public override void Update(GameInfo gameInfo)
    {
        
    }
}
