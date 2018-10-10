// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntitiesTest.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntitiesTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating.Tests
{
    using System.IO;
    using System.Linq;

    using Gorba.Center.Media.T4Templating;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Defines the tests for the generation of data view models.
    /// </summary>
    [DeploymentItem(@"DataViewModels.xml")]
    [DeploymentItem(@"Entities.xml")]
    [TestClass]
    public class EntitiesTest
    {
        /// <summary>
        /// Gets or sets the test context.
        /// </summary>
        /// <value>
        /// The test context.
        /// </value>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Default test.
        /// </summary>
        [TestMethod]
        public void DefaultFileTest()
        {
            var entitiesFilePath = Path.Combine(this.TestContext.TestDeploymentDir, "Entities.xml");
            var exists = File.Exists(entitiesFilePath);
            Assert.IsTrue(exists, "The Entities.xml file was not correctly deployed");
            var dataViewModelsFilePath = Path.Combine(this.TestContext.TestDeploymentDir, "DataViewModels.xml");
            exists = File.Exists(dataViewModelsFilePath);
            Assert.IsTrue(exists, "The DataViewModels.xml file was not correctly deployed");

            var entities = DataViewModelTemplatedEntities.Load(entitiesFilePath, dataViewModelsFilePath);
            Assert.AreEqual(5, entities.NamespaceEntityDescriptors.Count);

            TestLayoutNamespace(entities);
            TestPresentationNamespace(entities);
            TestPresentationCycleNamespace(entities);
            TestEvalNamespace(entities);
        }

        private static void TestLayoutNamespace(DataViewModelTemplatedEntities entities)
        {
            var layout = entities.NamespaceEntityDescriptors.SingleOrDefault(descriptor => descriptor.Name == "Layout");
            Assert.IsNotNull(layout, "Layout namespace not found");

            var baseElement =
                layout.DataViewModelEntityDescriptors.SingleOrDefault(descriptor => descriptor.Name == "Base");
            Assert.IsNotNull(baseElement);
            Assert.AreSame(layout, baseElement.ParentObject);

            Assert.AreEqual("Base", baseElement.Name);
            Assert.AreEqual(
                "LayoutElementDataViewModelBase",
                baseElement.ViewModelName,
                "Base element is not handled as DataViewModelBase");
            var graphicalBaseElement =
                layout.DataViewModelEntityDescriptors.SingleOrDefault(descriptor => descriptor.Name == "GraphicalBase");
            Assert.IsNotNull(graphicalBaseElement);
            Assert.AreSame(layout, graphicalBaseElement.ParentObject);

            Assert.AreEqual("GraphicalBase", graphicalBaseElement.Name);
            Assert.AreEqual("GraphicalElementBase", graphicalBaseElement.EntityName);
            Assert.AreEqual("GraphicalElementDataViewModelBase", graphicalBaseElement.ViewModelName);
            Assert.AreEqual("DataViewModelBase", graphicalBaseElement.BaseDataViewModel.ViewModelName);
            Assert.AreEqual("GraphicalElementDataViewModelBaseConverter", graphicalBaseElement.ConverterName);
            Assert.AreEqual("DataViewModelConverterBase", graphicalBaseElement.BaseConverterName);
            var xaxisProperty = graphicalBaseElement.PropertyDescriptors.SingleOrDefault(
                @base => @base.Name == "X");
            Assert.IsNotNull(xaxisProperty, "X property not found on GraphicalBase element");
            Assert.AreEqual("Layout", xaxisProperty.UserVisibleGroupName);

            var drawableBaseElement =
                layout.DataViewModelEntityDescriptors.SingleOrDefault(descriptor => descriptor.Name == "DrawableBase");
            Assert.IsNotNull(drawableBaseElement);
            Assert.AreSame(layout, drawableBaseElement.ParentObject);

            Assert.AreEqual("DrawableBase", drawableBaseElement.Name);
            Assert.AreEqual("DrawableElementBase", drawableBaseElement.EntityName);
            Assert.AreEqual("DrawableElementDataViewModelBase", drawableBaseElement.ViewModelName);
            Assert.AreEqual("GraphicalElementDataViewModelBase", drawableBaseElement.BaseDataViewModel.ViewModelName);
            Assert.AreEqual("DrawableElementDataViewModelBaseConverter", drawableBaseElement.ConverterName);
            Assert.AreEqual("GraphicalElementDataViewModelBaseConverter", drawableBaseElement.BaseConverterName);
            var indexProperty = drawableBaseElement.PropertyDescriptors.SingleOrDefault(
                @base => @base.Name == "ZIndex");
            Assert.IsNotNull(indexProperty, "ZIndex property not found on DrawableBase element");
            Assert.IsTrue(indexProperty.IsHidden);
            Assert.AreEqual("Layers", indexProperty.UserVisibleGroupName);
            Assert.AreEqual("Z-Index", indexProperty.UserVisibleFieldName);

            var fontElement =
                layout.DataViewModelEntityDescriptors.SingleOrDefault(descriptor => descriptor.Name == "Font");
            Assert.IsNotNull(fontElement);
            Assert.AreSame(layout, fontElement.ParentObject);

            Assert.AreEqual("Font", fontElement.Name);
            Assert.AreEqual("Font", fontElement.EntityName);
            Assert.AreEqual("FontDataViewModel", fontElement.ViewModelName);
            Assert.AreEqual("DataViewModelBase", fontElement.BaseDataViewModel.ViewModelName);
            Assert.AreEqual("FontDataViewModelConverter", fontElement.ConverterName);
            Assert.AreEqual("DataViewModelConverterBase", fontElement.BaseConverterName);
            var isNullOrEmptyFontBase = string.IsNullOrEmpty(fontElement.Base);
            Assert.IsTrue(isNullOrEmptyFontBase);
            Assert.IsFalse(fontElement.IsAbstract);
            Assert.IsTrue(fontElement.IsRoot);

            Assert.AreEqual(7, fontElement.PropertyDescriptors.Count);
        }

        private static void TestPresentationNamespace(DataViewModelTemplatedEntities entities)
        {
            var presentation =
                entities.NamespaceEntityDescriptors.SingleOrDefault(descriptor => descriptor.Name == "Presentation");
            Assert.IsNotNull(presentation, "Presentation namespace not found");

            var fontElement =
                presentation.DataViewModelEntityDescriptors.SingleOrDefault(descriptor => descriptor.Name == "Font");
            Assert.IsNotNull(fontElement);
            Assert.AreSame(presentation, fontElement.ParentObject);

            Assert.AreEqual("Font", fontElement.Name);
            Assert.AreEqual("FontConfig", fontElement.EntityName);
            Assert.AreEqual("FontConfigDataViewModel", fontElement.ViewModelName);
            var isNullOrEmptyFontBase = string.IsNullOrEmpty(fontElement.Base);
            Assert.IsTrue(isNullOrEmptyFontBase);
            Assert.IsFalse(fontElement.IsAbstract);

            var evaluationElement =
                presentation.DataViewModelEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == "Evaluation");
            Assert.IsNotNull(evaluationElement);
            Assert.AreSame(presentation, evaluationElement.ParentObject);

            Assert.AreEqual("Evaluation", evaluationElement.Name);
            Assert.AreEqual("EvaluationConfig", evaluationElement.EntityName);
            Assert.AreEqual("EvaluationConfigDataViewModel", evaluationElement.ViewModelName);
            Assert.AreEqual(
                "Gorba.Center.Media.Core.DataViewModels.Presentation.EvaluationConfigDataViewModel",
                evaluationElement.FullQualifiedViewModelName);
            Assert.AreEqual("ContainerEvalDataViewModelBase", evaluationElement.BaseDataViewModel.ViewModelName);

            var masterPresentationElement =
                presentation.DataViewModelEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == "MasterPresentation");
            Assert.IsNotNull(masterPresentationElement);
            Assert.AreSame(presentation, masterPresentationElement.ParentObject);

            Assert.AreEqual("MasterPresentation", masterPresentationElement.Name);
            Assert.AreEqual("MasterPresentationConfig", masterPresentationElement.EntityName);
            Assert.AreEqual("MasterPresentationConfigDataViewModel", masterPresentationElement.ViewModelName);
            Assert.AreEqual("DataViewModelBase", masterPresentationElement.BaseDataViewModel.ViewModelName);

            var masterCyclesProperty =
                masterPresentationElement.PropertyDescriptors.SingleOrDefault(@base => @base.Name == "MasterCycles") as
                ListProperty;
            Assert.IsNotNull(masterCyclesProperty);
            Assert.IsNotNull(masterCyclesProperty.ElementTypeDataViewModel);
            Assert.AreEqual(
                "MasterCycleConfigDataViewModel", masterCyclesProperty.ElementTypeDataViewModel.ViewModelName);

            var resolutionElement =
                presentation.DataViewModelEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == "Resolution");
            Assert.IsNotNull(resolutionElement);
            Assert.AreSame(presentation, resolutionElement.ParentObject);

            Assert.AreEqual("Resolution", resolutionElement.Name);
            Assert.AreEqual("ResolutionConfig", resolutionElement.EntityName);
            Assert.AreEqual("ResolutionConfigDataViewModel", resolutionElement.ViewModelName);
            Assert.AreEqual("DataViewModelBase", resolutionElement.BaseDataViewModel.ViewModelName);
            Assert.AreEqual(
                "Gorba.Center.Common.Wpf.Framework.DataViewModels.DataViewModelBase",
                resolutionElement.BaseDataViewModel.FullQualifiedViewModelName);

            var elementsProperty =
                resolutionElement.PropertyDescriptors.SingleOrDefault(@base => @base.Name == "Elements") as
                ListProperty;
            Assert.IsNotNull(elementsProperty);
            Assert.IsNotNull(elementsProperty.ElementTypeDataViewModel);
            Assert.AreEqual(
                "LayoutElementDataViewModelBase", elementsProperty.ElementTypeDataViewModel.ViewModelName);

            TestPhysicalScreenRefElement(presentation);

            TestVirtualDisplayRefElement(presentation);
        }

        private static void TestPhysicalScreenRefElement(NamespaceEntityDescriptor presentation)
        {
            var physicalScreenRefElement =
                presentation.DataViewModelEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == "PhysicalScreenRef");
            Assert.IsNotNull(physicalScreenRefElement);
            Assert.AreSame(presentation, physicalScreenRefElement.ParentObject);

            Assert.AreEqual("PhysicalScreenRef", physicalScreenRefElement.Name);
            Assert.AreEqual("PhysicalScreenRefConfig", physicalScreenRefElement.EntityName);
            Assert.AreEqual("PhysicalScreenRefConfigDataViewModel", physicalScreenRefElement.ViewModelName);
            Assert.AreEqual("DataViewModelBase", physicalScreenRefElement.BaseDataViewModel.ViewModelName);
            Assert.AreEqual("DataViewModelConverterBase", physicalScreenRefElement.BaseConverterName);
            Assert.IsTrue(physicalScreenRefElement.IsRoot);
            Assert.IsTrue(physicalScreenRefElement.IsReference);
        }

        private static void TestVirtualDisplayRefElement(NamespaceEntityDescriptor presentation)
        {
            var virtualDisplayRefElement =
                presentation.DataViewModelEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == "VirtualDisplayRef");
            Assert.IsNotNull(virtualDisplayRefElement);
            Assert.AreSame(presentation, virtualDisplayRefElement.ParentObject);

            Assert.AreEqual("VirtualDisplayRef", virtualDisplayRefElement.Name);
            Assert.AreEqual("VirtualDisplayRefConfig", virtualDisplayRefElement.EntityName);
            Assert.AreEqual("VirtualDisplayRefConfigDataViewModel", virtualDisplayRefElement.ViewModelName);
            Assert.AreEqual(
                "DrawableElementDataViewModelBase", virtualDisplayRefElement.BaseDataViewModel.ViewModelName);
            Assert.AreEqual(
                "Gorba.Center.Media.Core.DataViewModels.Layout.DrawableElementDataViewModelBase",
                virtualDisplayRefElement.BaseDataViewModel.FullQualifiedViewModelName);
            Assert.IsFalse(virtualDisplayRefElement.IsRoot);
            Assert.IsTrue(virtualDisplayRefElement.IsReference);

            var referenceProperty =
                virtualDisplayRefElement.PropertyDescriptors.OfType<ReferenceProperty>().SingleOrDefault();
            if (referenceProperty == null)
            {
                throw new InvalidDataException("Can' find reference property for VirtualDisplayRef");
            }

            Assert.AreEqual("VirtualDisplayConfigDataViewModel", referenceProperty.ReferencedType.ViewModelName);
        }

        private static void TestPresentationCycleNamespace(DataViewModelTemplatedEntities entities)
        {
            var presentationCycle =
                entities.NamespaceEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == "Presentation.Cycle");
            Assert.IsNotNull(presentationCycle, "Presentation.Cycle namespace not found");

            var genericTrigger =
                presentationCycle.DataViewModelEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == "GenericTrigger");
            Assert.IsNotNull(genericTrigger);
            Assert.AreSame(presentationCycle, genericTrigger.ParentObject);
            Assert.AreEqual("GenericTriggerConfig", genericTrigger.EntityName);
            Assert.AreEqual("GenericTriggerConfigDataViewModel", genericTrigger.ViewModelName);
        }

        private static void TestEvalNamespace(DataViewModelTemplatedEntities entities)
        {
            var eval =
                entities.NamespaceEntityDescriptors.SingleOrDefault(descriptor => descriptor.Name == "Eval");
            Assert.IsNotNull(eval, "Eval namespace not found");

            var containerBase =
                eval.DataViewModelEntityDescriptors.SingleOrDefault(
                    descriptor => descriptor.Name == "ContainerBase");
            Assert.IsNotNull(containerBase);
            Assert.AreSame(eval, containerBase.ParentObject);
            Assert.AreEqual("ContainerEvalBase", containerBase.EntityName);
            Assert.AreEqual("ContainerEvalDataViewModelBase", containerBase.ViewModelName);

            var evaluationProperty =
                containerBase.PropertyDescriptors.SingleOrDefault(@base => @base.Name == "Evaluation") as
                CompositeProperty;
            Assert.IsNotNull(evaluationProperty);
            Assert.AreEqual("EvalDataViewModelBase", evaluationProperty.TypeDataViewModel.ViewModelName);

            Assert.AreEqual(30, eval.FilteredDataViewModelEntityDescriptors.Count());
        }
    }
}