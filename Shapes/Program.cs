using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shapes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public abstract class Player
    {
        public int[,] OwnMap = new int[10, 10];
        public int[,] EnemyMap = new int[10, 10];
        public int moveCounter = 0;
        public int hitCounter = 0;
        public int missCounter = 0;
        public Point? lastMove;
        public status status;
        public List<Point> huntCandidates;
        public List<Point> availableBoxes;

        public Player()
        {
            lastMove = null;
            availableBoxes = new List<Point>();
            huntCandidates = new List<Point>();
        }

        public abstract void CreateShapes();

        public void ClearData()
        {
            availableBoxes.Clear();
            huntCandidates.Clear();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    OwnMap[i, j] = 0;
                    EnemyMap[i, j] = 0;
                    availableBoxes.Add(new Point(i, j));
                }
            moveCounter = 0;
            hitCounter = 0;
            missCounter = 0;
            lastMove = null;
        }
    }

    public class ComputerPlayer : Player
    {
        public Random random;


        public ComputerPlayer()
        {
            random = new Random();
        }
        public override void CreateShapes()
        {
            List<Point> freeBoxes = new List<Point>();
            List<Point> freeNeighbours = new List<Point>();
            List<Point> freeAvailableBoxesAround = new List<Point>();

            int choice;
            Point choosenBox = new Point();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    OwnMap[i, j] = 0;
                    freeBoxes.Add(new Point(i, j));
                }

            // 4masztowiec x1
            choice = random.Next(0, freeBoxes.Count() - 1);
            setBox(choice, ref freeBoxes, ref choosenBox);
            setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);
            for (int i = 0; i < 3; i++)
            {
                choice = random.Next(0, freeNeighbours.Count() - 1);
                setBox(choice, ref freeNeighbours, ref choosenBox);
                freeBoxes.Remove(choosenBox);
                setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);
            }
            blockfreeNeighbours(ref freeNeighbours, ref freeBoxes);

            // 3masztowiec x2
            for (int i = 0; i < 2; i++)
            {
                do
                {
                    freeAvailableBoxesAround.Clear();
                    choice = random.Next(0, freeBoxes.Count() - 1);
                    choosenBox = freeBoxes.ElementAt(choice);
                    ObtainFreeAvailableBoxesAround(choosenBox.X, choosenBox.Y, ref freeAvailableBoxesAround);
                } while (freeAvailableBoxesAround.Count() < 3);

                setBox(choice, ref freeBoxes, ref choosenBox);
                setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);

                for (int j = 0; j < 2; j++)
                {
                    choice = random.Next(0, freeNeighbours.Count() - 1);
                    setBox(choice, ref freeNeighbours, ref choosenBox);
                    freeBoxes.Remove(choosenBox);
                    setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);
                }
                blockfreeNeighbours(ref freeNeighbours, ref freeBoxes);
            }

            //2masztowiec x3
            for (int i = 0; i < 3; i++)
            {
                do
                {
                    freeAvailableBoxesAround.Clear();
                    choice = random.Next(0, freeBoxes.Count() - 1);
                    choosenBox = freeBoxes.ElementAt(choice);
                    ObtainFreeAvailableBoxesAround(choosenBox.X, choosenBox.Y, ref freeAvailableBoxesAround);
                } while (freeAvailableBoxesAround.Count() < 2);

                setBox(choice, ref freeBoxes, ref choosenBox);
                setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);

                choice = random.Next(0, freeNeighbours.Count() - 1);
                setBox(choice, ref freeNeighbours, ref choosenBox);
                freeBoxes.Remove(choosenBox);
                setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);

                blockfreeNeighbours(ref freeNeighbours, ref freeBoxes);
            }

            //1masztowiec x4
            for (int i = 0; i < 4; i++)
            {
                choice = random.Next(0, freeBoxes.Count() - 1);
                setBox(choice, ref freeBoxes, ref choosenBox);
                setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);

                blockfreeNeighbours(ref freeNeighbours, ref freeBoxes);
            }
        }

        private void ObtainFreeAvailableBoxesAround(int x, int y, ref List<Point> freeAvailableBoxesAround)
        {
            int nx, ny;

            nx = x - 1;
            ny = y;
            if (nx >= 0 && OwnMap[nx, ny] == 0 && !freeAvailableBoxesAround.Contains(new Point(nx, ny)))
            {
                freeAvailableBoxesAround.Add(new Point(nx, ny));
                ObtainFreeAvailableBoxesAround(nx, ny, ref freeAvailableBoxesAround);
            }

            nx = x + 1;
            ny = y;
            if (nx <= 9 && OwnMap[nx, ny] == 0 && !freeAvailableBoxesAround.Contains(new Point(nx, ny)))
            {
                freeAvailableBoxesAround.Add(new Point(nx, ny));
                ObtainFreeAvailableBoxesAround(nx, ny, ref freeAvailableBoxesAround);
            }

            nx = x;
            ny = y - 1;
            if (ny >= 0 && OwnMap[nx, ny] == 0 && !freeAvailableBoxesAround.Contains(new Point(nx, ny)))
            {
                freeAvailableBoxesAround.Add(new Point(nx, ny));
                ObtainFreeAvailableBoxesAround(nx, ny, ref freeAvailableBoxesAround);
            }

            nx = x;
            ny = y + 1;
            if (ny <= 9 && OwnMap[nx, ny] == 0 && !freeAvailableBoxesAround.Contains(new Point(nx, ny)))
            {
                freeAvailableBoxesAround.Add(new Point(nx, ny));
                ObtainFreeAvailableBoxesAround(nx, ny, ref freeAvailableBoxesAround);
            }
        }

        private void blockfreeNeighbours(ref List<Point> freeNeighbours, ref List<Point> freeBoxes)
        {
            foreach (var fn in freeNeighbours)
            {
                OwnMap[fn.X, fn.Y] = 2;
                freeBoxes.Remove(fn);
            }

            freeNeighbours.Clear();
        }

        private void setFreeNeighbours(ref Point choosenBox, ref List<Point> freeNeighbours, ref List<Point> freeBoxes)
        {
            int x, y;

            x = choosenBox.X - 1;
            y = choosenBox.Y;
            if (x >= 0 && freeBoxes.Exists(fb => fb.X == x && fb.Y == y) && !freeNeighbours.Contains(new Point(x, y)))
                freeNeighbours.Add(new Point(x, y));

            x = choosenBox.X + 1;
            if (x <= 9 && freeBoxes.Exists(fb => fb.X == x && fb.Y == y) && !freeNeighbours.Contains(new Point(x, y)))
                freeNeighbours.Add(new Point(x, y));

            x = choosenBox.X;
            y = choosenBox.Y - 1;
            if (y <= 9 && freeBoxes.Exists(fb => fb.X == x && fb.Y == y) && !freeNeighbours.Contains(new Point(x, y)))
                freeNeighbours.Add(new Point(x, y));

            y = choosenBox.Y + 1;
            if (y >= 0 && freeBoxes.Exists(fb => fb.X == x && fb.Y == y) && !freeNeighbours.Contains(new Point(x, y)))
                freeNeighbours.Add(new Point(x, y));
        }

        private void setBox(int choice, ref List<Point> availableBoxes, ref Point choosenBox)
        {
            choosenBox = availableBoxes.ElementAt(choice);
            choosenBox = new Point(choosenBox.X, choosenBox.Y);

            OwnMap[choosenBox.X, choosenBox.Y] = 1;
            availableBoxes.RemoveAt(choice);
        }
    }

    public class UserPlayer : Player
    {
        public Fleet fleet;
        public UserPlayer()
        {
            fleet = new Fleet();
        }
        public override void CreateShapes()
        {

        }

        public void ClearFleet()
        {
            fleet.singles.Clear();
            fleet.doubles.Clear();
            fleet.triples.Clear();
            fleet.fourfold.Clear();
        }
    }


    public class Game
    {
        private UserPlayer UserPlayer;
        private ComputerPlayer ComputerPlayer;

        public Player Move;

        private Button[,] leftMap;
        private Button[,] rightMap;
        private Form1 form1;

        private List<Point> neighbours;
        private List<Point> freeBoxes;
        private List<Point> freeNeighbours;
        private int choice;
        private Point choosenBox;
        private List<Point> Ship;

        private int boxesUsed;

        public static string hittedSign = "X";
        public static string missedSign = "•";
        public static Color hittedSunkenColor = Color.Red;
        public static Color playerShipColor = Color.Yellow;
        public static Color emptyColor = SystemColors.Control;
        public static Color borderLastMoveColor = Color.Blue;
        public static Color borderNormalColor = Color.Black;


        public Game()
        {
        }

        public Game(Form1 form)
        {
            InitComponents(form);
        }

        private void InitComponents(Form1 form)
        {
            form1 = form;
            freeBoxes = new List<Point>();
            freeNeighbours = new List<Point>();
            choosenBox = new Point();
            neighbours = new List<Point>();
            Ship = new List<Point>();

            UserPlayer = new UserPlayer();
            ComputerPlayer = new ComputerPlayer();

            Button b;
            leftMap = new Button[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    b = new Button();
                    b.Location = new Point(94 + i * (33), 328 + j * (33));
                    b.Size = new Size(32, 32);
                    b.Text = "";
                    b.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold);
                    b.Enabled = false;
                    b.Name = "leftMap" + i.ToString() + j.ToString();
                    b.Click += new EventHandler(form1.ButtonLefttMap_Click);
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 1;
                    b.TabStop = false;
                    this.form1.Controls.Add(b);
                    leftMap[i, j] = b;
                }

            rightMap = new Button[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    b = new Button();
                    b.Location = new Point(500 + i * (33), 328 + j * (33));
                    b.Size = new Size(32, 32);
                    b.Text = "";
                    b.Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold);
                    b.BackColor = emptyColor;
                    b.Name = "righttMap" + i.ToString() + j.ToString();
                    b.Click += new EventHandler(form1.ButtonRightMap_Click);
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 1;
                    b.TabStop = false;
                    this.form1.Controls.Add(b);
                    rightMap[i, j] = b;
                }
        }

        internal void InitGame()
        {
            ComputerPlayer.ClearData();
            UserPlayer.ClearData();
            UserPlayer.ClearFleet();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    leftMap[i, j].BackColor = emptyColor;
                    leftMap[i, j].Enabled = false;
                    leftMap[i, j].Text = "";
                    rightMap[i, j].BackColor = emptyColor;
                    rightMap[i, j].Enabled = true;
                    rightMap[i, j].Text = "";
                }

            ComputerPlayer.CreateShapes();

            form1.Controls["panel1"].Visible = true;

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    //if (ComputerPlayer.OwnMap[i, j] == 1)
                    //leftMap[i, j].BackColor = playerShipColor;
                }

            UnlockMap(rightMap);
            boxesUsed = 0;
        }


        #region tworzenie statków gracza
        internal void ButtonRightMapClicked(Button b)
        {
            bool shapeAlready = b.BackColor == playerShipColor ? true : false;
            int x = 0, y = 0;
            GetCoordinates(ref x, ref y, b, rightMap);
            neighbours.Clear();
            FindAllNeighbours(x, y, UserPlayer.OwnMap);

            if (!shapeAlready)
            {
                //sprawdz czy moze byc
                if (boxesUsed >= 20 || neighbours.Count() > 3)
                    return;

                UserPlayer.OwnMap[x, y] = 1;
                changeBackground(b, playerShipColor);
                ++boxesUsed;
            }
            else
            {
                UserPlayer.OwnMap[x, y] = 0;
                changeBackground(b, emptyColor);
                --boxesUsed;
            }

            UpdateFleet();
            UpdateLabels();
            UpdatePictureBoxes();
            ReadyToPlay();
        }

        private void UpdateFleet()
        {
            UserPlayer.fleet.singles.Clear();
            UserPlayer.fleet.doubles.Clear();
            UserPlayer.fleet.triples.Clear();
            UserPlayer.fleet.fourfold.Clear();

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    if (UserPlayer.OwnMap[i, j] == 1)
                    {
                        FindWholeShip(i, j, UserPlayer.OwnMap);
                        if (!ShipAlreadyInFleet())
                            AddShipToFleet();
                    }
                }
        }

        private void AddShipToFleet()
        {
            switch (Ship.Count())
            {
                case 1:
                    {
                        UserPlayer.fleet.singles.Add(Ship[0]);
                        break;
                    }
                case 2:
                    {
                        UserPlayer.fleet.doubles.Add(new List<Point>(Ship));
                        break;
                    }
                case 3:
                    {
                        UserPlayer.fleet.triples.Add(new List<Point>(Ship));
                        break;
                    }
                case 4:
                    {
                        UserPlayer.fleet.fourfold.Add(new List<Point>(Ship));
                        break;
                    }
            }
        }

        private bool ShipAlreadyInFleet()
        {
            switch (Ship.Count())
            {
                case 1:
                    {
                        return UserPlayer.fleet.singles.Contains(Ship[0]);
                    }
                case 2:
                    {
                        foreach (var s in UserPlayer.fleet.doubles)
                            if (s.All(Ship.Contains))
                                return true;
                        break;
                    }
                case 3:
                    {
                        foreach (var s in UserPlayer.fleet.triples)
                            if (s.All(Ship.Contains))
                                return true;
                        break;
                    }
                case 4:
                    {
                        foreach (var s in UserPlayer.fleet.fourfold)
                            if (s.All(Ship.Contains))
                                return true;
                        break;
                    }
            }
            return false;
        }

        private void FindWholeShip(int i, int j, int[,] map)
        {
            Ship.Clear();
            neighbours.Clear();
            Ship.Add(new Point(i, j));
            FindAllNeighbours(i, j, map);
            Ship.AddRange(neighbours);
        }

        private void ReadyToPlay()
        {
            if (UserPlayer.fleet.singles.Count() == 4 &&
                UserPlayer.fleet.doubles.Count() == 3 &&
                UserPlayer.fleet.triples.Count() == 2 &&
                UserPlayer.fleet.fourfold.Count() == 1)
            {
                form1.Controls["panel1"].Controls["btnPlay"].Enabled = true;
                form1.Controls["panel1"].Controls["btnPlay"].BackColor = Color.SpringGreen;
            }
            else
            {
                form1.Controls["panel1"].Controls["btnPlay"].Enabled = false;
                form1.Controls["panel1"].Controls["btnPlay"].BackColor = emptyColor;
            }

        }
        private void UpdateLabels()
        {
            form1.Controls["panel1"].Controls["label1"].Text = UserPlayer.fleet.singles.Count().ToString() + "/4";
            form1.Controls["panel1"].Controls["label2"].Text = UserPlayer.fleet.doubles.Count().ToString() + "/3";
            form1.Controls["panel1"].Controls["label3"].Text = UserPlayer.fleet.triples.Count().ToString() + "/2";
            form1.Controls["panel1"].Controls["label4"].Text = UserPlayer.fleet.fourfold.Count().ToString() + "/1";
        }
        private void UpdatePictureBoxes()
        {
            if (UserPlayer.fleet.singles.Count() == 4)
                form1.Controls["panel1"].Controls["pictureBox1"].BackColor = Color.Green;
            else
                form1.Controls["panel1"].Controls["pictureBox1"].BackColor = Color.Red;

            if (UserPlayer.fleet.doubles.Count() == 3)
                form1.Controls["panel1"].Controls["pictureBox2"].BackColor = Color.Green;
            else
                form1.Controls["panel1"].Controls["pictureBox2"].BackColor = Color.Red;

            if (UserPlayer.fleet.triples.Count() == 2)
                form1.Controls["panel1"].Controls["pictureBox3"].BackColor = Color.Green;
            else
                form1.Controls["panel1"].Controls["pictureBox3"].BackColor = Color.Red;

            if (UserPlayer.fleet.fourfold.Count() == 1)
                form1.Controls["panel1"].Controls["pictureBox4"].BackColor = Color.Green;
            else
                form1.Controls["panel1"].Controls["pictureBox4"].BackColor = Color.Red;
        }

        private void FindAllNeighbours(int x, int y, int[,] map)
        {
            CheckNeighbours(x, y, map);

            if (neighbours.Contains(new Point(x, y)))
                neighbours.Remove(new Point(x, y));
        }

        private void CheckNeighbours(int x, int y, int[,] map)
        {
            int nx = x - 1;
            int ny = y;
            if (nx >= 0 && map[nx, ny] == 1 && !neighbours.Contains(new Point(nx, ny)))
            {
                neighbours.Add(new Point(nx, ny));
                CheckNeighbours(nx, ny, map);
            }

            nx = x + 1;
            ny = y;
            if (nx <= 9 && map[nx, ny] == 1 && !neighbours.Contains(new Point(nx, ny)))
            {
                neighbours.Add(new Point(nx, ny));
                CheckNeighbours(nx, ny, map);
            }

            nx = x;
            ny = y - 1;
            if (ny >= 0 && map[nx, ny] == 1 && !neighbours.Contains(new Point(nx, ny)))
            {
                neighbours.Add(new Point(nx, ny));
                CheckNeighbours(nx, ny, map);
            }

            nx = x;
            ny = y + 1;
            if (ny <= 9 && map[nx, ny] == 1 && !neighbours.Contains(new Point(nx, ny)))
            {
                neighbours.Add(new Point(nx, ny));
                CheckNeighbours(nx, ny, map);
            }
        }
        #endregion


        private void GetCoordinates(ref int x, ref int y, Button b, Button[,] table)
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (table[i, j].Name == b.Name)
                    {
                        x = i;
                        y = j;
                        return;
                    }
        }

        private void changeBackground(Button b, Color c)
        {
            b.BackColor = c;
        }


        internal void BtnPlayClicked(Button sender)
        {
            form1.Controls["panel1"].Visible = false;
            form1.Controls["panel2"].Visible = true;
            sender.Enabled = false;
            sender.BackColor = emptyColor;

            //ComputerPlayer.ClearData();
            //UserPlayer.ClearData();
            LockMap(rightMap);
            UserMove();
        }

        private void UserMove()
        {
            Move = UserPlayer;
            UnlockLeftMap();
        }

        public void UnlockMap(Button[,] map)
        {
            foreach (var btn in map)
                btn.Enabled = true;
        }
        private void UnlockLeftMap()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (UserPlayer.availableBoxes.Contains(new Point(i, j)))
                        leftMap[i, j].Enabled = true;
        }

        internal void ButtonLeftMapClicked(Button b)
        {
            if (Move is ComputerPlayer)
                return;

            int x = 0, y = 0;
            GetCoordinates(ref x, ref y, b, leftMap);

            

            if (leftMap[x, y].Text != "")
                return;

            UpdateGhostBorder(leftMap, UserPlayer.lastMove, new Point(x, y));

            if (ComputerPlayer.OwnMap[x, y] == 1)
            {
                leftMap[x, y].Text = hittedSign;
                UserPlayer.hitCounter++;

                if (IsSunken(x, y, ComputerPlayer.OwnMap))
                {
                    ColorShip(hittedSunkenColor, leftMap);
                    ClearShip();
                }

                if (UserPlayer.hitCounter >= 20)
                {
                    UpdateUserScore();
                    GameOver();
                    return;
                }
            }
            else
            {
                leftMap[x, y].Text = missedSign;
                UserPlayer.missCounter++;

            }



            UserPlayer.availableBoxes.Remove(new Point(x, y));
            UserPlayer.lastMove = new Point(x, y);
            //LockLeftMap();
            UserPlayer.moveCounter++;

            UpdateUserScore();



            Move = ComputerPlayer;
            ComputerMove();
        }

        private void UpdateGhostBorder(Button[,] map, Point? lastMove, Point point)
        {
            map[point.X, point.Y].FlatAppearance.BorderColor = borderLastMoveColor;
            map[point.X, point.Y].FlatAppearance.BorderSize = (Move is ComputerPlayer) ? 3 : 2;

            if (lastMove != null)
            {
                Point last = (Point)lastMove;
                map[last.X, last.Y].FlatAppearance.BorderColor = borderNormalColor;
                map[last.X, last.Y].FlatAppearance.BorderSize = 1;
                //map[last.X, last.Y].PerformClick();
            }

        }

        private bool IsSunken(int x, int y, int[,] ownMap)
        {
            FindWholeShip(x, y, ComputerPlayer.OwnMap);
            foreach (var b in Ship)
                if (leftMap[b.X, b.Y].Text == missedSign || leftMap[b.X, b.Y].Text == "")
                    return false;

            return true;
        }

        private void UpdateUserScore()
        {
            form1.Controls["panel2"].Controls["luhit"].Text = UserPlayer.hitCounter + "/20";
        }

        private void NewGame()
        {
            form1.Controls["panel1"].Visible = true;
            form1.Controls["panel2"].Visible = false;
            ClearPanels12();
            ClearGhosts();

            InitGame();
        }

        private void ClearGhosts()
        {
            for(int i=0; i<10; i++)
                for(int j=0; j<10; j++)
                {
                    leftMap[i, j].FlatAppearance.BorderColor = borderNormalColor;
                    leftMap[i, j].FlatAppearance.BorderSize = 1;
                    rightMap[i, j].FlatAppearance.BorderColor = borderNormalColor;
                    rightMap[i, j].FlatAppearance.BorderSize = 1;
                }

        }

        private void ClearPanels12()
        {
            form1.Controls["panel2"].Controls["luhit"].Text = "0/20";
            form1.Controls["panel2"].Controls["lchit"].Text = "0/20";

            UserPlayer.ClearFleet();
            UpdateLabels();
            UpdatePictureBoxes();
        }

        private void ComputerMove()
        {
            if (ComputerPlayer.lastMove is null || ComputerPlayer.status == status.Missed || ComputerPlayer.status == status.HittedAndSunked)
            {
                choice = ComputerPlayer.random.Next(0, ComputerPlayer.availableBoxes.Count());
                choosenBox = ComputerPlayer.availableBoxes.ElementAt(choice);
            }
            else
            {
                choice = ComputerPlayer.random.Next(0, ComputerPlayer.huntCandidates.Count());
                choosenBox = ComputerPlayer.huntCandidates.ElementAt(choice);
            }

            Point tmp = new Point(choosenBox.X, choosenBox.Y);

            UpdateGhostBorder(rightMap, ComputerPlayer.lastMove, tmp);

            if (UserPlayer.OwnMap[choosenBox.X, choosenBox.Y] == 1)
            {
                rightMap[choosenBox.X, choosenBox.Y].Text = hittedSign;
                ++ComputerPlayer.hitCounter;
                ComputerPlayer.availableBoxes.Remove(choosenBox);

                UpdateComputerHuntCandidates(tmp.X, tmp.Y);
                ComputerPlayer.huntCandidates.Remove(new Point(tmp.X, tmp.Y));
                ComputerPlayer.status = IsSunked() ? status.HittedAndSunked : status.Hitted;


                if (ComputerPlayer.status == status.HittedAndSunked)
                {
                    BlockHuntCandidates();
                    ClearShip();
                    ComputerPlayer.huntCandidates.Clear();
                    // color whole ship sth like FindWholeShip(tmp.X, tmp.Y);
                    FindWholeShip(tmp.X, tmp.Y, UserPlayer.OwnMap);
                    ColorShip(hittedSunkenColor, rightMap);
                    ClearShip();
                }

                if (ComputerPlayer.hitCounter >= 20)
                {
                    UpdateComputerScore();
                    GameOver();
                    return;
                }
            }
            else
            {
                rightMap[choosenBox.X, choosenBox.Y].Text = missedSign;
                ComputerPlayer.missCounter++;
                ComputerPlayer.availableBoxes.Remove(choosenBox);

                if (ComputerPlayer.status == status.Hitted || ComputerPlayer.status == status.MissedHunt)
                {
                    ComputerPlayer.huntCandidates.Remove(new Point(tmp.X, tmp.Y));
                    ComputerPlayer.status = status.MissedHunt;
                }
                else
                {
                    ComputerPlayer.status = status.Missed;
                }
            }

            ComputerPlayer.lastMove = new Point(tmp.X, tmp.Y);
            ComputerPlayer.moveCounter++;

            UpdateComputerScore();
            UserMove();
        }

        private void ClearShip()
        {
            Ship.Clear();
            neighbours.Clear();
        }

        private void ColorShip(Color color, Button[,] map)
        {
            foreach (var b in Ship)
            {
                map[b.X, b.Y].BackColor = color;
            }
        }

        private void GameOver()
        {
            string msg = "";
            string title = "";

            if (ComputerPlayer.hitCounter >= 20)
            {
                msg = "Computer is the winner :( Try again";
                title = "GAMEOVER";
            }
            else
            {
                msg = "You are the winner!!! :D";
                title = "VICTORY";
            }
            MessageBox.Show(msg, title);

            NewGame();
        }

        private void UpdateComputerScore()
        {
            form1.Controls["panel2"].Controls["lchit"].Text = ComputerPlayer.hitCounter + "/20";
        }

        private void BlockHuntCandidates()
        {
            foreach (var b in ComputerPlayer.huntCandidates)
                ComputerPlayer.availableBoxes.Remove(new Point(b.X, b.Y));
        }

        private bool IsSunked()
        {
            foreach (var s in ComputerPlayer.huntCandidates)
                if (UserPlayer.OwnMap[s.X, s.Y] == 1)
                    return false;
            return true;
        }

        private void UpdateComputerHuntCandidates(int px, int py)
        {
            int x = px - 1;
            int y = py;
            if (x >= 0 && ComputerPlayer.availableBoxes.Contains(new Point(x, y)) && !ComputerPlayer.huntCandidates.Contains(new Point(x, y)))
                ComputerPlayer.huntCandidates.Add(new Point(x, y));

            x = px + 1;
            if (x <= 9 && ComputerPlayer.availableBoxes.Contains(new Point(x, y)) && !ComputerPlayer.huntCandidates.Contains(new Point(x, y)))
                ComputerPlayer.huntCandidates.Add(new Point(x, y));

            x = px;
            y = py - 1;
            if (y >= 0 && ComputerPlayer.availableBoxes.Contains(new Point(x, y)) && !ComputerPlayer.huntCandidates.Contains(new Point(x, y)))
                ComputerPlayer.huntCandidates.Add(new Point(x, y));


            y = py + 1;
            if (y <= 9 && ComputerPlayer.availableBoxes.Contains(new Point(x, y)) && !ComputerPlayer.huntCandidates.Contains(new Point(x, y)))
                ComputerPlayer.huntCandidates.Add(new Point(x, y));
        }

        private void LockMap(Button[,] map)
        {
            foreach (var btn in map)
                btn.Enabled = false;
        }
    }

    public class Fleet
    {
        public List<Point> singles;
        public List<List<Point>> doubles;
        public List<List<Point>> triples;
        public List<List<Point>> fourfold;

        public Fleet()
        {
            this.singles = new List<Point>();
            this.doubles = new List<List<Point>>();
            this.triples = new List<List<Point>>();
            this.fourfold = new List<List<Point>>();
        }

    }

    public enum status
    {
        Missed,
        Hitted,
        MissedHunt,
        HittedAndSunked
    }

}

