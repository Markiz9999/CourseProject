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
        //список всех объектов в сцене
        public static List<FileWithObjects> objectsInfo = new List<FileWithObjects>();

        //вращение колес
        private static double angleWheel = 0;           //угол поворота колеса

        //поворот передних колес
        private static double angleFrontWheel = 0;      //поворот колес право/лево
        private static double maxAngleFrontWheel = 15;  //максимальный поворот
        private static double angleBack = 1;            //скорость поворота колес в исходное положение
        public static bool keyUp = false;

        //поворот руля
        private static double angleHelm = 0;            //угол поворота руля

        //перемещение автомобиля
        public static Vector translateVector = new Vector(0, 0);    //координаты автомобиля
        public static Vector directionVector = new Vector(0, 1);    //направление автомобиля
        public static double maxVectorLength = 1950;                //максимальная длина вектора перемещения
        public static double angle = 0;                             //угол поворота автомобиля
        public static double carSpeed = 0;                          //скорость автомобиля
        private static double reaction = -0.1;                      //сопротивление
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
                if (value < maxAngleFrontWheel && value > -maxAngleFrontWheel) {
                    angleFrontWheel = value;
                }
                else {
                    if (value > 0)
                    {
                        angleFrontWheel = maxAngleFrontWheel;
                    }
                    else {
                        angleFrontWheel = -maxAngleFrontWheel;
                    }
                }
                angleHelm = AngleFrontWheel * 20;
            }
        }
        ///
        public static Vector TranslateVector {
            get { return translateVector; }
            set {

                if (value.Length() > maxVectorLength) {
                    value = value.Unit().Multiply(maxVectorLength);
                }

                double length = translateVector.Subtract(value).Length();
                double r = 61.695 / Math.Sin(angleFrontWheel * Math.PI / 180.0);
                double w = length / r;

                double angleW = (length * 180) / (6.76 * Math.PI);

                if (CarSpeed > 0)
                {
                    directionVector = directionVector.Rotate(w);
                    angle += w * 180 / Math.PI;
                    AngleWheel += angleW;
                }
                else
                {
                    directionVector = directionVector.Rotate(-w);
                    angle -= w * 180 / Math.PI;
                    AngleWheel -= angleW;
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

        //загрузка объекта из файла ASE
        public static void LoadAse(string fileName)
        {
            string[] allRows = File.ReadAllText(fileName).Split(new char[] { '*', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            List<Object3D> objects = new List<Object3D>();
            List<int> objectsId = new List<int>();
            List<Texture> textures = new List<Texture>();

            int material = 0;

            for (int i = 0; i < allRows.Length; i++)
            {
                string firstWord = GetFirstWord(allRows[i]);

                if (firstWord == "MATERIAL")
                {
                    textures.Add(LoadMaterial(ref i, fileName, allRows));
                }

                if (firstWord == "GEOMOBJECT")
                {
                    string objName = "";
                    List<double[]> vertex = new List<double[]>();
                    List<int[]> poly = new List<int[]>();
                    List<int> subMaterial = new List<int>();
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
                            case "MESH_MTLID":
                                strPoint = allRows[i].Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                subMaterial.Add(Convert.ToInt32(strPoint[1]));
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
                                material = Convert.ToInt32(Regex.Replace(allRows[i], @"[^\d]+", ""));
                                break;
                            default:
                                break;
                        }

                        if (brackets == 0)
                        {
                            objects.Add(new Object3D(objName, vertex, poly, subMaterial, tVertex, tPoly, material, position));
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
        //загрузка материала из файла ASE (управление передается из ф-ции LoadAse)
        private static Texture LoadMaterial(ref int i, string fileName, string[] allRows) {

            string firstWord;

            int brackets = 1;

            string path = "";
            string materialClass = "";
            float[] materialDiffuse = new float[] { 1, 1, 1 };  //цвет материала
            float materialTransparency = 0;                     //прозрачность материала
            List<Texture> subMaterial = new List<Texture>();    //подцвета материала

            for (i++; i < allRows.Length; i++)
            {

                firstWord = GetFirstWord(allRows[i]);

                if (allRows[i].Contains("{")) brackets += CountWords(allRows[i], "{");
                
                string[] strBuff;       //буферная переменная

                switch (firstWord)
                {
                    case "MATERIAL_CLASS":
                        materialClass = allRows[i].Split(new char[] { ' ', '"' }, StringSplitOptions.RemoveEmptyEntries)[1];
                        break;
                    case "BITMAP":
                        path = @"Textures\";
                        strBuff = fileName.Split(new Char[] { '\\', '.' });
                        path += strBuff[strBuff.Length - 2] + @"\";
                        strBuff = allRows[i].Split(new char[] { ' ', '"' }, StringSplitOptions.RemoveEmptyEntries)[1].Split('\\');
                        path += strBuff[strBuff.Length - 1];
                        break;
                    case "MATERIAL_DIFFUSE":
                        strBuff = allRows[i].Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        materialDiffuse = new float[] { (float)Convert.ToDouble(strBuff[1].Replace('.', ',')), (float)Convert.ToDouble(strBuff[2].Replace('.', ',')), (float)Convert.ToDouble(strBuff[3].Replace('.', ',')) };
                        break;
                    case "MATERIAL_TRANSPARENCY":
                        strBuff = allRows[i].Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        materialTransparency = (float)Convert.ToDouble(strBuff[1].Replace('.', ','));
                        break;
                    case "SUBMATERIAL":
                        subMaterial.Add(LoadMaterial(ref i, fileName, allRows));
                        break;
                    default:
                        break;
                }

                if (allRows[i].Contains("}")) brackets -= CountWords(allRows[i], "}");

                if (brackets == 0)
                {
                    return new Texture(path, materialClass, materialDiffuse, materialTransparency, subMaterial);
                }
            }
            return null;
        }

        ///создание объекта в памяти на основании считанных данных из файла
        public static int CreateObj(Object3D objects, List<Texture> textures)
        {
            //отбираем полигоны, у которых одинаковые субматериалы
            //если нет субматериалов, записываем все точки
            List<int> subMaterials = new List<int>();
            List<int>[] polyList = new List<int>[] { new List<int>() };
            int i = 0;

            if (textures[objects.Material].MaterialClass != "Standard")
            {
                var temp = objects.SubMaterial.Distinct();

                foreach (int sub in temp)
                {
                    subMaterials.Add(sub);
                }

                polyList = new List<int>[subMaterials.Count];
                for (i = 0; i < polyList.Length; i++)
                {
                    polyList[i] = new List<int>();
                }

                for (i = 0; i < objects.Poly.Count; i++)
                {
                    for(int j = 0; j < subMaterials.Count; j++){
                        if(subMaterials[j] == objects.SubMaterial[i])
                            polyList[j].Add(i);
                    }
                }
            }
            else {
                polyList = new List<int>[] { new List<int>() };
                for (i = 0; i < objects.Poly.Count; i++)
                {
                    polyList[0].Add(i);
                }
            }
            //////////////////////////////////////////////////////////////////////////////////////

            int nom_l = Gl.glGenLists(1);       // получаем ID для создаваемого дисплейного списка

            Gl.glNewList(nom_l, Gl.GL_COMPILE); // генерируем новый дисплейный список

            Gl.glPushMatrix();

            Gl.glEnable(Gl.GL_NORMALIZE);

            Gl.glBegin(Gl.GL_TRIANGLES);

            i = 0;
             
            do
            {
                if (textures[objects.Material].MaterialClass != "Standard")
                {
                    Gl.glColor4f(textures[objects.Material].SubMaterial[subMaterials[i]].MaterialDiffuse[0],
                                    textures[objects.Material].SubMaterial[subMaterials[i]].MaterialDiffuse[1],
                                    textures[objects.Material].SubMaterial[subMaterials[i]].MaterialDiffuse[2],
                                    1 - textures[objects.Material].SubMaterial[subMaterials[i]].MaterialTransparency);
                    Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, textures[objects.Material].SubMaterial[subMaterials[i]].MaterialDiffuse);
                    Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, ArrayMultiply(textures[objects.Material].SubMaterial[subMaterials[i]].MaterialDiffuse, 0.2f));
                    Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, ArrayMultiply(textures[objects.Material].SubMaterial[subMaterials[i]].MaterialDiffuse, 0.2f));
                    Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, ArrayMultiply(textures[objects.Material].SubMaterial[subMaterials[i]].MaterialDiffuse, 0.2f));

                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[objects.Material].SubMaterial[subMaterials[i]].TextureId);
                }
                else
                {
                    Gl.glColor4f(textures[objects.Material].MaterialDiffuse[0],
                                    textures[objects.Material].MaterialDiffuse[1],
                                    textures[objects.Material].MaterialDiffuse[2],
                                    1 - textures[objects.Material].MaterialTransparency);
                    Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, textures[objects.Material].MaterialDiffuse);
                    Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, ArrayMultiply(textures[objects.Material].MaterialDiffuse, 0.2f));
                    Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, ArrayMultiply(textures[objects.Material].MaterialDiffuse, 0.2f));
                    Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, ArrayMultiply(textures[objects.Material].MaterialDiffuse, 0.2f));

                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[objects.Material].TextureId);
                }

                for (int j = 0; j < polyList[i].Count; j++)
                {

                    double x1, y1, z1, x2, y2, z2, x3, y3, z3;
                    double n1, n2, n3;
                    double tx1, ty1, tx2, ty2, tx3, ty3;

                    //добавляем в переменные для удобства записи/чтения
                    x1 = objects.Vertex[objects.Poly[polyList[i][j]][0]][0];
                    y1 = objects.Vertex[objects.Poly[polyList[i][j]][0]][1];
                    z1 = objects.Vertex[objects.Poly[polyList[i][j]][0]][2];
                    x2 = objects.Vertex[objects.Poly[polyList[i][j]][1]][0];
                    y2 = objects.Vertex[objects.Poly[polyList[i][j]][1]][1];
                    z2 = objects.Vertex[objects.Poly[polyList[i][j]][1]][2];
                    x3 = objects.Vertex[objects.Poly[polyList[i][j]][2]][0];
                    y3 = objects.Vertex[objects.Poly[polyList[i][j]][2]][1];
                    z3 = objects.Vertex[objects.Poly[polyList[i][j]][2]][2];

                    // рассчитываем нормаль 
                    n1 = (y2 - y1) * (z3 - z1) - (y3 - y1) * (z2 - z1);
                    n2 = (z2 - z1) * (x3 - x1) - (z3 - z1) * (x2 - x1);
                    n3 = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);

                    Gl.glNormal3d(n1, n2, n3);      // устанавливаем нормаль

                    if (objects.TPoly.Count > 0)
                    {
                        tx1 = objects.TVertex[objects.TPoly[polyList[i][j]][0]][0];
                        ty1 = objects.TVertex[objects.TPoly[polyList[i][j]][0]][1];
                        tx2 = objects.TVertex[objects.TPoly[polyList[i][j]][1]][0];
                        ty2 = objects.TVertex[objects.TPoly[polyList[i][j]][1]][1];
                        tx3 = objects.TVertex[objects.TPoly[polyList[i][j]][2]][0];
                        ty3 = objects.TVertex[objects.TPoly[polyList[i][j]][2]][1];

                        Gl.glTexCoord2d(tx1, ty1);
                        Gl.glVertex3d(x1, y1, z1);
                        Gl.glTexCoord2d(tx2, ty2);
                        Gl.glVertex3d(x2, y2, z2);
                        Gl.glTexCoord2d(tx3, ty3);
                        Gl.glVertex3d(x3, y3, z3);
                    }
                    else
                    {
                        Gl.glVertex3d(x1, y1, z1);
                        Gl.glVertex3d(x2, y2, z2);
                        Gl.glVertex3d(x3, y3, z3);
                    }
                }

                i++;

            } while (i < subMaterials.Count);

            Gl.glEnd();     // завершаем отрисовку
            Gl.glDisable(Gl.GL_NORMALIZE);
            Gl.glPopMatrix();

            Gl.glEndList();     // завершаем дисплейный список

            return nom_l;
        }
        //создание в памяти прямоугольника с заданными координатами
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
                                        new List<int[]> { new int[] { 0, 1, 3 }, new int[] { 1, 2, 3 } }, new List<int>(),
                                        tVertex, tPoly, 0, new double[] { 1.0, 1.0, 1.0});

            objects.Add(obj);

            objectsId.Add(Scene.CreateObj(objects[0], textures));
            Scene.objectsInfo.Add(new FileWithObjects(objects, objectsId, textures));

        }

        //отрисовка сцены
        public static void DrawScene() {

            Gl.glPushMatrix();

            Camera.Set();

            Gl.glRotated(angle, 0, 0, 1);
            Gl.glTranslated(translateVector.X, translateVector.Y, 0);
            //свет
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, new float[] { 1, 1, 1 });
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, new float[] { 0.2f, 0.2f, 0.2f });
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, new float[] { 0.2f, 0.2f, 0.2f });
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, new float[] { 0.2f, 0.2f, 0.2f });

            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, new float[] { 2000, 2000, 2000, 1 });
            Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, new float[] { -2000, -2000, 2000, 1 });
            Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, new float[] { -2000, 2000, 2000, 1 });
            Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, new float[] { 2000, -2000, 2000, 1 });

            Gl.glTranslated(-translateVector.X, -translateVector.Y, 0);
            Gl.glRotated(-angle, 0, 0, 1);
            ////////////////////////////////////////////////////////////

            for (int i = 0; i < objectsInfo.Count; i++)
            {
                int j = 0;
                foreach (int obj in objectsInfo[i].ObjectsId)
                {
                    //перемещение автомобиля
                    if (i > 0) {
                        Gl.glRotated(angle, 0, 0, 1);
                        Gl.glTranslated(translateVector.X, translateVector.Y, 0);
                    }

                    //вращение колес
                    if (i == 0 && (j == 25 || j == 26 || j == 28 || j == 29 || j == 27 || j == 34 || j == 35 || j == 36))
                    {
                        Gl.glTranslated(objectsInfo[i].Objects[j].Position[0], objectsInfo[i].Objects[j].Position[1], objectsInfo[i].Objects[j].Position[2]);

                        //поворот колес
                        if (j == 25 || j == 26 || j == 35 || j == 27)
                        {
                            Gl.glRotated(-AngleFrontWheel, 0, 0, 1);
                        }

                        Gl.glRotated(-AngleWheel, 1, 0, 0);

                        Gl.glTranslated(-objectsInfo[i].Objects[j].Position[0], -objectsInfo[i].Objects[j].Position[1], -objectsInfo[i].Objects[j].Position[2]);
                    }
                    //поворот тормозных колодок при повороте передних колес
                    if (i == 0)
                    {
                        switch (j)
                        {
                            case 31:
                                Gl.glTranslated(objectsInfo[i].Objects[35].Position[0], objectsInfo[i].Objects[35].Position[1], objectsInfo[i].Objects[35].Position[2]);
                                Gl.glRotated(-AngleFrontWheel, 0, 0, 1);
                                Gl.glTranslated(-objectsInfo[i].Objects[35].Position[0], -objectsInfo[i].Objects[35].Position[1], -objectsInfo[i].Objects[35].Position[2]);
                                break;
                            case 30:
                                Gl.glTranslated(objectsInfo[i].Objects[27].Position[0], objectsInfo[i].Objects[27].Position[1], objectsInfo[i].Objects[27].Position[2]);
                                Gl.glRotated(-AngleFrontWheel, 0, 0, 1);
                                Gl.glTranslated(-objectsInfo[i].Objects[27].Position[0], -objectsInfo[i].Objects[27].Position[1], -objectsInfo[i].Objects[27].Position[2]);
                                break;
                            default:
                                break;
                        }
                    }

                    if (i == 0 && j == 19) {
                        Gl.glTranslated(objectsInfo[i].Objects[j].Position[0], objectsInfo[i].Objects[j].Position[1] - 1.3, objectsInfo[i].Objects[j].Position[2]);
                        Gl.glRotated(-23, 1, 0, 0);
                        Gl.glRotated(angleHelm, 0, 1, 0);
                        Gl.glRotated(23, 1, 0, 0);
                        Gl.glTranslated(-objectsInfo[i].Objects[j].Position[0], -objectsInfo[i].Objects[j].Position[1] + 1.3, -objectsInfo[i].Objects[j].Position[2]);
                    }

                    Gl.glTranslated(objectsInfo[i].Objects[j].Translate[0], objectsInfo[i].Objects[j].Translate[1], objectsInfo[i].Objects[j].Translate[2]);

                    if (!(i == 0 && (j == 18 || j == 37 || j == 38) && Camera.cameraPosition == 1))
                    {
                        //отрисовка дисплейного списка
                        Gl.glCallList(obj);
                    }

                    Gl.glTranslated(-objectsInfo[i].Objects[j].Translate[0], -objectsInfo[i].Objects[j].Translate[1], -objectsInfo[i].Objects[j].Translate[2]);

                    //вращение колес
                    if (i == 0 && (j == 25 || j == 26 || j == 28 || j == 29 || j == 27 || j == 34 || j == 35 || j == 36))
                    {
                        Gl.glTranslated(objectsInfo[i].Objects[j].Position[0], objectsInfo[i].Objects[j].Position[1], objectsInfo[i].Objects[j].Position[2]);

                        Gl.glRotated(AngleWheel, 1, 0, 0);

                        //поворот колес
                        if (j == 25 || j == 26 || j == 35 || j == 27)
                        {
                            Gl.glRotated(AngleFrontWheel, 0, 0, 1);
                        }
                       
                        Gl.glTranslated(-objectsInfo[i].Objects[j].Position[0], -objectsInfo[i].Objects[j].Position[1], -objectsInfo[i].Objects[j].Position[2]);
                    }
                    //поворот тормозных колодок при повороте передних колес
                    if (i == 0)
                    {
                        switch (j) {
                            case 31:
                                Gl.glTranslated(objectsInfo[i].Objects[35].Position[0], objectsInfo[i].Objects[35].Position[1], objectsInfo[i].Objects[35].Position[2]);
                                Gl.glRotated(AngleFrontWheel, 0, 0, 1);
                                Gl.glTranslated(-objectsInfo[i].Objects[35].Position[0], -objectsInfo[i].Objects[35].Position[1], -objectsInfo[i].Objects[35].Position[2]);
                                break;
                            case 30:
                                Gl.glTranslated(objectsInfo[i].Objects[27].Position[0], objectsInfo[i].Objects[27].Position[1], objectsInfo[i].Objects[27].Position[2]);
                                Gl.glRotated(AngleFrontWheel, 0, 0, 1);
                                Gl.glTranslated(-objectsInfo[i].Objects[27].Position[0], -objectsInfo[i].Objects[27].Position[1], -objectsInfo[i].Objects[27].Position[2]);
                                break;
                            default:
                                break;
                        }
                    }

                    if (i == 0 && j == 19)
                    {
                        Gl.glTranslated(objectsInfo[i].Objects[j].Position[0], objectsInfo[i].Objects[j].Position[1] - 1.3, objectsInfo[i].Objects[j].Position[2]);
                        Gl.glRotated(-23, 1, 0, 0);
                        Gl.glRotated(-angleHelm, 0, 1, 0);
                        Gl.glRotated(23, 1, 0, 0);
                        Gl.glTranslated(-objectsInfo[i].Objects[j].Position[0], -objectsInfo[i].Objects[j].Position[1] + 1.3, -objectsInfo[i].Objects[j].Position[2]);
                    }

                    //вернуть перемещение
                    if (i > 0)
                    {
                        Gl.glTranslated(-translateVector.X, -translateVector.Y, 0);
                        Gl.glRotated(-angle, 0, 0, 1);
                    }

                    j++;
                }
            }

            Gl.glPopMatrix();

            Gl.glFlush();

            ////////////
            //добавить скорость, применить сопротивление
            TranslateVector = TranslateVector.Subtract(directionVector.Multiply(carSpeed));

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

        #region Воспомогательные ф-ции
        private static string GetFirstWord(string str) {
            return str.Split(new char[] { ' ', '\t'})[0];
        }
        private static int CountWords(string s, string s0)
        {
            int count = (s.Length - s.Replace(s0, "").Length) / s0.Length;
            return count;
        }
        private static float[] ArrayMultiply(float[] arr, float k) {

            float[] bufArr = new float[arr.Length];
            arr.CopyTo(bufArr, 0);

            for (int i = 0; i < bufArr.Length; i++)
                bufArr[i] *= k;

            return bufArr;
        }
        #endregion
    }
}
