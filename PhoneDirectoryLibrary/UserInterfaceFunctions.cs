using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using static PhoneDirectoryLibrary.PhoneDirectory;
using System.Text.RegularExpressions;
using System.Threading;

namespace PhoneDirectoryLibrary
{
    public static class UserInterfaceFunctions
    {
        private static bool firstRun = true;
        private static bool isInput = false;
        private static bool runProgram = true;

        public static void UserDisplayDashboard(ref PhoneDirectory phoneDirectory)
        {
            // We just keep showing this dashboard until the user selects exit
            while (runProgram)
            {
                Console.Clear();

                Console.Title = "RoboDex";
                SetColor();
                Console.CursorVisible = false;

                int width = Console.WindowWidth;

                if (firstRun)
                {
                    TypeText(Center("Welcome to RoboDex, your modern Phone Directory Application!"),10);
                    PrintRowBorder();
                    firstRun = false;
                }

                TypeText(Center("This is your dashboard, please select an option below."),5,ConsoleColor.Yellow);
                PrintRowBorder();

                //We divide the console into two columns and center options in each of them
                TypeText(ToColumns("1 - List all contacts", "2 - Search for a contact"));
                TypeText(ToColumns("3 - Create new contact", "X - Exit"));

                char option = Console.ReadKey(true).KeyChar;

                Console.Clear();

                switch (option)
                {
                    case '1':
                        phoneDirectory.read();
                        Console.ReadKey();
                        break;
                    case '2':
                        Console.CursorVisible = true;
                        UserSearchContacts(ref phoneDirectory);
                        Console.ReadKey();
                        break;
                    case '3':
                        Console.CursorVisible = true;
                        UserInsertContact(ref phoneDirectory);
                        Console.ReadKey();
                        break;
                    case 'X':
                        runProgram = false;
                        ExitSequence();
                        break;
                    case 'x':
                        runProgram = false;
                        ExitSequence();
                        break;
                    default:
                        UserDisplayDashboard(ref phoneDirectory);
                        break;
                }
            }
        }

