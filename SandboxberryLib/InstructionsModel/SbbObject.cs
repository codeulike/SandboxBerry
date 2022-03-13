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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace SandboxberryLib.InstructionsModel
{
    public class SbbObject
    {
        public SbbObject()
        {
            this.SbbFieldOptions = new List<SbbFieldOption>();
        }

        [XmlAttribute]
        public string ApiName { get; set; }
        [XmlAttribute]
        public string Filter { get; set; }

        [XmlAttribute]
        public string Limit { get; set; }

        public List<SbbFieldOption> SbbFieldOptions { get; set; }

        // controls whether SbbFieldOptions list is serialized
        [XmlIgnore]
        public bool SbbFieldOptionsSpecified
        {
            get { return (SbbFieldOptions != null && SbbFieldOptions.Count > 0); }
        }
    }
}
