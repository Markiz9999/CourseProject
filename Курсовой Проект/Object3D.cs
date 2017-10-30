using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовой_Проект
{
    public class Object3D
    {
        public string ObjName;                          //имя объекта
        public List<double[]> Vertex;                   //координаты точек
        public List<int[]> Poly;                        //полигоны
        public List<double[]> TVertex;                  //координаты точек для текстур
        public List<int[]> TPoly;                       //полигоны для текстур
        public string ObjFileType;                      //тип загружаемого файла (obj | ase - ase в приоритете (obj отключен))
        public int Material;                            //номер материала для объекта
        public double[] Position;                       //позиция объекта
        public List<int> SubMaterial = new List<int>(); //субматериалы для каждого полигона

        public double[] Translate = new double[] { 0, 0, 0}; //смещение объекта

        public Object3D(string objName, List<double[]> vertex, List<int[]> poly, List<int> submaterial, List<double[]> tVertex, List<int[]> tPoly, String objFileType, int material) {
            ObjName = objName;
            Vertex = vertex;
            Poly = poly;
            TVertex = tVertex;
            TPoly = tPoly;
            ObjFileType = objFileType;
            Material = material;
            SubMaterial = submaterial;

            Position = new double[] { 0, 0, 0};
        }

        public Object3D(string objName, List<double[]> vertex, List<int[]> poly, List<int> submaterial, List<double[]> tVertex, List<int[]> tPoly, String objFileType, int material, double[] position)
        {
            ObjName = objName;
            Vertex = vertex;
            Poly = poly;
            TVertex = tVertex;
            TPoly = tPoly;
            ObjFileType = objFileType;
            Material = material;
            SubMaterial = submaterial;

            Position = position;
        }
    }
}
