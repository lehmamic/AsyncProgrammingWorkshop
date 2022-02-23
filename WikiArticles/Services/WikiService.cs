using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using WikiArticles.Models;
using Timer = System.Threading.Timer;

namespace WikiArticles.Services;

public class WikiService
{
    private readonly IDocumentStore _store;
    private readonly object _lockObject = new ();

    private CancellationTokenSource _searchCancellationTokenSource = new();

    public EventHandler<IEnumerable<Article>>? SearchResultChanged;

    public WikiService(IDocumentStore store)
    {
        _store = store;
    }

    public async Task AddArticle(Article article)
    {
        using var session = _store.OpenAsyncSession();
        await session.StoreAsync(article);
        await session.SaveChangesAsync();
    }

    public async Task SearchAsync(string searchTerm)
    {
        var cancellationTokenSource = CancelPendingSearchRequests();

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);

        await SearchAndPollForNewArticles(searchTerm, cancellationTokenSource.Token);
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
        var t = new Timer(
            async _ =>
            {
                var articles = await SearchArticleAsync(searchTerm, cancellationToken);
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

    private async Task<IEnumerable<Article>> SearchArticleAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        using var session = _store.OpenAsyncSession();
        return await session.Query<Article>()
            .Search(a => a.Title, string.IsNullOrEmpty(searchTerm) ? searchTerm : $"*{searchTerm}*")
            .ToListAsync(cancellationToken);
    }
}
