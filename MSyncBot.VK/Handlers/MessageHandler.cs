using System.Text.Json;
using MLoggerService;
using MSyncBot.Types;
using MSyncBot.Types.Enums;
using MSyncBot.VK.Handlers.Server;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.StringEnums;
using VkNet.Model;
using Chat = MSyncBot.Types.Chat;
using File = MSyncBot.Types.File;
using Message = MSyncBot.Types.Message;
using User = MSyncBot.Types.User;

namespace MSyncBot.VK.Handlers;

public class MessageHandler
{
    public async Task HandleMessagesAsync(VkApi bot, MessageNew message)
    {
        try
        {
            if (message.Message.PeerId != 2000000002) // Check id of my group with friends
                return;

            var users = bot.Users.Get(
                new List<long>
                {
                    (long)message.Message.FromId!
                },
                ProfileFields.FirstName | ProfileFields.LastName,
                NameCase.Nom);
            var user = users[0];
            ReceivedMessageHandler.LastUserId = (ulong)user.Id!;
            
            var attachments = message.Message.Attachments;
            switch (attachments.Count)
            {
                case > 0 and 1:
                    var attachment = attachments.FirstOrDefault();
                    var downloadedFile = await new FileHandler()
                        .DownloadFileAsync(attachment!);

                    var messageType = downloadedFile!.Type switch
                    {
                        FileType.Photo => MessageType.Photo,
                        FileType.Video => MessageType.Video,
                        FileType.Audio => MessageType.Audio,
                        _ => MessageType.Document,
                    };

                    var fileMessage = new Message(
                        new Messenger("MSyncBot.VK", MessengerType.Vk),
                        messageType,
                        new User(user.FirstName, (ulong)user.Id)
                        {
                            LastName = user.LastName
                        },
                        new Chat(string.Empty, 1)
                    );
                    fileMessage.Files.Add(downloadedFile);
                    fileMessage.Text = message.Message.Text;

                    var fileJsonMessage = JsonSerializer.Serialize(fileMessage);
                    Bot.Server!.SendTextAsync(fileJsonMessage);
                    return;
                case > 0:
                {
                    var downloadFilesTasks = new List<Task<File?>>();
                    downloadFilesTasks.AddRange(attachments.Select(attachment =>
                        new FileHandler().DownloadFileAsync(attachment)));
                    var downloadedFiles = await Task.WhenAll(downloadFilesTasks);

                    var albumMessage = new Message(
                        new Messenger("MSyncBot.VK", MessengerType.Vk),
                        MessageType.Album,
                        new User(user.FirstName, (ulong)user.Id)
                        {
                            LastName = user.LastName
                        },
                        new Chat(string.Empty, 1)
                    );
                    albumMessage.Files.AddRange(downloadedFiles!);
                    albumMessage.Text = message.Message.Text;

                    var albumJsonMessage = JsonSerializer.Serialize(albumMessage);
                    Bot.Server!.SendTextAsync(albumJsonMessage);
                    return;
                }
            }

            var textMessage = new Message(
                new Messenger("MSyncBot.VK", MessengerType.Vk),
                MessageType.Text,
                new User(user.FirstName, (ulong)user.Id)
                {
                    LastName = user.LastName
                },
                new Chat(string.Empty, 1)
            )
            {
                Text = message.Message.Text
            };

            var jsonTextMessage = JsonSerializer.Serialize(textMessage);
            Bot.Server!.SendTextAsync(jsonTextMessage);
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}