using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestApplication
{
  [TestClass]
  public class UnitTestApplication
  {
    [TestMethod]
    public void TestMethod_GenerateRndNumberUsingCrypto_same_number()
    {
      int source1 = 2;
      int source2 = 2;
      int expected = 2;
      int result = WinForm2048.FormMain.GenerateRndNumberUsingCrypto(source1, source2);
      Assert.AreEqual(result, expected);
    }
  }
}
