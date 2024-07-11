using System.Diagnostics;
class Object // Parent class for objects
{
    public bool isUsable, isBuyable, isSellable;
    public float HP;
    public ushort Level, Cost, sellValue;
    public string? Name, Colour, Description, Owner, Usage;

    public static Object[] Inventory = Array.Empty<Object>(); //Array for Inventory     
    public void ObjInfo(bool info)
    {
        if (isBuyable == true)
        {
            Program.Conversation("Info", "\n" + Name + " Lvl " + Level + "\n" + "HP     : " + HP + "\nCost   : " + Cost + "\nColour : " + Colour + "\nOwner  : " + Owner + "\nDescription: " + Description, 1f);
        }
        else
        {
            Program.Conversation("Info", "\n" + Name + " Lvl " + Level + "\n\n" + "HP     : " + HP + "\nColour : " + Colour + "\nOwner  : " + Owner + "\nDescription: " + Description, 0.5f);
        }
        Thread.Sleep(1500);
        if (info == true)
        {
            ObjUse();
        }  
    }
    public void ObjUse()
    {
        if (Name != "Trash") { Program.Conversation("Self", "Would you like to use " + Name + "?   (X for yes, Z for no)", 0.6f); }
        else { Program.Conversation("Self", "Would you like to throw " + Name + "?   (X for yes, Z for no)", 0.6f); }
        Console.ForegroundColor = ConsoleColor.Blue;
        ConsoleKey usageChoice = Console.ReadKey().Key;
        Console.WriteLine();
        if (usageChoice == ConsoleKey.X)
        {
            if (isUsable == true)
            {
                Program.Conversation("Info", "\nYou have used " + Name + "\n" + Usage, 1f);
            }
            else
            {
                Program.Conversation("Info", "\nYou cannot use this object right now.", 1f);
            }
        }
    }
    public void ObjDamage(int ObjdmgTaken)
    {
        HP -= ObjdmgTaken;
        Program.Conversation("Self", Name + " took " + ObjdmgTaken + " Damage.", 0.5f);
    }
    public void ObjRepair(int ObjHpPlus)
    {
        HP += ObjHpPlus;
        Program.Conversation("Self", Name + " was repaired for " + ObjHpPlus + " HP.", 0.5f);
    }
    public void ObjSell(float cashDiscount, float tradeDiscount, string buyerName)
    {
        Program.Conversation("Self", $"Confirm selling {Name}?", 0f);
        if (Console.ReadKey().Key == ConsoleKey.X)
        {
            Player.Money += sellValue - 10 - (tradeDiscount / 100 * sellValue) + (cashDiscount / 100 * sellValue);
            Player.isTransaction = true;
            Owner = buyerName;
            Program.Conversation("Self", "Balance: " + Player.Money, 1f);
            Program.Conversation("Self", "Transaction Confirmed\nRelation with " + buyerName + " has improved", 1);
            Player.noSold++;
        }

        else if (Console.ReadKey().Key == ConsoleKey.Z)
        {
            Player.isTransaction = false;
            Program.Conversation("Self", "Transaction Cancelled\nRelation with " + buyerName + " has worsened", 1);
        }
    }
    public void ObjBuy(float cashDiscount, float tradeDiscount, string sellerName)
    {
        if (Player.Money >= Cost) //If you got that paisa
        {
            Player.Money -= Cost - (tradeDiscount / 100 * Cost) + (cashDiscount / 100 * Cost);
            Player.isTransaction = true;
            Program.Conversation("Info", "Balance: " + Player.Money, 1f);
            Program.Conversation("Info", "Transaction Confirmed\nRelation with " + sellerName + " has improved", 1);
            Player.noBought++;
        }
        else //If u broke
        {
            Program.Conversation("Info", "You don't have enough money.", 1f);
            Player.isTransaction = false;
        }
    }
    public static void ObjAppend(Object name) //Add after checking if isTransaction == true
    {
        Inventory.Append(name);
        if (name is Heal)
        {
            Heal.Heals.Append((Heal)name);
        }
        else if (name is Buff)
        {
            Buff.Buffs.Append((Buff)name);
        }
        else if (name is Toy)
        {
            Toy.Toys.Append((Toy)name);
        }
        if (Inventory.Length == 11)
        {
            Program.Conversation("Info", "You do not have enough space, remove an item from your inventory to continue.", 0.1f);
            foreach (Object item in Inventory)
            {
                Program.Conversation("Info", $"{item.Name} ", 0f);
            }
            ConsoleKey deleteChoice = Console.ReadKey().Key;
            switch (deleteChoice)
            {
                case ConsoleKey.D1:
                    RemoveName(0);
                    break;
                case ConsoleKey.D2:
                    RemoveName(1);
                    break;
                case ConsoleKey.D3:
                    RemoveName(2);
                    break;
                case ConsoleKey.D4:
                    RemoveName(3);
                    break;
                case ConsoleKey.D5:
                    RemoveName(4);
                    break;
                case ConsoleKey.D6:
                    RemoveName(5);
                    break;
                case ConsoleKey.D7:
                    RemoveName(6);
                    break;
                case ConsoleKey.D8:
                    RemoveName(7);
                    break;
                case ConsoleKey.D9:
                    RemoveName(8);
                    break;
                case ConsoleKey.D0:
                    RemoveName(9);
                    break;
            }
        }
    }
    public static void ObjRemove(Object name) //Remove after checking if isTransaction == true
    {
        Inventory = Inventory.Where(val => val != name).ToArray();
    }
    private static void RemoveName(int index)
    {
        Inventory = Inventory.Where(val => val != Inventory[index]).ToArray();
        Program.Conversation("Info", $"{Inventory[index].Name} has been removed", 0f);
    }
}
class Heal : Object // Class for heals
{
    public float healAmou;
    public static Heal[] Heals = Array.Empty<Heal>(); //Array for heals
    public static void HealsRemove(Heal name) //Remove after checking if isTransaction == true
    {
        Heals = Heals.Where(val => val != name).ToArray();
    }
    public float PlayerHeal()
    {
        float healed;
        if (Player.HP + healAmou <= Player.maxHP)
        {
            Player.HP += healAmou;
            healed = healAmou;
        }
        else
        {
            healed = Player.maxHP - Player.HP;
            Player.HP = Player.maxHP;
        }
        return healed;
    }
}
class Buff : Object // Class for buffs
{
    public string? buffType, handicapType; //Buffs strength, speed, stamina, etc.
    public float buffAmount, handicapAmount; //How much it buffs
    public bool isHandicap; //If you sacrifice another stat for buff
    public static Buff[] Buffs = Array.Empty<Buff>(); //Array for buffs
    private void Handicap()
    {
        switch (handicapType)
        {
            case "Strength":
                Player.Strength -= handicapAmount;
                break;
            case "Speed":
                Player.Speed -= handicapAmount;
                break;
            case "Stamina":
                Player.Stamina -= handicapAmount;
                break;
        }
    }
    public void Buffing() //Use buff
    {
        switch (buffType)
        {
            case "Strength":
                Player.Strength += buffAmount;
                Handicap();
                break;
            case "Stamina":
                Player.Stamina += buffAmount;
                Handicap();
                break;
            case "Speed":
                Player.Speed += buffAmount;
                Handicap();
                break;
        }
    }
}
class Toy : Object // Class for toys
{
    public static Toy[] Toys = Array.Empty<Toy>(); //Array for buffs
}
class Food : Object //Class for food
{
    public int healFromFoodAmou;
    public void HealFromFood()
    {
        Program.Conversation("Info", $"You have eaten {Name} and have healed for {healFromFoodAmou}", 0.7f);
        if (Player.HP + healFromFoodAmou > Player.maxHP)
        {
            Player.HP = Player.maxHP;
        }
        else
        {
            Player.HP += healFromFoodAmou;
        }
    }
}
class PrisonObject : Object //Class for PrisonObjects
{ }

class WeaponsBase // Base Parent class for all weapons
{
    public string? Desc, Name;
    public float Cost;
    public int HP;
    public static WeaponsBase[] AllWeapons = Array.Empty<WeaponsBase>();
    public static void AppendWeapon(WeaponsBase name, int num) //Write weapon name after checking if isTransaction is true
    {
        AllWeapons.Append(name);
        switch (num)
        {
            case 1:
                Gun.Guns.Append(name);
                break;
            case 2:
                Melee.MeleeArray.Append(name);
                break;
            case 3:
                MiscWeapon.MiscWeapons.Append(name);
                break;
        }
    }
    public static void RemoveWeapon(WeaponsBase name, int num)
    {
        AllWeapons = AllWeapons.Where(val => val != name).ToArray();
        switch (num)
        {
            case 1:
                Gun.Guns = Gun.Guns.Where(val => val != name).ToArray();
                break;
            case 2:
                Melee.MeleeArray = Melee.MeleeArray.Where(val => val != name).ToArray();
                break;
            case 3:
                MiscWeapon.MiscWeapons = MiscWeapon.MiscWeapons.Where(val => val != name).ToArray();
                break;
        }
    }
    public void BuyWeapon(string sellerName, int discount)
    {
        if (Player.Money >= Cost)
        {
            Console.WriteLine(Name + " has been bought");
            Player.Money -= Cost - discount / 100 * Cost;
            Player.isTransaction = true;
            Program.Conversation("Info", "Transaction confirmed\nRelation with " + sellerName + " has improved. ", 1f);
            Player.noBought++;
        }
        else
        {
            Program.Conversation("Friend", "You dont have enough money.", 1);
        }
    }
    public void SellWeapon(string buyerName, int discount)
    {
        Program.Conversation("Self", "Confirm sell of " + Name, 0.1f);
        if (Console.ReadKey().Key == ConsoleKey.X)
        {
            Console.WriteLine(Name + " has been sold");
            Player.Money += Cost - discount / 100 * Cost;
            Player.isTransaction = true;
            Console.ForegroundColor = ConsoleColor.White; Console.WriteLine("Transaction cancelled\nRelation with " + buyerName + " has improved.");
            Player.noSold++;
        }

        else if (Console.ReadKey().Key == ConsoleKey.Z)
        {
            Console.WriteLine("Transaction cancelled\nRelation with " + buyerName + " has worsened.");
            Player.isTransaction = false;
        }
    }
}
class Gun : WeaponsBase//Class for Weapons (weapons not considered as an item)
{
    public string? Type;
    public ushort magazinesPerClick, Range, Ammo, dePerM, Mag;
    public float reloadTime, damagePerBullet, fireRate;
    public static Gun[] Guns = Array.Empty<Gun>(); //Array for guns

