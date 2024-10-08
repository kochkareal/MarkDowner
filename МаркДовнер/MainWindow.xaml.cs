// MainWindow.xaml.cs
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

        private bool _isUpdating = false; // Флаг для предотвращения рекурсивных вызовов

        private void RichTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return; // Прекращаем выполнение, если текст уже обновляется

            _isUpdating = true;

            try
            {
                // Получаем полный текст из RichTextBox
                var textRange = new TextRange(textContent.Document.ContentStart, textContent.Document.ContentEnd);

                // Синхронизируем текст с ViewModel
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.Content = textRange.Text;
                }

                // Форматируем только активную строку
                FormatActiveLine(); // Перенесите сюда

                // Форматируем слова с звездочками
                FormatBoldWords(textRange);

                // Форматируем слова с подчеркиваниями
                FormatUnderlineWords(textRange);
            }
            finally
            {
                _isUpdating = false; // Снимаем флаг после завершения обновления
            }
        }


        // Метод для отслеживания изменения позиции курсора
        private void RichTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdating) return; // Прекращаем выполнение, если текст уже обновляется

            // Обновляем форматирование строки при изменении позиции курсора
            FormatActiveLine();
        }

        private void FormatActiveLine()
        {
            // Текущая позиция курсора
            var caretPosition = textContent.CaretPosition;

            var currentBlock = caretPosition.Paragraph;
            if (currentBlock == null) return;

            var paragraphRange = new TextRange(currentBlock.ContentStart, currentBlock.ContentEnd);
            string paragraphText = paragraphRange.Text.TrimStart(); // Убираем пробелы

            // Проверяем, начинается ли строка с '# ' или '## '
            if (paragraphText.StartsWith("# "))
            {
                ApplyFormatting(paragraphRange, 36, FontWeights.Bold, true);
            }
            else if (paragraphText.StartsWith("## "))
            {
                ApplyFormatting(paragraphRange, 30, FontWeights.Bold, true);
            }
            else if (paragraphText.StartsWith("### "))
            {
                ApplyFormatting(paragraphRange, 26, FontWeights.Bold, true);
            }
            else if (paragraphText.StartsWith("#### "))
            {
                ApplyFormatting(paragraphRange, 20, FontWeights.Bold, true);
            }
            else if (paragraphText.StartsWith("##### "))
            {
                ApplyFormatting(paragraphRange, 18, FontWeights.Bold, true);
            }
            else if (paragraphText.StartsWith("###### "))
            {
                ApplyFormatting(paragraphRange, 16, FontWeights.Bold, true);
            }
            else
            {
                // Сбрасываем форматирование для обычных строк, но не заголовков
                ResetFormatting(paragraphRange);
            }
        }




        private void RichTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            // Обрабатываем сочетание Ctrl + Enter
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true; // Предотвращаем стандартное поведение, чтобы избежать ошибки

                // Вставляем новую строку после текущего абзаца
                var currentCaret = textContent.CaretPosition;
                var newLine = new Paragraph();
                textContent.Document.Blocks.InsertAfter(currentCaret.Paragraph, newLine);

                // Устанавливаем курсор на новую строку
                textContent.CaretPosition = newLine.ContentStart;
            }
        }

        private void ApplyFormatting(TextRange textRange, double fontSize, FontWeight fontWeight, bool showHeader)
        {
            // Применяем форматирование к заголовкам
            if (showHeader)
            {
                // Устанавливаем меньший размер шрифта для символов '#' или '##'
                textRange.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);

                // Получаем текст для уменьшения размера шрифта для заголовков
                string text = textRange.Text;


                // Уменьшаем размер шрифта для символов '#' или '##'
                if (text.StartsWith("# ") || text.StartsWith("## ") || text.StartsWith("### ") || text.StartsWith("#### ") || text.StartsWith("##### ") || text.StartsWith("###### "))
                {
                    // Текущая позиция курсора
                    var caretPosition = textContent.CaretPosition;
                    var currentBlock = caretPosition.Paragraph;
                    if (currentBlock == null) return;

                    var paragraphRange = new TextRange(currentBlock.ContentStart, currentBlock.ContentEnd);
                    string paragraphText = paragraphRange.Text.TrimStart(); // Убираем пробелы

                    // Считаем количество решеток в начале строки
                    int hashCount = paragraphText.TakeWhile(c => c == '#').Count();

                    // Применяем меньший размер к первым двум символам
                    TextRange hashRange = new TextRange(textRange.Start, textRange.Start.GetPositionAtOffset(hashCount));
                    hashRange.ApplyPropertyValue(TextElement.FontSizeProperty, 11.0); // Маленький шрифт для '##'
                }
            }
            else
            {
                // Скрываем символы '#' или '##'
                string formattedText = textRange.Text.StartsWith("## ") ? textRange.Text.Substring(3) : textRange.Text.Substring(2);
                textRange.Text = formattedText.TrimStart(); // Убираем символы и пробелы
                textRange.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
            }
        }


        private void ResetFormatting(TextRange textRange)
        {
            // Сбрасываем форматирование только для обычных строк
            string text = textRange.Text;
            if (!text.StartsWith("#") && !text.StartsWith("##") &&
                !text.StartsWith("###") && !text.StartsWith("####") &&
                !text.StartsWith("#####") && !text.StartsWith("######"))
            {
                textRange.ApplyPropertyValue(TextElement.FontSizeProperty, 16.0);
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            }
        }


        private void FormatBoldWords(TextRange textRange)
        {
            string text = textRange.Text;
            int startIndex = 0;

            // Удаляем все предыдущие применения форматирования к тексту
            ResetFormatting(textRange);

            while ((startIndex = text.IndexOf("**", startIndex)) != -1)
            {
                int endIndex = text.IndexOf("**", startIndex + 2);
                if (endIndex == -1) break; // Не найдено закрывающей звездочки

                // Получаем текст, который нужно сделать жирным
                // Извлекаем текст между звездочками
                string wordToBold = text.Substring(startIndex + 2, endIndex - startIndex - 2);

                // Создаем новый TextRange для выделения
                TextPointer startPointer = textRange.Start.GetPositionAtOffset(startIndex + 2); // Увеличиваем на 2
                TextPointer endPointer = textRange.Start.GetPositionAtOffset(endIndex); // Оставляем как есть

                // Проверка на корректность указателей
                if (startPointer != null && endPointer != null && startPointer != endPointer)
                {
                    var wordRange = new TextRange(startPointer, endPointer);
                    wordRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }

                startIndex = endIndex + 2; // Продолжаем поиск
            }
        }





        private void FormatUnderlineWords(TextRange textRange)
        {
            string text = textRange.Text;
            int startIndex = 0;

            while ((startIndex = text.IndexOf("__", startIndex)) != -1)
            {
                int endIndex = text.IndexOf("__", startIndex + 2);
                if (endIndex == -1) break; // Не найдено закрывающей подчеркивающей черты

                // Получаем текст, который нужно сделать жирным
                string wordToBold = text.Substring(startIndex + 2, endIndex - startIndex - 2);

                // Создаем новый TextRange для выделения
                var wordRange = new TextRange(textRange.Start.GetPositionAtOffset(startIndex), textRange.Start.GetPositionAtOffset(endIndex + 2));
                wordRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

                startIndex = endIndex + 2; // Продолжаем поиск
            }
        }


    }
}