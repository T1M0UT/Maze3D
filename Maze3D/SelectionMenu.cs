using System;
using System.Collections.Generic;

namespace Maze3D
{
    class SelectionMenu
    {
        public int CurrentSelectedIndex;
        public int MaxIndex { get => items.Count - 1; }
        public string Name;
        public SelectionState Selection;
        public List<(string item, int value)> items { get; private set; }

        public (string item, int value) this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        public SelectionMenu(string name, List<(string item, int value)> items)
        {
            Name = name;
            this.items = items;
            CurrentSelectedIndex = 0;
            Selection = SelectionState.Select;
        }

        public override string ToString()
        {
            return $"\t-- {Name} --";
        }
    }
}
