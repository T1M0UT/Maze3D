using System;
namespace Maze3D
{
    class FieldCreationParams
    {
        public MyVector Size;
        public int Frequency;

        public FieldCreationParams(MyVector size, int frequency)
        {
            Size = size;
            Frequency = frequency;
        }
    }   
}
