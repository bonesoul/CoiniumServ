using System;
using System.Collections.Generic;
using System.Linq;
using CoiniumServ.Coin.Config;
using CoiniumServ.Overpool.Config;
using CoiniumServ.Overpool;
using CoiniumServ.Daemon.Exceptions;
using AustinHarris.JsonRpc;
using Newtonsoft.Json;
using System.Text;
using CoiniumServ.Server.Mining.Stratum.Sockets;
using CoiniumServ.Utils.Extensions;
using Serilog;
using CoiniumServ.Logging;
using CoiniumServ.Overpool.Stratum;
using System.Timers;
using System.Threading;
using System.ComponentModel;
//using CoiniumServ.Overpool.Requests;
//using CoiniumServ.Overpool.Responses;

namespace CoiniumServ.Overpool
{
	public class OverpoolClient : OverpoolBase, IOverpoolClient
	{

		//private static Miner CoinMiner;
		private static int CurrentDifficulty;
		//public static members are thread safe
		public static Queue<Job> IncomingJobs = new Queue<Job>();
		private static StratumClient Stratum;
		//private static BackgroundWorker worker;
		private static int SharesSubmitted = 0;
		private static int SharesAccepted = 0;
		//private static string Server = "";
		//private static int Port = 0;
		//private static string Username = "";
		//private static string Password = "";

		private static System.Timers.Timer KeepaliveTimer;

		private static void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			Console.Write("Keepalive - ");
			Stratum.SendAUTHORIZE();
		}


		public static readonly object[] EmptyString = { }; // used as empty parameter.

        public OverpoolClient(IOverpoolConfig overpoolConfig, ICoinConfig coinConfig, IRpcExceptionFactory rpcExceptionFactory)
            :base(overpoolConfig,coinConfig,rpcExceptionFactory)
		{ 
            _requestLogger = LogManager.PacketLogger.ForContext<OverpoolClient>().ForContext("Component", coinConfig.Name);
        }

        decimal IOverpoolClient.GetBalance(string account)
        {
            throw new NotImplementedException();
        }

        string IOverpoolClient.MakeRawRequest(string method, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        Dictionary<string, decimal> IOverpoolClient.ListAccounts()
        {
            throw new NotImplementedException();
        }

        string IOverpoolClient.GetAccount(string bitcoinAddress)
        {
            throw new NotImplementedException();
        }

        string IOverpoolClient.SendMany(string fromAccount, Dictionary<string, decimal> toBitcoinAddress, int minConf, string comment)
        {
            throw new NotImplementedException();
        }

        void IOverpoolClient.Subscribe()
        {
			var request = new JsonRequest
			{
				Id = 1,
				Method = "mining.subscribe",
                Params = new List<string> { "CoiniumServ" }
			};
            Send(request);
        }

        public IConnection Connection { get; private set; }
        //private ISocketServer SocketServer;
        private readonly ILogger _requestLogger;

		private void Send(JsonRequest request)
		{
            throw new NotImplementedException();
			var json = JsonConvert.SerializeObject(request) + "\n";

			var data = Encoding.UTF8.GetBytes(json);
            InstantiateConnection();
			Connection.Send(data);

			_requestLogger.Verbose("tx: {0}", data.ToEncodedString().PrettifyJson());
		}

        private void InstantiateConnection()
        {
            throw new NotImplementedException();
        }

		public void Run(string Server, int Port, string Username, string Password = "")
		{
            throw new NotImplementedException();
			if (Server == "")
			{
				_requestLogger.Error("Missing server URL. URL should be in the format of tcp://megahash.wemineltc.com:3333");
				Environment.Exit(-1);
			}
			else if (Port == 0)
			{
				_requestLogger.Error("Missing server port. URL should be in the format of tcp://megahash.wemineltc.com:3333");
				Environment.Exit(-1);
			}
			else if (Username == "")
			{
				_requestLogger.Error("Missing username");
				Environment.Exit(-1);
			}
			/*
            else if (Password == "")
            {
                Console.WriteLine("Missing password");
                Environment.Exit(-1);
            }
            */

			_requestLogger.Error("Connecting to '{0}' on port '{1}' with username '{2}' and password '{3}'", Server, Port, Username, Password);

			Stratum = new StratumClient();

			// Workaround for pools that keep disconnecting if no work is submitted in a certain time period. Send regular mining.authorize commands to keep the connection open
			KeepaliveTimer = new System.Timers.Timer(45000);
			KeepaliveTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			KeepaliveTimer.Start();

			// Set up event handlers
			Stratum.GotResponse += stratum_GotResponse;
			Stratum.GotSetDifficulty += stratum_GotSetDifficulty;
			Stratum.GotNotify += stratum_GotNotify;

			// Connect to the server
			Stratum.ConnectToServer(Server, Port, Username, Password);

			// Start mining!!
			StartCoinMiner();

			// This thread waits forever as the mining happens on other threads. Can press Ctrl+C to exit
			Thread.Sleep(System.Threading.Timeout.Infinite);
		}

		static void StartCoinMiner()
		{
			// Wait for a new job to appear in the queue
			while (IncomingJobs.Count == 0)
				Thread.Sleep(500);

			// Get the job
			Job ThisJob = IncomingJobs.Dequeue();

			if (ThisJob.CleanJobs)
				Stratum.ExtraNonce2 = 0;

			// Increment ExtraNonce2
			Stratum.ExtraNonce2++;

			// Calculate MerkleRoot and Target
			string MerkleRoot = Utilities.GenerateMerkleRoot(ThisJob.Coinb1, ThisJob.Coinb2, Stratum.ExtraNonce1, 
                                                             Stratum.ExtraNonce2.ToString("x8"), ThisJob.MerkleNumbers);
			string Target = Utilities.GenerateTarget(CurrentDifficulty);

			// Update the inputs on this job
			ThisJob.Target = Target;
			ThisJob.Data = ThisJob.Version + ThisJob.PreviousHash + MerkleRoot + ThisJob.NetworkTime + ThisJob.NetworkDifficulty;

			// Start a new miner in the background and pass it the job
			//worker = new BackgroundWorker();
			//worker.DoWork += new DoWorkEventHandler(CoinMiner.Mine);
			//worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CoinMinerCompleted);
			//worker.RunWorkerAsync(ThisJob);
            throw new NotImplementedException();
		}

		static void CoinMinerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			// If the miner returned a result, submit it
			if (e.Result != null)
			{
				Job ThisJob = (Job)e.Result;
				SharesSubmitted++;

				Stratum.SendSUBMIT(ThisJob.JobID, ThisJob.Data.Substring(68 * 2, 8), ThisJob.Answer.ToString("x8"), CurrentDifficulty);
			}

			// Mine again
			StartCoinMiner();
		}

