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
using log4net;

namespace SandboxberryLib
{
    public class RelationMapper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RelationMapper));


        private Dictionary<string, Dictionary<string, string>> _idMap;

        public RelationMapper()
        {
            _idMap = new Dictionary<string, Dictionary<string, string>>();
        }

        public void Remember(string apiName, string originalId, string newId)
        {
            if (!_idMap.ContainsKey(apiName))
                _idMap.Add(apiName, new Dictionary<string, string>());

            var objDict = _idMap[apiName];

            if (!objDict.ContainsKey(originalId))
                objDict.Add(originalId, newId);


        }

        public string RecallNewId(string apiName, string originalId)
        {
            if (!_idMap.ContainsKey(apiName))
            {
                logger.DebugFormat("RecallNewId: no ids mapped for object {0}", apiName);
                return null;
            }

            var objDict = _idMap[apiName];

            if (!objDict.ContainsKey(originalId))
            {
                logger.DebugFormat("RecallNewId: object {0} {1} does not have a newId saved in the map",
                    apiName, originalId);
                return null;
            }

            return objDict[originalId];
        }

    }
}
