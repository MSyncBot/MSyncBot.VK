﻿using System.Text;
using System.Text.Json;
using MSyncBot.Types.Enums;
using VkNet;
using VkNet.Model;
using Message = MSyncBot.Types.Message;

namespace MSyncBot.VK.Handlers.Server;

public class ReceivedMessageHandler
{
    public static ulong LastUserId { get; set; }

    public void ReceiveMessage(byte[] buffer, long offset, long size, VkApi bot) =>
        _ = Task.Run(async () =>
        {
            var jsonMessage = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            var message = JsonSerializer.Deserialize<Message>(jsonMessage);

            if (message.Messenger.Type is MessengerType.Vk)
                return;

            switch (message.Type)
            {
                case MessageType.Text:
                    Bot.Logger.LogInformation(
                        $"Received message from {message.Messenger.Name}: " +
                        $"{message.User.FirstName} ({message.User.Id}) - {message.Text}");
                    
                    var messageText = LastUserId != message.User.Id
                        ? $"{message.Messenger.Type}: {message.User.FirstName} {message.User.LastName}\n{message.Text}"
                        : message.Text;
                    
                    await bot.Messages.SendAsync(new MessagesSendParams()
                    {
                        Message = messageText,
                        PeerId = 2000000002,
                        RandomId = new Random().Next(0, 99999)
                    });

                    break;

                default:
                {
                    Bot.Logger.LogInformation(
                        $"Received album from {message.Messenger.Name} with {message.Files.Count} files: " +
                        $"{message.User.FirstName} ({message.User.Id})");

                    foreach (var file in message.Files)
                    {
                        var memoryStream = new MemoryStream(file.Data);
                    }

                    break;
                }
            }

            LastUserId = message.User.Id;
        });
}