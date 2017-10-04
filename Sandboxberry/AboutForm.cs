using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sandboxberry
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            PopulateAbout();
        }

        private void uxOkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PopulateAbout()
        {
            Version v = Assembly.GetEntryAssembly().GetName().Version;
            String verString = v.ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append("SandboxBerry" + Environment.NewLine + Environment.NewLine);
            sb.AppendFormat("Version {0}" + Environment.NewLine + Environment.NewLine, verString);
            sb.Append("(C) Ian Finch 2017" + Environment.NewLine + Environment.NewLine);
            sb.Append("This program is free software; you can redistribute it and/or ");
            sb.Append("modify it under the terms of the GNU General Public License ");
            sb.Append("as published by the Free Software Foundation; either version 2 ");
            sb.Append("of the License, or (at your option) any later version." + Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("This program is distributed in the hope that it will be useful, ");
            sb.Append("but WITHOUT ANY WARRANTY; without even the implied warranty of ");
            sb.Append("MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the ");
            sb.Append("GNU General Public License for more details." + Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("You should have received a copy of the GNU General Public License ");
            sb.Append("along with this program; if not, write to the Free Software ");
            sb.Append("Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA." + Environment.NewLine);

            uxAboutText.Text = sb.ToString();
            uxAboutText.Select(0, 0);
        }

        private void uxHomeLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://github.com/codeulike/sandboxberry");
            Process.Start(sInfo);
        }

        private void uxIconsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://icons8.com");
            Process.Start(sInfo);
            
        }

        private void uxAboutText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
