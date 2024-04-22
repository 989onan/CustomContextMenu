using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Elements.Assets;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using Leap;
using ResoniteModLoader;
using uDesktopDuplication;

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


        //[HarmonyPatch(typeof(UniversalImporter), "Import", typeof(AssetClass), typeof(IEnumerable<string>),
        //typeof(World), typeof(float3), typeof(floatQ), typeof(bool))]


        

        [HarmonyPatch]
        public class ContextMenuPatches
        {
            [HarmonyPatch(typeof(ContextMenu), "OnAwake")]
            public partial class Patch_ContextMenu_OnAwake
            {

                [HarmonyPrefix]
                public static bool Prefix()
                {
                    return false;
                }
            }

            [HarmonyPatch(typeof(ContextMenu), "OnAttach")]
            public partial class Patch_ContextMenu_OnAwake
            {

                [HarmonyPrefix]
                public static bool Prefix(ContextMenu __instance)
                {

                    return true;

                }
            }

            [HarmonyPatch(typeof(ContextMenu), "StartNewMenu")]
            public partial class Patch_ContextMenu_Close
            {
                [HarmonyPostfix]
                public static void PostFix(ContextMenu __instance)
                {
                    //__instance


                }
            }
            [HarmonyPatch(typeof(UIBuilder), nameof(UIBuilder.Arc), typeof(LocaleString), typeof(bool))]
            public partial class Patch_ContextMenu_Arc
            {

                //static FieldInfo f_someField = AccessTools.Field(typeof(SomeType), nameof(SomeType.someField));
                static MethodInfo CreateButton = SymbolExtensions.GetMethodInfo(() => AccessTools.DeclaredMethod(typeof(Patch_ContextMenu_Close), "Button", new Type[] { typeof(LocaleString) }, null));

                public static bool Prefix(ref ArcData __result, UIBuilder __instance, in LocaleString label, bool setupButton = true)
                {

                    __instance.Next("CustomContextItem");
                    List<Slot> TargetCustomSlots = Engine.Current.WorldManager.FocusedWorld.LocalUser.LocalUserRoot.Slot.GetChildrenWithTag("CustomContextMenuTarget");
                    Slot current = __instance.Current;
                    if (TargetCustomSlots.Count > 0){
                        __instance.Current.SetParent(TargetCustomSlots.FirstOrDefault());
                    }
                    ArcData arcData = default(ArcData);
                    arcData.arc = __instance.Current.AttachComponent<OutlinedArc>(true, null);
                    arcData.arcLayout = __instance.Current.AttachComponent<ArcSegmentLayout>(true, null);
                    if (setupButton)
                    {
                        arcData.button = __instance.Current.AttachComponent<Button>(true, null);
                        UIBuilder.SetupButtonColor(arcData.button, arcData.arc);
                    }
                    __instance.Nest();
                    arcData.image = __instance.Image();
                    arcData.arcLayout.Nested.Target = arcData.image.RectTransform;
                    if (label != null)
                    {
                        arcData.text = __instance.Text(label, true, null, true, null);
                        arcData.arcLayout.Label.Target = arcData.text;
                    }
                    __instance.NestOut();
                    __result = arcData;
                    return false;
                }
            }
        }
        /*
                    this
                    this.CheckBuildingUI();
                    ArcData arcData;
                    if (label.content != null)
                    {
                        arcData = this._ui.Arc(label, true);
                    }
                    else
                    {
                        UIBuilder ui = this._ui;
                        LocaleString localeString = "";
                        arcData = ui.Arc(localeString, true);
                    }
                    ArcData arcData2 = arcData;
                    if (sprite == null)
                    {
                        if (texture != null)
                        {
                            SpriteProvider spriteProvider = arcData2.image.Slot.AttachComponent<SpriteProvider>(true, null);
                            spriteProvider.Texture.Target = texture;
                            sprite = spriteProvider;
                        }
                        else
                        {
                            SpriteProvider spriteProvider2 = arcData2.image.Slot.AttachSprite(icon, false, true, true, null);
                            texture = spriteProvider2.Texture.Target;
                            sprite = spriteProvider2;
                        }
                    }
                    arcData2.arc.InnerRadiusRatio.Value = this.RadiusRatio;
                    arcData2.arcLayout.LabelSize.Value = this.LabelSize;
                    arcData2.arcLayout.NestedSizeRatio.Value = 0.65f;
                    arcData2.arc.OutlineThickness.Value = 3f;
                    arcData2.arc.RoundedCornerRadius.Value = 16f;
                    arcData2.arc.Material.Target = this._arcMaterial.Target;
                    arcData2.text.Material.Target = this._fontMaterial.Target;
                    arcData2.text.Color.Value = RadiantUI_Constants.TEXT_COLOR;
                    arcData2.text.Size.Value = 50f;
                    arcData2.text.AutoSizeMax.Value = 50f;
                    arcData2.image.Sprite.Target = sprite;
                    arcData2.image.Material.Target = this._spriteMaterial.Target;
                    ContextMenuItem contextMenuItem = arcData2.button.Slot.AttachComponent<ContextMenuItem>(true, null);
                    arcData2.button.HoverVibrate.Value = VibratePreset.Medium;
                    arcData2.button.PressVibrate.Value = VibratePreset.Long;
                    InteractionElement.ColorDriver colorDriver = arcData2.button.ColorDrivers.Add();
                    colorDriver.ColorDrive.Target = arcData2.image.Tint;
                    colorDriver.NormalColor.Value = colorX.White;
                    colorDriver.HighlightColor.Value = colorX.White;
                    colorDriver.PressColor.Value = colorX.White;
                    colorDriver.DisabledColor.Value = colorX.White.SetA(0.53f);
                    contextMenuItem.Initialize(this, arcData2.arc, arcData2.button);
                    contextMenuItem.Icon.Target = arcData2.image;
                    contextMenuItem.Sprite.Target = (sprite as SpriteProvider);
                    contextMenuItem.Label.Target = arcData2.text.Content;
                    if (color != null)
                    {
                        contextMenuItem.Color.Value = color.Value;
                    }
                    else
                    {
                        BitmapAssetMetadata bitmapAssetMetadata = contextMenuItem.Slot.AttachComponent<BitmapAssetMetadata>(true, null);
                        bitmapAssetMetadata.Asset.Target = (texture as IAssetProvider<Texture2D>);
                        contextMenuItem.Color.DriveFrom(bitmapAssetMetadata.AverageVisibleHSV, false, false, true);
                    }
                    contextMenuItem.UpdateColor();
                    return contextMenuItem;
                }
            }
        }*/
    }
}
