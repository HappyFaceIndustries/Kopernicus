/**
 * Kopernicus Planetary System Modifier
 * ====================================
 * Created by: - Bryce C Schroeder (bryce.schroeder@gmail.com)
 * 			   - Nathaniel R. Lewis (linux.robotdude@gmail.com)
 * 
 * Maintained by: - Thomas P.
 * 				  - NathanKell
 * 
* Additional Content by: Gravitasi, aftokino, KCreator, Padishar, Kragrathea, OvenProofMars, zengei, MrHappyFace
 * ------------------------------------------------------------- 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 * 
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright 2011-2014 Squad. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 * 
 * https://kerbalspaceprogram.com
 */

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Kopernicus.Constants;

using UnityEngine;

// Disable the "private fields `` is assigned but its value is never used warning"
#pragma warning disable 0414

namespace Kopernicus
{
	namespace Configuration 
	{
		namespace ModLoader
		{
			[RequireConfigType(ConfigType.Node)]
			public class PQSModLoader : IParserEventSubscriber
			{
				// Constructor with a PQSversion
				public PQSModLoader (PQS pqs)
				{
					pqsVersion = pqs;
				}

				public PQS pqsVersion
				{
					get;
					private set;
				}

				public ModLoader[] Mods = null;

				// IParserEventSubscriber methods
				void IParserEventSubscriber.Apply(ConfigNode node)
				{

				}
				void IParserEventSubscriber.PostApply(ConfigNode node)
				{
					// Get all preexisting PQSMods on the PQS, before any are added, as to avoid conflicts
					var existingMods = pqsVersion.GetComponentsInChildren<PQSMod> ();
					var patchedMods = new List<PQSMod> ();

					var allMods = new List<ModLoader> ();

					// Go through all nodes, added PQSMods from ModLoaders
					foreach (var modNode in node.GetNodes())
					{
						// Load the ModLoader type for this node; if it does not exist, log it and continue.
						Type pqsModType = null;
						var modLoaderType = GetModLoaderType (modNode.name, out pqsModType);
						if (modLoaderType == null || pqsModType == null)
						{
							Logger.Active.Log ("PQS Mod node " + modNode.name + " was not recognized");
							continue;
						}

						ModLoader modLoader = null;

						// Get the name
						string name = null;
						if (modNode.HasValue ("name"))
							name = modNode.GetValue ("name");

						// Check for preexisting PQSMods that we have not edited yet
						foreach(var mod in existingMods)
						{
							if(mod.GetType() == pqsModType)
							{
								// Skip if we have already patched this mod
								if (patchedMods.Contains (mod))
									continue;

								// Skip if a name is specified, and this mod's name is not the same
								if (name != null)
								{
									if (mod.name != name)
										continue;
								}

								// If all requirements are met, create a new loader from the existing mod
								modLoader = Parser.CreateObjectFromConfigNode (modLoaderType, modNode, new object[]{ mod });
								modLoader.patched = true;
								Logger.Active.Log ("PQSLoader.PostApply(ConfigNode): Patched preexisting PQS Mod => " + modLoaderType.Name + " : " + pqsModType.FullName);
							}
						}

						// If we are not patching, (we can tell because the modLoader will still be null) create a new one
						if (modLoader == null)
						{
							modLoader = (ModLoader)Parser.CreateObjectFromConfigNode (modLoaderType, modNode);
							Logger.Active.Log ("PQSLoader.PostApply(ConfigNode): Added new PQS Mod => " + modLoaderType.Name + " : " + pqsModType.FullName);
						}

						if (modLoader.mod != null)
						{
							modLoader.mod.transform.parent = pqsVersion.transform;
							modLoader.mod.gameObject.layer = GameLayers.LocalSpace;
							modLoader.mod.sphere = pqsVersion;

							allMods.Add (modLoader);
						}
					}

					// Put all ModLoaders into the Mods array
					Mods = allMods.ToArray ();
				}

				public void SphereApply()
				{
					//call SphereApply
					foreach (var mod in Mods)
					{
						mod.SphereApply ();
					}
				}

				private Type GetModLoaderType(string nodeName, out Type pqsModType)
				{
					// Look through every loaded assembly's types
					foreach (var assembly in AssemblyLoader.loadedAssemblies)
					{
						foreach (var type in assembly.assembly.GetTypes())
						{
							// Skip if it's name is not identical to nodeName
							if (type.Name != nodeName)
								continue;

							// Only check classes that extend ModLoader
							if (!type.IsSubclassOf (typeof(ModLoader)))
								continue;

							// Get the ModLoader attribute
							ModLoaderAttribute modLoaderAttr = null;
							foreach (var attribute in Attribute.GetCustomAttributes(type))
							{
								if (attribute is ModLoaderAttribute)
									modLoaderAttr = (ModLoaderAttribute)attribute;
							}

							// Skip if this type has no ModLoaderAttribute
							if (modLoaderAttr == null)
								continue;

							// Skip if the ModLoaderAttribute's type does not extend from PQSMod
							Type pqsMod = modLoaderAttr.type;
							if (!pqsMod.IsSubclassOf (typeof(PQSMod)))
								continue;

							// If all of these conditions are met, return
							pqsModType = pqsMod;
							return type;
						}
					}
				}
			}
		}
	}
}

