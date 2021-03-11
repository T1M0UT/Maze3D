using System;
namespace Maze3D
{
    struct MyVector
    {
        public int X;
        public int Y;
        public int Z;

        public MyVector(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public static MyVector operator +(MyVector vector1, MyVector vector2)
        {
            MyVector vector3;
            vector3.X = vector1.X + vector2.X;
            vector3.Y = vector1.Y + vector2.Y;
            vector3.Z = vector1.Z + vector2.Z;
            return vector3;
        }

        public static MyVector operator -(MyVector vector1, MyVector vector2)
        {
            MyVector vector3;
            vector3.X = vector1.X - vector2.X;
            vector3.Y = vector1.Y - vector2.Y;
            vector3.Z = vector1.Z - vector2.Z;
            return vector3;
        }

        public static bool operator ==(MyVector vector1, MyVector vector2)
        {
            if(vector1.X == vector2.X && vector1.Y == vector2.Y && vector1.Z == vector2.Z)
                return true;
            return false;
        }

        
        public static bool operator !=(MyVector vector1, MyVector vector2)
        {
            if (vector1.X != vector2.X && vector1.Y != vector2.Y && vector1.Z != vector2.Z)
                return true;
            return false;
        }

        public static bool operator >(MyVector vector1, MyVector vector2)
        {
            if (vector1.X > vector2.X && vector1.Y > vector2.Y && vector1.Z > vector2.Z)
                return true;
            return false;
        }

        public static bool operator <(MyVector vector1, MyVector vector2)
        {
            if (vector1.X < vector2.X && vector1.Y < vector2.Y && vector1.Z < vector2.Z)
                return true;
            return false;
        }

        public static bool operator >=(MyVector vector1, MyVector vector2)
        {
            if (vector1.X >= vector2.X && vector1.Y >= vector2.Y && vector1.Z >= vector2.Z)
                return true;
            return false;
        }

        public static bool operator <=(MyVector vector1, MyVector vector2)
        {
            if (vector1.X <= vector2.X && vector1.Y <= vector2.Y && vector1.Z <= vector2.Z)
                return true;
            return false;
        }

        public static implicit operator MyVector(int number)
        {
            return new MyVector(number, number, number);
        }
    }
}
