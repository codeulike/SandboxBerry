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
using SandboxberryLib.InstructionsModel;
using SandboxberryLib.ResultsModel;
using SandboxberryLib.SalesforcePartnerApi;
using log4net;
using System.Xml;

namespace SandboxberryLib
{
    public class ObjectTransformer
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ObjectTransformer));

        public RelationMapper RelationMapper { get; set; }

        public string RecursiveRelationshipField { get; set; }

        public Dictionary<string, string> ObjectRelationships { get; set; }

        public List<string> InactiveUserIds { get; set; }

        public List<string> MissingUserIds { get; set; }

        public string CurrentUserId { get; set; }

        public SbbObject SbbObjectInstructions { get; set; }

        public void ApplyTransformations(sObjectWrapper wrap)
        {
            CorrectInactiveUser(wrap.sObj, this.InactiveUserIds, this.CurrentUserId);
            if (this.RecursiveRelationshipField != null)
                RememberRecursiveId(wrap, this.RecursiveRelationshipField);
            FixRelatedIds(wrap.sObj, this.ObjectRelationships);
            RemoveIdFromSObject(wrap.sObj);

            foreach (var fieldOptions in SbbObjectInstructions.SbbFieldOptions)
            {
                XmlElement fieldNode = wrap.sObj.Any.FirstOrDefault(e => e.LocalName == fieldOptions.ApiName);
                if (fieldNode == null)
                {
                    logger.DebugFormat("SbbFieldOption specified for {0}.{1} but could not find field - perhaps removed for this row by relationship mapper?",
                        SbbObjectInstructions.ApiName, fieldOptions.ApiName);
                }
                else
                {
                    if (!string.IsNullOrEmpty(fieldOptions.ReplaceText))
                    {
                        fieldNode.InnerText = fieldOptions.ReplaceText;
                        fieldNode.IsEmpty = false;

                        fieldNode.Attributes.RemoveNamedItem("xsi:nil");
                    }
                    if (fieldOptions.Skip)
                    {
                        wrap.sObj.Any = wrap.sObj.Any.Where(f => f.LocalName != fieldOptions.ApiName).ToArray();
                    }
                }
                
            }

        }



        public void CorrectInactiveUser(sObject obj, List<string> inactiveUserIds, string replacementUserID)
        {
            XmlElement ownerNode = obj.Any.FirstOrDefault(e => e.LocalName == "OwnerId");
            if (ownerNode == null)
            {
                logger.DebugFormat("Object {0} does not have OwnerID field",
                    obj.type);
                return;
            }
            if (inactiveUserIds.Contains(ownerNode.InnerText))
            {
                logger.DebugFormat("Object {0} {1} has inactive owner {2} so correcting to {3}",
                    obj.type, obj.Id, ownerNode.InnerText, replacementUserID);

                ownerNode.InnerText = replacementUserID;
            }


        }

        public void FixRelatedIds(sObject obj, Dictionary<string, string> objectRelationships)
        {
            foreach (string fieldName in objectRelationships.Keys)
            {
                string destType = objectRelationships[fieldName];
                XmlElement relatedIdNode = obj.Any.FirstOrDefault(e => e.LocalName == fieldName);
                if (relatedIdNode == null)
                {
                    logger.DebugFormat("Object {0} {1} could not find data for field {2}",
                        obj.type, obj.Id, fieldName);
                    continue;
                }


                string currentValue = relatedIdNode.InnerText;
                if (!string.IsNullOrEmpty(currentValue))
                {
                    if (destType == "User")
                    {
                        // if user id is missing from target then we need to blank
                        if (this.MissingUserIds.Contains(currentValue))
                        {
                            logger.DebugFormat("Object {0} {1} lookup field {2} (points to User) has User ID {3} which is missing from Target - will clear id",
                                obj.type, obj.Id, fieldName, currentValue);
                            obj.Any = obj.Any.Where(e => e.LocalName != relatedIdNode.LocalName).ToArray();
                        }
                    }
                    else
                    {
                        string replaceValue = this.RelationMapper.RecallNewId(destType, currentValue);
                        if (replaceValue != null)
                        {
                            relatedIdNode.InnerText = replaceValue;
                            logger.DebugFormat("Object {0} {1} replaced lookup field {2} (points to {3}) value {4} with {5}",
                                obj.type, obj.Id, fieldName, destType, currentValue, replaceValue);

                        }
                        else
                        {
                            logger.DebugFormat("Object {0} {1} lookup field {2} (points to {3}) could not translate - will clear id",
                                obj.type, obj.Id, fieldName, destType);
                            obj.Any = obj.Any.Where(e => e.LocalName != relatedIdNode.LocalName).ToArray();
                        }
                    }
                }


            }

        }

        public void RemoveIdFromSObject(sObject obj)
        {
            obj.Any = obj.Any.Where(e => e.LocalName != "Id").ToArray();
            obj.Id = null;

        }

        private void RememberRecursiveId(ObjectTransformer.sObjectWrapper wrap, string recursiveField)
        {
            XmlElement recursiveNode = wrap.sObj.Any.FirstOrDefault(e => e.LocalName == recursiveField);
            wrap.RecursiveRelationshipOriginalId = recursiveNode.InnerText;


        }



        public class sObjectWrapper
        {
            public string OriginalId { get; set; }
            public sObject sObj { get; set; }
            public string NewId { get; set; }
            public string ErrorMessage { get; set; }

            public string RecursiveRelationshipField { get; set; }
            public string RecursiveRelationshipOriginalId { get; set; }
        }

    }
}
