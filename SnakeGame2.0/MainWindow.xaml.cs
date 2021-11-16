using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    #region Перечисления
    enum Trend { Up, Down, Left, Right, Stop }
    #endregion
    #region Делегаты
    public delegate void DelegateAddBody();
    public delegate void DelegateCutBody();
    #endregion

    public partial class MainWindow : Window
    {




        public MainWindow()
        {
            InitializeComponent();
        }


        #region GameParametrs
        #region Classes
        GameMechanick mechanick;
        SnakeHead head;
        Food food;
        Object locker;
        #endregion
        #region Timers
        DispatcherTimer Timer;
        DispatcherTimer FoodTimer;
        #endregion
        #region Delegates
        public DelegateAddBody DelegateCanBodyAdd;
        public DelegateCutBody DelegateCut;
        Func<int> DelegateFunc;
        #endregion
        #region List
        List<SnakeHead> Body;
        #endregion
        #endregion

        void InitGameParam()
        {

            DelegateCanBodyAdd = CanAddBody;
            DelegateCut = CanCutBody;

            locker = new object();

            head = new SnakeHead();
            Body = new List<SnakeHead>() { head };

            mechanick = new GameMechanick(this);
            food = new Food(mechanick.FoodPoint);


            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 0);
            Timer.Tick += Timer_Tick;

            FoodTimer = new DispatcherTimer();
            FoodTimer.Interval = new TimeSpan(0, 0, 0, 0);
            FoodTimer.Tick += FoodTimer_Tick;

            CanvasMap.Children.Add(head);
            CanvasMap.Children.Add(food);

            SetOnCanvas(mechanick.Player.X, mechanick.Player.Y, head);
            SetOnCanvas(mechanick.FoodPoint.X, mechanick.FoodPoint.Y, food);

            Timer.Start();
            FoodTimer.Start();
        }

        private void FoodTimer_Tick(object sender, EventArgs e)
        {
            SetOnCanvas(mechanick.FoodPoint.X, mechanick.FoodPoint.Y, food);
            Points.Text = mechanick.Score.ToString();
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

        void SetOnCanvas(double X, double Y, UIElement Element)
        {

            Canvas.SetLeft(Element, X);
            Canvas.SetTop(Element, Y);
        }

        #region ButtonEvent
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Hidden;
            CanvasMap.Visibility = Visibility.Visible;
            InitGameParam();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Visible;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Hidden;
            Menu.Visibility = Visibility.Visible;
        }

        private void ButtonWall_MouseEnter(object sender, MouseEventArgs e)
        {
            Discription.Text = "Использование границ. При включенных границах , змейка получит урон и игра будет закончена. Выключенные границы позволят змейке появиться с обратной стороны.";
        }

        private void ButtonDamage_MouseEnter(object sender, MouseEventArgs e)
        {
            Discription.Text = "При включенном уроне змейка , проходя через своё туловище будет откусывать часть своего тела.";

        }

        private void ButtonHealth_MouseEnter(object sender, MouseEventArgs e)
        {
            Discription.Text = "Добавляет в игру здоровье при использовании урона. Трата всех единиц здоровья будет означать проигрышь.";

        }

        private void ButtonBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Discription.Text = "После получение очков на поле будет появляться объект , касание с которым будет означать проигрышь.";

        }

        private void ButtonGrow_MouseEnter(object sender, MouseEventArgs e)
        {
            Discription.Text = "Параметр регулирующий скорость роста.";

        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
        public void CanCutBody()
        {
            if (mechanick.CutBody)
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
                            CanvasMap.Children.Remove(Body[i]);
                            Body.RemoveAt(i);
                        }
                    });
                }
            });
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
                        CanvasMap.Children.Add(snake1);
                        Body.Add(snake1);
                    }
                });
            }
        }
        public int GetBorderX(System.Windows.Shapes.Line rectangle)
        {
            int X = 0;
            rectangle.Dispatcher.Invoke((Action)(() => { X = (int)rectangle.X1; }));
            return X;
        }
        public int GetBorderY(System.Windows.Shapes.Line rectangle)
        {
            int Y = 0;
            rectangle.Dispatcher.Invoke((Action)(() => { Y = (int)rectangle.Y1; }));
            return Y;

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
    
        
    }
}
