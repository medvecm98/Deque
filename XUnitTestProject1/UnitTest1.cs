using System;
using System.Text;
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

        [Fact]
        public void InsertTest()
        {
            //Arrange
            var d = new Deque<int>();
            var rev = new ReverseDeque<int>(d);

            //Act
            rev.Insert(0, 5);
            rev.Insert(0, 4);
            rev.Insert(0, 3);
            rev.Insert(0, 2);
            rev.Insert(0, 1);
            d.Insert(0, 5);
            d.Insert(0, 4);
            d.Insert(0, 3);
            d.Insert(0, 2);
            d.Insert(0, 1);

            var sb = new StringBuilder();
            foreach (var i in rev)
            {
                sb.Append(i.ToString() + " ");
            }

            //Assert
            Assert.Equal(10, d.Count);
            Assert.Equal(10, rev.Count);
            Assert.Equal(1, d[1]);
            Assert.Equal(2, rev[1]);
            Assert.Equal("5 1 2 3 4 5 4 3 2 1", sb.ToString().Trim());

        }
    }
}
