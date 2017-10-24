using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовой_Проект
{
    public partial class Console : Form
    {
        bool translate = false;
        Point lastPosition;
        public Console()
        {
            InitializeComponent();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            translate = true;
            lastPosition = e.Location;
        }

        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            translate = false;
        }

        private void textBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (translate) {
                this.Location = new Point(Location.X + e.Location.X - lastPosition.X, Location.Y + e.Location.Y - lastPosition.Y);
            }
        }

        public void WriteLine(string str) {
            textBox1.AppendText(str + "\r\n");
        }

        public void Write(string str)
        {
            textBox1.AppendText(str);
        }
    }
}
