using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Skyrates.Common.Network;
using UnityEngine;

public class BitTestNetwork : MonoBehaviour {

    public class Base
    {

        [BitSerialize(0)]
        public int a;

        [BitSerialize(1)]
        public int b;

        [BitSerialize(2)]
        public int c;

    }

    public class Derived : Base
    {


        [BitSerialize(0)]
        public int d;

        [BitSerialize(1)]
        public int e;

        [BitSerialize(2)]
        public int f;


    }

    private struct BitAttribute
    {
        public BitSerializeAttribute attribute;
        public FieldInfo field;
    }

    void Start()
    {
        List<BitAttribute> allAttributes = new List<BitAttribute>();

        // Stack of parent->child types
        Stack<Type> parentStack = new Stack<Type>();
        Type t = typeof(Derived);
        while (t != typeof(object) && t != null)
        {
            parentStack.Push(t);
            t = t.BaseType;
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

        

        

    }

}
