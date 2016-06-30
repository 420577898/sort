using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DotRas;
namespace SortConsole
{
    public abstract class DialBase
    {
        public const string EntryName = "宽带连接";

        /// <summary>
        /// Holds a value containing the handle used by the connection that was dialed.
        /// </summary>
        public RasHandle handle = null;

        protected RasPhoneBook AllUsersPhoneBook = new RasPhoneBook();

        protected RasDialer Dialer = new RasDialer();

        public DialBase()
        {
            this.Dialer.Timeout = 20000;
            this.Dialer.Credentials = null;
            this.Dialer.EapOptions = new DotRas.RasEapOptions(false, false, false);
            this.Dialer.HangUpPollingInterval = 0;
            this.Dialer.Options = new DotRas.RasDialOptions(false, false, false, false, false, false, false, false, false, false);
            //this.Dialer.SynchronizingObject = this;
            //this.Dialer.StateChanged += new System.EventHandler<DotRas.StateChangedEventArgs>(this.Dialer_StateChanged);
            this.Dialer.DialCompleted += new System.EventHandler<DotRas.DialCompletedEventArgs>(this.Dialer_DialCompleted);
            string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            //创建VPN
            this.AllUsersPhoneBook.Open(path);

            if (!this.AllUsersPhoneBook.Entries.Contains(EntryName))
            {
                RasEntry entry = RasEntry.CreateBroadbandEntry(EntryName,
                    RasDevice.GetDeviceByName("(PPPoE)", RasDeviceType.PPPoE));

                this.AllUsersPhoneBook.Entries.Add(entry);
            }
        }

        public abstract void Process();

        public virtual void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
        }
    }
}
