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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public Message(Guid Pid, string messageText, string FirstName, string LastName, string Email, DateTime Received) : this(messageText, FirstName, LastName, Email)
        {
            this.Pid = Pid;
            this.Received = Received;
        }

        public Message(Guid Pid, string messageText, string FirstName, string LastName, string Email) : this(messageText, FirstName, LastName, Email)
        {
            this.Pid = Pid;
        }

        public Message(string messageText, string FirstName, string LastName, string Email)
        {
            this.Pid = Guid.NewGuid();
            this.MessageText = messageText ?? throw new ArgumentNullException(nameof(messageText));
            this.Received = DateTime.Now;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
        }

        public Guid InsertMessage()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            using(var connection = new SqlConnection(PhoneDirectory.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string messageCommandString = "INSERT INTO ContactMessages (Pid, MessageText, FirstName, LastName, Email, Received) VALUES (@Pid, @MessageText, @FirstName, @LastName, @Email, @Received)";
                    SqlCommand messageCommand = new SqlCommand(messageCommandString, connection);

                    //Add values for message
                    messageCommand.Parameters.AddWithValue("@Pid", (this.Pid == Guid.Empty ? Guid.NewGuid() : this.Pid));
                    messageCommand.Parameters.AddWithValue("@MessageText", this.MessageText);
                    messageCommand.Parameters.AddWithValue("@FirstName", this.FirstName);
                    messageCommand.Parameters.AddWithValue("@LastName", this.LastName);
                    messageCommand.Parameters.AddWithValue("@Email", this.Email);
                    messageCommand.Parameters.AddWithValue("@Received", DateTime.Now);

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
        public static List<Message> Get()
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
                            Received,
                            FirstName,
                            LastName,
                            Email                            
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
                                messageReader.GetString(3),
                                messageReader.GetString(4),
                                messageReader.GetString(5),
                                messageReader.GetDateTime(2)
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
                            Received,
                            FirstName,
                            LastName,
                            Email                            
                        FROM ContactMessages WHERE ContactID = @ContactID";

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
                                messageReader.GetString(2),
                                messageReader.GetString(3),
                                messageReader.GetString(4),
                                messageReader.GetDateTime(5)
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
                   Received == other.Received;
        }

        public override int GetHashCode()
        {
            var hashCode = 1051846227;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MessageText);
            hashCode = hashCode * -1521134295 + Received.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Message left, Message right) =>
            left.MessageText == right.MessageText;

        public static bool operator !=(Message left, Message right) =>
            left.MessageText != right.MessageText;


    }
}
