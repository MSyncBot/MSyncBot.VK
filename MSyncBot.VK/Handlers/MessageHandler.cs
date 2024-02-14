using VkNet;
using VkNet.Model;

namespace MSyncBot.VK.Handlers;

public class MessageHandler
{
    public async Task HandleMessagesAsync(VkApi bot, MessageNew message)
    {
        await bot.Messages.SendAsync(new MessagesSendParams()
        {
            Message = message.Message.Text,
            PeerId = message.Message.PeerId,
            RandomId = new Random().Next(0, 999999),
        });
    }
}