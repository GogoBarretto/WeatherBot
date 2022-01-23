using System;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
//using Telegram.Bot.Types.ReplyMarkups;

namespace pvcWeatherBot
{
    class Program
    {

        static void Main(string[] args)
        {
            Functionality.GetNews();
            string[] newsLinks = Functionality.news;
            var functionality = new Functionality();
            WeatherBotEventHandler(functionality);
            Console.ReadLine();
        }

        static async void WeatherBotEventHandler(Functionality functionality)
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

                var rkm = new ReplyKeyboardMarkup("");

                rkm.Keyboard = new KeyboardButton[][]
                {
                new KeyboardButton[]
                {  
                    new KeyboardButton("Новости"),
                    new KeyboardButton("Погода")
                }
                
                };
                rkm.ResizeKeyboard = true;

                Console.WriteLine(messageText); // Вывод введённых текстов


                if(messageText != "Погода" && messageText != "Новости") // Вывод погоды по городу
                { 
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"{functionality.ResponseBuilder(messageText)}\n",//Создание ответа на запрос погоды    
                    replyMarkup: rkm,
                    cancellationToken: cancellationToken) ;
                }
                if(messageText == "Погода") // Помощь по погоде
                {
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Чтобы узнать погоду в необходимом городе, просто введите название города (прим. \"Москва\")",
                        cancellationToken: cancellationToken);
                }
                if(messageText == "Новости") // Новости должны выводиться
                {
                    Functionality.GetNews();
                    string[] newsLinks = Functionality.news;
                    
                    for(int i = 0; i < 5; i++) 
                    {
                    Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"{newsLinks[i]}\n",//Переделать
                    replyMarkup: rkm,
                    cancellationToken: cancellationToken);
                    }
                }

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
