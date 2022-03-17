using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WikiArticles.Models;
using WikiArticles.Utils;
using Timer = System.Threading.Timer;

namespace WikiArticles.Services
{
    public class WikiService
    {
        private readonly IArticlesApi _api;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly object _lockObject = new ();

        private CancellationTokenSource _searchCancellationTokenSource = new();

        public EventHandler<IEnumerable<Article>>? SearchResultChanged;

        public WikiService(IArticlesApi api, ISchedulerProvider schedulerProvider)
        {
            _api = api;
            _schedulerProvider = schedulerProvider;
        }

        public async Task AddArticle(Article article)
        {
            await _api.AddArticleAsync(article);
        }

        public async Task SearchAsync(string searchTerm)
        {
            var cancellationTokenSource = CancelPendingSearchRequests();

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);

            await SearchAndPollForNewArticles(searchTerm, cancellationTokenSource.Token);
        }

        public IObservable<IEnumerable<Article>> Search(IObservable<string> searchTermStream)
        {
            return searchTermStream
                .Throttle(_schedulerProvider.CreateTime(TimeSpan.FromMilliseconds(400)), _schedulerProvider.Scheduler)
                .Select(term => Observable.Interval(_schedulerProvider.CreateTime(TimeSpan.FromMilliseconds(1000)), _schedulerProvider.Scheduler)
                    .Select(_ => _api.SearchArticles(term))
                    .Switch())
                .Switch();
        }

        private CancellationTokenSource CancelPendingSearchRequests()
        {
            lock (_lockObject)
            {
                _searchCancellationTokenSource.Cancel();
                _searchCancellationTokenSource.Dispose();

                _searchCancellationTokenSource = new();

                return _searchCancellationTokenSource;
            }
        }

        private async Task SearchAndPollForNewArticles(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                SearchResultChanged?.Invoke(this, Array.Empty<Article>());
                return;
            }

            var t = new Timer(
                async _ =>
                {
                    var articles = await _api.SearchArticlesAsync(searchTerm, cancellationToken);
                    SearchResultChanged?.Invoke(this, articles);
                },
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

            cancellationToken.Register(() =>
            {
                t.Dispose();
            });
        }
    }
}
