using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Elements.Assets;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using Leap;
using ResoniteModLoader;

#nullable enable
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
            Harmony ContextHarmony = new Harmony("net.989onan.CustomContextMenu");
            MethodInfo contextMenuPatch = AccessTools.Method(typeof(UIBuilder), "Arc");
            ContextHarmony.Patch(contextMenuPatch, prefix: AccessTools.Method(typeof(PatchMenu), "Prefix"));

            ContextHarmony.PatchAll();
            Config = GetConfiguration();
        }
        public class PatchMenu
        {
            public static bool Prefix(ref ArcData __result, UIBuilder __instance, LocaleString label, bool setupButton)
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
                Slot DuplicateTemplate = TemplateSlot.Duplicate();
                DuplicateTemplate.SetParent(PlacerSlot);
                Button Button = DuplicateTemplate.FindChild("CustomContextMenu_button").GetComponent<Button>();
                FrooxEngine.UIX.Image ButtonImage = DuplicateTemplate.FindChild("CustomContextMenu_image").GetComponent<FrooxEngine.UIX.Image>();
                FrooxEngine.UIX.Text ButtonText = DuplicateTemplate.FindChild("CustomContextMenu_text").GetComponent<FrooxEngine.UIX.Text>();
                Msg("1");
                if (PlacerSlot is null || Button is null || ButtonImage is null)
                {
                    DuplicateTemplate.Destroy();
                    Msg("PlacerSlot, Button, or ButtonImage is null");
                    return true;
                }
                Msg("2");
                ArcData arcData = default(ArcData);
                Msg("3");
                arcData.arc = __instance.Current.AttachComponent<OutlinedArc>(true, null);
                Msg("4");
                arcData.arcLayout = __instance.Current.AttachComponent<ArcSegmentLayout>(true, null);
                Msg("5");
                if (setupButton)
                {
                    Msg("6");
                    arcData.button = Button;
                    Msg("7");
                    UIBuilder.SetupButtonColor(arcData.button, arcData.arc);
                }
                Msg("9");
                __instance.Nest();
                Msg("10");
                arcData.image = ButtonImage;
                Msg("11");
                arcData.text = ButtonText;
                Msg("12");

                //arcData.arcLayout.Nested.Target = arcData.image.RectTransform;
                if ((label) != null)
                {
                    Msg("13");
                    arcData.text.LocaleContent = label;
                    Msg("14");
                    arcData.arcLayout.Label.Target = arcData.text;
                }
                else
                {
                    Msg("Label is null");
                }
                Msg("15");
                __instance.NestOut();
                Msg("16");
                __result = arcData;

                Msg("Function has run");

                ArkSlot.ActiveSelf = false;
                return false;
            }
        }

    }
}
