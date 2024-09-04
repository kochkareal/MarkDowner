// MainWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
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

            if (e.Key == Key.F1)
            {
                ShowHelpWindow();
                e.Handled = true;
            }
        }

        private void TitleBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox && (textBox.Name == "textTitleSmall" || textBox.Name == "textTitle"))
                {
                    textContent.Focus(); 
                    e.Handled = true; 
                }
            }
        }

        private void ShowHelpWindow()
        {
            MessageBox.Show("Инструкция и быстрые комбинации:\n\n" +
                            "Ctrl + S: Сохранить\n" +
                            "//Ctrl + B: Выделенное - Жирный текст\n",
                            "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}