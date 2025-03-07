﻿using System;

namespace WarpNetwork.api
{
	class WarpNetHandler : IWarpNetHandler
	{
		public Func<bool> getEnabled;
		public Func<string> getIconName;
		public Func<string> getLabel;
		public Action warp;
		internal WarpNetHandler(Func<bool> enabled = null, Func<string> icon = null, Func<string> label = null, Action warp = null)
		{
			getEnabled = enabled;
			getIconName = icon;
			getLabel = label;
			this.warp = warp;
		}
		public bool GetEnabled() => getEnabled();
		public string GetIconName() => getIconName();
		public string GetLabel() => getLabel();
		public void Warp() => warp();
	}
}
