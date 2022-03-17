using WikiArticles.Services;
using WikiArticles.Utils;

namespace WikiArticles.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            var wikiService = new WikiService(new ArticlesApi(), new SchedulerProvider());

            WikipediaSearch = new(wikiService);
        }

        public WikipediaSearchViewModel WikipediaSearch { get; }
    }
}
