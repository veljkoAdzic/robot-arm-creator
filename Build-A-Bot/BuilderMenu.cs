using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
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

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        public BuilderMenu(Robot r)
        {
            InitializeComponent();

            this.EditingRobot = new Robot(pnlPreview.Width / 2, pnlPreview.Height - 50, pnlPreview.Width, pnlPreview.Height);
            
            this.EditingRobot.BuildFrom(r.Segments, r.EndEffector, true);

            this.Modified = false;


            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
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

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;

            this.SelectedColour = Segment.DEFAULT_COLOUR;
            btnColour.BackColor = SelectedColour;
            this.EditingRobot.PreviewMode = true;

            foreach (var seg in EditingRobot.Segments.Reverse<Segment>() )
            {
                lbSegments.Items.Add(seg);
            }

            // Efektor combo box posavuvanje
            cbEffector.Items.Clear();
            foreach (EffectorType item in Enum.GetValues(typeof(EffectorType)))
            { 
                cbEffector.Items.Add(item);
            }

            cbEffector.SelectedItem = EditingRobot.EndEffector.Type;

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

        private void btnUp_Click(object sender, EventArgs e)
        {// Pomesti selektiran segment nagore (napred vo lista)
            int ind = lbSegments.SelectedIndex;
            if (ind <= 0) return;

            // Zamena vo lista
            Segment tmp = lbSegments.Items[ind] as Segment;
            lbSegments.Items.RemoveAt(ind);
            lbSegments.Items.Insert(ind - 1, tmp);
            lbSegments.SelectedIndex = ind - 1;

            ind = EditingRobot.Length - ind - 1; // Inverzija na indeks
            
            // Zamena vo Robot
            tmp = EditingRobot.Segments[ind];
            EditingRobot.Segments.RemoveAt(ind);
            EditingRobot.Segments.Insert(ind+1, tmp);

            // Update, Iscrtuvanje i modifikacija
            Vector2 end = EditingRobot.Segments.Last().end;
            EditingRobot.Update(end.X, end.Y);
            pnlPreview.Invalidate();
            this.Modified = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {// Pomesti selektiran segment nadolu (nazad vo lista)
            int ind = lbSegments.SelectedIndex;
            if (ind < 0 || ind >= EditingRobot.Length-1) return;

            // Zamena vo lista
            Segment tmp = lbSegments.Items[ind] as Segment;
            lbSegments.Items.RemoveAt(ind);
            lbSegments.Items.Insert(ind + 1, tmp);
            lbSegments.SelectedIndex = ind + 1;

            ind = EditingRobot.Length - ind - 1; // Inverzija na indeks

            // Zamena vo Robot
            tmp = EditingRobot.Segments[ind];
            EditingRobot.Segments.RemoveAt(ind);
            EditingRobot.Segments.Insert(ind - 1, tmp);

            // Update, Iscrtuvanje i modifikacija
            Vector2 end = EditingRobot.Segments.Last().end;
            EditingRobot.Update(end.X, end.Y);
            pnlPreview.Invalidate();
            this.Modified = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Modified = false;
            Form1 f = Parent as Form1;
            DialogResult = DialogResult.Cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(this.EditingRobot.Length == 0)
            {
                MessageBox.Show("Роботска рака мора да се состои од барем 1 сегмент!", "Невалиден робот", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Form1 f = Parent as Form1;
            DialogResult = DialogResult.OK;
        }
    }
}
