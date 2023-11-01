using DromAutoTrader.AdsPowerManager;
using DromAutoTrader.Data;

namespace DromAutoTrader.DromManager
{
    public class ChannelManager
    {

        public ChannelManager() { }

        public static async Task GetChannelsAsync()
        {
            List<Profile> profiles = await ProfileManager.GetProfiles();

             AppContext db = AppContextFactory.GetInstance();

            try
            {
                db.Channels.Load();

                foreach (var profile in profiles)
                {
                    string channelName = profile?.Name;

                    // Проверяем, существует ли канал с таким именем
                    bool channelExists = db.Channels.Any(c => c.Name == channelName);

                    if (!channelExists)
                    {
                        Channel channel = new()
                        {
                            Name = channelName
                        };

                        db.Channels.Add(channel);
                    }
                }

                db.SaveChanges();
            }
            catch (Exception)
            {
                // Обработка ошибок
            }
        }

    }
}
