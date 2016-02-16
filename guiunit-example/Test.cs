using System;
using Foundation;
using NUnit.Framework;

namespace GUITest 
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void NSAttribtedStringTest()
        {
		NSAttributedString a = new NSAttributedString ("asdf");
		Console.WriteLine (a);
        }
    }
}
