using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace PhoneDirectoryLibrary
{
    public class PhoneDirectory
    {
        private HashSet<Contact> contacts;
        private string dataFilePath = Path.ChangeExtension(Path.Combine("C:\\Dev","directory"),"json");

        public PhoneDirectory()
        {
            contacts = new HashSet<Contact>();
        }

        public PhoneDirectory(HashSet<Contact> contacts)
        {
            this.contacts = contacts ?? throw new ArgumentNullException(nameof(contacts));
        }

        public string DataPath()
        {
            return dataFilePath;
        }

        public int count()
        {
            return contacts.Count();
        }

        public void add(Contact contact)
        {
            contacts.Add(contact);
        }

        //Returns true if item was deleted, false otherwise
        public bool delete(string Pid)
        {
            foreach (var contact in contacts)
            {
                if(contact.Pid == Pid)
                {
                    contacts.Remove(contact);
                    return true;
                }
            }

            return false;
        }


        // Replace an existing contact with an updated version
        // Returns true if an update was made, false if object not found
        public bool update(Contact contact)
        {
            foreach(var c in contacts)
            {
                if(c.Pid == contact.Pid)
                {
                    contacts.Remove(c);
                    return true;
                }
            }

            return false;
        }

        public Contact search(SearchType type, string searchTerm)
        {
            Contact result;
            switch (type)
            {
                case SearchType.firstName:
                    result = (Contact)contacts.Where(query => (query.firstName).Contains(searchTerm)).First();
                    break;
                case SearchType.lastName:
                    result = (Contact)contacts.Where(query => (query.lastName).Contains(searchTerm)).First();
                    break;
                case SearchType.zip:
                    result = (Contact)contacts.Where(query => (query.address.zip).Contains(searchTerm)).First();
                    break;
                case SearchType.city:
                    result = (Contact)contacts.Where(query => (query.address.city).Contains(searchTerm)).First();
                    break;
                case SearchType.phone:
                    result = (Contact)contacts.Where(query => (query.phone).Contains(searchTerm)).First();
                    break;
                default:
                    return null;
            }

            return result;
        }

        public void save()
        {
            string jsonData = "";
            foreach(Contact contact in contacts)
            {
                jsonData += JsonConvert.SerializeObject(contact,Formatting.Indented);
            }

            File.WriteAllText(DataPath(),jsonData);
        }

        public enum SearchType
        {
            firstName,
            lastName,
            zip,
            city,
            phone
        }
    }
}
