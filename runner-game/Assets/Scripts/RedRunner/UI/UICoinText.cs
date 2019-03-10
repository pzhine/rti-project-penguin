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

        protected override void Awake()
        {
            base.Awake();
        }
        void Update()
        {
            text = string.Format(m_CoinTextFormat, GameManager.Singleton.m_Coin);
        }
	}
}