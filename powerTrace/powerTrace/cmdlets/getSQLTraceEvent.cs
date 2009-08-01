using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using Microsoft.SqlServer.Management.Trace;
using System.Data;
using Microsoft.AnalysisServices;

namespace Gosbell.powerTrace
{
    [Cmdlet(VerbsCommon.Get, "SQLTraceEvent")]
    public class getSQLTraceEvent : getSQLTraceBase
    {

    }
}