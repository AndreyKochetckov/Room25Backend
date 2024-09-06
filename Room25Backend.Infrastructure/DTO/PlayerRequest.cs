namespace Room25Backend.Infrastructure.DTO;

public class PlayerRequest
{
    public string Name { get; set; }
    public Guid GameId { get; set; }
    public string CharacterName { get; set; }
    public string Action { get; set; }
}
