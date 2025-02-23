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
            ContextHarmony.Patch(AccessTools.Method(typeof(UIBuilder), "Arc"), prefix: AccessTools.Method(typeof(PatchMenu), "CreateItem"));
            ContextHarmony.Patch(AccessTools.Method(typeof(ContextMenu), "Close"), prefix: AccessTools.Method(typeof(PatchMenu), "Close"));
            ContextHarmony.Patch(AccessTools.Method(typeof(ContextMenu), "OpenMenu"), prefix: AccessTools.Method(typeof(PatchMenu), "Open"), postfix: AccessTools.Method(typeof(PatchMenu), "OpenPost"));
            ContextHarmony.Patch(AccessTools.Method(typeof(Canvas), "ProcessTouchEvent"), prefix: AccessTools.Method(typeof(PatchMenu), "TouchEvent"));
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
                if (ArkSlot.ActiveUser != __instance.LocalUser)
                {
                    return true;
                }
                ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_placer", out Slot PlacerSlot);
                if (PlacerSlot is not null)
                {
                    PlacerSlot.DestroyChildren();
                }
                if (ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_Root", out Slot RootContextMenuObj))
                {

                    __instance.Slot.ActiveSelf = false;

                    FrooxEngine.Engine.Current.WorldManager.FocusedWorld.RunInUpdates(2, () =>
                    {
                        RootContextMenuObj.ActiveSelf = false;
                        is_unfocusing = false;
                    });
                    

                }

                return true;
            }

            public static bool is_unfocusing = false;
            public static bool TouchEvent(Canvas __instance, TouchEventInfo eventInfo, List<Predicate<IUIInteractable>> filters)
            {
                Slot ArkSlot = __instance.Slot;
                if (ArkSlot.ActiveUser != __instance.LocalUser)
                {
                    return true;
                }
                if (ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_Root", out Slot RootContextMenuObj))
                {
                    if(RootContextMenuObj.GetComponentInChildren<Canvas>() == __instance)
                    {
                        if (eventInfo.hover == EventState.End)
                        {
                            if (!is_unfocusing)
                            {
                                is_unfocusing = true;
                                FrooxEngine.Engine.Current.WorldManager.FocusedWorld.RunInSeconds(.2f, () =>
                                {
                                    if (is_unfocusing)
                                    {
                                        ArkSlot.ActiveUser.Root.Slot.GetComponentInChildren<ContextMenu>().Close();
                                    }
                                    is_unfocusing = false;
                                });
                            }


                        }
                        if (eventInfo.hover == EventState.Begin)
                        {
                            is_unfocusing = false;
                        }
                    }
                    
                }

                return true;
            }

            public static void OpenPost(ContextMenu __instance)
            {
                Slot ArkSlot = __instance.Slot;
                if (ArkSlot.ActiveUser != __instance.LocalUser)
                {
                    return;
                }
                if (ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_Root", out Slot RootContextMenuObj))
                {
                    __instance.Slot.ActiveSelf = false;
                }
                else
                {
                    __instance.Slot.ActiveSelf = true;
                }
            }

            public static bool Open(ContextMenu __instance)
            {
                Slot ArkSlot = __instance.Slot;

                if (ArkSlot.ActiveUser != __instance.LocalUser)
                {
                    return true;
                }
                ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_placer", out Slot PlacerSlot);
                if (PlacerSlot is not null)
                {
                    PlacerSlot.DestroyChildren();
                }

                if (ArkSlot.ActiveUser.Root.Slot.GetComponent<DynamicVariableSpace>(o => o.SpaceName.Value == "User").TryReadValue("CustomContextMenu_Root", out Slot RootContextMenuObj))
                {
                    RootContextMenuObj.ActiveSelf = true;
                    FrooxEngine.Engine.Current.WorldManager.FocusedWorld.RunInUpdates(2, () =>
                    {
                        RootContextMenuObj.CopyTransform(__instance.Slot);
                    });
                }

                return true;
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
                    DebugMsg("TemplateSlot is null");
                    return true;
                }
                Slot DuplicateTemplate = TemplateSlot.Duplicate();
                DuplicateTemplate.SetParent(PlacerSlot);
                Button Button = DuplicateTemplate.Name == "CustomContextMenu_button" ? DuplicateTemplate.GetComponent<Button>() : DuplicateTemplate.FindChild("CustomContextMenu_button").GetComponent<Button>();
                FrooxEngine.UIX.Image ButtonImage = DuplicateTemplate.FindChild("CustomContextMenu_image").GetComponent<FrooxEngine.UIX.Image>();
                FrooxEngine.UIX.Text ButtonText = DuplicateTemplate.FindChild("CustomContextMenu_text").GetComponent<FrooxEngine.UIX.Text>();
                if (PlacerSlot is null || Button is null || ButtonImage is null)
                {
                    DuplicateTemplate.Destroy();
                    DebugMsg("PlacerSlot, Button, or ButtonImage is null");
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

                //arcData.arcLayout.Nested.Target = arcData.image.RectTransform;
                if ((label) != null)
                {
                    arcData.text.LocaleContent = label;
                    arcData.arcLayout.Label.Target = arcData.text;
                }
                else
                {
                    DebugMsg("Label is null");
                }
                __instance.NestOut();
                __result = arcData;

                DebugMsg("Patch \'Arc\' has run successfully");

                ArkSlot.ActiveSelf = false;
                return false;
            }
        }
    }
}
