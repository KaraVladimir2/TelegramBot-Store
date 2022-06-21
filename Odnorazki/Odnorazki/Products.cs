using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Examples.Polling;

public class Products
{
    public static async Task<Message> ElfBar(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendMediaGroupAsync(chatId: message.Chat.Id,
                                                media: new IAlbumInputMedia[]
                                                {
                                                    new InputMediaPhoto("https://vandalvape.com.ua/image/cache/catalog/odnorazki/elf-bar-1500/elf-bar-1500-puffs-600x600.jpg"),
                                                    new InputMediaPhoto("https://vandalvape.com.ua/image/cache/catalog/odnorazki/elf-bar-lux/elf-bar-lux-1500-600x600.jpg"),
                                                    new InputMediaPhoto("https://vandalvape.com.ua/image/cache/catalog//odnorazki/elf-bar-2000/233/8-600x600.png"),
                                                    new InputMediaPhoto("https://vandalvape.com.ua/image/cache/catalog/odnorazki/elf-bar-2500/1-600x600.jpg"),
                                                    new InputMediaPhoto("https://vandalvape.com.ua/image/cache/catalog/odnorazki/elf-bar-3600-5000/zag-600x600.jpg")
                                                }
                                                );
        await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                             text: "1) ELF BAR 1500 850MAH 5% - 289 грн\n\n" +
                                                   "2) ELF BAR LUX 1500 850MAH 5% - 289 грн\n\n" +
                                                   "3) ELF BAR 2000 1200MAH 5% - 349 грн\n\n" +
                                                   "4) ELF BAR 2500 1400MAH 5% - 389 грн\n\n" +
                                                   "5) ELF BAR 3600 650MAH 5% ПЕРЕЗАРЯЖАЕМЫЙ - 469 грн", parseMode: null,
                                             replyMarkup: null);
        InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1","ElfBar1"),
                        InlineKeyboardButton.WithCallbackData("2","ElfBar2"),
                        InlineKeyboardButton.WithCallbackData("3","ElfBar3")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("4","ElfBar4"),
                        InlineKeyboardButton.WithCallbackData("5","ElfBar5"),
                    }
                });

        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                    text: "Выберите товар для покупки",
                                                    replyMarkup: inlineKeyboard);
    }
}