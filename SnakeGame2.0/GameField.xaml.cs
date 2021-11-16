using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;

namespace SnakeGame2._0
{
    
    /// <summary>
    /// Логика взаимодействия для GameField.xaml
    /// </summary>


    public partial class GameField : Window
    {
        public DelegateAddBody DelegateCanBodyAdd;
        public DelegateCutBody DelegateCut;
        Func<int> DelegateFunc;
        GameMechanick mechanick;
        DispatcherTimer timer;
        DispatcherTimer FoodTimer;
        SnakeHead head;
        Food food;
        List<SnakeHead> Body;
        Image[] ArrayHeart;
        object locker;
        bool Damage;
        int Health;
        public GameField()
        {
            InitializeComponent();
            InitParametrs();
            timer.Start();
            FoodTimer.Start();
         
        }
        void InitParametrs()
        {
            DelegateCanBodyAdd = CanAddBody;
            DelegateCut = CanCutBody;

            locker = new object();

            head = new SnakeHead();
            Body = new List<SnakeHead>() { head };

            mechanick = new GameMechanick(this);
            food = new Food(mechanick.FoodPoint);


            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            timer.Tick += Timer_Tick;

            FoodTimer = new DispatcherTimer();
            FoodTimer.Interval = new TimeSpan(0, 0, 0, 0);
            FoodTimer.Tick += FoodTimer_Tick;


            Field.Children.Add(head);
            Field.Children.Add(food);

            SetOnCanvas(mechanick.Player.X, mechanick.Player.Y, head);
            SetOnCanvas(mechanick.FoodPoint.X, mechanick.FoodPoint.Y, food);

            Health = 3;
        
      
        }
        private void FoodTimer_Tick(object sender, EventArgs e)
        {
            SetOnCanvas(mechanick.FoodPoint.X, mechanick.FoodPoint.Y, food);
            Points.Text =mechanick.Score.ToString();
            HeartScore.Text = Health.ToString();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (mechanick.locker)
            {
                GoLabel:
                try
                {                                                                                           
                    if (mechanick.BodyPoints.Count() == Body.Count())
                        for (int i = 0; i < Body.Count(); i++)
                            SetOnCanvas(mechanick.BodyPoints[i].X, mechanick.BodyPoints[i].Y, Body[i]);
                }
                catch
                {
                    goto GoLabel;
                }
              
            }
        }
        private void SetOnCanvas(double X, double Y, UIElement Element)
        {
            Canvas.SetLeft(Element, X);
            Canvas.SetTop(Element, Y);
        }
        public void CanAddBody()
        {
            if (mechanick.AddBody)
                AddBody();
        }
        private void AddBody()
        {
            lock (mechanick.locker)
            {                                                                     // Была ошибка "Вызывающим потоком должен быть STA"
                Dispatcher.BeginInvoke(() =>                                      // Использование Dispatcher решило проблему
                {
                    for (int i = 0; i < 15; i++)                                  //todo: Установить GrowIndex
                    {
                        SnakeHead snake1 = new SnakeHead();
                        SetOnCanvas(mechanick.Player.X, mechanick.Player.Y, snake1);
                        Field.Children.Add(snake1);
                        Body.Add(snake1);
                    }
                });
            }
        }
        public void CanCutBody()
        {
            if (mechanick.CutBody)
                CutBody();
        }
        private void CutBody()
        {
            RemoveElement();   
        }
        async void RemoveElement()
        {
            await Task.Run(() =>
            {
                lock (mechanick.locker)
                {
                    Dispatcher.BeginInvoke(() =>
                {
                    for (int i = mechanick.IndexBody; i < Body.Count(); i++)
                    {
                        Field.Children.Remove(Body[i]);
                        Body.RemoveAt(i);
                    }
                    });
                }
            });           
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    if (mechanick.SnakeTrend != Trend.Down)
                        mechanick.TrendUp();
                    break;
                case Key.S:
                    if (mechanick.SnakeTrend != Trend.Up)
                        mechanick.TrendDown();
                    break;
                case Key.D:
                    if (mechanick.SnakeTrend != Trend.Left)
                        mechanick.TrendRight();
                    break;
                case Key.A:
                    if (mechanick.SnakeTrend != Trend.Right)
                        mechanick.TrendLeft();
                    break;
                case Key.Space:
                    mechanick.TrendPause();
                    switch (Pause.Visibility) 
                    {
                        case Visibility.Visible:
                            Pause.Visibility = Visibility.Hidden;
                            break;
                        case Visibility.Hidden:
                            Pause.Visibility = Visibility.Visible;
                            break;
                        default:
                            break;
                    }
                   

                    break;
                default:
                    break;
            }
        }
        public int GetBorderX(System.Windows.Shapes.Line rectangle)
        {
          int X=0;
          rectangle.Dispatcher.Invoke((Action)(() => { X= (int)rectangle.X1; }));
          return X;
        } 
        public int GetBorderY(System.Windows.Shapes.Line rectangle)
        {
            int Y = 0;
            rectangle.Dispatcher.Invoke((Action)(() => { Y = (int)rectangle.Y1; }));
            return Y;

        }
      
    }
}