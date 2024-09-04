// ViewModels/MainViewModel.cs
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace МаркДовнер.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _content = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public ICommand SaveCommand { get; }

        public MainViewModel()
        {
            Title = "Без названия";
            SaveCommand = new RelayCommand(SaveFile);
        }

        private void SaveFile(object? obj) // Nullable параметр
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|Markdown file (*.md)|*.md";
            saveFileDialog.Title = "Сохранить файл как...";
            saveFileDialog.FileName = GetValidFileName(Title);

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, Content);
            }
        }

        private string GetValidFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Concat(fileName.Where(c => !invalidChars.Contains(c)));
        }

        public event PropertyChangedEventHandler? PropertyChanged; // Nullable событие

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
