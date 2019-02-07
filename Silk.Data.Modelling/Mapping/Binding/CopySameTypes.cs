﻿using Silk.Data.Modelling.Analysis;
using Silk.Data.Modelling.GenericDispatch;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class CopySameTypesFactory<TFromModel, TFromField, TToModel, TToField> :
		IBindingFactory<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public void CreateBinding(
			MappingFactoryContext<TFromModel, TFromField, TToModel, TToField> mappingFactoryContext,
			IntersectedFields<TFromModel, TFromField, TToModel, TToField> intersectedFields)
		{
			if (!intersectedFields.LeftField.CanRead ||
				!intersectedFields.RightField.CanWrite ||
				intersectedFields.LeftField.FieldDataType != intersectedFields.RightField.FieldDataType ||
				mappingFactoryContext.IsToFieldBound(intersectedFields))
			{
				return;
			}

			var builder = new BindingBuilder();
			intersectedFields.Dispatch(builder);

			mappingFactoryContext.Bindings.Add(builder.Binding);
		}

		private class BindingBuilder : IIntersectedFieldsGenericExecutor
		{
			public IBinding<TFromModel, TFromField, TToModel, TToField> Binding { get; private set; }

			void IIntersectedFieldsGenericExecutor.Execute<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData>(
				IntersectedFields<TLeftModel, TLeftField, TRightModel, TRightField, TLeftData, TRightData> intersectedFields
				)
			{
				Binding = new CopySameTypesBinding<TFromModel, TFromField, TToModel, TToField, TLeftData>(
					intersectedFields.LeftPath as IFieldPath<TFromModel, TFromField>,
					intersectedFields.RightPath as IFieldPath<TToModel, TToField>
					);
			}
		}
	}

	public class CopySameTypesBinding<TFromModel, TFromField, TToModel, TToField, TData> :
		IBinding<TFromModel, TFromField, TToModel, TToField>
		where TFromField : class, IField
		where TToField : class, IField
		where TFromModel : IModel<TFromField>
		where TToModel : IModel<TToField>
	{
		public TToField ToField => ToPath.FinalField;

		public TFromField FromField => FromPath.FinalField;

		public IFieldPath<TToModel, TToField> ToPath { get; }

		public IFieldPath<TFromModel, TFromField> FromPath { get; }

		public CopySameTypesBinding(IFieldPath<TFromModel, TFromField> fromPath, IFieldPath<TToModel, TToField> toPath)
		{
			FromPath = fromPath;
			ToPath = toPath;
		}

		public void Run(IGraphReader<TFromModel, TFromField> source, IGraphWriter<TToModel, TToField> destination)
		{
			if (!source.CheckPath(FromPath) || !destination.CheckPath(ToPath))
				return;

			destination.Write<TData>(ToPath, source.Read<TData>(FromPath));
		}
	}
}
