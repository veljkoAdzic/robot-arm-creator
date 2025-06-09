using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Build_A_Bot
{
    [Serializable]
    public class Segment : ISerializable
    {
        public Vector2 pos { get; set; }
        public Vector2 end { get; set; }
        public double angle { get; set; }
        public double len { get; set; }
        public double change { get; set; }
        public double range_min { get; set; }
        public double range_max { get; set; }

        public static double OFFSET = (-90 * Math.PI / 180);

        public static Color DEFAULT_COLOUR = Color.FromArgb(205, 140, 25);
        public Color Colour { get; set; }

        public Segment(Vector2 pos, double len, double ang_deg, double range_min, double range_max, Color? c = null)
        {
            this.pos = pos;
            this.len = len;
            this.angle = ang_deg * Math.PI / 180 + OFFSET ; // sekogash ochekuva stepeni

            this.range_min = (range_min * Math.PI / 180);
            this.range_max = (range_max * Math.PI / 180);


            this.Colour = c.GetValueOrDefault(DEFAULT_COLOUR); 

            this.change = 0;

            calculateEnd();
        }

        public Segment(Segment s, double len, Color? c = null)//, double range_min, double range_max)
        {
            this.pos = s.end;
            this.len = len;
            this.angle = s.angle; // sekogash ochekuva stepeni

            this.range_min = -Math.PI * 2;//(range_min * Math.PI / 180);
            this.range_max = Math.PI * 2;//(range_max * Math.PI / 180);

            this.Colour = c.GetValueOrDefault(DEFAULT_COLOUR);

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
            Pen p = new Pen(this.Colour, 10.0f);
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            p.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;

            //p.Color = Color.FromArgb(179, 121, 21);
            //p.Width = 12.0f;
            //g.DrawLine(p, pos.X, pos.Y, end.X, end.Y);

            //p.Color = Color.FromArgb(205, 140, 25);
            //p.Width = 8.0f;
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PosX", pos.X);
            info.AddValue("PosY", pos.Y);

            info.AddValue("EndX", end.X);
            info.AddValue("EndY", end.Y);

            info.AddValue("Angle", angle);
            info.AddValue("Len", len);
            info.AddValue("Change", change);
            info.AddValue("RangeMin", range_min);
            info.AddValue("RangeMax", range_max);
            info.AddValue("Colour", Colour.ToArgb());

        }

        private Segment(SerializationInfo info, StreamingContext context)
        {
            this.pos = new Vector2((float)info.GetDouble("PosX"), (float)info.GetDouble("PosY"));
            this.end = new Vector2((float)info.GetDouble("EndX"), (float)info.GetDouble("EndY"));

            this.angle = info.GetDouble("Angle");
            this.len = info.GetDouble("Len");
            this.change = info.GetDouble("Change");
            this.range_min = info.GetDouble("RangeMin");
            this.range_max = info.GetDouble("RangeMax");
            this.Colour = Color.FromArgb( info.GetInt32("Colour") );
        }

        override public String ToString()
        {
            return $"Length: {len} - Color: RGB({this.Colour.R}, {Colour.G}, {Colour.B})";
        }
    }
}
