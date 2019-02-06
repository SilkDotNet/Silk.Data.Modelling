﻿using System.Collections.Generic;

namespace Silk.Data.Modelling
{
	/// <summary>
	/// Data model of a CLR type.
	/// </summary>
	public abstract partial class TypeModel : IModel<PropertyInfoField>
	{
		public abstract IReadOnlyList<PropertyInfoField> Fields { get; }

		IReadOnlyList<IField> IModel.Fields => Fields;

		public abstract IEnumerable<PropertyInfoField> GetPathFields(IFieldPath<PropertyInfoField> fieldPath);
	}

	/// <summary>
	/// Data model of a CLR type.
	/// </summary>
	public class TypeModel<T> : TypeModel
	{
		public override IReadOnlyList<PropertyInfoField> Fields { get; }

		public TypeModel(IReadOnlyList<PropertyInfoField> fields)
		{
			Fields = fields;
		}

		public override IEnumerable<PropertyInfoField> GetPathFields(IFieldPath<PropertyInfoField> fieldPath)
			//  todo: if FieldDataType is an enumerable type should the element type be modelled or the the enumerable type?
			=> GetModelOf(fieldPath.FinalField.FieldDataType).Fields;
	}
}
