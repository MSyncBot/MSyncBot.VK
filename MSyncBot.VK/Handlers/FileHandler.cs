using MSyncBot.Types.Enums;
using VkNet.Model;
using File = MSyncBot.Types.File;

namespace MSyncBot.VK.Handlers
{
    public class FileHandler
    {
        public async Task<File?> DownloadFileAsync(Attachment attachment)
        {
            try
            {
                using var httpClient = new HttpClient();
                
                var fileExtension = Path.GetExtension(attachment.Instance.ToString());
                
                var fileName = GetFileName(attachment.Instance);
                
                var mediaType = !string.IsNullOrEmpty(attachment.Instance.ToString())
                    ? attachment.Instance.ToString().Split('/')[0]
                    : "document";

                var fileType = mediaType switch
                {
                    "photo" => FileType.Photo,
                    "video" => FileType.Video,
                    "audio" => FileType.Audio,
                    _ => FileType.Document
                };

                // Сохраняем файл
                var fileData = await httpClient.GetByteArrayAsync(new Uri(attachment.Instance.ToString()));
                return new File(fileName, fileExtension, fileData, fileType);
            }
            catch (Exception ex)
            {
                Bot.Logger.LogError(ex.ToString());
            }

            return null;
        }

        private static string GetFileName(MediaAttachment mediaAttachment)
        {
            return mediaAttachment switch
            {
                Photo photo => photo.Text ?? $"photo_{photo.OwnerId}_{photo.Id}",
                Video video => video.Title ?? $"video_{video.OwnerId}_{video.Id}",
                _ => "unknown"
            };
        }
    }
}
