using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace Skyrates.Common.Network
{

    /// <summary>
    /// Handles (de)serializing of specific fields.
    /// Valid types include: bool, byte, char, (u)short, (u)int, (u)long, float, double, ISerializing, and any composite arrays of the former.
    /// If the <see cref="MonoBehaviour"/> being serialized is ISerializing, the ISerializing methods are treated as additions to all fields marked with BitSerialize
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BitSerializeAttribute : Attribute
    {

        /// <summary>
        /// The numerical order in which attributes are sorted - lowest is first.
        /// </summary>
        public uint _order;

        public BitSerializeAttribute(uint order = 0)
        {
            this._order = order;
        }

        /// <summary>
        /// The interface to use <see cref="SerializationModule{T}"/> in collections without knowing the type.
        /// </summary>
        public interface ISerializationModule
        {
            /// <summary>
            /// Determine the size of some object - effectively sizeof(object).
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>int</returns>
            int GetSize(object obj);

            /// <summary>
            /// Serialize a specfic object into a byte array. Returns the byte[] with the populated _gameStateData.
            /// </summary>
            /// <param name="obj">The object to serialize.</param>
            /// <param name="data">The byte array of _gameStateData.</param>
            /// <param name="start">How far into the _gameStateData to put the object's serialized _gameStateData.</param>
            /// <returns>byte[]</returns>
            byte[] Serialize(object obj, byte[] data, int start);

            /// <summary>
            /// Deserialize a byte array into an object.
            /// </summary>
            /// <param name="data">the byte[] of _gameStateData.</param>
            /// <param name="start">How far into the serialized _gameStateData the object was put.</param>
            /// <returns>object</returns>
            object Deserialize(object obj, byte[] data, int start, Type type);
        }

        /// <summary>
        /// The implementation of <see cref="ISerializationModule"/>, where all the generic "object" fields are objects with the type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The generic type which this struct wraps the (De)Serialization of.</typeparam>
        public class SerializationModule<T> : ISerializationModule
        {
            /// <summary>
            /// A generic, and module settable, version of <see cref="ISerializationModule.GetSize(object)"/>.
            /// </summary>
            public Func<T, int> _GetSize;

            /// <summary>
            /// A generic, and module settable, version of <see cref="ISerializationModule.Serialize(object, byte[], int)"/>.
            /// </summary>
            public Func<T, byte[], int, byte[]> _Serialize;

            /// <summary>
            /// A generic, and module settable, version of <see cref="ISerializationModule.Deserialize(byte[], int)"/>.
            /// </summary>
            public Func<T, byte[], int, Type, T> _Deserialize;

            /// <summary>
            /// Determine the size of some object - effectively sizeof(object).
            /// </summary>
            /// <param name="obj">The object with the type <see cref="T"/>.</param>
            /// <returns>int</returns>
            public int GetSize(object obj)
            {
                return _GetSize((T) obj);
            }

            /// <summary>
            /// Serialize a specfic object into a byte array. Returns the byte[] with the populated _gameStateData.
            /// </summary>
            /// <param name="obj">The object to serialize with the type <see cref="T"/>.</param>
            /// <param name="data">The byte array of _gameStateData.</param>
            /// <param name="start">How far into the _gameStateData to put the object's serialized _gameStateData.</param>
            /// <returns>byte[]</returns>
            public byte[] Serialize(object obj, byte[] data, int start)
            {
                return _Serialize((T) obj, data, start);
            }

            /// <summary>
            /// Deserialize a byte array into an object with the type <see cref="T"/>.
            /// </summary>
            /// <param name="data">the byte[] of _gameStateData.</param>
            /// <param name="start">How far into the serialized _gameStateData the object was put.</param>
            /// <returns>object with the type <see cref="T"/></returns>
            public object Deserialize(object obj, byte[] data, int start, Type type)
            {
                return _Deserialize((T) obj, data, start, type);
            }
        }

        /// <summary>
        /// A dictionary of (De)Serialization interfaces for handling how any one type should be handled.
        /// </summary>
        public static Dictionary<Type, ISerializationModule> MODULES = new Dictionary<Type, ISerializationModule>()
        {
            // Byte
            {
                typeof(Byte), new SerializationModule<Byte>
                {
                    _GetSize = new Func<Byte, int>((Byte valueA) => { return 1; }),
                    _Serialize = new Func<Byte, byte[], int, byte[]>((Byte valueB, byte[] data, int start) =>
                    {
                        data[start] = valueB;
                        return data;
                    }),
                    _Deserialize = new Func<Byte, byte[], int, Type, Byte>(
                        (Byte obj, byte[] data, int start, Type type) =>
                        {
                            return data[start];
                        }),
                }
            },
            // Boolean
            {
                typeof(Boolean), new SerializationModule<Boolean>
                {
                    _GetSize = new Func<Boolean, int>((Boolean valueA) => { return 1; }),
                    _Serialize = new Func<Boolean, byte[], int, byte[]>((Boolean valueB, byte[] data, int start) =>
                    {
                        data[start] = (byte) (valueB ? 1 : 0);
                        return data;
                    }),
                    _Deserialize = new Func<Boolean, byte[], int, Type, Boolean>(
                        (Boolean obj, byte[] data, int start, Type type) =>
                        {
                            return data[start] > 0;
                        }),
                }
            },
            // Char
            {
                typeof(Char), new SerializationModule<Char>
                {
                    _GetSize = new Func<Char, int>((Char valueA) => { return sizeof(Char); }),
                    _Serialize = new Func<Char, byte[], int, byte[]>((Char valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<Char, byte[], int, Type, Char>(
                        (Char obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToChar(data, start);
                        }),
                }
            },
            // Int16
            {
                typeof(Int16), new SerializationModule<Int16>
                {
                    _GetSize = new Func<Int16, int>((Int16 valueA) => { return sizeof(Int16); }),
                    _Serialize = new Func<Int16, byte[], int, byte[]>((Int16 valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<Int16, byte[], int, Type, Int16>(
                        (Int16 obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToInt16(data, start);
                        }),
                }
            },
            // Int32
            {
                typeof(Int32), new SerializationModule<Int32>
                {
                    _GetSize = new Func<Int32, int>((Int32 valueA) => { return sizeof(Int32); }),
                    _Serialize = new Func<Int32, byte[], int, byte[]>((Int32 valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<Int32, byte[], int, Type, Int32>(
                        (Int32 obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToInt32(data, start);
                        }),
                }
            },
            // Int64
            {
                typeof(Int64), new SerializationModule<Int64>
                {
                    _GetSize = new Func<Int64, int>((Int64 valueA) => { return sizeof(Int64); }),
                    _Serialize = new Func<Int64, byte[], int, byte[]>((Int64 valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<Int64, byte[], int, Type, Int64>(
                        (Int64 obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToInt64(data, start);
                        }),
                }
            },
            // UInt16
            {
                typeof(UInt16), new SerializationModule<UInt16>
                {
                    _GetSize = new Func<UInt16, int>((UInt16 valueA) => { return sizeof(UInt16); }),
                    _Serialize = new Func<UInt16, byte[], int, byte[]>((UInt16 valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<UInt16, byte[], int, Type, UInt16>(
                        (UInt16 obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToUInt16(data, start);
                        }),
                }
            },
            // UInt32
            {
                typeof(UInt32), new SerializationModule<UInt32>
                {
                    _GetSize = new Func<UInt32, int>((UInt32 valueA) => { return sizeof(UInt32); }),
                    _Serialize = new Func<UInt32, byte[], int, byte[]>((UInt32 valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<UInt32, byte[], int, Type, UInt32>(
                        (UInt32 obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToUInt32(data, start);
                        }),
                }
            },
            // UInt64
            {
                typeof(UInt64), new SerializationModule<UInt64>
                {
                    _GetSize = new Func<UInt64, int>((UInt64 valueA) => { return sizeof(UInt64); }),
                    _Serialize = new Func<UInt64, byte[], int, byte[]>((UInt64 valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<UInt64, byte[], int, Type, UInt64>(
                        (UInt64 obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToUInt64(data, start);
                        }),
                }
            },
            // Single
            {
                typeof(Single), new SerializationModule<Single>
                {
                    _GetSize = new Func<Single, int>((Single valueA) => { return sizeof(Single); }),
                    _Serialize = new Func<Single, byte[], int, byte[]>((Single valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<Single, byte[], int, Type, Single>(
                        (Single obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToSingle(data, start);
                        }),
                }
            },
            // Double
            {
                typeof(Double), new SerializationModule<Double>
                {
                    _GetSize = new Func<Double, int>((Double valueA) => { return sizeof(Double); }),
                    _Serialize = new Func<Double, byte[], int, byte[]>((Double valueB, byte[] data, int start) =>
                    {
                        CopyTo(ref data, start, BitConverter.GetBytes(valueB));
                        return data;
                    }),
                    _Deserialize = new Func<Double, byte[], int, Type, Double>(
                        (Double obj, byte[] data, int start, Type type) =>
                        {
                            return BitConverter.ToDouble(data, start);
                        }),
                }
            },
            // String
            {
                typeof(string), new SerializationModule<string>
                {
                    _GetSize = new Func<string, int>((string valueA) =>
                    {
                        return GetSizeOf(valueA.ToCharArray(), null);
                    }),
                    _Serialize = new Func<string, byte[], int, byte[]>((string valueB, byte[] data, int start) =>
                    {
                        // we KNOW chars don't have BitSerialize in them
                        data = Serialize(valueB.ToCharArray(), data, start, new AttributeField());
                        return data;
                    }),
                    _Deserialize = new Func<string, byte[], int, Type, string>(
                        (string obj, byte[] data, int start, Type type) =>
                        {
                            return new string((char[]) Deserialize(null, data, start, new AttributeField(),
                                typeof(char[])));
                        }),
                }
            },
            // Vector3
            {
                typeof(Vector3), new SerializationModule<Vector3>
                {
                    _GetSize = new Func<Vector3, int>((Vector3 valueA) => { return 3 * sizeof(Single); }),
                    _Serialize = new Func<Vector3, byte[], int, byte[]>((Vector3 valueB, byte[] data, int start) =>
                    {
                        ISerializationModule module = MODULES[typeof(Single)];
                        data = module.Serialize(valueB.x, data, start);
                        start += sizeof(Single);
                        data = module.Serialize(valueB.y, data, start);
                        start += sizeof(Single);
                        data = module.Serialize(valueB.z, data, start);
                        return data;
                    }),
                    _Deserialize = new Func<Vector3, byte[], int, Type, Vector3>(
                        (Vector3 obj, byte[] data, int start, Type type) =>
                        {
                            Type single = typeof(Single);
                            ISerializationModule module = MODULES[single];
                            float x = (float) MODULES[typeof(Single)].Deserialize(0.0f, data, start, single);
                            start += sizeof(Single);
                            float y = (float) MODULES[typeof(Single)].Deserialize(0.0f, data, start, single);
                            start += sizeof(Single);
                            float z = (float) MODULES[typeof(Single)].Deserialize(0.0f, data, start, single);
                            return new Vector3(x, y, z);
                        }),
                }
            },
            // Quaternion
            {
                typeof(Quaternion), new SerializationModule<Quaternion>
                {
                    _GetSize = new Func<Quaternion, int>((Quaternion valueA) => { return 3 * sizeof(Single); }),
                    _Serialize = new Func<Quaternion, byte[], int, byte[]>((Quaternion valueB, byte[] data, int start) =>
                        MODULES[typeof(Vector3)].Serialize(valueB.eulerAngles, data, start)),
                    _Deserialize = new Func<Quaternion, byte[], int, Type, Quaternion>(
                        (Quaternion obj, byte[] data, int start, Type type) =>
                        Quaternion.Euler((Vector3)MODULES[typeof(Vector3)]
                            .Deserialize(Vector3.zero, data, start, typeof(Vector3)))),
                }
            },
            // Color
            {
                typeof(Color), new SerializationModule<Color>
                {
                    _GetSize = new Func<Color, int>((Color valueA) => { return 4 * sizeof(Single); }),
                    _Serialize = new Func<Color, byte[], int, byte[]>((Color valueB, byte[] data, int start) =>
                    {
                        ISerializationModule module = MODULES[typeof(Single)];
                        data = module.Serialize(valueB.r, data, start);
                        start += sizeof(Single);
                        data = module.Serialize(valueB.g, data, start);
                        start += sizeof(Single);
                        data = module.Serialize(valueB.b, data, start);
                        start += sizeof(Single);
                        data = module.Serialize(valueB.a, data, start);
                        return data;
                    }),
                    _Deserialize = new Func<Color, byte[], int, Type, Color>(
                        (Color obj, byte[] data, int start, Type type) =>
                        {
                            Type single = typeof(Single);
                            ISerializationModule module = MODULES[single];
                            float r = (float) MODULES[typeof(Single)].Deserialize(0.0f, data, start, single);
                            start += sizeof(Single);
                            float g = (float) MODULES[typeof(Single)].Deserialize(0.0f, data, start, single);
                            start += sizeof(Single);
                            float b = (float) MODULES[typeof(Single)].Deserialize(0.0f, data, start, single);
                            start += sizeof(Single);
                            float a = (float) MODULES[typeof(Single)].Deserialize(0.0f, data, start, single);
                            return new Color(r, g, b, a);
                        }),
                }
            },
            // ISerializing
            {
                typeof(ISerializing), new SerializationModule<ISerializing>
                {
                    _GetSize = new Func<ISerializing, int>((ISerializing valueA) => { return valueA.GetSize(); }),
                    _Serialize = new Func<ISerializing, byte[], int, byte[]>(
                        (ISerializing valueB, byte[] data, int start) =>
                        {
                            valueB.Serialize(ref data, ref start);
                            return data;
                        }),
                    _Deserialize = new Func<ISerializing, byte[], int, Type, ISerializing>(
                        (ISerializing obj, byte[] data, int start, Type type) =>
                        {
                            obj.Deserialize(data, ref start);
                            return obj;
                        }),
                }
            },
            // Guid
            {
                typeof(Guid), new SerializationModule<Guid>
                {
                    _GetSize =
                        new Func<Guid, int>((Guid valueA) =>
                            sizeof(Int32) +
                            16), // https://msdn.microsoft.com/en-us/library/system.guid.tobytearray(v=vs.110).aspx
                    _Serialize = new Func<Guid, byte[], int, byte[]>((Guid valueB, byte[] data, int start) =>
                    {
                        byte[] guidData = valueB.ToByteArray();

                        data = MODULES[typeof(Int32)].Serialize(guidData.Length, data, start);
                        start += sizeof(Int32);

                        CopyTo(ref data, start, valueB.ToByteArray());

                        return data;
                    }),
                    _Deserialize = new Func<Guid, byte[], int, Type, Guid>(
                        (Guid obj, byte[] data, int start, Type type) =>
                        {
                            int size = (int)BitSerializeAttribute.Deserialize(0, data, ref start);

                            try
                            {
                                byte[] guidData = new byte[size];
                                CopyFrom(data, ref guidData, start, size);
                                return new Guid(guidData);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }

                            return Guid.Empty;
                        }),
                }
            },
        };

        /// <summary>
        /// Write some byte array into another byte array at some offset.
        /// </summary>
        /// <param name="dest">The _gameStateData to copy.</param>
        /// <param name="start">The offset in bytes.</param>
        /// <param name="source">The _gameStateData object to copy into at the offset.</param>
        public static void CopyTo(ref byte[] dest, int start, byte[] source)
        {
            Array.Copy(source, 0, dest, start, source.Length);
        }

        public static void CopyFrom(byte[] source, ref byte[] dest, int start, int length)
        {
            Debug.Assert(source.Length >= start + length);
            Debug.Assert(dest.Length >= 0 + length);
            Array.Copy(source, start, dest, 0, length);
        }

        public class AttributeField
        {

            public FieldInfo info = null;
            public int size = 0;
            public List<AttributeField> fields = new List<AttributeField>();
            public List<AttributeField> fieldsbyGeneric = new List<AttributeField>();

        }

        private struct BitAttribute
        {
            public BitSerializeAttribute attribute;
            public FieldInfo field;
        }

        private static List<BitAttribute> GetBitAttributeFields<T>(T value, Type fieldType = null)
        {
            List<BitAttribute> allAttributes = new List<BitAttribute>();

            // Get the type of the object
            Type type = null;
            if (value != null)
            {
                type = value.GetType();
            }
            else
            {
                Debug.Assert(fieldType != null);
                type = fieldType;
            }
            Type subType = type.GetElementType();
            if (subType != null) type = subType;
            
            // Stack of parent->child types
            Stack<Type> parentStack = new Stack<Type>();
            while (type != typeof(object) && type != null)
            {
                parentStack.Push(type);
                type = type.BaseType;
            }

            while (parentStack.Count > 0)
            {
                Type supertype = parentStack.Pop();

                List<BitAttribute> attributes = new List<BitAttribute>();

                FieldInfo[] objectFields = supertype.GetFields(
                    BindingFlags.Instance
                    | BindingFlags.Public
                    // | BindingFlags.NonPublic
                    | BindingFlags.DeclaredOnly
                );

                foreach (FieldInfo field in objectFields)
                {
                    BitSerializeAttribute attribute =
                        Attribute.GetCustomAttribute(field, typeof(BitSerializeAttribute)) as
                            BitSerializeAttribute;

                    // if we detect any attribute
                    if (attribute != null)
                    {
                        attributes.Add(new BitAttribute { attribute = attribute, field = field });
                    }
                }

                attributes.Sort(new Comparison<BitAttribute>(
                    (a, b) => (int) a.attribute._order - (int) b.attribute._order
                ));

                allAttributes.AddRange(attributes);
            }

            return allAttributes;
        }

        /// <summary>
        /// Gets all BitSerialize attribute fields in a monobehavior, calculating the size of each along the way
        /// </summary>
        /// <param name="value">The MonoBehaviour with BitSerialize fields</param>
        /// <param name="totalBitSize">The total bytes of the BitSerialize fields</param>
        /// <returns></returns>
        private static List<AttributeField> GetAttributeFields<T>(T value, Type fieldType = null)
        {
            List<AttributeField> attributeFields = new List<AttributeField>();

            foreach (BitAttribute attributeEntry in GetBitAttributeFields(value, fieldType))
            {
                T def;
                try
                {
                    def = value != null ? value : (T) Activator.CreateInstance(fieldType);
                }
                catch (Exception)
                {
                    break;
                }
                var fieldObj = attributeEntry.field.GetValue(def);

                AttributeField field =
                    new AttributeField
                    {
                        info = attributeEntry.field,
                        fieldsbyGeneric = new List<AttributeField>(),
                        fields = GetAttributeFields(fieldObj, attributeEntry.field.FieldType)
                    };
                // empty for collections

                // This is for collections
                Type[] genericArgs = field.info.FieldType.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                    for (int iGeneric = 0; iGeneric < genericArgs.Length; iGeneric++)
                    {
                        AttributeField fieldTypeGeneric = new AttributeField
                        {
                            size = 0,
                            fields = GetAttributeFields(fieldObj, genericArgs[iGeneric])
                        };
                        field.fieldsbyGeneric.Add(fieldTypeGeneric);
                    }
                }

                // Get the sizeof the object
                field.size = GetSizeOf(fieldObj, field.fields, field.fieldsbyGeneric);
                // A valid size was found
                if (field.size >= 0)
                {
                    attributeFields.Add(field);
                }
                // the size returned was < 0, so it was an invalid attribute
                else
                {
                    Debug.Log("Could not serialize field " + typeof(ValueType).Name + "#" + attributeEntry.field.Name +
                              " even though it is marked as bitserialize");
                }
            }

            return attributeFields;
        }

        /// <summary>
        /// Attempts to determine the size of some object (effectively sizeof(object))
        /// </summary>
        /// <param name="value">The object to size up</param>
        /// <returns>The size of some object, using sizeof for primitive objects, recursive behavior for arrays, and GetSize for ISerializing</returns>
        public static int GetSizeOf<T>(T obj, List<AttributeField> fields = null, List<AttributeField> generics = null)
        {
            if (obj == null) return 0;
            if (fields == null) fields = new List<AttributeField>();
            if (generics == null) generics = new List<AttributeField>();

            Type type = obj.GetType();
            if (MODULES.ContainsKey(type))
            {
                return MODULES[type].GetSize(obj);
            }
            else if (type.IsArray)
            {
                // We can cast to ILIst because arrays implement it and we verfied that it is an array in the if statement
                System.Collections.IList fieldArray = (System.Collections.IList) obj;
                // The value has no entries, so it has no _gameStateData
                if (fieldArray.Count <= 0)
                {
                    return sizeof(int);
                }
                // the value has entries, so it has a size
                else
                {
                    int size = sizeof(int);
                    for (int iArr = 0; iArr < fieldArray.Count; iArr++)
                    {
                        object fieldObj = fieldArray[iArr];
                        size += GetSizeOf(fieldObj, fields);
                    }
                    return size;
                }
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                ICollection collection = obj as ICollection;
                int count = collection.Count;
                if (count > 0)
                {
                    if (typeof(IList).IsAssignableFrom(type))
                    {
                        IList list = obj as IList;
                        int size = sizeof(int);
                        for (int iList = 0; iList < count; iList++)
                        {
                            size += GetSizeOf(list[iList], fields);
                        }
                        return size;
                    }
                    else if (typeof(IDictionary).IsAssignableFrom(type))
                    {
                        IDictionary dictionary = obj as IDictionary;
                        int size = sizeof(int);
                        foreach (object key in dictionary.Keys)
                        {
                            object value = dictionary[key];
                            int sizeKey = GetSizeOf(key, generics[0].fields);
                            int sizeValue = GetSizeOf(value, generics[1].fields);
                            size += (sizeKey + sizeValue);
                        }
                        return size;
                    }
                }
                return sizeof(int);
            }

            // Is not a collection, nor in MODULES

            // Starting size, none
            int totalSize = 0;

            // Get the total size of all auto-serializing fields
            foreach (AttributeField field in fields)
            {
                totalSize += field.size;
            }

            // Check all parent classes in MODULES (ISerializing)
            foreach (Type key in MODULES.Keys)
            {
                if (key.IsAssignableFrom(type))
                {
                    totalSize += MODULES[key].GetSize(obj);
                }
            }

            return totalSize > 0 ? totalSize : -1;
        }

        public static byte[] Serialize<T>(T obj, byte[] data = null, int start = 0, AttributeField attribute = null)
        {
            // For GetSizeOf and Serializing, the fields are required
            if (attribute == null)
            {
                attribute = new AttributeField
                {
                    fields = GetAttributeFields(obj)
                };
            }
            Type type = obj.GetType();

            // No incoming _gameStateData object, so this is the beginning of a serialization, so we need the size
            if (data == null)
            {
                // Determine size of obj
                int totalBitSize = GetSizeOf(obj, attribute.fields);
                // Can be serialized?
                if (totalBitSize >= 0)
                {
                    data = new byte[totalBitSize];
                }
                else
                {
                    // Cannot serialize
                    return null;
                }
            }

            // Serialize obj into _gameStateData, starting at start

            if (MODULES.ContainsKey(type))
            {
                return MODULES[type].Serialize(obj, data, start);
            }
            else if (type.IsArray)
            {
                // We can cast to ILIst because arrays implement it and we verfied that it is an array in the if statement
                System.Collections.IList fieldArray = (System.Collections.IList) obj;

                // Write the size of the array
                AttributeField intFields = new AttributeField();
                data = Serialize(fieldArray.Count, data, start, intFields);
                start += GetSizeOf(fieldArray.Count, intFields.fields);

                // Write all values
                for (int i = 0; i < fieldArray.Count; i++)
                {
                    data = Serialize(fieldArray[i], data, start, attribute);
                    start += GetSizeOf(fieldArray[i], attribute.fields);
                }

                return data;
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                ICollection collection = obj as ICollection;
                int count = collection.Count;

                // Write the size of the array
                AttributeField intFields = new AttributeField();
                data = Serialize(count, data, start, intFields);
                start += GetSizeOf(count, intFields.fields);

                if (count > 0)
                {
                    if (typeof(IList).IsAssignableFrom(type))
                    {
                        IList list = obj as IList;
                        for (int iCount = 0; iCount < count; iCount++)
                        {
                            data = Serialize(list[iCount], data, start, attribute);
                            start += GetSizeOf(list[iCount], attribute.fields);
                        }
                    }
                    else if (typeof(IDictionary).IsAssignableFrom(type))
                    {
                        IDictionary dictionary = obj as IDictionary;
                        foreach (object key in dictionary.Keys)
                        {
                            data = Serialize(key, data, start, attribute.fieldsbyGeneric[0]);
                            start += GetSizeOf(key, attribute.fieldsbyGeneric[0].fields);
                            data = Serialize(dictionary[key], data, start, attribute.fieldsbyGeneric[1]);
                            start += GetSizeOf(dictionary[key], attribute.fieldsbyGeneric[1].fields);
                        }
                    }
                }

                return data;
            }
            // Is not a collection, nor in MODULES
            else
            {
                // Fill the data with the serialized fields
                foreach (AttributeField attributeField in attribute.fields)
                {
                    object value = attributeField.info.GetValue(obj);
                    data = Serialize(value, data, start, attributeField);
                    // increment the total size of what is being serialized
                    start += attributeField.size;
                }

                // Check all parent classes in MODULES (ISerializing)
                foreach (Type key in MODULES.Keys)
                {
                    if (key.IsAssignableFrom(type))
                    {
                        data = MODULES[key].Serialize(obj, data, start);
                        start += MODULES[key].GetSize(obj);
                    }
                }

                return data;
            }
        }

        public static object Deserialize(object obj, byte[] data, int start = 0, AttributeField attribute = null,
            Type typeIn = null)
        {
            return Deserialize(obj, data, ref start, attribute, typeIn);
        }

        public static object Deserialize(object obj, byte[] data, ref int start, AttributeField attribute = null,
            Type typeIn = null)
        {
            if (attribute == null)
            {
                attribute = new AttributeField
                {
                    fields = GetAttributeFields(obj)
                };
            }

            Type type = typeIn == null && obj != null ? obj.GetType() : typeIn;

            if (MODULES.ContainsKey(type))
            {
                obj = MODULES[type].Deserialize(obj, data, start, type);
                start += MODULES[type].GetSize(obj);
                return obj;
            }
            else if (type.IsArray)
            {
                // Get the size of the array
                int size = 0;
                AttributeField intFields = new AttributeField();
                size = (int) Deserialize(size, data, start, intFields);
                start += GetSizeOf(size, intFields.fields);

                Type subtype = type.GetElementType();
                Array arr = Array.CreateInstance(subtype, size);

                // Iterate over all elements that will be deserializes
                for (int i = 0; i < size; i++)
                {
                    object element = null;

                    try
                    {
                        element = Activator.CreateInstance(subtype);
                        element = Deserialize(element, data, start, attribute, subtype);
                        arr.SetValue(element, i);
                        start += GetSizeOf(element, attribute.fields);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                return (object) arr;
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                int size = 0;
                AttributeField intFields = new AttributeField();
                size = (int) Deserialize(size, data, start, intFields);
                start += GetSizeOf(size, intFields.fields);

                if (size >= 0)
                {
                    if (typeof(IList).IsAssignableFrom(type))
                    {
                        IList list = (IList) Activator.CreateInstance(type);
                        Type subtype = type.GetGenericArguments()[0];
                        for (int iList = 0; iList < size; iList++)
                        {
                            object element = null;

                            try
                            {
                                element = Activator.CreateInstance(subtype);
                            }
                            catch
                            {
                                // ignored
                            }

                            list.Add(Deserialize(element, data, start, attribute, subtype));
                            start += GetSizeOf(list[iList], attribute.fields);
                        }
                        return list;
                    }
                    else if (typeof(IDictionary).IsAssignableFrom(type))
                    {
                        IDictionary dict = (IDictionary) Activator.CreateInstance(type);
                        Type typeKey = type.GetGenericArguments()[0];
                        Type typeValue = type.GetGenericArguments()[1];
                        for (int iDict = 0; iDict < size; iDict++)
                        {
                            object key = null;
                            try
                            {
                                key = Activator.CreateInstance(typeKey);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }

                            key = Deserialize(key, data, start, attribute.fieldsbyGeneric[0], typeKey);
                            start += GetSizeOf(key, attribute.fieldsbyGeneric[0].fields);
                            object value = null;
                            try
                            {
                                value = Activator.CreateInstance(typeValue);
                            }
                            catch (Exception e)
                            {
                            }
                            value = Deserialize(value, data, start, attribute.fieldsbyGeneric[1], typeValue);
                            start += GetSizeOf(value, attribute.fieldsbyGeneric[1].fields);
                            dict[key] = value;
                        }
                        return dict;
                    }
                }

                return null;
            }
            // Is not a collection, nor in MODULES
            else
            {
                // Fill the _gameStateData with the serialized fields
                foreach (AttributeField attributeField in attribute.fields)
                {
                    object element = null;

                    try
                    {
                        element = Activator.CreateInstance(attributeField.info.FieldType);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    element = Deserialize(element, data, start, attributeField, attributeField.info.FieldType);
                    attributeField.info.SetValue(obj, element);
                    // increment the total size of what is being serialized
                    start += GetSizeOf(element, attributeField.fields, attributeField.fieldsbyGeneric);
                }

                // Check all parent classes in MODULES (ISerializing)
                foreach (Type key in MODULES.Keys)
                {
                    if (key.IsAssignableFrom(type))
                    {
                        obj = MODULES[key].Deserialize(obj, data, start, type);
                        start += MODULES[key].GetSize(obj);
                    }
                }

                return obj;
            }

        }

    }

}