﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Data.Modelling.Mapping.Binding
{
	public class EnumerableBinding<TFrom, TTo, TFromElement, TToElement> : MappingBinding
		where TFrom : class, IEnumerable<TFromElement>
		where TTo : class, IEnumerable<TToElement>
	{
		public EnumerableBinding(string[] fromPath, string[] toPath, MappingBinding elementBinding) : base(fromPath, toPath)
		{
			ElementBinding = elementBinding;
		}

		public MappingBinding ElementBinding { get; }

		public override void CopyBindingValue(IModelReadWriter from, IModelReadWriter to)
		{
			var source = from.ReadField<TFrom>(FromPath, 0);
			var result = new List<TToElement>();

			var sourceReader = new EnumerableModelReader<TFromElement>(source, TypeModel.GetModelOf(typeof(TFromElement)));
			var resultWriter = new EnumerableModelWriter<TToElement>(result, TypeModel.GetModelOf(typeof(TToElement)));

			while(sourceReader.MoveNext())
			{
				ElementBinding.CopyBindingValue(sourceReader, resultWriter);
			}

			if (typeof(TTo).IsArray)
				to.WriteField<TTo>(ToPath, 0, result.ToArray() as TTo);
			else
				to.WriteField<TTo>(ToPath, 0, result as TTo);
		}
	}

	public class EnumerableBindingFactory : IMappingBindingFactory<IMappingBindingFactory>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, IMappingBindingFactory bindingOption)
		{
			var elementBinding = typeof(IMappingBindingFactory).GetTypeInfo()
				.GetDeclaredMethod(nameof(IMappingBindingFactory.CreateBinding))
				.MakeGenericMethod(fromField.ElementType, toField.ElementType)
				.Invoke(bindingOption, new object[] { fromField, toField })
				as MappingBinding;

			return Activator.CreateInstance(typeof(EnumerableBinding<,,,>)
				.MakeGenericType(typeof(TFrom), typeof(TTo), fromField.ElementType, toField.ElementType),
				new object[] { fromField.FieldPath, toField.FieldPath, elementBinding }) as MappingBinding;
		}
	}

	public class EnumerableBindingFactory<T> : IMappingBindingFactory<(IMappingBindingFactory<T> factory, T option)>
	{
		public MappingBinding CreateBinding<TFrom, TTo>(ISourceField fromField, ITargetField toField, (IMappingBindingFactory<T> factory, T option) bindingOption)
		{
			var elementBinding = typeof(IMappingBindingFactory<T>).GetTypeInfo()
				.GetDeclaredMethod(nameof(IMappingBindingFactory<T>.CreateBinding))
				.MakeGenericMethod(fromField.ElementType, toField.ElementType)
				.Invoke(bindingOption.factory, new object[] { fromField, toField, bindingOption.option })
				as MappingBinding;

			return Activator.CreateInstance(typeof(EnumerableBinding<,,,>)
				.MakeGenericType(typeof(TFrom), typeof(TTo), fromField.ElementType, toField.ElementType),
				new object[] { fromField.FieldPath, toField.FieldPath, elementBinding }) as MappingBinding;
		}
	}

	internal class EnumerableModelWriter<TToElement> : IModelReadWriter
	{
		public IModel Model { get; }
		public List<TToElement> List { get; }
		private ObjectReadWriter _objectReadWriter;

		public EnumerableModelWriter(List<TToElement> list, IModel model)
		{
			Model = model;
			List = list;
		}

		public T ReadField<T>(string[] path, int offset)
		{
			//  todo: how to get a potential source object
			return default(T);
		}

		public void WriteField<T>(string[] path, int offset, T value)
		{
			if (path.Length > offset + 1 && path[offset + 1] == ".")
			{
				//  path taken for object mapping
				List.Add((TToElement)(object)value);
				_objectReadWriter = new ObjectReadWriter((TToElement)(object)value, Model, typeof(TToElement));
			}
			else
			{
				if (_objectReadWriter != null)
					_objectReadWriter.WriteField<T>(path, offset + 1, value);
				else
					//  path taken for straight value binding
					List.Add((TToElement)(object)value);
			}
		}
	}

	internal class EnumerableModelReader<TFromElement> : IModelReadWriter
	{
		public IModel Model { get; }
		public IEnumerable Source { get; }
		private IEnumerator _enumerator;
		private ObjectReadWriter _objectReadWriter;

		public EnumerableModelReader(IEnumerable source, IModel model)
		{
			Model = model;
			Source = source;
			_enumerator = Source.GetEnumerator();
		}

		public bool MoveNext()
		{
			if (_enumerator.MoveNext())
			{
				_objectReadWriter = new ObjectReadWriter(_enumerator.Current, Model, typeof(TFromElement));
				return true;
			}

			_objectReadWriter = null;
			return false;
		}

		public T ReadField<T>(string[] path, int offset)
		{
			if (path.Length == offset + 1)
				return _objectReadWriter.ReadField<T>(path.Concat(new[] { "." }).ToArray(), offset + 1);
			return _objectReadWriter.ReadField<T>(path, offset + 1);
		}

		public void WriteField<T>(string[] path, int offset, T value)
		{
			throw new NotSupportedException();
		}
	}
}