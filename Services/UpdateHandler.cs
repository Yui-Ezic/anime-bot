using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace AnimeBot.Services
{

    public class UpdateHandler
    {
        static int Yes = 0;
        static int t;
        static int ID = 0;
        static long[] IDMem = new long[4] { 0, 0, 0, 0 };

        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandler> _logger;

        public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(_botClient, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(_botClient, update.CallbackQuery);
                return;
            }

            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

        }

        public static async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(botClient, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery);
                return;
            }

            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

        }
        static async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (ID < 1)
            {
                if (message.Text == "/Аниме")
                {

                    InlineKeyboardMarkup YesNo = new InlineKeyboardMarkup(new[]
                {
                new[]
                {
                InlineKeyboardButton.WithCallbackData ("Да", "Да"),
                InlineKeyboardButton.WithCallbackData ("Нет", "Нет")
                }
            });
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Смотрим аниме?", replyMarkup: YesNo);
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{Yes}/4");
                    ID++;
                    return;
                }
            }
        }
        static async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            if (t == 0)
            {
                if (callbackQuery.Data != "Да") t = callbackQuery.Message.MessageId + 1;
            }

            if (callbackQuery.From.Id == IDMem[0] | callbackQuery.From.Id == IDMem[1] | callbackQuery.From.Id == IDMem[2] | callbackQuery.From.Id == IDMem[3]) ;

            else
            {
                if (callbackQuery.Data == "Нет")
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Не собираемся");
                    ID = 0;
                    IDMem = new long[4] { 0, 0, 0, 0 }; ;
                    Yes = 0;
                    if (callbackQuery.Message.Text == "Смотрим аниме?") await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                }

                if (callbackQuery.Data == "Да")
                {
                    if (Yes >= 4)
                    {
                        Console.WriteLine();
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Cобираемся автоботы");
                        ID = 0;
                        IDMem = new long[4] { 0, 0, 0, 0 };
                        Yes = 0;
                        if (callbackQuery.Message.Text == "Смотрим аниме?") await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "@xomya4ok @Kirinamie @Yui_ezic  @Hilligan");
                    }
                    IDMem[Yes] = callbackQuery.From.Id;
                    Yes++;
                    if (t == 0) t = callbackQuery.Message.MessageId + 1;
                    await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId = t, $"{Yes}/4");
                }

                ID++;
            }
            return;
        }
    }
}
