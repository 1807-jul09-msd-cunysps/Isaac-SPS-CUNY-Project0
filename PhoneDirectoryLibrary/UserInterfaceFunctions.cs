using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace PhoneDirectoryLibrary
{
    public static class UserInterfaceFunctions
    {
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
                firstName = Console.ReadLine();
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
                lastName = Console.ReadLine();
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
                phone = Console.ReadLine();
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

                    string countryInput = Console.ReadLine();
                    PrintRowBorder();

                    // If they enter one character they are probably asking for help
                    if (countryInput.Length == 1)
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

                        string stateInput = Console.ReadLine().Trim();
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

                    city = Console.ReadLine();
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

                    zip = Console.ReadLine();
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

                    street = Console.ReadLine();
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

                    houseNum = Console.ReadLine();
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

        private static void PrintRowBorder(int count = 40)
        {
            Console.WriteLine(new String('—', count));
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

        private static string ToProper(string text)
        {
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(text.ToLower());
        }
    }
}