        /// <summary>
        /// Allows the user to manually enter a new contact on the command line
        /// </summary>
        public static void UserInsertContact(ref PhoneDirectory phoneDirectory)
        {
            string firstName;
            string lastName;
            Country country = Country.United_States;
            string phone;
            string street;
            string houseNum;
            string city;
            string zip;
            Address address;
            State state = State.NA;
            var logger = NLog.LogManager.GetCurrentClassLogger();

            bool tryingAgain = false;

            Console.WriteLine("Creating a new contact..." + Environment.NewLine);

            Console.WriteLine("Let's start with the person's name." + Environment.NewLine);

            // We do little mini loops here in order to ensure we collect these fields
            do
            {
                if(tryingAgain)
                {
                    Console.WriteLine(RequiredMessage("First Name") + Environment.NewLine);
                }

                Console.WriteLine("What is their first name?" + Environment.NewLine);
                SwapColor();
                firstName = Console.ReadLine();
                SwapColor();
                PrintRowBorder();

                tryingAgain = true;

            } while (string.IsNullOrWhiteSpace(firstName));

            tryingAgain = false;

            do
            {
                if (tryingAgain)
                {
                    Console.WriteLine(RequiredMessage("Last Name") + Environment.NewLine);
                }

                Console.WriteLine($"What is {ProperPossesive(firstName)} last name" + Environment.NewLine);
                SwapColor();
                lastName = Console.ReadLine();
                SwapColor();
                PrintRowBorder();

                tryingAgain = true;

            } while (string.IsNullOrWhiteSpace(lastName));

            Console.WriteLine($"Got it, {firstName} {lastName}. Sounds good. Moving on..." + Environment.NewLine);

            tryingAgain = false;

            do
            {
                if (tryingAgain)
                {
                    Console.WriteLine(RequiredMessage("Phone Number") + Environment.NewLine);
                }

                Console.WriteLine($"What is {ProperPossesive(firstName)} phone number?" + Environment.NewLine);
                SwapColor();
                phone = Console.ReadLine();
                SwapColor();
                PrintRowBorder();

                tryingAgain = true;

            } while (string.IsNullOrWhiteSpace(phone));

            tryingAgain = false;

            Console.WriteLine($"Great, now let's enter {ProperPossesive(firstName)} address." + Environment.NewLine);

            // We wrap the whole address entry block in a loop in case entering the address fails
            bool validAddress = true;

            do
            {
                if(validAddress == false)
                {
                    Console.WriteLine("Something went wrong entering that address. Let's try it again." + Environment.NewLine);
                }

                Console.WriteLine("First order of business is the country. You can either enter the country code or you can enter the full country name." + Environment.NewLine);
                Console.WriteLine("If you want, you can also enter either 'h' or '?' to get a list of all the possible countries and their codes." + Environment.NewLine);

                // Get the country
                do
                {
                    if (tryingAgain)
                    {
                        Console.WriteLine("Please enter either a country code or a country name. You can also enter 'h' or 'q' for help." + Environment.NewLine);
                    }

                    SwapColor();
                    string countryInput = Console.ReadLine();
                    SwapColor();
                    PrintRowBorder();

                    // If they enter one character they are probably asking for help, unless they entered a number of course
                    if (countryInput.Length == 1 && !char.IsDigit(countryInput[0]))
                    {
                        if (char.ToUpper(countryInput[0]) == 'H' || countryInput[0] == '?')
                        {
                            Console.WriteLine(Lookups.ListCountryOptions());
                            tryingAgain = true;
                        }
                        // If they aren't asking for help, we try again
                        else
                        {
                            tryingAgain = true;
                            Console.WriteLine($"Sorry, '{countryInput}' isn't a valid command. Did you mean 'h' or 'q' to list available country codes?" + Environment.NewLine);
                        }
                    }
                    // Otherwise, try to look up the country
                    else
                    {
                        //If the parse fails, we try again, otherwise we accept the conversion to Enum
                        tryingAgain = !Enum.TryParse<Country>(AddRemoveUnderscores(countryInput), true, out country);
                        if (!tryingAgain) { Console.WriteLine($"Great, so {firstName} lives in {AddRemoveUnderscores(Enum.GetName(typeof(Country), country), false)}!" + Environment.NewLine); }
                    }
                } while (tryingAgain);

                tryingAgain = false;

                if(country == Country.United_States)
                {
                    Console.WriteLine($"{firstName} {lastName} lives in the United States, so we also need a state. What state do they live in?" + Environment.NewLine);
                    Console.WriteLine("You can enter the two-letter state code, or the full state name." + Environment.NewLine);

                    do
                    {
                        if (tryingAgain)
                        {
                            Console.WriteLine("Let's try entering the state again. This time choose a real state, please." + Environment.NewLine);
                        }

                        SwapColor();
                        string stateInput = Console.ReadLine().Trim();
                        SwapColor();
                        Console.WriteLine(Environment.NewLine);

                        if(stateInput.Length < 1 || stateInput.ToUpper() == "NA")
                        {
                            Console.WriteLine("Please enter either the two-letter state code or the full state name." + Environment.NewLine);
                            tryingAgain = true;
                        }
                        // If they enter only two characters, assume that they tried to enter a state code
                        else if(stateInput.Length == 2)
                        {
                            //If the parse fails, we try again, otherwise we accept the conversion to Enum
                            tryingAgain = !Enum.TryParse<State>(stateInput, true, out state);
                            tryingAgain = false;
                        }
                        // If they enter more than two characters, assume they want to look up by state name
                        else
                        {
                            try
                            {
                                state = Lookups.GetStateByName(stateInput);
                            }
                            catch (ArgumentException e)
                            {
                                logger.Warn($"User entered invalid state name. Received: {stateInput}");
                                Console.WriteLine(e.Message);
                            }
                        }

                        // Confirm their choice
                        if (!tryingAgain) { Console.WriteLine($"Oh, {firstName} lives in {ToProper(Lookups.StateNames[state])}. Okay." + Environment.NewLine); }

                        tryingAgain = true;

                    } while (state == State.NA);
                }

                PrintRowBorder();

                tryingAgain = false;

                Console.WriteLine("Now, what city or town do they live in?" + Environment.NewLine);

                // Get the city
                do
                {
                    if (tryingAgain)
                    {
                        Console.WriteLine(RequiredMessage("City") + Environment.NewLine);
                    }

                    SwapColor();
                    city = Console.ReadLine();
                    SwapColor();
                    PrintRowBorder();

                    tryingAgain = true;
                } while (string.IsNullOrWhiteSpace(city));

                Console.WriteLine($"So {firstName} {lastName} lives in {city}, {ToProper(Lookups.StateNames[state])}. Cool.");

                tryingAgain = false;

                PrintRowBorder();

                Console.WriteLine("Let's get the ZIP or Postal Code now." + Environment.NewLine);

                // Get the zip
                do
                {
                    if (tryingAgain)
                    {
                        Console.WriteLine(RequiredMessage("Zip or Postal Code") + Environment.NewLine);
                    }

                    SwapColor();
                    zip = Console.ReadLine();
                    SwapColor();
                    PrintRowBorder();

                    tryingAgain = true;
                } while (string.IsNullOrWhiteSpace(zip));

                tryingAgain = false;

                Console.WriteLine("What street do they live on? We'll get the house number in a second." + Environment.NewLine);

                //Get the street address
                do
                {
                    if (tryingAgain)
                    {
                        Console.WriteLine(RequiredMessage("Street Address") + Environment.NewLine);
                    }

                    SwapColor();
                    street = Console.ReadLine();
                    SwapColor();
                    PrintRowBorder();

                    tryingAgain = true;

                } while (string.IsNullOrWhiteSpace(street));

                tryingAgain = false;

                Console.WriteLine($"Fantastic, {firstName} lives on {street} in {city}. What is their house number?");

                do
                {
                    if (tryingAgain)
                    {
                        Console.WriteLine(RequiredMessage("House Number") + Environment.NewLine);
                    }

                    SwapColor();
                    houseNum = Console.ReadLine();
                    SwapColor();
                    PrintRowBorder();

                } while (string.IsNullOrWhiteSpace(houseNum));

                try
                {
                    address = new Address(street, houseNum, city, zip, country, state);
                    validAddress = true;
                    Contact contact = new Contact(firstName, lastName, address, phone);
                    phoneDirectory.add(contact);
                    Console.WriteLine($"Great! A contact for {firstName} {lastName} has been created!");
                }
                catch(Address.InvalidAddressFieldException e)
                {
                    logger.Error(e.Message);
                    validAddress = false;
                }
                catch(Exception e)
                {
                    logger.Error(e.Message);
                    validAddress = false;
                }

            } while (!validAddress);            
        }

