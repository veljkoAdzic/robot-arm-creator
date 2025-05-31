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
            this.angle = ang_deg * 180 / Math.PI; // sekogash ochekuva stepeni
            this.change = 0;
            calculateEnd();
        }

        private void calculateEnd()
        {
            // presmetka na nov end
            float nX = pos.X + (float)(len * Math.Cos(change));
            float nY = pos.Y + (float)(len * Math.Sin(change));
            end = new Vector2(nX, nY); 
        }

        public void target(float X, float Y)
        {
            // Presmetka na agol od baza na segment do target
            Vector2 t = new Vector2(X, Y);
            Vector2 dir = t - pos;
            double a = Math.Atan2(dir.Y, dir.X);
            // Presmetka na noviot agol
            change = angle + a;

            // Postavuvanje velichina so obratna nasoka na dir vektor 
            double mag = Math.Sqrt((double)(dir.X * dir.X + dir.Y * dir.Y));
            Vector2 MagModifier = new Vector2((float)( -len / mag));
            dir = dir * MagModifier;

            // pomestuvanje baza da pokazuva segemnt to target
            pos = t + dir;            

            // presmetka na end
            calculateEnd();
        }

        public void Update()
        {
            // test - rotacija na segment
            change = 0.1;
            angle = (angle + change) % (Math.PI * 2);
            calculateEnd();
        }

        public void Show(Graphics g)
        {
            // iscrtuvanje na segment
            Pen p = new Pen(Color.FromArgb(205, 140, 25), 10.0f);
            g.DrawLine(p, pos.X, pos.Y, end.X, end.Y);
            p.Dispose();
        }
    }
}
