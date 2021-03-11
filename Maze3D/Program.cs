using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Maze3D
{
    class Program
    {
        public static void Main()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<User>));
            List<User> users = Load(formatter);

            Game game = Game.GetGame(users);
            game.Run();

            Save(formatter, users);
            Console.WriteLine("Press ESC to quit");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
            Console.CursorVisible = true;
        }

        public static List<User> Load(XmlSerializer formatter)
        {
            using (FileStream fs = new FileStream("usersData.xml", FileMode.OpenOrCreate))
            {
                List<User> users = new List<User>();
                try
                {
                    users = (List<User>)formatter.Deserialize(fs);
                }
                catch (InvalidOperationException ex)
                {
                    users = new List<User>();
                    formatter.Serialize(fs, users);
                }
                return users;
            }
        }

        public static void Save(XmlSerializer formatter, List<User> users)
        {
            using (FileStream fs = new FileStream("usersData.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, users);
                string message = "";
                message = GetSaveStatus(users);
                Console.WriteLine(message);
            }
        }

        private static string GetSaveStatus(List<User> users)
        {
            if (users.Count > 0)
            {
                return "\nSave completed";
            }
            return "\nNo user to save";
        }
    }
}
