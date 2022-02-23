using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Raven.Embedded;
using WikiArticles.ViewModels;
using WikiArticles.Views;

namespace WikiArticles
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var store = EmbeddedServer.Instance.GetDocumentStore("Wiki");

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(store),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
