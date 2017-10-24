using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовой_Проект
{
    class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector() {
            X = 0;
            Y = 0;
        }
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        //квадрат длины вектора
	    public double LengthSquared()
        {
            return X * X + Y * Y;
        }
        //длина вектора
        public double Length()
        {
            return Math.Sqrt(LengthSquared());
        }
        //прибавить вектора
        public Vector Add(Vector v) {
            return new Vector(X + v.X, Y + v.Y);
        }
        //отнять вектора
        public Vector Subtract(Vector v) {
            return new Vector(X - v.X, Y - v.Y);
        }
        //нормирование вектора
        public Vector Unit()
        {
            var length = Length();

            if (length > 0)
            {
                return new Vector(X / length, Y / length);
            }
            else
            {
                return new Vector(0, 0);
            }
        }
        //поворот вектора на угол
        public Vector Rotate(double angle) {
            return new Vector(X * Math.Cos(angle) - Y * Math.Sin(angle), Y * Math.Cos(angle) + X * Math.Sin(angle));
        }
        //умножить вектор на число
        public Vector Multiply(double a) {
            return new Vector(X * a, Y * a);
        }

        //скалярное произведение
        public double DotProduct(Vector v){
            return X * Y + Y * v.Y;
        }

        static public double AngleBetween(Vector v1, Vector v2) {
            return Math.Acos(v1.DotProduct(v2) / (v1.Length() * v2.Length()));
        }
    }
}
