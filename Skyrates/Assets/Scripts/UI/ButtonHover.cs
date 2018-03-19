using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{
    [RequireComponent(typeof(Image))]
    public class ButtonHover : MonoBehaviour
    {

        public Sprite spriteHover;
        private Sprite spriteDefault;

        private Image button;

        void Start()
        {
            this.button = this.GetComponent<Image>();
            this.spriteDefault = this.button.sprite;
        }

        public void OnHoverStart()
        {
            this.button.sprite = this.spriteHover;
        }

        public void OnHoverEnd()
        {
            this.button.sprite = this.spriteDefault;
        }

    }
}