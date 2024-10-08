using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Input;
using МаркДовнер.ViewModels;
using System.IO;

public class MainViewModel : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _content = string.Empty;

    // Свойство для хранения и получения заголовка документа
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    // Свойство для хранения и получения содержания документа
    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            OnPropertyChanged(nameof(Content));
        }
    }

    // Команда для сохранения файла
    public ICommand SaveCommand { get; }

    public MainViewModel()
    {
        Title = "Без названия";
        SaveCommand = new RelayCommand(SaveFile);
    }

    // Метод для сохранения файла
    private void SaveFile(object? obj)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Text file (*.txt)|*.txt|Markdown file (*.md)|*.md",
            Title = "Сохранить файл как...",
            FileName = GetValidFileName(Title)
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            // Сохраняем содержание
            File.WriteAllText(saveFileDialog.FileName, Content);
        }
    }

    // Метод для очистки имени файла от недопустимых символов
    private string GetValidFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Where(c => !invalidChars.Contains(c)));
    }

    // Событие для уведомления об изменении свойства
    public event PropertyChangedEventHandler? PropertyChanged;

    // Метод для вызова события изменения свойства
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
