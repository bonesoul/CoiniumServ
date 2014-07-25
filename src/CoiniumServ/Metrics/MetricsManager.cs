#region License
// 
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2014, CoiniumServ Project - http://www.coinium.org
//     http://www.coiniumserv.com - https://github.com/CoiniumServ/CoiniumServ
// 
//     This software is dual-licensed: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//    
//     For the terms of this license, see licenses/gpl_v3.txt.
// 
//     Alternatively, you can license this software under a commercial
//     license or white-label it as set out in licenses/commercial.txt.
// 
#endregion

using System;
using CoiniumServ.Configuration;
using CoiniumServ.Server.Web;
using CoiniumServ.Utils.Helpers.IO;
using CoiniumServ.Utils.Platform;
using Metrics;
using Serilog;

namespace CoiniumServ.Metrics
{
    public class MetricsManager : IMetricsManager
    {
        private readonly IBackendConfig _config;

        private readonly ILogger _logger;

        public MetricsManager(IConfigManager configManager)
        {
            _config = configManager.WebServerConfig.Backend;

            if (!_config.MetricsEnabled)
                return;

            _logger = Log.ForContext<MetricsManager>();

            Metric.Config
                .WithReporting(c => c
                    .WithTextFileReport(string.Format("{0}/logs/metrics/report.log", FileHelpers.AssemblyRoot),TimeSpan.FromSeconds(5))
                    .WithCSVReports(string.Format(@"{0}/logs/metrics/csv", FileHelpers.AssemblyRoot),TimeSpan.FromSeconds(5)))
                .WithErrorHandler(exception => _logger.Error("Metrics error: {0}", exception.Message));

            if (PlatformManager.Framework == Frameworks.DotNet)
                Metric.Config.WithAllCounters(); // there is a still unresolved bug with mono borking with system.security.claimsidentity.
        }
    }
}
