/**************************************************************************\
*                                                                          *
*  DrkUO -- A text Based UO CLient                        *
*  Copyright (C) 2009  James 'DarkLotus, Belgeran' Kidd                            *
*  Some GPL Code written by Mikel Duke and Timo 'BtbN' Rothenpieler                                                                        *
*  This program is free software: you can redistribute it and/or modify    *
*  it under the terms of the GNU General Public License as published by    *
*  the Free Software Foundation, either version 3 of the License, or       *
*  (at your option) any later version.                                     *
*                                                                          *
*  This program is distributed in the hope that it will be useful,         *
*  but WITHOUT ANY WARRANTY; without even the implied warranty of          *
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the           *
*  GNU General Public License for more details.                            *
*                                                                          *
*  You should have received a copy of the GNU General Public License       *
*  along with this program.  If not, see <http://www.gnu.org/licenses/>.   *
*                                                                          *
\**************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Ultima;
        
namespace drkuo
{
    public partial class drkGui : Form
    {
        
        private Dictionary<String, GameVariable[]> variableCategs;
        private class GameVariable
        {
            public String name;
            public TreeNode node;
        }

        uonetwork uonet;
        Script myscript;
        private Thread mythread;
        private String ip;
        private int port;
        private String user;
        private String pass;
        
        public drkGui()
        {

            InitializeComponent();
           
            setuptreeview();
        }



        private void setuptreeview()
        {
            //Timo 'BtbN' Rothenpieler Code
                variableCategs = new Dictionary<String, GameVariable[]>
            {
                {
                    "Client",
                    new GameVariable[]
                    {
                        //new GameVariable { name = "CliCnt", node = null },
                        //new GameVariable { name = "CliLang", node = null },
                       // new GameVariable { name = "CliLogged", node = null },
                        //new GameVariable { name = "CliNr", node = null },
                        //new GameVariable { name = "CliLeft", node = null },
                       // new GameVariable { name = "CliTop", node = null },
                        //new GameVariable { name = "CliVer", node = null },
                       // new GameVariable { name = "CliXRes", node = null },
                        //new GameVariable { name = "CliYRes", node = null }
                    }
                },
                {
                    "Character",
                    new GameVariable[]
                    {
                        new GameVariable { name = "CharPosX", node = null },
                        new GameVariable { name = "CharPosY", node = null },
                        new GameVariable { name = "CharPosZ", node = null },
                        new GameVariable { name = "CursKind", node = null },
                        new GameVariable { name = "CharDir", node = null },
                        new GameVariable { name = "BackpackID", node = null },
                        new GameVariable { name = "CharID", node = null },
                        new GameVariable { name = "CharName", node = null },
                        new GameVariable { name = "CharStatus", node = null },
                        new GameVariable { name = "CharType", node = null },
                        new GameVariable { name = "Sex", node = null },
                    }
                },
                {
                    "Status",
                    new GameVariable[]
                    {
                        new GameVariable { name = "Str", node = null },
                        new GameVariable { name = "Dex", node = null },
                        new GameVariable { name = "Int", node = null },
                        new GameVariable { name = "MaxStats", node = null },
                        new GameVariable { name = "Hits", node = null },
                        new GameVariable { name = "MaxHits", node = null },
                        new GameVariable { name = "Stamina", node = null },
                        new GameVariable { name = "MaxStam", node = null },
                        new GameVariable { name = "Mana", node = null },
                        new GameVariable { name = "MaxMana", node = null },
                        new GameVariable { name = "MaxFol", node = null },
                        new GameVariable { name = "Followers", node = null },
                        new GameVariable { name = "MinDmg", node = null },
                        new GameVariable { name = "MaxDmg", node = null },
                        new GameVariable { name = "Weight", node = null },
                        new GameVariable { name = "MaxWeight", node = null },
                        new GameVariable { name = "Luck", node = null },
                        new GameVariable { name = "Gold", node = null },
                        new GameVariable { name = "AR", node = null },
                        new GameVariable { name = "PR", node = null },
                        new GameVariable { name = "FR", node = null },
                        new GameVariable { name = "CR", node = null },
                        new GameVariable { name = "ER", node = null },
                        new GameVariable { name = "TP", node = null },
                    }
                },
                {
                    "Last Action",
                    new GameVariable[]
                    {
                        new GameVariable { name = "LObjectID", node = null },
                        new GameVariable { name = "LObjectType", node = null },
                        new GameVariable { name = "LTargetID", node = null },
                        new GameVariable { name = "LTargetKind", node = null },
                        new GameVariable { name = "LTargetTile", node = null },
                        new GameVariable { name = "LTargetX", node = null },
                        new GameVariable { name = "LTargetY", node = null },
                        new GameVariable { name = "LTargetZ", node = null },
                        new GameVariable { name = "LLiftedID", node = null },
                        new GameVariable { name = "LLiftedKind", node = null },
                        new GameVariable { name = "LLiftedType", node = null },
                        new GameVariable { name = "LSkill", node = null },
                        new GameVariable { name = "LSpell", node = null },
                    }
                },
                {
                    "Container Info",
                    new GameVariable[]
                    {
                        new GameVariable { name = "ContID", node = null },
                        new GameVariable { name = "ContName", node = null },
                        new GameVariable { name = "ContKind", node = null },
                        new GameVariable { name = "ContType", node = null },
                        new GameVariable { name = "ContPosX", node = null },
                        new GameVariable { name = "ContPosY", node = null },
                        new GameVariable { name = "ContSizeX", node = null },
                        new GameVariable { name = "ContSizeY", node = null },
                        new GameVariable { name = "NextCPosX", node = null },
                        new GameVariable { name = "NextCPosY", node = null }
                    }
                },
                {
                    "Misc",
                    new GameVariable[]
                    {
                        new GameVariable { name = "TargCurs", node = null },
                        new GameVariable { name = "Shard", node = null },
                        new GameVariable { name = "LShard", node = null },
                        new GameVariable { name = "EnemyHits", node = null },
                        new GameVariable { name = "EnemyID", node = null },
                        new GameVariable { name = "LHandID", node = null },
                        new GameVariable { name = "RHandID", node = null },
                        new GameVariable { name = "SysMsg", node = null }
                    }
                }
            };
            setUpVariablesTree();
           // vartree.Nodes.Add("Player");
            //vartree.Nodes[0].Nodes.Add("Name:" + uonet.player.name);
        }

        private void updateVarsTimer_Tick()
        {
           // if (vartree.Focused) return; // make it copy-able

            foreach (GameVariable[] vars in variableCategs.Values)
                foreach (GameVariable var in vars)
                {
                    String temp = "";
                    //var.node.
                    //.temp = var.name + " = " + var.var;
                    switch (var.name)
                    {
                        case "CharPosX":
                            temp = var.name + " = " + uonet.player.CharPosX;
                            break;
                        case "CharPosY":
                            temp = var.name + " = " + uonet.player.CharPosY;
                            break;
                        case "CharPosZ":
                            temp = var.name + " = " + uonet.player.CharPosZ;
                            break;
                        case "BackpackID":
                            temp = var.name + " = " + uonet.player.BackpackID;
                            break;
                        case "CharID":
                            temp = var.name + " = " + uonet.player.CharID;
                            break;
                        case "CharStatus":
                            temp = var.name + " = " + uonet.player.flags;
                            break;
                        case "CharType":
                            temp = var.name + " = " + uonet.player.CharType;
                            break;
                        case "Sex":
                            temp = var.name + " = " + uonet.player.Sex;
                            break;
                        case "CharName":
                            temp = var.name + " = " + uonet.player.CharName;
                            break;
                        case "CursKind":
                            temp = var.name + " = " + uonet.player.Facet;
                            break;
                        case "Str":
                            temp = var.name + " = " + uonet.player.Str;
                            break;
                        case "Dex":
                            temp = var.name + " = " + uonet.player.Dex;
                            break;
                        case "Int":
                            temp = var.name + " = " + uonet.player.Int;
                            break;
                        case "MaxStats":
                            //temp = var.name + " = " + uonet ;
                            break;
                         case "Hits":
                            temp = var.name + " = " + uonet.player.Hits;
                            break;
                         case "MaxHits":
                            temp = var.name + " = " + uonet.player.MaxHits;
                            break;
                         case "Stamina":
                            temp = var.name + " = " + uonet.player.Stamina;
                            break;
                         case "MaxStam":
                            temp = var.name + " = " + uonet.player.MaxStam;
                            break;
                         case "Mana":
                            temp = var.name + " = " + uonet.player.Mana;
                            break;
                         case "MaxMana":
                            temp = var.name + " = " + uonet.player.MaxMana;
                            break;
                         case "Weight":
                            temp = var.name + " = " + uonet.player.Weight;
                            break;
                         case "MaxWeight":
                            temp = var.name + " = " + uonet.player.MaxWeight;
                            break;
                         case "Gold":
                            temp = var.name + " = " + uonet.player.Gold;
                            break;
                           
                    }
                    if ((var.node.Text != temp)&&(temp != "")) { var.node.Text = temp; }
                    /* GameDLL.SetTop(GH, 0);
                     GameDLL.PushStrVal(GH, "GetVar");
                     GameDLL.PushStrVal(GH, var.name);
                     if (GameDLL.Query(GH) != 0) continue;

                     String tmp = "";
                     switch (GameDLL.GetType(GH, 1))
                     {
                         case 1:
                             tmp = var.name + " = " + ((GameDLL.GetBoolean(GH, 1) != 0) ? "true" : "false");
                             break;
                         case 3:
                             tmp = var.name + " = " + GameDLL.GetInteger(GH, 1);
                             break;
                         case 4:
                             tmp = var.name + " = " + GameDLL.GetString(GH, 1);
                             break;
                         default:
                             tmp = var.name + " = nil";
                             break;
                     }
                     if (var.node.Text != tmp)
                         var.node.Text = tmp;*/
                }
        }

        private void setUpVariablesTree()
        {
            //Timo 'BtbN' Rothenpieler Code
            vartree.Nodes.Clear();

            foreach (String categ in variableCategs.Keys)
            {
                TreeNode node = vartree.Nodes.Add(categ);
                foreach (GameVariable var in variableCategs[categ])
                {
                    var.node = node.Nodes.Add(var.name);
                    var.node.Text = var.name + " = ?";
                }
            }
        }





        private void timer1_Tick(object sender, EventArgs e)
        {
            txtOutput.Text = uonet.myoutput;
           // txtOutput.Select(txtOutput.Text.Length, 0);
            //txtOutput.ScrollToCaret();
            //txtVarwindow.Text = uonet.myvars;
            updateVarsTimer_Tick();
           
        
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ip = txtIP.Text;
            user = txtUsername.Text;
            pass = txtPassword.Text;
            port = Convert.ToInt32(txtPort.Text);
            uonet = new uonetwork(ip, port, user, pass, Convert.ToInt32(txtCharSlot.Text));
            mythread = new Thread(new ThreadStart(uonet.main));
            mythread.IsBackground = true;
            mythread.Start();
            timer1.Interval = 100;
            timer1.Start();
            Pingtimer.Interval = 30000;
            Pingtimer.Start();
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mythread.IsAlive)
            {
                uonet.Dissconnect();
                mythread.Abort();
            }
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (uonet.bConnected)
            {
                myscript = new Script(uonet);
                Thread mythread2;
                mythread2 = new Thread(new ThreadStart(myscript.main));
                mythread2.IsBackground = true;
                mythread2.Start();
            }
            else
            {
                txtOutput.Text = txtOutput.Text + "\r\nNot Connected!";
            }
            //uonet.Send(Packets.Send.Packets.GetPlayerStatus(true,uonet.player.serial));
        }

        private void Pingtimer_Tick(object sender, EventArgs e)
        {
            byte[] pingpacket = new byte[2];
                    pingpacket[0] = 0x73;
                    pingpacket[1] = 0x00;
                    uonet.Send(pingpacket);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            uonet.Dissconnect();
            mythread.Abort();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Ultima.Tile mytile = new Ultima.Tile();
            Ultima.TileMatrix tm = new TileMatrix(0, 0, 6144, 4096);
            HuedTile[] htile = tm.GetStaticTiles(777, 1477);
            Ultima.Tile mytile2 = new Ultima.Tile();
            mytile = tm.GetLandTile(577,1777);
            int mytileid = (htile[0].ID & 0xFFF); // need a check, exception if ID is 0


            //mytile2 = 



            mytile = tm.GetLandTile(273, 1216);
            //txtOutput.Text = txtOutput.Text + StringList.Table[1076267];
            //txtOutput.Text = txtOutput.Text + 
        }

         
    }
}
