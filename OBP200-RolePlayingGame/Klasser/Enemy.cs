using System;

namespace OBP200_RolePlayingGame.Klasser;

class Enemy : IAttackable
{
    public string Type { get; set; }
    public string Name { get; set; }
    public int HP { get; set; }
    public int ATK { get; set; }
    public int DEF { get; set; }
    public int XPReward { get; set; }
    public int GoldReward { get; set; }

    public Enemy(string type, string name, int hp, int atk, int def, int xp, int gold)
    {
        Type = type;
        Name = name;
        HP = hp;
        ATK = atk;
        DEF = def;
        XPReward = xp;
        GoldReward = gold;
    }

    public void TakeDamage(int dmg)
    {
        HP -= Math.Max(0, dmg);
        if (HP < 0) HP = 0;
    }
}