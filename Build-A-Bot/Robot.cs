using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Build_A_Bot
{
    public class Robot
    {
        public List<Segment> Segments { get; set; }
        public int Length { get; set; }
        public Vector2 Base { get; set; }
        public Robot(float X, float Y)
        {
            this.Length = 0;
            this.Segments = new List<Segment>();
            this.Base = new Vector2(X, Y);
        }

        public void AddSegment(double len, double range_min, double range_max)
        {
            if (Length == 0)
                Segments.Add(new Segment(Base, len, 0, range_min, range_max)); // Dodavanje na prv segment
            else
            {   // Dodavanje na sleden segment
                Segment last = Segments.Last();
                Segments.Add(
                    new Segment(last, len, range_min, range_max)
                );
            }

            Length++;
        }

        public void Update(float X, float Y)
        {
            if (Length == 0) return;

            Segment s; // segment koj se updatira
            Vector2 target = new Vector2(X, Y); // cel
            for (int i = Length -1; i >= 0; i--)
            {
                s = Segments[i];
                s.target(target.X, target.Y);
                target = s.pos;  // da bidat povrzani segmenti
            }

            // Da ne se pomestuvaat od bazata
            Vector2 b = Base;
            foreach (Segment seg in Segments)
            {
                seg.Rebase(b);
                b = seg.end;
            }

        }

        public void Show(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias; // Poubav izgled
            foreach (Segment seg in Segments)
            {
                seg.Show(g);
            }
        }
    }
}
