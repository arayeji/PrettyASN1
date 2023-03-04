using PrettyASN1;
namespace PrettyASN1Tester
{
    [TestClass]
    public class PrettyTester
    {
        [TestMethod]
        public void CheckASN1Parser()
        {
            byte[] ASN = File.ReadAllBytes("template.dat");
            ASN1 asn = new ASN1(ASN);
            Assert.AreEqual(asn.Parameters.Count(), 5);
            Assert.AreEqual(asn.TagId, 1);
            Assert.AreEqual(asn.ToBytesNew().ByteArrayToString(), ASN.ByteArrayToString());
        }

        [TestMethod]
        public void CheckHuaweiASN1CDR()
        {
            byte[] ASN = File.ReadAllBytes("template.dat");
            ASN1 asn = new ASN1(ASN);
          
            Assert.AreEqual(asn.Parameters.Count(), 5);
            Assert.AreEqual(asn.TagId, 1);
            Assert.AreEqual(asn.ToBytesNew().ByteArrayToString(), ASN.ByteArrayToString());

            TransferBatch Template = new TransferBatch(asn);
            Assert.AreEqual(Template.callEventDetails.mobileOriginatedCall.Count(), 2);
            Assert.AreEqual(Template.callEventDetails.mobileOriginatedCall[0].equipmentIdentifier.imei, "3510945672656400");
        }
    }
}