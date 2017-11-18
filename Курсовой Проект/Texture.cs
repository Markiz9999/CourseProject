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
        public string MaterialClass = "";
        public int TextureId = 0;
        public float[] MaterialDiffuse;
        public float MaterialTransparency;
        public List<Texture> SubMaterial = new List<Texture>();

        public Texture(string path, string materialClass, float[] materialDiffuse, float materialTransparency, List<Texture> subMaterial)
        {
            MaterialClass = materialClass;
            MaterialDiffuse = materialDiffuse;
            MaterialTransparency = materialTransparency;
            SubMaterial = subMaterial;

            if (File.Exists(path))
            {
                string[] str = path.Split('.');

                if (str[str.Length - 1] == "tga")
                {
                    TextureId = OpenGl.LoadTGA(path);
                }
                else
                {
                    Bitmap bitmap = new Bitmap(path);
                    TextureId = OpenGl.Add_RGB(bitmap, new Size(bitmap.Width, bitmap.Height));
                }
            }
        }
    }
}
