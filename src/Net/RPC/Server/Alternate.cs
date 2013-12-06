        //private static object[] services = new object[]
        //    {
        //        new RPCServer.ExampleCalculatorService()
        //    };


        //[JsonRpcMethod("add")]
        //public int Add(int a, int b) { return a + b; }

        //[JsonRpcMethod("env")]
        //public IDictionary GetEnvironment() { return Environment.GetEnvironmentVariables(); }

        //[JsonRpcMethod("mining.subscribe")]
        //private string miningsubscribe()
        //{
        //    var info = new Info();
        //    info.Version = "abc";
        //    string json = JsonConvert.SerializeObject(info);

        //    return json;
        //}

            //TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 3333);

            //try
            //{
            //    server.Start();

            //    while (true)
            //    {
            //        Console.WriteLine("Waiting for a connection... ");

            //        using (TcpClient client = server.AcceptTcpClient())
            //        {
            //            Console.WriteLine("Connected with " + client.Client.RemoteEndPoint);

            //            using (NetworkStream stream = client.GetStream())
            //            {
            //                Service service = new Service();
            //                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            //                StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(false));
            //                JsonRpcDispatcher dispatcher = new JsonRpcDispatcher(service);
            //                dispatcher.Process(reader, writer);
            //                writer.Flush();
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.Error.WriteLine(e.GetBaseException().Message);
            //    Trace.WriteLine(e.ToString());
            //}
            //finally
            //{
            //    server.Stop();
            //}


