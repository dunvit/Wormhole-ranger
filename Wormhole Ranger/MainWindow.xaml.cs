using Dragablz;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wormhole_Ranger.Views;

namespace Wormhole_Ranger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RefreshWindow();
        }

        private void RefreshWindow()
        {
            Left = ApplicationSettings.Window.Location.X;
            Top = ApplicationSettings.Window.Location.Y;

            switch (ApplicationSettings.Window.State)
            {
                case State.States.Maximated:
                    ButtonMaximaze.Visibility = Visibility.Collapsed;
                    ButtonMinimaze.Visibility = Visibility.Visible;
                    break;
                case State.States.Minimizated:
                    ButtonMinimaze.Visibility = Visibility.Collapsed;
                    ButtonMaximaze.Visibility = Visibility.Visible;
                    break;
                case State.States.Hidden:
                    ButtonMinimaze.Visibility = Visibility.Collapsed;
                    ButtonMaximaze.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            Height = ApplicationSettings.Window.Height;
            Width = ApplicationSettings.Window.Width;

            switch (ApplicationSettings.Window.IsPinned)
            {
                case true:
                    ButtonPinOn.Visibility = Visibility.Collapsed;
                    ButtonPinOff.Visibility = Visibility.Visible;
                    Topmost = true;
                    break;

                case false:
                    ButtonPinOff.Visibility = Visibility.Collapsed;
                    ButtonPinOn.Visibility = Visibility.Visible;
                    Topmost = false;
                    break;
            }
        }

        private void ButtonPower_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GridTitlebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                switch (ApplicationSettings.Window.State)
                {
                    case State.States.Maximated:
                        ApplicationSettings.Window.ChangeState(State.States.Minimizated);
                        break;
                    case State.States.Minimizated:
                        ApplicationSettings.Window.ChangeState(State.States.Maximated);
                        break;
                }
                
                RefreshWindow();
                return;
            }

            DragMove();
            ApplicationSettings.Window.ChangeLocation(new System.Windows.Point(Left, Top));
        }

        private void ButtonMinimaze_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.Window.ChangeState(State.States.Minimizated);
            RefreshWindow();
        }

        private void ButtonMaximaze_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.Window.ChangeState(State.States.Maximated);
            RefreshWindow();
        }

        private void ButtonPinOff_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.Window.ChangePinState(false);
            RefreshWindow();
        }

        private void ButtonPinOn_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.Window.ChangePinState(true);
            RefreshWindow();
        }

        private void TabablzControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabName = ((sender as TabablzControl).SelectedItem as TabItem).Name as string;

            switch (tabName)
            {
                case "Control_Characters":
                    DataContext = ModulesControls.ModuleCharacters;
                    break;
                case "Pilots":
                    DataContext = ModulesControls.ModuleBlueView;
                    break;
                case "Browser":
                    DataContext = ModulesControls.ModuleOrangeView;
                    break;
                default:
                    break;
            }
        }
    }
}
