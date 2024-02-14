using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace MSyncBot.VK;

abstract class Program
{
    static async Task Main(string[] args)
    {
        var api = new VkApi();
        
        api.Authorize(new ApiAuthParams
        {
            AccessToken = "",
            Settings = Settings.All,
        });
        
        try
        {
            while (true)
            {
                var longPollResponse = await api.Groups.GetLongPollServerAsync(224017484);
                var updates = await api.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
                {
                    Ts = longPollResponse.Ts,
                    Key = longPollResponse.Key,
                    Server = longPollResponse.Server,
                    Wait = 25
                });

                if (updates.Updates is null) continue;
                if (updates.Updates.Count == 0) continue;
                
                var handleUpdatesTask = new Task(async () =>
                {
                    foreach (var update in updates.Updates)
                    {
                        if (update.Instance is MessageNew message)
                        {
                            await api.Messages.SendAsync(new MessagesSendParams()
                            {
                                Message = message.Message.Text,
                                PeerId = message.Message.PeerId,
                                RandomId = new Random().Next(0, 999999)
                            });
                        }
                    }
                    
                });
                handleUpdatesTask.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}