
namespace PrettyASN1
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ASNAttribute : Attribute, ICloneable
    {
        public enum ASN1TagTypes
        {
            End_of_Content = 0,
            BOOLEAN = 1,
            INTEGER = 2,
            BIT_STRING = 3,
            OCTET_STRING = 4,
            NULL = 5,
            OBJECT_IDENTIFIER = 6,
            Object_Descriptor = 7,
            EXTERNAL = 8,
            REAL = 9,
            ENUMERATED = 10,
            EMBEDDED_PDV = 11,
            UTF8String = 12,
            RELATIVE_OID = 13,
            TIME = 14,
            Reserved = 15,
            SEQUENCE_and_SEQUENCE_OF = 16,
            SET_and_SET_OF = 17,
            NumericString = 18,
            PrintableString = 19,
            T61String = 20,
            VideotexString = 21,
            IA5String = 22,
            UTCTime = 23,
            GeneralizedTime = 24,
            GraphicString = 25,
            VisibleString = 26,
            GeneralString = 27,
            UniversalString = 28,
            CHARACTER_STRING = 29,
            BMPString = 30,
            DATE = 31,
            TIME_OF_DAY = 32,
            DATE_TIME = 33,
            DURATION = 34,
            OID_IRI = 35,
            RELATIVE_OID_IRI = 36,
            Item = 100,
        }

        public ASN1.ClassTypes classType;
        public ASN1.ConstructorTypes constructorType;

        ASN1TagTypes _TagType;
        public virtual ASN1TagTypes TagType
        {
            get
            {
                return _TagType;
            }
            set
            {
                _TagType = value;
            }
        }
        public int[] TagId = new int[0];

        string _Description;
        public virtual string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        public virtual bool FixedLength
        {
            get
            {
                return _FixedLength;
            }
            set
            {
                _FixedLength = value;
            }
        }
        public virtual int FixedLengthCount
        {
            get
            {
                return _FixedLengthCount;
            }
            set
            {
                _FixedLengthCount = value;
            }
        }
        public virtual int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                _Index = value;
            }
        }
        public virtual bool Implict
        {
            get
            {
                return _Implict;
            }
            set
            {
                _Implict = value;
            }
        }

        bool _FixedLength;
        int _FixedLengthCount;
        int _Index = -1;
        bool _Implict = true;

        public string GetASNFullPath()
        {
            return string.Join(",", TagId);
        }
        public List<ASN1> FindASNs(ASN1 aSN)
        {
            List<ASN1> outasns = new List<ASN1> { aSN };
            foreach (int tag in TagId)
            {
                List<ASN1> toAdd = new List<ASN1>();
                foreach (ASN1 subsan in outasns)
                {
                    foreach (ASN1 Asn in subsan.Parameters.FindAll(x => x.TagId == tag))
                    {
                        toAdd.Add(Asn);
                    }
                }
                outasns = toAdd;


            }
            return outasns;

        }

        public ASN1 FindASN(ASN1 aSN)
        {
            ASN1 asn = aSN;
            if (TagId.Length > 0)
            {
                foreach (int tag in TagId)
                {
                    if (asn != null)
                    {
                        if (Index != -1)
                        {
                            asn = asn.Parameters.FindAll(x => x.TagId == tag)[Index];
                        }
                        else
                        {
                            asn = asn.Parameters.Find(x => x.TagId == tag);
                        }
                    }
                }
            }
            else if (Index != -1)
            {
                asn = asn.Parameters[Index];
            }

            return asn;

        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public ASNAttribute(int[] tagId)
        {
            TagId = tagId;
        }

    }

}
