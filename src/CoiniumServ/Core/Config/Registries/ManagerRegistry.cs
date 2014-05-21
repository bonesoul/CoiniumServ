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

using Coinium.Core.Mining.Jobs;
using Coinium.Core.Mining.Miner;
using Coinium.Core.Mining.Pool;
using Coinium.Core.Mining.Share;
using Ninject;

namespace Coinium.Core.Config.Registries
{
    public class ManagerRegistry : IRegistry
    {
        private readonly IKernel _kernel;

        public ManagerRegistry(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void RegisterInstances()
        {
            _kernel.Bind<IShareManager>().To<ShareManager>();
            _kernel.Bind<IMinerManager>().To<MinerManager>();
            _kernel.Bind<IJobManager>().To<JobManager>();
            _kernel.Bind<IPoolManager>().To<PoolManager>().InSingletonScope();
        }
    }
}
