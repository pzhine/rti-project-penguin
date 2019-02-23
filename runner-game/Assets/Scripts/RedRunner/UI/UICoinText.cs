using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RedRunner.Collectables;
using System;

namespace RedRunner.UI
{
	public class UICoinText : UIText
	{
		private string m_CoinTextFormat = "Objects Remaining: {0}";

		protected override void Awake ()
		{
			base.Awake ();
		}

        protected override void Start()
        {
            GameManager.Singleton.m_Coin.AddEventAndFire(UpdateCoinsText, this);
            Debug.Log("CoinText Start!");
        }

        private void UpdateCoinsText(int newCoinValue)
        {
            //GetComponent<Animator>().SetTrigger("Collect");
            text = string.Format(m_CoinTextFormat, newCoinValue);
        }
	}
}