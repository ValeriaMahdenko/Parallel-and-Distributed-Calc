using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Parallel_Forms_task_2
{
    public partial class Form1 : Form
    {
        private List<Thread> threads;
        private List<bool> status;


        public Form1()
        {
            InitializeComponent();
            threads = new List<Thread>();
            status = new List<bool> { true, false, false, false };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;
            threads.Add(new Thread(() => Draw1(ref x,ref y)));
            threads.Add(new Thread(() => Draw2(ref x, ref y)));
            threads.Add(new Thread(() => Draw3(ref x, ref y)));
            threads.Add(new Thread(() => Draw4(ref x, ref y)));
            threads.ForEach(p =>{
                p.Start();
            });

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            threads.ForEach(p => p.Abort());
        }

        public void Draw1(ref int x, ref int y)
        {
            Graphics gr = panel1.CreateGraphics();
            while (true)
            {
                while (y <= 0 && x < panel1.Width - 80)
                {
                    gr.Clear(panel1.BackColor);
                    gr.FillRectangle(Brushes.Crimson, x, y, 80, 80);
                    x += 10;
                    y = 0;

                    Thread.Sleep(100);
                }
                
            }
        }

        public void Draw2(ref int x, ref int y)
        {
            Graphics gr = panel1.CreateGraphics();
            while (true)
            {
                while (x >= panel1.Width - 80 && y < panel1.Height - 80)
                {
                    gr.Clear(panel1.BackColor);
                    gr.FillRectangle(Brushes.Gray, x, y, 80, 80);

                    x = panel1.Width - 80;
                    y += 10;

                    Thread.Sleep(100);
                }
               
            }
        }

        public void Draw3(ref int x, ref int y)
        { 
            Graphics gr = panel1.CreateGraphics();
            while (true)
            {
                while (y >= panel1.Height - 80 && x > 0)
                {
                    gr.Clear(panel1.BackColor);
                    gr.FillRectangle(Brushes.Yellow, x, y, 80, 80);

                    x -= 10;
                    y = panel1.Height - 80;

                    Thread.Sleep(100);
                }
               
            }
        }

        public void Draw4(ref int x, ref int y)
        {
            Graphics gr = panel1.CreateGraphics();
            while (true)
            {
                while (x <= 0 && y >= 0)
                {
                    gr.Clear(panel1.BackColor);
                    gr.FillRectangle(Brushes.Pink, x, y, 80, 80);

                    x = 0;
                    y -= 10;

                    Thread.Sleep(100);
                }
            }
        }

    }
}
