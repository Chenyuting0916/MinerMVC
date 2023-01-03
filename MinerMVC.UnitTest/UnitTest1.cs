using System.Collections.Generic;
using NUnit.Framework;

namespace MinerMVC.UnitTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var linkedList = new LinkedList<string>();
        linkedList.AddFirst("123");
        Assert.Pass();
    }
}