        public static void UserReadAllContacts(ref PhoneDirectory phoneDirectory)
        {
            Console.WriteLine(phoneDirectory.read());
        }

        public static void UserSearchContacts(ref PhoneDirectory phoneDirectory)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            Console.WriteLine("How would you like to search? You can enter the option ID or the option name.");

            Console.WriteLine("1 - First Name");
            Console.WriteLine("2 - Last Name");
            Console.WriteLine("3 - ZIP");
            Console.WriteLine("4 - City");
            Console.WriteLine("5 - Phone");

            PrintRowBorder();

            SwapColor();
            string searchTypeInput = Console.ReadLine();
            SwapColor();
            string searchTermInput;

            if (string.IsNullOrWhiteSpace(searchTypeInput))
            {
                Console.WriteLine(RequiredMessage("Search Type"));
                logger.Error($"Blank search type");
                return;
            }

            List<Contact> result = new List<Contact>();

            // Once we know the search type, get the search term
            if(searchTypeInput == "1" || Regex.Replace(searchTypeInput.ToUpper(),@"^a-zA-Z","") == "FIRSTNAME")
            {
                Console.Write("Please enter the first name to search for: ");
                result = GetSearchTerm(SearchType.firstName, phoneDirectory);
            }
            else if (searchTypeInput == "2" || Regex.Replace(searchTypeInput.ToUpper(), @"^a-zA-Z", "") == "LASTNAME")
            {
                Console.Write("Please enter the last name to search for: ");
                result = GetSearchTerm(SearchType.lastName, phoneDirectory);
            }
            else if (searchTypeInput == "3" || Regex.Replace(searchTypeInput.ToUpper(), @"^a-zA-Z", "") == "ZIP")
            {
                Console.Write("Please enter the ZIP or Postal Code to search for: ");
                result = GetSearchTerm(SearchType.zip, phoneDirectory);
            }
            else if (searchTypeInput == "4" || Regex.Replace(searchTypeInput.ToUpper(), @"^a-zA-Z", "") == "CITY")
            {
                Console.Write("Please enter the city to search for: ");
                result = GetSearchTerm(SearchType.city, phoneDirectory);
            }
            else if (searchTypeInput == "5" || Regex.Replace(searchTypeInput.ToUpper(), @"^a-zA-Z", "") == "PHONE")
            {
                Console.Write("Please enter the phone number to search for: ");
                SwapColor();
                searchTermInput = Console.ReadLine();
                SwapColor();
                result = string.IsNullOrWhiteSpace(searchTypeInput) ?
                    throw new InvalidSearchTermException() : 
                    phoneDirectory.search(SearchType.phone, searchTermInput).ToList<Contact>();
            }
            else
            {
                Console.WriteLine($"'{searchTypeInput}' is not a valid search type");
                logger.Error($"Invalid search type: {searchTypeInput}");
                return;
            }

