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

namespace CoiniumServ.Utils.Commands
{
    /// <summary>
    /// Marks a class as an command group.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandGroupAttribute : Attribute
    {
        /// <summary>
        /// Command group's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for command group.
        /// </summary>
        public string Help { get; private set; }

        public CommandGroupAttribute(string name, string help)
        {
            Name = name.ToLower();
            Help = help;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Command's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for command.
        /// </summary>
        public string Help { get; private set; }

        public CommandAttribute(string command, string help)
        {
            Name = command.ToLower();
            Help = help;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DefaultCommand : CommandAttribute
    {
        public DefaultCommand()
            : base("", "")
        { }
    }
}
