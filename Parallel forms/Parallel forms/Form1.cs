using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Parallel_forms
{
    public partial class Form1 : Form
    {
        private Thread thread;
        private bool Check;
        public Form1()
        {
            InitializeComponent();
            Check = true;

            Start.Enabled = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            thread = new Thread(Panel1Draw);
            thread.Start();

            Form2 form2 = new Form2();
            form2.Show();
        }

        public void Panel1Draw()
        {
            Brush b = new SolidBrush(Color.Yellow);
            int wid = 50, y = 150, speed = 3;
            int x1 = panel1.Width / 2 - wid;
            int x2 = panel1.Width / 2;
            bool away = true;
            Graphics e = panel1.CreateGraphics();
            Graphics g = panel1.CreateGraphics();
            while (true)
            {
                g.Clear(panel1.BackColor);
                e.Clear(panel1.BackColor);

                e.FillEllipse(Brushes.Red, x1, y, wid, wid);
                g.FillEllipse(Brushes.Blue, x2, y, wid, wid);
                if (away)
                {
                    x1 -= speed;
                    x2 += speed;
                    away = x1 > 0;
                }
                else
                {
                    x1 += speed;
                    x2 -= speed;
                    away = x1 >= Width / 2 - wid;
                }
                Invalidate();
                Thread.Sleep(100);
            }
        }
        private void Start_Click(object sender, EventArgs e)
        {
            if (Check == false)
            {
                thread.Resume();
                Check = true;
            }
            Stop.Enabled = true;
            Start.Enabled = false;
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (Check)
            {
                thread.Suspend();
                Check = false;
            }
            Start.Enabled = true;
            Stop.Enabled = false;

        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }
    }
}
