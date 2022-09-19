using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ConsoleSearch
{
    public class App
    {
        public void Run()
        {
            Console.WriteLine("Console Search");
            
            while (true)
            {
                Console.WriteLine("enter search terms - q for quit");
                string input = Console.ReadLine() ?? string.Empty;
                if (input.Equals("q")) break;

                var wordIds = new List<int>();
                var searchTerms = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach (var t in searchTerms)
                {
                    //int id = mSearchLogic.GetIdOf(t);
                    using (HttpClient client = new HttpClient())
                    {
                        var response = client.GetAsync("https://localhost:49173/WeatherForecast/GetIdOf?terms=" + t);
                        if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var result = response.Result.Content.ReadAsStringAsync().Result; 
                            int id = int.Parse(result);
                            if (id != -1)
                            {
                                wordIds.Add(id);
                            }
                            else
                            {
                                Console.WriteLine(t + " will be ignored");
                            }
                        }
                        else
                        {
                            var error = response;
                        }
                    }
                }

                DateTime start = DateTime.Now;

                var docIds = new List<KeyValuePair<int, int>>();
                using (HttpClient client = new HttpClient())
                {
                    var response = client.PostAsync("https://localhost:49173/WeatherForecast/GetDocuments", new StringContent(JsonConvert.SerializeObject(wordIds), System.Text.Encoding.UTF8, "application/json"));
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        docIds = JsonConvert.DeserializeObject<List<KeyValuePair<int, int>>>(response.Result.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        var error = response;
                    }
                }
                //var docIds = mSearchLogic.GetDocuments(wordIds);
                // get details for the first 10             
                var top10 = new List<int>();
                foreach (var p in docIds.GetRange(0, Math.Min(10, docIds.Count)))
                {
                    top10.Add(p.Key);
                }

                TimeSpan used = DateTime.Now - start;

                int idx = 0;
                //foreach (var doc in mSearchLogic.GetDocumentDetails(top10))
                var documentDetails = new List<string>();
                using (HttpClient client = new HttpClient())
                {
                    var response = client.PostAsync("https://localhost:49173/WeatherForecast/GetDocumentDetails", new StringContent(JsonConvert.SerializeObject(top10), System.Text.Encoding.UTF8, "application/json"));
                    if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        documentDetails = JsonConvert.DeserializeObject<List<string>>(response.Result.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        var error = response;
                    }
                }
                foreach (var doc in documentDetails)
                {
                    Console.WriteLine("" + (idx+1) + ": " + doc + " -- contains " + docIds[idx].Value + " search terms");
                    idx++;
                }
                Console.WriteLine("Documents: " + docIds.Count + ". Time: " + used.TotalMilliseconds);
            }
        }
    }
}
