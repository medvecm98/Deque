using System;
using Xunit;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            Deque<int> d = new Deque<int>();
            var rev = new ReverseDeque<int>(d);

            //Act
            d.Insert(0, 1);
            d.Insert(0, 2);
            d.Add(3);
            rev.Add(4); // REAR 4, 2, 1, 3 FRONT

            //Assert
            Assert.Equal(3, rev[d.IndexOf(4)]);
            Assert.Equal(4, d.Count);
            Assert.Equal(4, rev.Count);
            Assert.Equal(2, d[1]);
        }
    }
}
