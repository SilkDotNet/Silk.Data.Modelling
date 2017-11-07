﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Silk.Data.Modelling.Bindings
{
	public class EnumerableBinding : ModelBinding
	{
		private readonly Type _modelEnumerableType;
		private readonly Type _viewEnumerableType;
		private readonly ModelBinding _baseBinding;
		private readonly EnumerableBuilder _modelEnumerableBuilder;
		private readonly EnumerableBuilder _viewEnumerableBuilder;

		public override BindingDirection Direction => _baseBinding.Direction;

		public EnumerableBinding(ModelBinding baseBinding,
			Type modelEnumerableType, Type viewEnumerableType) :
			base(baseBinding.ModelFieldPath, baseBinding.ViewFieldPath, baseBinding.ResourceLoaders)
		{
			_modelEnumerableType = modelEnumerableType;
			_viewEnumerableType = viewEnumerableType;
			_baseBinding = baseBinding;
			_modelEnumerableBuilder = CreateEnumerableBuilder(modelEnumerableType);
			_viewEnumerableBuilder = CreateEnumerableBuilder(viewEnumerableType);
		}

		private EnumerableBuilder CreateEnumerableBuilder(Type enumerableType)
		{
			var (elementType, enumBaseType) = enumerableType.GetDataAndEnumerableType();
			if (enumerableType.IsArray)
			{
				return Activator.CreateInstance(typeof(ArrayBuilder<>).MakeGenericType(elementType))
					as EnumerableBuilder;
			}
			else
			{
				return Activator.CreateInstance(typeof(ListBuilder<>).MakeGenericType(elementType))
					as EnumerableBuilder;
			}
		}

		public override object ReadFromContainer(IContainer container, MappingContext mappingContext)
		{
			var containerEnum = _baseBinding.ReadFromContainer(container, mappingContext) as IEnumerable;
			return _modelEnumerableBuilder.CreateFromSource(containerEnum);
		}

		public override object ReadFromModel(IModelReadWriter modelReadWriter, MappingContext mappingContext)
		{
			var modelEnum = _baseBinding.ReadFromModel(modelReadWriter, mappingContext) as IEnumerable;
			return _viewEnumerableBuilder.CreateFromSource(modelEnum);
		}

		private abstract class EnumerableBuilder
		{
			public abstract IEnumerable CreateFromSource(IEnumerable source);
		}

		private class ArrayBuilder<T> : EnumerableBuilder
		{
			public override IEnumerable CreateFromSource(IEnumerable source)
			{
				return source.OfType<T>().ToArray();
			}
		}

		private class ListBuilder<T> : EnumerableBuilder
		{
			public override IEnumerable CreateFromSource(IEnumerable source)
			{
				return source.OfType<T>().ToList();
			}
		}
	}
}
