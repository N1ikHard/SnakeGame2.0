using System.Windows;
using System.Windows.Controls;


namespace SnakeGame2._0
{
    /// <summary>
    /// Логика взаимодействия для Food.xaml
    /// </summary>
    /// 

    public partial class Food : UserControl
    {
     
        public double FoodPointX { get; private set; }
        public double FoodPointY { get; private set; }
   
        public Food(System.Drawing.Point foodPoint)
        {
            InitializeComponent();
            FoodPointX = foodPoint.X;
            FoodPointY = foodPoint.Y;
        }
        public Food()
        {
            InitializeComponent();
        }
 
    }
}
