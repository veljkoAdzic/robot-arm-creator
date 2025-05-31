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
        }

        public void Update(int width, int height)
        {
            int mult = 10;
            X += Direction.X * mult;
            Y += Direction.Y * mult;

            if (X < Radius || X + Radius > width)
            {   // Sudir so horizontalni granici
                Direction = new Vector2(-Direction.X, Direction.Y);
            }
            if (Y < Radius || Y + Radius > height)
            {   // Sudir so vertikalni granici
                Direction = new Vector2(Direction.X, -Direction.Y);
            }
        }

        public Vector2 Pos()
        {
            return new Vector2(X, Y);
        }

        public void Show(Graphics g)
        {
            Pen p = new Pen(Color.Black);
            g.DrawEllipse(p, X - Radius, Y - Radius, Radius, Radius);
            p.Dispose();

            p = new Pen(Color.Red, 3);
            g.DrawEllipse(p, X - Radius / 2, Y - Radius / 2, 3, 3);
            p.Dispose();

        }
    }
}
