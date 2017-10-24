using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.DevIl;

namespace Курсовой_Проект
{
    static class Scene
    {
        public static double angleX = -70;
        public static double angleY = 0;
        public static int Z = -150;

        //вращение колес
        private static double angleWheel = 0;           //угол поворота колеса

        //поворот передних колес
        private static double angleFrontWheel = 0;      //поворот колес право/лево
        private static double maxAngleFrontWheel = 30;  //максимальный поворот
        private static double angleBack = 4;            //скорость поворота колес в исходное положение
        public static bool keyUp = false;

        //перемещение автомобиля
        public static Vector translateVector = new Vector(0, 0);    //координаты автомобиля
        public static Vector directionVector = new Vector(0, 1);    //направление автомобиля
        public static double angle = 0;                             //угол поворота автомобиля
        public static double carSpeed = 0;                          //скорость автомобиля
        private static double reaction = -0.3;                      //сопротивление
        private static double maxSpeed = 15;                        //максимальная скорость
        private static double minSpeed = -7;                        //миниимальная скорость
        ///
        public static double AngleWheel {
            get { return angleWheel; }
            set {

                if (value < 360 && value > 0)
                    angleWheel = value;
                else if (value < 0)
                    angleWheel = 360 - value % 360;
                else
                    angleWheel = value % 360;
            }
        }
        ///
        public static double AngleFrontWheel {
            get { return angleFrontWheel; }
            set {
                if (value < maxAngleFrontWheel && value > -maxAngleFrontWheel)
                    angleFrontWheel = value;
                else {
                    if (value > 0)
                    {
                        angleFrontWheel = maxAngleFrontWheel;
                    }
                    else {
                        angleFrontWheel = -maxAngleFrontWheel;
                    }
                }
            }
        }
        ///
        public static Vector TranslateVector {
            get { return translateVector; }
            set {
                double width = translateVector.Subtract(value).Length();
                double r = 61.695 / Math.Sin(angleFrontWheel * Math.PI / 180.0);
                double w = width / r;

                if (CarSpeed > 0)
                {
                    directionVector = directionVector.Rotate(w);
                    angle += w * 180 / Math.PI;
                    AngleWheel += width * 180 / (19.4688 * Math.PI);
                }
                else
                {
                    directionVector = directionVector.Rotate(-w);
                    angle -= w * 180 / Math.PI;
                    AngleWheel -= width * 180 / (19.4688 * Math.PI);
                }
                translateVector = value;
            }
        }
        public static double CarSpeed {
            get { return carSpeed; }
            set {
                if (value <= maxSpeed && value >= minSpeed)
                    carSpeed = value;
                else
                {
                    if (value > 0)
                        carSpeed = maxSpeed;
                    if (value < 0)
                        carSpeed = minSpeed;
                }
            }
        }
        ///
        public static List<FileWithObjects> objectsInfo = new List<FileWithObjects>();
        /*
        public static void LoadObj(string fileName)
        {
            string[] allRows = File.ReadAllLines(fileName);

            string objName = "none";
            List<double[]> vertex = new List<double[]>();
            List<int[]> poly = new List<int[]>();

            for (int i = 0; i < allRows.Length; i++)
            {
                if (allRows[i] == String.Empty || allRows[i][0] == '#')
                    continue;

                switch (allRows[i][0])
                {
                    case 'v':
                        switch (allRows[i][1])
                        {
                            case ' ':
                                string[] str1 = allRows[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                vertex.Add(new Double[] { Convert.ToDouble(str1[1].Replace('.', ',')), Convert.ToDouble(str1[2].Replace('.', ',')), Convert.ToDouble(str1[3].Replace('.', ',')) });
                                break;
                            default:
                                break;
                        }
                        break;
                    case 'f':
                        string[] str = allRows[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        poly.Add(new int[] { Convert.ToInt32(str[1].Split('/')[0]), Convert.ToInt32(str[2].Split('/')[0]), Convert.ToInt32(str[3].Split('/')[0]) });
                        break;
                    default:
                        break;
                }
            }
            objectsInfo.Add(new Object3D(objName, vertex, poly, ".obj"));
        }
        */
        public static void LoadAse(string fileName)
        {
            string[] allRows = File.ReadAllText(fileName).Split('*');

            List<Object3D> objects = new List<Object3D>();
            List<int> objectsId = new List<int>();
            List<Texture> textures = new List<Texture>();

            int textureId = 0;

            for (int i = 0; i < allRows.Length; i++)
            {
                string firstWord = GetFirstWord(allRows[i]);

                if (firstWord == "MATERIAL")
                {
                    int brackets = 0;

                    List<string> paths = new List<string>();

                    for (; i < allRows.Length; i++)
                    {

                        firstWord = GetFirstWord(allRows[i]);

                        if (allRows[i].Contains("{")) brackets += CountWords(allRows[i], "{");
                        if (allRows[i].Contains("}")) brackets -= CountWords(allRows[i], "}");

                        switch (firstWord)
                        {
                            case "BITMAP":
                                string path = @"Textures\";
                                string[] str = fileName.Split(new Char[] { '\\', '.' });
                                path += str[str.Length - 2] + @"\";
                                str = allRows[i].Split(new char[] { ' ', '"' }, StringSplitOptions.RemoveEmptyEntries)[1].Split('\\');
                                path += str[str.Length - 1];
                                paths.Add(path);

                                break;
                            default:
                                break;
                        }

                        if (brackets == 0)
                        {
                            textures.Add(new Texture(paths));
                            break;
                        }
                    }
                }

                if (firstWord == "GEOMOBJECT")
                {
                    string objName = "";
                    List<double[]> vertex = new List<double[]>();
                    List<int[]> poly = new List<int[]>();
                    List<double[]> tVertex = new List<double[]>();
                    List<int[]> tPoly = new List<int[]>();
                    double[] position = new double[] { 0, 0, 0};

                    int brackets = 0;

                    for (; i < allRows.Length; i++)
                    {
                        firstWord = GetFirstWord(allRows[i]);

                        if (allRows[i].Contains("{")) brackets += CountWords(allRows[i], "{");
                        if (allRows[i].Contains("}")) brackets -= CountWords(allRows[i], "}");

                        string[] strPoint;

                        switch (firstWord) {
                            case "NODE_NAME":
                                objName = allRows[i].Split(new char[] { ' ', '"' }, StringSplitOptions.RemoveEmptyEntries)[1];
                                break;
                            case "TM_POS":
                                strPoint = allRows[i].Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                position = new Double[] { Convert.ToDouble(strPoint[1].Replace('.', ',')), Convert.ToDouble(strPoint[2].Replace('.', ',')), Convert.ToDouble(strPoint[3].Replace('.', ',')) };
                                break;
                            case "MESH_VERTEX":
                                strPoint = allRows[i].Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                vertex.Add(new Double[] { Convert.ToDouble(strPoint[2].Replace('.', ',')), Convert.ToDouble(strPoint[3].Replace('.', ',')), Convert.ToDouble(strPoint[4].Replace('.', ',')) });
                                break;
                            case "MESH_FACE":
                                strPoint = allRows[i].Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                poly.Add(new int[] { Convert.ToInt32(strPoint[3]), Convert.ToInt32(strPoint[5]), Convert.ToInt32(strPoint[7]) });
                                break;
                            case "MESH_TVERT":
                                strPoint = allRows[i].Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                tVertex.Add(new Double[] { Convert.ToDouble(strPoint[2].Replace('.', ',')), Convert.ToDouble(strPoint[3].Replace('.', ',')), Convert.ToDouble(strPoint[4].Replace('.', ',')) });
                                break;
                            case "MESH_TFACE":
                                strPoint = allRows[i].Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                tPoly.Add(new int[] { Convert.ToInt32(strPoint[2]), Convert.ToInt32(strPoint[3]), Convert.ToInt32(strPoint[4]) });
                                break;
                            case "MATERIAL_REF":
                                textureId = Convert.ToInt32(Regex.Replace(allRows[i], @"[^\d]+", ""));
                                break;
                            default:
                                break;
                        }

                        if (brackets == 0)
                        {
                            objects.Add(new Object3D(objName, vertex, poly, tVertex, tPoly, ".ase", textureId, position));
                            break;
                        }
                    }
                }
            }

            //создаем дисплейный список и записываем Id
            foreach (Object3D obj in objects)
            {
                objectsId.Add(CreateObj(obj, textures));
            }

            objectsInfo.Add(new FileWithObjects(objects, objectsId, textures));
        }

