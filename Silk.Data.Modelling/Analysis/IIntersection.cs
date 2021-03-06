﻿namespace Silk.Data.Modelling.Analysis
{
	/// <summary>
	/// Describes overlapping relationship between two models.
	/// </summary>
	public interface IIntersection
	{
		/// <summary>
		/// Gets the left model in the relationship.
		/// </summary>
		IModel LeftModel { get; }

		/// <summary>
		/// Gets the right model in the relationship.
		/// </summary>
		IModel RightModel { get; }

		/// <summary>
		/// Gets an array of intersected fields.
		/// </summary>
		IntersectedFields[] IntersectedFields { get; }
	}

	/// <summary>
	/// Describes overlapping relationship between two models.
	/// </summary>
	public interface IIntersection<TLeftModel, TLeftField, TRightModel, TRightField> : IIntersection
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		/// <summary>
		/// Gets the left model in the relationship.
		/// </summary>
		new TLeftModel LeftModel { get; }

		/// <summary>
		/// Gets the right model in the relationship.
		/// </summary>
		new TRightModel RightModel { get; }

		/// <summary>
		/// Gets an array of intersected fields.
		/// </summary>
		new IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>[] IntersectedFields { get; }
	}
}
