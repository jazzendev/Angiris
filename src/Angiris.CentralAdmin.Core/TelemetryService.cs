﻿using Angiris.Core.DataStore;
using Angiris.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angiris.CentralAdmin.Core
{
    public class TelemetryService
    {
        public async Task<List<DaemonStatus>> GetAllDaemonStatusListAsync()
        {
            var db = DataProviderFactory.SingletonRedisDaemonStatusProvider;

            var results = await db.GetAll();

            return results.ToList();
        }
    }
}
