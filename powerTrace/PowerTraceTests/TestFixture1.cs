using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Gosbell.powerTrace;

namespace PowerTraceTests
{
    [TestFixture]
    public class TestFixture1
    {
        [Test]
        public void Test()
        {
            //
            // TODO: Add test logic here
            //
            Gosbell.powerTrace.getASTraceEvent st = new getASTraceEvent();
            st.TraceFile = "C:\\Data\\Projects\\powerTrace\\FlightRecorderBack.trc";
            System.Collections.IEnumerable res =  st.Invoke();
            System.Collections.IEnumerator e = res.GetEnumerator();
            int cnt = 0;
            while (e.MoveNext())
            {
                cnt++;
            }
            Assert.GreaterThan(cnt,0);
        }
    }
}
