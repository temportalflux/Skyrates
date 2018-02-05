using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Version")]
public class Version : ScriptableObject
{

    public string versionName;
    public int major;
    public int minor;
    public int patch;

    public string getSemantic()
    {
        return string.Format("{0}.{1}.{2}", this.major, this.minor, this.patch);
    }


}
