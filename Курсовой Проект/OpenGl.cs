using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.DevIl;

namespace Курсовой_Проект
{
    static class OpenGl
    {
        public static bool Light = true;
        public static bool FS = false;  //включен ли полноэкранный режим
        public static void Init(SimpleOpenGlControl AnT) {
            
            Glut.glutInit();        // инициализация бибилиотеки glut 
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);   // инициализация режима экрана

            Gl.glClearColor(1, 1, 1, 1);                // установка цвета очистки экрана (RGBA) 
            Gl.glViewport(0, 0, AnT.Width, AnT.Height); // установка порта вывода 
            Gl.glMatrixMode(Gl.GL_PROJECTION);          // активация проекционной матрицы
            Gl.glLoadIdentity();                        // очистка матрицы 
            Glu.gluPerspective(45, (float)AnT.Width / AnT.Height, 1, 500);   // установка перспективы
            Gl.glMatrixMode(Gl.GL_MODELVIEW);           // установка объектно-видовой матрицы 
            Gl.glLoadIdentity();                        // очистка матрицы 

            // начальные настройки OpenGL
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_LIGHT1);
            Gl.glEnable(Gl.GL_LIGHT2);
            Gl.glEnable(Gl.GL_LIGHT3);

            Gl.glLightModeli(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_TRUE);
            Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, new float[] { 1.0f, 1.0f, 1.0f, 1.0f });

            ////////////////////
            
            //Разрешить плавное цветовое сглаживание; 
            Gl.glShadeModel(Gl.GL_SMOOTH);
            // Слегка улучшим вывод перспективы; 
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
              
            // Разрешаем смешивание; 
            Gl.glEnable(Gl.GL_BLEND);
            // Устанавливаем тип смешивания; 
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            ///////////////////
            //туман

            Gl.glEnable(Gl.GL_FOG);                                             // Включает туман (GL_FOG)
            Gl.glFogi(Gl.GL_FOG_MODE, Gl.GL_LINEAR);                            // Выбираем тип тумана
            Gl.glFogfv(Gl.GL_FOG_COLOR, new float[] { 0.9f, 0.9f, 0.9f});       // Устанавливаем цвет тумана
            Gl.glFogf(Gl.GL_FOG_DENSITY, 0.07f);                                // Насколько густым будет туман
            Gl.glHint(Gl.GL_FOG_HINT, Gl.GL_NICEST);                            // Вспомогательная установка тумана
            Gl.glFogf(Gl.GL_FOG_START, 1.0f);                                   // Глубина, с которой начинается туман
            Gl.glFogf(Gl.GL_FOG_END, 4000.0f);                                  // Глубина, где туман заканчивается.

        }

        public static void ReSizeGLScene(SimpleOpenGlControl AnT,int width, int height)
        {
            if (height == 0) { height = 1; };
            Gl.glViewport(0, 0, width, height);

            Gl.glViewport(0, 0, AnT.Width, AnT.Height); // установка порта вывода 
            Gl.glMatrixMode(Gl.GL_PROJECTION);          // активация проекционной матрицы
            Gl.glLoadIdentity();                        // очистка матрицы 

            // установка перспективы
            Glu.gluPerspective(45, (double)AnT.Width / AnT.Height, 1, 10000);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);           // установка объектно-видовой матрицы 
            Gl.glLoadIdentity();                        // очистка матрицы 
        }

        public static void ScreenMode(Form1 form, SimpleOpenGlControl AnT, bool fullscreen)
        {   // Присваиваем значение "глобальной" переменной; 
            FS = fullscreen;
            if (FS) {
                // *** ПОЛНОЭКРАННЫЙ РЕЖИМ *** 
                // Скрываем рамку окна; 
                form.FormBorderStyle = FormBorderStyle.None;
                // Разворачиваем окно; 
                form.WindowState = FormWindowState.Maximized;

                Cursor.Hide();
            } else {
                // *** ОКОННЫЙ РЕЖИМ *** 
                // Возвращаем состояние окна; 
                form.WindowState = FormWindowState.Normal;
                // Показываем масштабируемую рамку окна; 
                form.FormBorderStyle = FormBorderStyle.FixedSingle;

                Cursor.Show();
                
                // Задаем размеры окна; 
                form.Width = 1280;
                // Ширина; 
                form.Height = 720;
                // Высота; 
            }
            ReSizeGLScene(AnT, form.Width, form.Height);
        }

        public static int Add_RGB(Bitmap B, Size S)
        {
            int texObject;
            Gl.glGenTextures(1, out texObject);
            BitmapData bitmapdata = new BitmapData();
            Bitmap image = new Bitmap(B, S.Width, S.Height);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            bitmapdata = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            // создаем привязку к только что созданной текстуре
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texObject);

            // устанавливаем режим фильтрации и повторения текстуры
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);
          
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, image.Width, image.Height, 0, Gl.GL_BGR_EXT, Gl.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

            return texObject;
        }

        struct TextureImage
        {
            public byte[] imageData;    // Image Data (Up To 32 Bits)
            public int bpp; // Image Color Depth In Bits Per Pixel.
            public int width;   // Image Width
            public int height;  // Image Height
            public int texID;   // Texture ID Used To Select A Texture
        }

        public static bool Compare(byte[] array1, byte[] array2) // this is called Helper.compare
        {
            int length1 = array1.Length;
            int length2 = array2.Length;
            if (length1 != length2)
            {
                return false;
            }
            // if continue the 2 lenghts are the same
            for (int i = 0; i < length1; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static int LoadTGA(string filename) // this function return the integer texture id
        {
            byte[] uTGACompare = { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };    // Uncompressed TGA Header
            byte[] cTGACompare = { 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0 };   // Compressed TGA Header
            byte[] TGACompare = new byte[12];   // Used To Compare TGA Header

            FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read); // Open The TGA File
            file.Read(TGACompare, 0, 12);
            if (Compare(TGACompare, uTGACompare))
            {
                return LoadUncompressedTGA(file, filename);
            }

            file.Close();
            return 0;
        }

        static int LoadUncompressedTGA(FileStream file, string name)
        {
            TextureImage texture;

            byte[] header = new byte[6];    // First 6 Useful Bytes From The Header
            int bytesPerPixel;  // Holds Number Of Bytes Per Pixel Used In The TGA File
            int imageSize;  // Used To Store The Image Size When Setting Aside Ram
            int temp;   // Temporary Variable
            int type = Gl.GL_RGBA;  // Set The Default GL Mode To RBGA (32 BPP)

            if (file == null || file.Read(header, 0, 6) != 6)
            {
                if (file == null)
                    return -1;
                else
                {
                    file.Close();
                    return -1;
                }
            }
            texture.width = header[1] * 256 + header[0];    // Determine The TGA Width	(highbyte*256+lowbyte)
            texture.height = header[3] * 256 + header[2];   // Determine The TGA Height	(highbyte*256+lowbyte)

            if (texture.width <= 0 || texture.height <= 0 || (header[4] != 24 && header[4] != 32))  // Is The TGA 24 or 32 Bit?
            {
                file.Close();
                return -1;
            }
            texture.bpp = header[4];    // Grab The TGA's Bits Per Pixel (24 or 32)
            bytesPerPixel = texture.bpp / 8;    // Divide By 8 To Get The Bytes Per Pixel
            imageSize = texture.width * texture.height * bytesPerPixel; // Calculate The Memory Required For The TGA Data 
            texture.imageData = new byte[imageSize];    // Reserve Memory To Hold The TGA Data
            if (imageSize == 0 || file.Read(texture.imageData, 0, imageSize) != imageSize)
            {
                if (texture.imageData != null)
                    texture.imageData = null;

                file.Close();
                return -1;
            }
            for (int i = 0; i < imageSize; i += bytesPerPixel)  // Loop Through The Image Data
            {   // Swaps The 1st And 3rd Bytes ('R'ed and 'B'lue)
                temp = texture.imageData[i];    // Temporarily Store The Value At Image Data 'i'
                texture.imageData[i] = texture.imageData[i + 2];    // Set The 1st Byte To The Value Of The 3rd Byte
                texture.imageData[i + 2] = (byte)temp;  // Set The 3rd Byte To The Value In 'temp' (1st Byte Value)
            }

            file.Close();
            // Build A Texture From The Data
            int[] textureArray = new int[1];
            
            Gl.glGenTextures(1, textureArray);  // Generate OpenGL texture IDs

            texture.texID = textureArray[0];

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureArray[0]);    // Bind Our Texture
            if (texture.bpp == 24)  // Was The TGA 24 Bits
            {
                type = Gl.GL_RGB;   // If So Set The 'type' To GL_RGB
            }

            //if the texture is intended to be rendered anisotropically
            //if (ContentManager.IsAnisotropic(name) && maximumAnisotropy != 0)
            //{
            //    Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, maximumAnisotropy);  // anisotropic Filtered
            //}

            // устанавливаем режим фильтрации и повторения текстуры
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);   // Linear Filtered
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);   // Linear Filtered
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE);

            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, 4, texture.width, texture.height, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, texture.imageData);


            texture.texID = textureArray[0];
            return texture.texID;
        }
    }
}
