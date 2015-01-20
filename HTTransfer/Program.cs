using System;
using HTInfrastructure.TDF;
using HTTransfer.Core;
using log4net;
using TinyIoC;
using HTTransfer.Configuration;
using TDFAPI;

namespace HTTransfer
{
    class Program
    {
        private static readonly TinyIoCContainer Container = TinyIoCContainer.Current;
        private static readonly System.Timers.Timer timer = new System.Timers.Timer();

        static Program()
        {
            Initialize();
        }

        static void Main(string[] args)
        {
            try
            {
                
                var logger = Container.Resolve<ILog>();

                logger.Debug("The main form is started. try to log something...");
                Console.WriteLine("The main form is started. try to log something...");

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

                using (var dataSource = TDFTransfer.TDFImpInstance)
                {
                    var nOpenRet = dataSource.Open();

                    if (nOpenRet == TDFERRNO.TDF_ERR_SUCCESS)
                    {
                        logger.Info("connect success!\n");
                        Console.WriteLine("connect success!\n");
                    }
                    else
                    {
                        //这里判断错误代码，进行对应的操作，对于 TDF_ERR_NETWORK_ERROR，用户可以选择重连
                        logger.Info(string.Format("open returned:{0}, program quit", nOpenRet));
                        Console.WriteLine(string.Format("open returned:{0}, program quit", nOpenRet));
                    }

                    Console.ReadLine();

                    logger.Debug("The market is closed, the application will be quit now.");
                    Console.WriteLine("The market is closed, the application will be quit now.");

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.ReadLine();
            }

        }

        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Enabled = false;
            TDFTransfer.TDFImpInstance.Dispose();
            Environment.Exit(0);
        }


        static void Initialize()
        {
            Container.AutoRegister();

            LoggerRegistration();

            HandlerRegistration();

        }

        private static void LoggerRegistration()
        {
            log4net.Config.XmlConfigurator.Configure();
            Container.Register<ILog>(LogManager.GetLogger("DefaultLogFile"));
        }

        static void HandlerRegistration()
        {
            Container.Register<ITDFNetServerHandler, TDFNetServerHandler>();
        }
    }
}
