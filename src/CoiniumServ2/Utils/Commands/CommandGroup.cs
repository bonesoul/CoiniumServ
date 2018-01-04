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
