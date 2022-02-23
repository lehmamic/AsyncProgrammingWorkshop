using Raven.Client.Documents;
using WikiArticles.Services;

namespace WikiArticles.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IDocumentStore store)
        {
            WikipediaSearch = new(new WikiService(store));
        }

        public WikipediaSearchViewModel WikipediaSearch { get; }
    }
}
