using Room25Backend.Domain.Entities.Characters;

namespace Room25Backend.Domain.Entities;

public class Player
{
    public string Name { get; set; } = string.Empty;
    public int X { get; set; } = 2;
    public int Y { get; set; } = 2;
    public Character Character { get; set; }
}
