using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public abstract class NbtField
    {
        /// <summary>
        /// The type ID of the NbtField
        /// </summary>
        public abstract byte TypeID { get; }

        /// <summary>
        /// The size of the the NbtField
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        /// The name of the NbtField
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the NbtField
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Serializes the NbtField for networking/IO
        /// </summary>
        /// <returns></returns>
        public abstract byte[] Serialize();

        /// <summary>
        /// Returns the byte value of the NbtField
        /// <para>Throws FormatException if not possible</para>
        /// </summary>
        public virtual byte ByteValue
        {
            get
            {
                byte val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }
        
        /// <summary>
        /// Returns the short value of the NbtField
        /// <para>Throws FormatException if not possible</para>
        /// </summary>
        public virtual short ShortValue
        {
            get
            {
                short val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }
        
        /// <summary>
        /// Returns the int value of the NbtField
        /// <para>Throws FormatException if not possible</para>
        /// </summary>
        public virtual int IntValue
        {
            get
            {
                int val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        /// <summary>
        /// Returns the long value of the NbtField
        /// <para>Throws FormatException if not possible</para>
        /// </summary>
        public virtual long LongValue
        {
            get
            {
                long val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        /// <summary>
        /// Returns the single value of the NbtField
        /// <para>Throws FormatException if not possible</para>
        /// </summary>
        public virtual float FloatValue
        {
            get
            {
                float val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        /// <summary>
        /// Returns the double value of the NbtField
        /// <para>Throws FormatException if not possible</para>
        /// </summary>
        public virtual double DoubleValue
        {
            get
            {
                double val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        /// <summary>
        /// Returns the byte array value of the NbtField
        /// <para>Throws FormatException if not possible</para>
        /// </summary>
        public virtual byte[] ByteArrayValue
        {
            get
            {
                throw new FormatException();
            }
        }

        /// <summary>
        /// Returns the string value of the NbtField
        /// </summary>
        public virtual string StringValue
        {
            get
            {
                return Value.ToString();
            }
        }
    }
}
