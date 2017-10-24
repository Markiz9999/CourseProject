using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовой_Проект
{
    public class Object3D
    {
        private string objName;
        private List<double[]> vertex;
        private List<int[]> poly;
        private List<double[]> tVertex;
        private List<int[]> tPoly;
        private string objFileType;
        private int textureId;
        private double[] position;

        public string ObjName {
            get { return objName; }
            set {
                if (objName != String.Empty) {
                    objName = value;
                }
            }
        }
        public List<double[]> Vertex {
            get { return vertex; }
            set {
                vertex = value;
            }
        }
        public List<int[]> Poly
        {
            get { return poly; }
            set
            {
                poly = value;
            }
        }
        public List<double[]> TVertex
        {
            get { return tVertex; }
            set
            {
                tVertex = value;
            }
        }
        public List<int[]> TPoly
        {
            get { return tPoly; }
            set
            {
                tPoly = value;
            }
        }
        public string ObjFileType
        {
            get { return objFileType; }
            set
            {
                if (objFileType != String.Empty)
                {
                    objFileType = value;
                }
            }
        }
        public double[] Position {
            get { return position; }
            set { position = value; }
        }

        public int TextureId { get { return textureId; } }

        public Object3D(string objName, List<double[]> vertex, List<int[]> poly, List<double[]> tVertex, List<int[]> tPoly, String objFileType, int TextureId) {
            ObjName = objName;
            Vertex = vertex;
            Poly = poly;
            TVertex = tVertex;
            TPoly = tPoly;
            ObjFileType = objFileType;
            textureId = TextureId;

            Position = new double[] { 0, 0, 0};
        }

        public Object3D(string objName, List<double[]> vertex, List<int[]> poly, List<double[]> tVertex, List<int[]> tPoly, String objFileType, int TextureId, double[] position)
        {
            ObjName = objName;
            Vertex = vertex;
            Poly = poly;
            TVertex = tVertex;
            TPoly = tPoly;
            ObjFileType = objFileType;
            textureId = TextureId;

            Position = position;
        }
    }
}
