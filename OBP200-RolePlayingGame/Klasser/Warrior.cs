using System;

namespace OBP200_RolePlayingGame.Klasser;

class Warrior : Player
{
    public Warrior(string name) : base(name)
    {
        ClassName = "Warrior";
        MaxHP = 40;
        HP = 40;
        ATK = 7;
        DEF = 5;
        Potions = 2;
        Gold = 15;
    }

    public override int CalculateDamage(int enemyDef)
    {
        int baseDmg = Math.Max(1, ATK - (enemyDef / 2));
        return baseDmg + 1; // warrior buff
    }

    public override int UseSpecial(int enemyDef, bool vsBoss)
    {
        Console.WriteLine("Warrior använder Heavy Strike!");
        int dmg = Math.Max(2, ATK + 3 - enemyDef);
        HP -= 2;
        return dmg;
    }
}