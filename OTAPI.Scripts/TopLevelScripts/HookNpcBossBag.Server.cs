﻿/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
#pragma warning disable CS8321 // Local function is declared but never used
#pragma warning disable CS0436 // Type conflicts with imported type

using System;
using System.Linq;
using ModFramework;
using Mono.Cecil;
using Mono.Cecil.Cil;

/// <summary>
/// @doc Creates Hooks.NPC.BossBag. Allows plugins to cancel boss bag items.
/// </summary>
[Modification(ModType.PreMerge, "Hooking npc boss bags")]
void HookNpcBossBag(ModFramework.ModFwModder modder)
{
    // replace NewItem calls, and handle the -1 result to cancel the method from actioning.

    var csr = modder.GetILCursor(() => (new Terraria.NPC()).DropItemInstanced(default, default, 0, 0, false));
    var callback = csr.Module.ImportReference(
        modder.GetMethodDefinition(() => OTAPI.Hooks.NPC.InvokeBossBag(0, 0, 0, 0, 0, 0, false, 0, false, false, null))
    );

    var instructions = csr.Body.Instructions.Where(x => x.OpCode == OpCodes.Call
                                                        && x.Operand is MethodReference
                                                        && (x.Operand as MethodReference).Name == "NewItem"
                                                        && x.Next.OpCode == OpCodes.Stloc_0);

    if (instructions.Count() != 1) throw new NotSupportedException("Only one server NewItem call expected in DropBossBags.");

    var ins = instructions.First();

    ins.Operand = callback;

    csr.Goto(ins);
    csr.EmitAll(
        new { OpCodes.Ldarg_0 }
    );

    csr.Goto(ins.Next.Next);
    csr.EmitAll(
        new { OpCodes.Ldloc_0 },
        new { OpCodes.Ldc_I4_M1 },
        new { OpCodes.Ceq },
        new { OpCodes.Brfalse_S, Operand = ins.Next.Next },
        new { OpCodes.Ret }
    );
}

namespace OTAPI
{
    public static partial class Hooks
    {
        public static partial class NPC
        {
            public class BossBagEventArgs : EventArgs
            {
                public HookResult? Result { get; set; }

                public Terraria.NPC Npc { get; set; }
                public int X { get; set; }
                public int Y { get; set; }
                public int Width { get; set; }
                public int Height { get; set; }
                public int Type { get; set; }
                public int Stack { get; set; }
                public bool NoBroadcast { get; set; }
                public int Pfix { get; set; }
                public bool NoGrabDelay { get; set; }
                public bool ReverseLookup { get; set; }
            }
            public static event EventHandler<BossBagEventArgs> BossBag;

            public static int InvokeBossBag(
                int X,
                int Y,
                int Width,
                int Height,
                int Type,
                int Stack,
                bool noBroadcast,
                int pfix,
                bool noGrabDelay,
                bool reverseLookup,
                Terraria.NPC npc
            )
            {
                var args = new BossBagEventArgs()
                {
                    X = X,
                    Y = Y,
                    Width = Width,
                    Height = Height,
                    Type = Type,
                    Stack = Stack,
                    NoBroadcast = noBroadcast,
                    Pfix = pfix,
                    NoGrabDelay = noGrabDelay,
                    ReverseLookup = reverseLookup,
                    Npc = npc,
                };
                BossBag?.Invoke(null, args);
                if (args.Result == HookResult.Cancel)
                    return -1;

#if TerrariaServer_EntitySourcesActive
                return Terraria.Item.NewItem(npc.GetItemSource_Loot(), args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.NoGrabDelay, args.ReverseLookup);
#else
                return Terraria.Item.NewItem(args.X, args.Y, args.Width, args.Height, args.Type, args.Stack, args.NoBroadcast, args.Pfix, args.NoGrabDelay, args.ReverseLookup);
#endif
            }
        }
    }
}