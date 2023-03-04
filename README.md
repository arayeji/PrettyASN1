# PrettyASN1
Pretty ASN1 Decoder and Encoder

Load ASN1 File:

```C#
ASN1 asn = new ASN1("template.dat");
```

Load ASN1 Byte Array:

```C#
ASN1 asn = new ASN1(TemplateByteArray);
```

Steps to decode specific ASN1 file like TAP3:

1. Create TAP3.cs

```C#
using System.Reflection;
using PrettyASN1;

namespace PrettyASN1Tester
{
    [ASN(new[] { 1 })]
    public class TransferBatch
    {
        public ASN1 ASN;
         

        [ASN(new[] { 3 })]
        public CallEventDetail callEventDetails;
         

        public TransferBatch(ASN1 aSN)
        {
            ASN = aSN;
            ASN1.Initialize(this, ASN);
        }
    }

    public class CallEventDetail
    {
        public ASN1 ASN;

        [ASN(new[] { 9 })]
        public List<MobileOriginatedCall> mobileOriginatedCall;
         
        public CallEventDetail(ASN1 aSN)
        {
            ASN = aSN;
            ASN1.Initialize(this, ASN);
        }
    }


    public class MobileOriginatedCall
    {
        public ASN1 ASN;

        

        [ASN(new[] { 429 })]
        public ImeiOrEsn equipmentIdentifier;
         
        [ASN(new[] { 209 })]
        public string supplServiceCode
        {
            get
            {
                return ASN1.Get(this, ASN, MethodBase.GetCurrentMethod());
            }
            set
            {
                ASN1.Set(this, ASN, MethodBase.GetCurrentMethod(), value);
            }
        }

         

        [ASN(new[] { 162 })]
        public string operatorSpecInformation
        {
            get
            {
                return ASN1.Get(this, ASN, MethodBase.GetCurrentMethod());
            }
            set
            {
                ASN1.Set(this, ASN, MethodBase.GetCurrentMethod(), value);
            }
        }


        public MobileOriginatedCall(ASN1 aSN)
        {
            ASN = aSN;
            ASN1.Initialize(this, ASN);
        }

        public void Delete()
        {
            ASN.Delete();
        }
    }

    public class ImeiOrEsn
    {
        public ASN1 ASN;

        [ASN(new[] { 128 }, TagType = ASNAttribute.ASN1TagTypes.OCTET_STRING)]
        public string imei
        {
            get
            {
                return ASN1.Get(this, ASN, MethodBase.GetCurrentMethod());
            }
            set
            {
                ASN1.Set(this, ASN, MethodBase.GetCurrentMethod(), value);
            }
        }


        [ASN(new[] { 103 })]
        public string esn
        {
            get
            {
                return ASN1.Get(this, ASN, MethodBase.GetCurrentMethod());
            }
            set
            {
                ASN1.Set(this, ASN, MethodBase.GetCurrentMethod(), value);
            }
        }

        public ImeiOrEsn(ASN1 aSN)
        {
            ASN = aSN;
            ASN1.Initialize(this, ASN);
        }
    }
}

```

2. Load file:

```C#
ASN1 asn = new ASN1("template.dat");
TransferBatch Template = new TransferBatch(asn);
```





