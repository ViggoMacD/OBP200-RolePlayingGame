using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OBP200_RolePlayingGame.Klasser;


namespace OBP200_RolePlayingGame;


class Program
{
    static Player player;
    // ======= Globalt tillstånd  =======

    // Spelarens "databas": alla värden som strängar
    // index: 0 Name, 1 Class, 2 HP, 3 MaxHP, 4 ATK, 5 DEF, 6 GOLD, 7 XP, 8 LEVEL, 9 POTIONS, 10 INVENTORY (semicolon-sep)

    // Rum: [type, label]
    // types: battle, treasure, shop, rest, boss
    static List<Room> Rooms = new List<Room>();

    // Fiendemallar: [type, name, HP, ATK, DEF, XPReward, GoldReward]
    static List<string[]> EnemyTemplates = new List<string[]>();

    // Status för kartan
    static int CurrentRoomIndex = 0;

    // Random
    static Random Rng = new Random();

    // ======= Main =======

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        InitEnemyTemplates();

        while (true)
        {
            ShowMainMenu();
            Console.Write("Välj: ");
            var choice = (Console.ReadLine() ?? "").Trim();

            if (choice == "1")
            {
                StartNewGame();
                RunGameLoop();
            }
            else if (choice == "2")
            {
                Console.WriteLine("Avslutar...");
                return;
            }
            else
            {
                Console.WriteLine("Ogiltigt val.");
            }

            Console.WriteLine();
        }
    }

    // ======= Meny & Init =======

    static void ShowMainMenu()
    {
        Console.WriteLine("=== Text-RPG ===");
        Console.WriteLine("1. Nytt spel");
        Console.WriteLine("2. Avsluta");
    }

    static void StartNewGame()
    {
        Console.Write("Ange namn: ");
        var name = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name)) name = "Namnlös";

        Console.WriteLine("Välj klass: 1) Warrior  2) Mage  3) Rogue");
        Console.Write("Val: ");
        var k = (Console.ReadLine() ?? "").Trim();

        switch (k)
        {
            case "1":
                player = new Warrior(name);
                break;
            case "2":
                player = new Mage(name);
                break;
            case "3":
                player = new Rogue(name);
                break;
            default:
                player = new Warrior(name);
                break;
        }

        // Initiera karta (linjärt äventyr)
        Rooms.Clear();
        Rooms.Add(new Room("battle", "Skogsstig"));
        Rooms.Add(new Room("treasure", "Gammal kista"));
        Rooms.Add(new Room("shop", "Vandrande köpman"));
        Rooms.Add(new Room("battle", "Grottans mynning"));
        Rooms.Add(new Room("rest", "Lägereld"));
        Rooms.Add(new Room("battle", "Grottans djup"));
        Rooms.Add(new Room("boss", "Urdraken"));

        CurrentRoomIndex = 0;

        Console.WriteLine($"Välkommen, {name} the {player.ClassName}!");
        ShowStatus();
    }

    static void RunGameLoop()
    {
        while (true)
        {
            var room = Rooms[CurrentRoomIndex];
            Console.WriteLine($"--- Rum {CurrentRoomIndex + 1}/{Rooms.Count}: {room.Name} ({room.Type}) ---");

            bool continueAdventure = EnterRoom(room.Type);
            
            if (player.IsDead())
            {
                Console.WriteLine("Du har stupat... Spelet över.");
                break;
            }
            
            if (!continueAdventure)
            {
                Console.WriteLine("Du lämnar äventyret för nu.");
                break;
            }

            CurrentRoomIndex++;
            
            if (CurrentRoomIndex >= Rooms.Count)
            {
                Console.WriteLine();
                Console.WriteLine("Du har klarat äventyret!");
                break;
            }
            
            Console.WriteLine();
            Console.WriteLine("[C] Fortsätt     [Q] Avsluta till huvudmeny");
            Console.Write("Val: ");
            var post = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

            if (post == "Q")
            {
                Console.WriteLine("Tillbaka till huvudmenyn.");
                break;
            }

            Console.WriteLine();
        }
    }

    // ======= Rumshantering =======

    static bool EnterRoom(string type)
    {
        switch ((type ?? "battle").Trim())
        {
            case "battle":
                return DoBattle(isBoss: false);
            case "boss":
                return DoBattle(isBoss: true);
            case "treasure":
                return DoTreasure();
            case "shop":
                return DoShop();
            case "rest":
                return DoRest();
            default:
                Console.WriteLine("Du vandrar vidare...");
                return true;
        }
    }

    // ======= Strid =======

    static bool DoBattle(bool isBoss)
    {
        var enemy = GenerateEnemy(isBoss);
        Console.WriteLine($"En {enemy.Name} dyker upp! (HP {enemy.HP}, ATK {enemy.ATK}, DEF {enemy.DEF})");

        int enemyDef = enemy.DEF;

        while (enemy.HP > 0 && !player.IsDead())
        {
            Console.WriteLine();
            ShowStatus();
            Console.WriteLine($"Fiende: {enemy.Name} HP={enemy.HP}");
            Console.WriteLine("[A] Attack   [X] Special   [P] Dryck   [R] Fly");
            if (isBoss) Console.WriteLine("(Du kan inte fly från en boss!)");
            Console.Write("Val: ");

            var cmd = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

            if (cmd == "A")
            {
                int damage = player.CalculateDamage(enemyDef) + Rng.Next(0, 3);
                enemy.TakeDamage(damage);
                Console.WriteLine($"Du slog {enemy.Name} för {damage} skada.");
            }
            else if (cmd == "X")
            {
                int special = player.UseSpecial(enemyDef, isBoss);
                enemy.TakeDamage(special);
                Console.WriteLine($"Special! {enemy.Name} tar {special} skada.");
            }
            else if (cmd == "P")
            {
                player.UsePotion();
            }
            else if (cmd == "R" && !isBoss)
            {
                if (TryRunAway())
                {
                    Console.WriteLine("Du flydde!");
                    return true; // fortsätt äventyr
                }
                else
                {
                    Console.WriteLine("Misslyckad flykt!");
                }
            }
            else
            {
                Console.WriteLine("Du tvekar...");
            }

            if (enemy.HP <= 0) break;

            // Fiendens tur
            int enemyDamage = CalculateEnemyDamage(enemy.ATK);
            player.TakeDamage(enemyDamage);
            Console.WriteLine($"{enemy.Name} anfaller och gör {enemyDamage} skada!");
        }

        if (player.IsDead())
        {
            return false; // avsluta äventyr
        }

        // Vinstrapporter, XP, guld, loot
        int xpReward = enemy.XPReward;
        int goldReward = enemy.GoldReward;

        player.AddXP(xpReward);
        player.AddGold(goldReward);

        Console.WriteLine($"Seger! +{xpReward} XP, +{goldReward} guld.");
        MaybeDropLoot(enemy.Name);

        return true;
    }

    static Enemy GenerateEnemy(bool isBoss)
    {
        if (isBoss)
        {
            return new Enemy("boss", "Urdraken", 55, 9, 4, 30, 50);
        }
        else
        {
            var template = EnemyTemplates[Rng.Next(EnemyTemplates.Count)];

            int hp = ParseInt(template[2], 10) + Rng.Next(-1, 3);
            int atk = ParseInt(template[3], 3) + Rng.Next(0, 2);
            int def = ParseInt(template[4], 0) + Rng.Next(0, 2);
            int xp = ParseInt(template[5], 4) + Rng.Next(0, 3);
            int gold = ParseInt(template[6], 2) + Rng.Next(0, 3);

            return new Enemy(template[0], template[1], hp, atk, def, xp, gold);
        }
    }

    static void InitEnemyTemplates()
    {
        EnemyTemplates.Clear();
        EnemyTemplates.Add(new[] { "beast", "Vildsvin", "18", "4", "1", "6", "4" });
        EnemyTemplates.Add(new[] { "undead", "Skelett", "20", "5", "2", "7", "5" });
        EnemyTemplates.Add(new[] { "bandit", "Bandit", "16", "6", "1", "8", "6" });
        EnemyTemplates.Add(new[] { "slime", "Geléslem", "14", "3", "0", "5", "3" });
    }

    static int CalculateEnemyDamage(int enemyAtk)
    {
        int def = player.DEF;
        int roll = Rng.Next(0, 3);

        int dmg = Math.Max(1, enemyAtk - (def / 2)) + roll;

        // Liten chans till "glancing blow" (minskad skada)
        if (Rng.NextDouble() < 0.1) dmg = Math.Max(1, dmg - 2);

        return dmg;
    }
    
    //Lite polymorfism sådär
    static bool TryRunAway()
    {
        return Rng.NextDouble() < player.GetEscapeChance();
    }

    static void MaybeDropLoot(string enemyName)
    {
        // Enkel loot-regel
        if (Rng.NextDouble() < 0.35)
        {
            string item = "Minor Gem";
            if (enemyName.Contains("Urdraken")) item = "Dragon Scale";

            var inv = (player.Inventory ?? "").Trim();
            if (string.IsNullOrEmpty(inv)) player.Inventory = item;
            else player.Inventory = inv + ";" + item;

            Console.WriteLine($"Föremål hittat: {item} (lagt i din väska)");
        }
    }

    // ======= Rumshändelser =======

    static bool DoTreasure()
    {
        Console.WriteLine("Du hittar en gammal kista...");
        if (Rng.NextDouble() < 0.5)
        {
            int gold = Rng.Next(8, 15);
            player.AddGold(gold);
            Console.WriteLine($"Kistan innehåller {gold} guld!");
        }
        else
        {
            var items = new[] { "Iron Dagger", "Oak Staff", "Leather Vest", "Healing Herb" };
            string found = items[Rng.Next(items.Length)];
            var inv = (player.Inventory ?? "").Trim();
            player.Inventory = string.IsNullOrEmpty(inv) ? found : (inv + ";" + found);
            Console.WriteLine($"Du plockar upp: {found}");
        }
        return true;
    }

    static bool DoShop()
    {
        Console.WriteLine("En vandrande köpman erbjuder sina varor:");
        while (true)
        {
            Console.WriteLine($"Guld: {player.Gold} | Drycker: {player.Potions}");
            Console.WriteLine("1) Köp dryck (10 guld)");
            Console.WriteLine("2) Köp vapen (+2 ATK) (25 guld)");
            Console.WriteLine("3) Köp rustning (+2 DEF) (25 guld)");
            Console.WriteLine("4) Sälj alla 'Minor Gem' (+5 guld/st)");
            Console.WriteLine("5) Lämna butiken");
            Console.Write("Val: ");
            var val = (Console.ReadLine() ?? "").Trim();

            if (val == "1")
            {
                TryBuy(10, () => player.Potions++, "Du köper en dryck.");
            }
            else if (val == "2")
            {
                TryBuy(25, () => player.ATK += 2, "Du köper ett bättre vapen.");
            }
            else if (val == "3")
            {
                TryBuy(25, () => player.DEF += 2, "Du köper bättre rustning.");
            }
            else if (val == "4")
            {
                SellMinorGems();
            }
            else if (val == "5")
            {
                Console.WriteLine("Du säger adjö till köpmannen.");
                break;
            }
            else
            {
                Console.WriteLine("Köpmannen förstår inte ditt val.");
            }
        }
        return true;
    }

    static void TryBuy(int cost, Action apply, string successMsg)
    {
        if (player.Gold >= cost)
        {
            player.Gold -= cost;
            apply();
            Console.WriteLine(successMsg);
        }
        else
        {
            Console.WriteLine("Du har inte råd.");
        }
    }

    static void SellMinorGems()
    {
        var inv = player.Inventory ?? "";
        if (string.IsNullOrWhiteSpace(inv))
        {
            Console.WriteLine("Du har inga föremål att sälja.");
            return;
        }

        var items = inv.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        int count = items.Count(x => x == "Minor Gem");
        if (count == 0)
        {
            Console.WriteLine("Inga 'Minor Gem' i väskan.");
            return;
        }

        items = items.Where(x => x != "Minor Gem").ToList();
        player.Inventory = items.Count == 0 ? "" : string.Join(";", items);

        player.AddGold(count * 5);
        Console.WriteLine($"Du säljer {count} st Minor Gem för {count * 5} guld.");
    }

    static bool DoRest()
    {
        Console.WriteLine("Du slår läger och vilar.");
        player.HP = player.MaxHP;
        Console.WriteLine("HP återställt till max.");
        return true;
    }

    // ======= Status =======

    static void ShowStatus()
    {
        Console.WriteLine($"[{player.Name} | {player.ClassName}]  HP {player.HP}/{player.MaxHP}  ATK {player.ATK}  DEF {player.DEF}  LVL {player.Level}  XP {player.XP}  Guld {player.Gold}  Drycker {player.Potions}");

        if (!string.IsNullOrWhiteSpace(player.Inventory))
        {
            Console.WriteLine($"Väska: {player.Inventory}");
        }
    }
    
    // ======= Hjälpmetoder =======

    static int ParseInt(string s, int fallback)
    {
        try
        {
            int value = Convert.ToInt32(s);
            return value;
        }
        catch (Exception e)
        {
            return fallback;
        }
    }
}