            if(result.Count > 1)
            {
                Console.WriteLine(phoneDirectory.read(result));
            }
            else
            {
                Console.WriteLine($"No contacts found for this search.");
            }
        }

        private static List<Contact> GetSearchTerm(SearchType searchType, PhoneDirectory phoneDirectory)
        {
            SwapColor();
            string searchTermInput = Console.ReadLine();
            SwapColor();
            Console.WriteLine(Environment.NewLine);

            if (string.IsNullOrWhiteSpace(searchTermInput))
            {
                throw new InvalidSearchTermException("Search term must not be blank.");
            }
            else
            {
                return phoneDirectory.search(searchType, searchTermInput).ToList<Contact>();
            }
        }

        private static string RequiredMessage(string field)
        {
            Random random = new Random();

            List<string> messages = new List<string>
            {
                $"Hey! You can't do that. {field} is required.",
                $"Nice try, but {field} is required. Please try again.",
                $"Oops, {field} is required. Please try again.",
                $"Nope, try again. We need to have the {field}.",
                $"Uh oh, {field} is required. Please try again.",
                $"Not quite...try entering the {field} again. Don't leave it blank this time.",
                $"Really, no {field}? I don't think so. Try again.",
                $"Nah, we gotta have the {field}."
            };

            return messages.ElementAt(random.Next(messages.Count));
        }

        /// <summary>
        /// Returns a string that is properly possesive
        /// </summary>
        /// <param name="toPossess"></param>
        /// <returns></returns>
        private static string ProperPossesive(string toPossess)
        {
            if(char.ToUpper(toPossess.Last()) == 'S')
            {
                return toPossess + '\'';
            }
            else
            {
                return toPossess + "'s";
            }
        }

        private static void PrintRowBorder()
        {
            Console.WriteLine(new String('—', Console.WindowWidth));
        }

        /// <summary>
        /// If passed true, this method replaces all spaces with underscores
        /// If passed false, this method replaces all underscores with spaces and coverts to proper/title case
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        private static string AddRemoveUnderscores(string text, bool add = true)
        {
            if (add)
            {
                return text.Replace(' ', '_');
            }
            else
            {    
                return ToProper(text.Replace('_', ' '));
            }
        }

        private static void ExitSequence()
        {
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            string message = "Goodbye!";

            TypeText("Goodbye!", 100, ConsoleColor.Yellow);

            Thread.Sleep(1000);
        }

        /// <summary>
        /// Writes the specified text to the console
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="speed">Time between letters, in milliseconds</param>
        private static void TypeText(string text,int speed = 5, ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = color;

            for(int i = 0; i < text.Length; i++)
            {
                Console.Write(text[i]);
                if(text[i] != ' ') { Thread.Sleep(speed); }
            }

            Console.Write(Environment.NewLine);

            SetColor();
        }

        private static string ToColumns(string column1 = "", string column2 = "")
        {
            column1 = column1.PadLeft(Console.WindowWidth / 3);
            column2 = column2.PadRight(Console.WindowWidth / 3);
            int padding = Console.WindowWidth - (column1.Length + column2.Length);
            return column1 + new string(' ', padding) + column2;
        }

        private static string Center(string toCenter)
        {
            int length = toCenter.Length;
            int padding = (Console.WindowWidth - length) / 2;

            return new string(' ', padding) + toCenter + new string(' ', padding);
        }

        private static string ToProper(string text)
        {
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(text.ToLower());
        }

        private static void SetColor(bool input)
        {
            isInput = input;
            SetColor();
        }

        private static void SetColor()
        {
            if (isInput)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }

        private static void SwapColor()
        {
            if (isInput)
            {
                SetColor(false);
            }
            else
            {
                SetColor(true);
            }
        }
    }
}
