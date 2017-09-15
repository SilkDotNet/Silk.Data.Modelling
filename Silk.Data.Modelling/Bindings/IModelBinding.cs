﻿using System;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Bindings
{
	/// <summary>
	/// Binds a viewfield to a source.
	/// </summary>
	public interface IModelBinding
	{
		/// <summary>
		/// Gets the direction the binding applies in.
		/// </summary>
		BindingDirection Direction { get; }

		/// <summary>
		/// Reads the binding value from the provided modelreadwriter.
		/// </summary>
		/// <param name="modelReadWriter"></param>
		/// <returns></returns>
		Task<object> ReadFromModelAsync(IModelReadWriter modelReadWriter);

		/// <summary>
		/// Writes the given value to the bound model field.
		/// </summary>
		/// <param name="modelReadWriter"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		Task WriteToModelAsync(IModelReadWriter modelReadWriter, object value);
	}

	[Flags]
	public enum BindingDirection
	{
		None = 0,
		ModelToView = 1,
		ViewToModel = 2,
		Bidirectional = 3
	}
}