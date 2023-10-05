using System.Windows.Controls;

namespace DromAutoTrader.Views
{
    internal class LocatorService
    {
        public Frame? ChannelFrame { get; set; }

        public static LocatorService Current { get; } = new LocatorService();
    }
}
