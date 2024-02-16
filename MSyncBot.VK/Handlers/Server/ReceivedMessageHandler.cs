﻿using System.Text;
using System.Text.Json;
using MLoggerService;
using MSyncBot.Types;
using MSyncBot.Types.Enums;

namespace MSyncBot.VK.Handlers.Server;

public class ReceivedMessageHandler
{
    private static ulong _lastUserId;

    public void ReceiveMessage(byte[] buffer, long offset, long size, MLogger logger) =>
        _ = Task.Run(async () =>
        {
            var jsonMessage = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            var message = JsonSerializer.Deserialize<Message>(jsonMessage);

            if (message.Messenger.Type is MessengerType.Vk)
                return;

            switch (message.Type)
            {
                case MessageType.Text:
                    logger.LogInformation(
                        $"Received message from {message.Messenger.Name}: " +
                        $"{message.User.FirstName} ({message.User.Id}) - {message.Text}");

                    break;

                default:
                {
                    logger.LogInformation(
                        $"Received album from {message.Messenger.Name} with {message.Files.Count} files: " +
                        $"{message.User.FirstName} ({message.User.Id})");

                    foreach (var file in message.Files)
                    {
                        var memoryStream = new MemoryStream(file.Data);
                    }

                    break;
                } 
            }
            
            _lastUserId = message.User.Id;
        });
}