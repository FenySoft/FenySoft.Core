namespace FenySoft.Core.Data;

[TestFixture]
public class TDataTest
{
  [SetUp]
  public void Setup()
  {
  }
  
  // TData constructor test
  [Test]
  public void TDataConstructorTest()
  {
    var data = new TData<int>();
    Assert.AreEqual("0", data.ToString());
  }
  
}