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
using System.Windows.Forms.VisualStyles;

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
            btnColour.BackColor = SelectedColour;
            this.EditingRobot.PreviewMode = true;

            foreach (var seg in EditingRobot.Segments.Reverse<Segment>() )
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.EditingRobot.AddSegment((double)nudLength.Value, this.SelectedColour);
            lbSegments.Items.Insert(0, this.EditingRobot.Segments.Last());
            pnlPreview.Invalidate();

            Modified = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int ind = lbSegments.SelectedIndex;
            if (ind == -1) return;

            lbSegments.Items.RemoveAt(ind);

            ind = this.EditingRobot.Length - ind - 1; // Vistinski indeks

            this.EditingRobot.RemoveSegement(ind);
            pnlPreview.Invalidate();

            Modified = true;
        }
    }
}
