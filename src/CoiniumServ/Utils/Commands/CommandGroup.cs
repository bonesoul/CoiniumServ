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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog;

namespace CoiniumServ.Utils.Commands
{
    public class CommandGroup
    {
        public CommandGroupAttribute Attributes { get; private set; }

        private readonly Dictionary<CommandAttribute, MethodInfo> _commands =
            new Dictionary<CommandAttribute, MethodInfo>();

        public void Register(CommandGroupAttribute attributes)
        {
            Attributes = attributes;
            RegisterDefaultCommand();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            foreach (var method in GetType().GetMethods())
            {
                object[] attributes = method.GetCustomAttributes(typeof(CommandAttribute), true);
                if (attributes.Length == 0) 
                    continue;

                var attribute = (CommandAttribute)attributes[0];
                if (attribute is DefaultCommand) 
                    continue;

                if (!_commands.ContainsKey(attribute))
                    _commands.Add(attribute, method);

                else
                    Log.ForContext<CommandGroup>().Warning("There exists an already registered command '{0}'.", attribute.Name);
            }
        }

        private void RegisterDefaultCommand()
        {
            foreach (var method in GetType().GetMethods())
            {
                object[] attributes = method.GetCustomAttributes(typeof(DefaultCommand), true);
                if (attributes.Length == 0) 
                    continue;

                if (method.Name.ToLower() == "fallback") 
                    continue;

                _commands.Add(new DefaultCommand(), method);
                return;
            }

            // set the fallback command if we couldn't find a defined DefaultCommand.
            _commands.Add(new DefaultCommand(), GetType().GetMethod("Fallback"));
        }

        public virtual string Handle(string parameters)
        {
            string[] @params = null;
            CommandAttribute target = null;

            if (parameters == string.Empty)
                target = GetDefaultSubcommand();
            else
            {
                @params = parameters.Split(' ');
                target = GetSubcommand(@params[0]) ?? GetDefaultSubcommand();

                if (target != GetDefaultSubcommand())
                    @params = @params.Skip(1).ToArray();
            }

            return (string)_commands[target].Invoke(this, new object[] { @params });
        }

        public string GetHelp(string command)
        {
            foreach (var pair in _commands)
            {
                if (command != pair.Key.Name) continue;
                return pair.Key.Help;
            }

            return string.Empty;
        }

        [DefaultCommand]
        public virtual string Fallback(string[] @params = null)
        {
            var output = "Available subcommands: ";
            foreach (var pair in _commands)
            {
                if (pair.Key.Name.Trim() == string.Empty) 
                    continue; // skip fallback command.

                output += pair.Key.Name + ", ";
            }

            return output.Substring(0, output.Length - 2) + ".";
        }

        protected CommandAttribute GetDefaultSubcommand()
        {
            return _commands.Keys.First();
        }

        protected CommandAttribute GetSubcommand(string name)
        {
            return _commands.Keys.FirstOrDefault(command => command.Name == name);
        }
    }
}
