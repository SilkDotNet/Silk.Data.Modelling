﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Conventions;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class SubMappingTests
	{
		[TestMethod]
		public async Task MapSubModelToSubView()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView(new MapReferenceTypesConvention());
			var modelInstance = new Model
			{
				Item1 = new SubModel
				{
					Value = 5
				},
				Item2 = new SubModel
				{
					Value = 10
				}
			};
			var viewInstance = await view.MapToViewAsync(modelInstance)
				.ConfigureAwait(false);
			Assert.IsNotNull(viewInstance);
			Assert.IsNotNull(viewInstance.Item1);
			Assert.IsNotNull(viewInstance.Item2);
			Assert.AreEqual(modelInstance.Item1.Value, viewInstance.Item1.Value);
			Assert.AreEqual(modelInstance.Item2.Value, viewInstance.Item2.Value);
		}

		private class Model
		{
			public SubModel Item1 { get; set; }
			public SubModel Item2 { get; set; }
		}

		private class SubModel
		{
			public int Value { get; set; }
		}

		private class View
		{
			public SubView Item1 { get; set; }
			public SubView Item2 { get; set; }
		}

		private class SubView
		{
			public int Value { get; set; }
		}
	}
}
