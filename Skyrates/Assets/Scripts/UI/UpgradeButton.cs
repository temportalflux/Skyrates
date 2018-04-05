using System.Collections.Generic;
using System.Linq;
using Skyrates.Data;
using Skyrates.Entity;
using Skyrates.Ship;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Skyrates.UI
{
	public class UpgradeButton : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
        //TODO: doxygen

	    public ShipComponentList ComponentList;
        public PlayerData PlayerData;

        // The inventory collectible count type (what count does the button look for in inventory)
        public ShipData.BrokenComponentType Type;

        // The ship component that this upgrade is for
	    public ShipData.ComponentType[] UpgradeComponentTypes;
        
	    public Text ComponentTitle;
        public Text LabelTier;
	    public Text LabelQuantity;

	    public Color Normal = Color.white;
        // not enough inventory contents
	    public Color UpgradeMissing;
        // ready for upgrade
	    public Color UpgradePending;

		private Button _button;
		private EntityPlayerShip _player;
	    private List<Upgrade> pendingUpgrades;
	    private uint totalCost;
	    private int tierMin;

        [HideInInspector]
		public Button Button
		{
			get
			{
				if(!this._button) this._button = GetComponent<Button>(); //Workaround: Since the parent game object is disabled at start, Awake is never called, so we must set the button explicitly.
				return this._button;
			}

			set
			{
				this._button = value;
			}
		}

	    public void RefreshPending()
	    {
	        this.pendingUpgrades = this.GetPendingUpgrades();
	        this.totalCost = this.GetTotalCost();
	        this.tierMin = this.GetMinTier();
	    }

	    public uint GetTotalCost()
	    {
	        return (uint)this.pendingUpgrades.Sum(upgrade => (int)upgrade.Cost);
	    }

	    public int GetMinTier()
	    {
	        return this.pendingUpgrades.Min(upgrade => upgrade.TierCurrent);
	    }

		//Removes item from local data and upgrades the tier by 1.
	    public void UpgradeItem()
	    {

            bool hasInfiniteInv = false;
#if UNITY_EDITOR
	        hasInfiniteInv = this.PlayerData.DebugInfiniteUpgrade;
#endif

	        bool isUpgradableFurther = this.pendingUpgrades.Count > 0;

	        // If all components have a next tier, and we have enough inventory
	        if (isUpgradableFurther && (hasInfiniteInv || this.PlayerData.Inventory.Remove(Type, this.totalCost) != 0))
	        {
	            // Upgrade the component
	            this._player.ShipGeneratorRoot.UpgradeComponents(this.UpgradeComponentTypes);

	            // Regenerate the ship rig
	            this._player.ShipGeneratorRoot.ReGenerate();

                this.RefreshPending();
	        }
	    }

	    struct Upgrade
	    {
	        public ShipData.ComponentType Type;
	        public int TierCurrent;
	        public int TierNext;
	        public uint Cost;
	    }
        
	    private List<Upgrade> GetPendingUpgrades()
	    {
            List<Upgrade> upgrades = new List<Upgrade>();

	        ShipData tierData = this._player.ShipData;

            foreach (ShipData.ComponentType type in this.UpgradeComponentTypes)
	        {
	            Upgrade upgrade = new Upgrade
	            {
                    Type = type,
                    TierCurrent = tierData.ComponentTiers[(int)type],
                };
	            upgrade.TierNext = upgrade.TierCurrent + 1;

	            MonoBehaviour[] prefabs = this.ComponentList.Categories[this.ComponentList.GetIndexFrom(type)].Prefabs;

	            if (upgrade.TierNext >= prefabs.Length) continue;

                upgrade.Cost = ((ShipComponent) prefabs[upgrade.TierNext]).CostToUpgradeTo;
	            upgrades.Add(upgrade);
	        }

	        return upgrades;
	    }

		public void AddUpgradeListener(EntityPlayerShip player)
		{
			this._player = player;
			this.Button.onClick.AddListener(UpgradeItem);
		}

		public void RemoveUpgradeListener()
		{
			this.Button.onClick.RemoveAllListeners();
		}

	    private uint GetInvAmount()
	    {
	        return this.PlayerData.Inventory.GetAmount(this.Type);
	    }

	    void Update()
	    {
	        this.LabelTier.text = string.Format("Tier {0}", this.tierMin + 1);
	        this.LabelQuantity.text = string.Format("{0} / {1}", this.GetInvAmount(), this.totalCost);
	        this.Button.interactable = this.pendingUpgrades.Count > 0;
	        this.Button.GetComponent<Image>().color = this.GetCurrentColor();
	    }

	    private Color GetCurrentColor()
	    {
	        if (this.pendingUpgrades.Count <= 0) return this.Normal;
            else if (this.GetInvAmount() < this.totalCost) return this.UpgradeMissing;
	        else return this.UpgradePending;
	    }

	    private string GetTitle()
	    {
	        if (this.UpgradeComponentTypes.Length <= 0) return this.Type.ToString();
	        else return ShipData.ToString(this.UpgradeComponentTypes[0]);
	    }

	    public void OnSelect(BaseEventData eventData)
	    {
	        this.ComponentTitle.text = this.GetTitle();
	    }

	    public void OnDeselect(BaseEventData eventData)
	    {
	    }

	}
}
