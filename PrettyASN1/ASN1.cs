using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PrettyASN1
{
    public class ASN1 : ICloneable
    {
        public object Clone()
        {
            return new ASN1(this.ToBytesNew());
        }

        public int TagId;
        public void Delete()
        {
            ParentsASNs[ParentsASNs.Count - 1].Parameters.Remove(this);
        }
        //public ASN1(int tagId, List<ASN1> parentsASNs, List<ASN1> parameters,  ClassTypes classType, ConstructorTypes constructorType, ASNAttribute.ASN1TagTypes aSN1TagType, byte[] valueBytes, int length, bool isIndefinite)
        //{
        //    TagId = tagId;
        //    ParentsASNs = parentsASNs;
        //    Parameters = parameters;
        //    this.isSet = isSet;
        //    ClassType = classType;
        //    ConstructorType = constructorType;
        //    ASN1TagType = aSN1TagType;
        //    ValueBytes = valueBytes;
        //    Length = length;
        //    this.isIndefinite = isIndefinite;
        //}

        public ASN1(ASNAttribute asn, List<ASN1> parentsASNs, List<ASN1> parameters)
        {
            TagId = asn.TagId[asn.TagId.Length - 1];
            if (asn.FixedLength)
                ValueBytes = new byte[asn.FixedLengthCount];
            ParentsASNs = parentsASNs;
            Parameters = parameters;
            ClassType = asn.classType;
            ConstructorType = asn.constructorType;
            ASN1TagType = asn.TagType;
        }
        public ASN1(ASNAttribute asn, List<ASN1> parentsASNs, List<ASN1> parameters, byte[] value)
        {
            TagId = asn.TagId[asn.TagId.Length - 1];
            if (asn.FixedLength)
                ValueBytes = new byte[asn.FixedLengthCount];

            if (value != null)
                ValueBytes = value;
            ParentsASNs = parentsASNs;
            Parameters = parameters;
            ClassType = asn.classType;
            ConstructorType = asn.constructorType;
            ASN1TagType = asn.TagType;
        }

        public List<ASN1> ParentsASNs = new List<ASN1>();
        public List<ASN1> Parameters = new List<ASN1>();
        public bool isSet = false;
        public ClassTypes ClassType;
        public ConstructorTypes ConstructorType;
        public ASNAttribute.ASN1TagTypes ASN1TagType = ASNAttribute.ASN1TagTypes.NULL;
        public string GetASNPath()
        {
            List<int> Tags = new List<int>();
            Tags.AddRange(ParentsASNs.Select(x => x.TagId));
            Tags.Add(TagId);
            return string.Join(",", Tags);
        }
        public byte[] ValueBytes;
        public int Length;
        public bool isIndefinite;

        public void SetValue(object NewValue, ASNAttribute attribute)
        {
            if (attribute.TagType != ASNAttribute.ASN1TagTypes.NULL && attribute.TagType != ASNAttribute.ASN1TagTypes.End_of_Content)
            {
                ASN1TagType = attribute.TagType;
            }
            switch (Type.GetTypeCode(NewValue.GetType()))
            {
                case TypeCode.UInt32:
                    if (attribute.FixedLength)
                    {
                        ValueBytes = Utils.IntToByte((uint)NewValue, attribute.FixedLengthCount);
                    }
                    else
                        ValueBytes = ((uint)NewValue).ToByte();
                    break;
                case TypeCode.Int32:
                    if (attribute.FixedLength)
                    {
                        ValueBytes = Utils.IntToByte((int)NewValue, attribute.FixedLengthCount);
                    }
                    else
                        ValueBytes = ((int)NewValue).ToByte();
                    break;
                case TypeCode.String:
                    switch (ASN1TagType)
                    {
                        case ASNAttribute.ASN1TagTypes.UTF8String:
                        case ASNAttribute.ASN1TagTypes.VisibleString:
                        default:
                            {
                                switch (Type.GetTypeCode(NewValue.GetType()))
                                {
                                    case TypeCode.String:
                                        ValueBytes = Encoding.UTF8.GetBytes((string)NewValue);
                                        break;

                                    case TypeCode.Byte:
                                        ValueBytes = (byte[])NewValue;
                                        break;

                                    default:
                                        break;

                                }
                            }
                            break;

                        case ASNAttribute.ASN1TagTypes.OBJECT_IDENTIFIER:
                        case ASNAttribute.ASN1TagTypes.OCTET_STRING:
                            {
                                switch (Type.GetTypeCode(NewValue.GetType()))
                                {
                                    case TypeCode.String:
                                        ValueBytes = ((string)NewValue).ToByteArray();
                                        break;

                                    case TypeCode.Byte:
                                        ValueBytes = (byte[])NewValue;
                                        break;

                                    default:
                                        break;

                                }
                            }
                            break;
                    }
                    break;

                case TypeCode.Byte:
                    switch (ASN1TagType)
                    {

                        case ASNAttribute.ASN1TagTypes.NULL:
                            ValueBytes = (byte[])NewValue;
                            break;
                        case ASNAttribute.ASN1TagTypes.INTEGER:
                            if (attribute.FixedLength)
                            {
                                ValueBytes = Utils.IntToByte((int)NewValue, attribute.FixedLengthCount);
                            }
                            else
                                ValueBytes = ((int)NewValue).ToByte();
                            break;
                        case ASNAttribute.ASN1TagTypes.OBJECT_IDENTIFIER:
                        case ASNAttribute.ASN1TagTypes.UTF8String:
                        case ASNAttribute.ASN1TagTypes.VisibleString:
                        case ASNAttribute.ASN1TagTypes.OCTET_STRING:
                            {
                                switch (Type.GetTypeCode(NewValue.GetType()))
                                {
                                    case TypeCode.String:
                                        ValueBytes = Encoding.UTF8.GetBytes((string)NewValue);
                                        break;

                                    case TypeCode.Byte:
                                        ValueBytes = (byte[])NewValue;
                                        break;

                                    default:
                                        break;

                                }
                            }
                            break;
                        case ASNAttribute.ASN1TagTypes.DATE_TIME:
                            ValueBytes = (Encoding.ASCII.GetBytes(((DateTime)NewValue).ToString("yyMMddHHmmss"))).MirrorBytes();

                            break;
                    }
                    break;
            }
        }

        public object GetValue(TypeCode type)
        {

            switch (type)
            {
                case TypeCode.UInt32:
                    return (ValueBytes).ToInt<uint>();
                case TypeCode.Int32:
                    return (ValueBytes).ToInt();
                case TypeCode.String:
                    return (ValueBytes).ByteArrayToString();
                case TypeCode.Byte:
                    return (ValueBytes);
                default:
                    return ValueBytes;
            }
        }

        byte[] GetTagId(ClassTypes ClassType, ConstructorTypes ConstructorType, int TagId)
        {
            BitArray ct = new BitArray(((int)ClassType).ToByte());
            BitArray tg = new BitArray((TagId).ToByte());
            BitArray cot = new BitArray(((int)ConstructorType).ToByte());


            BitArray ba = new BitArray(new bool[] { tg[0], tg[1], tg[2], tg[3], tg[4], cot[0], ct[0], ct[1] });
            byte[] seq = new byte[1];
            ba.CopyTo(seq, 0);


            if (TagId > 30)
            {
                BitArray ba1 = new BitArray(new bool[] { true, true, true, true, true, cot[0], ct[0], ct[1] });
                byte[] seq1 = new byte[1];
                ba1.CopyTo(seq1, 0);

                BitArray ba2 = new BitArray(new int[] { TagId });

                int nTagId = ((new BitArray(new bool[] { ba2[0], ba2[1], ba2[2], ba2[3], ba2[4], ba2[5], ba2[6], false })).ToInt());
                if (TagId >= 128)
                {
                    int x = (TagId / 128) + 128;
                    seq = new byte[] { seq1[0], (byte)x, (byte)nTagId };
                }
                else
                    seq = new byte[] { seq1[0], (byte)nTagId };

            }

            return seq;
        }

        public byte[] ToBytesNew()
        {
            List<byte> bytes = new List<byte>();

            byte[] Tag = GetTagId(ClassType, ConstructorType, TagId);

            List<byte> pBytes = new List<byte>();

            bytes.AddRange(Tag);

            if (Parameters.Count == 0 && ValueBytes != null)
            {
                pBytes.AddRange(ValueBytes);
            }


            foreach (ASN1 param in Parameters)
            {
                if (param != null)
                    pBytes.AddRange(param.ToBytesNew());
            }

            if (isIndefinite || Length == 0)
                bytes.Add(128);
            else
            {
                if (pBytes.Count > 128)
                {
                    bytes.Add(129);
                }
                bytes.Add((byte)pBytes.Count);
            }

            bytes.AddRange(pBytes);

            if (isIndefinite || Length == 0)
                bytes.AddRange(new byte[] { 0, 0 });




            return bytes.ToArray();
        }

        public class ASNAddedEventArgs : EventArgs
        {
            public ASNAddedEventArgs(ASN1 message)
            {
                Message = message;
            }

            public ASN1 Message { get; set; }
        }

        public event EventHandler<ASNAddedEventArgs> RaiseASNAddedEventArgs;
        protected virtual void OnASNAdded(ASNAddedEventArgs e)
        {
            EventHandler<ASNAddedEventArgs> handler = RaiseASNAddedEventArgs;
            if (handler != null)
            {
                handler(this, e);
            }
            else
            {

            }
        }

        void ReadSequence(MemoryStream ms, ASN1 Parent)
        {

            BinaryReader br = new BinaryReader(ms);
            while (ms.Position < ms.Length)
            {

                BitArray ba = new BitArray(br.ReadBytes(1));
                ASN1 aSN = new ASN1();

                aSN.ClassType = (ClassTypes)((new BitArray(new bool[] { ba[6], ba[7] })).ToInt());
                aSN.ConstructorType = (ConstructorTypes)((new BitArray(new bool[] { ba[5] })).ToInt());
                aSN.TagId = ((new BitArray(new bool[] { ba[0], ba[1], ba[2], ba[3], ba[4] })).ToInt());


                if (aSN.TagId > 30 && ba[0] == true && ba[1] == true && ba[2] == true && ba[3] == true && ba[4] == true)
                {

                    aSN.TagId = br.ReadByte();

                    if (aSN.TagId > 128)
                    {
                        if (aSN.TagId > 129)
                        {

                        }
                        ba = new BitArray(br.ReadBytes(1));
                        int HighTag = (aSN.TagId - 128) * 128;
                        aSN.TagId = ((new BitArray(new bool[] { ba[0], ba[1], ba[2], ba[3], ba[4], ba[5], ba[6], false })).ToInt()) + HighTag;

                        if (aSN.TagId > 256)
                        {

                        }
                    }
                }

                if (aSN.TagId == 56)
                {
                }


                byte[] LenByte = br.ReadBytes(1);

                BitArray Len = new BitArray(LenByte);
                if (aSN.TagId == 0 && LenByte.Length > 0 && LenByte[0] == 0)
                {

                    break;
                }
                if (Len[7])
                {
                    int MoreBytes = ((new BitArray(new bool[] { Len[0], Len[1], Len[2], Len[3], Len[4], Len[5], Len[6] })).ToInt());
                    if (MoreBytes == 0)
                    {
                        aSN.isIndefinite = true;

                        ReadSequence(ms, aSN);
                        aSN.Length = 0;

                    }
                    else
                    {
                        byte[] tLen = br.ReadBytes(MoreBytes);
                        if (tLen.Length > 0)
                            aSN.Length = (int)(tLen.ToInt());

                    }

                }
                else
                {
                    byte[] L = new byte[1];
                    Len.CopyTo(L, 0);
                    aSN.Length = ((new BitArray(new bool[] { Len[0], Len[1], Len[2], Len[3], Len[4], Len[5], Len[6] })).ToInt());
                }


                if (!aSN.isIndefinite)
                    aSN.ValueBytes = br.ReadBytes(Convert.ToInt32(aSN.Length));
                if (aSN.ClassType != ClassTypes.Application)
                    aSN.ASN1TagType = (ASNAttribute.ASN1TagTypes)aSN.TagId;

                if (Parent != null)
                    Parent.Parameters.Add(aSN);

                if (Parent != null)
                {
                    aSN.ParentsASNs.AddRange(Parent.ParentsASNs);
                    aSN.ParentsASNs.Add(Parent);
                }
                aSN.RaiseASNAddedEventArgs = RaiseASNAddedEventArgs;


                if (aSN.ConstructorType == ConstructorTypes.Constructed && (aSN.Length > 2 || aSN.Parameters.Count > 0))
                {

                    if (Parent == null)
                    {
                        this.ValueBytes = aSN.ValueBytes;
                        this.TagId = aSN.TagId;
                        this.Parameters = aSN.Parameters;
                        this.Length = aSN.Length;
                        this.ConstructorType = aSN.ConstructorType;
                        this.ClassType = aSN.ClassType;
                        this.ASN1TagType = aSN.ASN1TagType;
                        this.isIndefinite = aSN.isIndefinite;
                        if (!aSN.isIndefinite)
                            aSN.ReadSequence(aSN.ValueBytes, aSN);
                    }
                    else if (!aSN.isIndefinite)
                    {
                        aSN.ReadSequence(aSN.ValueBytes, aSN);

                    }
                    OnASNAdded(new ASNAddedEventArgs(aSN));

                }

            }

        }
        void ReadSequence(byte[] Data, ASN1 Parent)
        {
            using (MemoryStream ms = new MemoryStream(Data))
            {
                ReadSequence(ms, Parent);
            }
        }

        public ASN1(byte[] data)
        {
            ReadSequence(data, null);
        }

        public ASN1(string FilePath)
        {
            ReadSequence(File.ReadAllBytes(FilePath), null);
        }

        public void Parse(byte[] data)
        {
            ReadSequence(data, null);
        }

        public ASN1()
        {
        }


        public enum ClassTypes
        {
            Universal = 0, Application = 1, ContextSpecific = 2, Private = 3
        }

        public enum ConstructorTypes
        {
            Primitive = 0, Constructed = 1
        }

        public static void Initialize(object source, ASN1 ASN)
        {

            foreach (FieldInfo info in source.GetType().GetFields())
            {
                try
                {
                    if (info.FieldType.GetGenericArguments().Length > 0)
                    {


                        Type TypeOfList = info.FieldType.GetGenericArguments().Single();

                        //if (TypeOfList.IsClass && !TypeOfList.FullName.ToLower().StartsWith("system."))
                        //{ 
                        //    if (TypeOfList.IsAbstract)
                        //    {
                        //        foreach (Type subclasstype in TypeOfList
                        //        .Assembly.GetTypes().ToList()
                        //        .FindAll(t => t.IsSubclassOf(TypeOfList) && !t.IsAbstract))
                        //        {
                        //            foreach (ASN1 nasn1 in this.GetASNData(info.Name).FindASNs(ASN))
                        //                foreach (ASN1 nasn in this.GetASNData(subclasstype).FindASNs(nasn1))
                        //                info.FieldType.GetMethod("Add").Invoke( Activator.CreateInstance(subclasstype, new object[] { nasn }),null);
                        //        }
                        //    }
                        //    else
                        //    {

                        dynamic list = Convert.ChangeType(Activator.CreateInstance(info.FieldType), info.FieldType);
                        foreach (ASNAttribute attribute in source.GetASNData(info.Name))
                        {
                            if (attribute != null)
                            {
                                if (ASN != null)
                                    foreach (ASN1 nasn in attribute.FindASNs(ASN))
                                    {
                                        dynamic obj = Convert.ChangeType(Activator.CreateInstance(TypeOfList, new object[] { nasn }), TypeOfList);
                                        list.Add(obj);
                                    }
                            }
                        }
                        info.SetValue(source, list);
                    }
                    else
                    if (info.IsPublic && info.FieldType.IsClass && !info.FieldType.FullName.ToLower().StartsWith("system."))
                    {
                        foreach (ASNAttribute attribute in source.GetASNData(info.Name))
                        {
                            if (attribute != null)
                            {
                                if (info.FieldType.IsAbstract)
                                {
                                    foreach (Type subclasstype in info.FieldType
                                    .Assembly.GetTypes().ToList()
                                    .FindAll(t => t.IsSubclassOf(info.FieldType) && !t.IsAbstract))
                                    {
                                        foreach (ASN1 nasn1 in attribute.FindASNs(ASN))
                                            if (nasn1 != null)
                                            {
                                                foreach (ASN1 nasn in source.GetASNData(subclasstype).FindASNs(nasn1))
                                                    if (nasn != null)
                                                        info.FieldType.GetMethod("Add").Invoke(Activator.CreateInstance(subclasstype, new object[] { nasn }), null);
                                            }
                                    }
                                }

                                else
                                {
                                    ASN1 casn = attribute.FindASN(ASN);
                                    if (casn != null)
                                        info.SetValue(source, Activator.CreateInstance(info.FieldType, new object[] { attribute.FindASN(ASN) }));
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public static dynamic Get(object source, ASN1 ASN, MethodBase mBase)
        {
            string PropertyName = mBase.Name.Replace("set_", "").Replace("get_", "");
            dynamic Data = null;
            TypeCode tp;
            if (source.GetType().GetProperty(PropertyName).PropertyType.IsEnum)
            {
                foreach (ASNAttribute attribute in source.GetASNData(PropertyName))
                {
                    try
                    {
                        ASN1 asn = attribute.FindASN(ASN);
                        if (asn is not null)
                        {
                            object obj = attribute.FindASN(ASN).GetValue(TypeCode.Int32);
                            if (obj is not null)
                            {
                                return Enum.ToObject(source.GetType().GetProperty(PropertyName).PropertyType, Convert.ToInt32(obj));
                            }
                        }
                        return Enum.ToObject(source.GetType().GetProperty(PropertyName).PropertyType, -1);
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                if (source.GetType().GetProperty(PropertyName).PropertyType.GenericTypeArguments.Length > 0)
                    tp = Type.GetTypeCode(source.GetType().GetProperty(PropertyName).PropertyType.GenericTypeArguments[0]);
                else
                    tp = Enum.Parse<TypeCode>((source.GetType().GetProperty(PropertyName).PropertyType.Name));


                foreach (ASNAttribute attribute in source.GetASNData(PropertyName))
                {
                    try
                    {
                        ASN1 asn = attribute.FindASN(ASN);
                        if (asn is not null)
                        {
                            object obj = attribute.FindASN(ASN).GetValue(tp);
                            if (obj is not null)
                            {
                                switch (tp)
                                {
                                    case TypeCode.String:
                                        Data = (string)obj;
                                        return Data;
                                    case TypeCode.Int32:
                                        Data = Convert.ToInt32(obj);
                                        return Data;
                                    case TypeCode.UInt32:
                                        Data = Convert.ToUInt32(obj);
                                        return Data;
                                    case TypeCode.Boolean:
                                        return (bool)obj;
                                    default:
                                        return null;
                                }
                            }
                        }
                        switch (tp)
                        {
                            case TypeCode.String:
                                return null;
                            case TypeCode.Int32:
                                ;
                                return -1;
                            case TypeCode.Boolean:
                                return false;
                            default:
                                return null;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            return Data;
        }

        public static void Set(object source, ASN1 ASN, MethodBase mBase, object NewValue)
        {
            string PropertyName = mBase.Name.Replace("set_", "").Replace("get_", "");
            foreach (ASNAttribute attribute in source.GetASNData(PropertyName))
            {
                ASN1 asn = attribute.FindASN(ASN);

                if (asn == null)
                {
                    asn = new ASN1 { ASN1TagType = attribute.TagType, TagId = attribute.TagId[0] };
                    ASN.Parameters.Add(asn);
                }
                asn.SetValue(NewValue, attribute);
            }
        }
    }
}