        public static int CreateObj(Object3D objects, List<Texture> textures)
        {
            int nom_l = Gl.glGenLists(1);       // получаем ID для создаваемого дисплейного списка
            
            Gl.glNewList(nom_l, Gl.GL_COMPILE); // генерируем новый дисплейный список

            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_NORMALIZE);

            Gl.glBegin(Gl.GL_TRIANGLES);

            for (int j = 0; j < objects.Poly.Count; j++)
            {

                float x1, y1, z1, x2, y2, z2, x3, y3, z3;
                float n1, n2, n3;
                float tx1, ty1, tx2, ty2, tx3, ty3;

                int k = 0;

                //определяем как начато индексирование (obj - с нуля, ase - с единицы)
                if(objects.ObjFileType == ".obj") k = 1;

                //добавляем в переменные для удобства записи/чтения
                x1 = (float)objects.Vertex[objects.Poly[j][0] - k][0];
                y1 = (float)objects.Vertex[objects.Poly[j][0] - k][1];
                z1 = (float)objects.Vertex[objects.Poly[j][0] - k][2];
                x2 = (float)objects.Vertex[objects.Poly[j][1] - k][0];
                y2 = (float)objects.Vertex[objects.Poly[j][1] - k][1];
                z2 = (float)objects.Vertex[objects.Poly[j][1] - k][2];
                x3 = (float)objects.Vertex[objects.Poly[j][2] - k][0];
                y3 = (float)objects.Vertex[objects.Poly[j][2] - k][1];
                z3 = (float)objects.Vertex[objects.Poly[j][2] - k][2];

                // рассчитываем нормаль 
                n1 = (y2 - y1) * (z3 - z1) - (y3 - y1) * (z2 - z1);
                n2 = (z2 - z1) * (x3 - x1) - (z3 - z1) * (x2 - x1);
                n3 = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);

