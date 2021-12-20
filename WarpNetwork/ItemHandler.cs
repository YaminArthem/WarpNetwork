﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using WarpNetwork.models;

namespace WarpNetwork
{
    class ItemHandler
    {
        public static void ButtonPressed(object sender, ButtonPressedEventArgs action)
        {
            if (action.IsSuppressed())
            {
                return;
            }
            Farmer who = Game1.player;
            if (action.Button.IsActionButton())
            {
                if (CanUseHere(who))
                {
                    if (who.ActiveObject != null && !(who.ActiveObject is Furniture))
                    {
                        if (!who.canMove || who.ActiveObject.isTemporarilyInvisible)
                        {
                            return;
                        }
                        string id = null;
                        if (ModEntry.dgaAPI != null)
                        {
                            id = ModEntry.dgaAPI.GetDGAItemId(who.ActiveObject);

                        }
                        if (id == null)
                        {
                            id = who.ActiveObject.ParentSheetIndex.ToString();
                        }
                        if (UseItem(who, id))
                        {
                            ModEntry.helper.Input.Suppress(action.Button);
                        }
                    }
                }
            }
            else if (action.Button.IsUseToolButton() && who.CurrentTool is Wand && ModEntry.config.AccessFromWand && ModEntry.config.MenuEnabled)
            {
                if (CanUseHere(who) && who.CanMove)
                {
                    WarpHandler.ShowWarpMenu("_wand");
                    ModEntry.helper.Input.Suppress(action.Button);
                }
            }
        }
        private static bool UseItem(Farmer who, string id)
        {
            Dictionary<string, WarpItem> items = Utils.GetWarpItems();
            if (items.ContainsKey(id))
            {
                WarpItem item = items[id];
                if (item.Destination.ToLower() == "_all")
                {
                    WarpHandler.ShowWarpMenu("", item.Consume);
                    return true;
                }
                Color color = Utils.ParseColor(item.Color);
                DoTotemWarpEffects(color, id, item.Consume, who, (f) => WarpHandler.DirectWarp(item.Destination, item.IgnoreDisabled));
                return true;
            }
            return false;
        }
        private static bool CanUseHere(Farmer who)
        {
            return (
                    !who.UsingTool &&
                    !Game1.pickingTool &&
                    !Game1.menuUp &&
                    Game1.activeClickableMenu is null &&
                    !Game1.eventUp &&
                    !Game1.isFestival() &&
                    !Game1.nameSelectUp &&
                    Game1.numberOfSelectedItems == -1 &&
                    !Game1.fadeToBlack &&
                    !who.swimming &&
                    !who.bathingClothes &&
                    !who.onBridge
                    );
        }
        private static void DoTotemWarpEffects(Color color, string id, bool Consume, Farmer who, Func<Farmer, bool> action)
        {
            if (!int.TryParse(id, out int index))
            {
                index = Utils.GetDeterministicHashCode(id);
            }
            who.jitterStrength = 1f;
            who.currentLocation.playSound("warrior", NetAudio.SoundContext.Default);
            who.faceDirection(2);
            who.canMove = false;
            who.temporarilyInvincible = true;
            who.temporaryInvincibilityTimer = -4000;
            Game1.changeMusicTrack("none", false, Game1.MusicContext.Default);
            who.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[2]
            {
                new FarmerSprite.AnimationFrame(57, 2000, false, false,  null, false),
                new FarmerSprite.AnimationFrame( (short) who.FarmerSprite.CurrentFrame, 0, false, false, new AnimatedSprite.endOfAnimationBehavior((f) => {
                    if (action(f))
                    {
                        if(Consume){
                            who.reduceActiveItemByOne();
                        }
                    } else
                    {
                        who.temporarilyInvincible = false;
                        who.temporaryInvincibilityTimer = 0;
                    }
                }), true)
            }, null);
            // reflection
            Multiplayer mp = ModEntry.helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            // --
            mp.broadcastSprites(who.currentLocation,
            new TemporaryAnimatedSprite(index, 9999f, 1, 999, who.Position + new Vector2(0.0f, -96f), false, false, false, 0.0f)
            {
                motion = new Vector2(0.0f, -1f),
                scaleChange = 0.01f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                initialPosition = who.Position + new Vector2(0.0f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 1f
            },
            new TemporaryAnimatedSprite(index, 9999f, 1, 999, who.Position + new Vector2(-64f, -96f), false, false, false, 0.0f)
            {
                motion = new Vector2(0.0f, -0.5f),
                scaleChange = 0.005f,
                scale = 0.5f,
                alpha = 1f,
                alphaFade = 0.0075f,
                shakeIntensity = 1f,
                delayBeforeAnimationStart = 10,
                initialPosition = who.Position + new Vector2(-64f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 0.9999f
            },
            new TemporaryAnimatedSprite(index, 9999f, 1, 999, who.Position + new Vector2(64f, -96f), false, false, false, 0.0f)
            {
                motion = new Vector2(0.0f, -0.5f),
                scaleChange = 0.005f,
                scale = 0.5f,
                alpha = 1f,
                alphaFade = 0.0075f,
                delayBeforeAnimationStart = 20,
                shakeIntensity = 1f,
                initialPosition = who.Position + new Vector2(64f, -96f),
                xPeriodic = true,
                xPeriodicLoopTime = 1000f,
                xPeriodicRange = 4f,
                layerDepth = 0.9988f
            });
            Game1.screenGlowOnce(color, false, 0.005f, 0.3f);
            Utility.addSprinklesToLocation(who.currentLocation, who.getTileX(), who.getTileY(), 16, 16, 1300, 20, Color.White, null, true);
        }
    }
}
