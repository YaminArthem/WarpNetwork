﻿using System;

namespace WarpNetwork.api
{
	public interface IWarpNetAPI
	{
		void AddCustomDestinationHandler(string ID, Func<bool> getEnabled, Func<string> getLabel, Func<string> getIconName, Action warp);
		void RemoveCustomDestinationHandler(string ID);
		bool CanWarpTo(string ID);
		bool DestinationExists(string ID);
		bool DestinationIsCustomHandler(string ID);
		bool WarpTo(string ID);
		void ShowWarpMenu(bool Force = false);
		void ShowWarpMenu(string Exclude);
		string[] GetDestinations();
		string[] GetItems();
	}
}
