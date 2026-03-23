using System;

namespace OBP200_RolePlayingGame.Klasser;

class Mage : Player
{
    public Mage(string name) : base(name)
    {
        ClassName = "Mage";
        MaxHP = 28;
        HP = 28;
        ATK = 10;
        DEF = 2;
        Potions = 2;
        Gold = 15;
    }

    public override int CalculateDamage(int enemyDef)
    {
        int baseDmg = Math.Max(1, ATK - (enemyDef / 2));
        return baseDmg + 2;
    }

    public override int UseSpecial(int enemyDef, bool vsBoss)
    {
        if (Gold >= 3)
        {
            Console.WriteLine("Mage kastar Fireball!");
            Gold -= 3;
            return Math.Max(3, ATK + 5 - (enemyDef / 2));
        }

        Console.WriteLine("Inte tillräckligt med guld.");
        return 0;
    }
}