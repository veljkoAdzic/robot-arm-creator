using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

        public int Width { get; set; }
        public int Height { get; set; }
        public bool PreviewMode { get; set; } = false;

        public Robot(float X, float Y, int Width, int Height)
        {
            this.Length = 0;
            this.Width = Width - 15;
            this.Height = Height - 40;
            this.Segments = new List<Segment>();
            this.Base = new Vector2(X, Y);
            this.FollowMouse = true;
            this.Ball = new DummyBall(X, Y, 45);
        }

        public void AddSegment(double len)
        {
            AddSegment(len, -365.0, 365.0);
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
            Vector2  end = Segments.Last().end;
            Ball.X = end.X;
            Ball.Y = end.Y; // Math.Min(end.Y, Width - Ball.Radius);
            //Ball.Update(Widht, Height);
        }
        
        public void Update()
        {
            this.Update(-100, -100);
        }
        public void Update(float X, float Y)
        {
            if (Length == 0) return;

            this.FollowMouse = X > 0 && X < Width && Y > 0 && Y < Height;
            if (FollowMouse) BallUpdate();

            // cel za sledenje
            Vector2 target = FollowMouse || PreviewMode ? new Vector2(X, Y) : Ball.Pos();
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

            Ball.Update(Width, Height);

        }

        public void BuildFrom(List<Segment> segs, bool Rebase = false)
        {
            if (!Rebase)
            {
                this.Segments = segs;
                this.Length = this.Segments.Count;
                this.Base = new Vector2(segs[0].pos.X, segs[0].pos.Y);
            } else
            {
                this.Segments.Clear();
                this.Length = 0;
                foreach (var segment in segs)
                {
                    this.AddSegment(segment.len);//, segment.range_min, segment.range_max);
                }
            }

            Segment last = Segments.Last();
            Ball.X = last.end.X;
            Ball.Y = last.end.Y;
        }

        public void Show(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias; // Poubav izgled

            if (!FollowMouse && !PreviewMode)
                Ball.Show(g);

            foreach (Segment seg in Segments)
            {
                seg.Show(g);
            }
        }
    }
}
