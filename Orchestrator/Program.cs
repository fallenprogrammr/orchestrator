using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;

namespace Orchestrator {

    partial class Program {
        static string browsers;
        static int instances;
        static string player=string.Empty;
        private static IWebDriver browserDriver;

        static void Main(string[] args) {

            try {
                if (args.Count() < 3) {
                    ShowUsageMessage();
                }
                else {
                    var tinyArgs = new Tiny(args);
                    if (!ArgsAreValid(tinyArgs)) {
                        ShowUsageMessage();
                    }
                    else {
                        Orchestrate();
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Exception occurred:"+Environment.NewLine+ex);
            }
        }

        private static bool ArgsAreValid(Tiny tinyArgs) {
            try {
                browsers= tinyArgs.Arguments.browsers.ToLower();
                instances = Int32.Parse(tinyArgs.Arguments.instances);
                player = tinyArgs.Arguments.player;
            }
            catch (Exception) {
                return false;
            }
            if (!(browsers.Contains("chrome") || browsers.Contains("ie") || browsers.Contains("firefox") || browsers.Contains("phantomjs"))) {
                Console.WriteLine("The browsers argument passed does not contain a recognized browser.");
                return false;
            }
            if (instances < 1) {
                Console.WriteLine("The instances argument passed must have a value of at least 1, it will be defaulted to 1 if an invalid value was specified.");
                instances = 1;
            }
            if (!File.Exists(player)) {
                Console.WriteLine("The player argument passed contains a file that cannot be found.");
                return false;
            }
            return true;
        }

        private static void ShowUsageMessage() {
            Console.WriteLine("Orchestrator usage:");
            Console.WriteLine("orc browsers:<chrome,ie,firefox,phantomjs> instances:<no of instances of the browser> player:<path to the player dll file to run the scenario>");
            Console.WriteLine("parameters explained:");
            Console.WriteLine("browsers - You can use any combination from chrome,ie,firefox and phantomjs or pick a single one. example: browsers:ie,phantomjs or browsers:firefox.");
            Console.WriteLine("instances - Each selected browser in the list is instantiated these many times to play the script in the player dll.");
            Console.WriteLine("player: Path of the player dll to execute the scenario(s) for the web application.");
        }


    }
}
