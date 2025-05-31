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

            //this.seg = new Segment(new Vector2(300.0f, 300.0f), 100.0, 0);
            Rob = new Robot(this.Width / 2, this.Height - 50);
            Rob.AddSegment(150.0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //seg.Show(e.Graphics);
            Rob.Show(e.Graphics);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //seg.Update();
            //Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //seg.target(e.X, e.Y);
            Rob.Update(e.X, e.Y);
            Invalidate();
        }
    }
}
