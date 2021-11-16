using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Input;


namespace SnakeGame2._0
{
    class GameMechanick 
    {
        private event DelegateAddBody EventCallMethod;
        private event DelegateCutBody EventCutBody;
        private Action DelegateSnakeMove;
        public Trend SnakeTrend { get; protected set; }
        public Point Player { get; protected set; }
        public List<Point> BodyPoints;
        public bool AddBody { get; private set;}
        public bool CutBody { get; private set; }
        private bool Pause;
        public int IndexBody { get; private set;}
        public int Score { get; private set; }

        Thread ThreadUseWall;
        Thread ThreadMove;
        Thread ThreadEating;

        public object locker { get; private set; }
        int MovePointX;
        int MovePointY;
        int GrowIndex;
        Random rnd;
        MainWindow field;
        public Point FoodPoint { get; protected set;}
        public GameMechanick(MainWindow gameField)
        {
            field = gameField;
            InitParametrs();

            ThreadMove.Start();
            ThreadEating.Start(); 
        }
        void InitParametrs()
        {
            AddBody = false;
            GrowIndex = 15;                                                                  //Поставить на выбор из формы главного меню         
            Pause = true;

            EventCallMethod += field.DelegateCanBodyAdd;
            EventCutBody += field.CanCutBody;

            Player = new Point((int)field.Width/2,
                              (int)field.Height/2);
            
            rnd = new Random();
           
            BodyPoints = new List<Point>() { new Point(Player.X, Player.Y) };
            locker = new object();

            SnakeTrend = Trend.Stop;

            DelegateSnakeMove +=MethodSnakeMove;

            FoodPoint = new Point(10, 10);

            ThreadMove =new Thread (()=> { InvokeDelegateMove(); });
            ThreadEating = new Thread(() => SnakeEat());

            Score = 0;

            CreateFood();
        }
        #region Направления
        public void TrendUp()
        {
            SnakeTrend = Trend.Up;
        }
        public void TrendDown()
        {
            SnakeTrend = Trend.Down;
        }
        public void TrendLeft()
        {
            SnakeTrend = Trend.Left;
        }
        public void TrendRight()
        {
            SnakeTrend = Trend.Right;
        }
        public void TrendPause()
        {
            Pause=!Pause;
        }
        #endregion
        void SnakeEat()
        {
            while (true)
            {
                if (Math.Abs(FoodPoint.X - Player.X) < 20 & Math.Abs(FoodPoint.Y - Player.Y) < 20)
                {
                    CreateFood();
                    AddBody = true;
                    SnakeGrow();
                    AddBody = false;
                    Score++;
                }
                if (BodyPoints.Count() > 5)
                     EatYourSelf();
            }   
        }
        void EatYourSelf()
        {
              Task.Run(() => 
            {
                for (int i = 5; i < BodyPoints.Count(); i++)
                    if (Math.Abs(BodyPoints[i].X - Player.X) < 1 & Math.Abs(BodyPoints[i].Y - Player.Y) < 1)
                    {
                        CutBody = true;
                        IndexBody = i;

                        EventCutBody?.Invoke();
                        lock (locker)
                        {
                            for (int k = BodyPoints.Count() - 1; k >= IndexBody; k--)
                                BodyPoints.Remove(BodyPoints[k]);
                            CutBody = false;
                        }
                    }
            });
        }
        void SnakeGrow()
        {
            for (int i = 0; i < GrowIndex; i++)
                BodyPoints.Add(new Point(BodyPoints.Last().X, BodyPoints.Last().Y));
            EventCallMethod?.Invoke();
        }
        void CreateFood()
        {           
            label1:
                int X = rnd.Next(field.GetBorderX(field.Border1) + 20, field.GetBorderX(field.Border2) - 20);
                int Y = rnd.Next(field.GetBorderY(field.Border3) + 20, field.GetBorderY(field.Border4) - 20);

                for (int i = 0; i < BodyPoints.Count(); i++)
                    if (X == BodyPoints[i].X & Y == BodyPoints[i].Y)
                        goto label1;
                FoodPoint = new Point(X, Y);   
        }
        void InvokeDelegateMove()
        {
            while(true)
               if(Pause) 
                    DelegateSnakeMove?.Invoke();
        }
        void MethodSnakeMove()
        {
            switch (SnakeTrend)
            {
                case Trend.Down:
                    MovePointY = 1;
                    MovePointX = 0;
                    break;
                case Trend.Up:
                    MovePointY = -1;
                    MovePointX = 0;
                    break;
                case Trend.Left:
                    MovePointY = 0;
                    MovePointX = -1;
                    break;
                case Trend.Right:
                    MovePointY = 0;
                    MovePointX = 1;
                    break;
                case Trend.Stop:
             
                    break;
                default:
                    MovePointX = 0;
                    MovePointY = 0;
                    break;
            }
            Player = new Point(Player.X + MovePointX,
                                Player.Y + MovePointY);
            for (int i = BodyPoints.Count() - 1, k = BodyPoints.Count() - 2; i > 0; i--, k--)
                BodyPoints[i] = new Point(BodyPoints[k].X, BodyPoints[k].Y);

            BodyPoints[0] = new Point(Player.X, Player.Y);
            Thread.Sleep(10);
        }
        void Wall()
        {
                     
        }
    }
    
}
