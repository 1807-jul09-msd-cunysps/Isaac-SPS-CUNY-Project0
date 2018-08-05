using System;

namespace PhoneDirectoryLibrary
{
    public struct Email
    {
        public Guid Pid { get; }
        public string EmailAddress { get; set; }
        public Guid ContactID { get; set; }

        public Email(Guid pid, string EmailAddress, Guid ContactID)
        {
            Pid = pid;
            this.EmailAddress = EmailAddress ?? throw new ArgumentNullException(nameof(EmailAddress));
            this.ContactID = ContactID;
        }

        public Email(string EmailAddress, Guid ContactID)
        {
            this.Pid = Guid.NewGuid();
            this.EmailAddress = EmailAddress ?? throw new ArgumentNullException(nameof(EmailAddress));
            this.ContactID = ContactID;
        }
    }
}