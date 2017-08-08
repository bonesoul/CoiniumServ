using System;
using System.Collections;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Runtime.Serialization;

namespace CoiniumServ.Overpool.Stratum
{
    class StratumClient
    {
        public event EventHandler<StratumEventArgs> GotSetDifficulty;
        public event EventHandler<StratumEventArgs> GotNotify;
        public event EventHandler<StratumEventArgs> GotResponse;

        public static Hashtable PendingACKs = Hashtable.Synchronized(new Hashtable());
        public TcpClient tcpClient;
        private int SharesSubmitted = 0;
        private string page = "";
        public string ExtraNonce1 = "";
        public int ExtraNonce2 = 0;
        private string Server;
        private int Port;
        private string Username;
        private string Password;
        public int ID;

        public void ConnectToServer(string MineServer, int MinePort, string MineUser, string MinePassword)
        {
            try
            {
                ID = 1;
                Server = MineServer;
                Port = MinePort;
                Username = MineUser;
                Password = MinePassword;
                tcpClient = new TcpClient(AddressFamily.InterNetwork);

                // Start an asynchronous connection
                tcpClient.BeginConnect(Server, Port, new AsyncCallback(ConnectCallback), tcpClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket error:" + ex.Message);
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            if (tcpClient.Connected)
                Console.WriteLine("Connected");
            else
            {
                Console.WriteLine("Unable to connect to server {0} on port {1}", Server, Port);
                Environment.Exit(-1);
            }

            // We are connected successfully
            try
            {
                SendSUBSCRIBE();
                SendAUTHORIZE();

                NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffer = new byte[tcpClient.ReceiveBufferSize];

                // Now we are connected start async read operation.
                networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket error:" + ex.Message);
            }
        }

        public void SendSUBSCRIBE()
        {
            Byte[] bytesSent;
            StratumCommand Command = new StratumCommand();
            
            Command.Id = ID++;
            Command.Method = "mining.subscribe";
            Command.Parameters = new ArrayList();

            string request = Utilities.JsonSerialize(Command) + "\n";

            bytesSent = Encoding.ASCII.GetBytes(request);

            try
            {
                tcpClient.GetStream().Write(bytesSent, 0, bytesSent.Length);
                Console.WriteLine("Adding command: "+Command.Method+"("+Command.Id+")");
                PendingACKs.Add(Command.Id, Command.Method);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket error:" + ex.Message);
                ConnectToServer(Server, Port, Username, Password);
            }
            
            Console.WriteLine("Sent mining.subscribe");
        }

        public void SendAUTHORIZE()
        {
            Byte[] bytesSent;
            StratumCommand Command = new StratumCommand();

            Command.Id = ID++;
            Command.Method = "mining.authorize";
            Command.Parameters = new ArrayList();
            Command.Parameters.Add(Username);
            Command.Parameters.Add(Password);

            string request = Utilities.JsonSerialize(Command) + "\n";

            bytesSent = Encoding.ASCII.GetBytes(request);

            try
            {
                tcpClient.GetStream().Write(bytesSent, 0, bytesSent.Length);
                Console.WriteLine("Adding command: " + Command.Method + "(" + Command.Id + ")");
                PendingACKs.Add(Command.Id, Command.Method);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Socket error:" + ex.Message);
                ConnectToServer(Server, Port, Username, Password);
            }
            
            Console.WriteLine("Sent mining.authorize");
        }

        public void SendSUBMIT(string JobID, string nTime, string Nonce, int Difficulty)
        {
            StratumCommand Command = new StratumCommand();
            Command.Id = ID++;
            Command.Method = "mining.submit";
            Command.Parameters = new ArrayList();
            Command.Parameters.Add(Username);
            Command.Parameters.Add(JobID);
            Command.Parameters.Add(ExtraNonce2.ToString("x8"));
            Command.Parameters.Add(nTime);
            Command.Parameters.Add(Nonce);

            string SubmitString = Utilities.JsonSerialize(Command) + "\n";

            Byte[] bytesSent = Encoding.ASCII.GetBytes(SubmitString);

            try
            {
                tcpClient.GetStream().Write(bytesSent, 0, bytesSent.Length);
                Console.WriteLine("Adding command: " + Command.Method + "(" + Command.Id + ")");
                PendingACKs.Add(Command.Id, Command.Method);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket error:" + ex.Message);
                ConnectToServer(Server, Port, Username, Password);
            }

            SharesSubmitted++;
            Console.WriteLine("{0} - Submit (Difficulty {1})", DateTime.Now, Difficulty);
            Debug.WriteLine("[{0}] Submit (Difficulty {1}) : {2}", DateTime.Now, Difficulty, SubmitString);
        }

        // Callback for Read operation
        private void ReadCallback(IAsyncResult result)
        {
            NetworkStream networkStream;
            int bytesread;
            
            byte[] buffer = result.AsyncState as byte[];
            
            try
            {
                networkStream = tcpClient.GetStream();
                bytesread = networkStream.EndRead(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Socket error:" + ex.Message);
                return;
            }

            if (bytesread == 0)
            {
                Console.WriteLine(DateTime.Now +  " Disconnected. Reconnecting...");
                Debug.WriteLine(DateTime.Now + " Disconnected. Reconnecting...");
                tcpClient.Close();
                tcpClient = null;
                Console.WriteLine("Clear PendingACKs");
                PendingACKs.Clear();
                ConnectToServer(Server, Port, Username, Password);
                return;
            }

            // Get the data
            string data = Encoding.ASCII.GetString(buffer, 0, bytesread);
            Debug.WriteLine(data);

            page = page + data;

            int FoundClose = page.IndexOf('\n');

            while (FoundClose > 0)
            {
                string CurrentString = page.Substring(0, FoundClose + 1);

                // We can get either a command or response from the server. Try to deserialise both
                StratumCommand Command = Utilities.JsonDeserialize<StratumCommand>(CurrentString);
                StratumResponse Response = Utilities.JsonDeserialize<StratumResponse>(CurrentString);

                StratumEventArgs e = new StratumEventArgs();

                if (Command.Method != null)             // We got a command
                {
                    Debug.WriteLine(DateTime.Now + " Got Command: " + CurrentString);
                    e.MiningEventArg = Command;

                    switch (Command.Method)
                    {
                        case "mining.notify":
                            if (GotNotify != null)
                                GotNotify(this, e);
                            break;
                        case "mining.set_difficulty":
                            if (GotSetDifficulty != null)
                                GotSetDifficulty(this, e);
                            break;
                    }
                }
                else if (Response.Error != null || Response.Result != null)       // We got a response
                {
                    Debug.WriteLine(DateTime.Now + " Got Response: " + CurrentString);
                    e.MiningEventArg = Response;

                    // Find the command that this is the response to and remove it from the list of commands that we're waiting on a response to
                    try
                    {
                        string Cmd = (string)PendingACKs[Response.Id];
                        Console.WriteLine("Remove from PendingACKs id=" + Response.Id);
                        PendingACKs.Remove(Response.Id);
						if (Cmd == null)
							Console.WriteLine("Unexpected Response");
						else if (GotResponse != null)
							GotResponse(Cmd, e);
                    }catch(Exception ex){
                        Console.WriteLine("Unexpected Response: " + ex.Message);
                    }
                }

                page = page.Remove(0, FoundClose + 1);
                FoundClose = page.IndexOf('\n');
            }

            // Then start reading from the network again.
            networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }
    }

    [DataContract]
    public class StratumCommand
    {
        [DataMember(Name = "method")]
        public string Method;
        [DataMember(Name = "id")]
        public System.Nullable<int> Id;
        [DataMember(Name = "params")]
        public ArrayList Parameters;
    }

    [DataContract]
    public class StratumResponse
    {
        [DataMember(Name = "error")]
        public StratumError Error;
        [DataMember(Name = "id")]
        public System.Nullable<int> Id;
        [DataMember(Name = "result")]
        public object Result;
    }

    [DataContract]
	public class StratumError
	{
        [DataMember(Name = "code")]
		public int Code;
        [DataMember(Name = "message")]
		public string Message;
        [DataMember(Name = "data")]
		public string Data;
	}


    public class StratumEventArgs:EventArgs
    {
        public object MiningEventArg;
    }
}
