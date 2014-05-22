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

using System;
using System.Collections.Generic;
using Coinium.Core.Context;
using Nancy.TinyIoc;

namespace Coinium.Core.Repository.Registries
{
    public class Registry : IRegistry
    {
        private readonly TinyIoCContainer _kernel;

        public Registry(TinyIoCContainer kernel)
        {
            _kernel = kernel;
        }

        public void RegisterInstances()
        {
            _kernel.Register<IApplicationContext, ApplicationContext>().AsSingleton();

            _kernel.RegisterMultiple<IRegistry>(
                new List<Type>
                {
                    typeof(ManagerRegistry),
                    typeof(ServerRegistry),
                    typeof(ServiceRegistry),
                    typeof(ClassRegistry),
                    typeof(FactoryRegistry),
                });
        }
    }
}
