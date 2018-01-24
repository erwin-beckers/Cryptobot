using System.Configuration;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CryptoBotApp.bot
{
    public static class TelegramBot
    {
        public static void Send(string message)
        {
            var botClient = new TelegramBotClient(ConfigurationManager.AppSettings["telegramapi"]);
            var task = Task.Run(async () =>
            {
                await botClient.SendTextMessageAsync(ConfigurationManager.AppSettings["telegramchatid"], message);
            });
            task.Wait();
        }
    }
}