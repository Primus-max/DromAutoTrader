using DromAutoTrader.Data;

namespace DromAutoTrader.DromManager
{
    public class ChannelManager
    {

        public ChannelManager() { }

        public static async Task GetChannelsAsync()
        {
            List<Profile> profiles = await ProfileManager.GetProfiles();

            using AppContext db = AppContextFactory.GetInstance();

            try
            {                
                db.Channels.Load();

                foreach (var profile in profiles)
                {
                    Channel channel = new()
                    {
                        Name = profile?.Name
                    };

                    db.Channels.Add(channel);
                }
                db.SaveChanges();
            }
            catch (Exception)
            {
            }
        }
    }
}
