using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

        public void AddSegment(double len)
        {
            if (Length == 0)
                Segments.Add(new Segment(Base, len, 0));
            else
            {
                Segment last = Segments.Last();
                Segments.Add(
                    new Segment(last.pos, len, (last.angle * 180 / Math.PI))
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

        }

        public void Show(Graphics g)
        {
            foreach (Segment seg in Segments)
            {
                seg.Show(g);
            }
        }
    }
}
