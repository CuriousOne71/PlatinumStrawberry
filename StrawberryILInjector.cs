
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using YamlDotNet.Core;
using MonoMod.Cil;
using Celeste.Mod.CollabUtils2;
using MonoMod.RuntimeDetour;
using Mono.Cecil;
//using Celeste;

namespace Celeste.Mod.PlatinumStrawberry
{
    internal class StrawberryILInjector
    {

        private static ILHook ctorHook;

        internal static void Load()
        {
            Logger.Log(LogLevel.Info, "InjectStrawberry", "hooking...");
            ctorHook = HookHelper.HookCoroutine("Strawberry", "Strawberry", InjectStrawberry);
            //PrintTypes("Celeste.exe");
        }

        internal static void Unload()
        {
            ctorHook?.Dispose();
        }

        static void InjectStrawberry(ILContext il)
        {
            Logger.Log(LogLevel.Info, "InjectStrawberry", "started");

            IL.Celeste.Strawberry.ctor += (il) =>
            {
                ILCursor c = new ILCursor(il);
                Logger.Log(LogLevel.Info, "InjectStrawberry", c.ToString());
            };

            
            // jump where 0.3 or 0.15f are loaded (those are dash times)
            /* while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.3f) || instr.MatchLdcR4(0.15f)))
            {
                Logger.Log("ExtendedVariantMode/DashLength", $"Applying dash length to constant at {cursor.Index} in CIL code for {cursor.Method.FullName}");

                cursor.EmitDelegate<Func<float>>(determineDashLengthFactor);
                cursor.Emit(OpCodes.Mul);
            } */
        }

        private static void PrintTypes(string fileName)
        {
            ModuleDefinition module = ModuleDefinition.ReadModule(fileName);
            foreach (TypeDefinition type in module.Types)
            {
                if (!type.IsPublic)
                    continue;

                Logger.Log(LogLevel.Info, "PrintTypes", type.FullName );
            }
        }
    }
}
