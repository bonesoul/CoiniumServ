using System.Collections.Generic;

namespace coinium.Net.RPC.Responses
{
    public class AddedNodeInfo
    {
        public string AddedNode { get; set; }
        public bool Connected { get; set; }
        public List<NodeAddress> Addresses { get; set; }
    }
}
