using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types
using System.Data.Common;
using System.Text;


namespace SQMessageFactory
{
    public class StorageQueueFactory
    {
        // Backing variable for queue client connection
        CloudQueueClient queueClient;

        // Backing variable for queue that this instance of the factory is connected to
        CloudQueue queue;

        // Backing variable for message context, used to delete a message that has been peeked
        CloudQueueMessage messageContext;
        
        // Return Name of connected queue
        public string QueueName
        {
            get { return queue.Name; }
        }

        public StorageQueueFactory(string StorageConnectionString, string QueueName)
        {
            // Parse the connection string and return a reference to the Storage Account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a named queue
            queue = queueClient.GetQueueReference(QueueName);

            // Create the queue if it doesn't already exist within the Storage Account
            queue.CreateIfNotExists();
        }

        #region Send and Receive String Messages
            // Send a String as a Message
            public void SendString(string Message)
            {
                //ToDo handle throttling exception and re throw others
                queue.AddMessage(new CloudQueueMessage(Message));
            }

            // Reads the oldest message from the queue and returns the body as a string, deleting the message from the queue
            public string DequeueString()
            {
                var messageContext = queue.GetMessage();
                queue.DeleteMessage(messageContext);
                return messageContext.AsString;
            }

            // Reads the oldest message from the queue and returns the body as a string, 
            // locking the message for the default lock time without deleting the message.
            // You must call delete message on this class within the lock time period or the message will be re-queued
            public string PeekString()
            {
                messageContext = queue.PeekMessage();
                return messageContext.AsString;
            }

            public void DeleteMessage()
            {
                if (!(messageContext == null))
                {
                    queue.DeleteMessage(messageContext);
                    messageContext = null;
                }
            }
        #endregion

        #region Send and Receive Object as a Byte Array
            // Serializes any serializable object as a byte array and then sends it
            public void SendObject(object Message)
            {
                // This code will throw an exception if "Message" is not serializable
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, Message);
                    //This will throw an exception if the serialized Message exceeds the limit
                    queue.AddMessage(new CloudQueueMessage(ms.ToArray()));
                }
            }

            // Reads the oldest message from the queue as a byte array, and attempts to Deserialize it to type <T>
            // Message is deleted from the queue if deserialization is successful
            public T DequeueObject<T>()
            {
                var messageContext = queue.GetMessage();
                T obj;
            // This code will throw an exception if type "T" is not serializable

                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Write(messageContext.AsBytes, 0, messageContext.AsBytes.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    obj = (T)binForm.Deserialize(memStream);
                }
                queue.DeleteMessage(messageContext);
                return obj;
            }

            // Reads the oldest message from the queue as a byte array, and attempts to Deserialize it to type <T>
            // Message is locked and not deleted from the queue for the default timeout period
            // You must call DeleteMessage() on this class before the timeout expires, or the message will be re-queued.

            public T PeekObject<T>()
            {
                messageContext = queue.PeekMessage();
                T obj;
                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Write(messageContext.AsBytes, 0, messageContext.AsBytes.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    obj = (T)binForm.Deserialize(memStream);
                }
              
                return obj;
            }

        // Use the same DeleteMessage() method for objects as for strings

        #endregion

        // Helper Method that converts the current row of a DataReader to a delimited string, 
        // example CSV, pass a string containing a comma as the Delimiter.
        // Usage: StorageQueueFactory.SendString(DataReaderToDelimitedString( yourdatareader, ",");
        public string DataReaderToDelimitedString(DbDataReader Dr, string Delimiter)
        {
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < Dr.FieldCount - 1; index++)
            {
                if (!Dr.IsDBNull(index))
                {
                    string value = Dr.GetValue(index).ToString();
                    if (Dr.GetFieldType(index) == typeof(string))
                    {
                        //If double quotes are used in value, ensure each are replaced but 2.
                        if (value.IndexOf("\"") >= 0)
                            value = value.Replace("\"", "\"\"");

                        //If separator are is in value, ensure it is put in double quotes.
                        if (value.IndexOf(Delimiter) >= 0)
                            value = "\"" + value + "\"";
                    }
                    sb.Append(value);
                }

                if (index < Dr.FieldCount - 1)
                    sb.Append(Delimiter);
            }

            if (!Dr.IsDBNull(Dr.FieldCount - 1))
                sb.Append(Dr.GetValue(Dr.FieldCount - 1).ToString().Replace(Delimiter, " "));

            return sb.ToString();

        }


    }
}
