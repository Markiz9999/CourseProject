using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace Курсовой_Проект
{
    class Texture
    {

        private List<int> textureId = new List<int>();

        public List<int> TextureId { get { return textureId; } }

        public Texture(List<string> Paths) {
            foreach (string Path in Paths)
            {
                if (File.Exists(Path))
                {
                    string[] str = Path.Split('.');

                    if (str[str.Length - 1] == "tga")
                    {
                        textureId.Add(OpenGl.LoadTGA(Path));
                    }
                    else {
                        Bitmap bitmap = new Bitmap(Path);
                        textureId.Add(OpenGl.Add_RGB(bitmap, new Size(bitmap.Width, bitmap.Height)));
                    }
                }
            }
        }
    }
}
