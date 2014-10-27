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
using CoiniumServ.Banning;
using CoiniumServ.Configuration;
using CoiniumServ.Container;
using Nancy.TinyIoc;
using Should.Fluent;
using Xunit;

namespace CoiniumServ.Tests.Banning
{
    public class BanConfigTests
    {
        private readonly IJsonConfigReader _jsonConfigReader;

        public BanConfigTests()
        {
            var kernel = TinyIoCContainer.Current;
            new Bootstrapper(kernel);
            var configFactory = kernel.Resolve<IConfigFactory>();
            _jsonConfigReader = configFactory.GetJsonConfigReader();
        }

        /// <summary>
        /// Tests a json file with valid configuration.
        /// </summary>
        [Fact]
        public void ValidConfig_ShouldSuccess()
        {
            // read a valid json config sample.
            var data = _jsonConfigReader.Read("Banning/valid-config.json");
            var config = new BanConfig(data);

            // make sure our expected values are set.
            config.Enabled.Should().Equal(true);
            config.Duration.Should().Equal(1);
            config.InvalidPercent.Should().Equal(2);
            config.CheckThreshold.Should().Equal(3);
            config.PurgeInterval.Should().Equal(4);
            config.Valid.Should().Equal(true);
        }

        /// <summary>
        /// Tests a json file with valid configuration.
        /// </summary>
        [Fact]
        public void InvalidConfig_ShouldReturnDefaults()
        {
            // read a valid json config sample.
            var data = _jsonConfigReader.Read("invalid-config.json");
            var config = new BanConfig(data);

            // as we have just supplied an invalid config, we should get a valid config object with default values
            config.Enabled.Should().Equal(false);
            config.Duration.Should().Equal(600);
            config.InvalidPercent.Should().Equal(50);
            config.CheckThreshold.Should().Equal(100);
            config.PurgeInterval.Should().Equal(300);
            config.Valid.Should().Equal(true);
        }

        /// <summary>
        /// Tests a json file with valid configuration.
        /// </summary>
        [Fact]
        public void InvalidJson_ShouldBeInvalid()
        {
            // read a valid json config sample.
            var data = _jsonConfigReader.Read("invalid-json.json");
            var config = new BanConfig(data);

            // as we have just supplied an invalid json, config should be just marked as invalid.
            config.Valid.Should().Equal(false);
        }
    }
}
