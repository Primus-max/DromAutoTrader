using DromAutoTrader.ImageServices.Interfaces;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DromAutoTrader.ImageServices
{
    public class UnicomImageService : IWebsite
    {
        public string WebSiteName => throw new NotImplementedException();

        public string? Brand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? Articul { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<string> BrandImages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Authorization()
        {
            throw new NotImplementedException();
        }

        public IWebElement GetSearchInput()
        {
            throw new NotImplementedException();
        }

        public void SetArticulInSearchInput()
        {
            throw new NotImplementedException();
        }
    }
}
