﻿using System;
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
            try
            {
                this.contacts = contacts ?? throw new ArgumentNullException(nameof(contacts));
            }
            catch(Exception e)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error(e.Message);
            }
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
            try
            {
                if(contact == null)
                {
                    throw new ArgumentNullException("Cannot add a null contact.");
                }
                contacts.Add(contact);
            }            
            catch(Exception e)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error(e.Message);
            }
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
                    throw new InvalidSearchTermException($"{type.ToString()} is not a valid search term.");
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

        
        /// <summary>
        /// Returns a pretty printed string representing the specified contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public string read(Contact contact)
        {
            string headers = "|";
            string columns = "|";
            var contactRow = contact.ToRow(MaxWidths(new List<Contact> { contact }));

            foreach (var header in contactRow.Keys)
            {
                headers += header + '|';
            }

            headers += Environment.NewLine + new string('-', headers.Length) + Environment.NewLine;

            foreach (var value in contactRow.Values)
            {
                columns += value + '|';
            }

            columns += Environment.NewLine;

            return headers + columns;
        }

        /// <summary>
        /// Returns a pretty-printed string representing all contacts in the collection on the console
        /// </summary>
        /// <returns></returns>
        public string read()
        {
            string headers = "|";

            foreach (var header in contacts.First<Contact>().ToRow(MaxWidths(this.contacts)).Keys)
            {
                headers += header + '|';
            }

            // Adds a newline and a border under the headers
            headers += Environment.NewLine + new string('-',headers.Length) + Environment.NewLine;

            string columns = "";

            foreach (Contact contact in contacts)
            {
                columns += '|';

                foreach (var value in contact.ToRow(MaxWidths(this.contacts)).Values)
                {
                    columns += value + '|';
                }

                columns += Environment.NewLine;
            }

            return headers + columns;
        }

        private Dictionary<string, int> MaxWidths(IEnumerable<Contact> contacts)
        {
            Dictionary<string, int> maxWidths = new Dictionary<string, int>();

            // Default the widths to the header widths in case headers are the widest
            foreach (var header in contacts.First().ToRow().Keys)
            {
                maxWidths.Add(header, header.Length);
            }

            // Loop through contacts to get the maximum width for each column
            foreach (Contact contact in contacts)
            {
                foreach (var field in contact.ColumnWidths())
                {
                    if(maxWidths[field.Key] < field.Value)
                    {
                        maxWidths[field.Key] = field.Value;
                    }
                }
            }

            return maxWidths;
        }

        public enum SearchType
        {
            firstName,
            lastName,
            zip,
            city,
            phone
        }

        public class InvalidSearchTermException : Exception
        {
            public InvalidSearchTermException(string message) : base(message) { }

            public InvalidSearchTermException() : base() { }
        }
    }
}