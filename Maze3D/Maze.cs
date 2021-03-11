using System;
using System.Linq;
using System.Xml.Serialization;
using static Maze3D.Values;

namespace Maze3D
{
    class Maze
    {
        public MyVector Size { get; private set; }
        public MyVector FinishVector;
        public MyVector PlayerVector;
        private FieldCreationParams creationParams;
        private char[,,] Field;

        public Maze(MyVector size)
        {
            Size = size;
            Field = new char[size.X, size.Y, size.Z];
        }

        public char this[MyVector indexVector]
        {
            get
            {
                return Field[indexVector.X, indexVector.Y, indexVector.Z];
            }
            set
            {
                Field[indexVector.X, indexVector.Y, indexVector.Z] = value;
            }
        }

        public char this[int x, int y, int z]
        {
            get
            {
                return Field[x, y, z];
            }
            set
            {
                Field[x, y, z] = value;
            }
        }

        #region Init

        public static Maze MakeMaze(FieldCreationParams creationParams)
        {
            Maze maze = new Maze(creationParams.Size);
            maze.creationParams = creationParams;
            maze.MazeGeneration();
            maze.VisualMap();
            return maze;
        }


        private void MazeGeneration()
        {
            GenerateAnEmptyBorderedMap();

            FinishVector.X = random.Next(1, Size.X);
            FinishVector.Y = random.Next(1, Size.Y);
            FinishVector.Z = random.Next(1, Size.Z);
            PlayerVector.X = random.Next(1, Size.X);
            PlayerVector.Y = random.Next(1, Size.Y);
            PlayerVector.Z = random.Next(1, Size.Z);

            for (int k = 1; k < Size.Z; k++)
            {
                for (int j = 1; j < Size.Y; j++)
                {
                    for (int i = 1; i < Size.X; i++)
                    {
                        this[i, j, k] = GenerateAnEntity();
                    }
                }
            }

            this[FinishVector] = finish;
            this[PlayerVector] = player;

            int keyNumber = Size.X * Size.Y * Size.Z / 2000;
            if (keyNumber == 0) keyNumber = 1;
            for (int a = 1; a <= keyNumber; a++)
            {
                MyVector keyVector;
                keyVector.X = random.Next(1, Size.X);
                keyVector.Y = random.Next(1, Size.Y);
                keyVector.Z = random.Next(1, Size.Z);
                this[keyVector] = key;
            }
        }

        private void GenerateAnEmptyBorderedMap()
        {
            Field = new char[Size.X + 1, Size.Y + 1, Size.Z];

            for (int z = 1; z < Size.Z; z++)
            {
                for (int y = 1; y < Size.Y; y++)
                {
                    this[0, y, z] = verticalWall;
                    this[Size.X, y, z] = verticalWall;
                }

                for (int x = 1; x < Size.X; x++)
                {
                    this[x, 0, z] = horizontalWall;
                    this[x, Size.Y, z] = horizontalWall;
                }

                this[0, 0, z] = corner1;
                this[Size.X, Size.Y, z] = corner1;
                this[0, Size.Y, z] = corner2;
                this[Size.X, 0, z] = corner2;
            }
        }

        private char GenerateAnEntity()
        {
            int spawnRate = random.Next(0, 100);
            int extraSpawnRate = random.Next(creationParams.Frequency, 1000);
            char entity;

            if (spawnRate < creationParams.Frequency) entity = spike;
            else if (spawnRate < creationParams.Frequency * 5) entity = wall;
            else if (extraSpawnRate < 2) entity = oxygen;
            else entity = space;

            if (extraSpawnRate == 500) entity = coin;
            
            return entity;
        }

        private void VisualMap()
        {
            for (int k = 1; k < Size.Z; k++)
            {
                for (int j = 1; j < Size.Y; j++)
                {
                    for (int i = 1; i < Size.X; i++)
                    {
                        if (this[i, j, k] == space)
                            this[i, j, k] = GetMapPointer(new MyVector(i, j, k));
                    }
                }
            }
        }

        public char GetMapPointer(MyVector cell)
        {
            char willBeInited = this[cell];
            bool wallDownLayer = false;
            bool wallUpLayer = false;

            if (this[cell] == space)
            {
                if (cell.Z - 1 > 0 && this[cell.X, cell.Y, cell.Z - 1] == wall)
                {
                    willBeInited = wallBottom;
                    wallDownLayer = true;
                }
                else if(cell.Z - 1 > 0 && this[cell.X, cell.Y, cell.Z - 1] == spike)
                {
                    willBeInited = hazard;
                }

                if (cell.Z + 1 < Size.Z && this[cell.X, cell.Y, cell.Z + 1] == wall)
                {
                    willBeInited = wallTop;
                    wallUpLayer = true;
                }
                else if (!wallDownLayer && cell.Z + 1 < Size.Z && this[cell.X, cell.Y, cell.Z + 1] == spike)
                {
                    willBeInited = hazard;
                }

                if (wallDownLayer && wallUpLayer) willBeInited = wallBoth;
            }

            return willBeInited;
        }
        #endregion
    }
}
