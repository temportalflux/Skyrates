using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{
    public class UpdateVersion : MonoBehaviour {

        public Version version;

        public Text versionName;
        public Text semanticVersion;

        private void Start()
        {
            this.versionName.text = this.version.VersionName;
            this.semanticVersion.text = this.version.GetSemantic();
        }

    }
}