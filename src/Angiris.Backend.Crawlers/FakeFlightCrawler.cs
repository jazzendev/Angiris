﻿using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.Backend.Crawlers
{
    public class FakeFlightCrawler: FlightCrawlerBase
    {

        public override void Initialize(FlightCrawlEntity crawlEntity)
        {
            base.Initialize(crawlEntity);
        }

        public override async Task StartProcessing()
        {
            await base.StartProcessing();

        }
    }
}
