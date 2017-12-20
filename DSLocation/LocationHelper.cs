﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DSLocation
{
    public class LocationHelper
    {

        public async Task<List<User>> getLocationInfosAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Common.SERVER_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    HttpResponseMessage response = await client.GetAsync("targets");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var locationInfos = JsonConvert.DeserializeObject<RootObject>(responseContent);
                    return locationInfos.users;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return null;
                }
            }
        }

        
    }
}
