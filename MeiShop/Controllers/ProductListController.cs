using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace MeiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductListController : ControllerBase
    {

        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7034/'");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("api/ProductList");


            string content = "";

            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();


                var itemList = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic[]>(content);

                // Speichern der Beschreibungen in einem string[]
                string[] descriptions = new string[itemList.Length];
                for (int i = 0; i < itemList.Length; i++)
                {
                    descriptions[i] = itemList[i].description;
                }

                return descriptions;

            }

            return new string[] { };
           
        }
    }
}
