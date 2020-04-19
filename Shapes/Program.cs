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
        protected int[,] OwnMap = new int[10,10];
        protected int[,] EnemyMap = new int[10,10];

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
            int choice;
            Point choosenBox = new Point();

            for(int i=0; i<10; i++)
                for(int j=0; j<10; j++)
                {
                    OwnMap[i, j] = 0;
                    freeBoxes.Add(new Point(i, j));
                }

            // 4masztowiec x1
            choice = random.Next(0, freeBoxes.Count() - 1);
            setBox(choice, ref freeBoxes, ref choosenBox);
            setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);
            for(int i=0; i<3; i++)
            {
                choice = random.Next(0, freeNeighbours.Count() - 1);
                setBox(choice, ref freeNeighbours, ref choosenBox);
                freeNeighbours.Remove(choosenBox);
                setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);
            }
            blockfreeNeighbours(ref freeNeighbours, ref freeBoxes);

            // 3masztowiec x2
            for(int i=0; i<2; i++)
            {
                choice = random.Next(0, freeBoxes.Count() - 1);
                setBox(choice, ref freeBoxes, ref choosenBox);
                setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);
                for (int j = 0; j < 2; j++)
                {
                    choice = random.Next(0, freeNeighbours.Count() - 1);
                    setBox(choice, ref freeNeighbours, ref choosenBox);
                    freeNeighbours.Remove(choosenBox);
                    setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);
                }
                blockfreeNeighbours(ref freeNeighbours, ref freeBoxes);
            }

            //2masztowiec x3
            for (int i = 0; i < 3; i++)
            {
                choice = random.Next(0, freeBoxes.Count() - 1);
                setBox(choice, ref freeBoxes, ref choosenBox);
                setFreeNeighbours(ref choosenBox, ref freeNeighbours, ref freeBoxes);

                choice = random.Next(0, freeNeighbours.Count() - 1);
                setBox(choice, ref freeNeighbours, ref choosenBox);
                freeNeighbours.Remove(choosenBox);
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

        private void blockfreeNeighbours(ref List<Point> freeNeighbours, ref List<Point> freeBoxes)
        {
            foreach(var fn in freeNeighbours)
                freeBoxes.Remove(fn);

            freeNeighbours.Clear();
        }

        private void setFreeNeighbours(ref Point choosenBox, ref List<Point> freeNeighbours, ref List<Point> freeBoxes)
        {
            int x, y;

            x = choosenBox.X - 1;
            y = choosenBox.Y;
            if (x >= 0 && freeBoxes.Exists(fb => fb.X == x && fb.Y ==y))
                freeNeighbours.Add(new Point(x, y));

            x = choosenBox.X + 1;
            if (x <= 9 && freeBoxes.Exists(fb => fb.X == x && fb.Y == y))
                freeNeighbours.Add(new Point(x, y));

            x = choosenBox.X;
            y = choosenBox.Y - 1;
            if (y <= 9 && freeBoxes.Exists(fb => fb.X == x && fb.Y == y))
                freeNeighbours.Add(new Point(x, y));

            y = choosenBox.Y + 1;
            if (y >= 0 && freeBoxes.Exists(fb => fb.X == x && fb.Y == y))
                freeNeighbours.Add(new Point(x, y));
        }

        private void setBox(int choice, ref List<Point> availableBoxes, ref Point choosenBox)
        {
            choosenBox = availableBoxes.ElementAt(choice);
            choosenBox = new Point(choosenBox.X, choosenBox.Y);

            OwnMap[choosenBox.X, choosenBox.Y] = 1;
            availableBoxes.RemoveAt(choice);
        }

        private void setBox(int choice)
        {
            throw new NotImplementedException();
        }
    }

    public class UserPlayer : Player
    {
        public override void CreateShapes()
        {
            
        }
    }

    public class Game
    {
        private UserPlayer UserPlayer;
        private ComputerPlayer ComputerPlayer;

        private Player Move;

        private Button[,] leftMap;
        private Button[,] rightMap;
        private Form1 form1;

        public Game()
        {
            
        }

        public Game(Form1 form1)
        {
            this.form1 = form1;
            UserPlayer = new UserPlayer();
            ComputerPlayer = new ComputerPlayer();

            Button b;
            leftMap = new Button[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    b = new Button();
                    b.Location = new Point(94 + i * (32 + 1), 328 + j * (32 + 1));
                    b.Size = new Size(32, 32);
                    b.Text = "";
                    b.Enabled = false;
                    this.form1.Controls.Add(b);
                    leftMap[i, j] = b;
                }

            rightMap = new Button[10, 10];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    b = new Button();
                    b.Location = new Point(500 + i * (32 + 1), 328 + j * (32 + 1));
                    b.Size = new Size(32, 32);
                    b.Text = "";
                    this.form1.Controls.Add(b);
                    rightMap[i, j] = b;
                }
        }

        internal void InitGame()
        {
            ComputerPlayer.CreateShapes();
            UserPlayer.CreateShapes();
        }
    }
}
