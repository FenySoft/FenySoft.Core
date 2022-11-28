namespace FenySoft.Core;

[TestFixture]
public class TEnvironmentTest
{
  [SetUp]
  public void Setup()
  {
  }

  // RunningOnMono test
  [Test]
  public void RunningOnMonoTest()
  {
    Assert.IsFalse(TEnvironment.RunningOnMono);
  }
}