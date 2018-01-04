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
        //[Fact]
        //public void ValidConfig_ShouldSuccess()
        //{
        //    // read a valid json config sample.
        //    var data = _jsonConfigReader.Read("Banning/valid-config.json");
        //    var config = new BanConfig(data);

        //    // make sure our expected values are set.
        //    config.Enabled.Should().Equal(true);
        //    config.Duration.Should().Equal(1);
        //    config.InvalidPercent.Should().Equal(2);
        //    config.CheckThreshold.Should().Equal(3);
        //    config.PurgeInterval.Should().Equal(4);
        //    config.Valid.Should().Equal(true);
        //}

        /// <summary>
        /// Tests a json file with valid configuration.
        /// </summary>
        //[Fact]
        //public void InvalidConfig_ShouldReturnDefaults()
        //{
        //    // read a valid json config sample.
        //    var data = _jsonConfigReader.Read("invalid-config.json");
        //    var config = new BanConfig(data);

        //    // as we have just supplied an invalid config, we should get a valid config object with default values
        //    config.Enabled.Should().Equal(false);
        //    config.Duration.Should().Equal(600);
        //    config.InvalidPercent.Should().Equal(50);
        //    config.CheckThreshold.Should().Equal(100);
        //    config.PurgeInterval.Should().Equal(300);
        //    config.Valid.Should().Equal(true);
        //}

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
