using System;
namespace Maze3D
{
    static class Values
    {
        public static Random random = new Random();
        public static char player = '@';
        public static char wall = '#';
        public static char finish = 'F';
        public static char space = ' ';
        public static char wallTop = '^';
        public static char wallBottom = 'v';
        public static char wallBoth = 'I';
        public static char key = 'k';
        public static char oxygen = 'o';
        public static char spike = '*';
        public static char fullHeart = 'H';
        public static char emptyHeart = '_';
        public static char coin = 'c';//©
        public static char hazard = '!';
        public static char verticalWall = '|';
        public static char horizontalWall = '-';
        public static char corner1 = '/';
        public static char corner2 = '\\';
        public static ConsoleColor BackgroundColor = ConsoleColor.Black;
    }
}
