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
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace Sandboxberry
{
    static class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        private static MainForm _mainForm = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            logger.DebugFormat("Main() starting");
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _mainForm = new MainForm();
            Application.Run(_mainForm);
            logger.DebugFormat("Main() finishing");
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
            Exception ex = e.ExceptionObject as Exception;
            logger.Error("CurrentDomain_UnhandledException caught an error", ex);

            MessageBox.Show(string.Format("Unexpected error: {0} \r\n\r\n See log for details", ex.Message), "Unexpected Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (_mainForm != null)
                _mainForm.DisableForm();
            else
                Application.Exit();
            
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // nb this catches the original base exception, discarding any outer ones, see http://stackoverflow.com/q/8565761/22194
            Exception ex = e.Exception;
            logger.Error("Application_ThreadException caught an error", ex);

            MessageBox.Show(string.Format("Unexpected error: {0} \r\n\r\n See log for details", ex.Message), "Unexpected Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (_mainForm != null)
                _mainForm.DisableForm();
            else
                Application.Exit();

            
        }
    }
}
