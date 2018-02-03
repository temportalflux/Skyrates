using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateVersion : MonoBehaviour {

    public Version version;

    public Text versionName;
    public Text semanticVersion;

    private void Start()
    {
        this.versionName.text = this.version.versionName;
        this.semanticVersion.text = this.version.getSemantic();
    }

}
