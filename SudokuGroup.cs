using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver2000
{
    public static class SudokuGroupFactory
    {
        static private SudokuPuzzle Puzzle;

        static SudokuGroupFactory()
        {
            Puzzle = new SudokuPuzzle();
        }

        public static string[,] Solve(string[,] values)
        {
            string[,] poo = Puzzle.SetValues(values);
            return poo;
        }

        public static string[,] SetValue(int x, int y, string value)
        {
            return Puzzle.SetValue(x, y, value);
        }
    }

    public class SudokuPuzzle
    {
        private SudokuBox[,] Puzzle = new SudokuBox[9, 9];
        private List<SudokuGroup> Groups = new List<SudokuGroup>();

        public SudokuPuzzle()
        {
            for (int c = 0; c < 9; c++)
            {
                SudokuGroup group = new SudokuGroup();

                for (int r = 0; r < 9; r++)
                {
                    Puzzle[c, r] = new SudokuBox(c, r);
                    group.Boxes.Add(Puzzle[c, r]);
                }

                Groups.Add(group);
            }

            for (int r = 0; r < 9; r++)
            {
                SudokuGroup group = new SudokuGroup();

                for (int c = 0; c < 9; c++)
                {
                    group.Boxes.Add(Puzzle[c, r]);
                }

                Groups.Add(group);
            }

            for (int c_b = 0; c_b < 9; c_b += 3)
            {
                for (int r_b = 0; r_b < 9; r_b += 3)
                {
                    SudokuGroup group = new SudokuGroup();

                    for (int c = c_b; c < c_b + 3; c++)
                    {
                        for (int r = r_b; r < r_b + 3; r++)
                        {
                            group.Boxes.Add(Puzzle[c, r]);
                        }
                    }

                    Groups.Add(group);
                }
            }
        }

        public string[,] SetValues(string[,] values)
        {
            for (int c = 0; c < 9; c++)
            {
                for (int r = 0; r < 9; r++)
                {
                    Puzzle[c, r].SetValue(values[c, r]);
                }
            }

            return GetValues();
        }

        public string[,] SetValue(int x, int y, string value)
        {
            Puzzle[x, y].Value = value;

            return GetValues();
        }

        private string[,] GetValues()
        {
            string[,] values = new string[9, 9];

            for (int c = 0; c < 9; c++)
            {
                for (int r = 0; r < 9; r++)
                {
                    values[c, r] = Puzzle[c, r].Value;
                }
            }

            return values;
        }
    }

    public enum SudokuGroupType
    {
        Col
       ,Row
       ,Box
    };

    public class SudokuGroup
    {
        public SudokuGroupType Type;


        public List<SudokuBox> Boxes = new List<SudokuBox>();

        public SudokuGroup()
        {

        }

        public void AddBox()
        {

        }

        public void NewValue(object sender, EventArgs e)
        {
            SudokuBox box = (SudokuBox)sender;

            for (int i = 0; i < 9; i++)
            {
                if (Boxes[i] != box)
                {
                    Boxes[i].Remove(box.Value);
                }
            }
        }
    }

    public class SudokuBox
    {
        public int X;
        public int Y;
        public string Value;

        public List<string> Possible = new List<string>();

        public SudokuBox(int _x, int _y)
        {
            for (int i = 1; i < 10; i++)
            {
                Possible.Add(i.ToString());
            }

            X = _x;
            Y = _y;
        }

        public void Remove(string value)
        {
            Possible.Remove(value);
            
            if (Possible.Count == 1)
            {
                SetValue(Possible[0]);
            }
        }

        public void SetValue(string value)
        {
            Value = value;

            SudokuValueSetArgs e = new SudokuValueSetArgs() {
                _X = X,
                _Y = Y,
                _Value = Value
            };

            OnValueSet(e);
        }

        public event EventHandler ValueSet;
        protected virtual void OnValueSet(SudokuValueSetArgs e)
        {
            ValueSet?.Invoke(this, e);
        }
    }

    public class SudokuValueSetArgs : EventArgs
    {
        public int _X;
        public int _Y;
        public string _Value;
    }
}