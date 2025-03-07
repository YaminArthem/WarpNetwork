﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using xTile.Dimensions;

namespace WarpNetwork.models
{
	class WarpLocation
	{
		private Texture2D cachedIcon = null;

		public string Location { set; get; }
		public int X { set; get; } = 0;
		public int Y { set; get; } = 1;
		public virtual bool Enabled { set; get; } = false;
		public virtual string Label { set; get; }
		public bool OverrideMapProperty { set; get; } = false;
		public bool AlwaysHide { get; set; } = false;
		public string RequiredBuilding { set; get; } = null;
		public virtual string Icon { set; get; } = "";

		[JsonIgnore]
		public Texture2D IconTex
			=> cachedIcon ??= ModEntry.helper.GameContent.Load<Texture2D>("Data/WarpNetwork/Icons/" + Icon);
		public Location CoordsAsLocation() => new(X, Y);
		public bool IsAccessible()
			=> Enabled && (ModEntry.config.WarpsEnabled != WarpEnabled.AfterObelisk || 
			RequiredBuilding is null || DataPatcher.buildingTypes.Contains(RequiredBuilding.Collapse()));
		public void Reload()
			=> cachedIcon = null;

		public static implicit operator Point(WarpLocation where) => new(where.X, where.Y);
	}
}
