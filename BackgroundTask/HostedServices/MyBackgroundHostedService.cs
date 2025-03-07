using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BackgroundTask.HostedServices
{
    public class MyBackgroundHostedService : IHostedService,IDisposable
    {
        private readonly ILogger<MyBackgroundHostedService> _logger;
        private Timer _timer;
        public MyBackgroundHostedService(ILogger<MyBackgroundHostedService> logger)
        {
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            //while (!cancellationToken.IsCancellationRequested) 
            //{
            //    Console.WriteLine("Hello");
            //}
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
        private void DoWork(object state)
        {
            Console.WriteLine("Helloe");
            _logger.LogInformation("Timed Hosted Service is working.");
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Close the application");

            //Change(Timeout.Infinite, 0):

            //The Change method is used to modify the behavior of the Timer. It accepts two parameters:
            // dueTime: The time to wait before the timer’s callback method is next invoked.
            // period: The time interval between subsequent invocations of the callback.
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogError("Timed Hosted Service is shuting down.");
            return Task.CompletedTask;
        }

        // this disposed method come from IDesposable interfase
        // It triggerd by the framework when the application is geting shutdown
        public void Dispose()
        {
            // Release all the resources that used by current instance of Timer
            _timer?.Dispose();
        }
    }
}
