using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitTest2 : BitTest
{

    //[BitSerialize(5)]
    public string bitTest2Str;

    [BitSerialize(6)]
    public List<int> list1;

    [BitSerialize(7)]
    public List<Sereal> list2;

    [BitSerialize(8)]
    public Dictionary<int, string> d1;

    protected override void init()
    {
        base.init();
        this.bitTest2Str = "Inehireted string";
        this.list1 = new List<int>();
        this.list1.Add(42);
        this.list2 = new List<Sereal>();
        this.list2.Add(new Sereal { clientID = 53 });
        this.d1 = new Dictionary<int, string>() { { 23, "dictvalue" } };
    }

    protected override void clear()
    {
        base.clear();

        this.bitTest2Str = null;
        this.list1 = null;
        this.list2 = null;
        this.d1 = null;
    }

    protected override void report()
    {
        base.report();
        Debug.Log(this.bitTest2Str);
        Debug.Log(this.list1.ToArray());
        Debug.Log(this.list2.ToArray());
        Debug.Log(this.d1[23]);
    }

}
