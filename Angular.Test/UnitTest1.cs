using NUnit.Framework;

namespace Tests
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using Newtonsoft.Json;
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
          var  data = GetToken("apiApp");
            var client = new HttpClient();
            var url = "http://localhost:5001/api/Identity";
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + data.access_token.ToString());
            var model=client.GetAsync(url).Result;
            //var model = client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(request1), Encoding.UTF8, "application/json"))
            //                  .Result;
            var resultdata = model.Content.ReadAsStringAsync();
        }

        private static dynamic GetToken(string scope)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("client_id", "mvc");
            dict.Add("client_secret", "secret");
            dict.Add("scope", scope );
            dict.Add("grant_type", "client_credentials");
            var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/connect/token") { Content = new FormUrlEncodedContent(dict) };
            var res = client.SendAsync(req).Result;
            var data = JsonConvert.DeserializeObject<dynamic>(res.Content.ReadAsStringAsync().Result);
            return data;
        }
    }
}