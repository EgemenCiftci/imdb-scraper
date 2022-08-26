using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using ImdbScraper;
using System.Threading.Tasks;

namespace ImdbScraperClient
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            Data = new();
        }

        public Data Data
        {
            get { return GetProperty(() => Data); }
            set { SetProperty(() => Data, value); }
        }

        [Command]
        public async Task GetDataAsync()
        {
            if (Data != null)
            {
                if (string.IsNullOrWhiteSpace(Data.Id) && !string.IsNullOrWhiteSpace(Data.Name))
                {
                    Data.Id = await Scraper.GetIdFromNameAsync(Data.Name);
                }

                await Scraper.GetDataFromImdbAsync(Data);
            }
        }
    }
}
