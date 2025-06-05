using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
        public double range_min { get; set; }
        public double range_max { get; set; }

        public static double OFFSET = (-90 * Math.PI / 180);

        public Segment(Vector2 pos, double len, double ang_deg, double range_min, double range_max)
        {
            this.pos = pos;
            this.len = len;
            this.angle = ang_deg * Math.PI / 180 + OFFSET ; // sekogash ochekuva stepeni

            this.range_min = (range_min * Math.PI / 180);
            this.range_max = (range_max * Math.PI / 180);

            this.change = 0;

            calculateEnd();
        }

        public Segment(Segment s, double len)//, double range_min, double range_max)
        {
            this.pos = s.end;
            this.len = len;
            this.angle = s.angle; // sekogash ochekuva stepeni

            this.range_min = -Math.PI * 2;//(range_min * Math.PI / 180);
            this.range_max = Math.PI * 2;//(range_max * Math.PI / 180);

            this.change = 0;

            calculateEnd();
        }

        private void calculateEnd()
        {
            double a = angle + change;// * 0.1;
            // presmetka na nov end
            float nX = pos.X + (float)(len * Math.Cos(a));
            float nY = pos.Y + (float)(len * Math.Sin(a));
            end = new Vector2(nX, nY);
            //change -= angle - a;
        }

        public void target(float X, float Y)
        {
            // Presmetka na agol od baza na segment do target
            Vector2 t = new Vector2(X, Y);
            Vector2 dir = t - pos;
            double a = Math.Atan2(dir.Y, dir.X) - OFFSET;
            // Presmetka na noviot agol
            change = Constrain(a, range_min, range_max);


            // Postavuvanje velichina so obratna nasoka na dir vektor 
            dir = Vector2.Normalize(dir);
            dir = Vector2.Multiply(dir, (float)(-len));

            // pomestuvanje baza da pokazuva segemnt to target
            pos = t + dir;            

            // presmetka na end
            calculateEnd();
        }

        public void Update()
        {
            // test - rotacija na segment
            //change = 0.1;
            //angle = (angle + change) % (Math.PI * 2);
            calculateEnd();
        }

        public void Rebase(Vector2 b)
        {   // Pomoshna funkcija za vrakjanje na lokacija b
            this.pos = b;
            calculateEnd();
        }

        public void Show(Graphics g)
        {
            // iscrtuvanje na segment
            Pen p = new Pen(Color.FromArgb(205, 140, 25), 10.0f);
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            p.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
            
            p.Color = Color.FromArgb(179, 121, 21);
            p.Width = 12.0f;
            g.DrawLine(p, pos.X, pos.Y, end.X, end.Y);

            p.Color = Color.FromArgb(205, 140, 25);
            p.Width = 8.0f;
            g.DrawLine(p, pos.X, pos.Y, end.X, end.Y);
            p.Dispose();
        }

        private double Constrain(double value, double min, double max)
        {   // Ogranichuvanje vrednost vo daden obseks
            if (value >= min)
                return value <= max ? value : max;
            else 
                return min;
        }
    }
}
