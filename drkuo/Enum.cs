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
        Cunning = 10
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
