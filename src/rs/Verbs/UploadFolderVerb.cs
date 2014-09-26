﻿using System;
using System.IO;
using System.Linq;
using CYC.Logging.Interface;
using Rs.Commands;
using Rs.Constants;
using Rs.ReportService2010;
using Rs.Services;

namespace Rs.Verbs
{
    public class UploadFolderVerb
    {
        private readonly UploadFolderSubOptions options;
        private readonly IFileService fileService;
        private readonly IReportingServiceChannelFactory channelFactory;
        private readonly ILogger logger;

        public UploadFolderVerb(UploadFolderSubOptions options, ILogger logger)
        {
            this.options = options;
            this.fileService = new FileService();
            this.channelFactory = new ReportingServiceChannelFactory();
            this.logger = logger;
        }

        public void Process()
        {
            var files = Directory.EnumerateFiles(options.Folder, "*.rdl");
            
            if (files.Any())
            {
                var url = String.Format("http://{0}/reportserver/ReportService2010.asmx", options.Server);
                var channel = channelFactory.Create(url);

                foreach (var file in files)
                {
                    logger.Info("Uploading file '{0}' to folder '{1}' on report server '{2}'", file, options.DestinationFolder, options.Server);

                    channel.CreateCatalogItem(new CreateCatalogItemRequest
                    {
                        Definition = fileService.GetBytes(file),
                        Name = Path.GetFileNameWithoutExtension(file),
                        Parent = options.DestinationFolder,
                        ItemType = ItemType.Report,
                        Overwrite = true
                    });
                }
            }
            else
            {
                logger.Info("No .rdl files found to upload in folder '{0}'", options.DestinationFolder);   
            }
        }
    }
}
