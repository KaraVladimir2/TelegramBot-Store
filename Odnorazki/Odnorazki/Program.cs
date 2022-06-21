using Telegram.Bot;
using Telegram.Bot.Examples.Polling;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;

DBManipulations.Create();

string BotToken = "5391468920:AAEeilu0DdBdpQu3ExMGZmKzxRrt5ILGmr0";
var bot = new TelegramBotClient(BotToken);

var me = await bot.GetMeAsync();

using var cts = new CancellationTokenSource();

bot.StartReceiving(updateHandler: Handlers.HandleUpdateAsync,
                   errorHandler: Handlers.HandleErrorAsync,
                   receiverOptions: new ReceiverOptions()
                   {
                       AllowedUpdates = Array.Empty<UpdateType>()
                   },
                   cancellationToken: cts.Token);

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();
cts.Cancel();