using System;
using System.Collections.Generic;

namespace SandboxberryLib.ResultsModel
{
    /// <summary>
    /// Stores information about lookup relationships and the referenced IDs, sometimes they can't
    /// be populated during the initial import (when the referenced object doesn't exist yet) and
    /// need to be reprocessed after
    /// </summary>
    public class LookupInfo
    {
        public LookupInfo()
        {
            this.IdPairs = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// API Name of the object that owns the lookup relationship field
        /// </summary>
        public String ObjectName { get; set; }

        /// <summary>
        /// API Name of the object that the lookup relationship field references
        /// </summary>
        public String RelatedObjectName { get; set; }

        /// <summary>
        /// API Name of the lookup relationship field
        /// </summary>
        public String FieldName { get; set; }

        /// <summary>
        /// IDs used by this lookup relationship in the form of key=object, value=referenced object,
        /// saved during initial processing for reprocessing later
        /// </summary>
        public List<KeyValuePair<string, string>> IdPairs { get; set; }
    }
}