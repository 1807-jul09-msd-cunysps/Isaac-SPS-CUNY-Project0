using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address AddressID { get; set; }
        public string Phone { get; set; }
        public string Pid { get; }

        public Contact(string firstName, string lastName, Address address, string phone)
        {
            this.FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            this.LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            this.AddressID = address;

            phone = CleanToDigits(phone ?? throw new ArgumentNullException(nameof(phone)));

            if(phone.Length > 25)
            {
                throw new ArgumentOutOfRangeException($"Phone number is {phone.Length} numbers long. That's too long.");
            }
            else
            {
                this.Phone = phone;
            }

            this.Pid = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the column widths for each field in the contact
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> ColumnWidths()
        {
            Dictionary<string, int> widths = new Dictionary<string, int>();

            widths.Add("First Name", FirstName.Length);
            widths.Add("Last Name", LastName.Length);
            widths.Add("Phone", Phone.Length);

            //Get address column widths
            foreach (var column in AddressID.ColumnWidths())
            {
                widths.Add(column.Key, column.Value);
            }

            return widths;
        }

        /// <summary>
        /// Returns a string representation of this Contact padded to the specified column width for the given column
        /// </summary>
        /// <param name="columnWidth"></param>
        /// <returns>A dictionary of all the values in this contact as strings padded to the column width</returns>
        public Dictionary<string,string> ToRow(Dictionary<string, int> columnWidths)
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();
            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                Utilities.AddToDict(ref columns, "First Name", FirstName, columnWidths);
                Utilities.AddToDict(ref columns, "Last Name", LastName, columnWidths);
                Utilities.AddToDict(ref columns, "Phone", Phone, columnWidths);

                // Add the address fields
                foreach (var column in AddressID.ToRow(columnWidths))
                {
                    columns.Add(column.Key,column.Value);
                }

                return columns;
            }
            catch(KeyNotFoundException e)
            {
                logger.Error($"Could not convert Contact {Pid} to a column. {e.Message}");
            }
            catch(ArgumentOutOfRangeException e)
            {
                logger.Error(e.Message);
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
            }

            return new Dictionary<string, string>() { { "", "" } };
        }

        /// <summary>
        /// Returns a string representation of this Contact without special padding
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToRow()
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();
            columns.Add("First Name", FirstName);
            columns.Add("Last Name", LastName);
            columns.Add("Phone", Phone);

            foreach (var column in AddressID.ToRow())
            {
                columns.Add(column.Key, column.Value);
            }

            return columns;
        }

        /// <summary>
        /// Checks equality based only on the id of the Contact
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Contact && ((Contact)obj).Pid == this.Pid;
        }

        /// <summary>
        /// Creates a very simple hash using just the Pid
        /// Even if content changes, Contacts are compared based just on their ID
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = 0;

            foreach (char c in this.Pid.ToCharArray())
            {
                hashCode += (int)c;
            }

            return hashCode;
        }

        private static string CleanToDigits(string text)
        {
            Regex justDigits = new Regex(@"[^\d]");
            return justDigits.Replace(text, "");
        }
    }
}
