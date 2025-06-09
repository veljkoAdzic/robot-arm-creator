using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Build_A_Bot
{
    public partial class BuilderMenu : Form
    {
        public Robot EditingRobot { get; set; }
        public bool Modified { get; set; }
        public Color SelectedColour { get; set; }
        public BuilderMenu()
        {
            InitializeComponent();
            this.EditingRobot = new Robot(pnlPreview.Width/2, pnlPreview.Height - 50, pnlPreview.Width, pnlPreview.Height);
            this.Modified = false;

        }

        public BuilderMenu(Robot r)
        {
            InitializeComponent();

            this.EditingRobot = new Robot(pnlPreview.Width / 2, pnlPreview.Height - 50, pnlPreview.Width, pnlPreview.Height);
            
            this.EditingRobot.BuildFrom(r.Segments, true);

            this.Modified = false;
        }

        private void btnColour_Click(object sender, EventArgs e)
        {
            colorDialog.AllowFullOpen = true;
            colorDialog.ShowHelp = true;
            colorDialog.Color = SelectedColour;

            if( colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.SelectedColour = colorDialog.Color;
                btnColour.BackColor = SelectedColour;
            }
        }

        private void pnlPreview_Paint(object sender, PaintEventArgs e)
        {
            this.EditingRobot.Show(e.Graphics);
        }

        private void BuilderMenu_Load(object sender, EventArgs e)
        {
            // StackOverflow is a godsent
            typeof(Panel).InvokeMember("DoubleBuffered", 
                BindingFlags.SetProperty | BindingFlags.Instance | 
                BindingFlags.NonPublic, null, pnlPreview, new object[] {true});

            this.SelectedColour = Segment.DEFAULT_COLOUR;
            this.EditingRobot.PreviewMode = true;

            foreach (var seg in EditingRobot.Segments)
            {
                lbSegments.Items.Add(seg);
            }

        }

        private void pnlPreview_MouseMove(object sender, MouseEventArgs e)
        {
            this.EditingRobot.FollowMouse = false;
            this.EditingRobot.Update(e.X, e.Y);
            pnlPreview.Invalidate();
        }
    }
}
