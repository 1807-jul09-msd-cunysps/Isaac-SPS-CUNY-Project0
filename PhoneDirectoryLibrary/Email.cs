using System;

namespace PhoneDirectoryLibrary
{
    public struct Email
    {
        public Guid Pid { get; }
        public string EmailAddress { get; set; }

        public Email(Guid pid, string EmailAddress)
        {
            Pid = pid;
            this.EmailAddress = EmailAddress ?? throw new ArgumentNullException(nameof(EmailAddress));
        }

        public Email(string EmailAddress)
        {
            this.Pid = Guid.NewGuid();
            this.EmailAddress = EmailAddress ?? throw new ArgumentNullException(nameof(EmailAddress));
        }
    }
}