using System;
using Telegram.Bot;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Threading;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;

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
            foreach(var item in weatherJson.weather)
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
            if (responseByCity != "Ошибка: Такой город не найден!") {
                resultMessage = MessageBuilder(responseByCity);
            }
            else
            {
                resultMessage = responseByCity;
            }
            return resultMessage;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            WeatherBotEventHandler();
            Console.ReadLine();
        }

        static async void WeatherBotEventHandler()
        {
            TelegramBotToken token = new TelegramBotToken();// Токен тут
            var client = new TelegramBotClient(token.token);
            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };
            
            client.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            var me = await client.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                if (update.Type != UpdateType.Message)
                    return;
                if (update.Message!.Type != MessageType.Text)
                    return;

                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;

                

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text:  $"{Functionality.ResponseBuilder(messageText)}\n",//Создание ответа на запрос погоды 
                    
                    cancellationToken: cancellationToken);
            }
            
            Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }
        }
    }
}
