using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public struct Message : IEquatable<Message>
    {
        public Guid Pid { get; }
        public string MessageText { get; set; }
        public DateTime Received { get; }
        public Guid ContactID { get; set; }

        public Message(Guid Pid, string MessageText, Guid ContactID, DateTime Received) : this(MessageText, ContactID)
        {
            this.Pid = Pid;
            this.Received = Received;
        }

        public Message(Guid Pid, string MessageText, Guid ContactID) : this(MessageText, ContactID)
        {
            this.Pid = Pid;
        }

        public Message(string messageText, Guid ContactID)
        {
            this.Pid = Guid.NewGuid();
            this.MessageText = messageText ?? throw new ArgumentNullException(nameof(messageText));
            this.Received = DateTime.Now;
            this.ContactID = ContactID;
        }

        public Guid InsertMessage()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            using(var connection = new SqlConnection(PhoneDirectory.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string messageCommandString = "INSERT INTO ContactMessages (Pid, MessageText, ContactID, Received) VALUES (@Pid, @MessageText, @ContactID, @Received)";
                    SqlCommand messageCommand = new SqlCommand(messageCommandString, connection);

                    //Add values for message
                    messageCommand.Parameters.AddWithValue("@Pid", this.Pid);
                    messageCommand.Parameters.AddWithValue("@MessageText", this.MessageText);
                    messageCommand.Parameters.AddWithValue("@ContactID", this.ContactID);
                    messageCommand.Parameters.AddWithValue("@Received", this.Received);

                    if (messageCommand.ExecuteNonQuery() != 0)
                    {
                        return this.Pid;
                    }
                    else
                    {
                        return Guid.Empty;
                    }
                }
                catch (SqlException e)
                {
                    logger.Error(e.Message);
                    return Guid.Empty;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return Guid.Empty;
                }
            }   
        }

        /// <summary>
        /// Get all the messages from all contacts
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Contact, List<Message>> Get()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            var phoneDirectory = new PhoneDirectory();

            using (var connection = new SqlConnection(PhoneDirectory.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string messageCommandString = @"
                        SELECT 
                            Pid,
                            MessageText,
                            ContactID,
                            Received
                        FROM ContactMessages";
                    SqlCommand messageCommand = new SqlCommand(messageCommandString, connection);

                    var messageReader = messageCommand.ExecuteReader();

                    List<Message> messages = new List<Message>();

                    using (messageReader)
                    {
                        while (messageReader.Read())
                        {
                            messages.Add(new Message(
                                messageReader.GetGuid(0),
                                messageReader.GetString(1),
                                messageReader.GetGuid(2),
                                messageReader.GetDateTime(3)
                                ));
                        }
                    }

                    Dictionary<Contact, List<Message>> contacts = new Dictionary<Contact, List<Message>>();

                    foreach (Message message in messages)
                    {
                        Contact contact = phoneDirectory.GetContactFromDB(message.ContactID);
                        if (contacts.ContainsKey(contact))
                        {
                            contacts[contact].Add(message);
                        }
                        else
                        {
                            contacts.Add(contact, new List<Message>() { message });
                        }
                    }

                    return contacts;
                }
                catch (SqlException e)
                {
                    logger.Error(e.Message);
                    return new Dictionary<Contact, List<Message>>();
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return new Dictionary<Contact, List<Message>>();
                }
            }
        }

        /// <summary>
        /// Gets all messages for a given contact
        /// </summary>
        /// <param name="ContactID"></param>
        /// <returns></returns>
        public static IEnumerable<Message> Get(Guid ContactID)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            using (var connection = new SqlConnection(PhoneDirectory.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string messageCommandString = @"
                        SELECT 
                            Pid,
                            MessageText,
                            ContactID,
                            Received
                        FROM Message WHERE ContactID = @ContactID";
                    SqlCommand messageCommand = new SqlCommand(messageCommandString, connection);

                    //Add values for message
                    messageCommand.Parameters.AddWithValue("@ContactID", ContactID);

                    var messageReader = messageCommand.ExecuteReader();

                    List<Message> messages = new List<Message>();

                    using (messageReader)
                    {
                        while (messageReader.Read())
                        {
                            messages.Add(new Message(
                                messageReader.GetGuid(0),
                                messageReader.GetString(1),
                                messageReader.GetGuid(2),
                                messageReader.GetDateTime(3)
                                ));
                        }
                    }

                    return messages;
                }
                catch (SqlException e)
                {
                    logger.Error(e.Message);
                    return new List<Message>();
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return new List<Message>();
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Message && Equals((Message)obj);
        }

        public bool Equals(Message other)
        {
            return MessageText == other.MessageText &&
                   Received == other.Received &&
                   ContactID.Equals(other.ContactID);
        }

        public override int GetHashCode()
        {
            var hashCode = 1051846227;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MessageText);
            hashCode = hashCode * -1521134295 + Received.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(ContactID);
            return hashCode;
        }

        public static bool operator ==(Message left, Message right) =>
            left.MessageText == right.MessageText;

        public static bool operator !=(Message left, Message right) =>
            left.MessageText != right.MessageText;


    }
}
