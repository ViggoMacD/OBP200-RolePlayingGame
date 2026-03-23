namespace OBP200_RolePlayingGame.Klasser;

abstract class Player
{
    public string Name { get; set; }
    public string ClassName { get; set; }

    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int Gold { get; set; }
    public int XP { get; set; }
    public int Level { get; set; }
    public int Potions { get; set; }

    public string Inventory { get; set; }

    public Player(string name)
    {
        Name = name;
        XP = 0;
        Level = 1;
        Inventory = "Wooden Sword;Cloth Armor";
    }

// lite trixig polymorfism
    public abstract int CalculateDamage(int enemyDef);
    public abstract int UseSpecial(int enemyDef, bool vsBoss);
}