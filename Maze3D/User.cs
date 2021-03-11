using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Maze3D
{
    public class User
    {
        [XmlElement("Username")]
        public string Username;

        [XmlElement("CoinCounter")]
        public int CoinCounter { get; set; }

        [XmlElement("MaxHearts")]
        public int MaxHearts { get; set; }

        [XmlElement("MaxOxygen")]
        public int MaxOxygen { get; set; }

        [XmlElement("BonusOxygen")]
        public int BonusOxygen { get; set; }

        [XmlElement("GamesCounter")]
        public int GamesCounter { get; set; }

        [XmlIgnore]
        public int CurrentOxygen;

        [XmlIgnore]
        public int CurrentHearts;

        [XmlIgnore]
        public bool HasKey;

        public User()
        {
            Username = "Guest";
            CoinCounter = 0;
            MaxHearts = 1;
            MaxOxygen = 70;
            CurrentHearts = MaxHearts;
            CurrentOxygen = MaxOxygen;
            BonusOxygen = 10;
            HasKey = false;
            GamesCounter = 0;
        }

        public User(string username)
        {
            Username = username;
            CoinCounter = 0;
            MaxHearts = 1;
            MaxOxygen = 70;
            CurrentHearts = MaxHearts;
            CurrentOxygen = MaxOxygen;
            BonusOxygen = 10;
            HasKey = false;
            GamesCounter = 0;
        }
    }
}
