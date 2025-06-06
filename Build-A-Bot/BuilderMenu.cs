using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Build_A_Bot
{
    public partial class BuilderMenu : Form
    {
        public Robot EditingRobot { get; set; }
        public bool Modified { get; set; }
        public BuilderMenu()
        {
            InitializeComponent();
            this.EditingRobot = new Robot(this.Width/2, this.Height - 50, this.Width, this.Height);
            this.Modified = false;
        }

        public BuilderMenu(Robot r)
        {
            InitializeComponent();
            this.EditingRobot = r;
            this.Modified = false;
        }



    }
}
