using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace Курсовой_Проект
{
    public partial class Form1 : Form
    {
        public bool click = false;
        public Point lastPoint;
        public Console console;

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();

            console = new Console();
            console.Show();

            OpenGl.Init(AnT);                             //инициализировать openGl
            AnT.Size = new Size(Width, Height);
            OpenGl.ReSizeGLScene(AnT, Width, Height);

            Scene.LoadAse("Objects\\mustang_GT.ase");     //загрузить объект

            //пол
            Scene.Rectangle("land", new double[] { 4000, 0, 4000 }, new double[] { 4000, 0, -4000 }, new double[] { -4000, 0, -4000 }, new double[] { -4000, 0, 4000 }, 
                            new List<double[]> { new double[] { 10, 10, 0 }, new double[] { 10, -10, 0 }, new double[] { -10, -10, 0 }, new double[] { -10, 10, 0 } }, new Texture(new List<string> { "Textures\\no-translate-detected_318-30285.jpg" }));
            //стены
            Scene.Rectangle("wall1", new double[] { 4000, 500, 4000 }, new double[] { 4000, 500, -4000 }, new double[] { 4000, 0, -4000 }, new double[] { 4000, 0, 4000 });
            Scene.Rectangle("wall2", new double[] { 4000, 500, -4000 }, new double[] { -4000, 500, -4000 }, new double[] { -4000, 0, -4000 }, new double[] { 4000, 0, -4000 });
            Scene.Rectangle("wall3", new double[] { -4000, 500, -4000 }, new double[] { -4000, 500, 4000 }, new double[] { -4000, 0, 4000 }, new double[] { -4000, 0, -4000 });
            Scene.Rectangle("wall4", new double[] { -4000, 500, 4000 }, new double[] { 4000, 500, 4000 }, new double[] { 4000, 0, 4000 }, new double[] { -4000, 0, 4000 });

            //событие на движение колесика мышки
            this.MouseWheel += new MouseEventHandler(this.Form1_Wheel);

            timer1.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {     
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            AnT.Size = new Size(Width, Height);
            OpenGl.ReSizeGLScene(AnT, Width, Height);
        }
        private void Form1_Wheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                Scene.Z += 10;
            else
                Scene.Z -= 10;
        }

        private void AnT_MouseDown(object sender, MouseEventArgs e)
        {
            click = true;
            lastPoint = e.Location;
        }
        private void AnT_MouseMove(object sender, MouseEventArgs e)
        {
            if (click) {
                Scene.angleY += (double)(e.Location.X - lastPoint.X) / 5;
                Scene.angleX += (double)(e.Location.Y - lastPoint.Y) / 5;

                lastPoint = e.Location;
            }
        }
        private void AnT_MouseUp(object sender, MouseEventArgs e)
        {
            click = false;
        }

        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.W:
                    Scene.CarSpeed += 0.5;
                    break;
                case Keys.S:
                    Scene.CarSpeed -= 0.5;
                    break;
                case Keys.A:
                    Scene.keyUp = true;
                    Scene.AngleFrontWheel -= 3;
                    break;
                case Keys.D:
                    Scene.keyUp = true;
                    Scene.AngleFrontWheel += 3;
                    break;
            }
        }
        private void AnT_KeyUp(object sender, KeyEventArgs e)
        {
            Scene.keyUp = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Scene.DrawScene();

            AnT.Invalidate();

            //textBox1.AppendText(Scene.SpeedAngleWheel + "\r\n");
            //console.textBox1.AppendText(Scene.translateVector.X + " " + Scene.translateVector.Y + "\r\n");
        }
    }
}
