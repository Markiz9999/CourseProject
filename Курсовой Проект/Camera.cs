using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.DevIl;

namespace Курсовой_Проект
{
    static class Camera
    {
        public static double angleX = -70;
        public static double angleZ = 0;

        private static double maxAngleX = 0;
        private static double minAngleX = -85;
        private static double maxAngleZ = 1.0 / 0;
        private static double minAngleZ = -1.0 / 0;

        public static int X = 0;
        public static int Y = 0;
        public static int Z = -120;

        public static int cameraPosition = 0;
        public static bool cameraType = true;

        public static double AngleX
        {
            get { return angleX; }
            set
            {
                if (value >= minAngleX && value < maxAngleX)
                    angleX = value % 360;
                else if (value < minAngleX)
                    angleX = minAngleX;
                else
                    angleX = maxAngleX;
            }
        }
        public static double AngleZ
        {
            get { return angleZ; }
            set
            {
                if (value >= minAngleZ && value < maxAngleZ)
                    angleZ = value % 360;
                else if (value < minAngleZ)
                    angleZ = minAngleZ;
                else
                    angleZ = maxAngleZ;
            }
        }

        public static void SetCamera()
        {
            switch (cameraPosition)
            {
                case 0:
                    Z = -120;
                    Y = 0;
                    X = 0;

                    AngleX = -70;
                    AngleZ = 0;

                    maxAngleX = 0;
                    minAngleX = -85;
                    maxAngleZ = 1.0 / 0;
                    minAngleZ = -1.0 / 0;

                    cameraType = true;
                    break;
                case 1:
                    Z = -28;
                    Y = 10;
                    X = 8;

                    AngleX = -70;
                    AngleZ = 0;

                    maxAngleX = -30;
                    minAngleX = -150;
                    maxAngleZ = 150;
                    minAngleZ = -150;

                    cameraType = false;
                    break;
                default:
                    break;
            }
        }

        public static void Set() {
            if (!cameraType)
            {
                //угол камеры
                Gl.glRotated(angleX, 1, 0, 0);
                Gl.glRotated(angleZ, 0, 0, 1);
                //растояние камеры
                Gl.glTranslated(X, Y, Z);
            }
            else
            {
                //растояние камеры
                Gl.glTranslated(X, Y, Z);
                //угол камеры
                Gl.glRotated(angleX, 1, 0, 0);
                Gl.glRotated(angleZ, 0, 0, 1);
            }
        }

    }
}
