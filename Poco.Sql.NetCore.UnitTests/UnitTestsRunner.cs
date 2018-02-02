using System;
using System.Collections.Generic;
using System.Text;

namespace Poco.Sql.NetCore.UnitTests
{
    public class UnitTestsRunner
    {
        public bool RunTest(Action testMethod)
        {
            try
            {
                testMethod.Invoke();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool RunTest(Action testMethod, string testText, string passText, string failText)
        {
            if (RunTest(testMethod))
            {
                
            }
        }
    }
}
