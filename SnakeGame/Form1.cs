using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace SnakeGame
{
    public partial class MainForm : Form
    {
        private const int SnakeWidth = 10;
        private const int SnakeHeight = 10;
        private const int FoodWidth = 10;
        private const int FoodHeight = 10;

        private Snake snake;
        private Food food;
        private Random random;
        private int score=0;

        public MainForm()
        {
            InitializeComponent();
            random = new Random();
            NewGame();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                snake.Direction = Direction.Left;
            }
            else if (e.KeyCode == Keys.Right)
            {
                snake.Direction = Direction.Right;
            }
            else if (e.KeyCode == Keys.Up)
            {
                snake.Direction = Direction.Up;
            }
            else if (e.KeyCode == Keys.Down)
            {
                snake.Direction = Direction.Down;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            snake.Move();

            if (snake.CollidesWith(food))
            {
                snake.Grow();
                score += 10;
                label1.Text = "Score: " + score;
                PlaceFood();
            }
            else if (snake.CollidesWithSelf())
            {
                timer1.Stop();
                MessageBox.Show("Game Over");
                NewGame();
            }
            else if (snake.CollidesWithWall(pictureBox1.ClientSize))
            {
                timer1.Stop();
                MessageBox.Show("Game Over");
                NewGame();
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            snake.Draw(e.Graphics);
            food.Draw(e.Graphics);
        }

        private void NewGame()
        {
            snake = new Snake();
            snake.InitialPosition();
            PlaceFood();
            timer1.Start();
        }

        private void PlaceFood()
        {
            int x = random.Next(pictureBox1.ClientSize.Width / SnakeWidth) * SnakeWidth;
            int y = random.Next(pictureBox1.ClientSize.Height / SnakeHeight) * SnakeHeight;
            food = new Food(x, y, FoodWidth, FoodHeight);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            label1.Text = "Score: " + score;
        }
    }

    public class Snake
    {
        private const int StepSize = 10;

        private readonly List<Point> body;
        private bool isGrowing;

        public Snake()
        {
            body = new List<Point>();
            Direction = Direction.Right;
        }

        public Direction Direction { get; set; }

        public void InitialPosition()
        {
            for (int i = 0; i < 3; i++)
            {
                Point point = new Point(10 - i, 0);
                body.Insert(0, point);
            }
        }

        public void Move()
        {
            Point head = body[0];
            Point newHead = new Point(head.X, head.Y);

            if (Direction == Direction.Left)
            {
                newHead.X -= StepSize;
            }
            else if (Direction == Direction.Right)
            {
                newHead.X += StepSize;
            }
            else if (Direction == Direction.Up)
            {
                newHead.Y -= StepSize;
            }
            else if (Direction == Direction.Down)
            {
                newHead.Y += StepSize;
            }

            body.Insert(0, newHead);
            if (!isGrowing)
            {
                body.RemoveAt(body.Count - 1);
            }
            else
            {
                isGrowing = false;
            }
        }

        public bool CollidesWith(Point point)
        {
            return body.Any(b => b == point);
        }

        public bool CollidesWithSelf()
        {
            Point head = body[0];
            return body.Skip(1).Any(b => b == head);
        }

        public bool CollidesWithWall(Size bounds)
        {
            Point head = body[0];
            return head.X < 0 || head.Y < 0 || head.X >= bounds.Width || head.Y >= bounds.Height;
        }

        public bool CollidesWith(Food food)
        {
            return CollidesWith(food.Location);
        }

        public void Grow()
        {
            isGrowing = true;
        }

        public void Draw(Graphics graphics)
        {
            for (int i = 0; i < body.Count; i++)
            {
                Brush brush = i == 0 ? Brushes.Black : Brushes.Gray;
                graphics.FillRectangle(brush, new Rectangle(body[i].X, body[i].Y, StepSize, StepSize));
            }
        }
    }

    public class Food
    {
        public Food(int x, int y, int width, int height)
        {
            Location = new Point(x, y);
            Width = width;
            Height = height;
        }

        public Point Location { get; }
        public int Width { get; }
        public int Height { get; }

        public void Draw(Graphics graphics)
        {
            graphics.FillRectangle(Brushes.Red, new Rectangle(Location.X, Location.Y, Width, Height));
        }
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
}
