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
        :base(pos, 50.0, 0)
        {
            this.Type = Type;
            this.Active = false;
        }

        // Copy konstruktor
        public Effector(Effector ot)
            :base(ot, ot.len, ot.Colour)
        {
            this.Type = ot.Type;
            this.Active = ot.Active;
        }

        public void Show(Graphics g)
        {
            Color jointColour = Color.FromArgb(98, 100, 102);
            float jointWidth = 18.0f;
            Pen p = new Pen(this.Colour, 10.0f);
            Brush b = new SolidBrush(jointColour);

            switch (this.Type)
            {
                case EffectorType.Grabber: // Isrctuvanje Pipalo
                    p.EndCap = System.Drawing.Drawing2D.LineCap.Triangle;
                    double openAngle = Math.PI *0.30 * (this.Active ?0.5d : 1d) ;
                    double innerAngle = -Math.PI * 0.30 * 0.5;
                    // Iscrtuvanje na prsti od pipalo
                    for (int i = 0; i < 2; i++)
                    {
                        // Presmetki i iscrtuvanje na dolen del od prst
                        Vector2 endPart = new Vector2(0f);
                        endPart.X = (float)(Math.Cos(openAngle + angle + change) * this.len * 0.7) + pos.X;
                        endPart.Y = (float)(Math.Sin(openAngle + angle + change) * this.len * 0.7) + pos.Y;
                        g.DrawLine(p, pos.X, pos.Y, endPart.X, endPart.Y);
                        
                        // Presmetki i iscrtuvanje na goren del od prst
                        Vector2 tip = new Vector2(0f);
                        tip.X = (float)(Math.Cos(innerAngle + angle + change) * this.len * 0.7) + endPart.X;
                        tip.Y = (float)(Math.Sin(innerAngle + angle + change) * this.len * 0.7) + endPart.Y;
                        g.DrawLine(p, endPart.X, endPart.Y, tip.X, tip.Y);

                        // Iscrtuvanje zglob na prst
                        g.FillEllipse(b, endPart.X - jointWidth/3, endPart.Y - jointWidth/3, (int)(jointWidth/1.5), (int)(jointWidth/1.5));

                        // Prevrtuvanje zada bidat simetrichni
                        openAngle *= -1;
                        innerAngle *= -1;
                    }
                break;
                
                case EffectorType.Laser: // Iscrtuvanje na laser
                    this.calculateEnd();
                    // Presmetka na normala 
                    Vector2 normal = new Vector2(0f);
                    normal.X = (float)(Math.Cos(angle + change + Segment.OFFSET));
                    normal.Y = (float)(Math.Sin(angle + change + Segment.OFFSET));

                    // Presmetka na boja na laserot
                    Color laserColour = this.Colour;
                    if (!this.Active)
                        laserColour = Color.FromArgb(
                            Math.Max(laserColour.R - 20, 0),
                            Math.Max(laserColour.G - 20, 0),
                            Math.Max(laserColour.B - 20, 0)
                        );

                    // Postavuvanje chetka i penkalo
                    Brush brush = new SolidBrush(laserColour);
                    p.Width = 5f;
                    p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    p.EndCap   = System.Drawing.Drawing2D.LineCap.Round;

                    // telo na laser
                    Point[] points = {
                        new Point((int)(pos.X + normal.X*jointWidth*0.5), (int)(pos.Y + normal.Y*jointWidth*0.5)),
                        new Point((int)(end.X + normal.X*jointWidth*0.1), (int)(end.Y + normal.Y*jointWidth*0.1)),
                        new Point((int)(end.X - normal.X*jointWidth*0.1), (int)(end.Y - normal.Y*jointWidth*0.1)),
                        new Point((int)(pos.X - normal.X*jointWidth*0.5), (int)(pos.Y - normal.Y*jointWidth*0.5))
                    };
                    g.FillPolygon(brush, points);


                    // prsteni
                    Vector2 section = new Vector2();
                    for(float i = 0.55f; i < 0.8; i += 0.2f)
                    {
                    section.X = pos.X + (float)(len*i * Math.Cos(change + angle));
                    section.Y = pos.Y + (float)(len*i * Math.Sin(change + angle));
                    g.DrawLine(p, 
                        section.X + normal.X * jointWidth*0.25f/i, section.Y + normal.Y * jointWidth*0.25f/i, 
                        section.X - normal.X * jointWidth*0.25f/i, section.Y - normal.Y * jointWidth*0.25f/i);
                    }

                    // Laser
                    if (this.Active && this.Type == EffectorType.Laser)
                    {
                    Vector2 laserEnd = new Vector2(0f);
                    laserEnd.X = end.X + (float)(1000 * Math.Cos(change + angle));
                    laserEnd.Y = end.Y + (float)(1000 * Math.Sin(change + angle));

                    p.Width = 7f;
                    g.DrawLine(p, end.X, end.Y, laserEnd.X, laserEnd.Y);

                    p.Width = 2f;
                    p.Color = Color.White;
                    g.DrawLine(p, end.X, end.Y, laserEnd.X, laserEnd.Y);
                    }

                    // vrv na laser
                    g.FillEllipse(brush, end.X - jointWidth * 0.25f, end.Y - jointWidth * 0.25f, jointWidth * 0.5f, jointWidth * 0.5f);


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
