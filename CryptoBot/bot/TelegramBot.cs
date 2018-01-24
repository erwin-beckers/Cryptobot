using System;
using System.Configuration;
using Telegram.Bot;

namespace CryptoBotApp.bot
{
    public static class TelegramBot
    {
        public async Task<void> SendAsync(string message)
        {
            var botClient = new TelegramBotClient(ConfigurationManager.AppSettings["telegramapi"]);
            await botClient.SendTextMessageAsync(ConfigurationManager.AppSettings["telegramchatid"], message);
        }
    }
}
