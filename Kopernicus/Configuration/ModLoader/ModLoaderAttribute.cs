using System;
using System.Reflection;
using System.Collections.Generic;

using Kopernicus.Configuration;

namespace Kopernicus
{
	namespace Configuration
	{
		namespace ModLoader
		{
			[AttributeUsage(AttributeTargets.Class)]
			public class ModLoaderAttribute : Attribute
			{
				public Type type;

				public ModLoaderAttribute (Type pqsModType)
				{
					type = pqsModType;
				}
			}
		}
	}
}

