using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientChatApp.Models
{
    public class AccountModel : LoginModel
    {
        public long Id { get; set; }
        public List<ChannelInfo> Channels { get; set; }
    }

    public class ChannelInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
