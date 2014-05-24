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
using System.IO;
using System.Linq;
using System.Reflection;
using Should.Fluent;
using Xunit;

namespace Tests.Extra
{
    // original code from: https://gist.github.com/brianlow/1553265

    public class ConflictTests
    {
        // [Fact] - enable the test when you want to check conflicts.
        public void FindConflictingReferences()
        {
            var assemblies = GetAllAssemblies(@"E:\Coding\CoiniumServ\bin\Debug");

            var references = GetReferencesFromAllAssemblies(assemblies);

            var groupsOfConflicts = FindReferencesWithTheSameShortNameButDiffererntFullNames(references);

            var conflictingAssemblies = new List<string>();

            foreach (var group in groupsOfConflicts)
            {
                conflictingAssemblies.Add(group.Key);

                if (group.Key == "mscorlib" || group.Key == "System") // just skip mscorlib and System conflicts.
                    continue;

                Console.Out.WriteLine("Possible conflicts for {0}:", group.Key);

                foreach (var reference in group)
                {
                    Console.Out.WriteLine("{0} references {1}", reference.Assembly.Name.PadRight(25), reference.ReferencedAssembly.FullName);
                }
            }

            conflictingAssemblies
                .Should().Contain.Item("mscorlib")
                .Should().Contain.Item("System");

            conflictingAssemblies.Should().Count.Exactly(2);
        }

        private IEnumerable<IGrouping<string, Reference>> FindReferencesWithTheSameShortNameButDiffererntFullNames(List<Reference> references)
        {
            return from reference in references
                   group reference by reference.ReferencedAssembly.Name
                       into referenceGroup
                       where referenceGroup.ToList().Select(reference => reference.ReferencedAssembly.FullName).Distinct().Count() > 1
                       select referenceGroup;
        }

        private List<Reference> GetReferencesFromAllAssemblies(List<Assembly> assemblies)
        {
            var references = new List<Reference>();
            foreach (var assembly in assemblies)
            {
                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {
                    references.Add(new Reference
                    {
                        Assembly = assembly.GetName(),
                        ReferencedAssembly = referencedAssembly
                    });
                }
            }
            return references;
        }

        private List<Assembly> GetAllAssemblies(string path)
        {
            var files = new List<FileInfo>();
            var directoryToSearch = new DirectoryInfo(path);
            files.AddRange(directoryToSearch.GetFiles("*.dll", SearchOption.AllDirectories));
            files.AddRange(directoryToSearch.GetFiles("*.exe", SearchOption.AllDirectories));
            return files.ConvertAll(file => Assembly.LoadFile(file.FullName));
        }

        private class Reference
        {
            public AssemblyName Assembly { get; set; }
            public AssemblyName ReferencedAssembly { get; set; }
        }

        public string AssemblyRoot
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }
    }
}