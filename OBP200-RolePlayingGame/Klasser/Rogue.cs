using System;

namespace OBP200_RolePlayingGame.Klasser;

class Rogue : Player
{
    private static Random rng = new Random();

    public Rogue(string name) : base(name)
    {
        ClassName = "Rogue";
        MaxHP = 32;
        HP = 32;
        ATK = 8;
        DEF = 3;
        Potions = 3;
        Gold = 20;
    }

    public override int CalculateDamage(int enemyDef)
    {
        int baseDmg = Math.Max(1, ATK - (enemyDef / 2));
        if (rng.NextDouble() < 0.2)
            baseDmg += 4;

        return baseDmg;
    }

    public override int UseSpecial(int enemyDef, bool vsBoss)
    {
        if (rng.NextDouble() < 0.5)
        {
            Console.WriteLine("Rogue utför en lyckad Backstab!");
            return Math.Max(4, ATK + 6);
        }

        Console.WriteLine("Backstab misslyckades!");
        return 1;
    }
}