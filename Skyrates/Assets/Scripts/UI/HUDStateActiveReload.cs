using Skyrates.Data;
using Skyrates.Misc;

namespace Skyrates.UI
{

    public abstract class HUDStateActiveReload : HUDState
    {

        public UIStateActiveReload UIState;

        protected override void Awake()
        {
            base.Awake();
            this.UIState.OnAwake();
        }

        public override void UpdateWith(PlayerData source)
        {
            base.UpdateWith(source);
            if (this.IsVisible(source))
            {
                this.UIState.UpdateWith(this.GetState(source));
            }
        }

        protected override void OnSetVisibility(bool isVisible)
        {
            base.OnSetVisibility(isVisible);
            this.UIState.IsActive = isVisible;
        }

        public abstract StateActiveReload GetState(PlayerData source);

    }

}