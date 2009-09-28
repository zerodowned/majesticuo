using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
        
namespace drkuo
{
    public partial class drkGui : Form
    {
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
            ip = txtIP.Text;
            user = txtUsername.Text;
            pass = txtPassword.Text;
            port = Convert.ToInt32(txtPort.Text);
            uonet = new uonetwork(ip, port, user, pass, Convert.ToInt32(txtCharSlot.Text));
            mythread = new Thread(new ThreadStart(uonet.main));
            mythread.IsBackground = true;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            txtOutput.Text = uonet.myoutput;
            txtOutput.Select(txtOutput.Text.Length, 0);
            txtOutput.ScrollToCaret();
            txtVarwindow.Text = uonet.myvars;
            //lsVars.Items.Add("Name: " + uonet.player.name);
            
        
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
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
    }
}
