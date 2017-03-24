#region License
// 
//     MIT License
//
//     CoiniumServ - Crypto Currency Mining Pool Server Software
//     Copyright (C) 2013 - 2017, CoiniumServ Project
//     Hüseyin Uslu, shalafiraistlin at gmail dot com
//     https://github.com/bonesoul/CoiniumServ
// 
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// 
#endregion

using System;
using CoiniumServ.Configuration;
using CoiniumServ.Utils.Helpers;
using CoiniumServ.Utils.Platform;
using Metrics;
using Serilog;

namespace CoiniumServ.Utils.Metrics
{
    public class MetricsManager : IMetricsManager
    {
        private readonly ILogger _logger;

        public MetricsManager(IConfigManager configManager)
        {
            if (!configManager.WebServerConfig.Backend.MetricsEnabled)
                return;

            _logger = Log.ForContext<MetricsManager>();

            Metric.Config
                //.WithReporting(c => c
                //    .WithTextFileReport(string.Format("{0}/logs/metrics/report.log", FileHelpers.AssemblyRoot),TimeSpan.FromSeconds(5))
                //    .WithCSVReports(string.Format(@"{0}/logs/metrics/csv", FileHelpers.AssemblyRoot),TimeSpan.FromSeconds(5)))
                .WithErrorHandler(exception => _logger.Error("Metrics error: {0}", exception.Message));

            if (PlatformManager.Framework == Frameworks.DotNet)
                Metric.Config.WithAllCounters(); // there is a still unresolved bug with mono borking with system.security.claimsidentity.
        }
    }
}
