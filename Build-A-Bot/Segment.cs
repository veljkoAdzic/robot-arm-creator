using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Build_A_Bot
{
    public class Segment
    {
        [JsonConverter(typeof(JsonVec2Converter))]
        public Vector2 pos { get; set; }
        [JsonConverter(typeof(JsonVec2Converter))]
        public Vector2 end { get; set; }
        public double angle { get; set; }
        public double len { get; set; }
        public double change { get; set; }

        public static double OFFSET = (-90 * Math.PI / 180);
        [JsonIgnore]
        public static Color DEFAULT_COLOUR = Color.FromArgb(205, 140, 25);
        [JsonConverter(typeof(JsonColorConverter))]
        public Color Colour { get; set; }

        public Segment() {}
        public Segment(Vector2 pos, double len, double ang_deg, Color? c = null)
        {
            this.pos = pos;
            this.len = len;
            this.angle = ang_deg * Math.PI / 180 + OFFSET ; // sekogash ochekuva stepeni

            this.Colour = c.GetValueOrDefault(DEFAULT_COLOUR); 

            this.change = 0;

            calculateEnd();
        }

        public Segment(Segment s, double len, Color? c = null)
        {
            this.pos = s.end;
            this.len = len;
            this.angle = s.angle; // sekogash ochekuva stepeni

            this.Colour = c.GetValueOrDefault(DEFAULT_COLOUR);

            this.change = 0;

            calculateEnd();
        }

        protected void calculateEnd()
        {
            double a = angle + change; // vistinski agol
            // presmetka na nov end
            float nX = pos.X + (float)(len * Math.Cos(a));
            float nY = pos.Y + (float)(len * Math.Sin(a));
            end = new Vector2(nX, nY);
        }

        public void target(float X, float Y)
        {   // Funkcija za sledenje na cel
            // Presmetka na agol od baza na segment do target
            Vector2 t = new Vector2(X, Y);
            Vector2 dir = t - pos;
            // Presmetka na noviot agol
            change = Math.Atan2(dir.Y, dir.X) - OFFSET;

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
            Color bgColour = Color.FromArgb(230, 230, 230);
            float segWidth = 20.0f;


            Pen p = new Pen(this.Colour, segWidth);
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            p.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;

            // Isrctuvanje na segmentot
            g.DrawLine(p, pos.X, pos.Y, end.X, end.Y);

            // Iscrtuvanje na praznina
            p.Color = bgColour;
            p.Width = segWidth*0.4f;
            p.StartCap = System.Drawing.Drawing2D.LineCap.Triangle;
            g.DrawLine(p, pos.X, pos.Y, end.X, end.Y);

            p.Dispose();

            if (this.len < segWidth*2) return;

            // Iscrtuvanje na zglob
            segWidth *= 0.55f;
            Brush b = new SolidBrush(Color.FromArgb(98, 100, 102));
            g.FillEllipse(b, pos.X - segWidth, pos.Y - segWidth, segWidth * 2, segWidth * 2);

            segWidth *= 0.35f;
            b = new SolidBrush(this.Colour);
            g.FillEllipse(b, pos.X - segWidth, pos.Y - segWidth, segWidth * 2, segWidth * 2);

            b.Dispose();
        }

        override public String ToString()
        {
            return $"Length: {len} - Color: RGB({this.Colour.R}, {Colour.G}, {Colour.B})";
        }
    }
}