                Gl.glNormal3f(n1, n2, n3);      // устанавливаем нормаль 

                if (objects.TPoly.Count > 0)
                {
                    tx1 = (float)objects.TVertex[objects.TPoly[j][0] - k][0];
                    ty1 = (float)objects.TVertex[objects.TPoly[j][0] - k][1];
                    tx2 = (float)objects.TVertex[objects.TPoly[j][1] - k][0];
                    ty2 = (float)objects.TVertex[objects.TPoly[j][1] - k][1];
                    tx3 = (float)objects.TVertex[objects.TPoly[j][2] - k][0];
                    ty3 = (float)objects.TVertex[objects.TPoly[j][2] - k][1];

                    Gl.glTexCoord2f(tx1, ty1);
                    Gl.glVertex3f(x1, y1, z1);
                    Gl.glTexCoord2f(tx2, ty2);
                    Gl.glVertex3f(x2, y2, z2);
                    Gl.glTexCoord2f(tx3, ty3);
                    Gl.glVertex3f(x3, y3, z3);
                }
                else {
                    Gl.glVertex3f(x1, y1, z1);
                    Gl.glVertex3f(x2, y2, z2);
                    Gl.glVertex3f(x3, y3, z3);
                }
            }

            Gl.glEnd();     // завершаем отрисовку
            Gl.glDisable(Gl.GL_NORMALIZE);
            Gl.glPopMatrix();

            Gl.glEndList();     // завершаем дисплейный список

