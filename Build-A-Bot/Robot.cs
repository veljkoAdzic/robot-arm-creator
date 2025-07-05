using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Build_A_Bot
{
    public class Robot
    {
        public List<Segment> Segments { get; set; }
        public int Length { get; set; }
        [JsonConverter(typeof(JsonVec2Converter))]
        public Vector2 Base { get; set; }
        [JsonIgnore]
        public bool FollowMouse { get; set; } = false;
        [JsonIgnore]
        public DummyBall Ball { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        [JsonIgnore]
        public bool PreviewMode { get; set; } = false;
        public Effector EndEffector { get; set; }
        [JsonIgnore]
        public List<Segment> AllSegments { get; set; }

        public Robot() { }
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
            if (Length == 0)
                Segments.Add(new Segment(Base, len, 0, c)); // Dodavanje na prv segment
            else
            {   // Dodavanje na sleden segment
                Segment last = Segments.Last();
                Segments.Add(
                    new Segment(last, len, c)
                );
            }

            EndEffector.Rebase(Segments.Last().end);
            this.BallUpdate();

            this.AllSegments = new List<Segment>(this.Segments);
            this.AllSegments.Add(this.EndEffector);

            Length++;
        }

        public void BallUpdate()
        {   // Postavuvanje topche na vrv na efektorot
            Ball.X = EndEffector.end.X;
            Ball.Y = EndEffector.end.Y;
        }

        public void Update()
        {   // Update so sledenje na topche
            this.Update(-100, -100);
        }
        public void Update(float X, float Y)
        {
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
        {   // Recreacija na robot of dadeni segmenti i efektor
            if (!Rebase) // Bez promena na pozicija na segmenti
            {
                this.Segments = segs;
                this.Length = this.Segments.Count;
                this.Base = new Vector2(segs[0].pos.X, segs[0].pos.Y);
            }
            else
            {   // So promena na pozicija do Base
                this.Segments.Clear();
                this.Length = 0;
                foreach (var segment in segs)
                {
                    this.AddSegment(segment.len, segment.Colour);//, segment.range_min, segment.range_max);
                }
            }

            this.EndEffector = new Effector(ef);
            this.EndEffector.Rebase(this.Segments.Last().end);
            this.AllSegments = new List<Segment>(this.Segments);
            this.AllSegments.Add(this.EndEffector);

            this.BallUpdate();
        }

        public void RemoveSegement(int index)
        {
            Vector2 end = AllSegments.Last().end;
            Segments.RemoveAt(index);
            AllSegments.RemoveAt(index);
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
            Brush b = new SolidBrush(Color.FromArgb(100, 102, 104));

            int Rad = 20;
            g.FillEllipse(b, Base.X - Rad, Base.Y - Rad, Rad * 2, Rad * 2);

            b = new SolidBrush(AllSegments[0].Colour);
            float mod = 0.4f;
            g.FillEllipse(b, Base.X - Rad* mod, Base.Y - Rad * mod, Rad * 2 * mod, Rad * 2 * mod);

            // baza blok
            b = new SolidBrush(c);
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
