using Newtonsoft.Json;
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
        public Guid Pid { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Address> Addresses { get; set; }
        public int GenderID { get; set; }
        public List<Phone> Phones { get; set; }
        public List<Email> Emails { get; set; }
        public short Age { get; set; }

        public Contact(string FirstName, string LastName, List<Email> Emails) : this(FirstName, LastName, 0, new List<Address>(), 3, Emails, new List<Phone>())
        {
            // Minimal constructor
        }

        public Contact(Guid Pid, string FirstName, string LastName, short Age, IEnumerable<Address> Addresses, int GenderID, IEnumerable<Email> Emails, IEnumerable<Phone> Phones) : this(FirstName, LastName, Age, Addresses, GenderID, Emails, Phones)
        {
            this.Pid = Pid;
        }

        [JsonConstructor]
        public Contact(string FirstName, string LastName, short Age, IEnumerable<Address> Addresses, int GenderID, IEnumerable<Email> Emails, IEnumerable<Phone> Phones)
        {
            this.FirstName = FirstName ?? throw new ArgumentNullException(nameof(FirstName));
            this.LastName = LastName ?? throw new ArgumentNullException(nameof(LastName));
            this.Addresses = Addresses.ToList<Address>();
            this.Age = Age;

            if(GenderID < 0 || GenderID > 2)
            {
                throw new ArgumentException("Gender parameter out of range.");
            }
            else
            {
                this.GenderID = GenderID;
            }

            this.Emails = Emails.ToList<Email>();
            this.Phones = Phones.ToList<Phone>();

            this.Pid = System.Guid.NewGuid();
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
        /// Does a deep comparison Contact
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="deep"></param>
        /// <returns></returns>
        public bool Equals(object obj, bool deep)
        {
            if (deep)
            {
                if (obj is Contact)
                {
                    Contact toCompare = (Contact)obj;

                    if(
                        this.Pid != toCompare.Pid ||
                        this.FirstName != toCompare.FirstName ||
                        this.LastName != toCompare.LastName ||
                        this.GenderID != toCompare.GenderID
                      )
                    {
                        return false;
                    }

                    if(this.Addresses.Count != toCompare.Addresses.Count)
                    {
                        return false;
                    }

                    for (int i = 0; i < Addresses.Count; i++)
                    {
                        if(this.Addresses.ElementAt(i) != toCompare.Addresses.ElementAt(i))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Do a shallow comparison
            else
            {
                return this.Equals(obj);
            }
        }

        /// <summary>
        /// Creates a very simple hash using just the Pid
        /// Even if content changes, Contacts are compared based just on their ID
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -1565936374;
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Pid);

            return hashCode;
        }

        /// <summary>
        /// Overrides equality operator to do a shallow comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Contact left, Contact right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }
            else
            {
                return left.Equals(right);
            }
        }

        /// <summary>
        /// Overrides inequality operator to do a shallow comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Contact left, Contact right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return !object.ReferenceEquals(right, null);
            }
            else
            {
                return !left.Equals(right);
            }
        }

        private static string CleanToDigits(string text)
        {
            Regex justDigits = new Regex(@"[^\d]");
            return justDigits.Replace(text, "");
        }
    }
}
