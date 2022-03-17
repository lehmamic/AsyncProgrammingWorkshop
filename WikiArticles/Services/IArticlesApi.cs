using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WikiArticles.Models;

namespace WikiArticles.Services;

public interface IArticlesApi
{
    Task AddArticleAsync(Article article);

    Task<IEnumerable<Article>> SearchArticlesAsync(string searchTerm, CancellationToken cancellationToken = default);

    IObservable<IEnumerable<Article>> SearchArticles(string searchTerm);
}