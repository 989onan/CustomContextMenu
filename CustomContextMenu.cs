using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elements.Assets;
using Elements.Core;
using FrooxEngine;
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


        //[HarmonyPatch(typeof(UniversalImporter), "Import", typeof(AssetClass), typeof(IEnumerable<string>),
        //typeof(World), typeof(float3), typeof(floatQ), typeof(bool))]
        public partial class PatchMenu
        {



            public static bool Prefix()
            {

                return true;
            }
        }
    }
}
