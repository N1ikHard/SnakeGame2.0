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

namespace SnakeGame2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    enum Trend { Up, Down, Left, Right, Stop }

    #region Делегаты
    public delegate void DelegateAddBody();
    public delegate void DelegateCutBody();
    #endregion

    public partial class MainWindow : Window
    {
       

        GameField field;

        public MainWindow()
        {          
           InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            field = new GameField();
            field.ShowDialog();
            field.Focus();
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
    }
}
