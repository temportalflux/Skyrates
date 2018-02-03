using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitTest : MonoBehaviour {

    public class Sereal : ISerializing
    {
        [BitSerialize]
        public uint clientID;

        public Color color;

        public int GetSize()
        {
            return BitSerializeAttribute.MODULES[typeof(Color)].GetSize(this.color);
        }

        public void Serialize(ref byte[] data, ref int lastIndex)
        {
            data = BitSerializeAttribute.MODULES[typeof(Color)].Serialize(this.color, data, lastIndex);
            lastIndex += BitSerializeAttribute.MODULES[typeof(Color)].GetSize(this.color);
        }

        public void Deserialize(byte[] data, ref int lastIndex)
        {
            this.color = (Color)BitSerializeAttribute.MODULES[typeof(Color)].Deserialize(this.color, data, lastIndex, typeof(Color));
            lastIndex += BitSerializeAttribute.MODULES[typeof(Color)].GetSize(this.color);
        }

    }

    [BitSerialize(0)]
    public int intVar;

    [BitSerialize(1)]
    public float floatVar;

    [BitSerialize(2)]
    public bool[] arrayTest;

    [BitSerialize(3)]
    public string testString;

    [BitSerialize(4)]
    public Sereal player;

    protected virtual void Start()
    {
        this.init();

        byte[] data = BitSerializeAttribute.Serialize(this);

        this.clear();

        BitSerializeAttribute.Deserialize(this, data);

        this.report();

    }

    protected virtual void init()
    {

        this.intVar = 22;
        this.floatVar = 30.5f;
        this.arrayTest = new bool[] { false, true, true, false, true };
        this.testString = "hello world";
        this.player = new Sereal();
        this.player.clientID = 25;
        this.player.color = Color.red;

    }

    protected virtual void clear()
    {

        this.intVar = 0;
        this.floatVar = 0;
        this.arrayTest = null;
        this.testString = null;
        this.player = new Sereal();

    }

    protected virtual void report()
    {

        Debug.Log(this.intVar);
        Debug.Log(this.floatVar);
        Debug.Log(this.testString);
        Debug.Log(this.player.clientID);
        Debug.Log(this.player.color);

    }
	
}
