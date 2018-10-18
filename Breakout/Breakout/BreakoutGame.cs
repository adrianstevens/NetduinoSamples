using System;

namespace Breakout
{
    public class BreakoutGame
    {
        Random rand = new Random();

        public Paddle Paddle { get; private set; }
        public Ball Ball { get; private set; }
        public Block[] Blocks { get; private set; }

        int width, height;

        public BreakoutGame(int width, int height)
        {
            Blocks = new Block[20];

            this.width = width;
            this.height = height;

            Initialize();

            ResetBoard();
            ResetBall();
        }

        public void Initialize ()
        {
            Paddle = new Paddle(24, 8, width / 2, height - 14);
            Ball = new Ball(2);

            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = new Block(4 + (i % 5) * (5 + BLOCK_WIDTH), 20 + (i/5)*(4 + BLOCK_HEIGHT),
                    BLOCK_WIDTH, BLOCK_HEIGHT);
            }
        }

        int BLOCK_WIDTH = 20;
        int BLOCK_HEIGHT = 8;
        public void ResetBoard ()
        {
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i].IsVisible = true;
            }
        }

        public void ResetBall ()
        {
            Ball.X = width / 2;
            Ball.Y = height / 2;

            Ball.XSpeed = rand.Next() % 6 - 3;
            Ball.YSpeed = rand.Next() % 2 + 3;
        }

        public void Left()
        {
            Paddle.X--;
        }

        public void Right()
        {
            Paddle.X++;
        }

        public void Update ()
        {
            Ball.X += Ball.XSpeed;
            Ball.Y += Ball.YSpeed;

            CheckCollisions();
        }

        void CheckCollisions ()
        {
            //Walls
            if(Ball.X - Ball.Radius <= 0 || 
               Ball.X + Ball.Radius >= width)
            {
                Ball.XSpeed *= -1;
                Ball.X += Ball.XSpeed;
            }
            
            if (Ball.Y - Ball.Radius <= 0 ||
               Ball.Y + Ball.Radius >= height)
            {
                Ball.YSpeed *= -1;
                Ball.Y += Ball.YSpeed;
            }

            //player paddle
            

        }
    }

    public class Paddle 
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Paddle (int width, int height, int x, int y)
        {
            Width = width;
            Height = height;
            X = x;
            Y = y;
        }

    }

    public class Block
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool IsVisible { get; set; }

        public Block (int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    public class Ball
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int XSpeed { get; set; }
        public int YSpeed { get; set; }
        public int Radius { get; private set; }

        public Ball (int radius)
        {
            Radius = radius;
        }
    }
}
