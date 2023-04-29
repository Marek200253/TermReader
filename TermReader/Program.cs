using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V110.Runtime;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;

namespace Program
{
    class Program
    {
        private static void Main(string[] args)
        {
            //Proměnné
            string[] userData = { "", "" };
            string uBrow = "";
            bool developer = false;
            bool allView = false;
            bool skip = false;
            bool nextTry = false;
            bool benchmark = false;
            bool debug = false;
            IWebDriver cd;
            List<Term> terms = new List<Term>();

            Console.WriteLine("Start!");
            Console.WriteLine("Zadejte argumenty nebo zmáčkněte enter: (? pro info)");

            //Argumenty
            try
            {
                string argumenty = Console.ReadLine();
                if (argumenty is null)
                    argumenty = "";
                if (argumenty.Contains("?"))
                {
                    Console.WriteLine("-u [přihlašovací jméno]\n" +
                        "-p [přihlašovací heslo]\n" +
                        "-v [1-chrome; 2-Edge; 3-Firefox]\n" +
                        "-b (benchmark programu)" +
                        "-skip (program přeskočí přihlašování z konzole a počká na přihlášení v prohlížeči)\n");
                    argumenty = Console.ReadLine();
                    if (argumenty is null)
                        argumenty = "";
                }
                var argument = argumenty.Split("-");
                bool[] finish = { false, false };
                foreach (string arg in argument)
                {
                    List<char> tempAlph = new List<char>();
                    var cmm = arg.Split(" ");
                    switch (cmm[0])
                    {
                        case "u": //uzivatel
                            userData[0] = cmm[1];
                            break;
                        case "p": //heslo
                            userData[1] = cmm[1];
                            break;
                        case "d": //developer - odemiká rozšířený pohled
                            developer = true;
                            allView = true;
                            Console.WriteLine("Vstupujete do vývojářského módu");
                            break;
                        case "skip":
                            skip = true;
                            break;
                        case "v":
                            uBrow = cmm[1];
                            break;
                        case "b":
                            Console.WriteLine("Vstupujete do testovacího módu");
                            benchmark = true;
                            break;
                        case "debug":
                            debug = true;
                            break;
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine(ex, "arguments"); }

            if (!debug)
            {
                //Otevření okna prohlížeče
                if (uBrow.Length == 0)
                {
                    Console.WriteLine("Vyberte prohlížeč: (výchozí: Chrome)\n" +
                        "1. Google Chrome\n" +
                        "2. MS Edge\n" +
                        "3. Firefox");
                    uBrow = Console.ReadLine();
                }
                try
                { //Pokus o spuštění prohlížeče
                    switch (uBrow)
                    {
                        case "2":
                            EdgeDriverService eDService = EdgeDriverService.CreateDefaultService();
                            EdgeOptions eOptions = new EdgeOptions();
                            eOptions.AcceptInsecureCertificates = false;
                            eOptions.PageLoadStrategy = PageLoadStrategy.Normal;
                            if (!allView)
                            {
                                eDService.EnableVerboseLogging = false;
                                eDService.SuppressInitialDiagnosticInformation = true;
                                eDService.HideCommandPromptWindow = true;
                                if (!skip)
                                    eOptions.AddArgument("--headless");
                                eOptions.AddArgument("--no-sandbox");
                                eOptions.AddArgument("--disable-gpu");
                                eOptions.AddArgument("--disable-crash-reporter");
                                eOptions.AddArgument("--disable-extensions");
                                eOptions.AddArgument("--disable-in-process-stack-traces");
                                eOptions.AddArgument("--disable-logging");
                                eOptions.AddArgument("--disable-dev-shm-usage");
                                eOptions.AddArgument("--log-level=3");
                                eOptions.AddArgument("--output=/dev/null");
                            }
                            cd = new EdgeDriver(eDService, eOptions);
                            break;
                        case "3":
                            FirefoxDriverService fDService = FirefoxDriverService.CreateDefaultService();
                            FirefoxOptions fOptions = new FirefoxOptions();
                            fOptions.AcceptInsecureCertificates = false;
                            fOptions.PageLoadStrategy = PageLoadStrategy.Normal;
                            if (!allView)
                            {
                                fDService.SuppressInitialDiagnosticInformation = true;
                                fDService.HideCommandPromptWindow = true;
                                if (!skip)
                                    fOptions.AddArgument("--headless");
                                fOptions.AddArgument("--no-sandbox");
                                fOptions.AddArgument("--disable-gpu");
                                fOptions.AddArgument("--disable-crash-reporter");
                                fOptions.AddArgument("--disable-extensions");
                                fOptions.AddArgument("--disable-in-process-stack-traces");
                                fOptions.AddArgument("--disable-logging");
                                fOptions.AddArgument("--disable-dev-shm-usage");
                                fOptions.AddArgument("--log-level=3");
                                fOptions.AddArgument("--output=/dev/null");
                            }
                            cd = new FirefoxDriver(fDService, fOptions);
                            break;
                        default:
                            ChromeDriverService cDService = ChromeDriverService.CreateDefaultService();
                            ChromeOptions cOptions = new ChromeOptions();
                            cOptions.AcceptInsecureCertificates = false;
                            cOptions.PageLoadStrategy = PageLoadStrategy.Normal;
                            if (!allView)
                            {
                                cDService.EnableVerboseLogging = false;
                                cDService.SuppressInitialDiagnosticInformation = true;
                                cDService.HideCommandPromptWindow = true;
                                if (!skip)
                                    cOptions.AddArgument("--headless");
                                cOptions.AddArgument("--no-sandbox");
                                cOptions.AddArgument("--disable-gpu");
                                cOptions.AddArgument("--disable-crash-reporter");
                                cOptions.AddArgument("--disable-extensions");
                                cOptions.AddArgument("--disable-in-process-stack-traces");
                                cOptions.AddArgument("--disable-logging");
                                cOptions.AddArgument("--disable-dev-shm-usage");
                                cOptions.AddArgument("--log-level=3");
                                cOptions.AddArgument("--output=/dev/null");
                            }
                            cd = new ChromeDriver(cDService, cOptions);
                            break;
                    }

                }
                catch (Exception ex) { Console.WriteLine("\nChybná verze webdriveru k prohlížeči (nebo prohlížeče). Stáhněte nebo vyberte správnou\n\n" + ex); return; }

                //Přihlášení do aplikace
                cd.Navigate().GoToUrl(@"https://new.kos.cvut.cz/login");
                string nameS = "";
                try
                {
                    while (cd.Url == @"https://new.kos.cvut.cz/login")
                    {
                        if (skip)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            Thread.Sleep(1000);
                            IWebElement e = cd.FindElement(By.Id("username"));
                            if (!developer)
                                Console.Clear();
                            if (!nextTry)
                            {
                                nextTry = true;
                                if (userData[0].Length < 3)
                                {
                                    for (int i = 0; i < 50; i++)
                                        e.SendKeys("\b");
                                    Console.WriteLine("Zadejte přihlašovají jméno:");
                                    nameS = Console.ReadLine();
                                    if (nameS is null)
                                        nameS = "";
                                    e.SendKeys(nameS);
                                }
                                else { nameS = userData[0]; e.SendKeys(userData[0]); }
                            }
                            else
                            {
                                Console.WriteLine("Špatně zadané údaje!\nUživatel: " + nameS);
                            }
                            e = cd.FindElement(By.Id("password"));
                            if (userData[1].Length < 8)
                            {
                                StringBuilder stringBuilder = new StringBuilder();
                                Console.WriteLine("Heslo: ");
                                for (int i = 0; i < 50; i++)
                                    e.SendKeys("\b");
                                while (true)
                                {
                                    ConsoleKeyInfo newKey = Console.ReadKey(true);
                                    char passwordKey = newKey.KeyChar;
                                    if (newKey.Key == ConsoleKey.Backspace)
                                    { nextTry = false; }
                                    if (newKey.Key == ConsoleKey.Enter)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if ((newKey.Key == ConsoleKey.Backspace) || (newKey.Key == ConsoleKey.Delete))  //podmínka pro zobrazování "*" místo znaků
                                        { Console.Write("\b"); }
                                        else { Console.Write("*"); }
                                        stringBuilder.Append(passwordKey.ToString());
                                    }
                                }
                                e.SendKeys(stringBuilder.ToString());
                            }
                            else { e.SendKeys(userData[1]); }
                            if (!developer)
                                Console.Clear();
                            e = cd.FindElement(By.CssSelector("button[data-testid='button-login']"));
                            e.Click();
                            Thread.Sleep(1500);
                        }
                    }
                }
                catch (Exception ex) { Debug.WriteLine("Chyba: " + ex, "Login"); }

                //Nedosažitelná výjimka
                if (cd.Url == @"https://new.kos.cvut.cz/login")
                {
                    cd.Close();
                    Console.WriteLine("Přihlášení neproběhlo úspěšně, spusťe program znovu (zkuste argument -skip pro neomezený počet pokusů přihlášení v prohlížeči)");
                    return;
                }
                string UserNAME = cd.FindElement(By.CssSelector("span[data-testid='fullname']")).Text;
                Console.WriteLine("\nPřihlášen jako uživatel: " + UserNAME);

                //Cookies
                CookieContainer cc = new CookieContainer();
                foreach (OpenQA.Selenium.Cookie c in cd.Manage().Cookies.AllCookies)
                {
                    string name = c.Name;
                    string value = c.Value;
                    cc.Add(new System.Net.Cookie(name, value, c.Path, c.Domain));
                }

                cd.Url = "https://new.kos.cvut.cz/terms-offered";
                cd.Navigate();
                Thread.Sleep(5000);
                Console.WriteLine("Čtu...");
                var terminy = cd.FindElement(By.CssSelector("div[class='base-table-wrapper table-student-terms']")).FindElements(By.TagName("tr"));
                int listedID = 0;
                Console.WriteLine("Hotovo:");
                foreach (var t in terminy)
                {
                    if (benchmark && (listedID >= 11))
                        break;
                    string reporter = $" {listedID}/{terminy.Count / 2}";
                    Console.Write(reporter);
                    string[] items = { "", "", "", "", "", "", "", "" };     //0.ID, 1.TYP, 2.NAZEV, 3.UCITEL, 4.CAS, 5.MISTNOST
                    bool open = false;
                    try
                    {
                        if (t.GetAttribute("class").Contains("row-headline"))
                        {
                            t.FindElement(By.CssSelector("td[data-testid='details']")).Click();
                            items[0] = t.FindElement(By.CssSelector("td[data-testid='course-code']")).Text;
                            items[1] = t.FindElement(By.CssSelector("td[data-testid='term-type']")).Text;
                            items[2] = t.FindElement(By.CssSelector("td[data-testid='course-name']")).Text;
                            items[4] = t.FindElement(By.CssSelector("td[data-testid='date']")).Text;
                            items[5] = t.FindElement(By.CssSelector("td[data-testid='room']")).Text;
                            items[6] = t.FindElement(By.CssSelector("td[data-testid='state']")).Text;
                            open = t.FindElement(By.CssSelector("td[data-testid='state-text']")).Text.ToLower() is "volná kapacita";
                            terms.Add(new Term(items[2], items[1], items[4], items[6], items[5], items[3], open, items[0], items[7]));
                            listedID++;
                        }
                        else
                        {
                            if ((terms.Count >= listedID) && (terms.Count > 0))
                            {
                                terms[listedID - 1].teacher = t.FindElement(By.CssSelector("div[data-testid='examiner']")).FindElement(By.CssSelector("a[data-testid='usermap']")).Text;
                                terms[listedID - 1].text = t.FindElement(By.CssSelector("div[data-testid='note']")).FindElement(By.CssSelector("span[class='attribute-value']")).Text;
                            }
                        }
                    }
                    catch (NoSuchElementException ex)
                    {
                        Debug.WriteLine(ex.Message, "Lister");
                    }
                    for (int i = 0; i < reporter.Length; i++)
                        Console.Write("\b");
                }

                cd.Url = "https://new.kos.cvut.cz/logout";
                cd.Navigate();
                cd.Quit();

                string[] parser = new string[terms.Count];
                for (int i = 0; i < terms.Count; i++)
                    parser[i] = terms[i].toBlock;
                printToFile(parser);
            }
            else
            {
                new Term("TEST0", "TEST1", "1.1.2022 8:00 - 10:00", "TEST3", "TEST4", "TEST5", true, "TEST7", "TEST8");
                Console.ReadLine();
            }
        }

