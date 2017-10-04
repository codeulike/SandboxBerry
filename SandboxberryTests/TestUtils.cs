// SandboxBerry - tool for copying test data into Salesforce sandboxes
// Copyright (C) 2017 Ian Finch
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxberryLib;
using SandboxberryLib.InstructionsModel;
using log4net;
using System.Configuration;

namespace SandboxberryTests
{
    public class TestUtils
    {
        
        // fake credentials for tests
        public static SbbCredentials MakeDummyCredentials()
        {
            var res = new SbbCredentials();
            res.SalesforceUrl = "https://test.salesforce.com/services/Soap/u/32.0";
            res.SalesforceLogin = "madeup@example.com.sandbox";
            res.SalesforcePassword = "passwordAndSecurityToken";
            return res;
        }

        public static SbbInstructionSet MakeInstructionSet(List<string> objNames)
        {
            var ret = new SbbInstructionSet();
            ret.SourceCredentials = MakeDummyCredentials();
            ret.TargetCredentuals = MakeDummyCredentials();
            ret.SbbObjects = new List<SbbObject>();
            foreach (string nameLoop in objNames)
            {
                var o = new SbbObject();
                o.ApiName = nameLoop;
                ret.SbbObjects.Add(o);
            }
            return ret;
        }
    }
}