		static void stratum_GotResponse(object sender, StratumEventArgs e)
		{
			StratumResponse Response = (StratumResponse)e.MiningEventArg;

			Console.Write("Got Response to {0} - ", (string)sender);

			switch ((string)sender)
			{
				case "mining.authorize":
					if ((bool)Response.Result)
						Console.WriteLine("Worker authorized");
					else
					{
						Console.WriteLine("Worker rejected");
						Environment.Exit(-1);
					}
					break;

				case "mining.subscribe":
					Stratum.ExtraNonce1 = (string)((object[])Response.Result)[1];
					Console.WriteLine("Subscribed. ExtraNonce1 set to " + Stratum.ExtraNonce1);
					break;

				case "mining.submit":
					try
					{
						//Should be careful here!
						if ((bool)(Response.Result ?? false))
						{
							SharesAccepted++;
							Console.WriteLine("Share accepted ({0} of {1})", SharesAccepted, SharesSubmitted);
						}
						else
							Console.WriteLine("Share rejected: {0}", Response.Error.Message);
						//Console.WriteLine("Share rejected. {0}", Response.error.Count >0 ? Response.error[1] : "No error message provided.");
					}
					catch (Exception ex)
					{
						Console.WriteLine("Exception inside event: " + ex.Message);
					}
					break;
			}
		}

		static void stratum_GotSetDifficulty(object sender, StratumEventArgs e)
		{
			StratumCommand Command = (StratumCommand)e.MiningEventArg;
			//Vagabondan 2017-07-26: difficulty type should be modified!
			Console.WriteLine("Program.stratum_GotSetDifficulty: this should be MODIFIED!!! CurrentDiffficulty should not be integer!");
			CurrentDifficulty = Convert.ToInt32(Command.Parameters[0]);

			Console.WriteLine("Got Set_Difficulty " + CurrentDifficulty);
		}

		static void stratum_GotNotify(object sender, StratumEventArgs e)
		{
			Job ThisJob = new Job();
			StratumCommand Command = (StratumCommand)e.MiningEventArg;

			ThisJob.JobID = (string)Command.Parameters[0];
			ThisJob.PreviousHash = (string)Command.Parameters[1];
			ThisJob.Coinb1 = (string)Command.Parameters[2];
			ThisJob.Coinb2 = (string)Command.Parameters[3];
			Array a = (Array)Command.Parameters[4];
			ThisJob.Version = (string)Command.Parameters[5];
			ThisJob.NetworkDifficulty = (string)Command.Parameters[6];
			ThisJob.NetworkTime = (string)Command.Parameters[7];
			ThisJob.CleanJobs = (bool)Command.Parameters[8];

			ThisJob.MerkleNumbers = new string[a.Length];

			int i = 0;
			foreach (string s in a)
				ThisJob.MerkleNumbers[i++] = s;

			// Cancel the existing mining threads and clear the queue if CleanJobs = true
			if (ThisJob.CleanJobs)
			{
				Console.WriteLine("Stratum detected a new block. Stopping old threads.");

				IncomingJobs.Clear();
				//CoinMiner.done = true;
			}

			// Add the new job to the queue
			IncomingJobs.Enqueue(ThisJob);
		}
	}

	public class Job
	{
		// Inputs
		public string JobID;
		public string PreviousHash;
		public string Coinb1;
		public string Coinb2;
		public string[] MerkleNumbers;
		public string Version;
		public string NetworkDifficulty;
		public string NetworkTime;
		public bool CleanJobs;

		// Intermediate
		public string Target;
		public string Data;

		// Output
		public uint Answer;
	}
}
