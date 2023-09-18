using HubNet.Log4Net.Logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;


namespace FileMoverService.AppCode
{
    internal class FileMover
    {
        private readonly IConfiguration _configuration;
        public FileMover(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool IsFilemoved(string fileLocation)
        {
            int NoOfDays = Convert.ToInt32(_configuration["Path:NoOfDays"]); 
            string folderPath = fileLocation;
            //string archivePath = $@"{fileLocation}\{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_archived.zip";
            string archivePath = $@"{fileLocation}\{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_archived.zip";
            DateTime thresholdDate = DateTime.Now.AddDays(-NoOfDays);
            try
            {
                var filesToArchive = Directory.GetFiles(folderPath).Where(file => File.GetLastWriteTime(file) < thresholdDate && file.ToLower().EndsWith(".xml"));
                if(filesToArchive.Count()>0) 
                {
                    if (File.Exists(archivePath))
                    {
                        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
                        {
                            int i = 0;
                            foreach (string fileToArchive in filesToArchive)
                            {
                                if (fileToArchive.ToLower().EndsWith(".xml"))
                                {
                                    i++;
                                    archive.CreateEntryFromFile(fileToArchive, Path.GetFileName(fileToArchive));
                                    File.Delete(fileToArchive);
                                }
                            }
                            LogProcesser.Info(@$"Total Xml Files: On Locaton {fileLocation}  is {i}");
                        }
                    }
                    else
                    {
                        using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
                        {
                            int i = 0;
                            foreach (string fileToArchive in filesToArchive)
                            {
                                if (fileToArchive.ToLower().EndsWith(".xml"))
                                {
                                    i++;
                                    archive.CreateEntryFromFile(fileToArchive, Path.GetFileName(fileToArchive));
                                    File.Delete(fileToArchive);
                                }
                            }
                            LogProcesser.Info(@$"Total Xml Files: On Locaton {fileLocation}  is {i}");
                        }
                    }

                    //using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
                    //{
                    //    int i=0; 
                    //    foreach (string fileToArchive in filesToArchive)
                    //    {
                    //        if (fileToArchive.ToLower().EndsWith(".xml"))
                    //        {
                    //            i++;
                    //            archive.CreateEntryFromFile(fileToArchive, Path.GetFileName(fileToArchive));
                    //            File.Delete(fileToArchive);
                    //        }
                    //    }
                    //    LogProcesser.Info(@$"Total Xml Files: On Locaton {fileLocation}  is {i}");
                    //}
                }
                else
                {
                    LogProcesser.Info(@$"The Location {fileLocation} don't having XML Files");
                }
                
            }
            catch (Exception ex)
            {

                LogProcesser.Error("Worker running :" , ex.Message);
                throw ex;
            }
            return false;

        }
    }
}
