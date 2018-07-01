using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSolver2000
{
	static class TheSolver
	{
		static public bool usedashit = true;

		static public string[,] JustDoIt(string[,] values)
		{
			Prep(values);

			int solved = 0;

			for (;;)
			{
				Clean(values);
				if (usedashit)
				{
					FindSingles(values);
					FindPairs(values);
					FindLockedCandidates(values);
					FindLockedCandidatesRows(values);
					FindLockedCandidatesCols(values);
					FindDemTrips(values);
					XWing(values);
				}

				int check = CountSolved(values);
				if (solved != check) solved = check;
				else break;
			}

			return values;
		}

		static private void Prep(string[,] values)
		{
			for (int c = 0; c < 9; c++)
			{
				for (int r = 0; r < 9; r++)
				{
					if (values[c, r].Equals("") || values[c, r].Length > 1)
					{
						values[c, r] = "123456789";
					}
				}
			}
		}

		static private int CountSolved(string[,] values)
		{
			int solved = 0;

			for (int c = 0; c < 9; c++)
			{
				for (int r = 0; r < 9; r++)
				{
					solved += values[c, r].Length;
				}
			}

			return solved;
		}

		static private void Clean(string[,] values)
		{
			for (int c = 0; c < 9; c++)
			{
				for (int r = 0; r < 9; r++)
				{
					if (values[c, r].Length == 1)
					{
						for (int s = 0; s < 9; s++)
						{
							if (s != r)
							{
								values[c, s] = values[c, s].Replace(values[c, r], "");
							}

							if (s != c)
							{
								values[s, r] = values[s, r].Replace(values[c, r], "");
							}
						}
					}
				}
			}

			for (int b_c = 0; b_c < 9; b_c += 3)
			{
				for (int b_r = 0; b_r < 9; b_r += 3)
				{
					for (int c = b_c; c < b_c + 3; c++)
					{
						for (int r = b_r; r < b_r + 3; r++)
						{
							if (values[c, r].Length == 1)
							{
								for (int c_s = b_c; c_s < b_c + 3; c_s++)
								{
									for (int r_s = b_r; r_s < b_r + 3; r_s++)
									{
										if (c_s != c || r_s != r)
										{
											values[c_s, r_s] = values[c_s, r_s].Replace(values[c, r], "");
										}
									}
								}
							}
						}
					}
				}
			}
		}

		static private void FindSingles(string[,] values)
		{
			int count = 0;
			int x = -1;
			int y = -1;

			for (int n = 1; n < 10; n++)
			{
				//COLUMNS
				for (int c = 0; c < 9; c++)
				{
					count = 0;
					x = -1;
					y = -1;

					for (int r = 0; r < 9; r++)
					{
						if (values[c, r].Contains(n.ToString()))
						{
							if (values[c, r].Length == 1)
							{
								count = -1;
								break;
							}
							else if (values[c, r].Length > 1)
							{
								count++;
								x = c;
								y = r;
							}
						}

						if (count > 1)
						{
							break;
						}
					}

					if (count == 1)
					{
						values[x, y] = n.ToString();
					}

					//NOW FOR ROWS YUH
					count = 0;
					x = -1;
					y = -1;

					for (int r = 0; r < 9; r++)
					{
						if (values[r, c].Contains(n.ToString()))
						{
							if (values[r, c].Length == 1)
							{
								count = -1;
								break;
							}
							else if (values[r, c].Length > 1)
							{
								count++;
								x = r;
								y = c;
							}
						}

						if (count > 1)
						{
							break;
						}
					}

					if (count == 1)
					{
						values[x, y] = n.ToString();
					}
				}

				//Now for boxames
				for (int b_c = 0; b_c < 9; b_c += 3)
				{
					for (int b_r = 0; b_r < 9; b_r += 3)
					{
						count = 0;
						x = -1;
						y = -1;

						for (int c = b_c; c < b_c + 3; c++)
						{
							for (int r = b_r; r < b_r + 3; r++)
							{
								if (values[c, r].Contains(n.ToString()))
								{
									if (values[c, r].Length == 1)
									{
										count = -2;
										break;
									}
									else if (values[c, r].Length > 1)
									{
										count++;
										x = c;
										y = r;
									}
								}

								if (count > 1)
								{
									break;
								}
							}

							if (count > 1 || count == -2)
							{
								break;
							}
						}

						if (count == 1)
						{
							values[x, y] = n.ToString();
						}
					}
				}
			}
		}

		static private void FindPairs(string[,] values)
		{
			for (int n = 1; n < 9; n++)
			{
				for (int n2 = n + 1; n2 < 10; n2++)
				{
					int count = 0;
					int x1 = -1;
					int x2 = -1;

					//cols
					for (int c = 0; c < 9; c++)
					{
						count = 0;
						x1 = -1;

						for (int r = 0; r < 9; r++)
						{
							if (values[c, r].Length == 2 && values[c, r].Contains(n.ToString()) && values[c, r].Contains(n2.ToString()))
							{
								count++;

								if (x1 == -1)
								{
									x1 = r;
								}
								else
								{
									x2 = r;
								}
							}

							if (count > 2)
							{
								count = 0;
								x1 = -1;
								x2 = -1;
								break;
							}
						}

						if (count == 2)
						{
							for (int r = 0; r < 9; r++)
							{
								if (r != x1 && r != x2)
								{
									values[c, r] = values[c, r].Replace(n.ToString(), "");
									values[c, r] = values[c, r].Replace(n2.ToString(), "");
								}
							}
						}
					}

					//rows
					count = 0;
					x1 = -1;
					x2 = -1;

					for (int r = 0; r < 9; r++)
					{
						count = 0;
						x1 = -1;

						for (int c = 0; c < 9; c++)
						{
							if (values[c, r].Length == 2 && values[c, r].Contains(n.ToString()) && values[c, r].Contains(n2.ToString()))
							{
								count++;

								if (x1 == -1)
								{
									x1 = c;
								}
								else
								{
									x2 = c;
								}
							}

							if (count > 2)
							{
								count = 0;
								x1 = -1;
								x2 = -1;
								break;
							}
						}

						if (count == 2)
						{
							for (int c = 0; c < 9; c++)
							{
								if (c != x1 && c != x2)
								{
									values[c, r] = values[c, r].Replace(n.ToString(), "");
									values[c, r] = values[c, r].Replace(n2.ToString(), "");
								}
							}
						}
					}

					//boxes
					for (int c_b = 0; c_b < 9; c_b += 3)
					{
						for (int r_b = 0; r_b < 9; r_b += 3)
						{
							count = 0;
							x1 = -1;
							x2 = -1;

							int y1 = -1;
							int y2 = -1;

							for (int c = c_b; c < c_b + 3; c++)
							{
								for (int r = r_b; r < r_b + 3; r++)
								{
									if (values[c, r].Length == 2 && values[c, r].Contains(n.ToString()) && values[c, r].Contains(n2.ToString()))
									{
										count++;

										if (x1 == -1)
										{
											x1 = c;
											y1 = r;
										}
										else
										{
											x2 = c;
											y2 = r;
										}
									}
								}
							}

							if (count == 2)
							{
								for (int c = c_b; c < c_b + 3; c++)
								{
									for (int r = r_b; r < r_b + 3; r++)
									{
										if ((c != x1 || r != y1) && (c != x2 || r != y2))
										{
											values[c, r] = values[c, r].Replace(n.ToString(), "");
											values[c, r] = values[c, r].Replace(n2.ToString(), "");
										}
									}
								}
							}

							count = 0;
							x1 = -1;
							x2 = -1;
							y1 = -1;
							y2 = -1;
						}
					}
					
				}
			}
		}

		static private void FindLockedCandidates(string[,] values)
		{
			for (int n = 1; n < 10; n++)
			{
				for (int b_r = 0; b_r < 9; b_r += 3)
				{
					for (int b_c = 0; b_c < 9; b_c += 3)
					{
						List<BoxSpot> spots = new List<BoxSpot>();

						for (int r = b_r; r < b_r + 3; r++)
						{
							for (int c = b_c; c < b_c + 3; c++)
							{
								if (values[r, c].Contains(n.ToString()))
								{
									BoxSpot spot = new BoxSpot();
									spot.r = r;
									spot.c = c;
									spot.n = n;
									spots.Add(spot);
								}
							}
						}

						if (spots.Count < 4)
						{
							int r_count = 0;
							int c_count = 0;

							for (int i = 1; i < spots.Count; i++)
							{
								if (spots[i].r == spots[i - 1].r)
								{
									r_count++;
								}

								if (spots[i].c == spots[i - 1].c)
								{
									c_count++;
								}
							}

							if (r_count == spots.Count - 1)
							{
								for (int c = 0; c < 9; c++)
								{
									if (spots.Where(s => s.c == c).ToList().Count == 0)
									{
										values[spots[0].r, c] = values[spots[0].r, c].Replace(spots[0].n.ToString(), "");
									}
								}
							}

							if (c_count == spots.Count - 1)
							{
								for (int r = 0; r < 9; r++)
								{
									if (spots.Where(s => s.r == r).ToList().Count == 0)
									{
										values[r, spots[0].c] = values[r, spots[0].c].Replace(spots[0].n.ToString(), "");
									}
								}
							}
						}
					}
				}
			}
		}

		static private void FindLockedCandidatesRows(string[,] values)
		{
			for (int n = 1; n < 10; n++)
			{
				for (int r = 0; r < 9; r++)
				{
					List<BoxSpot> spots = new List<BoxSpot>();
					for (int c = 0; c < 9; c++)
					{
						if (values[r, c].Contains(n.ToString()))
						{
							if (spots.Count == 0)
							{
								spots.Add(new BoxSpot(r, c));
							}
							else
							{
								bool same_box = true;
								foreach (BoxSpot spot in spots)
								{
									if (!spot.CompareBox(r, c))
									{
										same_box = false;
										break;
									}
								}

								if (same_box)
								{
									spots.Add(new BoxSpot(r, c));
								}
								else
								{
									spots = null;
									break;
								}
							}
						}
					}

					if (spots != null && spots.Count > 0)
					{
						BoxSpot box = GetCurrentBox(spots[0].r, spots[0].c);

						for (int rr = box.r; rr < box.r + 3; rr++)
						{
							for (int cc = box.c; cc < box.c + 3; cc++)
							{
								if (spots.Where(x => x.r == rr && x.c == cc).ToList().Count == 0)
								{
									values[rr, cc] = values[rr, cc].Replace(n.ToString(), "");
								}
							}
						}
					}
				}
			}
		}

		static private BoxSpot GetCurrentBox(int r, int c)
		{
			BoxSpot start = new BoxSpot();

			if (r < 3)
			{
				start.r = 0;
			}
			else if (r < 6)
			{
				start.r = 3;
			}
			else if (r < 9)
			{
				start.r = 6;
			}

			if (c < 3)
			{
				start.c = 0;
			}
			else if (c < 6)
			{
				start.c = 3;
			}
			else if (c < 9)
			{
				start.c = 6;
			}

			return start;
		}

		static private void FindLockedCandidatesCols(string[,] values)
		{
			for (int n = 1; n < 10; n++)
			{
				for (int c = 0; c < 9; c++)
				{
					List<BoxSpot> spots = new List<BoxSpot>();
					for (int r = 0; r < 9; r++)
					{
						if (values[r, c].Contains(n.ToString()))
						{
							if (spots.Count == 0)
							{
								spots.Add(new BoxSpot(r, c));
							}
							else
							{
								bool same_box = true;
								foreach (BoxSpot spot in spots)
								{
									if (!spot.CompareBox(r, c))
									{
										same_box = false;
										break;
									}
								}

								if (same_box)
								{
									spots.Add(new BoxSpot(r, c));
								}
								else
								{
									spots = null;
									break;
								}
							}
						}
					}

					if (spots != null && spots.Count > 0)
					{
						BoxSpot box = GetCurrentBox(spots[0].r, spots[0].c);

						for (int cc = box.c; cc < box.c + 3; cc++)
						{
							for (int rr = box.r; rr < box.r + 3; rr++)
							{
								if (spots.Where(x => x.r == rr && x.c == cc).ToList().Count == 0)
								{
									values[rr, cc] = values[rr, cc].Replace(n.ToString(), "");
								}
							}
						}
					}
				}
			}
		}

		static private void FindDemTrips(string[,] values)
		{
			for (int r = 0; r < 9; r++)
			{
				for (int c = 0; c < 9; c++)
				{
					if (values[r, c].Length > 3)
					{
						continue;
					}

					for (int i = 0; i < values[r, c].Length - 2; i++)
					{
						for (int i2 = i + 1; i2 < values[r, c].Length - 1; i2++)
						{
							for (int i3 = i2 + 1; i3 < values[r, c].Length; i3++)
							{
								char one = values[r, c][i];
								char two = values[r, c][i2];
								char three = values[r, c][i3];
								List<int> boxes = new List<int>();

								for (int c2 = 0; c2 < 9; c2++)
								{
									if (c2 == c || values[r, c2].Length != 2)
									{
										continue;
									}

									int num_count = 0;

									if (values[r, c2].Contains(one))
									{
										num_count++;
									}
									if (values[r, c2].Contains(two))
									{
										num_count++;
									}
									if (values[r, c2].Contains(three))
									{
										num_count++;
									}

									if (num_count > 1)
									{
										boxes.Add(c2);
									}
								}

								if (boxes.Count != 2)
								{
									continue;
								}
								else
								{
									for (int c2 = 0; c2 < 9; c2++)
									{
										if (boxes.Contains(c2) || c2 == c || values[r, c2].Length == 1)
										{
											continue;
										}

										values[r, c2] = values[r, c2].Replace(one.ToString(), "");
										values[r, c2] = values[r, c2].Replace(two.ToString(), "");
										values[r, c2] = values[r, c2].Replace(three.ToString(), "");
									}
								}
							}
						}
					}
				}
			}
			/**/
			for (int c = 0; c < 9; c++)
			{
				for (int r = 0; r < 9; r++)
				{
					if (values[r, c].Length > 3)
					{
						continue;
					}

					for (int i = 0; i < values[r, c].Length - 2; i++)
					{
						for (int i2 = i + 1; i2 < values[r, c].Length - 1; i2++)
						{
							for (int i3 = i2 + 1; i3 < values[r, c].Length; i3++)
							{
								char one = values[r, c][i];
								char two = values[r, c][i2];
								char three = values[r, c][i3];
								List<int> boxes = new List<int>();

								for (int r2 = 0; r2 < 9; r2++)
								{
									if (r2 == r || values[r2, c].Length != 2)
									{
										continue;
									}

									int num_count = 0;

									if (values[r2, c].Contains(one))
									{
										num_count++;
									}
									if (values[r2, c].Contains(two))
									{
										num_count++;
									}
									if (values[r2, c].Contains(three))
									{
										num_count++;
									}

									if (num_count > 1)
									{
										boxes.Add(r2);
									}
								}

								if (boxes.Count != 2)
								{
									continue;
								}
								else
								{
									for (int r2 = 0; r2 < 9; r2++)
									{
										if (boxes.Contains(r2) || r2 == r || values[r2, c].Length == 1)
										{
											continue;
										}

										values[r2, c] = values[r2, c].Replace(one.ToString(), "");
										values[r2, c] = values[r2, c].Replace(two.ToString(), "");
										values[r2, c] = values[r2, c].Replace(three.ToString(), "");
									}
								}
							}
						}
					}
				}
			}
			
			for (int b_r = 0; b_r < 9; b_r += 3)
			{
				for (int b_c = 0; b_c < 9; b_c += 3)
				{
					for (int r = 0; r < b_r + 3; r++)
					{
						for (int c = 0; c < b_c + 3; c++)
						{

						}
					}
				}
			}
		}

		static private void XWing(string[,] values)
		{
			for (int n = 1; n < 10; n++)
			{
				for (int r = 0; r < 9; r++)
				{
					List<BoxSpot> cs = new List<BoxSpot>();

					for (int c = 0; c < 9; c++)
					{
						if (values[r, c].Contains(n.ToString()))
						{
							cs.Add(new BoxSpot(r, c));
						}
					}

					if (cs.Count == 2)
					{
						for (int r2 = r + 1; r2 < 9; r2++)
						{
							List<BoxSpot> cs2 = new List<BoxSpot>();

							for (int c2 = 0; c2 < 9; c2++)
							{
								if (values[r2, c2].Contains(n.ToString()))
								{
									cs2.Add(new BoxSpot(r2, c2));
								}
							}

							if (cs2.Count == 2)
							{
								if (   cs[0].c == cs2[0].c && cs[1].c == cs2[1].c
									&& cs[0].r == cs[1].r && cs2[0].r == cs2[1].r)
								{
									for (int r3 = 0; r3 < 9; r3++)
									{
										if (r3 != cs[0].r && r3 != cs2[0].r)
										{
											values[r3, cs[0].c] = values[r3, cs[0].c].Replace(n.ToString(), "");
											values[r3, cs[1].c] = values[r3, cs[1].c].Replace(n.ToString(), "");
										}
									}
								}
							}
						}
					}
				}
				
				for (int c = 0; c < 9; c++)
				{
					List<BoxSpot> rs = new List<BoxSpot>();

					for (int r = 0; r < 9; r++)
					{
						if (values[r, c].Contains(n.ToString()))
						{
							rs.Add(new BoxSpot(r, c));
						}
					}

					if (rs.Count == 2)
					{
						for (int c2 = c + 1; c2 < 9; c2++)
						{
							List<BoxSpot> rs2 = new List<BoxSpot>();

							for (int r2 = 0; r2 < 9; r2++)
							{
								if (values[r2, c2].Contains(n.ToString()))
								{
									rs2.Add(new BoxSpot(r2, c2));
								}
							}

							if (rs2.Count == 2)
							{
								if (   rs[0].c == rs2[0].c && rs[1].c == rs2[1].c
									&& rs[0].r == rs[1].r && rs2[0].r == rs2[1].r)
								{
									for (int c3 = 0; c3 < 9; c3++)
									{
										if (c3 != rs[0].c && c3 != rs2[0].c)
										{
											values[rs[0].r, c3] = values[rs[0].r, c3].Replace(n.ToString(), "");
											values[rs[1].r, c3] = values[rs[1].r, c3].Replace(n.ToString(), "");
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	class BoxSpot
	{
		public BoxSpot()
		{

		}

		public BoxSpot(int _r, int _c)
		{
			r = _r;
			c = _c;
		}

		public int r;
		public int c;
		public int b;
		public int n;

		public bool CompareBox(int _r, int _c)
		{
			int this_r = GetCloseBox(r);
			int this_c = GetCloseBox(c);

			int that_r = GetCloseBox(_r);
			int that_c = GetCloseBox(_c);

			return (this_r == that_r && this_c == that_c);
		}

		private int GetCloseBox(int start)
		{
			while (start != 0 && start != 3 && start != 6)
			{
				start--;
			}

			return start;
		}
	}
}