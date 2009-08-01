using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.ComponentModel;

namespace powerTrace
{
    [RunInstaller(true)]
    public class powerTraceSnapIn :  PSSnapIn
    {
        public override string Name
        {
            get { return "powerTrace"; }
        }
        public override string Vendor
        {
            get { return "Darren Gosbell - http://geekswithblogs.net/darrengosbell"; }
        }
        public override string VendorResource
        {
            get { return "powerTrace, Darren Gosbell - http://geekswithblogs.net/darrengosbell"; }
        }
        public override string Description
        {
            get { return "Provides cmdlets to work with Microsoft SQL Server trace information"; }
        }
        public override string DescriptionResource
        {
            get { return "powerTrace,Provides cmdlets to work with Microsoft SQL Server trace information"; }
        }
    }
}
