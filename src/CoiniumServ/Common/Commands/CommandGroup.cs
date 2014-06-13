/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Coinium.Common.Commands
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
                    Log.Warning("There exists an already registered command '{0}'.", attribute.Name);
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
