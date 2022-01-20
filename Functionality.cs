using System.IO;
using Newtonsoft.Json;
using System.Net;


namespace pvcWeatherBot
{
    public class Functionality
    {
        public static string GetWeather(string msgTxt)
        {


            {
                try
                {
                    //Сделать смену языка
                    WeatherAPIToken token = new WeatherAPIToken();

                    string url = $"http://api.openweathermap.org/data/2.5/weather?q={msgTxt}&units=metric&appid={token.token}&lang=ru";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    string response;

                    using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        response = streamReader.ReadToEnd();
                    }

                    return response;
                }
                catch
                {
                    return "Ошибка: Такой город не найден!";
                }
            }
        }
        public static string MessageBuilder(string response)
        {
            WeatherResponse weatherJson = JsonConvert.DeserializeObject<WeatherResponse>(response);

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
        public static string ResponseBuilder(string MsgText)
        {
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
