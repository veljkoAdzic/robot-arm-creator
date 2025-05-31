using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Build_A_Bot
{
    public class Robot
    {
        public List<Segment> Segments { get; set; }
        public int Length { get; set; }
        public Vector2 Base { get; set; }
        public bool FollowMouse { get; set; }
        public DummyBall Ball { get; set; }

        public int Widht { get; set; }
        public int Height { get; set; }

        public Robot(float X, float Y, int Width, int Height)
        {
            this.Length = 0;
            this.Widht = Width;
            this.Height = Height;
            this.Segments = new List<Segment>();
            this.Base = new Vector2(X, Y);
            this.FollowMouse = true;
            this.Ball = new DummyBall(X, Y, 60);
        }

        public void AddSegment(double len, double range_min, double range_max)
        {
            if (Length == 0)
                Segments.Add(new Segment(Base, len, 0, range_min, range_max)); // Dodavanje na prv segment
            else
            {   // Dodavanje na sleden segment
                Segment last = Segments.Last();
                Segments.Add(
                    new Segment(last, len)//, range_min, range_max)
                );
            }

            Vector2  end = Segments.Last().end;
            Ball.X = end.X;
            Ball.Y = end.Y;

            Length++;
        }

        public void BallUpdate()
        {
            Ball.X = Widht/2;
            Ball.Y = Height/2;
        }
        

        public void Update(float X, float Y)
        {
            if (Length == 0) return;

            this.FollowMouse =  X > 0 && X < Widht && Y > 0 && Y < Height;
            if (!FollowMouse) Ball.Update(Widht, Height);

            // cel za sledenje
            Vector2 target = FollowMouse ? new Vector2(X, Y) : Ball.Pos();
            Segment s;
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

            if (!FollowMouse)
                Ball.Show(g);

            foreach (Segment seg in Segments)
            {
                seg.Show(g);
            }
        }
    }
}
