using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Build_A_Bot
{
    public class Segment
    {
        public Vector2 pos { get; set; }
        public Vector2 end { get; set; }
        public double angle { get; set; }
        public double len { get; set; }
        public double change { get; set; }

        public Segment(Vector2 pos, double len, double ang_deg)
        {
            this.pos = pos;
            this.len = len;
            this.angle = ang_deg * 180 / Math.PI; // Always in radians
            this.change = 0;
            calculateEnd();
        }

        private void calculateEnd()
        {
            float nX = pos.X + (float)(len * Math.Cos(angle));
            float nY = pos.Y + (float)(len * Math.Sin(angle));
            end = new Vector2(nX, nY); 
        }

        public void Update()
        {
            change = 0.1;
            angle = (angle + change) % (Math.PI * 2);
            calculateEnd();
        }

        public void Show(Graphics g)
        {
            Pen p = new Pen(Color.FromArgb(205, 140, 25), 10.0f);
            g.DrawLine(p, pos.X, pos.Y, end.X, end.Y);
            p.Dispose();
        }
    }
}
