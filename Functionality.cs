using System.IO;
using Newtonsoft.Json;
using System.Net;
using System;
using System.Net.Http;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace pvcWeatherBot
{
    public class Functionality
    {
        public static string[] news = new string[5];
        public static void GetNews()
        {
            
            var html = @"https://yandex.ru/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);
            
            int i = 0;
            IEnumerable<HtmlNode> nodes = htmlDoc.DocumentNode.Descendants().Where(n => n.HasClass("list__item"));// Иногда выдаёт ошибку (ето яндекс виноват)
            foreach (var item in nodes)
            {
                var a = item.Descendants("a").FirstOrDefault();
                string b ="";
                try {
                    b = a.GetAttributeValue("href", null);
                }
                catch { }
                try {
                    news[i] = b;
                    //Console.WriteLine(news[i]);
                    i++;
                    if (i == 5)
                        break;

                }
                catch { }
                
                
            }
        }
        private string GetWeather(string msgTxt)
        {
                try
                {
                    WeatherAPIToken token = new WeatherAPIToken();
                    
                    string url = $"http://api.openweathermap.org/data/2.5/weather?q={msgTxt}&units=metric&appid={token.token}&lang=ru";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    using HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    
                    string response;

                    using StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                    response = streamReader.ReadToEnd();

                     
                    return response;
                }
                catch
                {
                    return "Ошибка: Такой город не найден!";
                }
        }
        private string MessageBuilder(string response)
        {
            var weatherJson = JsonConvert.DeserializeObject<WeatherResponse>(response);

            string description = "";
            foreach (var item in weatherJson.weather)
            {
                description = item.description;
            }

            string weatherMessage =
            "Город: " + weatherJson.Name +
            "\nОсадки: " + description +
            "\nТемпература: " + weatherJson.Main.Temp + " °C" +
            "\nЧувствуется как: " + weatherJson.Main.Feels_like + " °C" +
            "\nДавление: " + weatherJson.Main.Pressure + " мм рт.ст." +
            "\nВлажность: " + weatherJson.Main.Humidity + " %" +
            "\nСкорость ветра: " + weatherJson.Wind.Speed + " м/c";

            return weatherMessage;
        }

        public string ResponseBuilder(string MsgText)
        {
            if(MsgText == "Новости")
            {
                GetNews();
                return news[5];
            }
            string responseByCity = GetWeather(MsgText);
            string resultMessage;
            if (responseByCity != "Ошибка: Такой город не найден!")
            {
                resultMessage = MessageBuilder(responseByCity);
            }
            else
            {
                resultMessage = responseByCity;
            }
            return resultMessage;
        }
    }
}
