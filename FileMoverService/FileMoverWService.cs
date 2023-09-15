using FileMoverService.AppCode;
using HubNet.Log4Net.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMoverService
{


    internal class FileMoverWService : BackgroundService
    {
        private readonly IConfiguration _configuration;

        private static DateTime startTime;

        public FileMoverWService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //startTime = DateTime.Now.AddMinutes(1);

            // Calculate the initial delay until the start time
            //double initialDelayMilliseconds = (startTime - DateTime.Now).TotalMilliseconds;

            string logfilePath = _configuration["Path:LogFilepath"];
            //logfilePath = @"R:\FileMoverLogfile.log";
            LogProcesser.InitHubLogManager(logfilePath);
            LogProcesser.Info("--------------------------------------------------------------------------------------------------------------------");
            LogProcesser.Info("Log File Has been Inilized");
            while (!stoppingToken.IsCancellationRequested)
            {
               LogProcesser.Info("FileMover Service has been started!!");
                FileMover fileMover=new FileMover(_configuration);
                var filesLocations= _configuration["Path:FileLocation"];
                //filesLocations = @"R:\filemove,R:\filemove1";
                LogProcesser.Info("Location of Files"+ filesLocations);
                try 
                {
                    foreach (var fileLocation in filesLocations?.Split(','))
                    {
                        LogProcesser.Info("File Moving Started To ::" + filesLocations);
                        fileMover.IsFilemoved(fileLocation);
                        LogProcesser.Info("File Moving Finish To ::" + filesLocations);
                    }
                }
                catch(Exception ex) 
                {
                    LogProcesser.Info("Worker running :" + ex.Message);
                }
                await Task.Delay(1000*61, stoppingToken); // Delay for 1 mi 1 second
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

    }
}
