﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Silk.Data.Modelling
{
	[DebuggerDisplay("{Name} {DataType}")]
	public class ViewField : IViewField
	{
		public string Name { get; }

		public Type DataType { get; }

		public object[] Metadata { get; }

		public ViewField(string name, Type dataType, IEnumerable<object> metadata)
		{
			Name = name;
			DataType = dataType;
			Metadata = metadata.ToArray();
		}
	}
}
