﻿namespace Silk.Data.Modelling.Conventions
{
	/// <summary>
	/// Defines a convention used for creating views.
	/// </summary>
	public abstract class ViewConvention
	{
		/// <summary>
		/// View types the view convention should be executed for.
		/// </summary>
		public abstract ViewType SupportedViewTypes { get; }

		/// <summary>
		/// Defines view fields from the given model field.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="field"></param>
		/// <param name="viewDefinition"></param>
		public virtual void MakeModelFields(Model model, TypedModelField field, ViewDefinition viewDefinition)
		{
		}

		/// <summary>
		/// Makes any finalizing changes to a view definition before a view is created.
		/// </summary>
		/// <param name="viewDefinition"></param>
		public virtual void FinalizeModel(ViewDefinition viewDefinition)
		{
		}
	}
}
