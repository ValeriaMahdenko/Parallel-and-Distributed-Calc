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
    public partial class Form2 : Form
    {
        private Thread thread;
        private bool Check;
        public Form2()
        {
            InitializeComponent();
            Check = true;
            Start.Enabled = false;
        }
        
        private void Form2_Load(object sender, EventArgs e)
        {
            thread = new Thread(PanelDraw);
            thread.Start();
            Form3 form3 = new Form3();
            form3.Show();
        }

        public void PanelDraw()
        {
            Pen pen = new Pen(Color.Blue, 7.0F);
            Graphics g = panel1.CreateGraphics();
            float yEx = 200;
            float eF = 20;
           
            while (true)
            {
                g.Clear(panel1.BackColor);
                float x1 = 0;
                float y1 = 0;
                float y2 = 0;
                for (float x = 0; x < 20; x += 0.1F)
                {
                    y2 = (float)Math.Sin(x);
                    g.DrawLine(pen, x1 * eF, y1 * eF + yEx, x * eF, y2 * eF + yEx);
                    x1 = x;
                    y1 = y2;
                    Thread.Sleep(50);
                }
                Invalidate();
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
