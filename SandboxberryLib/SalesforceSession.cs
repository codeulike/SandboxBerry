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
using System.Configuration;
using System.Web.Services;

using SandboxberryLib.SalesforcePartnerApi;
using System.Web.Services.Protocols;
using SandboxberryLib.InstructionsModel;
using System.Net;

namespace SandboxberryLib
{
    public class SalesforceSession
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SalesforceSession));

        private SbbCredentials _credentials;

        public SalesforceSession(SbbCredentials cred)
        {
            _credentials = cred;
            // set up for TLS1.2 or TLS1.1 but not TLS1.0
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }

        public SforceService Login()
        {

            SforceService service = new SforceService();
            service.Url = _credentials.SalesforceUrl;

            
            LoginResult loginResult;
            try
            {
                loginResult = service.login(_credentials.SalesforceLogin, _credentials.SalesforcePassword);
            }
            catch (SoapException ex)
            {
                logger.Error(string.Format("error during attempted login for {0}", _credentials.SalesforceLogin), ex);
                throw ex;

            }

            /** 
              * Once the client application has logged in successfully, it will use
              * the results of the login call to reset the endpoint of the service
              * to the virtual server instance that is servicing your organization
            */
            service.Url = loginResult.serverUrl;

            /** 
            * The client application now has an instance of the SforceService
            * that is pointing to the correct endpoint. Next, the sample client
            * application sets a persistent SOAP header (to be included on all
            * subsequent calls that are made with SforceService) that contains the
            * valid sessionId for our login credentials. To do this, the 
            * client application creates a new SessionHeader object and persists it to
            * the SforceService. Add the session ID returned from the login to the
            * session header.
            */
            service.SessionHeaderValue = new SessionHeader();
            service.SessionHeaderValue.sessionId = loginResult.sessionId;

            logger.DebugFormat("Successful login for {0}", _credentials.SalesforceLogin);

            return service;
        }

        // like login, but doesn't return anything
        public void TestLogin()
        {
            var dummy = this.Login();

        }

        public bool AllowDeletion()
        {
            bool allowDelete = false;
            if (_credentials.SalesforceUrl.ToLower().StartsWith("https://test.salesforce.com"))
                allowDelete = true;
            return allowDelete;
        }


    }
}
