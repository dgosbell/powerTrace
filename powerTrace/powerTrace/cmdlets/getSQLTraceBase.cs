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
    public class getSQLTraceBase : PSCmdlet
    {
        private string mTraceFile = "";
        [Parameter(Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The path to the .trc file"
            )]
        [ValidateNotNullOrEmpty]
        public string TraceFile
        {
            get { return mTraceFile; }
            set
            {
                mTraceFile = value;
            }
        }

        protected virtual bool OutputSSASTraceSubclasses
        {
            get
            {
                return false;
            }
        }

        protected override void ProcessRecord()
        {

            // this resolves relative paths and wildcards to a collection of resolved paths
            foreach (PathInfo filePath in this.SessionState.Path.GetResolvedPSPathFromPSPath(mTraceFile))
            {
                TraceFile tf = new TraceFile();
                tf.InitializeAsReader(filePath.ProviderPath);
                while (tf.Read())
                {
                    // dynamically build a powershell response object based on 
                    // the available fields in the trace file.
                    PSObject pso = new PSObject();
                    for (int i = 0; i < tf.FieldCount; i++)
                    {
                        if (!tf.IsNull(i) && tf.GetFieldType(i) == typeof(System.DateTime))
                        {
                            DateTime dt = tf.GetDateTime(i);
                            dt = new DateTime(dt.Ticks, DateTimeKind.Utc);
                            pso.Properties.Add(new PSNoteProperty(tf.GetName(i), dt.ToLocalTime()));
                        }
                        else
                        {
                            pso.Properties.Add(new PSNoteProperty(tf.GetName(i), tf.GetValue(i)));
                        }
                    }
                    if (!tf.IsNull(tf.GetOrdinal("EventSubclass")) && OutputSSASTraceSubclasses)
                    {
                        //pso.Properties.Add(new PSNoteProperty("EventSubClassName",tf.TranslateSubclass((string)tf.GetValue(tf.GetOrdinal("EventClass")),"1" , (int)tf.GetValue(tf.GetOrdinal("EventSubclass")) )));
                        pso.Properties.Add(new PSNoteProperty("EventSubclassName"
                            , System.Enum.GetName(typeof(Microsoft.AnalysisServices.TraceEventSubclass), tf.GetValue(tf.GetOrdinal("EventSubclass")))
                            ));
                    }
                    WriteObject(pso);
                }
                tf.Close();
            }
        }
    }
}
