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
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;
using log4net;

namespace SandboxberryLib.InstructionsModel
{
    public class SbbInstructionSet
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SbbInstructionSet));



        [XmlIgnore]
        public SbbCredentials SourceCredentials { get; set; }

        [XmlIgnore]
        public SbbCredentials TargetCredentuals { get; set; }

        public List<SbbObject> SbbObjects { get; set; }

        public static void SaveToFile(string filename, SbbInstructionSet inst)
        {
        
            using (var writer = new System.IO.StreamWriter(filename))
            {
                var serializer = new XmlSerializer(inst.GetType());
                serializer.Serialize(writer, inst);
                writer.Flush();
            }
        
        }

        public static SbbInstructionSet LoadFromFile(string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(GetInstructionSetSchema());
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags =
                           XmlSchemaValidationFlags.ProcessIdentityConstraints |
                           XmlSchemaValidationFlags.ReportValidationWarnings;
            XmlSchemaException firstException = null;
            settings.ValidationEventHandler +=
                delegate(object sender, ValidationEventArgs args)
                {
                    if (args.Severity == XmlSeverityType.Warning)
                    {
                        logger.DebugFormat("Warning from Xml Validations: {0}", args.Message);
                    }
                    else
                    {
                        if (firstException == null)
                        {
                            firstException = args.Exception;
                        }
                        
                        logger.Error(string.Format("Error from Xml Validation at Line {0} Pos {1}",
                            args.Exception.LineNumber, args.Exception.LinePosition), args.Exception);
                    }
                };

            var res = new SbbInstructionSet();
            using (var stream = System.IO.File.OpenRead(filename))
            {
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    XmlSerializer ser = new XmlSerializer(res.GetType());
                    res = ser.Deserialize(reader) as SbbInstructionSet;
                }

            }

            if (firstException != null)
            {
                throw new ApplicationException(string.Format("Could not load instruction files due to XML validation error at Line {0} Position {1}: {2}",
                     firstException.LineNumber, firstException.LinePosition, firstException.Message), firstException);
            }

            return res;
        }

        public static XmlSchema GetInstructionSetSchema()
        {
            XmlSchema schema = null;
            using(var reader = XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("SandboxberryLib.Resources.SbbInstructionSetSchema.xsd")))
            {
                schema = XmlSchema.Read(reader,
                    new ValidationEventHandler(
                    delegate(Object sender, ValidationEventArgs e)
                    {
                        logger.Error(string.Format("Error while loading Schema File"), e.Exception);
                        throw new ApplicationException(string.Format("Schema File Validation: {0}", e.Message), e.Exception);
                    }));    
            }
            return schema;
        }
    }
}
