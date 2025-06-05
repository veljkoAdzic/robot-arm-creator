using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
namespace Build_A_Bot
{
    public partial class Form1 : Form
    {
        public Robot Rob { get; set; }
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Rob = new Robot(this.Width / 2, this.Height - 50, this.Width, this.Height);
            
            // For testing
            for (double len = 100; len > 30; len-= 15)
                Rob.AddSegment(len, -95.0, 95.0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Rob.Show(e.Graphics);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Rob.Update();
            Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Rob.Update(e.X, e.Y);
            Invalidate();
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            Rob.BallUpdate();
            timer1.Start();
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            timer1.Stop();
        }
    }
}
