using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using ReactiveUI;
using WikiArticles.Models;
using WikiArticles.Services;

namespace WikiArticles.ViewModels
{
     public class WikipediaSearchViewModel : ViewModelBase
     {
          private readonly WikiService _service;
          private readonly Subject<string> _searchTermStream = new Subject<string>();

          private string _searchTerm = string.Empty;
          private string _articleName = string.Empty;

          public WikipediaSearchViewModel(WikiService service)
          {
               _service = service;

               /*
               _service.SearchResultChanged += OnSearchResultChanged;
               */

               this.WhenAnyValue(x => x.SearchTerm)
                    .Subscribe( term => _searchTermStream.OnNext(term));

               _service.Search(_searchTermStream)
                    .ObserveOn(RxApp.MainThreadScheduler) // make sure we handle it in the UI thread
                    .Subscribe(articles =>
                    {
                         Articles.Clear();
                         Articles.AddRange(articles);
                    });

               // We could do it directly without a Subject
               /*
               var termStream = this.WhenAnyValue(x => x.SearchTerm);
               _service.Search(termStream)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(articles =>
                    {
                         Articles.Clear();
                         Articles.AddRange(articles);
                    });
               */
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
               }
          }

          public ObservableCollection<Article> Articles { get; } = new();

          public void AddArticle()
          {
               _ = _service.AddArticle(new Article { Title = ArticleName });
               ArticleName = string.Empty;
          }

          /*
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
          */
     }
}
