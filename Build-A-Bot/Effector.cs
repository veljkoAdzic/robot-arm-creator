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

        public Effector() : base() { }
        public Effector(Vector2 pos, EffectorType Type = EffectorType.Grabber)
        :base(pos, 50.0, 0, -360.0, 360.0)
        {
            this.Type = Type;
            this.Active = false;
        }

        public void Show(Graphics g)
        {
            Color jointColour = Color.FromArgb(98, 100, 102);
            float jointWidth = 18.0f;
            Pen p = new Pen(this.Colour, 10.0f);
            p.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
            Brush b = new SolidBrush(jointColour);

            switch (this.Type)
            {
                case EffectorType.Grabber:
                    double openAngle = Math.PI *0.30 * (this.Active ?0.5d : 1d) ;
                    double innerAngle = -Math.PI * 0.30 * 0.5;
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 endPart = new Vector2(0f);
                        endPart.X = (float)(Math.Cos(openAngle + angle + change) * this.len * 0.7) + pos.X;
                        endPart.Y = (float)(Math.Sin(openAngle + angle + change) * this.len * 0.7) + pos.Y;
                        g.DrawLine(p, pos.X, pos.Y, endPart.X, endPart.Y);
                        
                        Vector2 tip = new Vector2(0f);
                        tip.X = (float)(Math.Cos(innerAngle + angle + change) * this.len * 0.7) + endPart.X;
                        tip.Y = (float)(Math.Sin(innerAngle + angle + change) * this.len * 0.7) + endPart.Y;
                        g.DrawLine(p, endPart.X, endPart.Y, tip.X, tip.Y);

                        g.FillEllipse(b, endPart.X - jointWidth/3, endPart.Y - jointWidth/3, (int)(jointWidth/1.5), (int)(jointWidth/1.5));

                        openAngle *= -1;
                        innerAngle *= -1;
                    }
                break;

                case EffectorType.Laser:
                    // TODO
                break;
            }
            p.Dispose();
            // Iscrtuvanje na zglob
            jointWidth *= 0.55f;
            
            g.FillEllipse(b, pos.X - jointWidth, pos.Y - jointWidth, jointWidth * 2, jointWidth * 2);

            jointWidth *= 0.35f;
            b = new SolidBrush(this.Colour);
            g.FillEllipse(b, pos.X - jointWidth, pos.Y - jointWidth, jointWidth * 2, jointWidth * 2);

            b.Dispose();

        }
    }
}
