using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace AustinHarris.JsonRpc
{
    public class SMD
    {
        public string transport { get; set; }
        public string envelope { get; set; }
        public string target { get; set; }
        public bool additonalParameters { get; set; }
        public SMDAdditionalParameters[] parameters { get; set; }
        [JsonIgnore]
        public static List<string> TypeHashes { get; set; }
        [JsonProperty("types")]
        public static Dictionary<int,JObject> Types { get; set; }
        [JsonProperty("services")]
        public Dictionary<string, SMDService> Services { get; set; }

        public SMD ()
	    {
            transport = "POST";
            envelope = "URL";
            target = "/json.rpc";
            additonalParameters = false;
            parameters = new SMDAdditionalParameters[0];
            Services = new Dictionary<string,SMDService>();
            Types = new Dictionary<int, JObject>();
            TypeHashes = new List<string>();
	    }

        public void AddService(string method, Dictionary<string,Type> parameters)
        {
            var newService = new SMDService(transport,"JSON-RPC-2.0",parameters);
            Services.Add(method,newService);
        }

        public static int AddType(JObject jo)
        {
            var hash = "t_" + jo.ToString().GetHashCode();
            lock (TypeHashes)
            {                
                var idx = 0;
                if (TypeHashes.Contains(hash) == false)
                {
                    TypeHashes.Add(hash);
                    idx = TypeHashes.IndexOf(hash);                    
                    Types.Add(idx, jo);
                }
            }
            return TypeHashes.IndexOf(hash); 
        }
    }

    public class SMDService
    {
        /// <summary>
        /// Defines a service method http://dojotoolkit.org/reference-guide/1.8/dojox/rpc/smd.html
        /// </summary>
        /// <param name="transport">POST, GET, REST, JSONP, TCP/IP</param>
        /// <param name="envelope">URL, PATH, JSON, JSON-RPC-1.0, JSON-RPC-1.1, JSON-RPC-2.0</param>
        /// <param name="parameters"></param>
        public SMDService(string transport, string envelope, Dictionary<string, Type> parameters)
        {
            // TODO: Complete member initialization
            this.transport = transport;
            this.envelope = envelope;
            this.parameters = new SMDAdditionalParameters[parameters.Count-1]; // last param is return type similar to Func<,>
            int ctr=0;
            foreach (var item in parameters)
	        {
                if (ctr < parameters.Count -1)// never the last one. last one is the return type.
                {
                    this.parameters[ctr++] = new SMDAdditionalParameters(item.Key, item.Value);
                }
	        }

            // this is getting the return type from the end of the param list
            this.returns = new SMDResult(parameters.Values.LastOrDefault());
        }
        public string transport { get; private set; }
        public string envelope { get; private set; }
        public SMDResult returns { get; private set; }

        /// <summary>
        /// This indicates what parameters may be supplied for the service calls. 
        /// A parameters value MUST be an Array. Each value in the parameters Array should describe a parameter 
        /// and follow the JSON Schema property definition. Each of parameters that are defined at the root level
        /// are inherited by each of service definition's parameters. The parameter definition follows the 
        /// JSON Schema property definition with the additional properties:
        /// </summary>
        public SMDAdditionalParameters[] parameters { get; private set; }
    }

    public class SMDResult
    {
        [JsonProperty("__type")]
        public int Type { get; private set; }

        public SMDResult(System.Type type)
        {
            Type = SMDAdditionalParameters.GetTypeRecursive(type);
        }
    }

    public class SMDAdditionalParameters
    {
        public  SMDAdditionalParameters(string parametername, System.Type type)
        {
            Name = parametername;
            Type = GetTypeRecursive(ObjectType = type);
          
        }

        [JsonIgnore()]
        public Type ObjectType { get; set; }
        [JsonProperty("__name")]
        public string Name { get; set; }
        [JsonProperty("__type")]
        public int Type { get; set; }

        internal static int GetTypeRecursive(Type t)
        {
            JObject jo = new JObject();
            jo.Add("__name", t.Name.ToLower());
            if (isSimpleType(t))
            {                
                return SMD.AddType(jo);
            }

            var genArgs = t.GetGenericArguments();
            PropertyInfo[] properties = t.GetProperties();
            FieldInfo[] fields = t.GetFields();

            if (genArgs.Length > 0)
            {
                var ja = new JArray();
                foreach (var item in genArgs)
                {
                    if (item != t)
                    {
                        var jt = GetTypeRecursive(item);
                        ja.Add(jt);
                    }
                    else
                    {
                        // make a special case where -1 indicates this type
                        ja.Add(-1);
                    }
                }
                jo.Add("__genericArguments", ja);
            }

            foreach (var item in properties)
            {
                if (item.GetAccessors().Where(x => x.IsPublic).Count() > 0)
                {
                    if (item.PropertyType != t)
                    {
                        var jt = GetTypeRecursive(item.PropertyType);
                        jo.Add(item.Name, jt);
                    }
                    else
                    {
                        // make a special case where -1 indicates this type
                        jo.Add(item.Name, -1);
                    }
                }
            }

            foreach (var item in fields)
            {
                if (item.IsPublic)
                {
                    if (item.FieldType != t)
                    {
                        var jt = GetTypeRecursive(item.FieldType);
                        jo.Add(item.Name, jt);
                    }
                    else
                    {
                        // make a special case where -1 indicates this type
                        jo.Add(item.Name, -1);
                    }
                }
            }

            return SMD.AddType(jo);
        }

        internal static bool isSimpleType(Type t)
        {
            var name = t.FullName.ToLower();

            if (name.Contains("newtonsoft")
                || name == "system.sbyte"
                || name == "system.byte"
                || name == "system.int16"
                || name == "system.uint16"
                || name == "system.int32"
                || name == "system.uint32"
                || name == "system.int64"
                || name == "system.uint64"
                || name == "system.char"
                || name == "system.single"
                || name == "system.double"
                || name == "system.boolean"
                || name == "system.decimal"
                || name == "system.float"
                || name == "system.numeric"
                || name == "system.money"
                || name == "system.string"
                || name == "system.object"
                || name == "system.type"
               // || name == "system.datetime"
                || name == "system.reflection.membertypes")
            {
                return true;
            }

            return false;
        }
    }
}
