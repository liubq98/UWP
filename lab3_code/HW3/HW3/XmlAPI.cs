using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace HW3
{
    public class XmlAPI
    {
        public async static Task<string> GetArea(string idNum)
        {
            var http = new HttpClient();
            var res = await http.GetAsync("http://api.k780.com/?app=idcard.get&idcard=" + idNum + "&appkey=33173&sign=04957c37d1a768804a833a4b59f20c0b&format=xml");
            var result = await res.Content.ReadAsStringAsync();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);

            var data = doc.DocumentElement;
            
            var str = data.GetElementsByTagName("style_simcall");
            var te = str[0].ChildNodes[0].Value;

            return te;
        }
    }

    [XmlRoot(ElementName = "Result")]
    public class Result
    {
        public string status { get; set; }
        public string par { get; set; }
        public string idcard { get; set; }
        public string born { get; set; }
        [XmlElement(ElementName = "sex")]
        public string sex { get; set; }
        [XmlElement(ElementName = "att")]
        public string att { get; set; }
        public string postno { get; set; }
        public string areano { get; set; }
        public string style_simcall { get; set; }
        public string style_citynm { get; set; }
    }

    [XmlRoot(ElementName = "Root")]
    public class Root
    {
        [XmlElement(ElementName = "success")]
        public string success { get; set; }
        [XmlElement(ElementName = "result")]
        public Result result { get; set; }
    }

    [XmlRoot(ElementName = "RootObject2")]
    public class RootObject2
    {
        [XmlElement(ElementName = "root")]
        public Root root { get; set; }
    }
}
