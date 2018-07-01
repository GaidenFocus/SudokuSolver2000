using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver2000
{
    public partial class frmSudokuSolver : Form
    {
        TextBox[,] Boxes = new TextBox[9, 9];
        CheckBox chk = new CheckBox();

        string[,] Save = new string[9, 9];

        public frmSudokuSolver()
        {
            InitializeComponent();

            int r_margin = 0;
            int c_margin = 0;

            for (int r = 0; r < 9; r++)
            {
                if (r % 3 == 0)
                {
                    r_margin += 5;
                }

                for (int c = 0; c < 9; c++)
                {
                    if (c % 3 == 0)
                    {
                        c_margin += 5;
                    }

                    TextBox txt = new TextBox();
                    txt.Multiline = true;
                    txt.Width = 200;
                    txt.Height = 50;
                    txt.Font = new Font("Arial", 22, FontStyle.Regular);
                    txt.TextAlign = HorizontalAlignment.Center;
                    txt.Location = new Point((r * 202) + r_margin, (c * 52) + c_margin);
                    txt.MaxLength = 1;
                    txt.Name = "b" + r.ToString() + c.ToString();
                    this.Controls.Add(txt);
                    Boxes[r, c] = txt;

                    txt.PreviewKeyDown += Navigate;
                    txt.KeyDown += CheckNav;
                    txt.KeyDown += CheckChar;
                }

                c_margin = 0;
            }

            Button btn = new Button();
            btn.Text = "Clear";
            btn.Click += ClearPuzzle;
            btn.Location = new Point(5, 485);
            btn.BackColor = Color.LightGray;
            btn.Width = 100;
            btn.Height = 50;
            this.Controls.Add(btn);

            Button save = new Button();
            save.Text = "Save";
            save.Click += SavePuzzle;
            save.Location = new Point(5, 535);
            save.BackColor = Color.LightGray;
            save.Width = 100;
            save.Height = 50;
            this.Controls.Add(save);

            Button load = new Button();
            load.Text = "Load";
            load.Click += LoadPuzzle;
            load.Location = new Point(5, 585);
            load.BackColor = Color.LightGray;
            load.Width = 100;
            load.Height = 50;
            this.Controls.Add(load);

            chk.Text = "Use extra";
            chk.Location = new Point(120, 490);
            chk.BackColor = Color.White;
            chk.Checked = true;
            this.Controls.Add(chk);
        }

        private void ClearPuzzle(object sender, EventArgs e)
        {
            foreach (TextBox box in Boxes)
            {
                box.Text = "";
            }
        }

		string path = Directory.GetCurrentDirectory() + @"\save.txt";
		private void SavePuzzle(object sender, EventArgs e)
        {
			string save = "";
            for (int c = 0; c < 9; c++)
            {
                for (int r = 0; r < 9; r++)
                {
					save += Boxes[r, c].Text + ",";
                }
            }

			File.WriteAllText(path, save);
        }

        private void LoadPuzzle(object sender, EventArgs e)
        {
			if (!File.Exists(path))
			{
				return;
			}

			string[] save = File.ReadAllText(path).Split(',');
			int limit = 0;
			for (int c = 0; c < 9; c++)
			{
				for (int r = 0; r < 9; r++)
				{
					Boxes[r, c].Text = save[limit++];
				}
			}
        }

        private void Navigate(object sender, PreviewKeyDownEventArgs e)
        {
            string name = (sender as TextBox).Name;

            int x = int.Parse(name[1].ToString());
            int y = int.Parse(name[2].ToString());

            switch (e.KeyCode)
            {
                case Keys.Down:
                {
                    if (y == 8) y = 0; else y++;
                    break;
                }
                case Keys.Left:
                {
                    if (x == 0) x = 8; else x--;
                    break;
                }
                case Keys.Up:
                {
                    if (y == 0) y = 8; else y--;
                    break;
                }
                case Keys.Right:
                {
                    if (x == 8) x = 0; else x++;
                    break;
                }
            }

            Boxes[x, y].Select();
            handled = true;
        }

        bool handled = false;
        private void CheckNav(object sender, KeyEventArgs e)
        {
            if (handled)
            {
                e.Handled = true;
                handled = false;
            }
        }

        private void CheckChar(object sender, KeyEventArgs e)
        {
            TextBox send = (TextBox)sender;
            bool solve = false;

            if ((e.KeyValue >= 48 && e.KeyValue <= 57) 
             || (e.KeyValue >= 96 && e.KeyValue <= 105))
            {
                solve = true;
            }
            else if (e.KeyData == Keys.Left || e.KeyData == Keys.Down || e.KeyData == Keys.Right || e.KeyData == Keys.Up)
            {

            }
            else if (e.KeyData == Keys.Back || e.KeyData == Keys.Delete)
            {
                send.Text = "";
                solve = true;
            }
            else
            {
                e.SuppressKeyPress = true;
            }

            if (solve)
            {
                string num = new KeysConverter().ConvertToString(e.KeyCode).Replace("NumPad", "");

                send.Text = num;

                string[,] values = new string[9, 9];

                for (int r = 0; r < 9; r++)
                    for (int c = 0; c < 9; c++)
                        values[r, c] = Boxes[r, c].Text;

                TheSolver.usedashit = chk.Checked;
                TheSolver.JustDoIt(values);

                values = SudokuGroupFactory.Solve(values);


                for (int r = 0; r < 9; r++)
                {
                    for (int c = 0; c < 9; c++)
                    {
                        Boxes[r, c].KeyDown -= CheckChar;
                        Boxes[r, c].Text = values[r, c];
                        Boxes[r, c].KeyDown += CheckChar;
                    }
                }
            }
        }
    }
}