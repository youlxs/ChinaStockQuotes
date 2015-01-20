using System;
using System.Windows.Forms;
using ChinaStockQuotes.Configuration;
using ChinaStockQuotes.Core;
using ChinaStockQuotes.Core.Handler;
using log4net;
using System.Threading;

namespace ChinaStockQuotes
{
    public class MainForm  : IDisposable
    {
        private static readonly ILog logger = TinyIoC.TinyIoCContainer.Current.Resolve<ILog>();
        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private SocketWrapper socketWrapper;
 
        public MainForm()
        {
            try
            {
                timer.Interval = ConfigurationData.MarketCloseTime.Subtract(DateTime.Now).TotalMilliseconds;

                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Enabled = false;
            Application.Exit();
            Environment.Exit(0);
        }

        public void Start()
        {
            logger.Debug("The main form is started. try to log something...");

            Console.WriteLine("The main form is started.");

            socketWrapper = new SocketWrapper(TinyIoC.TinyIoCContainer.Current.Resolve<INetClientHandler>());

            socketWrapper.ProcessData();
        }

        public void Dispose()
        {
            logger.Debug("The market is closed, the application will be quit now.");

            if (socketWrapper != null)
            {
                socketWrapper.Dispose();
            }  
        }
    }   
}
