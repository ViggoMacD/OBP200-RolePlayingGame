using System;

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

    public void TakeDamage(int dmg)
    {
        HP -= Math.Max(0, dmg);
        if (HP < 0)
            HP = 0;
    }
    
    public void UsePotion()
    {
        if (Potions <= 0)
        {
            Console.WriteLine("Du har inga drycker kvar.");
            return;
        }

        int heal = 12;
        int oldHp = HP;
        HP = Math.Min(MaxHP, HP + heal);
        Potions--;

        Console.WriteLine($"Du dricker en dryck och återfår {HP - oldHp} HP.");
    }
    
    public bool IsDead()
    {
        return HP <= 0;
    }
    
    public void AddGold(int amount)
    {
        Gold += Math.Max(0, amount);
    }
    
    public void AddXP(int amount)
    {
        XP += Math.Max(0, amount);
        LevelUpIfNeeded();
    }
    
    private void LevelUpIfNeeded()
    {
        int nextThreshold = Level == 1 ? 10 :
            Level == 2 ? 25 :
            Level == 3 ? 45 :
            Level * 20;

        if (XP >= nextThreshold)
        {
            Level++;

            switch (ClassName)
            {
                case "Warrior":
                    MaxHP += 6; ATK += 2; DEF += 2;
                    break;
                case "Mage":
                    MaxHP += 4; ATK += 4; DEF += 1;
                    break;
                case "Rogue":
                    MaxHP += 5; ATK += 3; DEF += 1;
                    break;
                default:
                    MaxHP += 4; ATK += 3; DEF += 1;
                    break;
            }

            HP = MaxHP;

            Console.WriteLine($"Du når nivå {Level}! Värden ökade och HP återställd.");
        }
    }
    
    public virtual double GetEscapeChance()
    {
        return 0.25;
    }
    
// lite trixig polymorfism
    public abstract int CalculateDamage(int enemyDef);
    public abstract int UseSpecial(int enemyDef, bool vsBoss);
}