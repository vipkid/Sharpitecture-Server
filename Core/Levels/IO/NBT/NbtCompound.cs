using Sharpitecture.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtCompound : NbtField
    {
        public override byte TypeID { get { return 10; } }

        /// <summary>
        /// The list of fields within the NbtCompound
        /// </summary>
        public List<NbtField> Fields { get; private set; } = new List<NbtField>();

        public override int Size
        {
            get
            {
                return Fields.Sum(f => f.Size) + 4 + Name.Length;
            }
        }

        public override byte[] Serialize()
        {
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);

            foreach (NbtField field in Fields)
                buffer.Write(field.Serialize(), 0, field.Size);

            buffer.WriteByte(0);
            return buffer.Data;
        }

        /// <summary>
        /// Adds a field to the NbtCompound
        /// </summary>
        public void AddField(NbtField field)
        {
            if (Fields.Any(f => f.Name.CaselessEquals(field.Name)))
                throw new Exception("Field with name '" + field.Name + "' already exists!");
            Fields.Add(field);
        }

        /// <summary>
        /// Removes a field from the NbtCompound
        /// </summary>
        public void RemoveField(string fieldName)
        {
            NbtField field = GetField(fieldName);
            if (field == null)
                return;
            Fields.Remove(field);
        }

        /// <summary>
        /// Gets a field with the specified name
        /// </summary>
        public NbtField GetField(string fieldName)
        {
            return Fields.FirstOrDefault(field => field.Name.CaselessEquals(fieldName));
        }
    }
}
