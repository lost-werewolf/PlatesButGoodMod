using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader;

namespace PlatesButGood
{
	public class PlatesButGood : Mod
	{
        public override void Load()
        {
            On_TEFoodPlatter.FitsFoodPlatter += (_, _) => true;
            IL_TileDrawing.DrawSpecialTilesLegacy += IL_TileDrawingOnDrawSpecialTilesLegacy;
        }

        private void IL_TileDrawingOnDrawSpecialTilesLegacy(ILContext il)
        {
            try
            {
                // Start the Cursor at the start
                var c = new ILCursor(il);

                // Try to locate where to perform the IL edit
                c.GotoNext(i => i.MatchLdsfld<ItemID.Sets>("IsFood"));

                c.EmitLdloc(24); // Load item3 local var onto stack (Item)
                c.EmitLdloc(25); // Load value3 local var on stack (Texture2D)

                c.EmitDelegate<Func<Item, Texture2D, Rectangle>>((item, texture) =>
                    !ItemID.Sets.IsFood[item.type]
                        ? Main.itemAnimations[item.type] == null
                            ? texture.Frame()
                            : Main.itemAnimations[item.type].GetFrame(texture)
                        : texture.Frame(1, 3, 0, 2));

                // Save current cursor position
                var index = c.Index;

                // Locate the next stloc.s instruction
                c.GotoNext(i => i.MatchStloc(out _));
                var insn = c.Next!;

                // Return to the saved cursor position
                c.Index = index;

                // Emit break instruction
                c.EmitBr(insn);
            }
            catch
            {
                MonoModHooks.DumpIL(this, il);
            }
        }
    }
}
