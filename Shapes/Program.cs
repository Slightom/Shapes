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

        public abstract void CreateShapes();
    }

    public class ComputerPlayer : Player
    {
        Random random;

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
            if (x >= 0 && freeBoxes.Exists(fb => fb.X == x && fb.Y == y) && !freeNeighbours.Contains(new Point(x,y)))
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
    }

    public class Game
    {
        private UserPlayer UserPlayer;
        private ComputerPlayer ComputerPlayer;

        public Player Move;

        private Button[,] leftMap;
        private Button[,] rightMap;
        private Form1 form1;

        private List<Point> freeBoxes;
        private List<Point> freeNeighbours;
        private List<Point> neighbours;
        private int choice;
        private Point choosenBox;
        private List<Point> OldShip;
        private List<Point> NewShape;



        private int boxesUsed;

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
            OldShip = new List<Point>();

            UserPlayer = new UserPlayer();
            ComputerPlayer = new ComputerPlayer();

            Button b;
            leftMap = new Button[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    b = new Button();
                    b.Location = new Point(94 + i * (32), 328 + j * (32));
                    b.Size = new Size(32, 32);
                    b.Text = "";
                    b.Enabled = false;
                    b.Name = "leftMap" + i.ToString() + j.ToString();
                    this.form1.Controls.Add(b);
                    leftMap[i, j] = b;
                }

            rightMap = new Button[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    b = new Button();
                    b.Location = new Point(500 + i * (32), 328 + j * (32));
                    b.Size = new Size(32, 32);
                    b.Text = "";
                    b.BackColor = Color.LightSkyBlue;
                    b.Name = "righttMap" + i.ToString() + j.ToString();
                    b.Click += new EventHandler(form1.ButtonRightMap_Click);
                    this.form1.Controls.Add(b);
                    rightMap[i, j] = b;
                }

            boxesUsed = 0;
        }

        internal void InitGame()
        {
            ComputerPlayer.CreateShapes();
            for(int i=0; i<10; i++)
                for(int j=0; j<10; j++)
                {
                    if (ComputerPlayer.OwnMap[i, j] == 1)
                        leftMap[i, j].BackColor = Color.Yellow;
                }
            UserPlayer.CreateShapes();
        }

        internal void ButtonRightMapClicked(Button b)
        {
            bool shapeAlready = b.BackColor == Color.Yellow ? true : false;
            int x = 0, y = 0;
            GetCoordinates(ref x, ref y, b);
            neighbours.Clear();
            OldShip.Clear();
            CheckNeighbours(x, y);
            int neighboursAmount = neighbours.Count();

            if (!shapeAlready)
            {
                //sprawdz czy moze byc
                if (boxesUsed >= 20)
                    return;

                switch (neighboursAmount)
                {
                    case 0:
                        {
                            if (UserPlayer.fleet.singles.Count() == 4)
                                return;

                            UserPlayer.fleet.singles.Add(new Point(x, y));
                            break;
                        }
                    case 1:
                        {
                            if (UserPlayer.fleet.doubles.Count() == 3)
                                return;

                            FindOldShape(1);
                            UserPlayer.fleet.singles.Remove(OldShip[0]);
                            OldShip.Add(new Point(x, y));
                            UserPlayer.fleet.doubles.Add(new List<Point>(OldShip));
                            break;
                        }
                    case 2:
                        {
                            if (UserPlayer.fleet.triples.Count() == 2)
                                return;

                            FindOldShape(2);
                            UserPlayer.fleet.doubles.Remove(OldShip);
                            OldShip.Add(new Point(x, y));
                            UserPlayer.fleet.triples.Add(new List<Point>(OldShip));
                            break;
                        }
                    case 3:
                        {
                            if (UserPlayer.fleet.fourfold.Count() == 1)
                                return;

                            FindOldShape(3);
                            UserPlayer.fleet.triples.Remove(OldShip);
                            OldShip.Add(new Point(x, y));
                            UserPlayer.fleet.fourfold.Add(new List<Point>(OldShip));
                            break;
                        }
                    default:
                        return;
                }


                UserPlayer.OwnMap[x, y] = 1;
                changeBackground(b, Color.Yellow);
                ++boxesUsed;
            }
            else
            {
                switch(neighboursAmount)
                {
                    case 0:
                        {
                            UserPlayer.fleet.singles.Remove(new Point(x, y));
                            break;
                        }
                    case 1:
                        {
                            FindOldShape(2);
                            UserPlayer.fleet.doubles.Remove(OldShip);
                            break;
                        }
                    case 2:
                        {
                            FindOldShape(3);
                            UserPlayer.fleet.triples.Remove(OldShip);
                            break;
                        }
                    case 3:
                        {
                            FindOldShape(4);
                            UserPlayer.fleet.fourfold.Remove(OldShip);
                            break;
                        }
                }

                UserPlayer.OwnMap[x, y] = 0;
                changeBackground(b, Color.LightSkyBlue);
                --boxesUsed;
            }

            //changeBackground(b, c);
            ////create player's map
            //freeBoxes = new List<Point>();
            //freeNeighbours = new List<Point>();
            //choosenBox = new Point();

            //for (int i = 0; i < 10; i++)
            //    for (int j = 0; j < 10; j++)
            //    {
            //        game.UserPlayer.OwnMap[i, j] = 0;
            //        freeBoxes.Add(new Point(i, j));
            //    }
        }

        private void FindOldShape(int shipAmount)
        {
            switch (shipAmount)
            {
                case 1:
                    {
                        OldShip.Add(UserPlayer.fleet.singles.Find(x => x.X == neighbours[0].X && x.Y == neighbours[0].Y));
                        return;
                    }
                case 2:
                    {
                        foreach (var ship in UserPlayer.fleet.doubles)
                        {
                            if (ship.Contains(neighbours[0]))
                            {
                                OldShip = ship;
                                return;
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        foreach (var ship in UserPlayer.fleet.triples)
                        {
                            if (ship.Contains(neighbours[0]))
                            {
                                OldShip = ship;
                                return;
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        foreach (var ship in UserPlayer.fleet.fourfold)
                        {
                            if (ship.Contains(neighbours[0]))
                            {
                                OldShip = ship;
                                return;
                            }
                        }
                        break;
                    }
            }
        }

        private void CheckNeighbours(int x, int y)
        {
            int nx, ny;

            nx = x - 1;
            ny = y;
            if (nx >= 0 && UserPlayer.OwnMap[nx, ny] == 1 && !neighbours.Contains(new Point(nx, ny)))
            {
                neighbours.Add(new Point(nx, ny));
                CheckNeighbours(nx, ny);
            }

            nx = x + 1;
            ny = y;
            if (nx <= 9 && UserPlayer.OwnMap[nx, ny] == 1 && !neighbours.Contains(new Point(nx, ny)))
            {
                neighbours.Add(new Point(nx, ny));
                CheckNeighbours(nx, ny);
            }

            nx = x;
            ny = y - 1;
            if (ny >= 0 && UserPlayer.OwnMap[nx, ny] == 1 && !neighbours.Contains(new Point(nx, ny)))
            {
                neighbours.Add(new Point(nx, ny));
                CheckNeighbours(nx, ny);
            }

            nx = x;
            ny = y + 1;
            if (ny <= 9 && UserPlayer.OwnMap[nx, ny] == 1 && !neighbours.Contains(new Point(nx, ny)))
            {
                neighbours.Add(new Point(nx, ny));
                CheckNeighbours(nx, ny);
            }
        }

        private void GetCoordinates(ref int x, ref int y, Button b)
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (rightMap[i, j].Name == b.Name)
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
}

