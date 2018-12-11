using System;
using System.Collections;

namespace Snake
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point (int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public enum SnakeDirection : byte
    {
        Up,
        Down,
        Left,
        Right,
        Stop, //to start
    }

    public class SnakeGame
    {
        public int Score { get; private set; }
        public int Level { get; private set; }
        
        public int BoardWidth {get; private set; }
        public int BoardHeight { get; private set;  }

        public bool Playing { get; private set; } = true;

        public ArrayList SnakePosition { get; private set; }

        public Point FoodPosition { get; private set; }

        public SnakeDirection Direction { get; set; }
 
        Random rand = new Random((int)DateTime.Now.Ticks);
        


        enum CellType : byte
        {
            Empty,
            Food,
        }

        public SnakeGame (int width, int height)
        {
            BoardWidth = width;
            BoardHeight = height;

            SnakePosition = new ArrayList();

            Reset();
        }

        public void Update()
        {
            if (Direction == SnakeDirection.Stop)
                return;

            for (int i = 0; i < SnakePosition.Count - 2; i++)
            {
                SnakePosition[i+1] = SnakePosition[i];
            }

            var head = (Point)SnakePosition[0];

            if (Direction == SnakeDirection.Left)
                head.X--;
            if (Direction == SnakeDirection.Right)
                head.X++;
            if (Direction == SnakeDirection.Up)
                head.Y--;
            if (Direction == SnakeDirection.Down)
                head.Y++;

            if(IsCellEmpty(head.X, head.Y) == false)
            {
                Playing = false;
            }

            //update the snake - may not be needed since reference type ....
            SnakePosition[0] = head;
        }

        void Reset ()
        {
            SnakePosition = new ArrayList();
            SnakePosition.Add(new Point(BoardWidth / 2, BoardHeight / 2));
            Direction = SnakeDirection.Stop;

            Level = 1;

            AddFood();
        }

        void AddFood ()
        {
            int foodX, foodY;
            do
            {
                foodX = rand.Next() % BoardWidth;
                foodY = rand.Next() % BoardHeight;
            }
            while (IsCellEmpty(foodX, foodY) == false);

            FoodPosition = new Point(foodX, foodY);
            Level++;
        }

        bool IsCellEmpty (int x, int y)
        {
            Point snakeBody;

            if (x < 0 || y < 0 || x >= BoardWidth || y >= BoardHeight)
                return false;

            for (int i = 0; i < SnakePosition.Count; i++)
            {
                snakeBody = (Point)SnakePosition[i];
                if (snakeBody.X == x && snakeBody.Y == y)
                    return false;
            }
            return true;
        }
    }
}