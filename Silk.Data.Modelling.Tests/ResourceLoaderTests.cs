﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Silk.Data.Modelling.Bindings;
using Silk.Data.Modelling.Conventions;
using Silk.Data.Modelling.ResourceLoaders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silk.Data.Modelling.Tests
{
	[TestClass]
	public class ResourceLoaderTests
	{
		[TestMethod]
		public async Task CustomResourceLoaderWorks()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView(CustomViewBuilder.Create, new SubObjectSupport());

			var viewInstance = new View
			{
				Object1Value = 10,
				Object2Value = 50
			};
			var modelInstance = await view.MapToModelAsync(viewInstance)
				.ConfigureAwait(false);

			Assert.IsNotNull(modelInstance);
			Assert.IsNotNull(modelInstance.Object1);
			Assert.IsNotNull(modelInstance.Object2);
			Assert.AreEqual(viewInstance.Object1Value, modelInstance.Object1.Value);
			Assert.AreEqual(viewInstance.Object2Value, modelInstance.Object2.Value);
		}

		[TestMethod]
		public async Task ResourceLoaderSupportsEnumerable()
		{
			var model = TypeModeller.GetModelOf<Model>();
			var view = model.GetModeller<View>().CreateTypedView(CustomViewBuilder.Create, new SubObjectSupport());

			var viewInstances = new View[]
			{
				new View
				{
					Object1Value = 10,
					Object2Value = 50
				},
				new View
				{
					Object1Value = 10,
					Object2Value = 50
				},
				new View
				{
					Object1Value = 15,
					Object2Value = 25
				}
			};
			SubObjectResourceLoader.RunCount = 0;
			var modelInstances = await view.MapToModelAsync(viewInstances);
			Assert.AreEqual(1, SubObjectResourceLoader.RunCount);
			for (var i = 0; i < modelInstances.Length; i++)
			{
				Assert.IsNotNull(modelInstances[i].Object1);
				Assert.IsNotNull(modelInstances[i].Object2);
				Assert.AreEqual(viewInstances[i].Object1Value, modelInstances[i].Object1.Value);
				Assert.AreEqual(viewInstances[i].Object2Value, modelInstances[i].Object2.Value);
			}
		}

		private class SubObject
		{
			public int Value { get; }

			public SubObject(int value)
			{
				Value = value;
			}
		}

		private class Model
		{
			public SubObject Object1 { get; set; }
			public SubObject Object2 { get; set; }
		}

		private class View
		{
			public int Object1Value { get; set; }
			public int Object2Value { get; set; }
		}

		private class CustomViewBuilder : ViewBuilder
		{
			protected CustomViewBuilder(Modelling.Model sourceModel, Modelling.Model targetModel, ViewConvention[] viewConventions) : base(sourceModel, targetModel, viewConventions)
			{
			}

			public new static CustomViewBuilder Create(Modelling.Model sourceModel, Modelling.Model targetModel,
				ViewConvention[] viewConventions)
			{
				return new CustomViewBuilder(sourceModel, targetModel, viewConventions);
			}
		}

		private class SubObjectSupport : ViewConvention<CustomViewBuilder>
		{
			public override ViewType SupportedViewTypes => ViewType.All;
			public override bool PerformMultiplePasses => false;
			public override bool SkipIfFieldDefined => true;

			public override void MakeModelField(CustomViewBuilder viewBuilder, ModelField field)
			{
				var subObjectLoader = viewBuilder.ViewDefinition.ResourceLoaders
					.OfType<SubObjectResourceLoader>().FirstOrDefault();
				if (subObjectLoader == null)
				{
					subObjectLoader = new SubObjectResourceLoader();
					viewBuilder.ViewDefinition.ResourceLoaders.Add(subObjectLoader);
				}

				var fieldName = field.Name.Replace("Value", "");
				var bindField = viewBuilder.FindField(field, fieldName);

				viewBuilder.ViewDefinition.FieldDefinitions.Add(new ViewFieldDefinition(field.Name,
					new SubObjectBinding(new[] { bindField.Field.Name }, new[] { field.Name }))
				{
					DataType = field.DataType
				});
				subObjectLoader.AddField(field.Name);
			}
		}

		private class SubObjectBinding : ModelBinding
		{
			public override BindingDirection Direction => BindingDirection.ViewToModel;

			public SubObjectBinding(string[] modelFieldPath, string[] viewFieldPath)
				: base(modelFieldPath, viewFieldPath)
			{
			}

			public override void CopyBindingValue(IContainerReadWriter from, IContainerReadWriter to, MappingContext mappingContext)
			{
				var value = ReadValue<int>(from);
				WriteValue(
					to,
					mappingContext.Resources.Retrieve($"subObject:{value}") as SubObject
					);
			}
		}

		private class SubObjectResourceLoader : IResourceLoader
		{
			private readonly List<string> _fieldNames = new List<string>();

			public static int RunCount { get; set; }

			public void AddField(string fieldName)
			{
				_fieldNames.Add(fieldName);
			}

			public Task LoadResourcesAsync(IView view, ICollection<IContainerReadWriter> sources, MappingContext mappingContext)
			{
				RunCount++;
				var builtObjects = new List<int>();
				var fields = view.Fields.Where(q => _fieldNames.Contains(q.Name)).ToArray();
				foreach (var container in sources)
				{
					foreach (var field in fields)
					{
						var value = field.ModelBinding.ReadValue<int>(container);
						if (!builtObjects.Contains(value))
						{
							mappingContext.Resources.Store($"subObject:{value}", new SubObject(value));
							builtObjects.Add(value);
						}
					}
				}
				return Task.CompletedTask;
			}
		}
	}
}
