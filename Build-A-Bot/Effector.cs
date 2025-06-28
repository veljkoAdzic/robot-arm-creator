using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Build_A_Bot
{

    public enum EffectorType
    {
        Grabber,
        Laser
    };

    public class Effector : Segment
    {
        public bool Active { get; set; }
        public EffectorType Type { get; set; }

        public Effector(Vector2 pos, EffectorType Type = EffectorType.Grabber)
        :base(pos, 50.0, 0, -360.0, 360.0)
        {
            this.Type = Type;
            this.Active = false;
        }

        public void Show(Graphics g)
        {
            // TODO
            Pen p = new Pen(this.Colour, 15.0f);

            g.DrawLine(p, this.pos.X, this.pos.Y, this.end.X, this.end.Y);

            p.Dispose();

        }
    }
}
