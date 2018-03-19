using Skyrates.AI.Custom;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{
    [RequireComponent(typeof(Text))]
    public class UpdateControlScheme : MonoBehaviour
    {

        public UserControlled source;

        private Text render;

        private void Start()
        {
            this.render = this.GetComponent<Text>();
        }

        private void Update()
        {
            //this.render.text = this.source.ControlScheme.ToString();
        }

    }
}