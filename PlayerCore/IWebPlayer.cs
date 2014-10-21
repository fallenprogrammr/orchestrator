using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PlayerCore
{
    public interface IWebPlayer {
        void Execute(IWebDriver browser);
    }
}
