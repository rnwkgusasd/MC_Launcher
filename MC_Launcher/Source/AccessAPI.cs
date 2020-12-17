using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MC_Launcher.Source
{
    public class AccessAPI
    {
        private string name = "";
        private string id = "";

        public async void GetUUID(string username)
        {
            string url = $"https://api.mojang.com/users/profiles/minecraft/{username}?at=0";

            HttpClient client = new HttpClient();

            string response = await client.GetStringAsync(url);

            var data = JsonConvert.DeserializeObject<UUID_OBJ>(response);

            name = data.name;
            id = data.id;
        }

        public class UUID_OBJ
        {
            public string name { get; set; }
            public string id { get; set; }
        }
    }
}