    public float FightTurn(int distance, string userTurn, NPC name) //Idk why its in this class, could be in Program as well
    {
        float totDmg = 0;
        int dupAmmo = Ammo;
        Console.ForegroundColor = ConsoleColor.White;
        if (Ammo >= magazinesPerClick) // So that ammo isnt go below 0, shoot.
        {
            bool isHit = true;
            if (distance >= Range) //Object not in range
            {
                if (userTurn == "Player") //Only when player is pew pew then their turn
                {
                    Program.Conversation("Info", "Enemy out of range.", 0.3f);
                }
            }
            else //Object in range
            {
                int headShotCheck = Program.RandomNumberGenerator.Next(1, 8);
                int missCheck = Program.RandomNumberGenerator.Next(1, 10); ;
                if (userTurn == "Player") //If players turn
                {
                    Console.WriteLine("---------------------------------------\nPick a number between 1-8");
                    int headShotCheckInput = Convert.ToInt16(Console.ReadLine());
                    switch (Type) //Check if you missed
                    {
                        case "Handgun":
                            if (missCheck == 2 | missCheck == 3) // Chance of missing
                            {
                                isHit = false;
                            }
                            else
                            {
                                isHit = true;
                            }
                            break;
                        case "Shotgun":
                            if (missCheck == 2) // Chance of missing
                            {
                                isHit = false;
                            }
                            else
                            {
                                isHit = true;
                            }
                            break;
                        case "Rifle":
                            if (missCheck == 2 | missCheck == 3 | missCheck == 1) // Chance of missing
                            {
                                isHit = false;
                            }
                            else
                            {
                                isHit = true;
                            }
                            break;
                    }

                    if (headShotCheckInput == headShotCheck & isHit == true) //Check if headshot
                    {
                        Console.WriteLine("HEADSHOT!");
                        damagePerBullet -= dePerM;
                        totDmg = 2 * magazinesPerClick * damagePerBullet; //Double dmg for headshot
                        name.TakeDamage(totDmg);
                    }
                    else if (headShotCheckInput != headShotCheck & isHit == true)
                    {
                        Console.WriteLine("BODY SHOT!");
                        damagePerBullet -= dePerM;
                        totDmg = magazinesPerClick * damagePerBullet;
                        name.TakeDamage(totDmg);
                    }
                    else
                    {
                        Console.WriteLine("YOU MISSED!");
                    }
                }
                else //If enemy turn
                {
                    Random rng3 = new();
                    int headShotCheckInput1 = Program.RandomNumberGenerator.Next(1, 8); ;
                    switch (Type) //Check if they missed
                    {
                        case "Handgun":
                            if (missCheck == 2 | missCheck == 3) // Chance of missing
                            {
                                isHit = false;
                            }
                            else
                            {
                                isHit = true;
                            }
                            break;
                        case "Shotgun":
                            if (missCheck == 2) // Chance of missing
                            {
                                isHit = false;
                            }
                            else
                            {
                                isHit = true;
                            }
                            break;
                        case "Rifle":
                            if (missCheck == 2 | missCheck == 3 | missCheck == 1) // Chance of missing
                            {
                                isHit = false;
                            }
                            else
                            {
                                isHit = true;
                            }
                            break;
                    }

                    if (headShotCheckInput1 == headShotCheck & isHit == true) //Check if headshot
                    {
                        Console.WriteLine("HEADSHOT!");
                        damagePerBullet -= dePerM;
                        totDmg = 2 * magazinesPerClick * damagePerBullet; //Double dmg for headshot
                        Player.HP -= totDmg;
                        Program.Conversation("Info", $"Enemy hit you for {totDmg} HP", 0f);
                    }
                    else if (headShotCheckInput1 != headShotCheck & isHit == true)
                    {
                        Console.WriteLine("BODY SHOT!");
                        damagePerBullet -= dePerM;
                        totDmg = magazinesPerClick * damagePerBullet;
                        Player.HP -= totDmg;
                        Program.Conversation("Info", $"Enemy hit you for {totDmg} HP", 0f);
                    }
                    else
                    {
                        Console.WriteLine("ENEMY MISSED!");
                    }
                }
            }
            int reloadTimeMili = (int)(reloadTime * 1000 / 3);
            Ammo -= magazinesPerClick;
            if ((dupAmmo - Ammo) % Mag == 0) //Check if you have to reload
            {
                if (Ammo >= magazinesPerClick)
                {
                    if (userTurn == "Player")
                    {
                        Program.Conversation("Info", "Reloading", 1f);
                        Thread.Sleep(reloadTimeMili); Console.Write(" . "); Thread.Sleep(reloadTimeMili); Console.Write(". "); Thread.Sleep(reloadTimeMili); Console.Write(". ");
                    }
                    else
                    {
                        Program.Conversation("Info", userTurn + " is reloading", 1);
                        Thread.Sleep(reloadTimeMili * 3);
                    }
                }
                else
                {
                    if (userTurn == "Player")
                    {
                        Console.WriteLine("You have run out of ammo");
                    }
                    else
                    {
                        Program.Conversation("Info", userTurn + " has run out of ammo", 1f);
                    }
                }
            }
            Player.shotsShot += magazinesPerClick;
            Player.dmgDone += totDmg;
            Ammo -= magazinesPerClick;
        }

        int fireRateMili = (int)(fireRate * 1000);
        Thread.Sleep(fireRateMili);
        return totDmg;
    }
    public void GunInfo()
    {
        Program.Conversation("Info", Name + " :\n" + Type + $"\nCost: {Cost}", 0f);
        Program.Conversation("Info", "Fire Rate    : " + fireRate + "\nMagazine Size: " + magazinesPerClick + "\nReload Time  : " + reloadTime + "\nRange        : " + Range + "m", 2.5f);
    }
    public static void BuyAmmo(string sellerName, float cashDiscount, string ammoType, int ammoAmou)
    {
        float ammoCost;
        if (Player.Money < ammoAmou * 0.673)
        {
            Program.Conversation("Hostile", "You do not have enough money", 0);
        }
        else
        {
            Program.Conversation("Self", $"Confirm purchase of {ammoAmou} {ammoType} ammo", 1f);
            if (Console.ReadKey().Key == ConsoleKey.X)
            {
                Console.WriteLine(ammoAmou + " " + ammoType + " ammo has been bought");
                switch (ammoType)
                {
                    case "Shotgun":
                        ammoCost = 0.6f * ammoAmou;
                        Player.Money -= ammoCost - cashDiscount / 100 * ammoCost;
                        break;
                    case "Rifle":
                        ammoCost = 0.92f * ammoAmou;
                        Player.Money -= ammoCost - cashDiscount / 100 * ammoCost;
                        break;
                    case "Handgun":
                        ammoCost = 0.5f * ammoAmou;
                        Player.Money -= ammoCost - cashDiscount / 100 * ammoCost;
                        break;
                }
                Console.WriteLine("Balance: " + Player.Money);
                Player.isTransaction = true;
                Program.Conversation("Self", "Transaction confirmed\nRelation with " + sellerName + " has improved.", 1);
            }

            else if (Console.ReadKey().Key == ConsoleKey.Z)
            {
                Program.Conversation("Self", "Transaction cancelled\nRelation with " + sellerName + " has worsened.", 1f);
                Player.isTransaction = false;
            }
        }
    }
    public void SellAmmo(string buyerName, float cashDiscount, float tradeDiscount, string ammoType, int ammoAmou)
    {
        float ammoCost;
        Program.Conversation("Self", "Confirm selling of " + ammoAmou + " " + ammoType + " ammo", 1f);
        if (Console.ReadKey().Key == ConsoleKey.X)
        {
            Program.Conversation("Self", ammoAmou + " " + ammoType + " ammo has been sold", 1);
            switch (ammoType)
            {
                case "Shotgun":
                    ammoCost = 0.6f * ammoAmou;
                    Player.Money -= ammoCost - tradeDiscount / 100 * Cost - cashDiscount / 100 * Cost;
                    break;
                case "Rifle":
                    ammoCost = 0.92f * ammoAmou;
                    Player.Money -= ammoCost - tradeDiscount / 100 * Cost - cashDiscount / 100 * Cost;
                    break;
                case "Handgun":
                    ammoCost = 0.5f * ammoAmou;
                    Player.Money -= ammoCost - tradeDiscount / 100 * Cost - cashDiscount / 100 * Cost;
                    break;
            }
            Console.WriteLine("Balance: " + Player.Money);
            Player.isTransaction = true;
            Program.Conversation("Self", "Transaction confirmed\nRelation with " + buyerName + " has improved.", 1);
        }

        else if (Console.ReadKey().Key == ConsoleKey.Z)
        {
            Program.Conversation("Self", "Transaction cancelled\nRelation with " + buyerName + " has worsened.", 1);
            Player.isTransaction = false;
        }
    }
}
class Melee : WeaponsBase//Class for Melee
{
    public int Damage;
    public float Length;
    public string? meleeType;
    public static Melee[] MeleeArray = Array.Empty<Melee>(); //Array for weapons
    public void MeleeInfo()
    {
        Program.Conversation("Info", $"{Name}\n{Desc}\nCost: {Cost}", 0);
    }
}
class MiscWeapon : WeaponsBase //Class for misc guns
{
    public static MiscWeapon[] MiscWeapons = Array.Empty<MiscWeapon>(); //Array for weapons
    public void MiscInfo()
    {
        Program.Conversation("Info", $"{Name}\n{Desc}\nCost: {Cost}", 0);
    }
}

class NPC // Class for NPCs (Non- Player Characters)
{

    public ushort npcAge, npcStrength, npcLvl, npcRelationship, Speed;
    public string? npcName, npcRumor, npcJob;
    public float npcHP;
    public bool isSeller, isFriend, isEnemy, isHostile, hasRumor;

    public void NPCInfo()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        if (npcRelationship >= 10)
        {
            Program.Conversation("\nSelf", npcName + " Lvl " + npcLvl + ":", 0f);
            if (isFriend == true) { Program.Conversation("Info", "Friend", 0f); }
            else if (isEnemy == true) { Program.Conversation("Info", "Enemy", 0f); }
            else if (isHostile == true) { Program.Conversation("Info", "Hostile", 0f); }
            if (isSeller == true) { Program.Conversation("Info", "Seller", 0f); }
            Program.Conversation("Self", npcJob + "\n" + npcAge + " years old\n" + npcHP + " HP\n" + npcStrength + " Strength\n", 1);
            if (hasRumor == true & npcRumor is not null)
            {
                Program.Conversation("Info", npcRumor, 0f);
            }
        }
        else
        {
            Program.Conversation("Info", "You do not know this character well enough.", 1f);
        }
    }
    public void TakeDamage(float NPCdamageTaken)
    {
        npcHP -= NPCdamageTaken;
        Program.Conversation("Info", npcName + " took " + NPCdamageTaken + " damage", 1);
    }
    public void NPCHeal(float npcHeal)
    {
        npcHP += npcHeal;
        Program.Conversation("Info", npcName + " was healed for " + npcHeal + " HP", 1);
    }
    public void NpcFriend()
    {
        if (npcRelationship >= 10)
        {
            Program.Conversation("Info", npcName + " is now your Friend", 1);
            Player.npcFriended++;
        }
    }
}

static class Player // Class for Player Details
{
    public static float Money, HP, dmgDone, Stamina, Strength, Speed, maxHP;
    public static ushort distanceTravelled, shotsShot, npcFriended, gramsSowed, shoesMade, soapMade, workNo, noBought, noSold;
    public static string? Name;
    public static bool isTransaction, isCensor;
    public static void PlayerStats()
    {
        Console.WriteLine(Name + "\nMoney: " + Money + "\nHP   : " + HP + "\nDistance Travelled: " + distanceTravelled + "\nShots Shot: " + shotsShot + "\nDamage Done: " + dmgDone + "\nNPCs Friended: " + npcFriended);
    }
}

