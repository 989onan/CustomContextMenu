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

                if (PlacerSlot is null || Button is null || ButtonImage is null)
                {
                    DuplicateTemplate.Destroy();
                    Msg("PlacerSlot, Button, or ButtonImage is null");
                    return true;
                }

                ArcData arcData = default(ArcData);
                arcData.arc = __instance.Current.AttachComponent<OutlinedArc>(true, null);
                arcData.arcLayout = __instance.Current.AttachComponent<ArcSegmentLayout>(true, null);
                if (setupButton)
                {
                    arcData.button = Button;
                    UIBuilder.SetupButtonColor(arcData.button, arcData.arc);
                }
                __instance.Nest();
                arcData.image = ButtonImage;
                arcData.text = ButtonText;

                arcData.arcLayout.Nested.Target = arcData.image.RectTransform;
                if ((label) != null)
                {
                    arcData.text.LocaleContent = label;
                    arcData.arcLayout.Label.Target = arcData.text;
                }
                __instance.NestOut();
                __result = arcData;

                Msg("Function has run");

                ArkSlot.ActiveSelf = false;
                return false;
            }
        }

    }
}
