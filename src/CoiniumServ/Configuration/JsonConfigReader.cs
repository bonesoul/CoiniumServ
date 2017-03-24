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
using System.IO;
using System.Text.RegularExpressions;
using CoiniumServ.Utils.Helpers;
using JsonConfig;
using Newtonsoft.Json;
using Serilog;

namespace CoiniumServ.Configuration
{
    // TODO: let this be a factory and add log.forcontext<>
    public class JsonConfigReader : IJsonConfigReader
    {
        private const string Comments = @"#(.*?)\r?\n";

        public dynamic Read(string fileName)
        {
            try
            {
                var path = FileHelpers.GetAbsolutePath(fileName); // get the absolute path for the config file.
                var json = ReadJsonFromFile(path); // read the json.

                if (json == null) // make sure we were able to load the json file.
                    return null;

                json = CleanComments(json); // strip out comment lines that starts with # as they'll be preventing validation.
                var valid = ValidateJson(json, fileName); // check if it's valid.

                return !valid ? null : Config.ApplyJson(json, new ConfigObject()); // read configuration from the json.
            }
            catch (Exception e)
            {
                Log.Error("Json parsing failed for: {0:l} - {1:l}", fileName, e.Message);
                return null;
            }
        }

        private bool ValidateJson(string json, string fileName)
        {
            try
            {
                // try to validate the json.
                JsonConvert.DeserializeObject<dynamic>(json);
                return true;
            }
            catch (JsonReaderException e)
            {
                Log.Error("Json validation failed for: {0:l} - {1:l}", fileName,e.Message);
                return false;
            }
        }

        private string ReadJsonFromFile(string fileName)
        {
            try
            {
                var json = File.ReadAllText(fileName);
                return json;
            }
            catch (FileNotFoundException)
            {
                Log.Error("Can not read json file {0:l}", fileName);
                return null;
            }
        }

        private string CleanComments(string json)
        {
            // strip out comment lines that starts with # as they'll be preventing validation.
            json = Regex.Replace(json, Comments, "");

            return json;
        }
    }
}
