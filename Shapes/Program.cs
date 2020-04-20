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
        private List<Point> Ship;
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
            Ship = new List<Point>();

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
                    b.Click += new EventHandler(form1.ButtonLefttMap_Click);
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
            GetCoordinates(ref x, ref y, b, rightMap);
            neighbours.Clear();
            FindAllNeighbours(x, y);

            if (!shapeAlready)
            {
                //sprawdz czy moze byc
                if (boxesUsed >= 20 || neighbours.Count() > 3)
                    return;
              
                UserPlayer.OwnMap[x, y] = 1;
                changeBackground(b, Color.Yellow);
                ++boxesUsed;
            }
            else
            {
                UserPlayer.OwnMap[x, y] = 0;
                changeBackground(b, Color.LightSkyBlue);
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

            for(int i=0; i<10; i++)
                for (int j=0; j<10; j++)
                {
                    if(UserPlayer.OwnMap[i,j] == 1)
                    {
                        neighbours.Clear();
                        Ship.Clear();

                        FindWholeShip(i, j);
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
            switch(Ship.Count())
            {
                case 1:
                    {
                        return UserPlayer.fleet.singles.Contains(Ship[0]);
                    }
                case 2:
                    {
                        foreach(var s in UserPlayer.fleet.doubles)
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

        private void FindWholeShip(int i, int j)
        {
            Ship.Add(new Point(i, j));
            FindAllNeighbours(i, j);
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
                form1.Controls["panel1"].Controls["btnPlay"].BackColor = SystemColors.Control;
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
            if(UserPlayer.fleet.singles.Count() == 4)
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

        private void FindAllNeighbours(int x, int y)
        {
            CheckNeighbours(x, y);

            if (neighbours.Contains(new Point(x, y)))
                neighbours.Remove(new Point(x, y));
        }

        private void CheckNeighbours(int x, int y)
        {
            int nx = x - 1;
            int ny = y;
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

        private void GetCoordinates(ref int x, ref int y, Button b, Button [,] table)
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
            sender.Enabled = false;
            sender.BackColor = SystemColors.Control;

            freeBoxes.Clear();
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    freeBoxes.Add(new Point(i, j));

            UserMove();
        }

        private void UserMove()
        {
            Move = UserPlayer;
            UnlockLeftMap();
        }

        private void UnlockLeftMap()
        {
            foreach (var btn in leftMap)
                btn.Enabled = true;
        }

        internal void ButtonLeftMapClicked(Button b)
        {
            if (Move is ComputerPlayer)
                return;

            int x = 0, y = 0;
            GetCoordinates(ref x, ref y, b, leftMap);
            
            if(ComputerPlayer.OwnMap[x,y] == 1)
            {
                leftMap[x, y].BackColor = Color.Red;
            }
            else
            {
                leftMap[x, y].BackColor = Color.Black;
            }

            LockLeftMap();

            Move = ComputerPlayer;
            ComputerMove();
        }

        private void ComputerMove()
        {
            choice = ComputerPlayer.random.Next(0, freeBoxes.Count());
            choosenBox = freeBoxes.ElementAt(choice);

            if(UserPlayer.OwnMap[choosenBox.X, choosenBox.Y] == 1)
            {
                rightMap[choosenBox.X, choosenBox.Y].BackColor = Color.Red;
            }
            else
            {
                rightMap[choosenBox.X, choosenBox.Y].BackColor = Color.Black;
            }

            freeBoxes.Remove(choosenBox);

            UserMove();
        }

        private void LockLeftMap()
        {
            foreach (var btn in leftMap)
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
}

