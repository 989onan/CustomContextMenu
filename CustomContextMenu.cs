using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elements.Assets;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using Leap;
using ResoniteModLoader;


namespace CustomContextMenu
{
    public class CustomContextMenu : ResoniteMod
    {
        public override string Name => "CustomContextMenu";
        public override string Author => "989onan";
        public override string Version => "1.0.0";

        internal static ModConfiguration Config;

        public override void OnEngineInit()
        {
            new Harmony("net.989onan.CustomContextMenu").PatchAll();
            Config = GetConfiguration();
        }

        [HarmonyPatch(typeof(ContextMenu), "AddItem", typeof(LocaleString), typeof(IAssetProvider<ITexture2D>), typeof(Uri), typeof(IAssetProvider<Sprite>),
typeof(colorX?))]
        public partial class PatchMenu
        {
            public static bool Prefix(ref ContextMenuItem __result, ContextMenu __instance, in LocaleString label, IAssetProvider<ITexture2D> texture, Uri icon, IAssetProvider<Sprite> sprite, in colorX? color)
            {
                DynamicVariableSpace space = __instance.ActiveUser.Slot.FindNearestCompnent<DynamicVariableSpace>(o => o.SpaceName.Value == "CustomContextMenu");
                if (space == null) { return true; }
                //private methods, innaccessable?
                //__instance.CheckOwner();
                //__instance.CheckBuildingUI();
                ArcData arcData = ((label.content == null) ? _ui.Arc((LocaleString)"") : _ui.Arc(in label));
                if (sprite == null)
                {
                    if (texture != null)
                    {
                        SpriteProvider spriteProvider = arcData.image.Slot.AttachComponent<SpriteProvider>();
                        spriteProvider.Texture.Target = texture;
                        sprite = spriteProvider;
                    }
                    else
                    {
                        SpriteProvider spriteProvider2 = arcData.image.Slot.AttachSprite(icon, uncompressed: false, evenNull: true);
                        texture = spriteProvider2.Texture.Target;
                        sprite = spriteProvider2;
                    }
                }
                //arcData.arc.InnerRadiusRatio.Value = RadiusRatio;
                //arcData.arcLayout.LabelSize.Value = LabelSize;
                arcData.arcLayout.NestedSizeRatio.Value = 0.65f;
                arcData.arc.OutlineThickness.Value = 3f;
                arcData.arc.RoundedCornerRadius.Value = 16f;
                //arcData.arc.Material.Target = _arcMaterial.Target;
                //arcData.text.Material.Target = _fontMaterial.Target;
                arcData.text.Color.Value = RadiantUI_Constants.TEXT_COLOR;
                arcData.text.Size.Value = 50f;
                arcData.text.AutoSizeMax.Value = 50f;
                arcData.image.Sprite.Target = sprite;
                //arcData.image.Material.Target = _spriteMaterial.Target;
                ContextMenuItem contextMenuItem = arcData.button.Slot.AttachComponent<ContextMenuItem>();
                arcData.button.HoverVibrate.Value = VibratePreset.Medium;
                arcData.button.PressVibrate.Value = VibratePreset.Long;
                InteractionElement.ColorDriver colorDriver = arcData.button.ColorDrivers.Add();
                colorDriver.ColorDrive.Target = arcData.image.Tint;
                colorDriver.NormalColor.Value = colorX.White;
                colorDriver.HighlightColor.Value = colorX.White;
                colorDriver.PressColor.Value = colorX.White;
                colorDriver.DisabledColor.Value = colorX.White.SetA(0.53f);
                //contextMenuItem.Initialize(this, arcData.arc, arcData.button);
                contextMenuItem.Icon.Target = arcData.image;
                contextMenuItem.Sprite.Target = sprite as SpriteProvider;
                contextMenuItem.Label.Target = arcData.text.Content;
                if (color.HasValue)
                {
                    contextMenuItem.Color.Value = color.Value;
                }
                else
                {
                    BitmapAssetMetadata bitmapAssetMetadata = contextMenuItem.Slot.AttachComponent<BitmapAssetMetadata>();
                    bitmapAssetMetadata.Asset.Target = texture as IAssetProvider<Texture2D>;
                    contextMenuItem.Color.DriveFrom(bitmapAssetMetadata.AverageVisibleHSV);
                }
                contextMenuItem.UpdateColor();
                __result = contextMenuItem;

                return false;
            }
        }
    }
}