        private static void printToFile(string[] linky) //partly provided by ChatGPT
        {
            string[] lines = linky;
            string icsFileContent = "BEGIN:VCALENDAR\nVERSION:2.0\n";

            foreach (string line in lines)
            {
                Debug.Print(line);
                string[] parts = line.Split(';');
                string[] Tdatum = parts[1].Split(" ");
                string[] datum = { Tdatum[0] + " " + Tdatum[1], Tdatum[0] + " " + Tdatum[3]};

                // načtení data a času události
                DateTime eventDateS = DateTime.Today;
                DateTime.TryParseExact(datum[0], "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventDateS);
                DateTime eventDateE = DateTime.Today;
                DateTime.TryParseExact(datum[1], "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventDateE);

                // formát pro data a čas v .ics souboru
                string eventDateStringS = eventDateS.ToString("yyyyMMddTHHmmssZ");
                string eventDateStringE = eventDateE.ToString("yyyyMMddTHHmmssZ");

                // přidání události do .ics souboru
                icsFileContent += "BEGIN:VEVENT\n";
                icsFileContent += "DTSTART:" + eventDateStringS + "\n";
                icsFileContent += "DTEND:" + eventDateStringE + "\n";
                icsFileContent += "SUMMARY:" + parts[0] + "\n";
                icsFileContent += "DESCRIPTION:" + parts[2] + "\n";
                icsFileContent += "END:VEVENT\n";
            }

            // konec kalendáře
            icsFileContent += "END:VCALENDAR";

            // zápis souboru .ics
            try
            {
                File.WriteAllText("udalosti.ics", icsFileContent);
                Console.WriteLine("Zapsáno do souboru události.ics, soubor naleznete ve složce programu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Zapsání do souboru se nezdařilo, vyskytla se chyba: " + ex);
            }
        }

        
    }

    class Term
    {
        public string name = "$Jméno", typ = "$Typ", date = "1.1.2000 0:00 - 0:00", cap = "$Kapacita", room = "neuvedeno", 
            ID = "$ID_Předmětu", teacher = "neuvedeno", text = "neuvedeno";
        public bool capable = false;
        public Term(string name, string typ, string date, string cap, string room, string teacher, bool capable, string ID, string text)
        {
            try
            {
                this.name = parseText(name).Length > 1 ? parseText(name) : this.name;
                this.typ = parseText(typ).Length > 1 ? parseText(typ) : this.typ;
                this.date = parseText(date).Length > 1 ? parseText(date) : this.date;
                this.cap = parseText(cap).Length > 1 ? parseText(cap) : this.cap;
                this.capable = capable;
                this.ID = parseText(ID).Length > 1 ? parseText(ID) : this.ID;
                this.room = parseText(room).Length > 1 ? parseText(room) : this.room;
                this.teacher = parseText(teacher).Length > 1 ? parseText(teacher) : this.teacher;
                this.text = parseText(text).Length > 1 ? parseText(text) : this.text;
            }catch (Exception ex) { Console.WriteLine(ex); }
        }

        private string parseText(string text)
        {
            string result = "";
            char[] charakters = text.ToCharArray();
            string inventory = "abcdefghijklmnopqrstuvwxyzěščřžýáíéóúůťď";
            inventory = inventory + inventory.ToUpper() + "0123456789;-.!_-?:, ";
            char[] inventChar = inventory.ToCharArray();
            foreach (char c in charakters)
            {
                int index = Array.IndexOf(inventChar, c);
                if(index != -1)
                    result += c.ToString();
            }
            return result.Trim();
        }
        public new string ToString => $"\n\n{typ}\n{ID}\n{name}\n{date}\n{cap}\n{capable}\n{room}\n{teacher}\n{text}";

        public string toBlock => ($"{name};{date};{ID} - {typ} - Možno se přihlásit: {(capable ? "Ano":"Ne")}, kapacita: {cap}, " +
            $"zkoušející: {teacher}, poznámka předmětu: {text}").Replace("\n","");
    }
}