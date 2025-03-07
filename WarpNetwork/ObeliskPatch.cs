﻿using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using WarpNetwork.models;

namespace WarpNetwork
{
	class ObeliskPatch
	{
		private static readonly Dictionary<string, Point> ObeliskTargets = new()
		{
			{ "Farm", new Point(48, 7) },
			{ "IslandSouth", new Point(11, 11) },
			{ "Mountain", new Point(31, 20) },
			{ "Beach", new Point(20, 4) },
			{ "Desert", new Point(35, 43) }
		};
		public static void MoveAfterWarp(object sender, WarpedEventArgs ev)
		{
			if (ev.IsLocalPlayer)
			{
				string Name = (ev.NewLocation.Name == "BeachNightMarket") ? "Beach" : ev.NewLocation.Name;
				if (Name == "Desert")
				{
					//desert warp patch

					Point point = ObeliskTargets["Desert"];
					if (point != ev.Player.TilePoint)
						return;

					Point to = point;
					if (ModEntry.config.PatchObelisks)
						if (Utils.GetWarpLocations().TryGetValue("desert", out var dest) && dest.OverrideMapProperty)
							to = new(dest.X, dest.Y);
						else
							to = ev.NewLocation.GetPropertyPosition("WarpNetworkEntry", point);
					ev.Player.setTileLocation(new Vector2(to.X, to.Y));
				}
				else if (ModEntry.config.PatchObelisks)
				{
					if (ObeliskTargets.ContainsKey(Name))
					{
						Point point = ObeliskTargets[Name];
						if (Name == "Farm")
							point = Utils.GetActualFarmPoint(point.X, point.Y);
						if (ev.Player.TilePoint == point)
						{
							Dictionary<string, WarpLocation> dests = Utils.GetWarpLocations();
							string target = (Name == "IslandSouth") ? "island" : Name;
							Point to = (dests.TryGetValue(target, out var dest) && dest.OverrideMapProperty) ?
								new(dest.X, dest.Y) : ev.NewLocation.GetPropertyPosition("WarpNetworkEntry", point);
							ev.Player.setTileLocation(new Vector2(to.X, to.Y));
						}
					}
				}
			}
		}
	}
}
