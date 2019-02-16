﻿using System;
using System.Linq;
using System.Reflection;
using Silk.Data.Modelling.Analysis.CandidateSources;

namespace Silk.Data.Modelling.Analysis.Rules
{
	/// <summary>
	/// Intersection rule for types that can be converted with an explicit cast.
	/// </summary>
	/// <typeparam name="TLeftModel"></typeparam>
	/// <typeparam name="TLeftField"></typeparam>
	/// <typeparam name="TRightModel"></typeparam>
	/// <typeparam name="TRightField"></typeparam>
	public class ExplicitCastRule<TLeftModel, TLeftField, TRightModel, TRightField> :
		IIntersectionRule<TLeftModel, TLeftField, TRightModel, TRightField>
		where TLeftModel : IModel<TLeftField>
		where TRightModel : IModel<TRightField>
		where TLeftField : class, IField
		where TRightField : class, IField
	{
		public bool IsValidIntersection(IntersectCandidate<TLeftModel, TLeftField, TRightModel, TRightField> intersectCandidate, out IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField> intersectedFields)
		{
			var castMethod = GetExplicitCast(
				intersectCandidate.LeftField.RemoveEnumerableType(),
				intersectCandidate.RightField.RemoveEnumerableType()
				);
			if (castMethod == null)
				castMethod = GetExplicitCast(
					intersectCandidate.RightField.RemoveEnumerableType(),
					intersectCandidate.RightField.RemoveEnumerableType()
					);

			if (intersectCandidate.LeftField.FieldDataType == intersectCandidate.RightField.FieldDataType || 
				castMethod == null ||
				intersectCandidate.LeftField.IsEnumerableType != intersectCandidate.RightField.IsEnumerableType)
			{
				intersectedFields = null;
				return false;
			}

			intersectedFields = IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField>.Create(
				intersectCandidate,
				typeof(ExplicitCastRule<TLeftModel, TLeftField, TRightModel, TRightField>),
				castMethod
				);
			return true;
		}

		private MethodInfo GetExplicitCast(Type fromType, Type toType)
		{
			return fromType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.FirstOrDefault(q => q.Name == "op_Explicit" && q.ReturnType == toType);
		}
	}
}