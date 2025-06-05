using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Build_A_Bot
{
    public class DummyBall
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Radius { get; set; }
        public Vector2 Direction { get; set; }

        public DummyBall(float X, float Y, int Radius = 30)
        {
            this.X = X;
            this.Y = Y;
            this.Radius = Radius;
            Random r = new Random();
            Direction = new Vector2(    // random pravec na dvizenje
                (float)r.NextDouble() * 2 - 1,
                (float)r.NextDouble() * 2 - 1
                );
            Direction = Vector2.Normalize(Direction);
        }

        public void Update(int width, int height)
        {
            int mult = 10;
            X += Direction.X * mult;
            Y += Direction.Y * mult;

            bool flag = false;

            if ((Direction.X < 0 && X < Radius) || ( Direction.X > 0 && X + Radius > width))
            {   // Sudir so horizontalni granici
                Direction = new Vector2(-Direction.X, Direction.Y);
                flag = true;
            }
            if ((Direction.Y < 0 && Y < Radius) || ( Direction.Y > 0 && Y + Radius > height))
            {   // Sudir so vertikalni granici
                Direction = new Vector2(Direction.X, -Direction.Y);
                flag = true;
            }

            if (flag)
            {   // Da ima malo odstapuvanje vo odbivanje
                Random r = new Random();
                float mod = 0.01f;
                Direction = Direction + new Vector2( mod * (float)(r.NextDouble() - 0.5), mod * (float)(r.NextDouble() - 0.5));
                Direction = Vector2.Normalize(Direction);
            }
        }

        public Vector2 Pos()
        {
            return new Vector2(X, Y);
        }

        public void Show(Graphics g)
        {
            float[] penDashStyle = { 7.0f, 15.0f };
            Pen p = new Pen(Color.DarkGray);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            p.DashPattern = penDashStyle;

            g.DrawEllipse(p, X - Radius, Y - Radius, Radius*2, Radius*2);

            p.Dispose();

            p = new Pen(Color.IndianRed, 3);
            g.DrawEllipse(p, X, Y, 3, 3);
            p.Dispose();

        }
    }
}
