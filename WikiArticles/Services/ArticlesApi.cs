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
        return await _context.Articles
            .Where(a =>  a.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase))
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