class Program //Main Program
{
    static void Main(string[] args)
    {
        ushort dayCount = 1; //Count no. of days
        ushort eventNo = 1; //Count event number
        //Actual program start
        Player.distanceTravelled = 0; Player.dmgDone = 0; Player.HP = 100; Player.Money = 499999; Player.npcFriended = 0; Player.shotsShot = 0; Player.Speed = 25; Player.Stamina = 36; Player.Strength = 15; Player.maxHP = 100; Player.gramsSowed = 0; Player.Name = "Ayman"; Player.isCensor = true; Player.workNo = 428;//Assign Player values as it's a static class
        if (OperatingSystem.IsWindows()) //Checks if operating system is windows to avoid Warning CA1416
        {
            Console.Title = "Story - Based Puzzle Game";
            Console.CursorVisible = false; //cus nobody likes u
        }
        while (true) //Whole ass game
        {
            switch (dayCount)
            {
                case 1: //DAY 1
                    switch (eventNo)
                    {
                        case 0: //START MENU    
                            Conversation("Info", "<gameName>\n", 1);
                            Conversation("Info", "\t\t\tSTART MENU", 1);
                            Conversation("Info", "1) CONTROLS\n2) LOAD SAVE\n3) NEW GAME", 0);
                            ConsoleKey startThingy = Console.ReadKey().Key;
                            Console.WriteLine();
                            switch (startThingy)
                            {
                                case ConsoleKey.D1: //Controls
                                    Conversation("Info", "\n\t\t\tCONTROLS:", 1);
                                    Conversation("Info", "W - Forward\nA - Left\nS - Back\nD - Right", 0);
                                    Conversation("Info", "1 - Opt1\n2 - Opt2\n3 - Opt3...\n0 - Opt10", 0);
                                    Conversation("Info", "X - Select\nZ - Reject", 5);
                                    break;
                            }
                            Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                            eventNo++; break;
                        case 1: //Intro to PlayerMovement, colour based text, buying and eating food 
                            Conversation("Narrator", "Petince Prison, Okuwunya. A prison controlled jointly by prisoners and guards", 1);
                            Conversation("Friend", "Good morning sister! Today is your turn to pick up the trash in the hallways.", 1);
                            Conversation("Player", ":(", 1f);
                            Conversation("Narrator", "You see trash 2m in front of you and 1m to your left (W for forward, A for left, S for back, D for right)", 1);
                            PlayerMovement(-1, 2, "Trash", false);
                            trash.ObjUse();
                            Conversation("Friend", "\nAnyways sister would you like to go to the canteen and eat breakfast now!", 1);
                            PressX("X for thumbs up");
                            Conversation("Friend", "\nYipee let's go now", 1);
                            PlayerMovement(190, 20, "Canteen", true);
                            PlayerMovement(2, -2, "Line for Stall", false);
                            Conversation("Narrator", "You see three men in line. The first one has a gun on him, the second one looks like a bodybuilder, and the third one is an old man. Which one will you pickpocket? (1,2,3)", 1f);
                            Console.ForegroundColor = ConsoleColor.Blue;
                            while (true)
                            {
                                ConsoleKey pp1Ch = Console.ReadKey().Key;
                                bool isCon = false;
                                switch (pp1Ch)
                                {
                                    case ConsoleKey.D1:
                                        PickPocketing(100, 15, "Money", "Gunman");
                                        Conversation("Self", "Medium risk because how's he going to shoot you in the middle of a breakfast hall.", 1f);
                                        break;
                                    case ConsoleKey.D2:
                                        PickPocketing(100, 11, "Money", "Bodybuilder");
                                        Conversation("Self", "High risk because bodybuilders usually have good reflexes. You got lucky.", 1f);
                                        break;
                                    case ConsoleKey.D3:
                                        PickPocketing(100, 5, "Money", "Old Man");
                                        Conversation("Self", "Very low risk as the old man can't do anything.", 1f);
                                        break;
                                    default:
                                        Conversation("Self", "Wrong input", 1f);
                                        isCon = true;
                                        break;
                                }
                                if (isCon == true) { continue; }
                                else { break; }
                            }
                            Conversation("Narrator", "Alejandro smiles", 1);
                            Conversation("Hostile", "Ey what do you want kid.", 1);
                            EatFood(Waffle, Pancake, Porridge);
                            Conversation("Narrator", "You eat your food with Alejandro", 1);
                            Conversation("Friend", "It seems to be time for The Games now sister! I have to go to Game Hall 12 today, all the best with yours!", 1);
                            PlayerMovement(109, 7, "Game Hall 4", true);
                            Console.WriteLine(eventNo);
                            eventNo++; break;
                        case 2: //Time Based Button Clicking game 
                            Conversation("Friend", "WELCOME TO THE 983rd DAILY GAMES. TODAY'S GAME IS A TIME ACCURACY BUTTON-CLICKING GAME AND IS RATED A 5.5/10 DIFFICULTY", 1f);
                            Conversation("Friend", "THE PLAYERS FOR THIS GAME WILL BE ADIL, FABRIZIO, OUMAR, AND AYMAN", 1f);
                            Conversation("Friend", "Players, please take a seat and enjoy :)", 1);
                            Conversation("Narrator", "You sit down", 1f);
                            Conversation("Friend", "The rules are simple! You'll be given a number and when the timer starts you have to count in your mind and when you reach that number, click the button(X). The closer you are, the more money you get.", 1);
                            float copyMoney = Player.Money;
                            TimeButton(12, 1); TimeButton(7, 2); TimeButton(31.2f, 3);
                            float moneyEarned = Player.Money - copyMoney;
                            Conversation("Friend", "Well folks, that was an entertaining round! Thank you participants for participating in this game, we shall see you tommorrow :)", 1);
                            Conversation("Friend", "Participants please go to the counter to receive your money", 1);
                            PlayerMovement(4, 2, "Money Counter", false);
                            Conversation("Friend", $"You have earned {moneyEarned} coins, and here is your coupon for tomorrow's game! (100 coins)", 1);
                            day1208gameCoupon.ObjBuy(0, 0, "Money Counter");
                            day1208gameCoupon.ObjInfo(true);
                            Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine(":)");
                            PlayerMovement(100, 200, "Main Building", true);
                            Conversation("Narrator", "You travel to the main building and meet up with Alejandro", 1);
                            Conversation("Friend", "Hello sister! My game went pretty good, how was yours?", 0.25f);
                            PressX("X for thumbs up");
                            Conversation("Friend", "Nice. Let's go eat lunch now", 1);
                            PlayerMovement(50, 70, "Canteen", true);
                            EatFood(MuttonBiryani, Porridge, HoneyChickenThighs);
                            Conversation("Friend", "Well that was a good rest sister, now let's do some exercise in the courtyard", 1);
                            eventNo++; break;
                        case 3: //Exercise
                            PlayerMovement(132, 12, "Courtyard", true);
                            Exercise("Friend");
                            Conversation("Narrator", "You sit down on a bench and look at the sky...", 5);
                            Conversation("Friend", "Sister, let's go do our daily jobs now!", 1);
                            PressX("Press X to dap Alejandro up");
                            eventNo++; break;
                        case 4: //Laundry
                            PlayerMovement(80, 2, "Laundry Room", true);
                            Conversation("Narrator", "You enter a room with 14 people beginning to do the entire prisons' laundry. You spot Pedro in the corner and walk towards him", 0);
                            Conversation("Friend", "Ay wassup Ayman, looks like we'll be stuck here for an hour. Also here's the 10 bucks I owed you", 1);
                            Player.Money += 10;
                            Conversation("Friend", "Thanks man you've been a great help. Anyways, all the best with your shift mine, ends now", 1);
                            Conversation("Narrator", "You begin cleaning the clothes", 1);
                            Conversation("Narrator", "An hour passes and your shift ends, you get paid 10 coins", 1);
                            Player.Money += 10;
                            Conversation("Self", $"Balance: {Player.Money}", 1);
                            eventNo++; break;
                        case 5: //Labour
                            Conversation("Friend", "Alright sister, it's time for labour now :( Have fun sister, hopefully we get the soap making one!", 1);
                            PlayerMovement(100, 32, "Labour Center", true);
                            Conversation("Narrator", "You walk in and see a small hall full of hundreds of people with a screen in the middle and four people with large signs in each corner of the room. You hand your work coupon to the \"Work Inspector\" and are given the number 428", 1);
                            day1207workCoupon.ObjInfo(true);
                            Conversation("Hostile", "AS USUAL WE SHALL CALL YOUR NAMES, YOU WILL WALK UP, PRESENT YOUR TICKET, AND GO TO YOUR RESPECTIVE TEAMS.", 1);
                            Conversation("Narrator", "10 minutes pass and your number comes", 1);
                            Labour();
                            Conversation("Narrator", "You see Pedro running towards you", 1);
                            Conversation("Friend", "AYYYYYYMANN WASSUPPP Let's go grab some lunch", 1);
                            PressX("X for thumbs up");
                            Conversation("Friend", "Any idea where Alej is? Haven't seen him all day", 1);
                            PressX("Press X for Thumbs Down");
                            Conversation("Friend", "Dang", 1);
                            eventNo++; break;
                        case 6: //Dinner Time nom nom
                            PlayerMovement(123, 45, "Canteen", true);
                            Conversation("Hostile", "Not this fatass again, what do you want.", 1);
                            Player.isTransaction = false;   
                            EatFood(MuttonBiryani, LiteralAcid, Porridge);
                            eventNo++; break;
                        case 7: //Market yipeeeeee
                            Conversation("Friend", "Man where is Alej this is insane, anyways let's go to the market, I know you're running low on pistol ammo and heals so get those", 1);
                            PressX("X for thumbs up");
                            PlayerMovement(128, 124, "Market", true);
                            Player.isTransaction = false;
                            Market();
                            Conversation("Friend", "That was fun I guess. Man where's Alej it's been like seventy million years or something. Anyways let's go to the main building and eep", 1);
                            PlayerMovement(128, 124, "Main Building", true);
                            Conversation("Friend", "Aight see you later goodnight", 1);
                            PressX("X to Dap Up Pedro");
                            PlayerMovement(7, -2, "Jail Cell 24A", false);
                            Conversation("Friend", "Hello sister what took you so long!?", 1);
                            Conversation("Narrator", "You write down your whole day till you parted ways with Pedro and ask Alejandro where he was on writing", 1);
                            Conversation("Friend", "Do not worry about it sister, now let's sleep. Goodnight.", 1);
                            Conversation("Narrator", "You sleep", 5);
                            dayCount++; break;
                    }
                    break;
                
                case 2:
                    break;
            }
        }
    }
    static void Exercise(string Speaker)
    {
        Console.WriteLine();
        int cnt = 1; float x = 3; //x is the amount of times you can exercise
        float y = (Player.Stamina - 36) / 6;
        if (Player.Stamina % 6 == 0) //Increase exercise amount every 6 increments of Player.Stamina
        {
            x += y;
        }
        Conversation("Narrator", "You warm up", 2f);
        Conversation("Narrator", "You have warmed up, you now begin your exercise", 1f);
        while (cnt <= x)
        {
            if (cnt == 1)
            {
                Conversation(Speaker, "What will you be starting with, sister? (Upper Body, Lower Body, Cardio)", 0.4f);
            }
            else
            {
                Conversation(Speaker, "\nWhich one are you going to do now? (Upper Body, Lower Body, Cardio)", 0.4f);
            }
            ConsoleKey wkChoice = Console.ReadKey().Key;
            switch (wkChoice)
            {
                case ConsoleKey.D1:
                    Conversation("Info", "\nYou spent 10mins doing upper body Exercise, strength increase by 3", 0.4f);
                    Player.Strength += 3;
                    Conversation("Info", "Strength: " + Player.Strength, 1f);
                    break;
                case ConsoleKey.D2:
                    Conversation("Info", "\nYou spent 10mins doing lower body Exercise, speed increase by 1", 0.4f);
                    Player.Speed += 1;
                    Conversation("Info", "Speed: " + Player.Speed, 1f);
                    break;
                case ConsoleKey.D3:
                    Conversation("Info", "\nYou spent 10mins doing cardio, stamina increase by 2", 0.4f);
                    Player.Stamina += 2;
                    Conversation("Info", "Stamina: " + Player.Stamina, 1f);
                    break;
            }
            cnt++;
        }

    }
    static bool PickPocketing(float chance, float value, string Object, string name)
    {
        bool isFail;
        int rngChance = RandomNumberGenerator.Next(1, 101);
        if (rngChance <= chance)
        {
            if (Object == "Money")
            {
                Conversation("Info", "\nYou have pickpocketed " + name + " for " + value + "$", 1f);
                Player.Money += value;
            }
            else
            {
                Conversation("Info", "\nYou have pickpocketed " + name + " for " + Object, 1f);
            }
            isFail = false;
        }
        else
        {
            Conversation("Info", "You have failed to pickpocket " + name, 1f);
            isFail = true;
        }
        return isFail;
    }
    public static void Conversation(string Speaker, string? Text, float timePause)
    {
        //Censorship
        if (Player.isCensor == true & Text is not null)
        {
            Text = Text.Replace("fucking ", "").Replace("fuck", "screw").Replace("cocksucker", "waste of air").Replace("bitch", "dude").Replace("asshole", "jerk").Replace("bullshit", "nonsense").Replace("son of a bitch", "douchebag").Replace("pussy", "wimp");
            Text = Text.Replace("Fucking ", "").Replace("Fuck", "Screw").Replace("Cocksucker", "Waste of air").Replace("Bitch", "Dude").Replace("Asshole", "Jerk").Replace("Bullshit", "Nonsense").Replace("Son of a bitch", "Douchebag").Replace("Pussy", "Wimp"); ;
            Text = Text.Replace("FUCKING ", "").Replace("FUCK", "SCREW").Replace("COCKSUCKER", "WASTE OF AIR").Replace("BITCH", "DUDE").Replace("ASSHOLE", "JERK").Replace("BULLSHIT", "NONSENSE").Replace("SON OF A BITCH", "DOUCHEBAG").Replace("PUSSY", "WIMP"); ;
        }
        switch (Speaker)
        {
            case "Self":
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case "Enemy" or "Guard":
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case "Hostile":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case "Friend":
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case "Player":
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                break;
            case "Narrator":
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  [");
                break;
            case "Desc":
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case "Info":
                Console.ForegroundColor = ConsoleColor.Green;
                break;
        }
        int k = 0; int j = 1;
        if (Text is not null)
        {
            foreach (char a in Text)
            {
                if ((k + 1) / (80 * j) == 1 & Speaker == "Narrator")
                {
                    Console.Write(a + "-\n   ");
                    j++;
                }
                else if ((k + 1) / (135 * j) == 1 & Speaker != "Narrator" & Speaker != "Info")
                {
                    Console.Write(a + "-\n");
                    j++;
                }
                else if ((k + 1) / (230 * j) == 1 & Speaker == "Info")
                {
                    Console.Write(a + "-\n");
                    j++;
                }
                else
                {
                    Console.Write(a);
                }
                if (Text.IndexOf(a) != -1)
                {
                    switch (a)
                    {
                        case ',':
                            Thread.Sleep(80);
                            break;
                        case '.' or '?' or '!' or '-':
                            Thread.Sleep(200);
                            break;
                        case ' ':
                            Thread.Sleep(30);
                            break;
                        default:
                            Thread.Sleep(55);
                            break;
                    }
                }
                k++;
            }
        }

        if (Speaker == "Narrator")
        {
            Console.Write(']');
        }
        int miliSecondsTimeOut = (int)timePause * 1000;
        Thread.Sleep(miliSecondsTimeOut);
        Console.WriteLine();
    }
    static void PlayerMovement(int xPos, int yPos, string objective, bool fastTravel) //My goat
    {
        objective = char.ToUpper(objective[0]) + objective[1..]; Console.ForegroundColor = ConsoleColor.White;
        ushort countMoves = 0;
        int xplayerPos = 0;
        int yplayerPos = 0;//Set player coordinates as (0,0)

        //Visualize the distance and placement of objective and player on  cartesian plane
        int yDistance, xDistance;
        int distance = Math.Abs(xPos) + Math.Abs(yPos);
        int cnt = 1; //Check how long it took to reach
        Console.WriteLine("-------------------------------------------------------------------");
        if (fastTravel == true)
        {
            int i = 0;
            Conversation("Info", $"{objective} is " + distance + "m away. However, fast travel is available.", 0.5f);
            string x = "Fast Travel in process";
            while (i < 22)
            {
                Console.Write(x[i]);
                Thread.Sleep(30);
                i++;
            }
            Console.Write("."); Thread.Sleep(1000); Console.Write("."); Thread.Sleep(1000); Console.WriteLine("."); Thread.Sleep(1000);
            Conversation("Info", "You have fast travelled", 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-------------------------------------------------------------------");
        }
        else
        {
            while (distance != 0)
            {
                xDistance = Math.Abs(xPos - xplayerPos); yDistance = Math.Abs(yPos - yplayerPos); // x-distance to the objective and the  y-distance to the objective

                //Print distance to objective
                int nCart_xPos = xPos - xplayerPos;
                int nCart_yPos = yPos - yplayerPos;// Make  new cartesian plane, assume player position as origin, check where the objective is, and which quadrant
                int x_pDistance = Math.Abs(xPos - xplayerPos);
                int y_pDistance = Math.Abs(yPos - yplayerPos);
                Console.ForegroundColor = ConsoleColor.Green;
                if (nCart_xPos > 0 & nCart_yPos > 0) //Top right
                {
                    Console.WriteLine(objective + " is " + x_pDistance + "m to your right and " + y_pDistance + "m in front of you"); countMoves++;
                }
                else if (nCart_xPos < 0 & nCart_yPos > 0) //Top left
                {
                    Console.WriteLine(objective + " is " + x_pDistance + "m to your left and " + y_pDistance + "m in front of you"); countMoves++;
                }
                else if (nCart_xPos < 0 & nCart_yPos < 0) // Bottom left
                {
                    Console.WriteLine(objective + " is " + x_pDistance + "m to your left and " + y_pDistance + "m behind you"); countMoves++;
                }
                else if (nCart_xPos > 0 & nCart_yPos < 0) // Bottom right
                {
                    Console.WriteLine(objective + " is " + x_pDistance + "m to your right and " + y_pDistance + "m behind you"); countMoves++;
                }
                else if (nCart_xPos == 0 & nCart_yPos > 0) //Front
                {
                    Console.WriteLine(objective + " is " + y_pDistance + "m in front of you"); countMoves++;
                }
                else if (nCart_xPos == 0 & nCart_yPos < 0) //Behind
                {
                    Console.WriteLine(objective + " is " + y_pDistance + "m behind you"); countMoves++;
                }
                else if (nCart_xPos > 0 & nCart_yPos == 0) //Right
                {
                    Console.WriteLine(objective + " is " + x_pDistance + "m to your right"); countMoves++;
                }
                else if (nCart_xPos < 0 & nCart_yPos == 0)
                {
                    Console.WriteLine(objective + " is " + x_pDistance + "m to your left"); countMoves++;
                }
                //Get user input and recalculate distance based on input
                Console.ForegroundColor = ConsoleColor.Green;
                ConsoleKeyInfo userMove = Console.ReadKey(); // Not using switch statements cus tm
                Console.WriteLine();
                if (userMove.Key == ConsoleKey.W) //Going forward
                {
                    if (yDistance > Math.Abs(yPos - (yplayerPos + 1))) //If u went closer
                    {
                        distance--;
                        Console.WriteLine("You went closer");
                    }
                    else  //If u went farther
                    {
                        distance++;
                        Console.WriteLine("You went farther");
                    }
                    yplayerPos++;
                }

                else if (userMove.Key == ConsoleKey.A) //Going left
                {
                    if (xDistance > Math.Abs(xPos - (xplayerPos - 1))) //If u went closer
                    {
                        distance--;
                        Console.WriteLine("You went closer");
                    }
                    else  //If u went farther
                    {
                        distance++;
                        Console.WriteLine("You went farther");
                    }
                    xplayerPos--;
                }

                else if (userMove.Key == ConsoleKey.S) //Going back
                {
                    if (yDistance > Math.Abs(yPos - (yplayerPos - 1))) //If u went closer
                    {
                        distance--;
                        Console.WriteLine("You went closer");
                    }
                    else  //If u went farther
                    {
                        distance++;
                        Console.WriteLine("You went farther");
                    }
                    yplayerPos--;
                }

                else if (userMove.Key == ConsoleKey.D) //Going right
                {
                    if (xDistance > Math.Abs(xPos - (xplayerPos + 1))) //If u went closer
                    {
                        distance--;
                        Console.WriteLine("You went closer");
                    }

                    else  //If u went farther
                    {
                        distance++;
                        Console.WriteLine("You went farther");
                    }
                    xplayerPos++;
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("-------------------------------------------------------------------");
                cnt++;
                Player.distanceTravelled++;
            }
        }
        Conversation("Info", "\nYou have reached " + objective + ".", 0.5f);
    }
    static void GunFight(float enemyHP, int distance, NPC Enemy, Gun PlayerGun, Gun EnemWeapon) //Gunfight method
    {
        enemyHP = Enemy.npcHP;
        while (Player.HP > 0 | enemyHP > 0) //Your turn
        {
            Conversation("Info", "Fight\nInventory\nHide\nGuns", 1);
            ConsoleKey fightCbh = Console.ReadKey().Key;
            switch (fightCbh)
            {
                case ConsoleKey.D1:
                    PlayerGun.FightTurn(distance, "Player", Enemy);
                    break;
                case ConsoleKey.D2:
                    if (Object.Inventory.Length > 0)
                    {
                        for (int i = 0; i <= Object.Inventory.Length; i++)
                        {
                            Conversation("Info", Object.Inventory[i].Name + ": " + Object.Inventory[i].Description, 0.1f);
                        }
                        ConsoleKey invenCh = Console.ReadKey().Key;
                        Console.WriteLine();
                        switch (invenCh)
                        {
                            case ConsoleKey.D1: //I usually would check if the array length is great but that is way too much for me so this is all you get, each check would be like 20 lines. I'd have to do that 10 times.
                                Object.Inventory[0].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[0]).ToArray();
                                break;
                            case ConsoleKey.D2:
                                Object.Inventory[1].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[1]).ToArray();
                                break;
                            case ConsoleKey.D3:
                                Object.Inventory[2].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[2]).ToArray();
                                break;
                            case ConsoleKey.D4:
                                Object.Inventory[3].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[3]).ToArray();
                                break;
                            case ConsoleKey.D5:
                                Object.Inventory[4].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[4]).ToArray();
                                break;
                            case ConsoleKey.D6:
                                Object.Inventory[5].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[5]).ToArray();
                                break;
                            case ConsoleKey.D7:
                                Object.Inventory[6].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[6]).ToArray();
                                break;
                            case ConsoleKey.D8:
                                Object.Inventory[7].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[7]).ToArray();
                                break;
                            case ConsoleKey.D9:
                                Object.Inventory[8].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[8]).ToArray();
                                break;
                            case ConsoleKey.D0:
                                Object.Inventory[9].ObjUse();
                                Object.Inventory = Object.Inventory.Where(val => val != Object.Inventory[9]).ToArray();
                                break;
                        }
                    }
                    else
                    {
                        Conversation("Info", "You do not have enough Inventory", 0f);
                    }
                    break;
                case ConsoleKey.D3:
                    int oddHide = (int)((Player.Speed * 128 / Enemy.Speed) + 30) % 256;
                    int y = RandomNumberGenerator.Next(1, 256);
                    if (y <= oddHide)
                    {
                        Conversation("Info", "You have successfully hidden. The enemy's turn will be skipped", 1);
                    }
                    else
                    {
                        Conversation("Info", "You have not hidden.", 1);
                    }
                    break;
                case ConsoleKey.D4:
                    if (Gun.Guns.Length > 0)
                    {
                        for (int i = 0; i <= Gun.Guns.Length; i++)
                        {
                            Conversation("Info", Gun.Guns[i].Name + ": " + Gun.Guns[i].Type, 0.1f);
                        }
                        ConsoleKey gunCh = Console.ReadKey().Key;
                        Console.WriteLine();
                        switch (gunCh)
                        {
                            case ConsoleKey.D1: //I usually would check if the array length is great but that is way too much for me so this is all you get, each check would be like 20 lines. I'd have to do that 10 times.
                                GunChange(0, PlayerGun);
                                if (GunChange(0, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[0];
                                }
                                break;
                            case ConsoleKey.D2:
                                GunChange(1, PlayerGun);
                                if (GunChange(1, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[1];
                                }
                                break;
                            case ConsoleKey.D3:
                                GunChange(2, PlayerGun);
                                if (GunChange(2, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[2];
                                }
                                break;
                            case ConsoleKey.D4:
                                GunChange(3, PlayerGun);
                                if (GunChange(3, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[3];
                                }
                                break;
                            case ConsoleKey.D5:
                                GunChange(4, PlayerGun);
                                if (GunChange(4, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[4];
                                }
                                break;
                            case ConsoleKey.D6:
                                GunChange(5, PlayerGun);
                                if (GunChange(5, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[5];
                                }
                                break;
                            case ConsoleKey.D7:
                                GunChange(6, PlayerGun);
                                if (GunChange(6, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[6];
                                }
                                break;
                            case ConsoleKey.D8:
                                GunChange(7, PlayerGun);
                                if (GunChange(7, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[7];
                                }
                                break;
                            case ConsoleKey.D9:
                                GunChange(8, PlayerGun);
                                if (GunChange(8, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[8];
                                }
                                break;
                            case ConsoleKey.D0:
                                GunChange(9, PlayerGun);
                                if (GunChange(9, PlayerGun) == true)
                                {
                                    PlayerGun = Gun.Guns[9];
                                }
                                break;
                        }
                    }
                    else
                    {
                        Conversation("Info", "You do not have enough guns", 0);
                    }
                    break;
            }
            int d = Program.RandomNumberGenerator.Next(4); ;
            switch (d)
            {
                case 0: //Fight
                    if (Enemy.npcName is not null)
                    {
                        EnemWeapon.FightTurn(distance, Enemy.npcName, Enemy);
                    }
                    break;
                case 1: //Use objects
                    int y = RandomNumberGenerator.Next(3);
                    switch (y)
                    {
                        case 0: //Heal
                            y = RandomNumberGenerator.Next(10, 65); ;
                            if (Enemy.npcHP + y <= enemyHP)
                            {
                                enemyHP += y;
                                Conversation("Info", Enemy.npcName + " has healed for " + y + " HP", 0.9f);
                            }
                            else
                            {
                                Conversation("Info", Enemy.npcName + " has healed for " + y + " HP and has regained their original HP", 0.9f);
                                Enemy.npcHP = enemyHP;
                            }
                            break;
                        case 1: //Increase speed
                            Enemy.Speed += 6;
                            Conversation("Info", Enemy.npcName + " has increased their speed by 6.", 0.5f);
                            break;
                        case 2: //Increase strength
                            Enemy.npcStrength += 5;
                            Conversation("Info", Enemy.npcName + " has increased their strength by 5.", 0.5f);
                            break;
                    }
                    break;
                case 2: //Hide
                    int oddHide = (int)((Player.Speed * 128 / Enemy.Speed) + 30) % 256;
                    y = RandomNumberGenerator.Next(1, 256);
                    if (y <= oddHide)
                    {
                        Conversation("Info", "Enemy has hidden", 1);
                    }
                    else
                    {
                        Conversation("Info", "Enemy failed to hide.", 1);
                    }
                    break;
                case 3: //Change pew pew
                    int e = Program.RandomNumberGenerator.Next(1, 4);
                    if (distance <= 10)
                    {
                        EnemWeapon = GunArray[1, e];
                        Conversation("Info", "The enemy has switched weapons to " + EnemWeapon, 1);
                    }
                    else if (distance <= 25)
                    {
                        EnemWeapon = GunArray[0, e];
                        Conversation("Info", "The enemy has switched weapons to " + EnemWeapon, 1);
                    }
                    else
                    {
                        EnemWeapon = GunArray[2, e];
                        Conversation("Info", "The enemy has switched weapons to " + EnemWeapon, 1);
                    }
                    break;
            }
        }

    }
    static void Labour() //Method to do labour and earn money
    {
        int lbCh = RandomNumberGenerator.Next(4);
        switch (lbCh)
        {   
            case 0: //Wanna do agriculture
                Conversation("Hostile", $"NUMBER {Player.workNo}. AGRICULTURE", 1);
                PlayerMovement(6, 4, "Agriculture Teams", false);
                Conversation("Hostile", "As usual, you're split into teams of 16. I better see 19 acres full of wheat in the next 8hrs.", 1);
                int p = RandomNumberGenerator.Next(900, 1875); //Grams sown
                Conversation("Narrator", $"8hrs pass and you sow {p}g of wheat. You got {p / 8}$", 0.2f); //Can earn between 112.5$ and 234.375$
                Player.Money += p / 8;
                Player.gramsSowed += (ushort)p;
                break;
            case 1: //Manufacture clothes
                Conversation("Hostile", $"NUMBER {Player.workNo}. CLOTHES MANUFACTURING", 1);
                PlayerMovement(-5, 8, "Clothes Team", false);
                int q = RandomNumberGenerator.Next(10, 20);
                Conversation("Hostile", "As the leading manufacturer for Nike, we expect amazing results from you all.", 0.5f);
                Conversation("Narrator", $"8hrs pass and you create {q} shoes. You got {q * 13}$", 0.2f); //Can earn between 130$ and 260$
                Player.Money += q * 13;
                Player.shoesMade += (ushort)q;
                break;
            case 2: //Manufacture Soap
                Conversation("Hostile", $"NUMBER {Player.workNo}. SOAP MANUFACTURING", 1);
                PlayerMovement(-7, -2, "Soap Team", false);
                int r = RandomNumberGenerator.Next(40, 60);
                Conversation("Hostile", "As the leading manufacturer for Bath & Body Works, we expect amazing results from you all.", 0.5f);
                Conversation("Narrator", $"8hrs pass and you create {r} soap bars. You got {r * 3}$", 0.2f); //Can earn between 120$ and 180$
                Player.Money += r * 3;
                Player.soapMade += (ushort)r;
                break;
            case 3: //Clean Prison
                Conversation("Hostile", $"NUMBER {Player.workNo}. PRISON CLEANING", 1);
                PlayerMovement(1, -8, "Cleaners Team", false);
                int s = RandomNumberGenerator.Next(90, 120);
                Conversation("Hostile", "Chop chop cleaner boy, this place better be spotless by 5:30", 0.5f);
                Conversation("Narrator", $"8hrs pass and you clean the prison well, you earn {s}$", 0.2f); //Can earn between 90$ and 119$
                Player.Money += s;
                Player.soapMade += (ushort)s;
                break;
        }
    }
    static void Market() //Function for browsing through marketplace
    {
        int cnt = 1;
        while (Player.Money >= 0) // and if the user wants to continue
        {
            if (cnt == 1)
            {
                Conversation("Friend", "Welcome to the Prison Marketplace", 0);
                Console.WriteLine($"Balance: {Player.Money}");
                cnt++;
            }
            else
            {
                Console.WriteLine($"Balance: {Player.Money}");
                Conversation("Self", "Would you like to continue shopping?", 0f);
                ConsoleKey continue_Shopping = Console.ReadKey().Key;
                if (continue_Shopping != ConsoleKey.X)
                {
                    break; //Break while loop if player doesn't want to continue
                }
                else
                {
                    Player.isTransaction = false;
                }
            }
            Conversation("Friend", "Which shop would you like to go to? \n1) Weapons Shop\n2) Items Shop\n3) Toy Store ", 0.3f);
            var shopCh = Console.ReadKey().Key;
            Console.WriteLine();
            switch (shopCh)
            {
                case ConsoleKey.D1: //Weapons Shop
                    PlayerMovement(4, -3, "Weapons Shop", false);
                    while (Player.isTransaction == false)
                    {
                        Conversation("Friend", "Would you like to buy weapons or sell weapons?", 0.3f);
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        var buyOrSell = Console.ReadKey().Key;
                        Console.WriteLine();
                        switch (buyOrSell)
                        {
                            case ConsoleKey.D1: //Buy weapons
                                Conversation("Friend", "Welcome valued customer! What kind of weapons are you interested in today? \n1) Guns\n2) Melee\n3) Miscellaneous ", 0);
                                var typeWeapon = Console.ReadKey().Key;
                                Console.WriteLine();
                                switch (typeWeapon)
                                {
                                    case ConsoleKey.D1: //Guns
                                        Conversation("Info", "1) Handguns\n2) Shotguns\n3) Rifles\n4) Ammo", 0); Console.ForegroundColor = ConsoleColor.DarkBlue;
                                        var gunCh = Console.ReadKey().Key;
                                        Console.WriteLine();
                                        switch (gunCh) //Check what type of gun
                                        {
                                            case ConsoleKey.D1: //Hanguns
                                                BuyGun(0);
                                                break;
                                            case ConsoleKey.D2: //Shotguns
                                                BuyGun(1);
                                                break;
                                            case ConsoleKey.D3: //Rifles
                                                BuyGun(2);
                                                break;
                                            case ConsoleKey.D4: //Ammos
                                                Conversation("Friend", "What ammo would you like? \n1) Handgun\n2) Shotgun\n3) Rifle", 0);
                                                var ammoBuy = Console.ReadKey().Key;
                                                switch (ammoBuy)
                                                {
                                                    case ConsoleKey.D1:
                                                        Conversation("Friend", "How much handgun ammo do you want?", 1);
                                                        int ammoAmou = Convert.ToInt16(Console.ReadLine()); Console.WriteLine();
                                                        Gun.BuyAmmo("Gunshop Owner", 5, "Handgun", ammoAmou);
                                                        break;
                                                    case ConsoleKey.D2:
                                                        Conversation("Friend", "How much shotgun ammo do you want?", 1);
                                                        int ammoAmou1 = Convert.ToInt16(Console.ReadLine()); Console.WriteLine();
                                                        Gun.BuyAmmo("Gunshop Owner", 5, "Shotgun", ammoAmou1);
                                                        break;
                                                    case ConsoleKey.D3:
                                                        Conversation("Friend", "How much rifle ammo do you want?", 1);
                                                        int ammoAmou2 = Convert.ToInt16(Console.ReadLine()); Console.WriteLine();
                                                        Gun.BuyAmmo("Gunshop Owner", 5, "Rifle", ammoAmou2);
                                                        break;
                                                }
                                                break;
                                            default:
                                                Conversation("Info", "Wrong input", 1);
                                                break;
                                        }
                                        break;
                                    case ConsoleKey.D2: //Melee
                                        for (int i = 0; i < 5; i++) //Loop between and print names of melee weapons
                                        {
                                            Conversation("Info", $"{i + 1}. {MeleeArray[i].Name}", 0);
                                        }
                                        var meleeWeaponChoice = Console.ReadKey().Key;
                                        Console.WriteLine();
                                        switch (meleeWeaponChoice)
                                        {
                                            case ConsoleKey.D1:
                                                MeleeBuy(0);
                                                break;
                                            case ConsoleKey.D2:
                                                MeleeBuy(1);
                                                break;
                                            case ConsoleKey.D3:
                                                MeleeBuy(2);
                                                break;
                                            case ConsoleKey.D4:
                                                MeleeBuy(3);
                                                break;
                                            case ConsoleKey.D5:
                                                MeleeBuy(4);
                                                break;
                                            default:
                                                Conversation("Info", "Wrong input", 1);
                                                break;
                                        }
                                        break;
                                    case ConsoleKey.D3: //Miscellaneous
                                        for (int i = 0; i < 4; i++) //Loop between and print names of melee weapons
                                        {
                                            Conversation("Info", $"{i + 1}. {MiscArray[i].Name}", 0);
                                        }
                                        ConsoleKey miscWeaponChoice = Console.ReadKey().Key;
                                        Console.WriteLine();
                                        switch (miscWeaponChoice)
                                        {
                                            case ConsoleKey.D1:
                                                MiscBuy(0);
                                                break;
                                            case ConsoleKey.D2:
                                                MiscBuy(1);
                                                break;
                                            case ConsoleKey.D3:
                                                MiscBuy(2);
                                                break;
                                            case ConsoleKey.D4:
                                                MiscBuy(3);
                                                break;
                                            default:
                                                Conversation("Info", "Wrong input", 1);
                                                break;
                                        }
                                        break;
                                    default:
                                        Conversation("Info", "Wrong input", 1);
                                        break;
                                }
                                break;
                            case ConsoleKey.D2: //Sell weapons
                                if (WeaponsBase.AllWeapons.Length > 0)
                                {
                                    Conversation("Friend", "Which one would you like to sell?", 0.2f);
                                    for (int i = 1; i <= WeaponsBase.AllWeapons.Length; i++)
                                    {
                                        Conversation("Info", $"{i}. {WeaponsBase.AllWeapons[i - 1].Name}", 0f);
                                    }
                                    var sellWeaponChoice = Console.ReadKey().Key;
                                    Console.WriteLine();
                                    switch (sellWeaponChoice)
                                    {
                                        case ConsoleKey.D1:
                                            SellGun(0);
                                            break;
                                        case ConsoleKey.D2:
                                            SellGun(1);
                                            break;
                                        case ConsoleKey.D3:
                                            SellGun(2);
                                            break;
                                        case ConsoleKey.D4:
                                            SellGun(3);
                                            break;
                                        case ConsoleKey.D5:
                                            SellGun(4);
                                            break;
                                        case ConsoleKey.D6:
                                            SellGun(5);
                                            break;
                                        case ConsoleKey.D7:
                                            SellGun(6);
                                            break;
                                        case ConsoleKey.D8:
                                            SellGun(7);
                                            break;
                                        case ConsoleKey.D9:
                                            SellGun(8);
                                            break;
                                        case ConsoleKey.D0:
                                            SellGun(9);
                                            break;
                                    }
                                }
                                else
                                {
                                    Conversation("Info", "You do not have enough weapons to sell", 1);
                                }
                                break;
                            default:
                                Conversation("Info", "Wrong input", 0.3f);
                                break;
                        }
                    }
                    break;
                case ConsoleKey.D2: //Items Shop
                    PlayerMovement(-7, 0, "Items Shop", false);
                    while (Player.isTransaction == false)
                    {
                        Conversation("Friend", "Would you like to buy items or sell items?", 0.3f);
                        var buyOrSellItems = Console.ReadKey().Key; Console.WriteLine();
                        switch (buyOrSellItems)
                        {
                            case ConsoleKey.D1: //Buy items
                                Conversation("Friend", "Welcome valued customer! What kinds of items are you interested in today? \n1) Heals\n2) Buffs ", 0.3f);
                                var itemBuyCheck = Console.ReadKey().Key; Console.WriteLine();
                                switch (itemBuyCheck) //Check what kind of item
                                {
                                    case ConsoleKey.D1: //Heals
                                        for (int i = 0; i < 5; i++) //Loop between and print names of melee weapons
                                        {
                                            Conversation("Info", $"{i + 1}. {HealsArray[i].Name}", 0);
                                        }
                                        var healsChoice = Console.ReadKey().Key; Console.WriteLine();
                                        switch (healsChoice)
                                        {
                                            case ConsoleKey.D1:
                                                HealsBuy(0);
                                                break;
                                            case ConsoleKey.D2:
                                                HealsBuy(1);
                                                break;
                                            case ConsoleKey.D3:
                                                HealsBuy(2);
                                                break;
                                            case ConsoleKey.D4:
                                                HealsBuy(3);
                                                break;
                                            case ConsoleKey.D5:
                                                HealsBuy(4);
                                                break;
                                            default:
                                                Conversation("Info", "Wrong input", 1);
                                                break;
                                        }
                                        break;
                                    case ConsoleKey.D2: //Buffs
                                        for (int i = 0; i < 3; i++) //Loop between and print names of melee weapons
                                        {
                                            Conversation("Info", $"{i + 1}. {BuffsArray[i].Name}", 0);
                                        }
                                        var buffsChoice = Console.ReadKey().Key; Console.WriteLine();
                                        switch (buffsChoice)
                                        {
                                            case ConsoleKey.D1:
                                                BuffsBuy(0);
                                                break;
                                            case ConsoleKey.D2:
                                                BuffsBuy(1);
                                                break;
                                            case ConsoleKey.D3:
                                                BuffsBuy(2);
                                                break;
                                            case ConsoleKey.D4:
                                                BuffsBuy(3);
                                                break;
                                            case ConsoleKey.D5:
                                                BuffsBuy(4);
                                                break;
                                            default:
                                                Conversation("Info", "Wrong input", 1);
                                                break;
                                        }
                                        break;
                                    default:
                                        Conversation("Info", "Wrong input", 1);
                                        break;
                                }
                                break;
                            case ConsoleKey.D2: //Sell items
                                if (Object.Inventory.Length > 0)
                                {
                                    for (int i = 0; i < Object.Inventory.Length; i++)
                                    {
                                        Conversation("Player", $"{i + 1}. {Object.Inventory[i].Name}", 0);
                                    }
                                    var sellItemChoice = Console.ReadKey().Key; Console.WriteLine();
                                    switch (sellItemChoice)
                                    {
                                        case ConsoleKey.D1:
                                            SellItem(0);
                                            break;
                                        case ConsoleKey.D2:
                                            SellItem(1);
                                            break;
                                        case ConsoleKey.D3:
                                            SellItem(2);
                                            break;
                                        case ConsoleKey.D4:
                                            SellItem(3);
                                            break;
                                        case ConsoleKey.D5:
                                            SellItem(4);
                                            break;
                                        case ConsoleKey.D6:
                                            SellItem(5);
                                            break;
                                        case ConsoleKey.D7:
                                            SellItem(6);
                                            break;
                                        case ConsoleKey.D8:
                                            SellItem(7);
                                            break;
                                        case ConsoleKey.D9:
                                            SellItem(8);
                                            break;
                                        case ConsoleKey.D0:
                                            SellItem(9);
                                            break;
                                    }
                                }
                                else { Conversation("Info", "You do not have enough weapons", 1); }
                                break;
                            default:
                                Conversation("Hostile", "That's not an option.", 0.3f);
                                break;
                        }
                    } 
                    break;
                case ConsoleKey.D3: //Toy shop, has no effect on stats
                    PlayerMovement(5, 2, "Toy Shop", false);
                    Conversation("Friend", "Would you like to buy toys or sell toys?", 0.3f);
                    var buyOrSellToys = Console.ReadKey().Key; Console.WriteLine();
                    switch (buyOrSellToys)
                    {
                        case ConsoleKey.D1: //Buy toys
                            for (int i = 0; i < ToysArray.Length; i++)
                            {
                                Conversation("Info", $"{i + 1}. {ToysArray[0].Name}", 0f);
                            }
                            var toyBuyChoice = Console.ReadKey().Key; Console.WriteLine();
                            switch (toyBuyChoice)
                            {
                                case ConsoleKey.D1:
                                    ToysBuy(0);
                                    break;
                                case ConsoleKey.D2:
                                    ToysBuy(1);
                                    break;
                                case ConsoleKey.D3:
                                    ToysBuy(2);
                                    break;
                                case ConsoleKey.D4:
                                    ToysBuy(3);
                                    break;
                                case ConsoleKey.D5:
                                    ToysBuy(4);
                                    break;
                                default:
                                    Conversation("Info", "Wrong input", 0);
                                    break;
                            }
                            break;
                        case ConsoleKey.D2: //Sell toys
                            if (Toy.Toys.Length > 0)
                            {
                                for (int i = 0; i < Toy.Toys.Length; i++)
                                {
                                    Conversation("Player", $"{i + 1}. {Toy.Toys[i].Name}", 0);
                                }
                                var sellToyChoice = Console.ReadKey().Key; Console.WriteLine();
                                switch (sellToyChoice)
                                {
                                    case ConsoleKey.D1:
                                        SellToy(0);
                                        break;
                                    case ConsoleKey.D2:
                                        SellToy(1);
                                        break;
                                    case ConsoleKey.D3:
                                        SellToy(2);
                                        break;
                                    case ConsoleKey.D4:
                                        SellToy(3);
                                        break;
                                    case ConsoleKey.D5:
                                        SellToy(4);
                                        break;
                                    case ConsoleKey.D6:
                                        SellToy(5);
                                        break;
                                    case ConsoleKey.D7:
                                        SellToy(6);
                                        break;
                                    case ConsoleKey.D8:
                                        SellToy(7);
                                        break;
                                    case ConsoleKey.D9:
                                        SellToy(8);
                                        break;
                                    case ConsoleKey.D0:
                                        SellToy(9);
                                        break;
                                }
                            }
                            else {Conversation("Info", "You do not have enough toys", 1);}
                            break;
                        default:
                            Conversation("Hostile", "That's not an option.", 0.3f);
                            break;
                    }
                    break;
                default:
                    Conversation("Info", "Wrong input", 1);
                    break;
            }
        }
        if (Player.Money == 0)
        {
            Conversation("Self", "Broke ass bitch. Imagine having all your money gone ", 0);
        }
    }
    static void TimeButton(float time, int roundNo)
    {
        int playerName = RandomNumberGenerator.Next(3);
        switch (roundNo)
        {
            case 1:
                Conversation("Friend","The first round will begin at the end of this sentence, click after 12 seconds. All the best.",0);
                break;
            case 2:
                Conversation("Friend", "OOF that was a good first round, now the second round will begin after this sentence, click after 8 seconds. Do your worst :)", 0);
                break;
            case 3:
                Conversation("Friend", "WOAH that was an amazing play by our participants, now we move on to our final round. Click after 31.2 seconds :)", 0);
                break;
        }
        Stopwatch sW = new();
        sW.Start();
        while (Console.ReadKey().Key != ConsoleKey.X) {}
        sW.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = sW.Elapsed;
        float miliseconds = ts.Milliseconds;
        float miliInSeconds = miliseconds / 1000;
        float seconds = ts.Seconds + miliInSeconds; //x.y Float value of user time
        float accuracy = 1;
        if (Math.Abs(seconds - time) <= 3) //Accuracy is for displaying who got shocked and all
        {
            accuracy = -100 * (Math.Abs(seconds - time) - 3)/3;
        }
        else
        {
            accuracy = 0;
        }
        Console.WriteLine($"{seconds} seconds");
        if (time - 2 <= seconds && seconds <= time)
        {
            Player.Money += 50*(seconds + 2 - time)/3; //Math type shit
        }
        else if (seconds < time + 2 && seconds > time)
        {
            Player.Money += -100 * (seconds - time - 2)/6;
        }
        if (accuracy >= 95)
        {
            Conversation("Friend", "AYMAN GOT IT RIGHT!! All other players will now be shocked :)", 1f);
            Conversation("Hostile", "AAAAAAAAAAAAAAAA FF- SHITTTTTTTTT", 1);
        }
        else if (accuracy == 0)
        {
            Conversation("Friend", "OOF AYMAN WAS WAYY OFF! He's gonna get shocked real bad now!!", 0.3f);
            Conversation("Narrator", "You get shocked at 10000 volts for 1 second. You wish you could scream", 1);
        }
        else if (accuracy >= 0)
        {
            int chancePlayer = RandomNumberGenerator.Next(2);
            switch (chancePlayer)
            {
                case 0:
                    switch (playerName)
                    {
                        case 0:
                            Conversation("Friend", "ADIL GOT IT RIGHT!! All other players will now be shocked :)", 1);
                            Conversation("Narrator", "You get shocked at 10000 volts for 1 second. You wish you could scream", 1);
                            break;
                        case 1:
                            Conversation("Friend", "OUMAR GOT IT RIGHT!! All other players will now be shocked :)", 1);
                            Conversation("Narrator", "You get shocked at 10000 volts for 1 second. You wish you could scream", 1);
                            break;
                        case 2:
                            Conversation("Friend", "FABRIZIO GOT IT RIGHT!! All other players will now be shocked :)", 1);
                            Conversation("Narrator", "You get shocked at 10000 volts for 1 second. You wish you could scream", 1);
                            break;
                    }
                    break;
                default:
                    Conversation("Friend", "OOF everyone got it close this time! tch", 1);
                    break;
            }
        }
        switch(roundNo)
        {
            case 1:
                Conversation("Hostile", "Ay Adil, you heard of any agent among the prisoners? Heard the big guys in parliament are tryna get this place shut down.", 1);
                break;
            case 2:
                Conversation("Hostile", "No, I haven't come across such rumours, what about you Fabrizio?", 1);
                Conversation("Hostile", "No.", 1);
                Conversation("Hostile", "What 'bout you Ayma- Oh right my fau-", 0);
                break;
            case 3:
                if (accuracy >= 95)
                {
                    Conversation("Hostile", "AAAAAAAAAAAAAA NO FUCKING WAY YOU GOT IT I'M GONNA KILL YOU WHEN I SEE YOU NEXT AYMAN", 1);
                }
                else if(accuracy > 0)
                {
                    switch (playerName)
                    {
                        case 0:
                            Conversation("Hostile", "AAAAAAAAAAAAAA NO FUCKING WAY YOU GOT IT I'M GONNA KILL YOU WHEN I SEE YOU NEXT ADIL", 1);
                            break;
                        case 1:
                            Conversation("Hostile", "Easyyy", 1);
                            Conversation("Hostile", "Oumar I genuinely hate you. I, Fabrizio, shall no longer be friends with y-", 0);
                            break;
                        case 2:
                            Conversation("Hostile", "AAAAAAAAAAAAAA NO FUCKING WAY YOU GOT IT I'M GONNA KILL YOU WHEN I SEE YOU NEXT FABRIZIO" ,1);
                            break;
                    }
                }
                Conversation("Friend", "Haha that's what we call good ol' sportsmanship, anyways we hope you enjoyed dear viewers, join in tomorrow at the same time for more exciting games!", 1);
                break;
        }
    }
    static void PressX (string text)
    {
        Conversation("Self", text, 0.7f); 
        Console.ForegroundColor = ConsoleColor.Blue;
        while (Console.ReadKey().Key != ConsoleKey.X) { }
        Console.WriteLine();
    }
    //Functions to make the code easier to read
    static void EatFood(Food choice1, Food choice2, Food choice3)
    {
        while (Player.isTransaction == false)//food time nom nom nom
        {
            Conversation("Info", $"1. {choice1.Name} ({choice1.Cost}) coins\n2. {choice2.Name} ({choice2.Cost}) coins\n3. {choice3.Name} ({choice3.Cost}) coins", 0.3f);
            Console.ForegroundColor = ConsoleColor.Blue;
            var foodChoiceHall = Console.ReadKey().Key;
            Console.WriteLine();
            switch (foodChoiceHall)
            {
                case ConsoleKey.D1:
                    if (Player.Money >= choice1.Cost)
                    {
                        Conversation("Narrator", $"You point at the {choice1.Name}", 1);
                        choice1.ObjBuy(0, 0, "Chef");
                        choice1.HealFromFood();
                    }
                    else
                    {
                        Conversation("Info", "You do not have enough money.", 0.3f);
                    }
                    break;
                case ConsoleKey.D2:
                    if (Player.Money >= choice2.Cost)
                    {
                        Conversation("Narrator", $"You point at the {choice2.Name}", 1);
                        choice2.ObjBuy(0, 0, "Chef");
                        choice2.HealFromFood();
                    }
                    else
                    {
                        Conversation("Info", "You do not have enough money.", 0.3f);
                    }
                    break;
                case ConsoleKey.D3:
                    if (Player.Money >= choice3.Cost)
                    {
                        Conversation("Narrator", $"You point at the {choice3.Name}", 1);
                        choice3.ObjBuy(0, 0, "Chef");
                        choice3.HealFromFood();
                    }
                    else
                    {
                        Conversation("Info", "You do not have enough money.", 0.3f);
                    }
                    break;
                default:
                    Conversation("Self", "Give correct input", 1f);
                    Player.isTransaction = false;
                    break;
            }
        }
    }
    static void BuyGun(int indexofType)
    {
        for (int i = 0; i < 3; i++) //Loop between and print names of guns
        {
            Conversation("Info", $"{i + 1}. {GunArray[indexofType, i].Name}", 0);
        }
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        var handGunCh = Console.ReadKey().Key;
        Console.WriteLine();
        switch (handGunCh) //Check for user input for buying handgun
        {
            case ConsoleKey.D1:
                GunArray[0, 0].GunInfo();
                Conversation("Friend", $"Would you like to buy a {GunArray[indexofType, 0].Name}? ", 0);
                var buyGun1 = Console.ReadKey().Key;
                if (buyGun1 == ConsoleKey.X)
                {
                    GunArray[0, 0].BuyWeapon("Gunshop Owner", 5);
                }
                else
                {
                    Conversation("Hostile", "Choose something else then.", 0);
                }
                break;
            case ConsoleKey.D2:
                GunArray[0, 1].GunInfo();
                Conversation("Friend", $"Would you like to buy a {GunArray[indexofType, 1].Name}?", 0);
                var buyGun2 = Console.ReadKey().Key;
                if (buyGun2 == ConsoleKey.X)
                {
                    GunArray[0, 1].BuyWeapon("Gunshop Owner", 5);
                }
                else
                {
                    Conversation("Hostile", "Choose something else then.", 0);
                }
                break;
            case ConsoleKey.D3:
                GunArray[0, 2].GunInfo();
                Conversation("Friend", $"Would you like to buy a {GunArray[indexofType, 2].Name}?", 0);
                var buyGun3 = Console.ReadKey().Key;
                if (buyGun3 == ConsoleKey.X)
                {
                    GunArray[0, 2].BuyWeapon("Gunshop Owner", 5);
                }
                else
                {
                    Conversation("Hostile", "Choose something else then.", 0);
                }
                break;
        }
    }
    static void HealsBuy(int index)
    {
        HealsArray[index].ObjInfo(false);
        Conversation("Friend", $"Would you like to buy a {HealsArray[index].Name}? ", 0);
        if (Console.ReadKey().Key == ConsoleKey.X)
        {
            HealsArray[index].ObjBuy(0, 5, "Items Shop Owner");
        }
    }
    static void BuffsBuy(int index)
    {
        BuffsArray[index].ObjInfo(false);
        Conversation("Friend", $"Would you like to buy a {BuffsArray[index].Name}? ", 0);
        if (Console.ReadKey().Key == ConsoleKey.X)
        {
            BuffsArray[index].ObjBuy(0, 5, "Items Shop Owner");
        }
    }
    static void ToysBuy(int index)
    {
        ToysArray[index].ObjInfo(false);
        Conversation("Friend", $"Would you like to buy a {ToysArray[index].Name}? ", 0);
        if (Console.ReadKey().Key == ConsoleKey.X)
        {
            ToysArray[index].ObjBuy(0, 5, "Items Shop Owner");
        }
    }
    static bool GunChange(int index, Gun name)
    {
        bool isDone = false;
        if (Gun.Guns.Length <= index + 1)
        {
            Gun.Guns[index].GunInfo();
            isDone = true;
        }
        else
        {
            Conversation("Info", "You do not have that many guns", 0f);
        }
        return isDone;
    }
    static void MeleeBuy(int index)
    {
        MeleeArray[index].MeleeInfo();
        Conversation("Friend", $"Would you like to buy a {MeleeArray[index].Name}? ", 0);
        if (Console.ReadKey().Key == ConsoleKey.X)
        {
            MeleeArray[index].BuyWeapon("Weapons Shop Owner", 5);
        }
    }
    static void MiscBuy(int index)
    {
        MiscArray[index].MiscInfo();
        Conversation("Friend", $"Would you like to buy a {MiscArray[index].Name}? ", 0);
        if (Console.ReadKey().Key == ConsoleKey.X)
        {
            MiscArray[index].BuyWeapon("Weapons Shop Owner", 5);
        }
    }
    static void SellGun(int index)
    {
        if (WeaponsBase.AllWeapons.Length >= index + 1)
        {
            Conversation("Friend", $"Would you like to sell a {WeaponsBase.AllWeapons[index].Name}? ", 0);
            if (Console.ReadKey().Key == ConsoleKey.X)
            {
                WeaponsBase.AllWeapons[index].SellWeapon("Weapons Shop", 5);
            }
        }
        else
        {
            Conversation("Hostile", "That's not an option.", 0.5f);
        }
    }
    static void SellItem(int index)
    {
        if (Object.Inventory.Length >= index + 1)
        {
            Conversation("Friend", $"Would you like to sell a {Object.Inventory[index].Name}? ", 0);
            if (Console.ReadKey().Key == ConsoleKey.X)
            {
                Object.Inventory[index].ObjSell(5, 0, "Items shop owner");
            }
            else
            {
                Conversation("Hostile", "Stop wasting my time.", 0.5f);
            }
        }
        else
        {
            Conversation("Hostile", "That's not an option.", 0.5f);
        }
    }
    static void SellToy(int index)
    {
        if (Toy.Toys.Length >= index + 1)
        {
            Conversation("Friend", $"Would you like to sell a {Toy.Toys[index].Name}? ", 0);
            if (Console.ReadKey().Key == ConsoleKey.X)
            {
                Toy.Toys[index].ObjSell(5, 0, "Toys shop owner");
            }
            else
            {
                Conversation("Hostile", "Stop wasting my time.", 0.5f);
            }
        }
        else
        {
            Conversation("Hostile", "That's not an option.", 0.5f);
        }
    }
    //Location of every place in prison
    
    //Listing down every item in game + info
    public static readonly Random RandomNumberGenerator = new();
    //1. Pistols
    static readonly Gun Beretta21ABobcat = new() { Ammo = 21, dePerM = 30 / 40, magazinesPerClick = 1, Range = 19, damagePerBullet = 30, fireRate = 0.5f, Type = "Handgun", reloadTime = 5, Cost = 120, Desc = "It is user-Friend, durable, reliable and accurate, while with its snag-free lines is can be tucked in any kind of holster or pocket for deep concealment", Name = "Beretta 21A Bobcat", Mag = 7 }; //Pew pew
    static readonly Gun HiPoint45Pistol = new() { Ammo = 27, Cost = 180, damagePerBullet = 40, Desc = "The Hi-Point Firearms .45 ACP Pistol features a 4.5' barrel and a polymer grip. The double-action pistol has a 9-round magazine capacity.", fireRate = 0.8f, Mag = 9, magazinesPerClick = 1, Name = "Hi-Point Firearms .45 ACP Pistol", Range = 23, reloadTime = 2.3f, dePerM = 40 / 23, Type = "Pistol" };
    static readonly Gun TaurusRestrikePistol = new() { Ammo = 36, Cost = 280, damagePerBullet = 50, Desc = "Designed with concealed carry in mind, the Taurus G2C SS Restrike 9mm Pistol boasts rugged, compact polymer frame with a streamlined, ergonomic design and features a 3.2-inch barrel with a matte stainless finish.", fireRate = 0.5f, Mag = 12, magazinesPerClick = 1, Name = "Taurus G2C SS Restrike 9mm Pistol", Range = 30, reloadTime = 2.1f, Type = "Pistol", dePerM = 50 / 30 };
    //2. Shotguns
    static readonly Gun BenelliNovaShotgun = new() { Ammo = 30, dePerM = 41 / 10, fireRate = 1, Type = "Shotgun", magazinesPerClick = 2, Range = 10, reloadTime = 4.5f, Cost = 135, Name = "Benelli Nova Pump-Action Shotgun", Desc = "The Nova is an ultra reliable pump shotgun that's ideal for all-round rugged use.", damagePerBullet = 23, Mag = 5 };
    static readonly Gun MossbergAllPurposeShotgun = new() { Ammo = 36, Cost = 240, damagePerBullet = 30, Desc = "The Maverick 88 shotgun's design incorporates dual extractors that take a solid grip on the cartridge's rim, dual action bars to ensure reliable cycling of the action without binding or twisting, and an anti-jam shell elevator.", fireRate = 1.2f, Mag = 6, magazinesPerClick = 3, Name = "Mossberg Maverick Pump-Action Shotgun", Range = 13, reloadTime = 4f, Type = "Shotgun", dePerM = 30 / 13 };
    static readonly Gun TriStarPumpShotgun = new() { Ammo = 42, Cost = 250, damagePerBullet = 32, Desc = "This pump shotgun boasts a beautiful walnut stock and forearm to stand up to field use, while a rubber recoil pad minimizes felt recoil.", fireRate = 1f, Mag = 6, magazinesPerClick = 3, Name = "TriStar Pump-Action Shotgun", Range = 16, reloadTime = 3.7f, Type = "Shotgun", dePerM = 32 / 16 };
    //3. Rifles
    static readonly Gun MossbergBoltActionRifle = new() { Ammo = 10, damagePerBullet = 85, fireRate = 3, Cost = 230, Desc = "With suppressor-ready threaded barrels; optics-ready picatinny rail, and built on the powerful and proven Patriot chassis, a smooth user-adjustable trigger and oversized bolt handle, this rifle offers unmatched accuracy and dependability.", Mag = 2, magazinesPerClick = 1, Name = "Mossberg Bolt Action Rifle", Type = "Rifle", Range = 80, dePerM = 85 / 80, reloadTime = 5 };
    static readonly Gun SavageSemiAutoRifle = new() { Ammo = 20, damagePerBullet = 90, fireRate = 2, Cost = 140, Desc = "The Savage 64 F .22 LR Rimfire Semiautomatic Rifle features a synthetic stock and comes with a 10-round, detachable magazine.", Mag = 10, magazinesPerClick = 1, Name = "Savage SemiAutomatic Rifle", Type = "Rifle", Range = 100, dePerM = 90 / 100, reloadTime = 4 };
    static readonly Gun SavageWinchesterRifle = new() { Ammo = 24, damagePerBullet = 93, fireRate = 1.8f, Cost = 400, Desc = "Take aim with the Savage Axis .243 Winchester Rifle. This centerfire rifle features a 4-round capacity and a carbon-steel barrel, and it's equipped with Weaver-style rings and bases. ", Mag = 4, magazinesPerClick = 1, Name = "Savage Winchester Rifle", Type = "Rifle", Range = 210, dePerM = 93 / 210, reloadTime = 3.5f };
    //4. Melee Weapons
    static readonly Melee BowieKnife = new() { Name = "Bowie Knife", Cost = 100, Desc = "This knife is a large, independent survival implement with a 12-inch D2 steel blade, solid brass guard, and ebony pakkawood handle.", Damage = 53, meleeType = "Sharp", Length = 0.4f };
    static readonly Melee MetalPipe = new() { Name = "Metal Pipe", Cost = 54, Desc = "This metal pipe fell from the ceiling of the shop one day, it's pretty sturdy and can seriously harm a person or two ahem", Damage = 60, meleeType = "Blunt", Length = 1.4f };
    static readonly Melee BigAssSword = new() { Name = "Sword", Cost = 101, Desc = "This sword stands a huge 1.82m, seems to be shorter than you though.", Damage = 94, meleeType = "Sharp", Length = 1.82f };
    static readonly Melee TinyAxe = new() { Name = "Tiny Axe", Cost = 24, Desc = "This axe is a tiny 34cm but packs a huge punch, it's main purpose is for concealment.", Damage = 49, meleeType = "Sharp", Length = 0.34f };
    static readonly Melee BigAxe = new() { Name = "Big Axe", Cost = 42, Desc = "This axe stands at a huge 110cm but packs a huge punch, it's main purpose is for forestry and (murder).", Damage = 60, meleeType = "Sharp", Length = 1.1f };
    //5. Miscellaneous Weapons
    static readonly MiscWeapon StrengthNerf = new() { Name = "Strength Nerfer", Cost = 75, Desc = "Nerfs the strength of an enemy in a fight by 10" };
    static readonly MiscWeapon HPNerf = new() { Name = "HP Nerfer", Cost = 75, Desc = "Nerfs the hp of an enemy in a fight by 20" };
    static readonly MiscWeapon SpeedNerf = new() { Name = "Speed Nerfer", Cost = 60, Desc = "Nerfs the speed of an enemy in a fight by 10" };
    static readonly MiscWeapon StaminaNerf = new() { Name = "Stamina Nerfer", Cost = 20, Desc = "Nerfs the strength of an enemy in a fight by 10" };
    //6. Heals
    static readonly Heal mysteriousPotion = new() { Name = "Mysterious Potion", Colour = "Green", Cost = 70, healAmou = RandomNumberGenerator.Next(46, 60), isBuyable = true, isUsable = true, Level = 31, Owner = "Items Shop Owner", Usage = $"You have used mysterious potion and have healed for a mysterious amount\nPlayer HP: {Player.HP}", Description = "This mysterious potion heals you for a mysterious amount of HP" };
    static readonly Heal Soda = new() { Name = "Soda", Colour = "Black", Cost = 20, healAmou = 23, isBuyable = true, isUsable = true, Level = 12, Owner = "Items Shop Owner", Usage = $"You drank soda and healed for 23HP\nPlayer HP: {Player.HP}", Description = "Yum" };
    static readonly Heal Medicine = new() { Name = "Medicine", Colour = "Pink", Cost = 35, healAmou = 32, isBuyable = true, isUsable = true, Level = 10, Owner = "Items Shop Owner", Usage = $"You ate medicine and healed for 32HP\nPlayer HP: {Player.HP}", Description = "Using 18th century medicinal practices, this medicine is guaranteed to maybe heal you!" };
    static readonly Heal Bandages = new() { Name = "Bandages", Colour = "White", Cost = 12, healAmou = 15, isBuyable = true, isUsable = true, Level = 10, Owner = "Items Shop Owner", Usage = $"You wrapped a bandage around you and healed for 15HP\nPlayer HP: {Player.HP}", Description = "These bandages are sometimes clean" };
    static readonly Heal Syringe = new() { Name = "Health Syringe", Colour = "Transparent", Cost = 14, healAmou = 20, isBuyable = true, isUsable = true, Level = 10, Owner = "Items Shop Owner", Usage = $"You inject a health syringe into your arm and heal for 20HP\nPlayer HP: {Player.HP}", Description = "These syringes are easy to make and use" };
    //7. Buffs
    static readonly Buff strengthBuff = new() { buffAmount = 15, buffType = "Strength", Colour = "Orange", Cost = 30, Description = "Increases strength by 15", handicapAmount = 10, handicapType = "HP", isBuyable = true, isHandicap = true, isUsable = true, Level = 1, Name = "Strength Buff", Owner = "Items Shop Owner", sellValue = 20, Usage = "You have used strength buff and increased strength by 15 temporarily" };
    static readonly Buff speedBuff = new() { buffAmount = 10, buffType = "Speed", Colour = "Green", Cost = 25, Description = "Increases speed by 10", handicapAmount = 0, handicapType = "", isBuyable = true, isHandicap = false, isUsable = true, Level = 1, Name = "HP Buff", Owner = "Items Shop Owner", sellValue = 15, Usage = "You have used speed buff and increased speed by 10 temporarily" };
    static readonly Buff staminaBuff = new() { buffAmount = 15, buffType = "Stamina", Colour = "Brown", Cost = 25, Description = "Increases stamina by 15", handicapAmount = 0, handicapType = "", isBuyable = true, isHandicap = false, isUsable = true, Level = 1, Name = "Stamina Buff", Owner = "Items Shop Owner", sellValue = 10, Usage = "You have used stamina buff and increased stamina by 15 temporarily" };
    //8. Toys
    static readonly Toy tempName2 = new() { };
    //9. NPCs
    static readonly NPC Alejandro = new() { isFriend = true, hasRumor = false, isSeller = false, npcAge = 64, npcHP = 216, npcJob = "Labourer", npcLvl = 47, npcName = "Alejandro", npcRelationship = 0, npcStrength = 23, Speed = 17 };
    static readonly NPC Pedro = new() { npcAge = 28, hasRumor = false, isHostile = true, npcHP = 180, npcJob = "Washer", npcLvl = 41, npcRelationship = 1, npcStrength = 19, Speed = 26, npcName = "Pedro" };
    //10. PrisonObjects
    static readonly PrisonObject jcToilet = new() { isBuyable = false, Colour = "White", HP = 210, Level = 1, Name = "Toilet", Owner = "Old Man", Description = "Actually well kept", isUsable = true, Usage = "You have taken a piss", Cost = 0, isSellable = false };
    static readonly PrisonObject WashBasin = new() { isBuyable = false, Colour = "White", HP = 143, Level = 7, Name = "Washbasin", Owner = "Old Man", Description = "Old man really likes the wash basin for some reason", Cost = 0, isSellable = false, isUsable = true, Usage = "You have washed your hands" };
    static readonly PrisonObject BunkBed = new() { isBuyable = false, Colour = "Blue", HP = 80, Level = 1, Name = "Bunk Bed", Owner = "Old Man", Description = "Old man sleeps on bottom.", Cost = 0, isSellable = false, isUsable = true, Usage = "You have laid down" };
    static readonly PrisonObject JailWalls = new() { isBuyable = false, Colour = "Cream", HP = 430, Level = 13, Name = "Jail Cell 24A Walls", Owner = "Old Man", Description = "Old man tried to break it", Cost = 0, isSellable = false, isUsable = true, Usage = "You have looked at the wall, it looks nice" };
    static readonly PrisonObject jcClock = new() { isBuyable = false, Colour = "Yellow", HP = 30, Level = 1, Name = "Clock", Owner = "Old Man", Cost = 0, isSellable = false, Description = "Looks really old", isUsable = true, Usage = "The time is 20:38" };
    static readonly PrisonObject trash = new() { isBuyable = false, Colour = "Gray", HP = 2, Level = 1, Name = "Trash", Owner = "Nobody", Cost = 0, isSellable = false, Description = "Looks disgusting", isUsable = true, Usage = "You have cleaned it up" };
    static readonly PrisonObject day1208gameCoupon = new() { isBuyable = true, Colour = "White", HP = 1, Level = 1, Name = "Game Coupon", Owner = "Them", Cost = 100, isSellable = false, Description = "Used for participating in games", isUsable = true, Usage = "You have looked at tomorrow's game coupon" };
    static readonly PrisonObject day1207workCoupon = new() { isBuyable = true, Colour = "White", HP = 1, Level = 1, Name = "Work Coupon", Owner = "Them", Cost = 50, isSellable = false, Description = "Used for working", isUsable = true, Usage = "You have looked at today's work coupon" };
    //11. Food
    static readonly Food MuttonBiryani = new() { Name = "Mutton Biryani", Cost = 16, isBuyable = true, Colour = "White with red meat", Usage = "You have eaten mutton biryani", isUsable = true, Owner = "Food Stall Owner", Level = 1, healFromFoodAmou = 70 };
    static readonly Food LiteralAcid = new() { Name = "Literal pH 5 Acid", Cost = 1, isBuyable = true, Colour = "Green with bits of literal hellspawn", Usage = "You have ingested literal acid for reasons I am not able to decipher", isUsable = true, Owner = "Food Stall Owner", Level = 1, healFromFoodAmou = -3 };
    static readonly Food HoneyChickenThighs = new() { Name = "Honey Chicken Thighs", Cost = 18, isBuyable = true, Colour = "Orange with Yellow honey on it", Usage = "You have eaten Honey Chicken Thighs", isUsable = true, Owner = "Food Stall Owner", Level = 1, healFromFoodAmou = 60 };
    static readonly Food Waffle = new() { Name = "Waffle", Cost = 13, isBuyable = true, Colour = "Light Brown", isUsable = true, Usage = "You have eaten waffles", Owner = "Food Stall Owner", Level = 1, healFromFoodAmou = 55 };
    static readonly Food Pancake = new() { Name = "Pancake", Cost = 8, isBuyable = true, Colour = "Light Brown", isUsable = true, Usage = "You have eaten pancakes", Owner = "Food Stall Owner", Level = 1, healFromFoodAmou = 50 };
    static readonly Food Porridge = new() { Name = "Porridge", Cost = 2, isBuyable = true, Colour = "Light brown", isUsable = true, Usage = "You have eaten porridge", Owner = "Food Stall Owner", Level = 1, healFromFoodAmou = 50 };
    //Arrays of every item in game
    public static Gun[,] GunArray = { { Beretta21ABobcat, HiPoint45Pistol, TaurusRestrikePistol }, { BenelliNovaShotgun, MossbergAllPurposeShotgun, TriStarPumpShotgun }, { MossbergBoltActionRifle, SavageSemiAutoRifle, SavageWinchesterRifle } }; //Two Dimensional Array with all the guns, 0 is handguns, 1 is shotguns, 2 is rifles.
    public static Melee[] MeleeArray = { BowieKnife, MetalPipe, BigAssSword, TinyAxe, BigAxe };
    public static MiscWeapon[] MiscArray = { StrengthNerf, HPNerf, SpeedNerf, StaminaNerf };
    public static Heal[] HealsArray = { mysteriousPotion, Soda, Medicine, Bandages, Syringe };
    public static Buff[] BuffsArray = { strengthBuff, staminaBuff, speedBuff };
    public static Toy[] ToysArray = { tempName2 };
}
