using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Skyrates.UI
{

    public class StatBar : MonoBehaviour
    {

        public Text Label;
        public Text Amount;
        public Image Bar;

        public void SetAmountFilled(float amount, string text = "")
        {
            if (this.Amount)
                this.Amount.text = text;
            this.Bar.fillAmount = amount;
        }

    }

}
