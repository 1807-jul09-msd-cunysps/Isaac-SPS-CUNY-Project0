using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public struct Address : IEquatable<Address>
    {
        public Guid Pid { get; set; }
        public string Street { get; set; }
        public string HouseNum { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public State StateCode { get; set; }
        public Country CountryCode { get; set; }

        public Address(string street, string houseNum, string city, string zip, Country country, State state = State.NA)
        {
            this.Street = street ?? throw new ArgumentNullException(nameof(street));
            this.HouseNum = houseNum ?? throw new ArgumentNullException(nameof(houseNum));
            this.City = city ?? throw new ArgumentNullException(nameof(city));
            this.Zip = zip ?? throw new ArgumentNullException(nameof(zip));

            if(state != State.NA && country != Country.United_States)
            {
                throw new InvalidAddressFieldException($"State must be NA or left blank if country is not United States. Received: {country}.");
            }
            else
            {
                this.StateCode = state;
                this.CountryCode = country;
            }

            Pid = System.Guid.NewGuid();
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
                Utilities.AddToDict(ref columns, "City", City, columnWidths);
                Utilities.AddToDict(ref columns, "ZIP", Zip, columnWidths);
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

        /// <summary>
        /// Converts the address to a string ready for output as a row
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToRow()
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();
            columns.Add("Address", AddressLineOne());
            columns.Add("City", City);
            columns.Add("ZIP", Zip);

            return columns;
        }

        /// <summary>
        /// Combines the house number and street into a single string
        /// </summary>
        /// <returns></returns>
        private string AddressLineOne()
        {
            return HouseNum + ", " + Street;
        }

        /// <summary>
        /// Calculates the column widths for this address instance
        /// </summary>
        /// <returns>A dictionary of column names and column widths</returns>
        public Dictionary<string, int> ColumnWidths()
        {
            Dictionary<string, int> widths = new Dictionary<string, int>();

            widths.Add("Address", AddressLineOne().Length);
            widths.Add("City", City.Length);
            widths.Add("ZIP", Zip.Length);

            return widths;
        }

        public class InvalidAddressFieldException : ArgumentException
        {
            public InvalidAddressFieldException()
            {
                //
            }

            public InvalidAddressFieldException(string message) : base(message)
            {
                //
            }
        }

        public override string ToString()
        {
            return $"{HouseNum} {Street}, {City} {(StateCode != State.NA ? Lookups.StateNames[StateCode] : "")}";
        }

        public override bool Equals(object obj)
        {
            return obj is Address && ((Address)obj).Pid == this.Pid;
        }

        public bool Equals(object obj, bool deep)
        {
            if (deep)
            {
                if(obj is Address)
                {
                    Address toCompare = (Address)obj;
                    if (
                        this.Pid != toCompare.Pid ||
                        this.Street != toCompare.Street ||
                        this.HouseNum != toCompare.HouseNum ||
                        this.City != toCompare.City ||
                        this.Zip != toCompare.Zip ||
                        this.StateCode != toCompare.StateCode ||
                        this.CountryCode != toCompare.CountryCode
                      )
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return this.Equals(obj);
            }
        }

        public bool Equals(Address other)
        {
            return Pid == other.Pid &&
                   Street == other.Street &&
                   HouseNum == other.HouseNum &&
                   City == other.City &&
                   Zip == other.Zip &&
                   StateCode == other.StateCode &&
                   CountryCode == other.CountryCode;
        }

        public override int GetHashCode()
        {
            var hashCode = -1565936374;
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Pid);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Street);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HouseNum);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(City);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Zip);
            hashCode = hashCode * -1521134295 + StateCode.GetHashCode();
            hashCode = hashCode * -1521134295 + CountryCode.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Overrides equals operator to do a deep comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Address left, Address right) =>
            left.Equals(right, true);

        /// <summary>
        /// Overrides not-equal operator to do a deep comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Address left, Address right) =>
            !left.Equals(right, true);
    }
}
