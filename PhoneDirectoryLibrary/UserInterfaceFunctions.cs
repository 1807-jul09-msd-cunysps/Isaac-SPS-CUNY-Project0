using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using static PhoneDirectoryLibrary.PhoneDirectory;
using System.Text.RegularExpressions;
using System.Threading;
using PhoneDirectoryLibrary.Seeders;
using System.IO;

namespace PhoneDirectoryLibrary
{
    public static class UserInterfaceFunctions
    {
        private static bool firstRun = true;
        private static bool isInput = false;
        private static bool runProgram = true;

        public static void UserDisplayDashboard(ref PhoneDirectory phoneDirectory)
        {
            try
            {
                // We just keep showing this dashboard until the user selects exit
                while (runProgram)
                {
                    Console.Clear();

                    Console.Title = "RoboDex";
                    SetColor(false);
                    Console.CursorVisible = false;

                    int width = Console.WindowWidth;

                    if (firstRun)
                    {
                        TypeText(Center("Welcome to RoboDex, your modern Phone Directory Application!"), 10);
                        PrintRowBorder();
                        firstRun = false;
                    }

                    TypeText(Center("This is your dashboard, please select an option below."), 5, ConsoleColor.Yellow);
                    TypeText(Center($"A JSON copy of all data is stored at {phoneDirectory.DataFilePath}"), 5, ConsoleColor.Red);

                    PrintRowBorder();

                    //We divide the console into two columns and center options in each of them
                    TypeText(ToColumns("1 - List all contacts", "2 - Search for a contact"));
                    TypeText(ToColumns("3 - Create new contact", "4 - Update a contact"));
                    TypeText(ToColumns("5 - Seed with random contacts", "6 - Delete a contact"));
                    TypeText(Center("X - Exit"), color: ConsoleColor.Red);

                    char option = Console.ReadKey(true).KeyChar;

                    Console.Clear();

                    switch (option)
                    {
                        case '1':
                            SetColor(false);
                            Console.Title = "RoboDex - List All Contacts";
                            Console.WriteLine(phoneDirectory.Read());
                            Console.ReadKey();
                            break;
                        case '2':
                            SetColor(false);
                            Console.Title = "RoboDex - Search Contacts";
                            Console.CursorVisible = true;
                            UserSearchContacts(ref phoneDirectory);
                            break;
                        case '3':
                            SetColor(false);
                            Console.Title = "RoboDex - Insert New Contact";
                            Console.CursorVisible = true;
                            UserInsertContact(ref phoneDirectory);
                            Console.ReadKey();
                            break;
                        case '4':
                            Console.CursorVisible = true;
                            SetColor(false);
                            Console.Title = "RoboDex - Update Existing Contact";
                            UserGetContactToUpdate(ref phoneDirectory);
                            Console.ReadKey();
                            break;
                        case '5':
                            SetColor(false);
                            Console.CursorVisible = true;
                            Console.Title = "RoboDex - Seed With Random Contacts";
                            UserSeedContacts(ref phoneDirectory);
                            Console.ReadKey();
                            break;
                        case '6':
                            SetColor(false);
                            Console.CursorVisible = true;
                            Console.Title = "RoboDex - Delete Contacts";
                            UserGetContactToDelete(ref phoneDirectory);
                            break;
                        case 'X':
                            runProgram = false;
                            Console.Title = "RoboDex - Exiting...";
                            ExitSequence(ref phoneDirectory);
                            break;
                        case 'x':
                            runProgram = false;
                            Console.Title = "RoboDex - Exiting...";
                            ExitSequence(ref phoneDirectory);
                            break;
                        default:
                            UserDisplayDashboard(ref phoneDirectory);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                // Never let the user see an unhandled exception
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error(e.Message);
                UserDisplayDashboard(ref phoneDirectory);
            }
        }

        private static void UserDeleteContact(ref PhoneDirectory phoneDirectory, List<Contact> contacts)
        {
            int count = 0;

            var logger = NLog.LogManager.GetCurrentClassLogger();

            Console.WriteLine(phoneDirectory.Read(contacts, true));

            PrintRowBorder();

            try
            {
                if (contacts.Count > 1)
                {
                    Console.WriteLine("Please enter the Selection ID of the contact you want to delete. You may also enter a range of IDs in the form #-#. You can also enter '*' to delete all listed contacts.");

                    SwapColor();
                    string inputString = Console.ReadLine();
                    SwapColor();

                    List<Contact> toDelete = new List<Contact>();

                    Regex singleNum = new Regex(@"[0-9]+|");
                    Regex range = new Regex(@"[0-9]+-[0-9]+");

                    // They entered a range
                    if (inputString == "*")
                    {
                        foreach (Contact contact in contacts)
                        {
                            phoneDirectory.Delete(contact);
                            count++;
                        }
                    }
                    else if (range.IsMatch(inputString))
                    {
                        int left;
                        int right;

                        if (!int.TryParse(inputString.Split('-')[0], out left) || !int.TryParse(inputString.Split('-')[1], out right))
                        {
                            throw new ArgumentException($"Couldn't parse input. Received {inputString}.");
                        }

                        left--;
                        right--;

                        // Both values are within the range
                        if (left >= 0 && left < contacts.Count && right >= left && right >= 0 && right < contacts.Count)
                        {
                            for (int i = left; i <= right; i++)
                            {
                                if (phoneDirectory.Delete(contacts.ElementAt(i)))
                                {
                                    count++;
                                }
                            }
                        }
                        else
                        {
                            throw new IndexOutOfRangeException($"Contact IDs were outside the range of available contacts.");
                        }
                    }
                    // They entered a single number
                    else if (singleNum.IsMatch(inputString))
                    {
                        int inputNum;

                        if(!int.TryParse(inputString, out inputNum))
                        {
                            throw new ArgumentException($"Couldn't parse input. Received {inputString}.");
                        }

                        inputNum--;

                        //Value is within range
                        if(inputNum >= 0 && inputNum < contacts.Count)
                        {
                            if (phoneDirectory.Delete(contacts.ElementAt(inputNum)))
                            {
                                count++;
                            }
                        }
                        else
                        {
                            throw new IndexOutOfRangeException($"Contact ID was outside the range of available contacts.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"The input '{inputString}' didn't make any sense in this context. Choose a number from the available Selection IDs.");
                    }
                }
                // There was only one option
                else
                {
                    if (phoneDirectory.Delete(contacts.First()))
                    {
                        count++;
                    }
                }

                if(count == 0)
                {
                    Console.WriteLine("Something went wrong. No contacts were deleted.");
                }
                else if(count == 1)
                {
                    Console.WriteLine("Deleted one contact.");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine($"Deleted {count} contacts.");
                    Console.ReadKey();
                }
            }
            catch (ArgumentException e)
            {
                logger.Warn(e.Message);
                Console.WriteLine(e.Message);
            }
            catch (IndexOutOfRangeException e)
            {
                logger.Warn(e.Message);
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        private static void UserGetContactToDelete(ref PhoneDirectory phoneDirectory)
        {
            UserDeleteContact(ref phoneDirectory, phoneDirectory.GetAll().ToList<Contact>());
        }

        public static void UserSeedContacts(ref PhoneDirectory phoneDirectory)
        {
            Console.WriteLine("How many contacts would you like to create?");

            SwapColor();
            string inputString = Console.ReadLine();
            SwapColor();

            int inputNum;

            Regex regex = new Regex(@"[0-9]+");

            // If they input a number
            if (regex.IsMatch(inputString))
            {
                // Try to parse the text as an integer
                if (int.TryParse(inputString, out inputNum))
                {
                    ContactSeeder.Seed(ref phoneDirectory, inputNum);
                    Console.WriteLine($"Created {inputNum} new random Contacts.");
                }
            }
            else
            {
                Console.WriteLine("You must input a number of contacts to create.");
                Console.ReadKey();
                Console.WriteLine(Environment.NewLine);
                UserSeedContacts(ref phoneDirectory);
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
                    phoneDirectory.Add(contact);
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
            Console.WriteLine(phoneDirectory.Read());
        }

        public static void UserSearchContacts(ref PhoneDirectory phoneDirectory)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            Console.WriteLine("How would you like to search? Enter the option ID.");

            Console.WriteLine("1 - First Name");
            Console.WriteLine("2 - Last Name");
            Console.WriteLine("3 - ZIP");
            Console.WriteLine("4 - City");
            Console.WriteLine("5 - Phone");

            PrintRowBorder();

            SwapColor();
            char searchTypeInput = Console.ReadKey().KeyChar;
            Console.WriteLine(Environment.NewLine);
            SwapColor();
            string searchTermInput;

            while (!char.IsDigit(searchTypeInput))
            {
                Console.WriteLine(RequiredMessage("Numeric Search Type"));
                logger.Error($"Non-number search type");
                SwapColor();
                searchTypeInput = Console.ReadKey().KeyChar;
                Console.WriteLine(Environment.NewLine);
                SwapColor();
            }

            List<Contact> result = new List<Contact>();

            PrintRowBorder();
            Console.WriteLine("You may enter either the exact term you want to search by or you can use '*' as a wildcard anywhere in the search term.");
            Console.WriteLine(Environment.NewLine);

            // Once we know the search type, get the search term
            switch (searchTypeInput)
            {
                case '1':
                    Console.Write("Please enter the first name to search for: ");
                    result = GetSearchTerm(SearchType.firstName, phoneDirectory);
                    break;
                case '2':
                    Console.Write("Please enter the last name to search for: ");
                    result = GetSearchTerm(SearchType.lastName, phoneDirectory);
                    break;
                case '3':
                    Console.Write("Please enter the ZIP or Postal Code to search for: ");
                    result = GetSearchTerm(SearchType.zip, phoneDirectory);
                    break;
                case '4':
                    Console.Write("Please enter the city to search for: ");
                    result = GetSearchTerm(SearchType.city, phoneDirectory);
                    break;
                default:
                    Console.WriteLine($"'{searchTypeInput}' is not a valid search type");
                    logger.Error($"Invalid search type: {searchTypeInput}");
                    Console.Read();
                    UserSearchContacts(ref phoneDirectory);
                    return;
            }

            if(result.Count >= 1)
            {
                Console.WriteLine(phoneDirectory.Read(ref result));

                Console.WriteLine("Do you want to [U]pdate or [D]elete one or more of these contacts? Enter 'U' to update, 'D' to delete, or anything else to exit.");

                SetColor(true);

                char inputChar = char.ToUpper(Console.ReadKey().KeyChar);

                if (inputChar == 'U')
                {
                    Console.WriteLine(Environment.NewLine);
                    SetColor(false);
                    UserGetContactToUpdate(ref phoneDirectory, result);
                }
                else if(inputChar == 'D')
                {
                    Console.WriteLine(Environment.NewLine);
                    SetColor(false);
                    UserDeleteContact(ref phoneDirectory, result);
                }
                else
                {
                    SetColor(false);
                    Console.WriteLine(Environment.NewLine);
                }
            }
            else
            {
                Console.WriteLine($"No contacts found for this search.");
                Console.Read();
            }
        }

        private static void UserGetContactToUpdate(ref PhoneDirectory phoneDirectory)
        {
            UserGetContactToUpdate(ref phoneDirectory, phoneDirectory.GetAll().ToList<Contact>());
        }

        private static void UserGetContactToUpdate(ref PhoneDirectory phoneDirectory, List<Contact> contacts)
        {
            Console.WriteLine(phoneDirectory.Read(contacts, true));

            PrintRowBorder();

            if (contacts.Count > 1)
            {

                Console.WriteLine("Please enter the Selection ID of the contact you want to edit.");

                SwapColor();
                string inputId = Console.ReadLine();
                SwapColor();

                int inputNum;

                Contact contact;

                Regex regex = new Regex(@"[0-9]+");

                // If they input a number
                if (regex.IsMatch(inputId))
                {
                    // Try to parse the text as an integer
                    if (int.TryParse(inputId, out inputNum))
                    {
                        // If the number is within the range of available contacts
                        if (inputNum > 0 && inputNum <= contacts.Count)
                        {
                            contact = contacts.ElementAt(inputNum - 1);
                            UserUpdateContact(ref phoneDirectory, ref contact);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException($"Requested selection ID ({inputNum}) is not valid.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Tried to cast {inputNum} to integer. Failed.");
                    }
                }
            }
            // There was only one option
            else
            {
                Contact contact = contacts.First();
                UserUpdateContact(ref phoneDirectory, ref contact);
            }
        }

        private static void UserUpdateContact(ref PhoneDirectory phoneDirectory, ref Contact contact)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            Console.WriteLine(ToColumns($"[F]irst Name: {contact.FirstName}", $"[L]ast Name: {contact.LastName}"));
            Console.WriteLine(ToColumns($"[P]hone: {contact.Phone}"));
            Console.WriteLine(ToColumns($"[H]ouse Number: {contact.AddressID.HouseNum}", $"[Str]eet: {contact.AddressID.Street}"));
            Console.WriteLine(ToColumns($"[Ci]ty: {contact.AddressID.City}", $"[Z]IP: {contact.AddressID.Zip}"));
            Console.WriteLine(ToColumns($"[Co]untry: {contact.AddressID.CountryCode}", (contact.AddressID.StateCode != State.NA ? $"[Sta]te: {contact.AddressID.StateCode}" : "")));

            PrintRowBorder();

            DoUpdate(ref contact);

            phoneDirectory.Read(contact);

            PrintRowBorder();

            SetColor(false);

            Console.WriteLine("Do you want to update another field? [Y/N]");

            SwapColor();

            if(char.ToUpper(Console.ReadKey().KeyChar) == 'Y')
            {
                SwapColor();
                UserUpdateContact(ref phoneDirectory, ref contact);
            }
            else
            {
                SwapColor();
                if (phoneDirectory.Update(contact))
                {
                    Console.CursorVisible = false;
                    Console.Clear();
                    Console.WriteLine($"{Environment.NewLine}Update to {contact.FirstName} {contact.LastName} complete.{Environment.NewLine}");
                    Console.WriteLine(phoneDirectory.Read(contact));
                }
                else
                {
                    logger.Error($"Update for contact {contact.Pid} failed.");
                    Console.WriteLine("Update failed.");
                }
            }
        }

        private static void DoUpdate(ref Contact contact, string inputString = "")
        {
            if (inputString.Length == 0)
            {
                Console.WriteLine("Please enter part or all of the name of the field you would like to change. Enter 'E[X]it to cancel update.");
                SwapColor();
                inputString += Console.ReadKey().KeyChar.ToString().ToUpper();
                SwapColor();
            }

            Address address = contact.AddressID;

            // Just in case we're entering an invalid search
            if(inputString.Length > 3)
            {
                // It looks like the user is trying to exit
                if (inputString.Contains("X"))
                {
                    return;
                }

                Console.WriteLine($"{Environment.NewLine}There is no field that matches the input '{inputString}'. Try again.");
                DoUpdate(ref contact);
            }

            Regex exit = new Regex(@".*X.*", RegexOptions.IgnoreCase);
            Regex firstName = new Regex(@".*F.*", RegexOptions.IgnoreCase);
            Regex lastName = new Regex(@".*L.*", RegexOptions.IgnoreCase);
            Regex phone = new Regex(@".*P.*", RegexOptions.IgnoreCase);
            Regex houseNum = new Regex(@".*H.*", RegexOptions.IgnoreCase);
            Regex street = new Regex(@".*STR.*", RegexOptions.IgnoreCase);
            Regex city = new Regex(@".*CI.*", RegexOptions.IgnoreCase);
            Regex zip = new Regex(@".*Z.*", RegexOptions.IgnoreCase);
            Regex country = new Regex(@".*CO.*", RegexOptions.IgnoreCase);
            Regex state = new Regex(@".*STA.*", RegexOptions.IgnoreCase);

            if (exit.IsMatch(inputString))
            {
                Console.WriteLine("Canceling update");
                TypeText("...");
                return;
            }
            else if (firstName.IsMatch(inputString))
            {
                do
                {
                    Console.WriteLine(Environment.NewLine + "New First Name: ");
                    SwapColor();
                    contact.FirstName = Console.ReadLine();
                    SwapColor();
                } while (contact.FirstName.Length < 1);
                return;
            }
            else if (lastName.IsMatch(inputString))
            {
                do
                {
                    Console.WriteLine(Environment.NewLine + "New Last Name: ");
                    SwapColor();
                    contact.LastName = Console.ReadLine();
                    SwapColor();
                } while (contact.LastName.Length < 1);
                return;
            }
            else if (phone.IsMatch(inputString))
            {
                do
                {
                    Console.WriteLine(Environment.NewLine + "New Phone Number: ");
                    SwapColor();
                    contact.Phone = Console.ReadLine();
                    SwapColor();
                } while (contact.Phone.Length < 1);
                return;
            }
            else if (houseNum.IsMatch(inputString))
            {
                do
                {
                    Console.WriteLine(Environment.NewLine + "New House Number: ");
                    SwapColor();
                    address.HouseNum = Console.ReadLine();
                    SwapColor();
                } while (address.HouseNum.Length < 1);

                contact.AddressID = address;
                return;
            }
            else if (street.IsMatch(inputString))
            {
                do
                {
                    Console.WriteLine(Environment.NewLine + "New Street: ");
                    SwapColor();
                    address.Street = Console.ReadLine();
                    SwapColor();
                } while (address.Street.Length < 1);

                contact.AddressID = address;
                return;
            }
            else if (city.IsMatch(inputString))
            {
                do
                {
                    Console.WriteLine(Environment.NewLine + "New City: ");
                    SwapColor();
                    address.City = Console.ReadLine();
                    SwapColor();
                } while (address.City.Length < 1);

                contact.AddressID = address;
                return;
            }
            else if (zip.IsMatch(inputString))
            {
                do
                {
                    Console.WriteLine(Environment.NewLine + "New ZIP: ");
                    SwapColor();
                    address.Zip = Console.ReadLine();
                    SwapColor();
                } while (address.Zip.Length < 1);

                contact.AddressID = address;
                return;
            }
            else if (country.IsMatch(inputString))
            {
                Console.WriteLine(Environment.NewLine + "New Country: ");
                SwapColor();
                address.CountryCode = (Country)Enum.Parse(typeof(Country), Console.ReadLine());
                SwapColor();
                contact.AddressID = address;
                return;
            }
            else if (state.IsMatch(inputString))
            {
                Console.WriteLine(Environment.NewLine + "New State: ");
                SwapColor();
                address.StateCode = (State)Enum.Parse(typeof(State), Console.ReadLine());
                SwapColor();
                contact.AddressID = address;
                return;
            }
            else
            {
                SwapColor();
                inputString += Console.ReadKey().KeyChar.ToString();
                SwapColor();
                DoUpdate(ref contact, inputString);
                return;
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
                return phoneDirectory.Search(searchType, searchTermInput).ToList<Contact>();
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

        /// <summary>
        /// The processes that run when the program exits
        /// </summary>
        private static void ExitSequence(ref PhoneDirectory phoneDirectory)
        {
            phoneDirectory.Save();
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            string message = "Goodbye!";

            TypeText(message, 100, ConsoleColor.Yellow);

            Thread.Sleep(1000);
        }

        /// <summary>
        /// Pads the input text to be equal width and fit withing the console
        /// </summary>
        /// <param name="column1"></param>
        /// <param name="column2"></param>
        /// <returns></returns>
        private static string ToColumns(string column1 = "", string column2 = "")
        {
            column1 = column1.PadLeft(Console.WindowWidth / 3);
            column2 = column2.PadRight(Console.WindowWidth / 3);
            int padding = Console.WindowWidth - (column1.Length + column2.Length);
            return column1 + new string(' ', padding) + column2;
        }

        /// <summary>
        /// Pads the input text equally on both sides
        /// </summary>
        /// <param name="toCenter"></param>
        /// <returns></returns>
        internal static string Center(string toCenter)
        {
            int length = toCenter.Length;
            int padding = (Console.WindowWidth - length) / 2;

            return new string(' ', padding) + toCenter + new string(' ', padding);
        }

        /// <summary>
        /// Converts the input text to Proper/Title case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string ToProper(string text)
        {
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(text.ToLower());
        }

        /// <summary>
        /// Sets the color of the text in the console and sets isInput to match
        /// </summary>
        /// <param name="input">True: set to input color, False: Set to output color</param>
        public static void SetColor(bool input)
        {
            isInput = input;
            SetColor();
        }

        /// <summary>
        /// Sets the console color based on the current value of isInput
        /// </summary>
        public static void SetColor()
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

        /// <summary>
        /// Toggles between the input and output console text colors
        /// </summary>
        public static void SwapColor()
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

        /// <summary>
        /// Writes the specified text to the console with an animated effect
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="speed">Time between letters, in milliseconds</param>
        public static void TypeText(string text, int speed, ConsoleColor leftColor, ConsoleColor rightColor)
        {
            Console.ForegroundColor = leftColor;

            for (int i = 0; i < text.Length; i++)
            {
                // Special handling for exit option
                if(i > 1 && (text[i] == 'X'))
                {
                    Console.ForegroundColor = rightColor;
                }

                Console.Write(text[i]);

                if (text[i] != ' ') { Thread.Sleep(speed); }
            }

            Console.Write(Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Blue;
        }

        /// <summary>
        /// Writes the specified text to the console with an animated effect
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <param name="speed">Time between letters, in milliseconds</param>
        public static void TypeText(string text, int speed = 5, ConsoleColor color = ConsoleColor.Green)
        {
            TypeText(text, speed, color, color);
        }
    }
}
