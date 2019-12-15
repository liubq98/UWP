using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

namespace HW3
{
    public class OpenWeatherMapProxy
    {
        public async static Task<RootObject> GetWeather(string location)
        {
            var http = new HttpClient();
            var res = await http.GetAsync("https://api.seniverse.com/v3/weather/now.json?key=husggnnzystx2h4e&location=" + location + "&language=zh-Hans&unit=c");
            var result = await res.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootObject)serializer.ReadObject(ms);

            return data;
        }
    }

    [DataContract]
    public class Location
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string country { get; set; }

        [DataMember]
        public string path { get; set; }

        [DataMember]
        public string timezone { get; set; }

        [DataMember]
        public string timezone_offset { get; set; }
    }

    [DataContract]
    public class Now
    {
        [DataMember]
        public string text { get; set; }

        [DataMember]
        public string code { get; set; }

        [DataMember]
        public string temperature { get; set; }
    }

    [DataContract]
    public class Results
    {
        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public Now now { get; set; }

        [DataMember]
        public string last_update { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public List<Results> results { get; set; }
    }
}
