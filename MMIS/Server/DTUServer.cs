using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;

namespace MMIS
{
    public class DTUServer : AppServer<DTUSession, DTURequestInfo>
    {
        public DTUServer()
            : base(new DTUReceiveFilterFactory())
        {

        }
    }
}
