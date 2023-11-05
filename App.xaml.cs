using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace DromAutoTrader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        // В вашем классе App
      


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

           // var services = new ServiceCollection();

            // Здесь вы можете добавить ваши сервисы в контейнер DI
            //services.AddTransient<AppContext>();

            //services.AddDbContext<AppContext>(options =>
            // {
            //     var config = new ConfigurationBuilder()
            //         .SetBasePath(Directory.GetCurrentDirectory())
            //         .AddJsonFile("appsettings.json")
            //         .Build();

            //     var connectionString = config.GetConnectionString("DefaultConnection");

            //     options.UseSqlite(connectionString);
            // });

            // И другие сервисы...

            // Постройте ServiceProvider на основе сервисов, добавленных в контейнер
           // ServiceProvider = services.BuildServiceProvider();

            // Запустите ваше главное окно или другие части приложения
            //var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            //mainWindow.Show();
        }
    }
}
