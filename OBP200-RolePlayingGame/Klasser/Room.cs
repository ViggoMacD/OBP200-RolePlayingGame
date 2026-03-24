namespace OBP200_RolePlayingGame.Klasser;

class Room
{
    public string Type { get; set; }
    public string Name { get; set; }

    public Room(string type, string name)
    {
        Type = type;
        Name = name;
    }
}