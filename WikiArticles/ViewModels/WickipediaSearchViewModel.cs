using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using WikiArticles.Models;
using WikiArticles.Services;

namespace WikiArticles.ViewModels;

public class WikipediaSearchViewModel : ViewModelBase
{
     private readonly WikiService _service;
     private string _searchTerm = string.Empty;
     private string _articleName = string.Empty;

     public WikipediaSearchViewModel(WikiService service)
     {
          _service = service;

          _service.SearchResultChanged += OnSearchResultChanged;
     }

     public string ArticleName
     {
          get => _articleName;
          set => this.RaiseAndSetIfChanged(ref _articleName, value);
     }

     public string SearchTerm
     {
          get => _searchTerm;
          private set
          {
               this.RaiseAndSetIfChanged(ref _searchTerm, value);

               _ = _service.SearchAsync(_searchTerm);
          }
     }

     public ObservableCollection<Article> Articles { get; } = new();

     public void AddArticle()
     {
          _ = _service.AddArticle(new Article { Title = ArticleName });
          ArticleName = string.Empty;
     }

     private async void OnSearchResultChanged(object? sender, IEnumerable<Article> e)
     {
          await SetSearchResultsAsync(e);
     }

     private async Task SetSearchResultsAsync(IEnumerable<Article> articles)
     {
          await Dispatcher.UIThread.InvokeAsync(() =>
          {
               Articles.Clear();
               Articles.AddRange(articles);
          });
     }
}
