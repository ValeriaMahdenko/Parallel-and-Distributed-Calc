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
    public partial class Form3 : Form
    {
        private Thread thread;
        private bool Check;
        public Form3()
        {
            InitializeComponent();
            Check = true;
            Start.Enabled = false;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            thread = new Thread(PanelDraw);
            thread.Start();
            Form4 form4 = new Form4();
            form4.Show();
        }
        public void PanelDraw()
        {
            Random rand = new Random();
            Brush brush = Brushes.DeepSkyBlue;
            Graphics g = panel1.CreateGraphics();
            while (true)
            {
                g.Clear(panel1.BackColor);
                int width = rand.Next(5, panel1.Width / 2);
                int height = rand.Next(5, panel1.Height / 2);
                int s1 = rand.Next(50, 200);
                int s2 = rand.Next(50, 200);
                var point = new PointF(rand.Next(width, panel1.Width - width), rand.Next(height, panel1.Height - height));
                RectangleF rectangleF = new RectangleF(point, new SizeF(s1, s2));
                g.FillEllipse(brush, rectangleF);
                Invalidate();
                Thread.Sleep(1000);
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
