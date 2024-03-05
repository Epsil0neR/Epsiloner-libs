using System;
using Xunit;

namespace Tests.Epsil0ner.Core
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.Throws<NotImplementedException>(MyMethod);
        }

        private void MyMethod()
        {
            throw new NotImplementedException();
        }
    }
}
