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

using Coinium.Common.Context;
using Coinium.Common.Repository.Registries;
using Nancy.TinyIoc;

namespace Coinium.Common.Repository
{
    /// <summary>
    /// IOC & DI registry.
    /// To get a better overview of IOC, check http://joelabrahamsson.com/inversion-of-control-an-introduction-with-examples-in-net/
    /// </summary>
    public class Bootstrapper
    {
        private readonly TinyIoCContainer _container ;

        public Bootstrapper(TinyIoCContainer container)
        {
            _container = container;
        }

        public void Run()
        {
            var masterRegistry = new Registry(_container);
            masterRegistry.RegisterInstances();

            var applicationContext = _container.Resolve<IApplicationContext>();
            applicationContext.Initialize(_container);

            foreach (var registry in _container.ResolveAll<IRegistry>())
            {
                registry.RegisterInstances();
            }
        }
    }
}
