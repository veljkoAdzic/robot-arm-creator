using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace Build_A_Bot
{
    public partial class Form1 : Form
    {
        public Robot Rob { get; set; }
        public String FileLocation { get; set; }
        public bool Modified { get; set; }
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Rob = new Robot(this.Width / 2, this.Height - 100, this.Width, this.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FileLocation = Application.StartupPath + "\\Assets\\DefaultRobot.rarm";
            loadFromFile();
            this.FileLocation = null;
            this.Modified = false;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.FromArgb(230, 230, 230));
            Rob.Show(e.Graphics);
            this.Text = (Modified ? "* " : "" ) + 
                "Креатор на Роботска Рака" + 
                (FileLocation != null ? " | " + FileLocation : "");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Rob.Update();
            Invalidate();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Rob.Update(e.X, e.Y);
            Invalidate();
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            Rob.BallUpdate();
            timer1.Start();
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            saveToFile();
        }
        private void tsbSaveAs_Click(object sender, EventArgs e)
        {
            this.FileLocation = null;
            saveToFile();
        }
        private void tsbNew_Click(object sender, EventArgs e)
        {
            if (this.Modified)
            {

                DialogResult ans = MessageBox.Show(
                    "Не се зачувани промени! Дали сакате да зачувате?",
                    "Не зачувана работа",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    );
                if (ans == DialogResult.OK) saveToFile();
            }

            BuilderMenu menu = new BuilderMenu();
            timer1.Stop();
            if (menu.ShowDialog() == DialogResult.OK && menu.Modified)
            {
                this.Modified = true;
                this.FileLocation = null;
                this.Rob.BuildFrom(menu.EditingRobot.Segments, true);
            }
            timer1.Start();

        }
        private void tsbLoad_Click(object sender, EventArgs e)
        {
            if (this.Modified)
            {
                
                DialogResult ans = MessageBox.Show(
                    "Не се зачувани промени! Дали сакате да зачувате?", 
                    "Не зачувана работа", 
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    );
                if (ans == DialogResult.OK) saveToFile();
            }
            this.FileLocation = null;
            loadFromFile();
            Invalidate();
        }

        private void tsbBuild_Click(object sender, EventArgs e)
        {
            BuilderMenu menu = new BuilderMenu(this.Rob);
            timer1.Stop();
            if (menu.ShowDialog() == DialogResult.OK && menu.Modified)
            {
                this.Modified = this.Modified || menu.Modified;
                this.Rob.BuildFrom(menu.EditingRobot.Segments, true);
            }
            timer1.Start();

        }

        private void saveToFile()
        {
            if(FileLocation == null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Robot Arm File (*.rarm)|*.rarm";
                sfd.Title = "Зачувај Робот";
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    FileLocation = sfd.FileName;
                }

            }

            if( FileLocation != null)
            {
                using(FileStream fs = new FileStream(FileLocation, FileMode.OpenOrCreate))
                {
                    IFormatter ifmt = new BinaryFormatter();
                    ifmt.Serialize(fs, this.Rob.Segments);
                }

                this.Modified = false;
            }

        }
        private void loadFromFile()
        {
            if(FileLocation == null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Robot Arm File (*.rarm)|*.rarm";
                ofd.Title = "Вчитај Робот";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileLocation = ofd.FileName;
                }
            }

            if(FileLocation != null)
            {
                try
                {
                    List<Segment> segs;
                    using (FileStream fs = new FileStream(FileLocation, FileMode.Open))
                    {
                        IFormatter fmt = new BinaryFormatter();
                        segs = (List<Segment>) fmt.Deserialize(fs);
                    }

                    this.Rob.BuildFrom(segs);
                    this.Modified = false;
                } catch (Exception)
                {
                    //
                }
            }
        }

    }
}
