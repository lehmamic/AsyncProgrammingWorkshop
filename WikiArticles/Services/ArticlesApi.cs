using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WikiArticles.Models;

namespace WikiArticles.Services;

public class ArticlesApi : IArticlesApi
{
    private readonly DatabaseContext _context = CreateDatabaseContext();

    public async Task AddArticleAsync(Article article)
    {
        await _context.Articles.AddAsync(article);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<Article>> SearchArticlesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var trimmedTerm = string.IsNullOrEmpty(searchTerm) ? string.Empty : searchTerm.Trim();

        // the behavior of the inmemory db string comparison is weird "te" matches with "tante", but it does not matter for the workshop
        return await _context.Articles
            .Where(a => trimmedTerm != "" && a.Title.Contains(trimmedTerm, StringComparison.CurrentCultureIgnoreCase))
            .ToListAsync(cancellationToken);
    }

    public IObservable<IEnumerable<Article>> SearchArticles(string searchTerm)
    {
        return Observable.Defer(() => SearchArticlesAsync(searchTerm).ToObservable());
    }

    private static DatabaseContext CreateDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "WikiArticles")
            .Options;

        return new DatabaseContext(options);
    }
}