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
        public Effector EndEffector { get; set; }
        public List<Segment> AllSegments { get; set; }
        public Robot(float X, float Y, int Width, int Height, EffectorType et = EffectorType.Grabber)
        {
            this.Length = 0;
            this.Width = Width - 15;
            this.Height = Height - 40;
            this.Segments = new List<Segment>();
            this.Base = new Vector2(X, Y);
            this.FollowMouse = true;
            this.Ball = new DummyBall(X, Y, 45);
            this.EndEffector = new Effector(this.Base, et);
            this.AllSegments = new List<Segment>(1);
            this.AllSegments.Add(this.EndEffector);
        }

        public void AddSegment(double len, Color? c = null)
        {
            AddSegment(len, -365.0, 365.0, c.GetValueOrDefault(Segment.DEFAULT_COLOUR));
        }
        public void AddSegment(double len, double range_min, double range_max, Color? c)
        {
            if (Length == 0)
                Segments.Add(new Segment(Base, len, 0, range_min, range_max, c)); // Dodavanje na prv segment
            else
            {   // Dodavanje na sleden segment
                Segment last = Segments.Last();
                Segments.Add(
                    new Segment(last, len, c)//, range_min, range_max)
                );
            }

            EndEffector.Rebase(Segments.Last().end);
            this.BallUpdate();

            this.AllSegments = new List<Segment>(this.Segments);
            this.AllSegments.Add(this.EndEffector);

            Length++;
        }

        public void BallUpdate()
        {
            Ball.X = EndEffector.end.X;
            Ball.Y = EndEffector.end.Y;
        }
        
        public void Update()
        {
            this.Update(-100, -100);
        }
        public void Update(float X, float Y)
        {
            //if (Length == 0) return

            this.FollowMouse = X > 0 && X < Width && Y > 0 && Y < Height;
            if (FollowMouse) BallUpdate();

            // cel za sledenje
            Vector2 target = FollowMouse || PreviewMode ? new Vector2(X, Y) : Ball.Pos();
            Segment s;
            for (int i = Length; i >= 0; i--)
            {
                s = AllSegments[i];
                s.target(target.X, target.Y);
                target = s.pos;  // da bidat povrzani segmenti
            }

            // Da ne se pomestuvaat od bazata
            Vector2 b = Base;
            foreach (Segment seg in AllSegments)
            {
                seg.Rebase(b);
                b = seg.end;
            }

            Ball.Update(Width, Height);

        }

        public void BuildFrom(List<Segment> segs, Effector ef, bool Rebase = false)
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
                    this.AddSegment(segment.len, segment.Colour);//, segment.range_min, segment.range_max);
                }
            }

            this.EndEffector = ef;
            this.AllSegments = new List<Segment>(this.Segments);
            this.AllSegments.Add(this.EndEffector);

            this.BallUpdate();
        }

        public void RemoveSegement(int index)
        {
            Vector2 end = Segments.Last().end;
            Segments.RemoveAt(index);
            this.AllSegments = new List<Segment>(Segments);
            this.AllSegments.Add(EndEffector);
            this.Length--;
            this.Update(end.X, end.Y);
        }

        public void Show(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias; // Poubav izgled

            // Iscrtuvanje topche
            if (!FollowMouse && !PreviewMode)
                Ball.Show(g);

            // Iscrtuvanje segmenti
            foreach (Segment seg in Segments)
            {
                seg.Show(g);
            }

            this.EndEffector.Show(g);

            // Iscrtuvanje baza na robotska raka
            Color c = Color.FromArgb(108, 110, 111);
            Brush b = new SolidBrush(c);

            int Rad = 20;
            g.FillEllipse(b, Base.X - Rad, Base.Y - Rad, Rad * 2, Rad * 2);

            Point[] points = { 
                new Point((int)(Base.X-Rad*3), this.Width + 50),
                new Point((int)(Base.X-Rad*3), (int)(Base.Y+20)),
                new Point((int)(Base.X-Rad*1.5), (int)(Base.Y)),
                new Point((int)(Base.X+Rad*1.5), (int)(Base.Y)),
                new Point((int)(Base.X+Rad*3), (int)(Base.Y+20)),
                new Point((int)(Base.X+Rad*3), this.Width + 50)
            };
            g.FillPolygon(b, points);

            b.Dispose();
        }
    }
}
