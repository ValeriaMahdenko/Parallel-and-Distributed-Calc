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
    public partial class Form4 : Form
    {
        private Thread thread;
        private bool Check;
        public Form4()
        {
            InitializeComponent();
            Check = true;
            Start.Enabled = false;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            thread = new Thread(PanelDraw);
            thread.Start();
        }

        public void PanelDraw()
        {
            double scale = 100;
            double repetitions = Math.Round(scale, 0);
            double basis = (2.0d * Math.PI) / scale;
            double petals = 2.0d;
            Graphics g = panel1.CreateGraphics();
            Pen pen = new Pen(Color.Red, 5);
            while (true)
            {
                g.Clear(panel1.BackColor);
                for (double i = 0.0f; i < (repetitions - 1); i++)
                {
                    double t0 = i * basis;
                    double t1 = (i + 1) * basis;

                    double x0 = Math.Sin(petals * t0) * Math.Cos(t0);
                    double x1 = Math.Sin(petals * t1) * Math.Cos(t1);

                    double y0 = Math.Sin(petals * t0) * Math.Sin(t0);
                    double y1 = Math.Sin(petals * t1) * Math.Sin(t1);

                    g.DrawLine
                        (pen, (Single)((scale * x0) + scale),
                        (Single)((scale * y0) + scale),
                        (Single)((scale * x1) + scale),
                        (Single)((scale * y1) + scale));
                    Thread.Sleep(100);
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
