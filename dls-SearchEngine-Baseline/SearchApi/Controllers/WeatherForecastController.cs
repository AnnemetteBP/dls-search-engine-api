using Microsoft.AspNetCore.Mvc;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static SearchLogic mSearchLogic = new SearchLogic(new Database());

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetIdOf")]
        public async Task<int> GetIdOf(string terms)
        {
            return await Task<int>.Run(() => {
                return WeatherForecastController.mSearchLogic.GetIdOf(terms);
            });
        }

        [HttpPost(Name = "GetDocuments")]
        public async Task<List<KeyValuePair<int, int>>> GetDocuments(List<int> wordIds)
        {
            return await Task<List<KeyValuePair<int, int>>>.Run(() => {
                return WeatherForecastController.mSearchLogic.GetDocuments(wordIds);
            });
        }

        [HttpPost(Name = "GetDocumentDetails")]
        public async Task<List<string>> GetDocumentDetails(List<int> docIds)
        {
            return await Task<List<string>>.Run(() => {
                return WeatherForecastController.mSearchLogic.GetDocumentDetails(docIds);
            });
        }

        [HttpGet(Name = "Search")]
        public async Task<List<string>> Search(string terms)
        {
            return await Task<int>.Run(() => {
                int id = WeatherForecastController.mSearchLogic.GetIdOf(terms);
                var list = WeatherForecastController.mSearchLogic.GetDocuments(new List<int>() { id });
                var newList = new List<int>();
                foreach (var item in list)
                {
                    if(item.Value == 1)
                    {
                        newList.Add(item.Key);
                    }
                }
                var result = WeatherForecastController.mSearchLogic.GetDocumentDetails(newList);
                return result.GetRange(0, result.Count >= 10 ? 10 : result.Count);
            });
        }
    }
}