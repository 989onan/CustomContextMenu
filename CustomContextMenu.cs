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

#nullable enable
namespace CustomContextMenu
{
    public class CustomContextMenu : ResoniteMod
    {
        public override string Name => "CustomContextMenu";
        public override string Author => "989onan";
        public override string Version => "1.0.0";

        internal static ModConfiguration? Config;

        public override void OnEngineInit()
        {
            Harmony ContextHarmony = new Harmony("net.989onan.CustomContextMenu");
            ContextHarmony.Patch(AccessTools.Method(typeof(UIBuilder), "Arc"),          prefix: AccessTools.Method(typeof(PatchMenu), "CreateItem"));
            ContextHarmony.Patch(AccessTools.Method(typeof(ContextMenu), "Close"),      prefix: AccessTools.Method(typeof(PatchMenu), "Close"));
            ContextHarmony.Patch(AccessTools.Method(typeof(ContextMenu), "OpenMenu"),   prefix: AccessTools.Method(typeof(PatchMenu), "Open"));
            ContextHarmony.PatchAll();
            Config = GetConfiguration();
        }

        //[HarmonyPatch(typeof(UniversalImporter), "Import", typeof(AssetClass), typeof(IEnumerable<string>),
        //typeof(World), typeof(float3), typeof(floatQ), typeof(bool))]



        public class PatchMenu
        {
            public static bool Close(ContextMenu __instance)
            {
                //this._state.Value = ContextMenu.State.Closed;
                Slot ArkSlot = __instance.Slot;
                ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_placer", out Slot PlacerSlot);
                if (PlacerSlot is not null)
                {
                    PlacerSlot.DestroyChildren();
                }
                return true;
            }

            public static void OpenMenu(ContextMenu __instance)
            {
                Slot ArkSlot = __instance.Slot;
                ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_placer", out Slot PlacerSlot);
                if (PlacerSlot is not null)
                {
                    PlacerSlot.DestroyChildren();
                }
            }

            private static void DebugMsg(string msg)
            {
                #if DEBUG
                    Msg(msg);
                #endif
            }

            public static bool CreateItem(ref ArcData __result, UIBuilder __instance, LocaleString label, bool setupButton)
            {
                /*
                LocaleString label = (LocaleString)__args[0];
                bool setupButton = (bool)__args[1]
                */

                Slot ArkSlot = __instance.Next("Arc");

                ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_placer", out Slot PlacerSlot);
                ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_template", out Slot TemplateSlot);

                if (TemplateSlot is null)
                {
                    Msg("TemplateSlot is null");
                    return true;
                }
                DebugMsg("-4");
                Slot DuplicateTemplate = TemplateSlot.Duplicate();
                DebugMsg("-3");
                DuplicateTemplate.SetParent(PlacerSlot);
                DebugMsg("-2");
                Button Button = DuplicateTemplate.Name == "CustomContextMenu_button" ? DuplicateTemplate.GetComponent<Button>() : DuplicateTemplate.FindChild("CustomContextMenu_button").GetComponent<Button>();
                DebugMsg("-1");
                FrooxEngine.UIX.Image ButtonImage = DuplicateTemplate.FindChild("CustomContextMenu_image").GetComponent<FrooxEngine.UIX.Image>();
                DebugMsg("0");
                FrooxEngine.UIX.Text ButtonText = DuplicateTemplate.FindChild("CustomContextMenu_text").GetComponent<FrooxEngine.UIX.Text>();
                DebugMsg("1");
                if (PlacerSlot is null || Button is null || ButtonImage is null)
                {
                    DuplicateTemplate.Destroy();
                    Msg("PlacerSlot, Button, or ButtonImage is null");
                    return true;
                }
                DebugMsg("2");
                ArcData arcData = default(ArcData);
                DebugMsg("3");
                arcData.arc = __instance.Current.AttachComponent<OutlinedArc>(true, null);
                DebugMsg("4");
                arcData.arcLayout = __instance.Current.AttachComponent<ArcSegmentLayout>(true, null);
                DebugMsg("5");
                if (setupButton)
                {
                    DebugMsg("6");
                    arcData.button = Button;
                    DebugMsg("7");
                    UIBuilder.SetupButtonColor(arcData.button, arcData.arc);
                }
                DebugMsg("9");
                __instance.Nest();
                DebugMsg("10");
                arcData.image = ButtonImage;
                DebugMsg("11");
                arcData.text = ButtonText;
                DebugMsg("12");

                //arcData.arcLayout.Nested.Target = arcData.image.RectTransform;
                if ((label) != null)
                {
                    DebugMsg("13");
                    arcData.text.LocaleContent = label;
                    DebugMsg("14");
                    arcData.arcLayout.Label.Target = arcData.text;
                }
                else
                {
                    Msg("Label is null");
                }
                DebugMsg("15");
                __instance.NestOut();
                DebugMsg("16");
                __result = arcData;

                Msg("Function has run");

                ArkSlot.ActiveSelf = false;
                return false;
            }
        }
    }
}
