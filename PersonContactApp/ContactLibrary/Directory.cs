using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ContactLibrary
{
    public class ContactDirectory
    {
        List<Contact> contacts;
        protected internal static string rootPath = Directory.GetCurrentDirectory();

        public ContactDirectory()
        {
            contacts = new List<Contact>();
        }

        public ContactDirectory(List<Contact> contacts)
        {
            this.contacts = new List<Contact>();
            this.contacts.AddRange(contacts);
        }

        /*
        public void InputContact()
        {
            string firstName;
            string lastName;
            Country countryCode = Country.United_States;
            string areaCode;
            string number;
            string extension;
            Address[] addresses;
            Phone phone;

            Console.WriteLine("Creating a new contact..." + Environment.NewLine);

            Console.WriteLine("Let's start with the person's name.");

            // We do little mini loops here in order to ensure we collect these fields
            do
            {
                Console.WriteLine("What is their first name?");
                firstName = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(firstName));

            do
            {
                Console.WriteLine("What is their last name?");
                lastName = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(lastName));

            do
            {
                Console.WriteLine("What is their phone number? This doesn't include any international dialing codes or extensions. We'll ask about those in a second.");
                number = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(number));

            do
            {
                Console.WriteLine("What country is this phone number from? You can either enter the country code or press 'h' or '?' to see a list of possible codes.");
                number = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(number));

            Console.WriteLine("Does their phone number have an area code? If so, go ahead and enter it. If not, just press [Enter].");
            areaCode = Console.ReadLine();

            Console.WriteLine("Does their phone number have an extension? If so, go ahead and enter it. If not, just press [Enter].");
            extension = Console.ReadLine();

            phone = new Phone(number, countryCode, areaCode, extension);

            Console.WriteLine("Okay, that's it for the phone number. Now we'll get thier address(s).");

            bool cont = true;
            int count = 1;
            string title;
            string country;
            string address;
            string city;
            string zip;

            // Collect one or more addresses
            while (cont)
            {
                if (count == 1)
                {
                    Console.WriteLine("Let's start with their primary address.");
                }
                else
                {
                    Console.WriteLine($"Now we'll do address number {count}.");
                }

                Console.WriteLine("Is there a title or name for this address? If so, go ahead and enter it. If not, just press [Enter]");
                title = Console.ReadLine();

                //@TODO Make this either a select, default to USA, or be clearer about format -- if I'm using enums, fix it
                do
                {
                    Console.WriteLine("What country is this address in?");
                    country = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(country));

                do
                {
                    Console.WriteLine("What city is the address in?");
                    city = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(city));

                do
                {
                    Console.WriteLine("What is the text of the address? This is probably a house number and street name, but there might be an apartment or suite number too. Enter everything on one line.");
                    address = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(address));

                do
                {
                    Console.WriteLine("What is the ZIP or postal code for the address?");
                    zip = Console.ReadLine();
                } while (string.IsNullOrWhiteSpace(zip));

                Address tempAddress = new Address(country, address, city, zip, title);

                Console.WriteLine($"Great, here's the address you entered:" + Environment.NewLine + $" {tempAddress.ToString()}");

                Console.WriteLine("Does that look right? Y/N");
                if (Char.ToUpper(Console.ReadKey().KeyChar) == 'Y')
                {
                    // If the address is correct, increment the address count
                    count++;
                    addresses.Add(tempAddress);
                    Console.WriteLine("Great! Is there another address?" + Environment.NewLine);
                    if (Char.ToUpper(Console.ReadKey().KeyChar) == 'N')
                    {
                        Console.WriteLine(Environment.NewLine);
                        cont = false;
                    }
                }
                else
                {
                    // The address was entered incorrectly
                    Console.WriteLine(Environment.NewLine + "Okay, we'll try again.");
                }
            }

            Contact contact = new Contact(firstName, lastName, phone, addresses);
        }
        */
    }
}