            return nom_l;
        }

        public static void Rectangle(string name, double[] point1, double[] point2, double[] point3, double[] point4, List<double[]> tVertex = null, Texture texture = null) {

            List<Object3D> objects = new List<Object3D>();
            List<int> objectsId = new List<int>();
            List<Texture> textures = new List<Texture>();
            List<int[]> tPoly = new List<int[]>();

            if (texture != null) {
                textures.Add(texture);
            }
            if (tVertex == null)
            {
                tVertex = new List<double[]>(0);
            }
            else {
                tPoly = new List<int[]> { new int[] { 0, 1, 3 }, new int[] { 1, 2, 3 } };
            }
            

            Object3D obj = new Object3D(name, new List<double[]> { point1, point2, point3, point4 },
                                        new List<int[]> { new int[] { 0, 1, 3 }, new int[] { 1, 2, 3 } },
                                        tVertex, tPoly, ".ase", 0);

            objects.Add(obj);

            objectsId.Add(Scene.CreateObj(objects[0], textures));
            Scene.objectsInfo.Add(new FileWithObjects(objects, objectsId, textures));

        }

        public static void DrawScene() {

            Gl.glPushMatrix();

            Gl.glTranslated(0, 0, Z);

            Gl.glRotated(angleY, 0, 1, 0);
            Gl.glRotated(angleX, 1, 0, 0);

            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 500, 500, 500, 1});

            for (int i = 0; i < objectsInfo.Count; i++)
            {
                Gl.glEnable(Gl.GL_TEXTURE_2D);

                int j = 0;
                foreach (int obj in objectsInfo[i].ObjectsId)
                {
                    //перемещение автомобиля
                    if (i > 0) {
                        Gl.glRotated(angle, 0, 1, 0);
                        Gl.glTranslated(translateVector.X, 0, translateVector.Y);
                    }

                    //вращение колес
                    if (i == 0 && (j == 37 || j == 36 || j == 35 || j == 34))
                    {
                        Gl.glTranslated(objectsInfo[i].Objects[j].Position[0], objectsInfo[i].Objects[j].Position[1], objectsInfo[i].Objects[j].Position[2]);

                        //поворот колес
                        if (j == 36 || j == 35)
                        {
                            Gl.glRotated(-AngleFrontWheel, 0, 0, 1);
                        }

                        Gl.glRotated(-AngleWheel, 1, 0, 0);

                        Gl.glTranslated(-objectsInfo[i].Objects[j].Position[0], -objectsInfo[i].Objects[j].Position[1], -objectsInfo[i].Objects[j].Position[2]);
                    }

                    if (objectsInfo[i].Textures.Count > 0 && objectsInfo[i].Textures[objectsInfo[i].Objects[j].TextureId].TextureId.Count > 0)
                    {
                        for (int k = 0; k < 1; k++)
                        {
                            Gl.glActiveTextureARB(Gl.GL_TEXTURE0 + k);
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, objectsInfo[i].Textures[objectsInfo[i].Objects[j].TextureId].TextureId[k]);
                            Gl.glEnable(Gl.GL_TEXTURE_2D);
                        }

                        Gl.glCallList(obj);

                        for (int k = 0; k < 1; k++)
                        {
                            Gl.glActiveTextureARB(Gl.GL_TEXTURE0 + k);
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
                            Gl.glDisable(Gl.GL_TEXTURE_2D);
                        }
                    }
                    else {
                        Gl.glCallList(obj);
                    }

                    if (i == 0 && (j == 37 || j == 36 || j == 35 || j == 34))
                    {
                        Gl.glTranslated(objectsInfo[i].Objects[j].Position[0], objectsInfo[i].Objects[j].Position[1], objectsInfo[i].Objects[j].Position[2]);

                        Gl.glRotated(AngleWheel, 1, 0, 0);

                        //поворот колес
                        if (j == 36 || j == 35)
                        {
                            Gl.glRotated(AngleFrontWheel, 0, 0, 1);
                        }
                       
                        Gl.glTranslated(-objectsInfo[i].Objects[j].Position[0], -objectsInfo[i].Objects[j].Position[1], -objectsInfo[i].Objects[j].Position[2]);
                    }
                    //вернуть перемещение
                    if (i > 0)
                    {
                        Gl.glTranslated(-translateVector.X, 0, -translateVector.Y);
                        Gl.glRotated(-angle, 0, 1, 0);
                    }

                    j++;
                }
                if (i == 0)
                {
                    Gl.glRotated(90, 1, 0, 0);
                }
                Gl.glDisable(Gl.GL_TEXTURE);
            }

            Gl.glPopMatrix();

            Gl.glFlush();

            ////////////
            //добавить скорость, применить сопротивление
            TranslateVector = TranslateVector.Add(directionVector.Multiply(carSpeed));

            if (carSpeed + reaction > 0)
                carSpeed += reaction;
            else if (carSpeed - reaction < 0)
                carSpeed -= reaction;
            else
                carSpeed = 0;

            ///////////
            //вернуть колеса в исходное состояние
            if (!keyUp && angleFrontWheel != 0)
            {
                if (AngleFrontWheel > 0 && AngleFrontWheel - angleBack > 0)
                    AngleFrontWheel -= angleBack;
                else if (AngleFrontWheel < 0 && AngleFrontWheel - angleBack < 0)
                    AngleFrontWheel += angleBack;
                else
                    AngleFrontWheel = 0;
            }
        }

        private static string GetFirstWord(string str) {
            return str.Split(new char[] { ' ', '\t'})[0];
        }

        private static int CountWords(string s, string s0)
        {
            int count = (s.Length - s.Replace(s0, "").Length) / s0.Length;
            return count;
        }

    }
}
