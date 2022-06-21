using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Examples.Polling;
public class Handlers
{
    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {

        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
            UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
            UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
            UpdateType.PreCheckoutQuery => answerPreCheckoutQuery(botClient, update.PreCheckoutQuery.Id.ToString()!),
            _ => UnknownUpdateHandlerAsync(botClient, update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(botClient, exception, cancellationToken);
        }
    }

    private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
    {
        Console.WriteLine($"Message type: {message.Type}\nMessage text: {message.Text}\nChat id: {message.Chat.Id}"); //1030464002
        if (message.Type != MessageType.Text)
            return;
        if (User.stage == 1)
        {
            if (message.Text == "/start") await SendReplyKeyboard(botClient, message);
            else if (message.Text == "/remove") await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Кнопки убраны", parseMode: null,
                                                    replyMarkup: new ReplyKeyboardRemove());
            else if (message.Text == "/help")  await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "/start - Главное меню\n/help - Помощь\n/remove - Удалить кнопки",
                                                    replyMarkup: null);
            else if (message.Text == "Назад") await SendReplyKeyboard(botClient, message);
            else if (message.Text == "ElfBar")
            {
                await Products.ElfBar(botClient, message);
            }
        }
        
        else if (User.stage == 2)
        {
            await BotOnOrderReceived(botClient, message);
        }
    }

    static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message)
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(
            new[]
            {
                        new KeyboardButton[] { "ElfBar", "Joyetech", "HQD" },
                        new KeyboardButton[] { "LostMary", "OctoLab", "ZeeZee" }

            })
        {
            ResizeKeyboard = true
        };

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Выберите товар",
                                                    replyMarkup: replyKeyboardMarkup);
    }

    private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        User.Product = callbackQuery.Data;
        User.stage = 2;
        await BotOnOrderReceived(botClient, callbackQuery.Message);
    }

    private static async Task BotOnOrderReceived(ITelegramBotClient botClient, Message message)
    {

        switch (User.ministage)
        {
            case 1:
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Введите количество товара для покупки",
                                                        replyMarkup: new ReplyKeyboardRemove());
                User.ministage++;
                break;
            case 2:
                User.Amount = int.Parse(message.Text);
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Введите ФИО получателя",
                                                        replyMarkup: null);
                User.ministage++;
                break;
            case 3:
                User.Name = message.Text;
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Введите город отделения почты",
                                                        replyMarkup: null);
                User.ministage++;
                break;
            case 4:
                User.Town = message.Text;
                ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "Нова Пошта", "Укр Пошта" },

                })
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Выберите способ доставки",
                                                            replyMarkup: replyKeyboardMarkup);
                User.ministage++;
                break;
            case 5:
                User.Delivery = message.Text;
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Введите номер отделения почты",
                                                        replyMarkup: new ReplyKeyboardRemove());
                User.ministage++;
                break;
            case 6:
                User.DeliveryAdress = message.Text;
                User.ministage = 1;
                User.stage = 1;
                Console.WriteLine(User.Amount+"\n"+User.Product+"\n"+User.Name+"\n"+User.Town+"\n"+User.Delivery+"\n"+User.DeliveryAdress);
                await Payment(botClient, message);
                break;
            default:
                break;
        }

    }

    private static async Task Payment(ITelegramBotClient botClient, Message message)
    {
        List<Telegram.Bot.Types.Payments.LabeledPrice> itemPrice = new List<Telegram.Bot.Types.Payments.LabeledPrice>();
        itemPrice.Add(new Telegram.Bot.Types.Payments.LabeledPrice("Test", 10000));
        await botClient.SendInvoiceAsync(chatId: message.Chat.Id,
                                  title: "Оплата",
                                  description: "Оплатите товар, и ждите ответа от администратора",
                                  payload: "Portmone Test",
                                  providerToken: "284685063:TEST:ZTUzNWE4NDVkYjUw",
                                  currency: "UAH",
                                  startParameter: "test",
                                  prices: itemPrice
                                  );
    }

    private static async Task answerPreCheckoutQuery(ITelegramBotClient botClient, string preCheckoutQuery)
    {
        User.PaymentFlag = true;
        botClient.AnswerPreCheckoutQueryAsync(preCheckoutQuery);
        Console.WriteLine("Payment Success");
        DBManipulations.Add();

    }

    private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
    {
        Console.WriteLine($"Unknown update type: {update.Type}");
        return Task.CompletedTask;
    }
}