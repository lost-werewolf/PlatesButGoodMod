using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Tile_Entities;
using Terraria.ModLoader;

namespace PlatesButGood
{
	public class PlatesButGood : Mod
	{
        public override void Load()
        {
            IL_TEFoodPlatter.FitsFoodPlatter += HookFitsFoodPlatter;
        }

        private static void HookFitsFoodPlatter(ILContext il)
        {
            try {
                ILCursor c = new ILCursor(il);

                c.GotoNext(i => i.MatchLdcI4(0));

                c.Index += 2; // Jump to after ble.s

                c.Emit(OpCodes.Ldc_I4_1); // Setup returning true

                c.Emit(OpCodes.Ret); // Return true
            }
            catch (Exception e) {
                MonoModHooks.DumpIL(ModContent.GetInstance<PlatesButGood>(), il);
            }
        }
        public override void Unload()
        {
            IL_TEFoodPlatter.FitsFoodPlatter -= HookFitsFoodPlatter;
        }
    }
}
