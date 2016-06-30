using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DotRas;
namespace SortConsole
{
    public class DialDisconn:DialBase
    {
        public event Action DisconnHandle;

        public override void Process()
        {
            if (base.Dialer.IsBusy)
            {
                // The connection attempt has not been completed, cancel the attempt.
                base.Dialer.DialAsyncCancel();
            }
            else
            {
                // The connection attempt has completed, attempt to find the connection in the active connections.
                RasConnection connection = RasConnection.GetActiveConnectionByHandle(base.handle);
                if (connection != null)
                {
                    // The connection has been found, disconnect it.
                    connection.HangUp();
                }
            }
            if (DisconnHandle != null)
                DisconnHandle();
        }

    }
}
