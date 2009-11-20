using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;

namespace drkuo
{
    class Script
    {
        uonetwork uonet;
        
        public Script(uonetwork muonet)
        {
            uonet = muonet;
        }

        public void main()
        {
            uonet.displaywipe();
            //uoobject tempob = (uoobject)uonet.GameObjects[3];
            uonet.display("Script Started!");
            Event Events = new Event(uonet);
            uoobject result = Events.Finditem(400);
            //Events.Cast(Spell.Agility);
           // while (true)
           // {

                Events.UseSkill(Skill.AnimalLore);
                while (uonet.UOClient.TargCurs == 0) { Thread.Sleep(10); }
                
               // Events.Target(
           // }
                //Events.Move(3503, 2580, 0, 5);
           // uonet.Send(Packets.Send.Packets.MoveRequestPacket(Direction.West,0,0));
           // Events.UseSkill(Skill.AnimalLore);
            //Thread.Sleep(500);
            //Events.UseSkill(Skill.Tracking);
            uonet.display("Script Ended!");
        }


    }

    class Event
    {
        uonetwork uonet;
        private System.Timers.Timer myTimer;
        private System.Timers.Timer myTimer2;
        public Event(uonetwork muonet)
        {
            uonet = muonet;
        }
        public uoobject Finditem(int Type)
        {
            uoobject myobj = new uoobject();
            foreach (DictionaryEntry Item in uonet.GameObjects)
            {
                uoobject mytemp = (uoobject)Item.Value;
                if (mytemp.type == Type)
                {
                    myobj = mytemp;
                    break;
                }
            }
            
            return myobj;
        }
        public void UseSkill(Skill skill)
        {
            int mskill = Convert.ToInt32(skill);
            uonet.Send(Packets.Send.Packets.useSkill(mskill));
        }
        public void Cast(Spell spell)
        {
            int mcast = Convert.ToInt32(spell);
            uonet.Send(Packets.Send.Packets.Cast(mcast));
        }
        public void Target(int X, int Y, int Z, int ID, int Model)
        {
            uonet.Send(uonet.ClickTargetPacket(ID, X, Y, Z, Model, CursorTarget.SelectObject));

            uonet.UOClient.TargCurs = 0;
        }
        public void Move(int x, int y, int precision, int timeout)
        {
            uonet.display("Moving From" + uonet.player.CharPosX + "/" + uonet.player.CharPosY + " To " + x + "/" + y);
            
            myTimer = new System.Timers.Timer(timeout * 1000);
            myTimer.Enabled = true;
            myTimer.AutoReset = false;
            myTimer.Elapsed+=new System.Timers.ElapsedEventHandler(myTimer_Elapsed);
            int seq = 0;
            int myx = uonet.player.CharPosX;
            int myy = uonet.player.CharPosY;
            while(((x != myx) | (y != myy)) & myTimer.Enabled)
            {          
                myx = uonet.player.CharPosX;
                myy = uonet.player.CharPosY;
                Direction direction = getDirection(x, y, myx, myy);
                uonet.Send(uonet.MoveRequestPacket(direction, seq, 0, true));
                Thread.Sleep(210);// fix this, 190 is min between steps, this static method will break on latency > local server TEST only
                if (seq != uonet.seq) { uonet.Send(Packets.Send.Packets.resync()); }
                    seq = seq + 1;
                    if (seq == 256) { seq = 1; }
            }
            if (!myTimer.Enabled)
            {
                uonet.display("Move Timed Out");
            }
        }
        public void myTimer_Elapsed(object source, ElapsedEventArgs e)
        {
            myTimer.Enabled = false;
        }

        private Direction getDirection(int x, int y, int myx, int myy)
        {
            Direction facing;
            if (x < myx)
            {
                if (y < myy)
                {
                    facing = Direction.NorthWest;
                }
                else if (y > myy)
                {
                    facing = Direction.SouthWest;
                }
                else
                {
                    facing = Direction.West;
                }
            }
            else if (x > myx)
            {
                if (y < myy)
                {
                    facing = Direction.NorthEast;
                }
                else if (y > myy)
                {
                    facing = Direction.SouthEast;
                }
                else
                {
                    facing = Direction.East;
                }
            }
            else
            {
                if (y < myy)
                {
                    facing = Direction.North;
                }
                else if (y > myy)
                {
                    facing = Direction.South;
                }
                else
                {
                    // We should never reach this.
                    facing = (Direction)0xFF;
                }
            }
            return facing;
        }

      }
    }

