using System.Windows;
using Microsoft.Extensions.Logging;

namespace Cartridge.UI
{
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            LogTextBox.Clear();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LogTextBox.Text);
        }

        public void AddLogMessage(string message)
        {
            // Ensure we're on the UI thread
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => AddLogMessage(message));
                return;
            }

            LogTextBox.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}{Environment.NewLine}");
            LogTextBox.ScrollToEnd();
        }
    }

    // Custom logger that writes to our debug window
    public class DebugWindowLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly DebugWindow _debugWindow;

        public DebugWindowLogger(string categoryName, DebugWindow debugWindow)
        {
            _categoryName = categoryName;
            _debugWindow = debugWindow;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var message = $"[{logLevel}] {_categoryName}: {formatter(state, exception)}";
            if (exception != null)
            {
                message += $"{Environment.NewLine}Exception: {exception}";
            }
            _debugWindow.AddLogMessage(message);
        }
    }

    // Logger provider that creates our custom loggers
    public class DebugWindowLoggerProvider : ILoggerProvider
    {
        private readonly DebugWindow _debugWindow;

        public DebugWindowLoggerProvider(DebugWindow debugWindow)
        {
            _debugWindow = debugWindow;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DebugWindowLogger(categoryName, _debugWindow);
        }

        public void Dispose() { }
    }
}