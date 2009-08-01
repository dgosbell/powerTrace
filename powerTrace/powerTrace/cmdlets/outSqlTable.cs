using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Collections;
using Microsoft.SqlServer.Management;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;

namespace powerTrace.cmdlets
{
    [Cmdlet(VerbsData.Out, "SqlTable", SupportsShouldProcess = true)]
    public class outSqlTable : Cmdlet
    {

        #region Parameters
        private string mServer;
        [System.Management.Automation.Parameter(Position = 0,
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Help Text")]
        [ValidateNotNullOrEmpty]
        public string Server
        {
            set{mServer =  value;}
            get { return mServer; }
        }

        private string mDatabase;
        [Parameter(Position = 1,
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Help Text")]
        [ValidateNotNullOrEmpty]
        public string Database
        {
            set { mDatabase = value; }
            get { return mDatabase; }
        }

        private string mTable;
        [Parameter(Position = 2,
            Mandatory = false,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Help Text")]
        [ValidateNotNullOrEmpty]
        public string Table
        {
            get { return mTable; }
            set { mTable = value; }
        }

        private PSObject mInputObject;
        [System.Management.Automation.Parameter(Mandatory = true, ValueFromPipeline = true)]
        public PSObject InputObject
        {
            get { return mInputObject; }
            set { mInputObject = value; }
        }

        #endregion

        private Table tb;
        private System.Data.Common.DbCommand cmd = SqlClientFactory.Instance.CreateCommand();

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            Server svr = new Microsoft.SqlServer.Management.Smo.Server(mServer);
            Database db = svr.Databases[mDatabase];
            if (db.Tables.Contains(mTable))
            {
                tb = db.Tables[mTable];
            }
            else
            {
                tb = new Table(db, mTable);
            }
            
            cmd.Connection = new SqlConnection(string.Format("Server={0};Database={1};Trusted_Connection=yes", mServer, mDatabase));
            cmd.Connection.Open();
        }

        protected override void ProcessRecord()
        {
            try
            {
                                
                List<string> sqlValues = new List<string>();
                List<string> sqlColumns = new List<string>();
                bool columnAdded = false;
                foreach (PSPropertyInfo p in mInputObject.Properties )
                {
                    if (p.MemberType == PSMemberTypes.AliasProperty
                        || p.MemberType == PSMemberTypes.CodeProperty
                        || p.MemberType == PSMemberTypes.NoteProperty
                        || p.MemberType == PSMemberTypes.Property
                        || p.MemberType == PSMemberTypes.ScriptProperty)
                    {
                        if (p.Value != null)
                        {
                            if (!tb.Columns.Contains(p.Name))
                            {
                                Column c = new Column(tb, p.Name);
                                c.DataType = getSqlDataType(p.TypeNameOfValue); //DataType.VarCharMax;
                                columnAdded = true;
                                tb.Columns.Add(c);
                            }
                            switch (p.TypeNameOfValue)
                            {
                                case "System.Int32":
                                case "System.Int64": sqlValues.Add(p.Value.ToString());
                                    break;
                                case "System.DateTime": sqlValues.Add("'" + ((DateTime)p.Value).ToString("yyyyMMdd HH:mm:ss.fff") + "'");
                                    break;
                                default: sqlValues.Add("'" + p.Value + "'");
                                    break;
                            }
                            //sqlValues.Add("'" + p.Value + "'");
                            sqlColumns.Add(p.Name);
                            //tb.Columns[p.Name] = mInputObject.Properties[p.Name];
                        }
                    }
                }
                if (tb.State == SqlSmoState.Creating)
                {
                    tb.Create();
                }
                else if (tb.State == SqlSmoState.Existing && columnAdded)
                {
                    tb.AlterWithNoCheck();
                }

                string sqlCmd = string.Format("insert into {0} ({1}) VALUES ({2})", tb.Name, string.Join(",",sqlColumns.ToArray()), string.Join(",",sqlValues.ToArray()));
                
                cmd.CommandText = sqlCmd;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                WriteError( new ErrorRecord(ex,"",ErrorCategory.InvalidData,cmd));
            }
        }
        protected override void EndProcessing()
        {
            base.EndProcessing();
            cmd.Connection.Close();
        }
        protected override void StopProcessing()
        {
            base.StopProcessing();
            cmd.Connection.Close();
        }

        private DataType getSqlDataType(string memberType)
        {
            switch (memberType)
            {
                case "System.DateTime": return DataType.DateTime;
                case "System.Int32": return DataType.Int;
                case "System.Int64": return DataType.BigInt;
                case "System.String": return DataType.VarCharMax;
                default: WriteWarning("MemberType: " + memberType + " not found");
                    return DataType.VarCharMax;
            }
        }
    }
}
