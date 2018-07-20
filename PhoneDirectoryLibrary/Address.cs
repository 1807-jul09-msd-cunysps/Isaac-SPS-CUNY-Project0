using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public struct Address
    {
        public string street { get; set; }
        public string houseNum { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string Pid;

        public Address(string street, string houseNum, string city, string zip) : this()
        {
            this.street = street ?? throw new ArgumentNullException(nameof(street));
            this.houseNum = houseNum ?? throw new ArgumentNullException(nameof(houseNum));
            this.city = city ?? throw new ArgumentNullException(nameof(city));
            this.zip = zip ?? throw new ArgumentNullException(nameof(zip));
            Pid = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Returns a string representation of this Address padded to the specified column width for the given column
        /// </summary>
        /// <param name="columnWidths"></param>
        /// <returns>A dictionary of all the values in this address as strings padded to the column width</returns>
        public Dictionary<string, string> ToRow(Dictionary<string, int> columnWidths)
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();

            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                Utilities.AddToDict(ref columns, "Address", AddressLineOne(), columnWidths);
                Utilities.AddToDict(ref columns, "City", city, columnWidths);
                Utilities.AddToDict(ref columns, "ZIP", zip, columnWidths);
            }
            catch (KeyNotFoundException e)
            {
                logger.Error($"Could not convert Contact {Pid} to a column. {e.Message}");
            }
            catch (ArgumentOutOfRangeException e)
            {
                logger.Error(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return columns;
        }

        public Dictionary<string, string> ToRow()
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();
            columns.Add("Address", AddressLineOne());
            columns.Add("City", city);
            columns.Add("ZIP", zip);

            return columns;
        }

        private string AddressLineOne()
        {
            return houseNum + ", " + street;
        }

        public Dictionary<string, int> ColumnWidths()
        {
            Dictionary<string, int> widths = new Dictionary<string, int>();

            widths.Add("Address", AddressLineOne().Length);
            widths.Add("City", city.Length);
            widths.Add("ZIP", zip.Length);

            return widths;
        }
    }
}
