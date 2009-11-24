using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace drkuo
{
    class uoclientvars
    {
        public int TargCurs;
        public int LTargetID;
        public int LTargetX;
        public int LTargetY;
        // Model must be set to 0 for map/landscape or the type/graphic if its a static tile
        public int LTargetModel;
        public int LTargetZ;
        public int CursorID; // internal value, it links sent/received target msgs
        public int CursorTarget; // 0/1 for object/ground we auto send what the server wants to keep shit simple
        public int EnemyID;
        public Boolean Combat;

        

    }

    class gump
    {
        public int ID;
        public int GumpID;
        public int X;
        public int Y;
        public int Z;
        public String Text;
        public String Commands;
    }
    class uoplayer
    {
        public int tempx;
            public int tempy; // temp movement fix
            public int seq; // temp
    public int CharID;
    public int CharType;
    public int CharPosX;
    public int CharPosY;
    public int CharPosZ;
    public int MaxHits;
    public int MaxMana;
    public int MaxStam;
    public int Hits;
    public int Mana;
    public int Stamina;
    public int Gold;
    public int Weight;
    public int MaxWeight;
    public String CharName;
    public int Sex;
    public int Int;
    public int Str;
    public int Dex;
    public int flags;
    public int hue;
    public int BackpackID;
    public bool WarMode;
    public int Facet;
    public int Direction;

    }
    class uoobject
    {
        public int serial;
        public int type;
        public int x;
        public int y;
        public int z;
        public int color;
        public int stack;
        public int flags;
    }
}
