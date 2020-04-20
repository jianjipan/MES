using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace MMIS
{
    public class DTUReceiveFilterFactory : IReceiveFilterFactory<DTURequestInfo>
    {
        public IReceiveFilter<DTURequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new DTUFixedHeaderReceiveFilter();
        }
    }
}
