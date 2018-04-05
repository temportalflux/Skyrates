using Skyrates.Data;
using UnityEngine;

namespace Skyrates.UI
{

    public class HUDState : MonoBehaviour
    {

        public GameObject Content;

        protected virtual void Awake()
        {

        }

        public virtual void UpdateWith(PlayerData source)
        {
            if (this.Content.activeSelf != this.IsVisible(source))
            {
                this.OnSetVisibility(!this.Content.activeSelf);
            }
        }

        protected virtual void OnSetVisibility(bool isVisible)
        {
            this.Content.SetActive(isVisible);
        }

        public virtual bool IsVisible(PlayerData source)
        {
            return true;
        }

    }

}
