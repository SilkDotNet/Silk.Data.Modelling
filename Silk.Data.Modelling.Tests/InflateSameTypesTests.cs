﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Mapping;
using System.Linq;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class InflateSameTypesTests
	{
		[TestMethod]
		public void InflateSameTypesBindsReadWriteProperties()
		{
			var mapping = CreateMapping<FlatData, SourceRoot>();
			Assert.AreEqual(1, mapping.Bindings.Length);

			var flattenBinding = (MappingBinding)mapping.Bindings[0];
			Assert.IsInstanceOfType(flattenBinding, typeof(CopyBinding<int>));
			Assert.IsTrue(flattenBinding.ToPath.SequenceEqual(new[] { "Data", "Property" }));
			Assert.IsTrue(flattenBinding.FromPath.SequenceEqual(new[] { "DataProperty" }));
		}

		private Mapping.Mapping CreateMapping<T>()
		{
			return CreateMapping<T, T>();
		}

		private Mapping.Mapping CreateMapping<TFrom, TTo>()
		{
			var fromPocoModel = TypeModel.GetModelOf<TFrom>();
			var toPocoModel = TypeModel.GetModelOf<TTo>();

			var builder = new MappingBuilder(fromPocoModel, toPocoModel);
			builder.AddConvention(InflateSameTypes.Instance);
			return builder.BuildMapping();
		}

		private class SourceRoot
		{
			public SourceData Data { get; set; }
		}

		private class SourceData
		{
			public int Property { get; set; }
		}

		public class FlatData
		{
			public int DataProperty { get; set; }
		}
	}
}