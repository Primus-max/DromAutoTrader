using OpenQA.Selenium;

namespace DromAutoTrader.AdsPowerManager
{
    /// <summary>
    /// Класс получения IWebdriver основанного на AdsPower
    /// </summary>
    public class AdsPower
    {
        public AdsPower()
        {

        }

        /// <summary>
        /// Получаю драейвер по имени канала (имя профиля соответствует имени канала)
        /// </summary>
        /// <param name="selectedChannel"></param>
        /// <returns></returns>
        public async Task< IWebDriver> GetDriverByChannel(string selectedChannel)
        {
            BrowserManager browserManager = new BrowserManager();
            List<Profile> profiles = await ProfileManager.GetProfiles();

            foreach (Profile profile in profiles) 
            {
                if (profile.Name != selectedChannel || profile == null) continue;

              return await browserManager.InitializeDriver(profile.UserId);
            }

            return null;
        }
    }
}
