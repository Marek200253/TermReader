


using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V110.Runtime;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using System.Net;
using System.Text;

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
                }
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex, "arguments"); }

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
        Thread.Sleep(2500);
        var terminy = cd.FindElement(By.CssSelector("div[class='table-responsive']")).FindElements(By.Id("tr"));
        int i = 0;
        foreach (var t in terminy)
        {
            string[] items = new string[8]; //0.ID, 1.TYP, 2.NAZEV, 3.UCITEL, 4.CAS, 5.MISTNOST, 6.CAPACITA, 7.OTEVŘENO
            try
            {
                if(t.GetCssValue("class") is "row-headline")
                {
                    items[1] = t.FindElement(By.CssSelector("td[data-testid='term-type']")).Text;
                    items[2] = t.FindElement(By.CssSelector("td[data-testid='course-name']")).Text;
                    items[4] = t.FindElement(By.CssSelector("td[data-testid='date']")).Text;

                }
/*
                if (t.FindElement(By.CssSelector("td[data-testid='term-type']")).Text is ("JA" or "ZK" or "Z"))
                {

                }
                else
                {
                    
                }*/
            }
            catch (NoSuchElementException ex)
            {
                Debug.WriteLine(ex.Message, "Lister");
            }
        }
    }

    class Term
    {
        string name, typ, date, cap, room, ID, teacher;
        bool capable = false;
        public Term(string name, string typ, string date, string cap, string room, string teacher, bool capable, string ID) {
            this.name = name;
            this.typ = typ;
            this.date = date;
            this.cap = cap;
            this.capable = capable;
            this.ID = ID;
            this.room = room;
            this.teacher = teacher;
        }
    }
}