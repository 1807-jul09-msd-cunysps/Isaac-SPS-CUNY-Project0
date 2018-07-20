using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public class Contact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Address address { get; set; }
        public string phone;
        public string Pid { get; }

        public Contact(string firstName, string lastName, Address address, string phone)
        {
            this.firstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            this.lastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            this.address = address;
            this.phone = phone ?? throw new ArgumentNullException(nameof(phone));
            this.Pid = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the column widths for each field in the contact
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> ColumnWidths()
        {
            Dictionary<string, int> widths = new Dictionary<string, int>();

            widths.Add("First Name", firstName.Length);
            widths.Add("Last Name", lastName.Length);
            widths.Add("Phone", phone.Length);

            //Get address column widths
            foreach (var column in address.ColumnWidths())
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
                Utilities.AddToDict(ref columns, "First Name", firstName, columnWidths);
                Utilities.AddToDict(ref columns, "Last Name", lastName, columnWidths);
                Utilities.AddToDict(ref columns, "Phone", phone, columnWidths);

                // Add the address fields
                foreach (var column in address.ToRow(columnWidths))
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
            columns.Add("First Name", firstName);
            columns.Add("Last Name", lastName);
            columns.Add("Phone", phone);

            foreach (var column in address.ToRow())
            {
                columns.Add(column.Key, column.Value);
            }

            return columns;
        }
    }
}
