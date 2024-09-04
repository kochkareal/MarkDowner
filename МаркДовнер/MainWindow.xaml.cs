// MainWindow.xaml.cs
using System.Windows;
using System.Windows.Input;
using МаркДовнер.ViewModels;

namespace МаркДовнер
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveExecuted));
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            viewModel?.SaveCommand.Execute(null);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SaveExecuted(this, null);
            }
            base.OnKeyDown(e);
        }
    }
}