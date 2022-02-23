using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using WikiArticles.ViewModels;

namespace WikiArticles.Views;

public partial class WikipediaSearchView : UserControl
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
