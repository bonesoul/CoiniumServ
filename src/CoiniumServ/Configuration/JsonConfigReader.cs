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
