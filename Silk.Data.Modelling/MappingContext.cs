﻿namespace Silk.Data.Modelling
{
	/// <summary>
	/// A context for mapping operations.
	/// </summary>
	public class MappingContext
	{
		/// <summary>
		/// Gets a collection of resources for the mapping context.
		/// </summary>
		public ResourceObjectCollection Resources { get; } = new ResourceObjectCollection();
	}
}