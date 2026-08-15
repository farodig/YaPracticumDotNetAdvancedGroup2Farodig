using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace IntegrationTests.Helpers
{
    [CollectionDefinition("SequentialTests", DisableParallelization = true)]
    public abstract class AHttpClient : IClassFixture<IntegrationTestFactory>
    {
        private readonly HttpClient _client;

        public AHttpClient(IntegrationTestFactory factory)
        {
            _client = factory.CreateClient();
            factory.ClearDatabase();
        }

        /// <summary>
        /// Get запрос
        /// </summary>
        protected async Task<Tout?> GetAsync<Tout>(string path, string? token = null)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await _client.GetAsync(path);

            response.EnsureSuccessStatusCode(); // Проверяет, что статус 200-299

            return await ReceiveDeserialized<Tout>(response);
        }

        /// <summary>
        /// Post запрос
        /// </summary>
        protected async Task<Tout?> PostAsync<Tout>(string path, object? requestData = null, string? token = null)
        {
            var content = SerializeContent(requestData);
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }


            using var response = await _client.PostAsync(path, content);

            response.EnsureSuccessStatusCode(); // Проверяет, что статус 200-299

            return await ReceiveDeserialized<Tout>(response);
        }

        /// <summary>
        /// Сериализация данных на отправку
        /// </summary>
        public static StringContent SerializeContent(object? requestData = null)
        {
            if (requestData is null)
            {
                return new StringContent(string.Empty, Encoding.UTF8, "application/json");
            }
            else
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore // Здесь задаём поведение
                };

                var json = JsonConvert.SerializeObject(requestData, Formatting.Indented, settings);
                return new StringContent(json, Encoding.UTF8, "application/json");
            }
        }

        /// <summary>
        /// Десериализация данных на приём
        /// </summary>
        public static async Task<Tout?> ReceiveDeserialized<Tout>(HttpResponseMessage response)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);

            var serializer = new JsonSerializer();
            var result = serializer.Deserialize<Tout>(jsonReader);

            return result;
        }
    }
}
