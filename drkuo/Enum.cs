using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drkuo
{
    public enum Direction
    {
        // these make you run, no reason we wil lwant to walk for now
        // add walk/run bool later
        North = 0x00,
        NorthEast = 0x01,
        East = 0x02,
        SouthEast = 0x03,
        South = 0x04,
        SouthWest = 0x05,
        West = 0x06,
        NorthWest = 0x07
    }
    public enum FlagByte
    {
        Poisoned = 0x04,
        Movable = 0x20, //If normally not
        Warmode = 0x40,
        Hidden = 0x80
    }
            public enum Spell
    {
        CreateFood = 2,
        Feeblemind = 3,
        Heal = 4,
        MagicArrow = 5,
        NightSight = 6,
        ReactiveArmor = 7,
        Weaken = 8,
        Agility = 9,
        Cunning = 10,
        Cure = 11,
        Harm = 12,
        MagicTrap = 13,
                MagicUntrap = 14,
                Protection = 15,
                Strength = 16,
                Bless = 17,
                Fireball = 18,
                MagicLock = 19,
                Poison = 20,
                Telekenisis = 21,
                Teleport = 22,
                Unlock = 23,
                WallOfStone = 24,
                ArchCure = 25,
                ArchProtection = 26,
                Curse = 27,
                FireField = 28,
                GreaterHeal = 29,
                Lightning = 30,
                ManaDrain = 31,
                Recall = 32,
                BladeSpirit = 33,
                DispelField = 34,
                Incognito = 35,
                Reflection = 36,
                MindBlast = 37,
                Paralyze = 38,
                PoisonField = 39,
                SummonCreature = 40,
                Dispel = 41,
                EnergyBolt = 42,
                Explosion = 43,
                Invisibility = 44,
                Mark = 45,
                MassCurse = 46,
                ParalyzeField = 47,
                Reveal = 48,
                ChainLightning = 49,
                EnergyField = 50,
                FlameStrike = 51,
                Gate = 52,
                ManaVampire = 53,
                MassDispel = 54,
                MeteotShower = 55,
                Polymorph = 56,
                Earthquake = 57,
                EnergyVortex = 58,
                Ressurection = 59,
                SummonAirElemental = 60,
                SummonDaemon = 61,
                SummonEarthElemental = 62,
                SummonFireElemental = 63,
                SummonWaterElemental = 64
    }
            public enum CursorTarget
            {
                SelectObject = 0,
                SelectXYZ = 1
            }
            public enum Facet
            {
                Fel = 0,
                Tram = 1,
                Ilsh = 2,
                Malas = 3,
                Tokuno = 4
            }
    public enum Skill
    {
        Anatomy = 1,
        AnimalLore = 2,
        ItemID = 3,
        ArmsLore = 4,
        Begging = 6,
        Peacemaking = 9,
        Cartography = 12,
        DetectHidden = 14,
        Entice = 15,
        EvalInt = 16,
        ForensicEval = 19,
        Hiding = 21,
        Provocation = 22,
        Inscription = 23,
        Poisoning = 30,
        SpiritSpeak = 32,
        Stealing = 33,
        Taming = 35,
        TasteID = 36,
        Tracking = 38
    }
}
