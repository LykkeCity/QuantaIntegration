using System;
using System.Net.Http;

namespace AzureRepositories
{
    public static class GeneralSettingsReader
    {
        public static T ReadGeneralSettings<T>(string url)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            var settingsData = httpClient.GetStringAsync("").Result;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(settingsData);
        }
    }
}
