using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NearestLine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            PointF[] pf = new PointF[2] { new PointF(20,20), new PointF(20,120) };
            list.Add(pf);
            pf = new PointF[2] { new PointF(120, 20), new PointF(120, 120) };
            list.Add(pf);
            pf = new PointF[2] { new PointF(30, 110), new PointF(130, 110) };
            list.Add(pf);
            pf = new PointF[2] { new PointF(200, 110), new PointF(330, 110) };
            list.Add(pf);
            pf = new PointF[2] { new PointF(20, 110), new PointF(130, 310) };
            list.Add(pf);
            pf = new PointF[2] { new PointF(220, 110), new PointF(230, 310) };
            list.Add(pf);            
        }

        public List<PointF[]> list = new List<PointF[]>();
        
        Color[] cls = new Color[] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Fuchsia, Color.Orange };
        public class Line
        {
            public int id;
            public Point p1;
            public Point p2;
            public Color c;

            public float k
            {
                get
                {
                    return ((float)p2.Y - (float)p1.Y) / ((float)p2.X - (float)p1.X);
                }
            }

            public float b
            {
                get
                {
                    return (float)p1.Y - k * (float)p1.X;
                }
            }
        }
        List<Line> lines = new List<Line>();
        Rectangle r = new Rectangle(200, 200, 100, 100);

        private void Form1_Load(object sender, EventArgs e)
        {
            button1_Click(this, e);
        }

        /// <summary>
        ///     DISTANCE
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="pointOnLine"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        private double DistanceFromPointToLine(PointF pt, PointF lineStart, PointF lineEnd, out PointF pointOnLine, out int side)
        {
            float dx = lineEnd.X - lineStart.X;
            float dy = lineEnd.Y - lineStart.Y;

            side = Math.Sign((lineEnd.X - lineStart.X) * (pt.Y - lineStart.Y) - (lineEnd.Y - lineStart.Y) * (pt.X - lineStart.X));

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - lineStart.X) * dx + (pt.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                pointOnLine = new PointF(lineStart.X, lineStart.Y);
                dx = pt.X - lineStart.X;
                dy = pt.Y - lineStart.Y;
            }
            else if (t > 1)
            {
                pointOnLine = new PointF(lineEnd.X, lineEnd.Y);
                dx = pt.X - lineEnd.X;
                dy = pt.Y - lineEnd.Y;
            }
            else
            {
                pointOnLine = new PointF(lineStart.X + t * dx, lineStart.Y + t * dy);
                dx = pt.X - pointOnLine.X;
                dy = pt.Y - pointOnLine.Y;
            };

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            DrawGraph(new Point(e.X, e.Y));                        
        }

        /// <summary>
        ///     Randomly fill lines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            lines.Clear();            

            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                Line l = new Line();
                l.id = i;
                l.c = cls[rnd.Next(cls.Length - 1)];
                l.p1 = new Point(rnd.Next(pictureBox1.Width), rnd.Next(pictureBox1.Height));
                l.p2 = new Point(rnd.Next(pictureBox1.Width), rnd.Next(pictureBox1.Height));
                lines.Add(l);
            };

            DrawGraph(new Point(-1,-1));
        }

        private void DrawGraph(Point e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);
            for (int i = 0; i < lines.Count; i++)
            {
                g.DrawLine(new Pen(new SolidBrush(lines[i].c)), lines[i].p1, lines[i].p2);
                g.DrawString(lines[i].id.ToString(), new Font("MS Sans Serif", 8, FontStyle.Regular), new SolidBrush(lines[i].c), lines[i].p2);
            };
            if (e.X > 0)
            {
                r = new Rectangle(e.X - 50, e.Y - 50, 100, 100);

                g.DrawLine(new Pen(new SolidBrush(Color.Beige)), r.Left, r.Top, r.Right, r.Top);
                g.DrawLine(new Pen(new SolidBrush(Color.Beige)), r.Right, r.Top, r.Right, r.Bottom);
                g.DrawLine(new Pen(new SolidBrush(Color.Beige)), r.Right, r.Bottom, r.Left, r.Bottom);
                g.DrawLine(new Pen(new SolidBrush(Color.Beige)), r.Left, r.Bottom, r.Left, r.Top);

                g.DrawEllipse(new Pen(new SolidBrush(Color.Beige), 2), e.X, e.Y, 3, 3);

                
                textBox1.Clear();
                // 1st Step select lines
                List<Line> selLines = new List<Line>();
                for (int i = 0; i < lines.Count; i++)
                {
                    if ((lines[i].p1.X < r.Left) && (lines[i].p2.X < r.Left)) continue;
                    if ((lines[i].p1.X > r.Right) && (lines[i].p2.X > r.Right)) continue;
                    if ((lines[i].p1.Y < r.Top) && (lines[i].p2.Y < r.Top)) continue;
                    if ((lines[i].p1.Y > r.Bottom) && (lines[i].p2.Y > r.Bottom)) continue;
                    selLines.Add(lines[i]);
                };
                textBox1.Text = "Lines across zone:";
                for (int i = 0; i < selLines.Count; i++) textBox1.Text += " " + selLines[i].id.ToString() + " ";
                textBox1.Text += "\r\n";

                // 2st Step lines cross area
                Line[] around = selLines.ToArray();
                selLines.Clear();
                for (int i = 0; i < around.Length; i++)
                {
                    float y1 = around[i].k * (float)r.Left + around[i].b;
                    float y2 = around[i].k * (float)r.Right + around[i].b;
                    float x1 = ((float)r.Top - around[i].b) / around[i].k;
                    float x2 = ((float)r.Bottom - around[i].b) / around[i].k;
                    if (
                        ((y1 >= r.Top) && (y1 <= r.Bottom))  
                        || 
                        ((y2 >= r.Top) && (y2 <= r.Bottom))
                        || 
                        ((x1 >= r.Left) && (x1 <= r.Right))
                        ||
                        ((x2 >= r.Left) && (x2 <= r.Right))
                        )
                        selLines.Add(around[i]);
                };
                textBox1.Text += "Lines in zone zone:";
                for (int i = 0; i < selLines.Count; i++) textBox1.Text += " " + selLines[i].id.ToString() + " ";
                textBox1.Text += "\r\n";

                PointF pf = new PointF(e.X, e.Y);

                double len = double.MaxValue;
                int line = -1;
                PointF online = new PointF(0, 0);
                int side = 0;

                // 3rd step get nearest
                for (int i = 0; i < selLines.Count; i++)
                {
                    PointF pol;
                    int lor = 0;
                    double d = DistanceFromPointToLine(pf, selLines[i].p1, selLines[i].p2, out pol, out lor);
                    if (d < len)
                    {
                        len = d;
                        line = i;
                        online = pol;
                        side = lor;
                    };
                };

                if (line >= 0)
                {
                    g.DrawEllipse(new Pen(new SolidBrush(selLines[line].c), 2), online.X, online.Y, 3, 3);
                    textBox1.Text += "Nearest Line: " + selLines[line].id.ToString() + "\r\nDistance: " + len + "\r\nOn-line point: " + online.X + ":" + online.Y + "\r\nLeft-or-Right: " + side;

                    // perpend
                    g.DrawLine(new Pen(new SolidBrush(selLines[line].c)), pf, online);

                    // on line route
                    if(side > 0) // or one-way
                        g.DrawLine(new Pen(new SolidBrush(selLines[line].c),2), online, selLines[line].p2);
                    else
                        g.DrawLine(new Pen(new SolidBrush(selLines[line].c),3), selLines[line].p1, online);
                }
                else
                    textBox1.Text += "No lines found around";
            };
            g.Dispose();
            pictureBox1.Image = bmp;
        }
    }
}