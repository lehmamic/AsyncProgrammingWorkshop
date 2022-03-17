using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using WikiArticles.ViewModels;

namespace WikiArticles.Views
{
    public partial class WikipediaSearchView : ReactiveUserControl<WikipediaSearchViewModel>
    {
        public WikipediaSearchView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
