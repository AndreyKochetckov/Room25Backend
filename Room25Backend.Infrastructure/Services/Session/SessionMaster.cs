using Newtonsoft.Json;
using Room25Backend.Domain.Entities;
using Room25Backend.Domain.Entities.Tiles;
using Room25Backend.Infrastructure.DTO;

namespace Room25Backend.Infrastructure.Services.Session;

public static class SessionMaster
{
    private static readonly JsonSerializerSettings _serializeSettings = new() { TypeNameHandling = TypeNameHandling.Auto };

    public static string Turn(GameInfo gameInfo, PlayerRequest request, PlayerConnection playerConnection)
    {
        bool isValid = false;

        if (playerConnection.Name == gameInfo.CurrentPlayer.Name)
        {
            isValid = true;
            var action = request.Action.Split(':');
            var (currentX, currentY) = (gameInfo.CurrentPlayer.X, gameInfo.CurrentPlayer.Y);
            var (destinationX, destinationY) = (0, 0);
            if (action[1].ToLower() == "up")        (destinationX, destinationY) = (currentX, currentY - 1);
            if (action[1].ToLower() == "right")     (destinationX, destinationY) = (currentX + 1, currentY);
            if (action[1].ToLower() == "down")      (destinationX, destinationY) = (currentX, currentY + 1);
            if (action[1].ToLower() == "left")      (destinationX, destinationY) = (currentX - 1, currentY);

            var player = gameInfo.Players.Single(p => p.Name == request.Name);

            isValid = gameInfo.Field[currentX, currentY].Action(gameInfo, player, action, destinationX, destinationY); //Обработка действия игрока в комнате

            if (isValid) //Если ход соблюдает правила игры
            {
                foreach (var tile in gameInfo.Field) { tile.Update(gameInfo); } //Фоновая обработка логики комнат

                if (gameInfo.CurrentPlayer.Name == gameInfo.Players.Last().Name)
                {
                    MoveFirstToLast(gameInfo.Players);
                    gameInfo.CurrentPlayer = gameInfo.Players[0];
                    gameInfo.CurrentPlayerIndex = 0;
                    gameInfo.RemainingTurns--;
                }
                else
                {
                    gameInfo.CurrentPlayer = gameInfo.Players[++gameInfo.CurrentPlayerIndex];
                }

                // Проигрыш, если ходов не осталось, или кто-то из персонажей погиб
                if (gameInfo.RemainingTurns == 0 || gameInfo.Players.Select(p => p.Character).Select(c => c.IsAlive).Contains(false))
                {
                    gameInfo.GameStatus = GameStatus.Defeat;
                    gameInfo.IsStarted = false;
                }
            }
        }

        return SerializeData(gameInfo, playerConnection, isValid);
    }

    private static string SerializeData(GameInfo gameInfo, PlayerConnection playerConnection, bool isValid)
    {
        string result = string.Empty;

        switch (gameInfo.GameMode)
        {
            case GameMode.Cooperation:
                {
                    var publicData = new PlayerResponse
                    {
                        IsValid = isValid,
                        GameMode = GameMode.Cooperation,
                        Players = gameInfo.Players,
                        CurrentPlayer = gameInfo.CurrentPlayer,
                        IsStarted = true,
                        RemainingTurns = gameInfo.RemainingTurns,
                        Field = new Tile[5, 5]
                    };

                    for (int i = 0; i < gameInfo.Field.GetLength(0); i++)
                    {
                        for (int j = 0; j < gameInfo.Field.GetLength(1); j++)
                        {
                            if (gameInfo.Field[i, j].IsVisible)
                            {
                                publicData.Field[i, j] = gameInfo.Field[i, j];
                            }
                        }
                    }

                    result = JsonConvert.SerializeObject(publicData, _serializeSettings);
                }
                break;

            case GameMode.Suspicion:
                {
                }
                break;
        }

        return result;
    }

    public static void InitializeField(GameInfo gameInfo)
    {
        gameInfo.Field = new Tile[5, 5];
        gameInfo.Field[2, 2] = new CentralRoom() { IsVisible = true, X = 2, Y = 2 };

        var coord = ExitRoom.GetRoomCoords();
        gameInfo.Field[coord.Item1, coord.Item2] = new ExitRoom() { X = coord.Item1, Y = coord.Item2 };

        var tiles = new Stack<Tile>();

        foreach (var tile in GetTiles(gameInfo))
        {
            tiles.Push(tile);
        }

        tiles = tiles.Shuffle();

        for (int i = 0; i < gameInfo.Field.GetLength(0); i++)
        {
            for (int j = 0; j < gameInfo.Field.GetLength(1); j++)
            {
                if (gameInfo.Field[i, j] == null)
                {
                    var tile = tiles.Pop();
                    (tile.X, tile.Y) = (i, j);
                    gameInfo.Field[i, j] = tile;
                }
            }
        }
    }

    private static List<Tile> GetTiles(GameInfo gameInfo)
    {
        var tiles = new List<Tile>();
        
        switch (gameInfo.GameMode)
        {
            case GameMode.Cooperation:
                {
                    tiles.AddRange(
                    [
                        new KeyRoom(),
                        new EmptyRoom(),
                        new EmptyRoom(),
                        new TunelRoom(),
                        new TunelRoom(),
                        new VortexRoom(),
                        new PrisonRoom(),
                        new PrisonRoom(),
                        new DarkRoom(),
                        new DarkRoom(),
                        new ColdRoom(),
                        new ColdRoom(),
                        new PivotingRoom(),
                        new PivotingRoom(),
                        new FloodedRoom(),
                        new FloodedRoom(),
                        new ShredderRoom(),
                        new ShredderRoom(),
                        new TrappedRoom(),
                        new TrappedRoom(),
                        new AcidRoom(),
                        new AcidRoom(),
                        new MortalRoom(),
                        new MortalRoom(),
                    ]);
                }
                break;

            case GameMode.Suspicion:
                {
                    
                }
                break;
        }

        return tiles;
    }

    public static int GetTurnCount(GameInfo gameInfo)
    {
        switch (gameInfo.GameMode)
        {
            case GameMode.Cooperation:
                {
                    if (gameInfo.Players.Count == 4) return 8;
                    if (gameInfo.Players.Count == 5) return 7;
                    if (gameInfo.Players.Count == 6) return 6;
                }
                break;

            case GameMode.Suspicion:
                {
                    if (gameInfo.Players.Count == 4) return 10;
                    if (gameInfo.Players.Count == 5) return 10;
                    if (gameInfo.Players.Count == 6) return 10;
                    if (gameInfo.Players.Count == 7) return 9;
                    if (gameInfo.Players.Count == 8) return 8;
                }
                break;
        }

        return 0;
    }

    private static void MoveFirstToLast<T>(this List<T> list)
    {
        if (list.Count > 1)
        {
            T firstItem = list[0];
            list.RemoveAt(0);
            list.Add(firstItem);
        }
    }

    private static Stack<T> Shuffle<T>(this Stack<T> stack)
    {
        var list = stack.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            int num = new Random().Next(list.Count);
            (list[num], list[i]) = (list[i], list[num]);
        }

        var shuffledStack = new Stack<T>();
        foreach (T t in list)
            shuffledStack.Push(t);

        return shuffledStack;
    }
}
