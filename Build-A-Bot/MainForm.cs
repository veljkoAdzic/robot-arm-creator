﻿using System;
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
using System.Text.Json;

namespace Build_A_Bot
{
    public partial class MainForm : Form
    {
        public Robot Rob { get; set; }
        public String FileLocation { get; set; }
        public bool Modified { get; set; }
        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Rob = new Robot(this.Width / 2, this.Height - 50, this.Width, this.Height);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.FileLocation = Application.StartupPath + "\\Assets\\DefaultRobot.rarm";
            loadFromFile();
            this.FileLocation = null;
            this.Modified = false;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
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

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            Rob.Update(e.X, e.Y);
            Invalidate();
        }

        private void MainForm_MouseLeave(object sender, EventArgs e)
        {
            Rob.BallUpdate();
            timer1.Start();
        }

        private void MainForm_MouseEnter(object sender, EventArgs e)
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
                this.Rob.BuildFrom(menu.EditingRobot.Segments, menu.EditingRobot.EndEffector, true);
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
                this.Rob.BuildFrom(menu.EditingRobot.Segments, menu.EditingRobot.EndEffector, true);
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
                using(FileStream fs = new FileStream(FileLocation, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    JsonSerializer.Serialize<Robot>(fs, this.Rob);
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
                    Robot newRob;
                    using (FileStream fs = new FileStream(FileLocation, FileMode.Open, FileAccess.Read))
                    {
                        newRob = JsonSerializer.Deserialize<Robot>(fs);
                    }
                    this.Rob.BuildFrom(newRob.Segments, newRob.EndEffector);
                    this.Modified = false;
                } catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            this.Rob.EndEffector.Active = true;
            Invalidate();
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            this.Rob.EndEffector.Active = false;
            Invalidate();
        }
    }
}
