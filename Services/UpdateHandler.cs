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
using StackExchange.Redis;

namespace AnimeBot.Services
{

    public class UpdateHandler
    {
        static int ID_Edit_Message;

        static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("176.221.1.219:6379");
        static IDatabase db = redis.GetDatabase();

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
            if (Convert.ToInt32(db.StringGet("Using_Anime")) < 1)
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
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{Convert.ToInt32(db.StringGet("Using_Anime"))}/4");
                    db.StringSet("Using_Anime", 1);
                    return;
                }
            }

            if (message.Text == "/Сброс")
            {
                db.StringSet("Using_Anime", 0);
                db.StringSet("ID_member0", 0);
                db.StringSet("ID_member1", 0);
                db.StringSet("ID_member2", 0);
                db.StringSet("ID_member3", 0);
                db.StringSet("Using_Anime", 0);
            }

        }
        static async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            if (ID_Edit_Message == 0)
            {
                if (callbackQuery.Data != "Да") ID_Edit_Message = callbackQuery.Message.MessageId + 1;
            }

            if (callbackQuery.From.Id == db.StringGet("ID_member0") | callbackQuery.From.Id == db.StringGet("ID_member1") | callbackQuery.From.Id == db.StringGet("ID_member2") | callbackQuery.From.Id == db.StringGet("ID_member3")) ;

            else
            {
                if (callbackQuery.Data == "Нет")
                {
                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Не собираемся");
                    db.StringSet("Using_Anime", 0);
                    db.StringSet("ID_member0", 0);
                    db.StringSet("ID_member1", 0);
                    db.StringSet("ID_member2", 0);
                    db.StringSet("ID_member3", 0);
                    db.StringSet("Using_Anime", 0);
                    if (callbackQuery.Message.Text == "Смотрим аниме?") await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                }

                if (callbackQuery.Data == "Да")
                {
                    if (Convert.ToInt32(db.StringGet("Using_Anime")) - 1 >= 4)
                    {
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Cобираемся автоботы");
                        db.StringSet("Using_Anime", 0);
                        db.StringSet("ID_member0", 0);
                        db.StringSet("ID_member1", 0);
                        db.StringSet("ID_member2", 0);
                        db.StringSet("ID_member3", 0);
                        if (callbackQuery.Message.Text == "Смотрим аниме?") await botClient.DeleteMessageAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId);
                        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "@xomya4ok @Kirinamie @Yui_ezic @Hilligan");
                    }
                    if (db.StringGet("Using_Anime") == 1) db.StringSet("ID_member0", callbackQuery.From.Id);
                    if (db.StringGet("Using_Anime") == 2) db.StringSet("ID_member1", callbackQuery.From.Id);
                    if (db.StringGet("Using_Anime") == 3) db.StringSet("ID_member2", callbackQuery.From.Id);
                    if (db.StringGet("Using_Anime") == 4) db.StringSet("ID_member3", callbackQuery.From.Id);
                    db.StringSet("Using_Anime", Convert.ToInt32(db.StringGet("Using_Anime")) + 1);
                    if (ID_Edit_Message == 0) ID_Edit_Message = callbackQuery.Message.MessageId + 1;
                    await botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId = ID_Edit_Message, $"{Convert.ToInt32(db.StringGet("Using_Anime")) - 1}/ 4");
                }
            }
            return;
        }
    }
}
