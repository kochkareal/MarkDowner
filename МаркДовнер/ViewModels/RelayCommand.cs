// ViewModels/RelayCommand.cs
using System;
using System.Windows.Input;

namespace МаркДовнер.ViewModels
{
    /// <summary>
    /// Реализация команды, которая позволяет связывать действия с командами в MVVM.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute; // Делегат для выполнения команды
        private readonly Predicate<object?>? _canExecute; // Делегат для проверки, может ли команда быть выполнена

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="execute">Метод для выполнения команды.</param>
        /// <param name="canExecute">Метод для проверки, может ли команда быть выполнена. Необязательно.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если параметр <paramref name="execute"/> равен null.</exception>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Определяет, может ли команда быть выполнена.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        /// <returns>true, если команда может быть выполнена; иначе, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        /// <param name="parameter">Параметр команды.</param>
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Событие, которое вызывается при изменении состояния выполнения команды.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value; // Подписка на изменение состояния выполнения команды
            remove => CommandManager.RequerySuggested -= value; // Отписка от изменения состояния выполнения команды
        }
    }
}
