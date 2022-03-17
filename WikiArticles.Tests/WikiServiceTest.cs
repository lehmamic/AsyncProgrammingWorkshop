using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using MarbleTest.Net;
using Moq;
using Moq.AutoMock;
using WikiArticles.Models;
using WikiArticles.Services;
using WikiArticles.Utils;
using Xunit;

namespace WikiArticles.Tests;

public class WikiServiceTest
{
    private readonly AutoMocker _mocker = new();
    private readonly MarbleScheduler _scheduler = new();
    
    [Fact]
    public void Test1()
    {
        // arrange
        _mocker.GetMock<ISchedulerProvider>()
            .Setup(s => s.CreateTime(It.IsAny<TimeSpan>()))
            .Returns(() => _scheduler.CreateTime("---|"));

        _mocker.GetMock<ISchedulerProvider>()
            .SetupGet(s => s.Scheduler)
            .Returns(() => _scheduler);

        var resultingArticles = new[] { new Article {Id = 1, Title = "Software design pattern"}};
        _mocker.GetMock<IArticlesApi>()
            .Setup(s => s.SearchArticles(It.IsAny<string>()))
            .Returns(()=> _scheduler.CreateColdObservable<IEnumerable<Article>>("--a|", new { a = resultingArticles}));

        var service = _mocker.CreateInstance<WikiService>();

        // act
        var searchTermStream = _scheduler.CreateHotObservable<string>(
            "--abc-",
            new
            {
                a = "t",
                b = "te",
                c = "tes",
            });

        var actual = service.Search(searchTermStream);

        // assert
        
        // --(a skipped)(b skipped)(c) --- (throttled) ---(result) 
        _scheduler.ExpectObservable(actual)
            .ToBe("---------a", new { a = resultingArticles });
    }
}