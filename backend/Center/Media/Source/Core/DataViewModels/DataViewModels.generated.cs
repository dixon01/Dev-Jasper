namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Presentation.Cycle;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Media.Core.Controllers;
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.ComponentModel;
    using System.Linq;
    public partial class FontDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> face;

        private DataValue<int> height;

        private DataValue<int> weight;

        private DataValue<bool> italic;

        private DataValue<string> color;

        private DataValue<string> outlinecolor;

        private DataValue<int> charspacing;

        public FontDataViewModel(IMediaShell mediaShell, FontDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Face = new DataValue<string>(string.Empty);
            this.Face.PropertyChanged += this.FaceChanged;
            this.Height = new DataValue<int>(default(int));
            this.Height.PropertyChanged += this.HeightChanged;
            this.Weight = new DataValue<int>(default(int));
            this.Weight.PropertyChanged += this.WeightChanged;
            this.Italic = new DataValue<bool>(default(bool));
            this.Italic.PropertyChanged += this.ItalicChanged;
            this.Color = new DataValue<string>(string.Empty);
            this.Color.PropertyChanged += this.ColorChanged;
            this.OutlineColor = new DataValue<string>(string.Empty);
            this.OutlineColor.PropertyChanged += this.OutlineColorChanged;
            this.CharSpacing = new DataValue<int>(1);
            this.CharSpacing.PropertyChanged += this.CharSpacingChanged;
            if (dataModel != null)
            {
                this.Face.Value = dataModel.Face;
                this.Height.Value = dataModel.Height;
                this.Weight.Value = dataModel.Weight;
                this.Italic.Value = dataModel.Italic;
                this.Color.Value = dataModel.Color;
                this.OutlineColor.Value = dataModel.OutlineColor;
                this.CharSpacing.Value = dataModel.CharSpacing;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected FontDataViewModel(IMediaShell mediaShell, FontDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Face = (DataValue<string>)dataViewModel.Face.Clone();
            this.Face.PropertyChanged += this.FaceChanged;
            this.Height = (DataValue<int>)dataViewModel.Height.Clone();
            this.Height.PropertyChanged += this.HeightChanged;
            this.Weight = (DataValue<int>)dataViewModel.Weight.Clone();
            this.Weight.PropertyChanged += this.WeightChanged;
            this.Italic = (DataValue<bool>)dataViewModel.Italic.Clone();
            this.Italic.PropertyChanged += this.ItalicChanged;
            this.Color = (DataValue<string>)dataViewModel.Color.Clone();
            this.Color.PropertyChanged += this.ColorChanged;
            this.OutlineColor = (DataValue<string>)dataViewModel.OutlineColor.Clone();
            this.OutlineColor.PropertyChanged += this.OutlineColorChanged;
            this.CharSpacing = (DataValue<int>)dataViewModel.CharSpacing.Clone();
            this.CharSpacing.PropertyChanged += this.CharSpacingChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Face.IsDirty || this.Height.IsDirty || this.Weight.IsDirty || this.Italic.IsDirty || this.Color.IsDirty || this.OutlineColor.IsDirty || this.CharSpacing.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Face
        {
            get
            {
                return this.face;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.face);
                if (this.face != null)
                {
                    this.face.PropertyChanged -= this.FaceChanged;
                }

                this.SetProperty(ref this.face, value, () => this.Face);
                if (value != null)
                {
                    this.face.PropertyChanged += this.FaceChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 1, GroupOrderIndex = 0)]
        public DataValue<int> Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.height);
                if (this.height != null)
                {
                    this.height.PropertyChanged -= this.HeightChanged;
                }

                this.SetProperty(ref this.height, value, () => this.Height);
                if (value != null)
                {
                    this.height.PropertyChanged += this.HeightChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 2, GroupOrderIndex = 0)]
        public DataValue<int> Weight
        {
            get
            {
                return this.weight;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.weight);
                if (this.weight != null)
                {
                    this.weight.PropertyChanged -= this.WeightChanged;
                }

                this.SetProperty(ref this.weight, value, () => this.Weight);
                if (value != null)
                {
                    this.weight.PropertyChanged += this.WeightChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 3, GroupOrderIndex = 0)]
        public DataValue<bool> Italic
        {
            get
            {
                return this.italic;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.italic);
                if (this.italic != null)
                {
                    this.italic.PropertyChanged -= this.ItalicChanged;
                }

                this.SetProperty(ref this.italic, value, () => this.Italic);
                if (value != null)
                {
                    this.italic.PropertyChanged += this.ItalicChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 4, GroupOrderIndex = 0)]
        public DataValue<string> Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.color);
                if (this.color != null)
                {
                    this.color.PropertyChanged -= this.ColorChanged;
                }

                this.SetProperty(ref this.color, value, () => this.Color);
                if (value != null)
                {
                    this.color.PropertyChanged += this.ColorChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "LED", OrderIndex = 5, GroupOrderIndex = 0)]
        public DataValue<string> OutlineColor
        {
            get
            {
                return this.outlinecolor;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.outlinecolor);
                if (this.outlinecolor != null)
                {
                    this.outlinecolor.PropertyChanged -= this.OutlineColorChanged;
                }

                this.SetProperty(ref this.outlinecolor, value, () => this.OutlineColor);
                if (value != null)
                {
                    this.outlinecolor.PropertyChanged += this.OutlineColorChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "LED", OrderIndex = 6, GroupOrderIndex = 0)]
        public DataValue<int> CharSpacing
        {
            get
            {
                return this.charspacing;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.charspacing);
                if (this.charspacing != null)
                {
                    this.charspacing.PropertyChanged -= this.CharSpacingChanged;
                }

                this.SetProperty(ref this.charspacing, value, () => this.CharSpacing);
                if (value != null)
                {
                    this.charspacing.PropertyChanged += this.CharSpacingChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public Font Export(object parameters = null)
        {
            var font = (Font)this.CreateExportObject();
            this.DoExport(font, parameters);
            return font;
        }

        public FontDataModel ToDataModel()
        {
            var font = (FontDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(font);
            return font;
        }

        public override void ClearDirty()
        {
            if (this.Face != null)
            {
                this.Face.ClearDirty();
            }

            if (this.Height != null)
            {
                this.Height.ClearDirty();
            }

            if (this.Weight != null)
            {
                this.Weight.ClearDirty();
            }

            if (this.Italic != null)
            {
                this.Italic.ClearDirty();
            }

            if (this.Color != null)
            {
                this.Color.ClearDirty();
            }

            if (this.OutlineColor != null)
            {
                this.OutlineColor.ClearDirty();
            }

            if (this.CharSpacing != null)
            {
                this.CharSpacing.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new FontDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is FontDataViewModel)
            {
                var that = (FontDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Face.EqualsValue(that.Face);
                        result = result && this.Height.EqualsValue(that.Height);
                        result = result && this.Weight.EqualsValue(that.Weight);
                        result = result && this.Italic.EqualsValue(that.Italic);
                        result = result && this.Color.EqualsValue(that.Color);
                        result = result && this.OutlineColor.EqualsValue(that.OutlineColor);
                        result = result && this.CharSpacing.EqualsValue(that.CharSpacing);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new Font();
        }

        protected object CreateDataModelObject()
        {
            return new FontDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (Font)exportModel;
            model.Face = this.Face.Value;
            model.Height = this.Height.Value;
            model.Weight = this.Weight.Value;
            model.Italic = this.Italic.Value;
            model.Color = this.Color.Value;
            model.OutlineColor = this.OutlineColor.Value;
            model.CharSpacing = this.CharSpacing.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (FontDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Face = this.Face.Value;
            model.Height = this.Height.Value;
            model.Weight = this.Weight.Value;
            model.Italic = this.Italic.Value;
            model.Color = this.Color.Value;
            model.OutlineColor = this.OutlineColor.Value;
            model.CharSpacing = this.CharSpacing.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FaceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Face);
        }

        private void HeightChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Height);
        }

        private void WeightChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Weight);
        }

        private void ItalicChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Italic);
        }

        private void ColorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Color);
        }

        private void OutlineColorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.OutlineColor);
        }

        private void CharSpacingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.CharSpacing);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(FontDataModel dataModel = null);

        partial void Initialize(FontDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(Font model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref FontDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class GraphicalElementDataViewModelBase : LayoutElementDataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> x;

        private DataValue<int> y;

        private DataValue<int> width;

        private DataValue<int> height;

        private AnimatedDynamicDataValue<bool> visible;

        public GraphicalElementDataViewModelBase(IMediaShell mediaShell, GraphicalElementDataModelBase dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.X = new DataValue<int>(default(int));
            this.X.PropertyChanged += this.XChanged;
            this.Y = new DataValue<int>(default(int));
            this.Y.PropertyChanged += this.YChanged;
            this.Width = new DataValue<int>(-1);
            this.Width.PropertyChanged += this.WidthChanged;
            this.Height = new DataValue<int>(-1);
            this.Height.PropertyChanged += this.HeightChanged;
            this.Visible = new AnimatedDynamicDataValue<bool>(true);
            this.Visible.PropertyChanged += this.VisibleChanged;
            if (dataModel != null)
            {
                this.X.Value = dataModel.X;
                this.Y.Value = dataModel.Y;
                this.Width.Value = dataModel.Width;
                this.Height.Value = dataModel.Height;
                if (dataModel.Visible != null)
                {
                    this.Visible.Value = dataModel.Visible;
                }

                if (dataModel.VisibleProperty != null)
                {
                    this.Visible.Formula = this.CreateEvalDataViewModel(dataModel.VisibleProperty.Evaluation);
                    if (dataModel.VisibleProperty.Animation != null)
                    {
                        this.Visible.Animation = new AnimationDataViewModel(this.mediaShell, dataModel.VisibleProperty.Animation);
                    }

                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected GraphicalElementDataViewModelBase(IMediaShell mediaShell, GraphicalElementDataViewModelBase dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.X = (DataValue<int>)dataViewModel.X.Clone();
            this.X.PropertyChanged += this.XChanged;
            this.Y = (DataValue<int>)dataViewModel.Y.Clone();
            this.Y.PropertyChanged += this.YChanged;
            this.Width = (DataValue<int>)dataViewModel.Width.Clone();
            this.Width.PropertyChanged += this.WidthChanged;
            this.Height = (DataValue<int>)dataViewModel.Height.Clone();
            this.Height.PropertyChanged += this.HeightChanged;
            this.Visible = (AnimatedDynamicDataValue<bool>)dataViewModel.Visible.Clone();
            this.Visible.PropertyChanged += this.VisibleChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.X.IsDirty || this.Y.IsDirty || this.Width.IsDirty || this.Height.IsDirty || this.Visible.IsDirty;
            }
        }

        [UserVisibleProperty("Layout", OrderIndex = 4, GroupOrderIndex = 0)]
        public DataValue<int> X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.x);
                if (this.x != null)
                {
                    this.x.PropertyChanged -= this.XChanged;
                }

                this.SetProperty(ref this.x, value, () => this.X);
                if (value != null)
                {
                    this.x.PropertyChanged += this.XChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Layout", OrderIndex = 5, GroupOrderIndex = 0)]
        public DataValue<int> Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.y);
                if (this.y != null)
                {
                    this.y.PropertyChanged -= this.YChanged;
                }

                this.SetProperty(ref this.y, value, () => this.Y);
                if (value != null)
                {
                    this.y.PropertyChanged += this.YChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Layout", OrderIndex = 2, GroupOrderIndex = 0)]
        public DataValue<int> Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.width);
                if (this.width != null)
                {
                    this.width.PropertyChanged -= this.WidthChanged;
                }

                this.SetProperty(ref this.width, value, () => this.Width);
                if (value != null)
                {
                    this.width.PropertyChanged += this.WidthChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Layout", OrderIndex = 3, GroupOrderIndex = 0)]
        public DataValue<int> Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.height);
                if (this.height != null)
                {
                    this.height.PropertyChanged -= this.HeightChanged;
                }

                this.SetProperty(ref this.height, value, () => this.Height);
                if (value != null)
                {
                    this.height.PropertyChanged += this.HeightChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Layout", OrderIndex = 1, GroupOrderIndex = 0)]
        public AnimatedDynamicDataValue<bool> Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.visible);
                if (this.visible != null)
                {
                    this.visible.PropertyChanged -= this.VisibleChanged;
                }

                this.SetProperty(ref this.visible, value, () => this.Visible);
                if (value != null)
                {
                    this.visible.PropertyChanged += this.VisibleChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public override ElementBase Export(object parameters = null)
        {
            var graphicalbase = (GraphicalElementBase)this.CreateExportObject();
            this.DoExport(graphicalbase, parameters);
            return graphicalbase;
        }

        public override LayoutElementDataModelBase ToDataModel()
        {
            var graphicalbase = (GraphicalElementDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(graphicalbase);
            return graphicalbase;
        }

        public override void ClearDirty()
        {
            if (this.X != null)
            {
                this.X.ClearDirty();
            }

            if (this.Y != null)
            {
                this.Y.ClearDirty();
            }

            if (this.Width != null)
            {
                this.Width.ClearDirty();
            }

            if (this.Height != null)
            {
                this.Height.ClearDirty();
            }

            if (this.Visible != null)
            {
                this.Visible.ClearDirty();
            }

            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is GraphicalElementDataViewModelBase)
            {
                var that = (GraphicalElementDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.X.EqualsValue(that.X);
                        result = result && this.Y.EqualsValue(that.Y);
                        result = result && this.Width.EqualsValue(that.Width);
                        result = result && this.Height.EqualsValue(that.Height);
                        result = result && this.Visible.EqualsValue(that.Visible);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected abstract object CreateExportObject();

        protected abstract object CreateDataModelObject();

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (GraphicalElementBase)exportModel;
            model.X = this.X.Value;
            model.Y = this.Y.Value;
            model.Width = this.Width.Value;
            model.Height = this.Height.Value;
            if (this.Visible.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Visible.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var visibleEval = Visible.Formula as CodeConversionEvalDataViewModel;
                    if (visibleEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = visibleEval.UseImage.Value ? "{2}" : "{3}";
                            model.VisibleProperty = new AnimatedDynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.VisibleProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Visible.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.VisibleProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Visible.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Visible = this.Visible.Value;
                }
            }
            else
            {
                model.Visible = this.Visible.Value;
            }

            if (this.Visible.Animation != null && this.Visible.Formula != null)
            {
                if (model.VisibleProperty == null)
                {
                    model.VisibleProperty = new AnimatedDynamicProperty();
                }

                model.VisibleProperty.Animation = new PropertyChangeAnimation {
                    Duration = ((AnimationDataViewModel)this.Visible.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Visible.Animation).Type.Value
                };
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (GraphicalElementDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            model.X = this.X.Value;
            model.Y = this.Y.Value;
            model.Width = this.Width.Value;
            model.Height = this.Height.Value;
            model.Visible = this.Visible.Value;
            if (this.Visible.Formula != null)
            {
                model.VisibleProperty = new AnimatedDynamicPropertyDataModel(((EvalDataViewModelBase)this.Visible.Formula).ToDataModel());
            }

            if (this.Visible.Animation != null)
            {
                if (model.VisibleProperty == null)
                {
                    model.VisibleProperty = new AnimatedDynamicPropertyDataModel();
                }

                model.VisibleProperty.Animation = new PropertyChangeAnimationDataModel {
                    Duration = ((AnimationDataViewModel)this.Visible.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Visible.Animation).Type.Value
                };
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void XChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.X);
        }

        private void YChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Y);
        }

        private void WidthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Width);
        }

        private void HeightChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Height);
        }

        private void VisibleChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Visible);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(GraphicalElementDataModelBase dataModel = null);

        partial void Initialize(GraphicalElementDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(GraphicalElementBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref GraphicalElementDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class DrawableElementDataViewModelBase : GraphicalElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> zindex;

        public DrawableElementDataViewModelBase(IMediaShell mediaShell, DrawableElementDataModelBase dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.ZIndex = new DataValue<int>(default(int));
            this.ZIndex.PropertyChanged += this.ZIndexChanged;
            if (dataModel != null)
            {
                this.ZIndex.Value = dataModel.ZIndex;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected DrawableElementDataViewModelBase(IMediaShell mediaShell, DrawableElementDataViewModelBase dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.ZIndex = (DataValue<int>)dataViewModel.ZIndex.Clone();
            this.ZIndex.PropertyChanged += this.ZIndexChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.ZIndex.IsDirty;
            }
        }

        public DataValue<int> ZIndex
        {
            get
            {
                return this.zindex;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.zindex);
                if (this.zindex != null)
                {
                    this.zindex.PropertyChanged -= this.ZIndexChanged;
                }

                this.SetProperty(ref this.zindex, value, () => this.ZIndex);
                if (value != null)
                {
                    this.zindex.PropertyChanged += this.ZIndexChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var drawablebase = (DrawableElementBase)this.CreateExportObject();
            this.DoExport(drawablebase, parameters);
            return drawablebase;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var drawablebase = (DrawableElementDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(drawablebase);
            return drawablebase;
        }

        public override void ClearDirty()
        {
            if (this.ZIndex != null)
            {
                this.ZIndex.ClearDirty();
            }

            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is DrawableElementDataViewModelBase)
            {
                var that = (DrawableElementDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.ZIndex.EqualsValue(that.ZIndex);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (DrawableElementBase)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.ZIndex = this.ZIndex.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (DrawableElementDataModelBase)dataModel;
            base.ConvertToDataModel(model);
            model.ZIndex = this.ZIndex.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ZIndexChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.ZIndex);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(DrawableElementDataModelBase dataModel = null);

        partial void Initialize(DrawableElementDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(DrawableElementBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref DrawableElementDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class AnalogClockElementDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private AnalogClockHandElementDataViewModel hour;

        private AnalogClockHandElementDataViewModel minute;

        private AnalogClockHandElementDataViewModel seconds;

        public AnalogClockElementDataViewModel(IMediaShell mediaShell, AnalogClockElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.hour = new AnalogClockHandElementDataViewModel(this.mediaShell);
            this.minute = new AnalogClockHandElementDataViewModel(this.mediaShell);
            this.seconds = new AnalogClockHandElementDataViewModel(this.mediaShell);
            if (dataModel != null)
            {
                this.hour = new AnalogClockHandElementDataViewModel(this.mediaShell, dataModel.Hour);
                this.minute = new AnalogClockHandElementDataViewModel(this.mediaShell, dataModel.Minute);
                this.seconds = new AnalogClockHandElementDataViewModel(this.mediaShell, dataModel.Seconds);
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected AnalogClockElementDataViewModel(IMediaShell mediaShell, AnalogClockElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            var clonedHour = dataViewModel.Hour;
            if (clonedHour != null)
            {
                this.Hour = (AnalogClockHandElementDataViewModel)clonedHour.Clone();
            }

            var clonedMinute = dataViewModel.Minute;
            if (clonedMinute != null)
            {
                this.Minute = (AnalogClockHandElementDataViewModel)clonedMinute.Clone();
            }

            var clonedSeconds = dataViewModel.Seconds;
            if (clonedSeconds != null)
            {
                this.Seconds = (AnalogClockHandElementDataViewModel)clonedSeconds.Clone();
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || (this.Hour != null && this.Hour.IsDirty) || (this.Minute != null && this.Minute.IsDirty) || (this.Seconds != null && this.Seconds.IsDirty);
            }
        }

        public AnalogClockHandElementDataViewModel Hour
        {
            get
            {
                return this.hour;
            }
            set
            {
                if (this.hour != null)
                {
                    this.hour.PropertyChanged -= this.HourChanged;
                }

                this.SetProperty(ref this.hour, value, () => this.Hour);
                if (value != null)
                {
                    this.hour.PropertyChanged += this.HourChanged;
                }

            }
        }

        [UserVisibleProperty("Hour", FieldName = "Mode", Filter = "TFT", OrderIndex = 8, GroupOrderIndex = 2)]
        public DataValue<AnalogClockHandMode> HourMode
        {
            get
            {
                return Hour.Mode;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "CenterX", Filter = "TFT", OrderIndex = 6, GroupOrderIndex = 2)]
        public DataValue<int> HourCenterX
        {
            get
            {
                return Hour.CenterX;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "CenterY", Filter = "TFT", OrderIndex = 7, GroupOrderIndex = 2)]
        public DataValue<int> HourCenterY
        {
            get
            {
                return Hour.CenterY;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "Filename", OrderIndex = 0, GroupOrderIndex = 2)]
        public AnimatedDynamicDataValue<string> HourFilename
        {
            get
            {
                return Hour.Filename;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "Scaling", Filter = "TFT", OrderIndex = 9, GroupOrderIndex = 2)]
        public DataValue<ElementScaling> HourScaling
        {
            get
            {
                return Hour.Scaling;
            }
            set
            {
                return;
            }
        }

        public DataValue<int> HourZIndex
        {
            get
            {
                return Hour.ZIndex;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "X", OrderIndex = 4, GroupOrderIndex = 2)]
        public DataValue<int> HourX
        {
            get
            {
                return Hour.X;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "Y", OrderIndex = 5, GroupOrderIndex = 2)]
        public DataValue<int> HourY
        {
            get
            {
                return Hour.Y;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "Width", OrderIndex = 2, GroupOrderIndex = 2)]
        public DataValue<int> HourWidth
        {
            get
            {
                return Hour.Width;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "Height", OrderIndex = 3, GroupOrderIndex = 2)]
        public DataValue<int> HourHeight
        {
            get
            {
                return Hour.Height;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Hour", FieldName = "Visible", OrderIndex = 1, GroupOrderIndex = 2)]
        public AnimatedDynamicDataValue<bool> HourVisible
        {
            get
            {
                return Hour.Visible;
            }
            set
            {
                return;
            }
        }

        public AnalogClockHandElementDataViewModel Minute
        {
            get
            {
                return this.minute;
            }
            set
            {
                if (this.minute != null)
                {
                    this.minute.PropertyChanged -= this.MinuteChanged;
                }

                this.SetProperty(ref this.minute, value, () => this.Minute);
                if (value != null)
                {
                    this.minute.PropertyChanged += this.MinuteChanged;
                }

            }
        }

        [UserVisibleProperty("Minute", FieldName = "Mode", Filter = "TFT", OrderIndex = 8, GroupOrderIndex = 3)]
        public DataValue<AnalogClockHandMode> MinuteMode
        {
            get
            {
                return Minute.Mode;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "CenterX", Filter = "TFT", OrderIndex = 6, GroupOrderIndex = 3)]
        public DataValue<int> MinuteCenterX
        {
            get
            {
                return Minute.CenterX;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "CenterY", Filter = "TFT", OrderIndex = 7, GroupOrderIndex = 3)]
        public DataValue<int> MinuteCenterY
        {
            get
            {
                return Minute.CenterY;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "Filename", OrderIndex = 0, GroupOrderIndex = 3)]
        public AnimatedDynamicDataValue<string> MinuteFilename
        {
            get
            {
                return Minute.Filename;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "Scaling", Filter = "TFT", OrderIndex = 9, GroupOrderIndex = 3)]
        public DataValue<ElementScaling> MinuteScaling
        {
            get
            {
                return Minute.Scaling;
            }
            set
            {
                return;
            }
        }

        public DataValue<int> MinuteZIndex
        {
            get
            {
                return Minute.ZIndex;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "X", OrderIndex = 4, GroupOrderIndex = 3)]
        public DataValue<int> MinuteX
        {
            get
            {
                return Minute.X;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "Y", OrderIndex = 5, GroupOrderIndex = 3)]
        public DataValue<int> MinuteY
        {
            get
            {
                return Minute.Y;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "Width", OrderIndex = 2, GroupOrderIndex = 3)]
        public DataValue<int> MinuteWidth
        {
            get
            {
                return Minute.Width;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "Height", OrderIndex = 3, GroupOrderIndex = 3)]
        public DataValue<int> MinuteHeight
        {
            get
            {
                return Minute.Height;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Minute", FieldName = "Visible", OrderIndex = 1, GroupOrderIndex = 3)]
        public AnimatedDynamicDataValue<bool> MinuteVisible
        {
            get
            {
                return Minute.Visible;
            }
            set
            {
                return;
            }
        }

        public AnalogClockHandElementDataViewModel Seconds
        {
            get
            {
                return this.seconds;
            }
            set
            {
                if (this.seconds != null)
                {
                    this.seconds.PropertyChanged -= this.SecondsChanged;
                }

                this.SetProperty(ref this.seconds, value, () => this.Seconds);
                if (value != null)
                {
                    this.seconds.PropertyChanged += this.SecondsChanged;
                }

            }
        }

        [UserVisibleProperty("Seconds", FieldName = "Mode", Filter = "TFT", OrderIndex = 8, GroupOrderIndex = 4)]
        public DataValue<AnalogClockHandMode> SecondsMode
        {
            get
            {
                return Seconds.Mode;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "CenterX", Filter = "TFT", OrderIndex = 6, GroupOrderIndex = 4)]
        public DataValue<int> SecondsCenterX
        {
            get
            {
                return Seconds.CenterX;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "CenterY", Filter = "TFT", OrderIndex = 7, GroupOrderIndex = 4)]
        public DataValue<int> SecondsCenterY
        {
            get
            {
                return Seconds.CenterY;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "Filename", OrderIndex = 0, GroupOrderIndex = 4)]
        public AnimatedDynamicDataValue<string> SecondsFilename
        {
            get
            {
                return Seconds.Filename;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "Scaling", Filter = "TFT", OrderIndex = 9, GroupOrderIndex = 4)]
        public DataValue<ElementScaling> SecondsScaling
        {
            get
            {
                return Seconds.Scaling;
            }
            set
            {
                return;
            }
        }

        public DataValue<int> SecondsZIndex
        {
            get
            {
                return Seconds.ZIndex;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "X", OrderIndex = 4, GroupOrderIndex = 4)]
        public DataValue<int> SecondsX
        {
            get
            {
                return Seconds.X;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "Y", OrderIndex = 5, GroupOrderIndex = 4)]
        public DataValue<int> SecondsY
        {
            get
            {
                return Seconds.Y;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "Width", OrderIndex = 2, GroupOrderIndex = 4)]
        public DataValue<int> SecondsWidth
        {
            get
            {
                return Seconds.Width;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "Height", OrderIndex = 3, GroupOrderIndex = 4)]
        public DataValue<int> SecondsHeight
        {
            get
            {
                return Seconds.Height;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Seconds", FieldName = "Visible", OrderIndex = 1, GroupOrderIndex = 4)]
        public AnimatedDynamicDataValue<bool> SecondsVisible
        {
            get
            {
                return Seconds.Visible;
            }
            set
            {
                return;
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var analogclock = (AnalogClockElement)this.CreateExportObject();
            this.DoExport(analogclock, parameters);
            return analogclock;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var analogclock = (AnalogClockElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(analogclock);
            return analogclock;
        }

        public override void ClearDirty()
        {
            if (this.Hour != null)
            {
                this.Hour.ClearDirty();
            }

            if (this.Minute != null)
            {
                this.Minute.ClearDirty();
            }

            if (this.Seconds != null)
            {
                this.Seconds.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new AnalogClockElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is AnalogClockElementDataViewModel)
            {
                var that = (AnalogClockElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Hour != null)
                        {
                            result = result && this.Hour.EqualsViewModel(that.Hour);
                        }

                        if (this.Minute != null)
                        {
                            result = result && this.Minute.EqualsViewModel(that.Minute);
                        }

                        if (this.Seconds != null)
                        {
                            result = result && this.Seconds.EqualsViewModel(that.Seconds);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new AnalogClockElement();
        }

        protected override object CreateDataModelObject()
        {
            return new AnalogClockElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (AnalogClockElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            if (this.Hour != null)
            {
                model.Hour = (AnalogClockHandElement)this.Hour.Export(exportParameters);
            }

            if (this.Minute != null)
            {
                model.Minute = (AnalogClockHandElement)this.Minute.Export(exportParameters);
            }

            if (this.Seconds != null)
            {
                model.Seconds = (AnalogClockHandElement)this.Seconds.Export(exportParameters);
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (AnalogClockElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            if (this.Hour != null)
            {
                model.Hour = (AnalogClockHandElementDataModel)this.Hour.ToDataModel();
            }

            if (this.Minute != null)
            {
                model.Minute = (AnalogClockHandElementDataModel)this.Minute.ToDataModel();
            }

            if (this.Seconds != null)
            {
                model.Seconds = (AnalogClockHandElementDataModel)this.Seconds.ToDataModel();
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void HourChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HourChangedPartial(sender, e);
        }

        private void MinuteChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MinuteChangedPartial(sender, e);
        }

        private void SecondsChanged(object sender, PropertyChangedEventArgs e)
        {
            this.SecondsChangedPartial(sender, e);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void HourChangedPartial(object sender, PropertyChangedEventArgs e);

        partial void MinuteChangedPartial(object sender, PropertyChangedEventArgs e);

        partial void SecondsChangedPartial(object sender, PropertyChangedEventArgs e);

        partial void Initialize(AnalogClockElementDataModel dataModel = null);

        partial void Initialize(AnalogClockElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(AnalogClockElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref AnalogClockElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class AnalogClockHandElementDataViewModel : ImageElementDataViewModel
    {
        private readonly IMediaShell mediaShell;

        private DataValue<AnalogClockHandMode> mode;

        private DataValue<int> centerx;

        private DataValue<int> centery;

        public AnalogClockHandElementDataViewModel(IMediaShell mediaShell, AnalogClockHandElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Mode = new DataValue<AnalogClockHandMode>(new AnalogClockHandMode());
            this.Mode.PropertyChanged += this.ModeChanged;
            this.CenterX = new DataValue<int>(default(int));
            this.CenterX.PropertyChanged += this.CenterXChanged;
            this.CenterY = new DataValue<int>(default(int));
            this.CenterY.PropertyChanged += this.CenterYChanged;
            if (dataModel != null)
            {
                this.Mode.Value = dataModel.Mode;
                this.CenterX.Value = dataModel.CenterX;
                this.CenterY.Value = dataModel.CenterY;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected AnalogClockHandElementDataViewModel(IMediaShell mediaShell, AnalogClockHandElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Mode = (DataValue<AnalogClockHandMode>)dataViewModel.Mode.Clone();
            this.Mode.PropertyChanged += this.ModeChanged;
            this.CenterX = (DataValue<int>)dataViewModel.CenterX.Clone();
            this.CenterX.PropertyChanged += this.CenterXChanged;
            this.CenterY = (DataValue<int>)dataViewModel.CenterY.Clone();
            this.CenterY.PropertyChanged += this.CenterYChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Mode.IsDirty || this.CenterX.IsDirty || this.CenterY.IsDirty;
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 8, GroupOrderIndex = 1)]
        public DataValue<AnalogClockHandMode> Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.mode);
                if (this.mode != null)
                {
                    this.mode.PropertyChanged -= this.ModeChanged;
                }

                this.SetProperty(ref this.mode, value, () => this.Mode);
                if (value != null)
                {
                    this.mode.PropertyChanged += this.ModeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 6, GroupOrderIndex = 1)]
        public DataValue<int> CenterX
        {
            get
            {
                return this.centerx;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.centerx);
                if (this.centerx != null)
                {
                    this.centerx.PropertyChanged -= this.CenterXChanged;
                }

                this.SetProperty(ref this.centerx, value, () => this.CenterX);
                if (value != null)
                {
                    this.centerx.PropertyChanged += this.CenterXChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 7, GroupOrderIndex = 1)]
        public DataValue<int> CenterY
        {
            get
            {
                return this.centery;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.centery);
                if (this.centery != null)
                {
                    this.centery.PropertyChanged -= this.CenterYChanged;
                }

                this.SetProperty(ref this.centery, value, () => this.CenterY);
                if (value != null)
                {
                    this.centery.PropertyChanged += this.CenterYChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var analogclockhand = (AnalogClockHandElement)this.CreateExportObject();
            this.DoExport(analogclockhand, parameters);
            return analogclockhand;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var analogclockhand = (AnalogClockHandElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(analogclockhand);
            return analogclockhand;
        }

        public override void ClearDirty()
        {
            if (this.Mode != null)
            {
                this.Mode.ClearDirty();
            }

            if (this.CenterX != null)
            {
                this.CenterX.ClearDirty();
            }

            if (this.CenterY != null)
            {
                this.CenterY.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new AnalogClockHandElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is AnalogClockHandElementDataViewModel)
            {
                var that = (AnalogClockHandElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Mode.EqualsValue(that.Mode);
                        result = result && this.CenterX.EqualsValue(that.CenterX);
                        result = result && this.CenterY.EqualsValue(that.CenterY);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new AnalogClockHandElement();
        }

        protected override object CreateDataModelObject()
        {
            return new AnalogClockHandElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (AnalogClockHandElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Mode = this.Mode.Value;
            model.CenterX = this.CenterX.Value;
            model.CenterY = this.CenterY.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (AnalogClockHandElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Mode = this.Mode.Value;
            model.CenterX = this.CenterX.Value;
            model.CenterY = this.CenterY.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ModeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Mode);
        }

        private void CenterXChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.CenterX);
        }

        private void CenterYChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.CenterY);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(AnalogClockHandElementDataModel dataModel = null);

        partial void Initialize(AnalogClockHandElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(AnalogClockHandElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref AnalogClockHandElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class FrameElementDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> frameid;

        public FrameElementDataViewModel(IMediaShell mediaShell, FrameElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.FrameId = new DataValue<int>(default(int));
            this.FrameId.PropertyChanged += this.FrameIdChanged;
            if (dataModel != null)
            {
                this.FrameId.Value = dataModel.FrameId;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected FrameElementDataViewModel(IMediaShell mediaShell, FrameElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.FrameId = (DataValue<int>)dataViewModel.FrameId.Clone();
            this.FrameId.PropertyChanged += this.FrameIdChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.FrameId.IsDirty;
            }
        }

        [UserVisibleProperty("Layout", Filter = "TFT", OrderIndex = 6, GroupOrderIndex = 0)]
        public DataValue<int> FrameId
        {
            get
            {
                return this.frameid;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.frameid);
                if (this.frameid != null)
                {
                    this.frameid.PropertyChanged -= this.FrameIdChanged;
                }

                this.SetProperty(ref this.frameid, value, () => this.FrameId);
                if (value != null)
                {
                    this.frameid.PropertyChanged += this.FrameIdChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var frame = (FrameElement)this.CreateExportObject();
            this.DoExport(frame, parameters);
            return frame;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var frame = (FrameElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(frame);
            return frame;
        }

        public override void ClearDirty()
        {
            if (this.FrameId != null)
            {
                this.FrameId.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new FrameElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is FrameElementDataViewModel)
            {
                var that = (FrameElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.FrameId.EqualsValue(that.FrameId);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new FrameElement();
        }

        protected override object CreateDataModelObject()
        {
            return new FrameElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (FrameElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.FrameId = this.FrameId.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (FrameElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.FrameId = this.FrameId.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FrameIdChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FrameId);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(FrameElementDataModel dataModel = null);

        partial void Initialize(FrameElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(FrameElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref FrameElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class ImageElementDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private AnimatedDynamicDataValue<string> filename;

        private DataValue<ElementScaling> scaling;

        public ImageElementDataViewModel(IMediaShell mediaShell, ImageElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Filename = new AnimatedDynamicDataValue<string>(string.Empty);
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.Scaling = new DataValue<ElementScaling>(ElementScaling.Stretch);
            this.Scaling.PropertyChanged += this.ScalingChanged;
            if (dataModel != null)
            {
                if (dataModel.Filename != null)
                {
                    this.Filename.Value = dataModel.Filename;
                }

                if (dataModel.FilenameProperty != null)
                {
                    this.Filename.Formula = this.CreateEvalDataViewModel(dataModel.FilenameProperty.Evaluation);
                    if (dataModel.FilenameProperty.Animation != null)
                    {
                        this.Filename.Animation = new AnimationDataViewModel(this.mediaShell, dataModel.FilenameProperty.Animation);
                    }

                }

                this.Scaling.Value = dataModel.Scaling;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected ImageElementDataViewModel(IMediaShell mediaShell, ImageElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Filename = (AnimatedDynamicDataValue<string>)dataViewModel.Filename.Clone();
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.Scaling = (DataValue<ElementScaling>)dataViewModel.Scaling.Clone();
            this.Scaling.PropertyChanged += this.ScalingChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Filename.IsDirty || this.Scaling.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 1)]
        public AnimatedDynamicDataValue<string> Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filename);
                if (this.filename != null)
                {
                    this.filename.PropertyChanged -= this.FilenameChanged;
                }

                this.SetProperty(ref this.filename, value, () => this.Filename);
                if (value != null)
                {
                    this.filename.PropertyChanged += this.FilenameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 9, GroupOrderIndex = 1)]
        public DataValue<ElementScaling> Scaling
        {
            get
            {
                return this.scaling;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.scaling);
                if (this.scaling != null)
                {
                    this.scaling.PropertyChanged -= this.ScalingChanged;
                }

                this.SetProperty(ref this.scaling, value, () => this.Scaling);
                if (value != null)
                {
                    this.scaling.PropertyChanged += this.ScalingChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var image = (ImageElement)this.CreateExportObject();
            this.DoExport(image, parameters);
            return image;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var image = (ImageElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(image);
            return image;
        }

        public override void ClearDirty()
        {
            if (this.Filename != null)
            {
                this.Filename.ClearDirty();
            }

            if (this.Scaling != null)
            {
                this.Scaling.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new ImageElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is ImageElementDataViewModel)
            {
                var that = (ImageElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Filename.EqualsValue(that.Filename);
                        result = result && this.Scaling.EqualsValue(that.Scaling);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new ImageElement();
        }

        protected override object CreateDataModelObject()
        {
            return new ImageElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (ImageElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            if (this.Filename.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Filename.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var filenameEval = Filename.Formula as CodeConversionEvalDataViewModel;
                    if (filenameEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = filenameEval.UseImage.Value ? "{2}" : "{3}";
                            model.FilenameProperty = new AnimatedDynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.FilenameProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Filename.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.FilenameProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Filename.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Filename = this.Filename.Value;
                }
            }
            else
            {
                model.Filename = this.Filename.Value;
            }

            if (this.Filename.Animation != null && this.Filename.Formula != null)
            {
                if (model.FilenameProperty == null)
                {
                    model.FilenameProperty = new AnimatedDynamicProperty();
                }

                model.FilenameProperty.Animation = new PropertyChangeAnimation {
                    Duration = ((AnimationDataViewModel)this.Filename.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Filename.Animation).Type.Value
                };
            }

            model.Scaling = this.Scaling.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (ImageElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Filename = this.Filename.Value;
            if (this.Filename.Formula != null)
            {
                model.FilenameProperty = new AnimatedDynamicPropertyDataModel(((EvalDataViewModelBase)this.Filename.Formula).ToDataModel());
            }

            if (this.Filename.Animation != null)
            {
                if (model.FilenameProperty == null)
                {
                    model.FilenameProperty = new AnimatedDynamicPropertyDataModel();
                }

                model.FilenameProperty.Animation = new PropertyChangeAnimationDataModel {
                    Duration = ((AnimationDataViewModel)this.Filename.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Filename.Animation).Type.Value
                };
            }

            model.Scaling = this.Scaling.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FilenameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Filename);
        }

        private void ScalingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Scaling);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(ImageElementDataModel dataModel = null);

        partial void Initialize(ImageElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(ImageElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref ImageElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class ImageListElementDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<TextOverflow> overflow;

        private DataValue<HorizontalAlignment> align;

        private DataValue<TextDirection> direction;

        private DataValue<int> horizontalimagegap;

        private DataValue<int> verticalimagegap;

        private DataValue<int> imagewidth;

        private DataValue<int> imageheight;

        private DataValue<string> delimiter;

        private DataValue<string> filepatterns;

        private DataValue<string> fallbackimage;

        private DynamicDataValue<string> values;

        public ImageListElementDataViewModel(IMediaShell mediaShell, ImageListElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Overflow = new DataValue<TextOverflow>(new TextOverflow());
            this.Overflow.PropertyChanged += this.OverflowChanged;
            this.Align = new DataValue<HorizontalAlignment>(new HorizontalAlignment());
            this.Align.PropertyChanged += this.AlignChanged;
            this.Direction = new DataValue<TextDirection>(new TextDirection());
            this.Direction.PropertyChanged += this.DirectionChanged;
            this.HorizontalImageGap = new DataValue<int>(default(int));
            this.HorizontalImageGap.PropertyChanged += this.HorizontalImageGapChanged;
            this.VerticalImageGap = new DataValue<int>(default(int));
            this.VerticalImageGap.PropertyChanged += this.VerticalImageGapChanged;
            this.ImageWidth = new DataValue<int>(default(int));
            this.ImageWidth.PropertyChanged += this.ImageWidthChanged;
            this.ImageHeight = new DataValue<int>(default(int));
            this.ImageHeight.PropertyChanged += this.ImageHeightChanged;
            this.Delimiter = new DataValue<string>(";");
            this.Delimiter.PropertyChanged += this.DelimiterChanged;
            this.FilePatterns = new DataValue<string>(string.Empty);
            this.FilePatterns.PropertyChanged += this.FilePatternsChanged;
            this.FallbackImage = new DataValue<string>(string.Empty);
            this.FallbackImage.PropertyChanged += this.FallbackImageChanged;
            this.Values = new DynamicDataValue<string>(string.Empty);
            this.Values.PropertyChanged += this.ValuesChanged;
            if (dataModel != null)
            {
                this.Overflow.Value = dataModel.Overflow;
                this.Align.Value = dataModel.Align;
                this.Direction.Value = dataModel.Direction;
                this.HorizontalImageGap.Value = dataModel.HorizontalImageGap;
                this.VerticalImageGap.Value = dataModel.VerticalImageGap;
                this.ImageWidth.Value = dataModel.ImageWidth;
                this.ImageHeight.Value = dataModel.ImageHeight;
                this.Delimiter.Value = dataModel.Delimiter;
                this.FilePatterns.Value = dataModel.FilePatterns;
                this.FallbackImage.Value = dataModel.FallbackImage;
                if (dataModel.Values != null)
                {
                    this.Values.Value = dataModel.Values;
                }

                if (dataModel.ValuesProperty != null)
                {
                    this.Values.Formula = this.CreateEvalDataViewModel(dataModel.ValuesProperty.Evaluation);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected ImageListElementDataViewModel(IMediaShell mediaShell, ImageListElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Overflow = (DataValue<TextOverflow>)dataViewModel.Overflow.Clone();
            this.Overflow.PropertyChanged += this.OverflowChanged;
            this.Align = (DataValue<HorizontalAlignment>)dataViewModel.Align.Clone();
            this.Align.PropertyChanged += this.AlignChanged;
            this.Direction = (DataValue<TextDirection>)dataViewModel.Direction.Clone();
            this.Direction.PropertyChanged += this.DirectionChanged;
            this.HorizontalImageGap = (DataValue<int>)dataViewModel.HorizontalImageGap.Clone();
            this.HorizontalImageGap.PropertyChanged += this.HorizontalImageGapChanged;
            this.VerticalImageGap = (DataValue<int>)dataViewModel.VerticalImageGap.Clone();
            this.VerticalImageGap.PropertyChanged += this.VerticalImageGapChanged;
            this.ImageWidth = (DataValue<int>)dataViewModel.ImageWidth.Clone();
            this.ImageWidth.PropertyChanged += this.ImageWidthChanged;
            this.ImageHeight = (DataValue<int>)dataViewModel.ImageHeight.Clone();
            this.ImageHeight.PropertyChanged += this.ImageHeightChanged;
            this.Delimiter = (DataValue<string>)dataViewModel.Delimiter.Clone();
            this.Delimiter.PropertyChanged += this.DelimiterChanged;
            this.FilePatterns = (DataValue<string>)dataViewModel.FilePatterns.Clone();
            this.FilePatterns.PropertyChanged += this.FilePatternsChanged;
            this.FallbackImage = (DataValue<string>)dataViewModel.FallbackImage.Clone();
            this.FallbackImage.PropertyChanged += this.FallbackImageChanged;
            this.Values = (DynamicDataValue<string>)dataViewModel.Values.Clone();
            this.Values.PropertyChanged += this.ValuesChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Overflow.IsDirty || this.Align.IsDirty || this.Direction.IsDirty || this.HorizontalImageGap.IsDirty || this.VerticalImageGap.IsDirty || this.ImageWidth.IsDirty || this.ImageHeight.IsDirty || this.Delimiter.IsDirty || this.FilePatterns.IsDirty || this.FallbackImage.IsDirty || this.Values.IsDirty;
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 5, GroupOrderIndex = 1)]
        public DataValue<TextOverflow> Overflow
        {
            get
            {
                return this.overflow;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.overflow);
                if (this.overflow != null)
                {
                    this.overflow.PropertyChanged -= this.OverflowChanged;
                }

                this.SetProperty(ref this.overflow, value, () => this.Overflow);
                if (value != null)
                {
                    this.overflow.PropertyChanged += this.OverflowChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 4, GroupOrderIndex = 1)]
        public DataValue<HorizontalAlignment> Align
        {
            get
            {
                return this.align;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.align);
                if (this.align != null)
                {
                    this.align.PropertyChanged -= this.AlignChanged;
                }

                this.SetProperty(ref this.align, value, () => this.Align);
                if (value != null)
                {
                    this.align.PropertyChanged += this.AlignChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 6, GroupOrderIndex = 1)]
        public DataValue<TextDirection> Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.direction);
                if (this.direction != null)
                {
                    this.direction.PropertyChanged -= this.DirectionChanged;
                }

                this.SetProperty(ref this.direction, value, () => this.Direction);
                if (value != null)
                {
                    this.direction.PropertyChanged += this.DirectionChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 9, GroupOrderIndex = 1)]
        public DataValue<int> HorizontalImageGap
        {
            get
            {
                return this.horizontalimagegap;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.horizontalimagegap);
                if (this.horizontalimagegap != null)
                {
                    this.horizontalimagegap.PropertyChanged -= this.HorizontalImageGapChanged;
                }

                this.SetProperty(ref this.horizontalimagegap, value, () => this.HorizontalImageGap);
                if (value != null)
                {
                    this.horizontalimagegap.PropertyChanged += this.HorizontalImageGapChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 10, GroupOrderIndex = 1)]
        public DataValue<int> VerticalImageGap
        {
            get
            {
                return this.verticalimagegap;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.verticalimagegap);
                if (this.verticalimagegap != null)
                {
                    this.verticalimagegap.PropertyChanged -= this.VerticalImageGapChanged;
                }

                this.SetProperty(ref this.verticalimagegap, value, () => this.VerticalImageGap);
                if (value != null)
                {
                    this.verticalimagegap.PropertyChanged += this.VerticalImageGapChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 7, GroupOrderIndex = 1)]
        public DataValue<int> ImageWidth
        {
            get
            {
                return this.imagewidth;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.imagewidth);
                if (this.imagewidth != null)
                {
                    this.imagewidth.PropertyChanged -= this.ImageWidthChanged;
                }

                this.SetProperty(ref this.imagewidth, value, () => this.ImageWidth);
                if (value != null)
                {
                    this.imagewidth.PropertyChanged += this.ImageWidthChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 8, GroupOrderIndex = 1)]
        public DataValue<int> ImageHeight
        {
            get
            {
                return this.imageheight;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.imageheight);
                if (this.imageheight != null)
                {
                    this.imageheight.PropertyChanged -= this.ImageHeightChanged;
                }

                this.SetProperty(ref this.imageheight, value, () => this.ImageHeight);
                if (value != null)
                {
                    this.imageheight.PropertyChanged += this.ImageHeightChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 2, GroupOrderIndex = 1)]
        public DataValue<string> Delimiter
        {
            get
            {
                return this.delimiter;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.delimiter);
                if (this.delimiter != null)
                {
                    this.delimiter.PropertyChanged -= this.DelimiterChanged;
                }

                this.SetProperty(ref this.delimiter, value, () => this.Delimiter);
                if (value != null)
                {
                    this.delimiter.PropertyChanged += this.DelimiterChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 1, GroupOrderIndex = 1)]
        public DataValue<string> FilePatterns
        {
            get
            {
                return this.filepatterns;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filepatterns);
                if (this.filepatterns != null)
                {
                    this.filepatterns.PropertyChanged -= this.FilePatternsChanged;
                }

                this.SetProperty(ref this.filepatterns, value, () => this.FilePatterns);
                if (value != null)
                {
                    this.filepatterns.PropertyChanged += this.FilePatternsChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 3, GroupOrderIndex = 1)]
        public DataValue<string> FallbackImage
        {
            get
            {
                return this.fallbackimage;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.fallbackimage);
                if (this.fallbackimage != null)
                {
                    this.fallbackimage.PropertyChanged -= this.FallbackImageChanged;
                }

                this.SetProperty(ref this.fallbackimage, value, () => this.FallbackImage);
                if (value != null)
                {
                    this.fallbackimage.PropertyChanged += this.FallbackImageChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 0, GroupOrderIndex = 1)]
        public DynamicDataValue<string> Values
        {
            get
            {
                return this.values;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.values);
                if (this.values != null)
                {
                    this.values.PropertyChanged -= this.ValuesChanged;
                }

                this.SetProperty(ref this.values, value, () => this.Values);
                if (value != null)
                {
                    this.values.PropertyChanged += this.ValuesChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var imagelist = (ImageListElement)this.CreateExportObject();
            this.DoExport(imagelist, parameters);
            return imagelist;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var imagelist = (ImageListElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(imagelist);
            return imagelist;
        }

        public override void ClearDirty()
        {
            if (this.Overflow != null)
            {
                this.Overflow.ClearDirty();
            }

            if (this.Align != null)
            {
                this.Align.ClearDirty();
            }

            if (this.Direction != null)
            {
                this.Direction.ClearDirty();
            }

            if (this.HorizontalImageGap != null)
            {
                this.HorizontalImageGap.ClearDirty();
            }

            if (this.VerticalImageGap != null)
            {
                this.VerticalImageGap.ClearDirty();
            }

            if (this.ImageWidth != null)
            {
                this.ImageWidth.ClearDirty();
            }

            if (this.ImageHeight != null)
            {
                this.ImageHeight.ClearDirty();
            }

            if (this.Delimiter != null)
            {
                this.Delimiter.ClearDirty();
            }

            if (this.FilePatterns != null)
            {
                this.FilePatterns.ClearDirty();
            }

            if (this.FallbackImage != null)
            {
                this.FallbackImage.ClearDirty();
            }

            if (this.Values != null)
            {
                this.Values.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new ImageListElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is ImageListElementDataViewModel)
            {
                var that = (ImageListElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Overflow.EqualsValue(that.Overflow);
                        result = result && this.Align.EqualsValue(that.Align);
                        result = result && this.Direction.EqualsValue(that.Direction);
                        result = result && this.HorizontalImageGap.EqualsValue(that.HorizontalImageGap);
                        result = result && this.VerticalImageGap.EqualsValue(that.VerticalImageGap);
                        result = result && this.ImageWidth.EqualsValue(that.ImageWidth);
                        result = result && this.ImageHeight.EqualsValue(that.ImageHeight);
                        result = result && this.Delimiter.EqualsValue(that.Delimiter);
                        result = result && this.FilePatterns.EqualsValue(that.FilePatterns);
                        result = result && this.FallbackImage.EqualsValue(that.FallbackImage);
                        result = result && this.Values.EqualsValue(that.Values);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new ImageListElement();
        }

        protected override object CreateDataModelObject()
        {
            return new ImageListElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (ImageListElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Overflow = this.Overflow.Value;
            model.Align = this.Align.Value;
            model.Direction = this.Direction.Value;
            model.HorizontalImageGap = this.HorizontalImageGap.Value;
            model.VerticalImageGap = this.VerticalImageGap.Value;
            model.ImageWidth = this.ImageWidth.Value;
            model.ImageHeight = this.ImageHeight.Value;
            model.Delimiter = this.Delimiter.Value;
            model.FilePatterns = this.FilePatterns.Value;
            model.FallbackImage = this.FallbackImage.Value;
            if (this.Values.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Values.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var valuesEval = Values.Formula as CodeConversionEvalDataViewModel;
                    if (valuesEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = valuesEval.UseImage.Value ? "{2}" : "{3}";
                            model.ValuesProperty = new DynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.ValuesProperty = new DynamicProperty(((EvalDataViewModelBase)this.Values.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.ValuesProperty = new DynamicProperty(((EvalDataViewModelBase)this.Values.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Values = this.Values.Value;
                }
            }
            else
            {
                model.Values = this.Values.Value;
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (ImageListElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Overflow = this.Overflow.Value;
            model.Align = this.Align.Value;
            model.Direction = this.Direction.Value;
            model.HorizontalImageGap = this.HorizontalImageGap.Value;
            model.VerticalImageGap = this.VerticalImageGap.Value;
            model.ImageWidth = this.ImageWidth.Value;
            model.ImageHeight = this.ImageHeight.Value;
            model.Delimiter = this.Delimiter.Value;
            model.FilePatterns = this.FilePatterns.Value;
            model.FallbackImage = this.FallbackImage.Value;
            model.Values = this.Values.Value;
            if (this.Values.Formula != null)
            {
                model.ValuesProperty = new DynamicPropertyDataModel(((EvalDataViewModelBase)this.Values.Formula).ToDataModel());
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void OverflowChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Overflow);
        }

        private void AlignChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Align);
        }

        private void DirectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Direction);
        }

        private void HorizontalImageGapChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.HorizontalImageGap);
        }

        private void VerticalImageGapChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.VerticalImageGap);
        }

        private void ImageWidthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.ImageWidth);
        }

        private void ImageHeightChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.ImageHeight);
        }

        private void DelimiterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Delimiter);
        }

        private void FilePatternsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FilePatterns);
        }

        private void FallbackImageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FallbackImage);
        }

        private void ValuesChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Values);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(ImageListElementDataModel dataModel = null);

        partial void Initialize(ImageListElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(ImageListElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref ImageListElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class TextElementDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> rotation;

        private DataValue<HorizontalAlignment> align;

        private DataValue<VerticalAlignment> valign;

        private DataValue<TextOverflow> overflow;

        private DataValue<int> scrollspeed;

        private FontDataViewModel font;

        private AnimatedDynamicDataValue<string> value;

        public TextElementDataViewModel(IMediaShell mediaShell, TextElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Rotation = new DataValue<int>(0);
            this.Rotation.PropertyChanged += this.RotationChanged;
            this.Align = new DataValue<HorizontalAlignment>(new HorizontalAlignment());
            this.Align.PropertyChanged += this.AlignChanged;
            this.VAlign = new DataValue<VerticalAlignment>(VerticalAlignment.Top);
            this.VAlign.PropertyChanged += this.VAlignChanged;
            this.Overflow = new DataValue<TextOverflow>(new TextOverflow());
            this.Overflow.PropertyChanged += this.OverflowChanged;
            this.ScrollSpeed = new DataValue<int>(0);
            this.ScrollSpeed.PropertyChanged += this.ScrollSpeedChanged;
            this.font = new FontDataViewModel(this.mediaShell);
            this.Value = new AnimatedDynamicDataValue<string>(string.Empty);
            this.Value.PropertyChanged += this.ValueChanged;
            if (dataModel != null)
            {
                this.Rotation.Value = dataModel.Rotation;
                this.Align.Value = dataModel.Align;
                this.VAlign.Value = dataModel.VAlign;
                this.Overflow.Value = dataModel.Overflow;
                this.ScrollSpeed.Value = dataModel.ScrollSpeed;
                this.font = new FontDataViewModel(this.mediaShell, dataModel.Font);
                if (dataModel.Value != null)
                {
                    this.Value.Value = dataModel.Value;
                }

                if (dataModel.ValueProperty != null)
                {
                    this.Value.Formula = this.CreateEvalDataViewModel(dataModel.ValueProperty.Evaluation);
                    if (dataModel.ValueProperty.Animation != null)
                    {
                        this.Value.Animation = new AnimationDataViewModel(this.mediaShell, dataModel.ValueProperty.Animation);
                    }

                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected TextElementDataViewModel(IMediaShell mediaShell, TextElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Rotation = (DataValue<int>)dataViewModel.Rotation.Clone();
            this.Rotation.PropertyChanged += this.RotationChanged;
            this.Align = (DataValue<HorizontalAlignment>)dataViewModel.Align.Clone();
            this.Align.PropertyChanged += this.AlignChanged;
            this.VAlign = (DataValue<VerticalAlignment>)dataViewModel.VAlign.Clone();
            this.VAlign.PropertyChanged += this.VAlignChanged;
            this.Overflow = (DataValue<TextOverflow>)dataViewModel.Overflow.Clone();
            this.Overflow.PropertyChanged += this.OverflowChanged;
            this.ScrollSpeed = (DataValue<int>)dataViewModel.ScrollSpeed.Clone();
            this.ScrollSpeed.PropertyChanged += this.ScrollSpeedChanged;
            var clonedFont = dataViewModel.Font;
            if (clonedFont != null)
            {
                this.Font = (FontDataViewModel)clonedFont.Clone();
            }

            this.Value = (AnimatedDynamicDataValue<string>)dataViewModel.Value.Clone();
            this.Value.PropertyChanged += this.ValueChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Rotation.IsDirty || this.Align.IsDirty || this.VAlign.IsDirty || this.Overflow.IsDirty || this.ScrollSpeed.IsDirty || this.Value.IsDirty || (this.Font != null && this.Font.IsDirty);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 5, GroupOrderIndex = 1)]
        public DataValue<int> Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.rotation);
                if (this.rotation != null)
                {
                    this.rotation.PropertyChanged -= this.RotationChanged;
                }

                this.SetProperty(ref this.rotation, value, () => this.Rotation);
                if (value != null)
                {
                    this.rotation.PropertyChanged += this.RotationChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 1, GroupOrderIndex = 1)]
        public DataValue<HorizontalAlignment> Align
        {
            get
            {
                return this.align;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.align);
                if (this.align != null)
                {
                    this.align.PropertyChanged -= this.AlignChanged;
                }

                this.SetProperty(ref this.align, value, () => this.Align);
                if (value != null)
                {
                    this.align.PropertyChanged += this.AlignChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 2, GroupOrderIndex = 1)]
        public DataValue<VerticalAlignment> VAlign
        {
            get
            {
                return this.valign;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.valign);
                if (this.valign != null)
                {
                    this.valign.PropertyChanged -= this.VAlignChanged;
                }

                this.SetProperty(ref this.valign, value, () => this.VAlign);
                if (value != null)
                {
                    this.valign.PropertyChanged += this.VAlignChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 3, GroupOrderIndex = 1)]
        public DataValue<TextOverflow> Overflow
        {
            get
            {
                return this.overflow;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.overflow);
                if (this.overflow != null)
                {
                    this.overflow.PropertyChanged -= this.OverflowChanged;
                }

                this.SetProperty(ref this.overflow, value, () => this.Overflow);
                if (value != null)
                {
                    this.overflow.PropertyChanged += this.OverflowChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 4, GroupOrderIndex = 1)]
        public DataValue<int> ScrollSpeed
        {
            get
            {
                return this.scrollspeed;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.scrollspeed);
                if (this.scrollspeed != null)
                {
                    this.scrollspeed.PropertyChanged -= this.ScrollSpeedChanged;
                }

                this.SetProperty(ref this.scrollspeed, value, () => this.ScrollSpeed);
                if (value != null)
                {
                    this.scrollspeed.PropertyChanged += this.ScrollSpeedChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public FontDataViewModel Font
        {
            get
            {
                return this.font;
            }
            set
            {
                if (this.font != null)
                {
                    this.font.PropertyChanged -= this.FontChanged;
                }

                this.SetProperty(ref this.font, value, () => this.Font);
                if (value != null)
                {
                    this.font.PropertyChanged += this.FontChanged;
                }

            }
        }

        [UserVisibleProperty("Font", FieldName = "Face", OrderIndex = 0, GroupOrderIndex = 2)]
        public DataValue<string> FontFace
        {
            get
            {
                return Font.Face;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Font", FieldName = "Height", Filter = "TFT", OrderIndex = 1, GroupOrderIndex = 2)]
        public DataValue<int> FontHeight
        {
            get
            {
                return Font.Height;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Font", FieldName = "Weight", Filter = "TFT", OrderIndex = 2, GroupOrderIndex = 2)]
        public DataValue<int> FontWeight
        {
            get
            {
                return Font.Weight;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Font", FieldName = "Italic", Filter = "TFT", OrderIndex = 3, GroupOrderIndex = 2)]
        public DataValue<bool> FontItalic
        {
            get
            {
                return Font.Italic;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Font", FieldName = "Color", OrderIndex = 4, GroupOrderIndex = 2)]
        public DataValue<string> FontColor
        {
            get
            {
                return Font.Color;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Font", FieldName = "OutlineColor", Filter = "LED", OrderIndex = 5, GroupOrderIndex = 2)]
        public DataValue<string> FontOutlineColor
        {
            get
            {
                return Font.OutlineColor;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Font", FieldName = "CharSpacing", Filter = "LED", OrderIndex = 6, GroupOrderIndex = 2)]
        public DataValue<int> FontCharSpacing
        {
            get
            {
                return Font.CharSpacing;
            }
            set
            {
                return;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 1)]
        public AnimatedDynamicDataValue<string> Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.value);
                if (this.value != null)
                {
                    this.value.PropertyChanged -= this.ValueChanged;
                }

                this.SetProperty(ref this.value, value, () => this.Value);
                if (value != null)
                {
                    this.value.PropertyChanged += this.ValueChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var text = (TextElement)this.CreateExportObject();
            this.DoExport(text, parameters);
            return text;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var text = (TextElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(text);
            return text;
        }

        public override void ClearDirty()
        {
            if (this.Rotation != null)
            {
                this.Rotation.ClearDirty();
            }

            if (this.Align != null)
            {
                this.Align.ClearDirty();
            }

            if (this.VAlign != null)
            {
                this.VAlign.ClearDirty();
            }

            if (this.Overflow != null)
            {
                this.Overflow.ClearDirty();
            }

            if (this.ScrollSpeed != null)
            {
                this.ScrollSpeed.ClearDirty();
            }

            if (this.Font != null)
            {
                this.Font.ClearDirty();
            }

            if (this.Value != null)
            {
                this.Value.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new TextElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is TextElementDataViewModel)
            {
                var that = (TextElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Rotation.EqualsValue(that.Rotation);
                        result = result && this.Align.EqualsValue(that.Align);
                        result = result && this.VAlign.EqualsValue(that.VAlign);
                        result = result && this.Overflow.EqualsValue(that.Overflow);
                        result = result && this.ScrollSpeed.EqualsValue(that.ScrollSpeed);
                        if (this.Font != null)
                        {
                            result = result && this.Font.EqualsViewModel(that.Font);
                        }

                        result = result && this.Value.EqualsValue(that.Value);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new TextElement();
        }

        protected override object CreateDataModelObject()
        {
            return new TextElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (TextElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Rotation = this.Rotation.Value;
            model.Align = this.Align.Value;
            model.VAlign = this.VAlign.Value;
            model.Overflow = this.Overflow.Value;
            model.ScrollSpeed = this.ScrollSpeed.Value;
            if (this.Font != null)
            {
                model.Font = this.Font.Export(exportParameters);
            }

            if (this.Value.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Value.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var valueEval = Value.Formula as CodeConversionEvalDataViewModel;
                    if (valueEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = valueEval.UseImage.Value ? "{2}" : "{3}";
                            model.ValueProperty = new AnimatedDynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.ValueProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Value.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.ValueProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Value.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Value = this.Value.Value;
                }
            }
            else
            {
                model.Value = this.Value.Value;
            }

            if (this.Value.Animation != null && this.Value.Formula != null)
            {
                if (model.ValueProperty == null)
                {
                    model.ValueProperty = new AnimatedDynamicProperty();
                }

                model.ValueProperty.Animation = new PropertyChangeAnimation {
                    Duration = ((AnimationDataViewModel)this.Value.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Value.Animation).Type.Value
                };
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (TextElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Rotation = this.Rotation.Value;
            model.Align = this.Align.Value;
            model.VAlign = this.VAlign.Value;
            model.Overflow = this.Overflow.Value;
            model.ScrollSpeed = this.ScrollSpeed.Value;
            if (this.Font != null)
            {
                model.Font = this.Font.ToDataModel();
            }

            model.Value = this.Value.Value;
            if (this.Value.Formula != null)
            {
                model.ValueProperty = new AnimatedDynamicPropertyDataModel(((EvalDataViewModelBase)this.Value.Formula).ToDataModel());
            }

            if (this.Value.Animation != null)
            {
                if (model.ValueProperty == null)
                {
                    model.ValueProperty = new AnimatedDynamicPropertyDataModel();
                }

                model.ValueProperty.Animation = new PropertyChangeAnimationDataModel {
                    Duration = ((AnimationDataViewModel)this.Value.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Value.Animation).Type.Value
                };
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void RotationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Rotation);
        }

        private void AlignChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Align);
        }

        private void VAlignChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.VAlign);
        }

        private void OverflowChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Overflow);
        }

        private void ScrollSpeedChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.ScrollSpeed);
        }

        private void FontChanged(object sender, PropertyChangedEventArgs e)
        {
            this.FontChangedPartial(sender, e);
        }

        private void ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Value);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void FontChangedPartial(object sender, PropertyChangedEventArgs e);

        partial void Initialize(TextElementDataModel dataModel = null);

        partial void Initialize(TextElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(TextElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref TextElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class VideoElementDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private AnimatedDynamicDataValue<string> videouri;

        private DataValue<ElementScaling> scaling;

        private DataValue<bool> replay;

        private DataValue<string> fallbackimage;

        public VideoElementDataViewModel(IMediaShell mediaShell, VideoElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.VideoUri = new AnimatedDynamicDataValue<string>(string.Empty);
            this.VideoUri.PropertyChanged += this.VideoUriChanged;
            this.Scaling = new DataValue<ElementScaling>(ElementScaling.Stretch);
            this.Scaling.PropertyChanged += this.ScalingChanged;
            this.Replay = new DataValue<bool>(true);
            this.Replay.PropertyChanged += this.ReplayChanged;
            this.FallbackImage = new DataValue<string>(string.Empty);
            this.FallbackImage.PropertyChanged += this.FallbackImageChanged;
            if (dataModel != null)
            {
                if (dataModel.VideoUri != null)
                {
                    this.VideoUri.Value = dataModel.VideoUri;
                }

                if (dataModel.VideoUriProperty != null)
                {
                    this.VideoUri.Formula = this.CreateEvalDataViewModel(dataModel.VideoUriProperty.Evaluation);
                    if (dataModel.VideoUriProperty.Animation != null)
                    {
                        this.VideoUri.Animation = new AnimationDataViewModel(this.mediaShell, dataModel.VideoUriProperty.Animation);
                    }

                }

                this.Scaling.Value = dataModel.Scaling;
                this.Replay.Value = dataModel.Replay;
                this.FallbackImage.Value = dataModel.FallbackImage;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected VideoElementDataViewModel(IMediaShell mediaShell, VideoElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.VideoUri = (AnimatedDynamicDataValue<string>)dataViewModel.VideoUri.Clone();
            this.VideoUri.PropertyChanged += this.VideoUriChanged;
            this.Scaling = (DataValue<ElementScaling>)dataViewModel.Scaling.Clone();
            this.Scaling.PropertyChanged += this.ScalingChanged;
            this.Replay = (DataValue<bool>)dataViewModel.Replay.Clone();
            this.Replay.PropertyChanged += this.ReplayChanged;
            this.FallbackImage = (DataValue<string>)dataViewModel.FallbackImage.Clone();
            this.FallbackImage.PropertyChanged += this.FallbackImageChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.VideoUri.IsDirty || this.Scaling.IsDirty || this.Replay.IsDirty || this.FallbackImage.IsDirty;
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 0, GroupOrderIndex = 1)]
        public AnimatedDynamicDataValue<string> VideoUri
        {
            get
            {
                return this.videouri;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.videouri);
                if (this.videouri != null)
                {
                    this.videouri.PropertyChanged -= this.VideoUriChanged;
                }

                this.SetProperty(ref this.videouri, value, () => this.VideoUri);
                if (value != null)
                {
                    this.videouri.PropertyChanged += this.VideoUriChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 6, GroupOrderIndex = 1)]
        public DataValue<ElementScaling> Scaling
        {
            get
            {
                return this.scaling;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.scaling);
                if (this.scaling != null)
                {
                    this.scaling.PropertyChanged -= this.ScalingChanged;
                }

                this.SetProperty(ref this.scaling, value, () => this.Scaling);
                if (value != null)
                {
                    this.scaling.PropertyChanged += this.ScalingChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 7, GroupOrderIndex = 1)]
        public DataValue<bool> Replay
        {
            get
            {
                return this.replay;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.replay);
                if (this.replay != null)
                {
                    this.replay.PropertyChanged -= this.ReplayChanged;
                }

                this.SetProperty(ref this.replay, value, () => this.Replay);
                if (value != null)
                {
                    this.replay.PropertyChanged += this.ReplayChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "TFT", OrderIndex = 0, GroupOrderIndex = 1)]
        public DataValue<string> FallbackImage
        {
            get
            {
                return this.fallbackimage;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.fallbackimage);
                if (this.fallbackimage != null)
                {
                    this.fallbackimage.PropertyChanged -= this.FallbackImageChanged;
                }

                this.SetProperty(ref this.fallbackimage, value, () => this.FallbackImage);
                if (value != null)
                {
                    this.fallbackimage.PropertyChanged += this.FallbackImageChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var video = (VideoElement)this.CreateExportObject();
            this.DoExport(video, parameters);
            return video;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var video = (VideoElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(video);
            return video;
        }

        public override void ClearDirty()
        {
            if (this.VideoUri != null)
            {
                this.VideoUri.ClearDirty();
            }

            if (this.Scaling != null)
            {
                this.Scaling.ClearDirty();
            }

            if (this.Replay != null)
            {
                this.Replay.ClearDirty();
            }

            if (this.FallbackImage != null)
            {
                this.FallbackImage.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new VideoElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is VideoElementDataViewModel)
            {
                var that = (VideoElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.VideoUri.EqualsValue(that.VideoUri);
                        result = result && this.Scaling.EqualsValue(that.Scaling);
                        result = result && this.Replay.EqualsValue(that.Replay);
                        result = result && this.FallbackImage.EqualsValue(that.FallbackImage);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new VideoElement();
        }

        protected override object CreateDataModelObject()
        {
            return new VideoElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (VideoElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            if (this.VideoUri.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.VideoUri.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var videouriEval = VideoUri.Formula as CodeConversionEvalDataViewModel;
                    if (videouriEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = videouriEval.UseImage.Value ? "{2}" : "{3}";
                            model.VideoUriProperty = new AnimatedDynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.VideoUriProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.VideoUri.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.VideoUriProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.VideoUri.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.VideoUri = this.VideoUri.Value;
                }
            }
            else
            {
                model.VideoUri = this.VideoUri.Value;
            }

            if (this.VideoUri.Animation != null && this.VideoUri.Formula != null)
            {
                if (model.VideoUriProperty == null)
                {
                    model.VideoUriProperty = new AnimatedDynamicProperty();
                }

                model.VideoUriProperty.Animation = new PropertyChangeAnimation {
                    Duration = ((AnimationDataViewModel)this.VideoUri.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.VideoUri.Animation).Type.Value
                };
            }

            model.Scaling = this.Scaling.Value;
            model.Replay = this.Replay.Value;
            model.FallbackImage = this.FallbackImage.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (VideoElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.VideoUri = this.VideoUri.Value;
            if (this.VideoUri.Formula != null)
            {
                model.VideoUriProperty = new AnimatedDynamicPropertyDataModel(((EvalDataViewModelBase)this.VideoUri.Formula).ToDataModel());
            }

            if (this.VideoUri.Animation != null)
            {
                if (model.VideoUriProperty == null)
                {
                    model.VideoUriProperty = new AnimatedDynamicPropertyDataModel();
                }

                model.VideoUriProperty.Animation = new PropertyChangeAnimationDataModel {
                    Duration = ((AnimationDataViewModel)this.VideoUri.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.VideoUri.Animation).Type.Value
                };
            }

            model.Scaling = this.Scaling.Value;
            model.Replay = this.Replay.Value;
            model.FallbackImage = this.FallbackImage.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void VideoUriChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.VideoUri);
        }

        private void ScalingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Scaling);
        }

        private void ReplayChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Replay);
        }

        private void FallbackImageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FallbackImage);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(VideoElementDataModel dataModel = null);

        partial void Initialize(VideoElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(VideoElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref VideoElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class RectangleElementDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private AnimatedDynamicDataValue<string> color;

        public RectangleElementDataViewModel(IMediaShell mediaShell, RectangleElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Color = new AnimatedDynamicDataValue<string>(string.Empty);
            this.Color.PropertyChanged += this.ColorChanged;
            if (dataModel != null)
            {
                if (dataModel.Color != null)
                {
                    this.Color.Value = dataModel.Color;
                }

                if (dataModel.ColorProperty != null)
                {
                    this.Color.Formula = this.CreateEvalDataViewModel(dataModel.ColorProperty.Evaluation);
                    if (dataModel.ColorProperty.Animation != null)
                    {
                        this.Color.Animation = new AnimationDataViewModel(this.mediaShell, dataModel.ColorProperty.Animation);
                    }

                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected RectangleElementDataViewModel(IMediaShell mediaShell, RectangleElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Color = (AnimatedDynamicDataValue<string>)dataViewModel.Color.Clone();
            this.Color.PropertyChanged += this.ColorChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Color.IsDirty;
            }
        }

        [UserVisibleProperty("Content", Filter = "LED", OrderIndex = 0, GroupOrderIndex = 1)]
        public AnimatedDynamicDataValue<string> Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.color);
                if (this.color != null)
                {
                    this.color.PropertyChanged -= this.ColorChanged;
                }

                this.SetProperty(ref this.color, value, () => this.Color);
                if (value != null)
                {
                    this.color.PropertyChanged += this.ColorChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var rectangle = (RectangleElement)this.CreateExportObject();
            this.DoExport(rectangle, parameters);
            return rectangle;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var rectangle = (RectangleElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(rectangle);
            return rectangle;
        }

        public override void ClearDirty()
        {
            if (this.Color != null)
            {
                this.Color.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new RectangleElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is RectangleElementDataViewModel)
            {
                var that = (RectangleElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Color.EqualsValue(that.Color);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new RectangleElement();
        }

        protected override object CreateDataModelObject()
        {
            return new RectangleElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (RectangleElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            if (this.Color.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Color.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var colorEval = Color.Formula as CodeConversionEvalDataViewModel;
                    if (colorEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = colorEval.UseImage.Value ? "{2}" : "{3}";
                            model.ColorProperty = new AnimatedDynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.ColorProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Color.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.ColorProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Color.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Color = this.Color.Value;
                }
            }
            else
            {
                model.Color = this.Color.Value;
            }

            if (this.Color.Animation != null && this.Color.Formula != null)
            {
                if (model.ColorProperty == null)
                {
                    model.ColorProperty = new AnimatedDynamicProperty();
                }

                model.ColorProperty.Animation = new PropertyChangeAnimation {
                    Duration = ((AnimationDataViewModel)this.Color.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Color.Animation).Type.Value
                };
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (RectangleElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Color = this.Color.Value;
            if (this.Color.Formula != null)
            {
                model.ColorProperty = new AnimatedDynamicPropertyDataModel(((EvalDataViewModelBase)this.Color.Formula).ToDataModel());
            }

            if (this.Color.Animation != null)
            {
                if (model.ColorProperty == null)
                {
                    model.ColorProperty = new AnimatedDynamicPropertyDataModel();
                }

                model.ColorProperty.Animation = new PropertyChangeAnimationDataModel {
                    Duration = ((AnimationDataViewModel)this.Color.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Color.Animation).Type.Value
                };
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ColorChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Color);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(RectangleElementDataModel dataModel = null);

        partial void Initialize(RectangleElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(RectangleElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref RectangleElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class AudioElementDataViewModelBase : LayoutElementDataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DynamicDataValue<bool> enabled;

        public AudioElementDataViewModelBase(IMediaShell mediaShell, AudioElementDataModelBase dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Enabled = new DynamicDataValue<bool>(true);
            this.Enabled.PropertyChanged += this.EnabledChanged;
            if (dataModel != null)
            {
                if (dataModel.Enabled != null)
                {
                    this.Enabled.Value = dataModel.Enabled;
                }

                if (dataModel.EnabledProperty != null)
                {
                    this.Enabled.Formula = this.CreateEvalDataViewModel(dataModel.EnabledProperty.Evaluation);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected AudioElementDataViewModelBase(IMediaShell mediaShell, AudioElementDataViewModelBase dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Enabled = (DynamicDataValue<bool>)dataViewModel.Enabled.Clone();
            this.Enabled.PropertyChanged += this.EnabledChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Enabled.IsDirty;
            }
        }

        [UserVisibleProperty("Layout", OrderIndex = 1, GroupOrderIndex = 0)]
        public DynamicDataValue<bool> Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.enabled);
                if (this.enabled != null)
                {
                    this.enabled.PropertyChanged -= this.EnabledChanged;
                }

                this.SetProperty(ref this.enabled, value, () => this.Enabled);
                if (value != null)
                {
                    this.enabled.PropertyChanged += this.EnabledChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public override ElementBase Export(object parameters = null)
        {
            var audiobase = (AudioElementBase)this.CreateExportObject();
            this.DoExport(audiobase, parameters);
            return audiobase;
        }

        public override LayoutElementDataModelBase ToDataModel()
        {
            var audiobase = (AudioElementDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(audiobase);
            return audiobase;
        }

        public override void ClearDirty()
        {
            if (this.Enabled != null)
            {
                this.Enabled.ClearDirty();
            }

            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is AudioElementDataViewModelBase)
            {
                var that = (AudioElementDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Enabled.EqualsValue(that.Enabled);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected abstract object CreateExportObject();

        protected abstract object CreateDataModelObject();

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (AudioElementBase)exportModel;
            if (this.Enabled.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Enabled.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var enabledEval = Enabled.Formula as CodeConversionEvalDataViewModel;
                    if (enabledEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = enabledEval.UseImage.Value ? "{2}" : "{3}";
                            model.EnabledProperty = new DynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.EnabledProperty = new DynamicProperty(((EvalDataViewModelBase)this.Enabled.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.EnabledProperty = new DynamicProperty(((EvalDataViewModelBase)this.Enabled.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Enabled = this.Enabled.Value;
                }
            }
            else
            {
                model.Enabled = this.Enabled.Value;
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (AudioElementDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            model.Enabled = this.Enabled.Value;
            if (this.Enabled.Formula != null)
            {
                model.EnabledProperty = new DynamicPropertyDataModel(((EvalDataViewModelBase)this.Enabled.Formula).ToDataModel());
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void EnabledChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Enabled);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(AudioElementDataModelBase dataModel = null);

        partial void Initialize(AudioElementDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(AudioElementBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref AudioElementDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class AudioOutputElementDataViewModel : AudioElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DynamicDataValue<int> volume;

        private DataValue<int> priority;

        public AudioOutputElementDataViewModel(IMediaShell mediaShell, AudioOutputElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Volume = new DynamicDataValue<int>(default(int));
            this.Volume.PropertyChanged += this.VolumeChanged;
            this.Priority = new DataValue<int>(default(int));
            this.Priority.PropertyChanged += this.PriorityChanged;
            if (dataModel != null)
            {
                if (dataModel.Volume != null)
                {
                    this.Volume.Value = dataModel.Volume;
                }

                if (dataModel.VolumeProperty != null)
                {
                    this.Volume.Formula = this.CreateEvalDataViewModel(dataModel.VolumeProperty.Evaluation);
                }

                this.Priority.Value = dataModel.Priority;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected AudioOutputElementDataViewModel(IMediaShell mediaShell, AudioOutputElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Volume = (DynamicDataValue<int>)dataViewModel.Volume.Clone();
            this.Volume.PropertyChanged += this.VolumeChanged;
            this.Priority = (DataValue<int>)dataViewModel.Priority.Clone();
            this.Priority.PropertyChanged += this.PriorityChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Volume.IsDirty || this.Priority.IsDirty;
            }
        }

        [UserVisibleProperty("Content", Filter = "Audio", OrderIndex = 0, GroupOrderIndex = 1)]
        public DynamicDataValue<int> Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.volume);
                if (this.volume != null)
                {
                    this.volume.PropertyChanged -= this.VolumeChanged;
                }

                this.SetProperty(ref this.volume, value, () => this.Volume);
                if (value != null)
                {
                    this.volume.PropertyChanged += this.VolumeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "Audio", OrderIndex = 1, GroupOrderIndex = 1)]
        public DataValue<int> Priority
        {
            get
            {
                return this.priority;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.priority);
                if (this.priority != null)
                {
                    this.priority.PropertyChanged -= this.PriorityChanged;
                }

                this.SetProperty(ref this.priority, value, () => this.Priority);
                if (value != null)
                {
                    this.priority.PropertyChanged += this.PriorityChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var audiooutput = (AudioOutputElement)this.CreateExportObject();
            this.DoExport(audiooutput, parameters);
            return audiooutput;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var audiooutput = (AudioOutputElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(audiooutput);
            return audiooutput;
        }

        public override void ClearDirty()
        {
            if (this.Volume != null)
            {
                this.Volume.ClearDirty();
            }

            if (this.Priority != null)
            {
                this.Priority.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new AudioOutputElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is AudioOutputElementDataViewModel)
            {
                var that = (AudioOutputElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Volume.EqualsValue(that.Volume);
                        result = result && this.Priority.EqualsValue(that.Priority);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new AudioOutputElement();
        }

        protected override object CreateDataModelObject()
        {
            return new AudioOutputElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (AudioOutputElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            if (this.Volume.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Volume.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var volumeEval = Volume.Formula as CodeConversionEvalDataViewModel;
                    if (volumeEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = volumeEval.UseImage.Value ? "{2}" : "{3}";
                            model.VolumeProperty = new DynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.VolumeProperty = new DynamicProperty(((EvalDataViewModelBase)this.Volume.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.VolumeProperty = new DynamicProperty(((EvalDataViewModelBase)this.Volume.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Volume = this.Volume.Value;
                }
            }
            else
            {
                model.Volume = this.Volume.Value;
            }

            model.Priority = this.Priority.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (AudioOutputElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Volume = this.Volume.Value;
            if (this.Volume.Formula != null)
            {
                model.VolumeProperty = new DynamicPropertyDataModel(((EvalDataViewModelBase)this.Volume.Formula).ToDataModel());
            }

            model.Priority = this.Priority.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void VolumeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Volume);
        }

        private void PriorityChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Priority);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(AudioOutputElementDataModel dataModel = null);

        partial void Initialize(AudioOutputElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(AudioOutputElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref AudioOutputElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class PlaybackElementDataViewModelBase : AudioElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public PlaybackElementDataViewModelBase(IMediaShell mediaShell, PlaybackElementDataModelBase dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected PlaybackElementDataViewModelBase(IMediaShell mediaShell, PlaybackElementDataViewModelBase dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var playbackbase = (PlaybackElementBase)this.CreateExportObject();
            this.DoExport(playbackbase, parameters);
            return playbackbase;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var playbackbase = (PlaybackElementDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(playbackbase);
            return playbackbase;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is PlaybackElementDataViewModelBase)
            {
                var that = (PlaybackElementDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (PlaybackElementBase)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (PlaybackElementDataModelBase)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(PlaybackElementDataModelBase dataModel = null);

        partial void Initialize(PlaybackElementDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(PlaybackElementBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref PlaybackElementDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class AudioFileElementDataViewModel : PlaybackElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DynamicDataValue<string> filename;

        public AudioFileElementDataViewModel(IMediaShell mediaShell, AudioFileElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Filename = new DynamicDataValue<string>(string.Empty);
            this.Filename.PropertyChanged += this.FilenameChanged;
            if (dataModel != null)
            {
                if (dataModel.Filename != null)
                {
                    this.Filename.Value = dataModel.Filename;
                }

                if (dataModel.FilenameProperty != null)
                {
                    this.Filename.Formula = this.CreateEvalDataViewModel(dataModel.FilenameProperty.Evaluation);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected AudioFileElementDataViewModel(IMediaShell mediaShell, AudioFileElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Filename = (DynamicDataValue<string>)dataViewModel.Filename.Clone();
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Filename.IsDirty;
            }
        }

        [UserVisibleProperty("Content", Filter = "Audio", OrderIndex = 0, GroupOrderIndex = 1)]
        public DynamicDataValue<string> Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filename);
                if (this.filename != null)
                {
                    this.filename.PropertyChanged -= this.FilenameChanged;
                }

                this.SetProperty(ref this.filename, value, () => this.Filename);
                if (value != null)
                {
                    this.filename.PropertyChanged += this.FilenameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var audiofile = (AudioFileElement)this.CreateExportObject();
            this.DoExport(audiofile, parameters);
            return audiofile;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var audiofile = (AudioFileElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(audiofile);
            return audiofile;
        }

        public override void ClearDirty()
        {
            if (this.Filename != null)
            {
                this.Filename.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new AudioFileElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is AudioFileElementDataViewModel)
            {
                var that = (AudioFileElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Filename.EqualsValue(that.Filename);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new AudioFileElement();
        }

        protected override object CreateDataModelObject()
        {
            return new AudioFileElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (AudioFileElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            if (this.Filename.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Filename.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var filenameEval = Filename.Formula as CodeConversionEvalDataViewModel;
                    if (filenameEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = filenameEval.UseImage.Value ? "{2}" : "{3}";
                            model.FilenameProperty = new DynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.FilenameProperty = new DynamicProperty(((EvalDataViewModelBase)this.Filename.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.FilenameProperty = new DynamicProperty(((EvalDataViewModelBase)this.Filename.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Filename = this.Filename.Value;
                }
            }
            else
            {
                model.Filename = this.Filename.Value;
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (AudioFileElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Filename = this.Filename.Value;
            if (this.Filename.Formula != null)
            {
                model.FilenameProperty = new DynamicPropertyDataModel(((EvalDataViewModelBase)this.Filename.Formula).ToDataModel());
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FilenameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Filename);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(AudioFileElementDataModel dataModel = null);

        partial void Initialize(AudioFileElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(AudioFileElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref AudioFileElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class AudioPauseElementDataViewModel : PlaybackElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<TimeSpan> duration;

        public AudioPauseElementDataViewModel(IMediaShell mediaShell, AudioPauseElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Duration = new DataValue<TimeSpan>(new TimeSpan());
            this.Duration.PropertyChanged += this.DurationChanged;
            if (dataModel != null)
            {
                this.Duration.Value = dataModel.Duration;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected AudioPauseElementDataViewModel(IMediaShell mediaShell, AudioPauseElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Duration = (DataValue<TimeSpan>)dataViewModel.Duration.Clone();
            this.Duration.PropertyChanged += this.DurationChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Duration.IsDirty;
            }
        }

        [UserVisibleProperty("Content", Filter = "Audio", OrderIndex = 0, GroupOrderIndex = 1)]
        public DataValue<TimeSpan> Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.duration);
                if (this.duration != null)
                {
                    this.duration.PropertyChanged -= this.DurationChanged;
                }

                this.SetProperty(ref this.duration, value, () => this.Duration);
                if (value != null)
                {
                    this.duration.PropertyChanged += this.DurationChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var audiopause = (AudioPauseElement)this.CreateExportObject();
            this.DoExport(audiopause, parameters);
            return audiopause;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var audiopause = (AudioPauseElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(audiopause);
            return audiopause;
        }

        public override void ClearDirty()
        {
            if (this.Duration != null)
            {
                this.Duration.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new AudioPauseElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is AudioPauseElementDataViewModel)
            {
                var that = (AudioPauseElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Duration.EqualsValue(that.Duration);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new AudioPauseElement();
        }

        protected override object CreateDataModelObject()
        {
            return new AudioPauseElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (AudioPauseElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Duration = this.Duration.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (AudioPauseElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Duration = this.Duration.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void DurationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Duration);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(AudioPauseElementDataModel dataModel = null);

        partial void Initialize(AudioPauseElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(AudioPauseElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref AudioPauseElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class TextToSpeechElementDataViewModel : PlaybackElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DynamicDataValue<string> voice;

        private DynamicDataValue<string> value;

        public TextToSpeechElementDataViewModel(IMediaShell mediaShell, TextToSpeechElementDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Voice = new DynamicDataValue<string>(string.Empty);
            this.Voice.PropertyChanged += this.VoiceChanged;
            this.Value = new DynamicDataValue<string>(string.Empty);
            this.Value.PropertyChanged += this.ValueChanged;
            if (dataModel != null)
            {
                if (dataModel.Voice != null)
                {
                    this.Voice.Value = dataModel.Voice;
                }

                if (dataModel.VoiceProperty != null)
                {
                    this.Voice.Formula = this.CreateEvalDataViewModel(dataModel.VoiceProperty.Evaluation);
                }

                if (dataModel.Value != null)
                {
                    this.Value.Value = dataModel.Value;
                }

                if (dataModel.ValueProperty != null)
                {
                    this.Value.Formula = this.CreateEvalDataViewModel(dataModel.ValueProperty.Evaluation);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected TextToSpeechElementDataViewModel(IMediaShell mediaShell, TextToSpeechElementDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Voice = (DynamicDataValue<string>)dataViewModel.Voice.Clone();
            this.Voice.PropertyChanged += this.VoiceChanged;
            this.Value = (DynamicDataValue<string>)dataViewModel.Value.Clone();
            this.Value.PropertyChanged += this.ValueChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Voice.IsDirty || this.Value.IsDirty;
            }
        }

        [UserVisibleProperty("Content", Filter = "Audio", OrderIndex = 1, GroupOrderIndex = 1)]
        public DynamicDataValue<string> Voice
        {
            get
            {
                return this.voice;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.voice);
                if (this.voice != null)
                {
                    this.voice.PropertyChanged -= this.VoiceChanged;
                }

                this.SetProperty(ref this.voice, value, () => this.Voice);
                if (value != null)
                {
                    this.voice.PropertyChanged += this.VoiceChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "Audio", OrderIndex = 0, GroupOrderIndex = 1)]
        public DynamicDataValue<string> Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.value);
                if (this.value != null)
                {
                    this.value.PropertyChanged -= this.ValueChanged;
                }

                this.SetProperty(ref this.value, value, () => this.Value);
                if (value != null)
                {
                    this.value.PropertyChanged += this.ValueChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ElementBase Export(object parameters = null)
        {
            var texttospeech = (TextToSpeechElement)this.CreateExportObject();
            this.DoExport(texttospeech, parameters);
            return texttospeech;
        }

        public new LayoutElementDataModelBase ToDataModel()
        {
            var texttospeech = (TextToSpeechElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(texttospeech);
            return texttospeech;
        }

        public override void ClearDirty()
        {
            if (this.Voice != null)
            {
                this.Voice.ClearDirty();
            }

            if (this.Value != null)
            {
                this.Value.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new TextToSpeechElementDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is TextToSpeechElementDataViewModel)
            {
                var that = (TextToSpeechElementDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Voice.EqualsValue(that.Voice);
                        result = result && this.Value.EqualsValue(that.Value);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new TextToSpeechElement();
        }

        protected override object CreateDataModelObject()
        {
            return new TextToSpeechElementDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (TextToSpeechElement)exportModel;
            base.DoExport(exportModel, exportParameters);
            if (this.Voice.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Voice.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var voiceEval = Voice.Formula as CodeConversionEvalDataViewModel;
                    if (voiceEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = voiceEval.UseImage.Value ? "{2}" : "{3}";
                            model.VoiceProperty = new DynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.VoiceProperty = new DynamicProperty(((EvalDataViewModelBase)this.Voice.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.VoiceProperty = new DynamicProperty(((EvalDataViewModelBase)this.Voice.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Voice = this.Voice.Value;
                }
            }
            else
            {
                model.Voice = this.Voice.Value;
            }

            if (this.Value.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Value.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var valueEval = Value.Formula as CodeConversionEvalDataViewModel;
                    if (valueEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = valueEval.UseImage.Value ? "{2}" : "{3}";
                            model.ValueProperty = new DynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.ValueProperty = new DynamicProperty(((EvalDataViewModelBase)this.Value.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.ValueProperty = new DynamicProperty(((EvalDataViewModelBase)this.Value.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Value = this.Value.Value;
                }
            }
            else
            {
                model.Value = this.Value.Value;
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (TextToSpeechElementDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Voice = this.Voice.Value;
            if (this.Voice.Formula != null)
            {
                model.VoiceProperty = new DynamicPropertyDataModel(((EvalDataViewModelBase)this.Voice.Formula).ToDataModel());
            }

            model.Value = this.Value.Value;
            if (this.Value.Formula != null)
            {
                model.ValueProperty = new DynamicPropertyDataModel(((EvalDataViewModelBase)this.Value.Formula).ToDataModel());
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void VoiceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Voice);
        }

        private void ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Value);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(TextToSpeechElementDataModel dataModel = null);

        partial void Initialize(TextToSpeechElementDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(TextToSpeechElement model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref TextToSpeechElementDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

}
namespace Gorba.Center.Media.Core.DataViewModels.Presentation
{
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Presentation.Cycle;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Media.Core.Controllers;
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.ComponentModel;
    using System.Linq;
    public partial class InfomediaConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<Version> version;

        private DataValue<DateTime> creationdate;

        private MasterPresentationConfigDataViewModel masterpresentation;

        private CyclesConfigDataViewModel cycles;

        private ExtendedObservableCollection<PhysicalScreenConfigDataViewModel> physicalscreens;

        private ExtendedObservableCollection<VirtualDisplayConfigDataViewModel> virtualdisplays;

        private ExtendedObservableCollection<EvaluationConfigDataViewModel> evaluations;

        private ExtendedObservableCollection<CyclePackageConfigDataViewModel> cyclepackages;

        private ExtendedObservableCollection<PoolConfigDataViewModel> pools;

        private ExtendedObservableCollection<LayoutConfigDataViewModel> layouts;

        private ExtendedObservableCollection<FontConfigDataViewModel> fonts;

        public InfomediaConfigDataViewModel(IMediaShell mediaShell, InfomediaConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Version = new DataValue<Version>(new Version());
            this.Version.PropertyChanged += this.VersionChanged;
            this.CreationDate = new DataValue<DateTime>(new DateTime());
            this.CreationDate.PropertyChanged += this.CreationDateChanged;
            this.PhysicalScreens = new ExtendedObservableCollection<PhysicalScreenConfigDataViewModel>();
            this.VirtualDisplays = new ExtendedObservableCollection<VirtualDisplayConfigDataViewModel>();
            this.masterpresentation = new MasterPresentationConfigDataViewModel(this.mediaShell);
            this.Evaluations = new ExtendedObservableCollection<EvaluationConfigDataViewModel>();
            this.cycles = new CyclesConfigDataViewModel(this.mediaShell);
            this.CyclePackages = new ExtendedObservableCollection<CyclePackageConfigDataViewModel>();
            this.Pools = new ExtendedObservableCollection<PoolConfigDataViewModel>();
            this.Layouts = new ExtendedObservableCollection<LayoutConfigDataViewModel>();
            this.Fonts = new ExtendedObservableCollection<FontConfigDataViewModel>();
            if (dataModel != null)
            {
                this.Version.Value = dataModel.Version;
                this.CreationDate.Value = dataModel.CreationDate;
                foreach (var item in dataModel.PhysicalScreens)
                {
                    var convertedItem = new PhysicalScreenConfigDataViewModel(mediaShell, item);
                    this.PhysicalScreens.Add(convertedItem);
                }

                foreach (var item in dataModel.VirtualDisplays)
                {
                    var convertedItem = new VirtualDisplayConfigDataViewModel(mediaShell, item);
                    this.VirtualDisplays.Add(convertedItem);
                }

                this.masterpresentation = new MasterPresentationConfigDataViewModel(this.mediaShell, dataModel.MasterPresentation);
                foreach (var item in dataModel.Evaluations)
                {
                    var convertedItem = new EvaluationConfigDataViewModel(mediaShell, item);
                    this.Evaluations.Add(convertedItem);
                }

                this.cycles = new CyclesConfigDataViewModel(this.mediaShell, dataModel.Cycles);
                foreach (var item in dataModel.CyclePackages)
                {
                    var convertedItem = new CyclePackageConfigDataViewModel(mediaShell, item);
                    this.CyclePackages.Add(convertedItem);
                }

                foreach (var item in dataModel.Pools)
                {
                    var convertedItem = new PoolConfigDataViewModel(mediaShell, item);
                    this.Pools.Add(convertedItem);
                }

                foreach (var item in dataModel.Layouts)
                {
                    var convertedItem = new LayoutConfigDataViewModel(mediaShell, item);
                    this.Layouts.Add(convertedItem);
                }

                foreach (var item in dataModel.Fonts)
                {
                    var convertedItem = new FontConfigDataViewModel(mediaShell, item);
                    this.Fonts.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected InfomediaConfigDataViewModel(IMediaShell mediaShell, InfomediaConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Version = (DataValue<Version>)dataViewModel.Version.Clone();
            this.Version.PropertyChanged += this.VersionChanged;
            this.CreationDate = (DataValue<DateTime>)dataViewModel.CreationDate.Clone();
            this.CreationDate.PropertyChanged += this.CreationDateChanged;
            this.PhysicalScreens = new ExtendedObservableCollection<PhysicalScreenConfigDataViewModel>();
            foreach (var item in dataViewModel.PhysicalScreens)
            {
                PhysicalScreenConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (PhysicalScreenConfigDataViewModel)item.Clone();
                }

                this.PhysicalScreens.Add(clonedItem);
            }

            this.VirtualDisplays = new ExtendedObservableCollection<VirtualDisplayConfigDataViewModel>();
            foreach (var item in dataViewModel.VirtualDisplays)
            {
                VirtualDisplayConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (VirtualDisplayConfigDataViewModel)item.Clone();
                }

                this.VirtualDisplays.Add(clonedItem);
            }

            var clonedMasterPresentation = dataViewModel.MasterPresentation;
            if (clonedMasterPresentation != null)
            {
                this.MasterPresentation = (MasterPresentationConfigDataViewModel)clonedMasterPresentation.Clone();
            }

            this.Evaluations = new ExtendedObservableCollection<EvaluationConfigDataViewModel>();
            foreach (var item in dataViewModel.Evaluations)
            {
                EvaluationConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (EvaluationConfigDataViewModel)item.Clone();
                }

                this.Evaluations.Add(clonedItem);
            }

            var clonedCycles = dataViewModel.Cycles;
            if (clonedCycles != null)
            {
                this.Cycles = (CyclesConfigDataViewModel)clonedCycles.Clone();
            }

            this.CyclePackages = new ExtendedObservableCollection<CyclePackageConfigDataViewModel>();
            foreach (var item in dataViewModel.CyclePackages)
            {
                CyclePackageConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (CyclePackageConfigDataViewModel)item.Clone();
                }

                this.CyclePackages.Add(clonedItem);
            }

            this.Pools = new ExtendedObservableCollection<PoolConfigDataViewModel>();
            foreach (var item in dataViewModel.Pools)
            {
                PoolConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (PoolConfigDataViewModel)item.Clone();
                }

                this.Pools.Add(clonedItem);
            }

            this.Layouts = new ExtendedObservableCollection<LayoutConfigDataViewModel>();
            foreach (var item in dataViewModel.Layouts)
            {
                LayoutConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (LayoutConfigDataViewModel)item.Clone();
                }

                this.Layouts.Add(clonedItem);
            }

            this.Fonts = new ExtendedObservableCollection<FontConfigDataViewModel>();
            foreach (var item in dataViewModel.Fonts)
            {
                FontConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (FontConfigDataViewModel)item.Clone();
                }

                this.Fonts.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Version.IsDirty || this.CreationDate.IsDirty || this.PhysicalScreens.IsDirty || this.VirtualDisplays.IsDirty || this.Evaluations.IsDirty || this.CyclePackages.IsDirty || this.Pools.IsDirty || this.Layouts.IsDirty || this.Fonts.IsDirty || (this.MasterPresentation != null && this.MasterPresentation.IsDirty) || (this.Cycles != null && this.Cycles.IsDirty);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<Version> Version
        {
            get
            {
                return this.version;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.version);
                if (this.version != null)
                {
                    this.version.PropertyChanged -= this.VersionChanged;
                }

                this.SetProperty(ref this.version, value, () => this.Version);
                if (value != null)
                {
                    this.version.PropertyChanged += this.VersionChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<DateTime> CreationDate
        {
            get
            {
                return this.creationdate;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.creationdate);
                if (this.creationdate != null)
                {
                    this.creationdate.PropertyChanged -= this.CreationDateChanged;
                }

                this.SetProperty(ref this.creationdate, value, () => this.CreationDate);
                if (value != null)
                {
                    this.creationdate.PropertyChanged += this.CreationDateChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<PhysicalScreenConfigDataViewModel> PhysicalScreens
        {
            get
            {
                return this.physicalscreens;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.physicalscreens);
                this.SetProperty(ref this.physicalscreens, value, () => this.PhysicalScreens);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<VirtualDisplayConfigDataViewModel> VirtualDisplays
        {
            get
            {
                return this.virtualdisplays;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.virtualdisplays);
                this.SetProperty(ref this.virtualdisplays, value, () => this.VirtualDisplays);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public MasterPresentationConfigDataViewModel MasterPresentation
        {
            get
            {
                return this.masterpresentation;
            }
            set
            {
                if (this.masterpresentation != null)
                {
                    this.masterpresentation.PropertyChanged -= this.MasterPresentationChanged;
                }

                this.SetProperty(ref this.masterpresentation, value, () => this.MasterPresentation);
                if (value != null)
                {
                    this.masterpresentation.PropertyChanged += this.MasterPresentationChanged;
                }

            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<EvaluationConfigDataViewModel> Evaluations
        {
            get
            {
                return this.evaluations;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.evaluations);
                this.SetProperty(ref this.evaluations, value, () => this.Evaluations);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public CyclesConfigDataViewModel Cycles
        {
            get
            {
                return this.cycles;
            }
            set
            {
                if (this.cycles != null)
                {
                    this.cycles.PropertyChanged -= this.CyclesChanged;
                }

                this.SetProperty(ref this.cycles, value, () => this.Cycles);
                if (value != null)
                {
                    this.cycles.PropertyChanged += this.CyclesChanged;
                }

            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<CyclePackageConfigDataViewModel> CyclePackages
        {
            get
            {
                return this.cyclepackages;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.cyclepackages);
                this.SetProperty(ref this.cyclepackages, value, () => this.CyclePackages);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<PoolConfigDataViewModel> Pools
        {
            get
            {
                return this.pools;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.pools);
                this.SetProperty(ref this.pools, value, () => this.Pools);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<LayoutConfigDataViewModel> Layouts
        {
            get
            {
                return this.layouts;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.layouts);
                this.SetProperty(ref this.layouts, value, () => this.Layouts);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<FontConfigDataViewModel> Fonts
        {
            get
            {
                return this.fonts;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.fonts);
                this.SetProperty(ref this.fonts, value, () => this.Fonts);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public InfomediaConfig Export(object parameters = null)
        {
            var infomedia = (InfomediaConfig)this.CreateExportObject();
            this.DoExport(infomedia, parameters);
            return infomedia;
        }

        public InfomediaConfigDataModel ToDataModel()
        {
            var infomedia = (InfomediaConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(infomedia);
            return infomedia;
        }

        public override void ClearDirty()
        {
            if (this.Version != null)
            {
                this.Version.ClearDirty();
            }

            if (this.CreationDate != null)
            {
                this.CreationDate.ClearDirty();
            }

            if (this.PhysicalScreens != null)
            {
                this.PhysicalScreens.ClearDirty();
            }

            if (this.VirtualDisplays != null)
            {
                this.VirtualDisplays.ClearDirty();
            }

            if (this.MasterPresentation != null)
            {
                this.MasterPresentation.ClearDirty();
            }

            if (this.Evaluations != null)
            {
                this.Evaluations.ClearDirty();
            }

            if (this.Cycles != null)
            {
                this.Cycles.ClearDirty();
            }

            if (this.CyclePackages != null)
            {
                this.CyclePackages.ClearDirty();
            }

            if (this.Pools != null)
            {
                this.Pools.ClearDirty();
            }

            if (this.Layouts != null)
            {
                this.Layouts.ClearDirty();
            }

            if (this.Fonts != null)
            {
                this.Fonts.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new InfomediaConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is InfomediaConfigDataViewModel)
            {
                var that = (InfomediaConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Version.EqualsValue(that.Version);
                        result = result && this.CreationDate.EqualsValue(that.CreationDate);
                        result = result && this.PhysicalScreens.Count == that.PhysicalScreens.Count;
                        if (result)
                        {
                            foreach (var item in this.PhysicalScreens)
                            {
                                var found = false;
                                foreach (var otherItem in that.PhysicalScreens)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        result = result && this.VirtualDisplays.Count == that.VirtualDisplays.Count;
                        if (result)
                        {
                            foreach (var item in this.VirtualDisplays)
                            {
                                var found = false;
                                foreach (var otherItem in that.VirtualDisplays)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        if (this.MasterPresentation != null)
                        {
                            result = result && this.MasterPresentation.EqualsViewModel(that.MasterPresentation);
                        }

                        result = result && this.Evaluations.Count == that.Evaluations.Count;
                        if (result)
                        {
                            foreach (var item in this.Evaluations)
                            {
                                var found = false;
                                foreach (var otherItem in that.Evaluations)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        if (this.Cycles != null)
                        {
                            result = result && this.Cycles.EqualsViewModel(that.Cycles);
                        }

                        result = result && this.CyclePackages.Count == that.CyclePackages.Count;
                        if (result)
                        {
                            foreach (var item in this.CyclePackages)
                            {
                                var found = false;
                                foreach (var otherItem in that.CyclePackages)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        result = result && this.Pools.Count == that.Pools.Count;
                        if (result)
                        {
                            foreach (var item in this.Pools)
                            {
                                var found = false;
                                foreach (var otherItem in that.Pools)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        result = result && this.Layouts.Count == that.Layouts.Count;
                        if (result)
                        {
                            foreach (var item in this.Layouts)
                            {
                                var found = false;
                                foreach (var otherItem in that.Layouts)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        result = result && this.Fonts.Count == that.Fonts.Count;
                        if (result)
                        {
                            foreach (var item in this.Fonts)
                            {
                                var found = false;
                                foreach (var otherItem in that.Fonts)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new InfomediaConfig();
        }

        protected object CreateDataModelObject()
        {
            return new InfomediaConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (InfomediaConfig)exportModel;
            model.Version = this.Version.Value;
            model.CreationDate = this.CreationDate.Value;
            foreach (var item in this.PhysicalScreens)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.PhysicalScreens.Add(convertedItem);
                }

            }

            foreach (var item in this.VirtualDisplays)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.VirtualDisplays.Add(convertedItem);
                }

            }

            if (this.MasterPresentation != null)
            {
                model.MasterPresentation = this.MasterPresentation.Export(exportParameters);
            }

            foreach (var item in this.Evaluations)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Evaluations.Add(convertedItem);
                }

            }

            if (this.Cycles != null)
            {
                model.Cycles = this.Cycles.Export(exportParameters);
            }

            foreach (var item in this.CyclePackages)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.CyclePackages.Add(convertedItem);
                }

            }

            foreach (var item in this.Pools)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Pools.Add(convertedItem);
                }

            }

            foreach (var item in this.Layouts)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Layouts.Add(convertedItem);
                }

            }

            foreach (var item in this.Fonts)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Fonts.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (InfomediaConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Version = this.Version.Value;
            model.CreationDate = this.CreationDate.Value;
            foreach (var item in this.PhysicalScreens)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.PhysicalScreens.Add(convertedItem);
                }

            }

            foreach (var item in this.VirtualDisplays)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.VirtualDisplays.Add(convertedItem);
                }

            }

            if (this.MasterPresentation != null)
            {
                model.MasterPresentation = this.MasterPresentation.ToDataModel();
            }

            foreach (var item in this.Evaluations)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Evaluations.Add(convertedItem);
                }

            }

            if (this.Cycles != null)
            {
                model.Cycles = this.Cycles.ToDataModel();
            }

            foreach (var item in this.CyclePackages)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.CyclePackages.Add(convertedItem);
                }

            }

            foreach (var item in this.Pools)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Pools.Add(convertedItem);
                }

            }

            foreach (var item in this.Layouts)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Layouts.Add(convertedItem);
                }

            }

            foreach (var item in this.Fonts)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Fonts.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void VersionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Version);
        }

        private void CreationDateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.CreationDate);
        }

        private void MasterPresentationChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MasterPresentationChangedPartial(sender, e);
        }

        private void CyclesChanged(object sender, PropertyChangedEventArgs e)
        {
            this.CyclesChangedPartial(sender, e);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void MasterPresentationChangedPartial(object sender, PropertyChangedEventArgs e);

        partial void CyclesChangedPartial(object sender, PropertyChangedEventArgs e);

        partial void Initialize(InfomediaConfigDataModel dataModel = null);

        partial void Initialize(InfomediaConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(InfomediaConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref InfomediaConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class CyclePackageConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> name;

        private ExtendedObservableCollection<StandardCycleRefConfigDataViewModel> standardcycles;

        private ExtendedObservableCollection<EventCycleRefConfigDataViewModel> eventcycles;

        public CyclePackageConfigDataViewModel(IMediaShell mediaShell, CyclePackageConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Name = new DataValue<string>(string.Empty);
            this.Name.PropertyChanged += this.NameChanged;
            this.StandardCycles = new ExtendedObservableCollection<StandardCycleRefConfigDataViewModel>();
            this.EventCycles = new ExtendedObservableCollection<EventCycleRefConfigDataViewModel>();
            if (dataModel != null)
            {
                this.Name.Value = dataModel.Name;
                foreach (var item in dataModel.StandardCycles)
                {
                    var convertedItem = new StandardCycleRefConfigDataViewModel(mediaShell, item);
                    this.StandardCycles.Add(convertedItem);
                }

                foreach (var item in dataModel.EventCycles)
                {
                    var convertedItem = new EventCycleRefConfigDataViewModel(mediaShell, item);
                    this.EventCycles.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected CyclePackageConfigDataViewModel(IMediaShell mediaShell, CyclePackageConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Name = (DataValue<string>)dataViewModel.Name.Clone();
            this.Name.PropertyChanged += this.NameChanged;
            this.StandardCycles = new ExtendedObservableCollection<StandardCycleRefConfigDataViewModel>();
            foreach (var item in dataViewModel.StandardCycles)
            {
                StandardCycleRefConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (StandardCycleRefConfigDataViewModel)item.Clone();
                }

                this.StandardCycles.Add(clonedItem);
            }

            this.EventCycles = new ExtendedObservableCollection<EventCycleRefConfigDataViewModel>();
            foreach (var item in dataViewModel.EventCycles)
            {
                EventCycleRefConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (EventCycleRefConfigDataViewModel)item.Clone();
                }

                this.EventCycles.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Name.IsDirty || this.StandardCycles.IsDirty || this.EventCycles.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.name);
                if (this.name != null)
                {
                    this.name.PropertyChanged -= this.NameChanged;
                }

                this.SetProperty(ref this.name, value, () => this.Name);
                if (value != null)
                {
                    this.name.PropertyChanged += this.NameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public ExtendedObservableCollection<StandardCycleRefConfigDataViewModel> StandardCycles
        {
            get
            {
                return this.standardcycles;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.standardcycles);
                this.SetProperty(ref this.standardcycles, value, () => this.StandardCycles);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public ExtendedObservableCollection<EventCycleRefConfigDataViewModel> EventCycles
        {
            get
            {
                return this.eventcycles;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.eventcycles);
                this.SetProperty(ref this.eventcycles, value, () => this.EventCycles);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public CyclePackageConfig Export(object parameters = null)
        {
            var cyclepackage = (CyclePackageConfig)this.CreateExportObject();
            this.DoExport(cyclepackage, parameters);
            return cyclepackage;
        }

        public CyclePackageConfigDataModel ToDataModel()
        {
            var cyclepackage = (CyclePackageConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(cyclepackage);
            return cyclepackage;
        }

        public override void ClearDirty()
        {
            if (this.Name != null)
            {
                this.Name.ClearDirty();
            }

            if (this.StandardCycles != null)
            {
                this.StandardCycles.ClearDirty();
            }

            if (this.EventCycles != null)
            {
                this.EventCycles.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new CyclePackageConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is CyclePackageConfigDataViewModel)
            {
                var that = (CyclePackageConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Name.EqualsValue(that.Name);
                        result = result && this.StandardCycles.Count == that.StandardCycles.Count;
                        if (result)
                        {
                            foreach (var item in this.StandardCycles)
                            {
                                var found = false;
                                foreach (var otherItem in that.StandardCycles)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        result = result && this.EventCycles.Count == that.EventCycles.Count;
                        if (result)
                        {
                            foreach (var item in this.EventCycles)
                            {
                                var found = false;
                                foreach (var otherItem in that.EventCycles)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new CyclePackageConfig();
        }

        protected object CreateDataModelObject()
        {
            return new CyclePackageConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (CyclePackageConfig)exportModel;
            model.Name = this.Name.Value;
            foreach (var item in this.StandardCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.StandardCycles.Add(convertedItem);
                }

            }

            foreach (var item in this.EventCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.EventCycles.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CyclePackageConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Name = this.Name.Value;
            foreach (var item in this.StandardCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.StandardCycles.Add(convertedItem);
                }

            }

            foreach (var item in this.EventCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.EventCycles.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Name);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(CyclePackageConfigDataModel dataModel = null);

        partial void Initialize(CyclePackageConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(CyclePackageConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref CyclePackageConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class CyclesConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private ExtendedObservableCollection<StandardCycleConfigDataViewModel> standardcycles;

        private ExtendedObservableCollection<EventCycleConfigDataViewModel> eventcycles;

        public CyclesConfigDataViewModel(IMediaShell mediaShell, CyclesConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.StandardCycles = new ExtendedObservableCollection<StandardCycleConfigDataViewModel>();
            this.EventCycles = new ExtendedObservableCollection<EventCycleConfigDataViewModel>();
            if (dataModel != null)
            {
                foreach (var item in dataModel.StandardCycles)
                {
                    var convertedItem = new StandardCycleConfigDataViewModel(mediaShell, item);
                    this.StandardCycles.Add(convertedItem);
                }

                foreach (var item in dataModel.EventCycles)
                {
                    var convertedItem = new EventCycleConfigDataViewModel(mediaShell, item);
                    this.EventCycles.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected CyclesConfigDataViewModel(IMediaShell mediaShell, CyclesConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.StandardCycles = new ExtendedObservableCollection<StandardCycleConfigDataViewModel>();
            foreach (var item in dataViewModel.StandardCycles)
            {
                StandardCycleConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (StandardCycleConfigDataViewModel)item.Clone();
                }

                this.StandardCycles.Add(clonedItem);
            }

            this.EventCycles = new ExtendedObservableCollection<EventCycleConfigDataViewModel>();
            foreach (var item in dataViewModel.EventCycles)
            {
                EventCycleConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (EventCycleConfigDataViewModel)item.Clone();
                }

                this.EventCycles.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.StandardCycles.IsDirty || this.EventCycles.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<StandardCycleConfigDataViewModel> StandardCycles
        {
            get
            {
                return this.standardcycles;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.standardcycles);
                this.SetProperty(ref this.standardcycles, value, () => this.StandardCycles);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<EventCycleConfigDataViewModel> EventCycles
        {
            get
            {
                return this.eventcycles;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.eventcycles);
                this.SetProperty(ref this.eventcycles, value, () => this.EventCycles);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public CyclesConfig Export(object parameters = null)
        {
            var cycles = (CyclesConfig)this.CreateExportObject();
            this.DoExport(cycles, parameters);
            return cycles;
        }

        public CyclesConfigDataModel ToDataModel()
        {
            var cycles = (CyclesConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(cycles);
            return cycles;
        }

        public override void ClearDirty()
        {
            if (this.StandardCycles != null)
            {
                this.StandardCycles.ClearDirty();
            }

            if (this.EventCycles != null)
            {
                this.EventCycles.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new CyclesConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is CyclesConfigDataViewModel)
            {
                var that = (CyclesConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.StandardCycles.Count == that.StandardCycles.Count;
                        if (result)
                        {
                            foreach (var item in this.StandardCycles)
                            {
                                var found = false;
                                foreach (var otherItem in that.StandardCycles)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        result = result && this.EventCycles.Count == that.EventCycles.Count;
                        if (result)
                        {
                            foreach (var item in this.EventCycles)
                            {
                                var found = false;
                                foreach (var otherItem in that.EventCycles)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new CyclesConfig();
        }

        protected object CreateDataModelObject()
        {
            return new CyclesConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (CyclesConfig)exportModel;
            foreach (var item in this.StandardCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.StandardCycles.Add(convertedItem);
                }

            }

            foreach (var item in this.EventCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.EventCycles.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CyclesConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            foreach (var item in this.StandardCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.StandardCycles.Add(convertedItem);
                }

            }

            foreach (var item in this.EventCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.EventCycles.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(CyclesConfigDataModel dataModel = null);

        partial void Initialize(CyclesConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(CyclesConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref CyclesConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class EvaluationConfigDataViewModel : ContainerEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> name;

        public EvaluationConfigDataViewModel(IMediaShell mediaShell, EvaluationConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Name = new DataValue<string>(string.Empty);
            this.Name.PropertyChanged += this.NameChanged;
            if (dataModel != null)
            {
                this.Name.Value = dataModel.Name;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected EvaluationConfigDataViewModel(IMediaShell mediaShell, EvaluationConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Name = (DataValue<string>)dataViewModel.Name.Clone();
            this.Name.PropertyChanged += this.NameChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Name.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.name);
                if (this.name != null)
                {
                    this.name.PropertyChanged -= this.NameChanged;
                }

                this.SetProperty(ref this.name, value, () => this.Name);
                if (value != null)
                {
                    this.name.PropertyChanged += this.NameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new EvaluationConfig Export(object parameters = null)
        {
            var evaluation = (EvaluationConfig)this.CreateExportObject();
            this.DoExport(evaluation, parameters);
            return evaluation;
        }

        public new EvaluationConfigDataModel ToDataModel()
        {
            var evaluation = (EvaluationConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(evaluation);
            return evaluation;
        }

        public override void ClearDirty()
        {
            if (this.Name != null)
            {
                this.Name.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new EvaluationConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is EvaluationConfigDataViewModel)
            {
                var that = (EvaluationConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Name.EqualsValue(that.Name);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new EvaluationConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new EvaluationConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (EvaluationConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Name = this.Name.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (EvaluationConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Name = this.Name.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Name);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(EvaluationConfigDataModel dataModel = null);

        partial void Initialize(EvaluationConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(EvaluationConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref EvaluationConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class FontConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> path;

        private DataValue<PhysicalScreenType> screentype;

        public FontConfigDataViewModel(IMediaShell mediaShell, FontConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Path = new DataValue<string>(string.Empty);
            this.Path.PropertyChanged += this.PathChanged;
            this.ScreenType = new DataValue<PhysicalScreenType>(PhysicalScreenType.TFT);
            this.ScreenType.PropertyChanged += this.ScreenTypeChanged;
            if (dataModel != null)
            {
                this.Path.Value = dataModel.Path;
                this.ScreenType.Value = dataModel.ScreenType;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected FontConfigDataViewModel(IMediaShell mediaShell, FontConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Path = (DataValue<string>)dataViewModel.Path.Clone();
            this.Path.PropertyChanged += this.PathChanged;
            this.ScreenType = (DataValue<PhysicalScreenType>)dataViewModel.ScreenType.Clone();
            this.ScreenType.PropertyChanged += this.ScreenTypeChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Path.IsDirty || this.ScreenType.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.path);
                if (this.path != null)
                {
                    this.path.PropertyChanged -= this.PathChanged;
                }

                this.SetProperty(ref this.path, value, () => this.Path);
                if (value != null)
                {
                    this.path.PropertyChanged += this.PathChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<PhysicalScreenType> ScreenType
        {
            get
            {
                return this.screentype;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.screentype);
                if (this.screentype != null)
                {
                    this.screentype.PropertyChanged -= this.ScreenTypeChanged;
                }

                this.SetProperty(ref this.screentype, value, () => this.ScreenType);
                if (value != null)
                {
                    this.screentype.PropertyChanged += this.ScreenTypeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public FontConfig Export(object parameters = null)
        {
            var font = (FontConfig)this.CreateExportObject();
            this.DoExport(font, parameters);
            return font;
        }

        public FontConfigDataModel ToDataModel()
        {
            var font = (FontConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(font);
            return font;
        }

        public override void ClearDirty()
        {
            if (this.Path != null)
            {
                this.Path.ClearDirty();
            }

            if (this.ScreenType != null)
            {
                this.ScreenType.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new FontConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is FontConfigDataViewModel)
            {
                var that = (FontConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Path.EqualsValue(that.Path);
                        result = result && this.ScreenType.EqualsValue(that.ScreenType);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new FontConfig();
        }

        protected object CreateDataModelObject()
        {
            return new FontConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (FontConfig)exportModel;
            model.Path = this.Path.Value;
            model.ScreenType = this.ScreenType.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (FontConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Path = this.Path.Value;
            model.ScreenType = this.ScreenType.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void PathChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Path);
        }

        private void ScreenTypeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.ScreenType);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(FontConfigDataModel dataModel = null);

        partial void Initialize(FontConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(FontConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref FontConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class LayoutConfigDataViewModelBase : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> name;

        public LayoutConfigDataViewModelBase(IMediaShell mediaShell, LayoutConfigDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Name = new DataValue<string>(string.Empty);
            this.Name.PropertyChanged += this.NameChanged;
            if (dataModel != null)
            {
                this.Name.Value = dataModel.Name;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected LayoutConfigDataViewModelBase(IMediaShell mediaShell, LayoutConfigDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Name = (DataValue<string>)dataViewModel.Name.Clone();
            this.Name.PropertyChanged += this.NameChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Name.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.name);
                if (this.name != null)
                {
                    this.name.PropertyChanged -= this.NameChanged;
                }

                this.SetProperty(ref this.name, value, () => this.Name);
                if (value != null)
                {
                    this.name.PropertyChanged += this.NameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public LayoutConfigBase Export(object parameters = null)
        {
            var layoutbase = (LayoutConfigBase)this.CreateExportObject();
            this.DoExport(layoutbase, parameters);
            return layoutbase;
        }

        public LayoutConfigDataModelBase ToDataModel()
        {
            var layoutbase = (LayoutConfigDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(layoutbase);
            return layoutbase;
        }

        public override void ClearDirty()
        {
            if (this.Name != null)
            {
                this.Name.ClearDirty();
            }

            base.ClearDirty();
        }

        public abstract object Clone();

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is LayoutConfigDataViewModelBase)
            {
                var that = (LayoutConfigDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Name.EqualsValue(that.Name);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected abstract object CreateExportObject();

        protected abstract object CreateDataModelObject();

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (LayoutConfigBase)exportModel;
            model.Name = this.Name.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (LayoutConfigDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            model.Name = this.Name.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Name);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(LayoutConfigDataModelBase dataModel = null);

        partial void Initialize(LayoutConfigDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(LayoutConfigBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref LayoutConfigDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class LayoutConfigDataViewModel : LayoutConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private string baselayoutnameReferenceName;

        private LayoutConfigDataViewModel baselayoutnameReference;

        private ExtendedObservableCollection<ResolutionConfigDataViewModel> resolutions;

        public LayoutConfigDataViewModel(IMediaShell mediaShell, LayoutConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            // BaseLayoutName
            this.Resolutions = new ExtendedObservableCollection<ResolutionConfigDataViewModel>();
            if (dataModel != null)
            {
                this.baselayoutnameReferenceName = dataModel.BaseLayoutName;
                foreach (var item in dataModel.Resolutions)
                {
                    var convertedItem = new ResolutionConfigDataViewModel(mediaShell, item);
                    this.Resolutions.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected LayoutConfigDataViewModel(IMediaShell mediaShell, LayoutConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.BaseLayoutNameName = dataViewModel.BaseLayoutNameName;
            this.Resolutions = new ExtendedObservableCollection<ResolutionConfigDataViewModel>();
            foreach (var item in dataViewModel.Resolutions)
            {
                ResolutionConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (ResolutionConfigDataViewModel)item.Clone();
                }

                this.Resolutions.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Resolutions.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public LayoutConfigDataViewModel BaseLayoutName
        {
            get
            {
                if (this.baselayoutnameReference == null)
                {
                    this.baselayoutnameReference = this.FindReference();
                    if (this.baselayoutnameReference != null)
                    {
                        this.baselayoutnameReference.PropertyChanged += this.BaseLayoutNameChanged;
                    }

                }

                return this.baselayoutnameReference;
            }
            set
            {
                if (this.BaseLayoutName != null)
                {
                    this.BaseLayoutName.PropertyChanged -= this.BaseLayoutNameChanged;
                }

                this.SetProperty(ref this.baselayoutnameReference, value, () => this.BaseLayoutName);
                if (value != null)
                {
                    this.BaseLayoutNameName = value.Name.Value;
                    this.BaseLayoutName.PropertyChanged += this.BaseLayoutNameChanged;
                }

            }
        }

        public string BaseLayoutNameName
        {
            get
            {
                return this.baselayoutnameReferenceName;
            }
            private set
            {
                this.SetProperty(ref this.baselayoutnameReferenceName, value, () => this.BaseLayoutName);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<ResolutionConfigDataViewModel> Resolutions
        {
            get
            {
                return this.resolutions;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.resolutions);
                this.SetProperty(ref this.resolutions, value, () => this.Resolutions);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new LayoutConfig Export(object parameters = null)
        {
            var layout = (LayoutConfig)this.CreateExportObject();
            this.DoExport(layout, parameters);
            return layout;
        }

        public new LayoutConfigDataModel ToDataModel()
        {
            var layout = (LayoutConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(layout);
            return layout;
        }

        public override void ClearDirty()
        {
            if (this.BaseLayoutName != null)
            {
                this.BaseLayoutName.ClearDirty();
            }

            if (this.Resolutions != null)
            {
                this.Resolutions.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new LayoutConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is LayoutConfigDataViewModel)
            {
                var that = (LayoutConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.BaseLayoutName.EqualsViewModel(that.BaseLayoutName);
                        result = result && this.Resolutions.Count == that.Resolutions.Count;
                        if (result)
                        {
                            foreach (var item in this.Resolutions)
                            {
                                var found = false;
                                foreach (var otherItem in that.Resolutions)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new LayoutConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new LayoutConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (LayoutConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.BaseLayoutName = this.BaseLayoutNameName;
            foreach (var item in this.Resolutions)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Resolutions.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (LayoutConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.BaseLayoutName = this.BaseLayoutNameName;
            foreach (var item in this.Resolutions)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Resolutions.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void BaseLayoutNameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.BaseLayoutName != null)
            {
                this.BaseLayoutNameName = this.BaseLayoutName.Name.Value;
            }

            this.RaisePropertyChanged(() => this.BaseLayoutName);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(LayoutConfigDataModel dataModel = null);

        partial void Initialize(LayoutConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(LayoutConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref LayoutConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class MasterLayoutConfigDataViewModel : LayoutConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private ExtendedObservableCollection<PhysicalScreenRefConfigDataViewModel> physicalscreens;

        public MasterLayoutConfigDataViewModel(IMediaShell mediaShell, MasterLayoutConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.PhysicalScreens = new ExtendedObservableCollection<PhysicalScreenRefConfigDataViewModel>();
            if (dataModel != null)
            {
                foreach (var item in dataModel.PhysicalScreens)
                {
                    var convertedItem = new PhysicalScreenRefConfigDataViewModel(mediaShell, item);
                    this.PhysicalScreens.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected MasterLayoutConfigDataViewModel(IMediaShell mediaShell, MasterLayoutConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.PhysicalScreens = new ExtendedObservableCollection<PhysicalScreenRefConfigDataViewModel>();
            foreach (var item in dataViewModel.PhysicalScreens)
            {
                PhysicalScreenRefConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (PhysicalScreenRefConfigDataViewModel)item.Clone();
                }

                this.PhysicalScreens.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.PhysicalScreens.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<PhysicalScreenRefConfigDataViewModel> PhysicalScreens
        {
            get
            {
                return this.physicalscreens;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.physicalscreens);
                this.SetProperty(ref this.physicalscreens, value, () => this.PhysicalScreens);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new MasterLayoutConfig Export(object parameters = null)
        {
            var masterlayout = (MasterLayoutConfig)this.CreateExportObject();
            this.DoExport(masterlayout, parameters);
            return masterlayout;
        }

        public new MasterLayoutConfigDataModel ToDataModel()
        {
            var masterlayout = (MasterLayoutConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(masterlayout);
            return masterlayout;
        }

        public override void ClearDirty()
        {
            if (this.PhysicalScreens != null)
            {
                this.PhysicalScreens.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new MasterLayoutConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is MasterLayoutConfigDataViewModel)
            {
                var that = (MasterLayoutConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.PhysicalScreens.Count == that.PhysicalScreens.Count;
                        if (result)
                        {
                            foreach (var item in this.PhysicalScreens)
                            {
                                var found = false;
                                foreach (var otherItem in that.PhysicalScreens)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new MasterLayoutConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new MasterLayoutConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (MasterLayoutConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            foreach (var item in this.PhysicalScreens)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.PhysicalScreens.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (MasterLayoutConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            foreach (var item in this.PhysicalScreens)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.PhysicalScreens.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(MasterLayoutConfigDataModel dataModel = null);

        partial void Initialize(MasterLayoutConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(MasterLayoutConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref MasterLayoutConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class MasterPresentationConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private ExtendedObservableCollection<MasterCycleConfigDataViewModel> mastercycles;

        private ExtendedObservableCollection<MasterEventCycleConfigDataViewModel> mastereventcycles;

        private ExtendedObservableCollection<MasterLayoutConfigDataViewModel> masterlayouts;

        public MasterPresentationConfigDataViewModel(IMediaShell mediaShell, MasterPresentationConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.MasterCycles = new ExtendedObservableCollection<MasterCycleConfigDataViewModel>();
            this.MasterEventCycles = new ExtendedObservableCollection<MasterEventCycleConfigDataViewModel>();
            this.MasterLayouts = new ExtendedObservableCollection<MasterLayoutConfigDataViewModel>();
            if (dataModel != null)
            {
                foreach (var item in dataModel.MasterCycles)
                {
                    var convertedItem = new MasterCycleConfigDataViewModel(mediaShell, item);
                    this.MasterCycles.Add(convertedItem);
                }

                foreach (var item in dataModel.MasterEventCycles)
                {
                    var convertedItem = new MasterEventCycleConfigDataViewModel(mediaShell, item);
                    this.MasterEventCycles.Add(convertedItem);
                }

                foreach (var item in dataModel.MasterLayouts)
                {
                    var convertedItem = new MasterLayoutConfigDataViewModel(mediaShell, item);
                    this.MasterLayouts.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected MasterPresentationConfigDataViewModel(IMediaShell mediaShell, MasterPresentationConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.MasterCycles = new ExtendedObservableCollection<MasterCycleConfigDataViewModel>();
            foreach (var item in dataViewModel.MasterCycles)
            {
                MasterCycleConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (MasterCycleConfigDataViewModel)item.Clone();
                }

                this.MasterCycles.Add(clonedItem);
            }

            this.MasterEventCycles = new ExtendedObservableCollection<MasterEventCycleConfigDataViewModel>();
            foreach (var item in dataViewModel.MasterEventCycles)
            {
                MasterEventCycleConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (MasterEventCycleConfigDataViewModel)item.Clone();
                }

                this.MasterEventCycles.Add(clonedItem);
            }

            this.MasterLayouts = new ExtendedObservableCollection<MasterLayoutConfigDataViewModel>();
            foreach (var item in dataViewModel.MasterLayouts)
            {
                MasterLayoutConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (MasterLayoutConfigDataViewModel)item.Clone();
                }

                this.MasterLayouts.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.MasterCycles.IsDirty || this.MasterEventCycles.IsDirty || this.MasterLayouts.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<MasterCycleConfigDataViewModel> MasterCycles
        {
            get
            {
                return this.mastercycles;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.mastercycles);
                this.SetProperty(ref this.mastercycles, value, () => this.MasterCycles);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<MasterEventCycleConfigDataViewModel> MasterEventCycles
        {
            get
            {
                return this.mastereventcycles;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.mastereventcycles);
                this.SetProperty(ref this.mastereventcycles, value, () => this.MasterEventCycles);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<MasterLayoutConfigDataViewModel> MasterLayouts
        {
            get
            {
                return this.masterlayouts;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.masterlayouts);
                this.SetProperty(ref this.masterlayouts, value, () => this.MasterLayouts);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public MasterPresentationConfig Export(object parameters = null)
        {
            var masterpresentation = (MasterPresentationConfig)this.CreateExportObject();
            this.DoExport(masterpresentation, parameters);
            return masterpresentation;
        }

        public MasterPresentationConfigDataModel ToDataModel()
        {
            var masterpresentation = (MasterPresentationConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(masterpresentation);
            return masterpresentation;
        }

        public override void ClearDirty()
        {
            if (this.MasterCycles != null)
            {
                this.MasterCycles.ClearDirty();
            }

            if (this.MasterEventCycles != null)
            {
                this.MasterEventCycles.ClearDirty();
            }

            if (this.MasterLayouts != null)
            {
                this.MasterLayouts.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new MasterPresentationConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is MasterPresentationConfigDataViewModel)
            {
                var that = (MasterPresentationConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.MasterCycles.Count == that.MasterCycles.Count;
                        if (result)
                        {
                            foreach (var item in this.MasterCycles)
                            {
                                var found = false;
                                foreach (var otherItem in that.MasterCycles)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        result = result && this.MasterEventCycles.Count == that.MasterEventCycles.Count;
                        if (result)
                        {
                            foreach (var item in this.MasterEventCycles)
                            {
                                var found = false;
                                foreach (var otherItem in that.MasterEventCycles)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        result = result && this.MasterLayouts.Count == that.MasterLayouts.Count;
                        if (result)
                        {
                            foreach (var item in this.MasterLayouts)
                            {
                                var found = false;
                                foreach (var otherItem in that.MasterLayouts)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new MasterPresentationConfig();
        }

        protected object CreateDataModelObject()
        {
            return new MasterPresentationConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (MasterPresentationConfig)exportModel;
            foreach (var item in this.MasterCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.MasterCycles.Add(convertedItem);
                }

            }

            foreach (var item in this.MasterEventCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.MasterEventCycles.Add(convertedItem);
                }

            }

            foreach (var item in this.MasterLayouts)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.MasterLayouts.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (MasterPresentationConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            foreach (var item in this.MasterCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.MasterCycles.Add(convertedItem);
                }

            }

            foreach (var item in this.MasterEventCycles)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.MasterEventCycles.Add(convertedItem);
                }

            }

            foreach (var item in this.MasterLayouts)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.MasterLayouts.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(MasterPresentationConfigDataModel dataModel = null);

        partial void Initialize(MasterPresentationConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(MasterPresentationConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref MasterPresentationConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class PhysicalScreenConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> name;

        private DataValue<PhysicalScreenType> type;

        private DataValue<string> identifier;

        private DataValue<int> width;

        private DataValue<int> height;

        private AnimatedDynamicDataValue<bool> visible;

        public PhysicalScreenConfigDataViewModel(IMediaShell mediaShell, PhysicalScreenConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Name = new DataValue<string>(string.Empty);
            this.Name.PropertyChanged += this.NameChanged;
            this.Type = new DataValue<PhysicalScreenType>(new PhysicalScreenType());
            this.Type.PropertyChanged += this.TypeChanged;
            this.Identifier = new DataValue<string>(string.Empty);
            this.Identifier.PropertyChanged += this.IdentifierChanged;
            this.Width = new DataValue<int>(default(int));
            this.Width.PropertyChanged += this.WidthChanged;
            this.Height = new DataValue<int>(default(int));
            this.Height.PropertyChanged += this.HeightChanged;
            this.Visible = new AnimatedDynamicDataValue<bool>(true);
            this.Visible.PropertyChanged += this.VisibleChanged;
            if (dataModel != null)
            {
                this.Name.Value = dataModel.Name;
                this.Type.Value = dataModel.Type;
                this.Identifier.Value = dataModel.Identifier;
                this.Width.Value = dataModel.Width;
                this.Height.Value = dataModel.Height;
                if (dataModel.Visible != null)
                {
                    this.Visible.Value = dataModel.Visible;
                }

                if (dataModel.VisibleProperty != null)
                {
                    this.Visible.Formula = this.CreateEvalDataViewModel(dataModel.VisibleProperty.Evaluation);
                    if (dataModel.VisibleProperty.Animation != null)
                    {
                        this.Visible.Animation = new AnimationDataViewModel(this.mediaShell, dataModel.VisibleProperty.Animation);
                    }

                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected PhysicalScreenConfigDataViewModel(IMediaShell mediaShell, PhysicalScreenConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Name = (DataValue<string>)dataViewModel.Name.Clone();
            this.Name.PropertyChanged += this.NameChanged;
            this.Type = (DataValue<PhysicalScreenType>)dataViewModel.Type.Clone();
            this.Type.PropertyChanged += this.TypeChanged;
            this.Identifier = (DataValue<string>)dataViewModel.Identifier.Clone();
            this.Identifier.PropertyChanged += this.IdentifierChanged;
            this.Width = (DataValue<int>)dataViewModel.Width.Clone();
            this.Width.PropertyChanged += this.WidthChanged;
            this.Height = (DataValue<int>)dataViewModel.Height.Clone();
            this.Height.PropertyChanged += this.HeightChanged;
            this.Visible = (AnimatedDynamicDataValue<bool>)dataViewModel.Visible.Clone();
            this.Visible.PropertyChanged += this.VisibleChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Name.IsDirty || this.Type.IsDirty || this.Identifier.IsDirty || this.Width.IsDirty || this.Height.IsDirty || this.Visible.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.name);
                if (this.name != null)
                {
                    this.name.PropertyChanged -= this.NameChanged;
                }

                this.SetProperty(ref this.name, value, () => this.Name);
                if (value != null)
                {
                    this.name.PropertyChanged += this.NameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public DataValue<PhysicalScreenType> Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.type);
                if (this.type != null)
                {
                    this.type.PropertyChanged -= this.TypeChanged;
                }

                this.SetProperty(ref this.type, value, () => this.Type);
                if (value != null)
                {
                    this.type.PropertyChanged += this.TypeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", Filter = "LED,Audio", OrderIndex = 1, GroupOrderIndex = 0)]
        public DataValue<string> Identifier
        {
            get
            {
                return this.identifier;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.identifier);
                if (this.identifier != null)
                {
                    this.identifier.PropertyChanged -= this.IdentifierChanged;
                }

                this.SetProperty(ref this.identifier, value, () => this.Identifier);
                if (value != null)
                {
                    this.identifier.PropertyChanged += this.IdentifierChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public DataValue<int> Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.width);
                if (this.width != null)
                {
                    this.width.PropertyChanged -= this.WidthChanged;
                }

                this.SetProperty(ref this.width, value, () => this.Width);
                if (value != null)
                {
                    this.width.PropertyChanged += this.WidthChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public DataValue<int> Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.height);
                if (this.height != null)
                {
                    this.height.PropertyChanged -= this.HeightChanged;
                }

                this.SetProperty(ref this.height, value, () => this.Height);
                if (value != null)
                {
                    this.height.PropertyChanged += this.HeightChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 2, GroupOrderIndex = 0)]
        public AnimatedDynamicDataValue<bool> Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.visible);
                if (this.visible != null)
                {
                    this.visible.PropertyChanged -= this.VisibleChanged;
                }

                this.SetProperty(ref this.visible, value, () => this.Visible);
                if (value != null)
                {
                    this.visible.PropertyChanged += this.VisibleChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public PhysicalScreenConfig Export(object parameters = null)
        {
            var physicalscreen = (PhysicalScreenConfig)this.CreateExportObject();
            this.DoExport(physicalscreen, parameters);
            return physicalscreen;
        }

        public PhysicalScreenConfigDataModel ToDataModel()
        {
            var physicalscreen = (PhysicalScreenConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(physicalscreen);
            return physicalscreen;
        }

        public override void ClearDirty()
        {
            if (this.Name != null)
            {
                this.Name.ClearDirty();
            }

            if (this.Type != null)
            {
                this.Type.ClearDirty();
            }

            if (this.Identifier != null)
            {
                this.Identifier.ClearDirty();
            }

            if (this.Width != null)
            {
                this.Width.ClearDirty();
            }

            if (this.Height != null)
            {
                this.Height.ClearDirty();
            }

            if (this.Visible != null)
            {
                this.Visible.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new PhysicalScreenConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is PhysicalScreenConfigDataViewModel)
            {
                var that = (PhysicalScreenConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Name.EqualsValue(that.Name);
                        result = result && this.Type.EqualsValue(that.Type);
                        result = result && this.Identifier.EqualsValue(that.Identifier);
                        result = result && this.Width.EqualsValue(that.Width);
                        result = result && this.Height.EqualsValue(that.Height);
                        result = result && this.Visible.EqualsValue(that.Visible);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new PhysicalScreenConfig();
        }

        protected object CreateDataModelObject()
        {
            return new PhysicalScreenConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (PhysicalScreenConfig)exportModel;
            model.Name = this.Name.Value;
            model.Type = this.Type.Value;
            model.Identifier = this.Identifier.Value;
            model.Width = this.Width.Value;
            model.Height = this.Height.Value;
            if (this.Visible.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Visible.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var visibleEval = Visible.Formula as CodeConversionEvalDataViewModel;
                    if (visibleEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = visibleEval.UseImage.Value ? "{2}" : "{3}";
                            model.VisibleProperty = new AnimatedDynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.VisibleProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Visible.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.VisibleProperty = new AnimatedDynamicProperty(((EvalDataViewModelBase)this.Visible.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Visible = this.Visible.Value;
                }
            }
            else
            {
                model.Visible = this.Visible.Value;
            }

            if (this.Visible.Animation != null && this.Visible.Formula != null)
            {
                if (model.VisibleProperty == null)
                {
                    model.VisibleProperty = new AnimatedDynamicProperty();
                }

                model.VisibleProperty.Animation = new PropertyChangeAnimation {
                    Duration = ((AnimationDataViewModel)this.Visible.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Visible.Animation).Type.Value
                };
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (PhysicalScreenConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Name = this.Name.Value;
            model.Type = this.Type.Value;
            model.Identifier = this.Identifier.Value;
            model.Width = this.Width.Value;
            model.Height = this.Height.Value;
            model.Visible = this.Visible.Value;
            if (this.Visible.Formula != null)
            {
                model.VisibleProperty = new AnimatedDynamicPropertyDataModel(((EvalDataViewModelBase)this.Visible.Formula).ToDataModel());
            }

            if (this.Visible.Animation != null)
            {
                if (model.VisibleProperty == null)
                {
                    model.VisibleProperty = new AnimatedDynamicPropertyDataModel();
                }

                model.VisibleProperty.Animation = new PropertyChangeAnimationDataModel {
                    Duration = ((AnimationDataViewModel)this.Visible.Animation).Duration.Value,
                    Type = ((AnimationDataViewModel)this.Visible.Animation).Type.Value
                };
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Name);
        }

        private void TypeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Type);
        }

        private void IdentifierChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Identifier);
        }

        private void WidthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Width);
        }

        private void HeightChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Height);
        }

        private void VisibleChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Visible);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(PhysicalScreenConfigDataModel dataModel = null);

        partial void Initialize(PhysicalScreenConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(PhysicalScreenConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref PhysicalScreenConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class PhysicalScreenRefConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private string referenceReferenceName;

        private PhysicalScreenConfigDataViewModel referenceReference;

        private ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel> virtualdisplays;

        public PhysicalScreenRefConfigDataViewModel(IMediaShell mediaShell, PhysicalScreenRefConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            // Reference
            this.VirtualDisplays = new ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel>();
            if (dataModel != null)
            {
                this.referenceReferenceName = dataModel.Reference;
                foreach (var item in dataModel.VirtualDisplays)
                {
                    var convertedItem = new VirtualDisplayRefConfigDataViewModel(mediaShell, item);
                    this.VirtualDisplays.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected PhysicalScreenRefConfigDataViewModel(IMediaShell mediaShell, PhysicalScreenRefConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.ReferenceName = dataViewModel.ReferenceName;
            this.VirtualDisplays = new ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel>();
            foreach (var item in dataViewModel.VirtualDisplays)
            {
                VirtualDisplayRefConfigDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (VirtualDisplayRefConfigDataViewModel)item.Clone();
                }

                this.VirtualDisplays.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.VirtualDisplays.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public PhysicalScreenConfigDataViewModel Reference
        {
            get
            {
                if (this.referenceReference == null)
                {
                    this.referenceReference = this.FindReference();
                    if (this.referenceReference != null)
                    {
                        this.referenceReference.PropertyChanged += this.ReferenceChanged;
                    }

                }

                return this.referenceReference;
            }
            set
            {
                if (this.Reference != null)
                {
                    this.Reference.PropertyChanged -= this.ReferenceChanged;
                }

                this.SetProperty(ref this.referenceReference, value, () => this.Reference);
                if (value != null)
                {
                    this.ReferenceName = value.Name.Value;
                    this.Reference.PropertyChanged += this.ReferenceChanged;
                }

            }
        }

        public string ReferenceName
        {
            get
            {
                return this.referenceReferenceName;
            }
            private set
            {
                this.SetProperty(ref this.referenceReferenceName, value, () => this.Reference);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<VirtualDisplayRefConfigDataViewModel> VirtualDisplays
        {
            get
            {
                return this.virtualdisplays;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.virtualdisplays);
                this.SetProperty(ref this.virtualdisplays, value, () => this.VirtualDisplays);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public PhysicalScreenRefConfig Export(object parameters = null)
        {
            var physicalscreenref = (PhysicalScreenRefConfig)this.CreateExportObject();
            this.DoExport(physicalscreenref, parameters);
            return physicalscreenref;
        }

        public PhysicalScreenRefConfigDataModel ToDataModel()
        {
            var physicalscreenref = (PhysicalScreenRefConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(physicalscreenref);
            return physicalscreenref;
        }

        public override void ClearDirty()
        {
            if (this.Reference != null)
            {
                this.Reference.ClearDirty();
            }

            if (this.VirtualDisplays != null)
            {
                this.VirtualDisplays.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new PhysicalScreenRefConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is PhysicalScreenRefConfigDataViewModel)
            {
                var that = (PhysicalScreenRefConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Reference != null)
                        {
                            result = result && this.Reference.EqualsViewModel(that.Reference);
                        }

                        result = result && this.VirtualDisplays.Count == that.VirtualDisplays.Count;
                        if (result)
                        {
                            foreach (var item in this.VirtualDisplays)
                            {
                                var found = false;
                                foreach (var otherItem in that.VirtualDisplays)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new PhysicalScreenRefConfig();
        }

        protected object CreateDataModelObject()
        {
            return new PhysicalScreenRefConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (PhysicalScreenRefConfig)exportModel;
            model.Reference = this.ReferenceName;
            foreach (var item in this.VirtualDisplays)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.VirtualDisplays.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (PhysicalScreenRefConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Reference = this.ReferenceName;
            foreach (var item in this.VirtualDisplays)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.VirtualDisplays.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ReferenceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Reference != null)
            {
                this.ReferenceName = this.Reference.Name.Value;
            }

            this.RaisePropertyChanged(() => this.Reference);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(PhysicalScreenRefConfigDataModel dataModel = null);

        partial void Initialize(PhysicalScreenRefConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(PhysicalScreenRefConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref PhysicalScreenRefConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class PoolConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> name;

        private DataValue<string> basedirectory;

        public PoolConfigDataViewModel(IMediaShell mediaShell, PoolConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Name = new DataValue<string>(string.Empty);
            this.Name.PropertyChanged += this.NameChanged;
            this.BaseDirectory = new DataValue<string>(string.Empty);
            this.BaseDirectory.PropertyChanged += this.BaseDirectoryChanged;
            if (dataModel != null)
            {
                this.Name.Value = dataModel.Name;
                this.BaseDirectory.Value = dataModel.BaseDirectory;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected PoolConfigDataViewModel(IMediaShell mediaShell, PoolConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Name = (DataValue<string>)dataViewModel.Name.Clone();
            this.Name.PropertyChanged += this.NameChanged;
            this.BaseDirectory = (DataValue<string>)dataViewModel.BaseDirectory.Clone();
            this.BaseDirectory.PropertyChanged += this.BaseDirectoryChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Name.IsDirty || this.BaseDirectory.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.name);
                if (this.name != null)
                {
                    this.name.PropertyChanged -= this.NameChanged;
                }

                this.SetProperty(ref this.name, value, () => this.Name);
                if (value != null)
                {
                    this.name.PropertyChanged += this.NameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> BaseDirectory
        {
            get
            {
                return this.basedirectory;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.basedirectory);
                if (this.basedirectory != null)
                {
                    this.basedirectory.PropertyChanged -= this.BaseDirectoryChanged;
                }

                this.SetProperty(ref this.basedirectory, value, () => this.BaseDirectory);
                if (value != null)
                {
                    this.basedirectory.PropertyChanged += this.BaseDirectoryChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public PoolConfig Export(object parameters = null)
        {
            var pool = (PoolConfig)this.CreateExportObject();
            this.DoExport(pool, parameters);
            return pool;
        }

        public PoolConfigDataModel ToDataModel()
        {
            var pool = (PoolConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(pool);
            return pool;
        }

        public override void ClearDirty()
        {
            if (this.Name != null)
            {
                this.Name.ClearDirty();
            }

            if (this.BaseDirectory != null)
            {
                this.BaseDirectory.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new PoolConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is PoolConfigDataViewModel)
            {
                var that = (PoolConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Name.EqualsValue(that.Name);
                        result = result && this.BaseDirectory.EqualsValue(that.BaseDirectory);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new PoolConfig();
        }

        protected object CreateDataModelObject()
        {
            return new PoolConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (PoolConfig)exportModel;
            model.Name = this.Name.Value;
            model.BaseDirectory = this.BaseDirectory.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (PoolConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Name = this.Name.Value;
            model.BaseDirectory = this.BaseDirectory.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Name);
        }

        private void BaseDirectoryChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.BaseDirectory);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(PoolConfigDataModel dataModel = null);

        partial void Initialize(PoolConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(PoolConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref PoolConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class ResolutionConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> width;

        private DataValue<int> height;

        private ExtendedObservableCollection<LayoutElementDataViewModelBase> elements;

        public ResolutionConfigDataViewModel(IMediaShell mediaShell, ResolutionConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Width = new DataValue<int>(default(int));
            this.Width.PropertyChanged += this.WidthChanged;
            this.Height = new DataValue<int>(default(int));
            this.Height.PropertyChanged += this.HeightChanged;
            this.Elements = new ExtendedObservableCollection<LayoutElementDataViewModelBase>();
            if (dataModel != null)
            {
                this.Width.Value = dataModel.Width;
                this.Height.Value = dataModel.Height;
                foreach (var item in dataModel.Elements)
                {
                    var typeName = item.GetType().Name.Replace("DataModel", "DataViewModel");
                    var assembly = this.GetType().Assembly;
                    var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                    if (type == null)
                    {
                        throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, item.GetType().Name));
                    }

                    var convertedItem = (LayoutElementDataViewModelBase)Activator.CreateInstance(type, mediaShell, item);
                    this.Elements.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected ResolutionConfigDataViewModel(IMediaShell mediaShell, ResolutionConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Width = (DataValue<int>)dataViewModel.Width.Clone();
            this.Width.PropertyChanged += this.WidthChanged;
            this.Height = (DataValue<int>)dataViewModel.Height.Clone();
            this.Height.PropertyChanged += this.HeightChanged;
            this.Elements = new ExtendedObservableCollection<LayoutElementDataViewModelBase>();
            foreach (var item in dataViewModel.Elements)
            {
                LayoutElementDataViewModelBase clonedItem = null;
                if (item != null)
                {
                    clonedItem = (LayoutElementDataViewModelBase)item.Clone();
                }

                this.Elements.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Width.IsDirty || this.Height.IsDirty || this.Elements.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.width);
                if (this.width != null)
                {
                    this.width.PropertyChanged -= this.WidthChanged;
                }

                this.SetProperty(ref this.width, value, () => this.Width);
                if (value != null)
                {
                    this.width.PropertyChanged += this.WidthChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.height);
                if (this.height != null)
                {
                    this.height.PropertyChanged -= this.HeightChanged;
                }

                this.SetProperty(ref this.height, value, () => this.Height);
                if (value != null)
                {
                    this.height.PropertyChanged += this.HeightChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<LayoutElementDataViewModelBase> Elements
        {
            get
            {
                return this.elements;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.elements);
                this.SetProperty(ref this.elements, value, () => this.Elements);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public ResolutionConfig Export(object parameters = null)
        {
            var resolution = (ResolutionConfig)this.CreateExportObject();
            this.DoExport(resolution, parameters);
            return resolution;
        }

        public ResolutionConfigDataModel ToDataModel()
        {
            var resolution = (ResolutionConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(resolution);
            return resolution;
        }

        public override void ClearDirty()
        {
            if (this.Width != null)
            {
                this.Width.ClearDirty();
            }

            if (this.Height != null)
            {
                this.Height.ClearDirty();
            }

            if (this.Elements != null)
            {
                this.Elements.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new ResolutionConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is ResolutionConfigDataViewModel)
            {
                var that = (ResolutionConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Width.EqualsValue(that.Width);
                        result = result && this.Height.EqualsValue(that.Height);
                        result = result && this.Elements.Count == that.Elements.Count;
                        if (result)
                        {
                            foreach (var item in this.Elements)
                            {
                                var found = false;
                                foreach (var otherItem in that.Elements)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new ResolutionConfig();
        }

        protected object CreateDataModelObject()
        {
            return new ResolutionConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (ResolutionConfig)exportModel;
            model.Width = this.Width.Value;
            model.Height = this.Height.Value;
            foreach (var item in this.Elements)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Elements.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (ResolutionConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Width = this.Width.Value;
            model.Height = this.Height.Value;
            foreach (var item in this.Elements)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Elements.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void WidthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Width);
        }

        private void HeightChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Height);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(ResolutionConfigDataModel dataModel = null);

        partial void Initialize(ResolutionConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(ResolutionConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref ResolutionConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class VirtualDisplayConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> name;

        private DataValue<int> width;

        private DataValue<int> height;

        private string cyclepackageReferenceName;

        private CyclePackageConfigDataViewModel cyclepackageReference;

        public VirtualDisplayConfigDataViewModel(IMediaShell mediaShell, VirtualDisplayConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Name = new DataValue<string>(string.Empty);
            this.Name.PropertyChanged += this.NameChanged;
            // CyclePackage
            this.Width = new DataValue<int>(default(int));
            this.Width.PropertyChanged += this.WidthChanged;
            this.Height = new DataValue<int>(default(int));
            this.Height.PropertyChanged += this.HeightChanged;
            if (dataModel != null)
            {
                this.Name.Value = dataModel.Name;
                this.cyclepackageReferenceName = dataModel.CyclePackage;
                this.Width.Value = dataModel.Width;
                this.Height.Value = dataModel.Height;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected VirtualDisplayConfigDataViewModel(IMediaShell mediaShell, VirtualDisplayConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Name = (DataValue<string>)dataViewModel.Name.Clone();
            this.Name.PropertyChanged += this.NameChanged;
            this.CyclePackageName = dataViewModel.CyclePackageName;
            this.Width = (DataValue<int>)dataViewModel.Width.Clone();
            this.Width.PropertyChanged += this.WidthChanged;
            this.Height = (DataValue<int>)dataViewModel.Height.Clone();
            this.Height.PropertyChanged += this.HeightChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Name.IsDirty || this.Width.IsDirty || this.Height.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.name);
                if (this.name != null)
                {
                    this.name.PropertyChanged -= this.NameChanged;
                }

                this.SetProperty(ref this.name, value, () => this.Name);
                if (value != null)
                {
                    this.name.PropertyChanged += this.NameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public CyclePackageConfigDataViewModel CyclePackage
        {
            get
            {
                if (this.cyclepackageReference == null)
                {
                    this.cyclepackageReference = this.FindReference();
                    if (this.cyclepackageReference != null)
                    {
                        this.cyclepackageReference.PropertyChanged += this.CyclePackageChanged;
                    }

                }

                return this.cyclepackageReference;
            }
            set
            {
                if (this.CyclePackage != null)
                {
                    this.CyclePackage.PropertyChanged -= this.CyclePackageChanged;
                }

                this.SetProperty(ref this.cyclepackageReference, value, () => this.CyclePackage);
                if (value != null)
                {
                    this.CyclePackageName = value.Name.Value;
                    this.CyclePackage.PropertyChanged += this.CyclePackageChanged;
                }

            }
        }

        public string CyclePackageName
        {
            get
            {
                return this.cyclepackageReferenceName;
            }
            private set
            {
                this.SetProperty(ref this.cyclepackageReferenceName, value, () => this.CyclePackage);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.width);
                if (this.width != null)
                {
                    this.width.PropertyChanged -= this.WidthChanged;
                }

                this.SetProperty(ref this.width, value, () => this.Width);
                if (value != null)
                {
                    this.width.PropertyChanged += this.WidthChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.height);
                if (this.height != null)
                {
                    this.height.PropertyChanged -= this.HeightChanged;
                }

                this.SetProperty(ref this.height, value, () => this.Height);
                if (value != null)
                {
                    this.height.PropertyChanged += this.HeightChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public VirtualDisplayConfig Export(object parameters = null)
        {
            var virtualdisplay = (VirtualDisplayConfig)this.CreateExportObject();
            this.DoExport(virtualdisplay, parameters);
            return virtualdisplay;
        }

        public VirtualDisplayConfigDataModel ToDataModel()
        {
            var virtualdisplay = (VirtualDisplayConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(virtualdisplay);
            return virtualdisplay;
        }

        public override void ClearDirty()
        {
            if (this.Name != null)
            {
                this.Name.ClearDirty();
            }

            if (this.CyclePackage != null)
            {
                this.CyclePackage.ClearDirty();
            }

            if (this.Width != null)
            {
                this.Width.ClearDirty();
            }

            if (this.Height != null)
            {
                this.Height.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new VirtualDisplayConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is VirtualDisplayConfigDataViewModel)
            {
                var that = (VirtualDisplayConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Name.EqualsValue(that.Name);
                        result = result && this.CyclePackage.EqualsViewModel(that.CyclePackage);
                        result = result && this.Width.EqualsValue(that.Width);
                        result = result && this.Height.EqualsValue(that.Height);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new VirtualDisplayConfig();
        }

        protected object CreateDataModelObject()
        {
            return new VirtualDisplayConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (VirtualDisplayConfig)exportModel;
            model.Name = this.Name.Value;
            model.CyclePackage = this.CyclePackageName;
            model.Width = this.Width.Value;
            model.Height = this.Height.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (VirtualDisplayConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Name = this.Name.Value;
            model.CyclePackage = this.CyclePackageName;
            model.Width = this.Width.Value;
            model.Height = this.Height.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Name);
        }

        private void CyclePackageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.CyclePackage != null)
            {
                this.CyclePackageName = this.CyclePackage.Name.Value;
            }

            this.RaisePropertyChanged(() => this.CyclePackage);
        }

        private void WidthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Width);
        }

        private void HeightChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Height);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(VirtualDisplayConfigDataModel dataModel = null);

        partial void Initialize(VirtualDisplayConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(VirtualDisplayConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref VirtualDisplayConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class VirtualDisplayRefConfigDataViewModel : DrawableElementDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private string referenceReferenceName;

        private VirtualDisplayConfigDataViewModel referenceReference;

        public VirtualDisplayRefConfigDataViewModel(IMediaShell mediaShell, VirtualDisplayRefConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            // Reference
            if (dataModel != null)
            {
                this.referenceReferenceName = dataModel.Reference;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected VirtualDisplayRefConfigDataViewModel(IMediaShell mediaShell, VirtualDisplayRefConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.ReferenceName = dataViewModel.ReferenceName;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public VirtualDisplayConfigDataViewModel Reference
        {
            get
            {
                if (this.referenceReference == null)
                {
                    this.referenceReference = this.FindReference();
                    if (this.referenceReference != null)
                    {
                        this.referenceReference.PropertyChanged += this.ReferenceChanged;
                    }

                }

                return this.referenceReference;
            }
            set
            {
                if (this.Reference != null)
                {
                    this.Reference.PropertyChanged -= this.ReferenceChanged;
                }

                this.SetProperty(ref this.referenceReference, value, () => this.Reference);
                if (value != null)
                {
                    this.ReferenceName = value.Name.Value;
                    this.Reference.PropertyChanged += this.ReferenceChanged;
                }

            }
        }

        public string ReferenceName
        {
            get
            {
                return this.referenceReferenceName;
            }
            private set
            {
                this.SetProperty(ref this.referenceReferenceName, value, () => this.Reference);
            }
        }

        public new VirtualDisplayRefConfig Export(object parameters = null)
        {
            var virtualdisplayref = (VirtualDisplayRefConfig)this.CreateExportObject();
            this.DoExport(virtualdisplayref, parameters);
            return virtualdisplayref;
        }

        public new VirtualDisplayRefConfigDataModel ToDataModel()
        {
            var virtualdisplayref = (VirtualDisplayRefConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(virtualdisplayref);
            return virtualdisplayref;
        }

        public override void ClearDirty()
        {
            if (this.Reference != null)
            {
                this.Reference.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new VirtualDisplayRefConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is VirtualDisplayRefConfigDataViewModel)
            {
                var that = (VirtualDisplayRefConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Reference != null)
                        {
                            result = result && this.Reference.EqualsViewModel(that.Reference);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new VirtualDisplayRefConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new VirtualDisplayRefConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (VirtualDisplayRefConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Reference = this.ReferenceName;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (VirtualDisplayRefConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Reference = this.ReferenceName;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ReferenceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Reference != null)
            {
                this.ReferenceName = this.Reference.Name.Value;
            }

            this.RaisePropertyChanged(() => this.Reference);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(VirtualDisplayRefConfigDataModel dataModel = null);

        partial void Initialize(VirtualDisplayRefConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(VirtualDisplayRefConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref VirtualDisplayRefConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

}
namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle
{
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Presentation.Cycle;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Media.Core.Controllers;
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.ComponentModel;
    using System.Linq;
    public abstract partial class CycleConfigDataViewModelBase : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> name;

        private DynamicDataValue<bool> enabled;

        private ExtendedObservableCollection<SectionConfigDataViewModelBase> sections;

        public CycleConfigDataViewModelBase(IMediaShell mediaShell, CycleConfigDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Name = new DataValue<string>(string.Empty);
            this.Name.PropertyChanged += this.NameChanged;
            this.Enabled = new DynamicDataValue<bool>(true);
            this.Enabled.PropertyChanged += this.EnabledChanged;
            this.Sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase>();
            if (dataModel != null)
            {
                this.Name.Value = dataModel.Name;
                if (dataModel.Enabled != null)
                {
                    this.Enabled.Value = dataModel.Enabled;
                }

                if (dataModel.EnabledProperty != null)
                {
                    this.Enabled.Formula = this.CreateEvalDataViewModel(dataModel.EnabledProperty.Evaluation);
                }

                foreach (var item in dataModel.Sections)
                {
                    var typeName = item.GetType().Name.Replace("DataModel", "DataViewModel");
                    var assembly = this.GetType().Assembly;
                    var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                    if (type == null)
                    {
                        throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, item.GetType().Name));
                    }

                    var convertedItem = (SectionConfigDataViewModelBase)Activator.CreateInstance(type, mediaShell, item);
                    this.Sections.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected CycleConfigDataViewModelBase(IMediaShell mediaShell, CycleConfigDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Name = (DataValue<string>)dataViewModel.Name.Clone();
            this.Name.PropertyChanged += this.NameChanged;
            this.Enabled = (DynamicDataValue<bool>)dataViewModel.Enabled.Clone();
            this.Enabled.PropertyChanged += this.EnabledChanged;
            this.Sections = new ExtendedObservableCollection<SectionConfigDataViewModelBase>();
            foreach (var item in dataViewModel.Sections)
            {
                SectionConfigDataViewModelBase clonedItem = null;
                if (item != null)
                {
                    clonedItem = (SectionConfigDataViewModelBase)item.Clone();
                }

                this.Sections.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Name.IsDirty || this.Enabled.IsDirty || this.Sections.IsDirty;
            }
        }

        [UserVisibleProperty("Common", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.name);
                if (this.name != null)
                {
                    this.name.PropertyChanged -= this.NameChanged;
                }

                this.SetProperty(ref this.name, value, () => this.Name);
                if (value != null)
                {
                    this.name.PropertyChanged += this.NameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Common", OrderIndex = 0, GroupOrderIndex = 0)]
        public DynamicDataValue<bool> Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.enabled);
                if (this.enabled != null)
                {
                    this.enabled.PropertyChanged -= this.EnabledChanged;
                }

                this.SetProperty(ref this.enabled, value, () => this.Enabled);
                if (value != null)
                {
                    this.enabled.PropertyChanged += this.EnabledChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public ExtendedObservableCollection<SectionConfigDataViewModelBase> Sections
        {
            get
            {
                return this.sections;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.sections);
                this.SetProperty(ref this.sections, value, () => this.Sections);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public CycleConfigBase Export(object parameters = null)
        {
            var cyclebase = (CycleConfigBase)this.CreateExportObject();
            this.DoExport(cyclebase, parameters);
            return cyclebase;
        }

        public CycleConfigDataModelBase ToDataModel()
        {
            var cyclebase = (CycleConfigDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(cyclebase);
            return cyclebase;
        }

        public override void ClearDirty()
        {
            if (this.Name != null)
            {
                this.Name.ClearDirty();
            }

            if (this.Enabled != null)
            {
                this.Enabled.ClearDirty();
            }

            if (this.Sections != null)
            {
                this.Sections.ClearDirty();
            }

            base.ClearDirty();
        }

        public abstract object Clone();

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is CycleConfigDataViewModelBase)
            {
                var that = (CycleConfigDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Name.EqualsValue(that.Name);
                        result = result && this.Enabled.EqualsValue(that.Enabled);
                        result = result && this.Sections.Count == that.Sections.Count;
                        if (result)
                        {
                            foreach (var item in this.Sections)
                            {
                                var found = false;
                                foreach (var otherItem in that.Sections)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected abstract object CreateExportObject();

        protected abstract object CreateDataModelObject();

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (CycleConfigBase)exportModel;
            model.Name = this.Name.Value;
            if (this.Enabled.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Enabled.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var enabledEval = Enabled.Formula as CodeConversionEvalDataViewModel;
                    if (enabledEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = enabledEval.UseImage.Value ? "{2}" : "{3}";
                            model.EnabledProperty = new DynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.EnabledProperty = new DynamicProperty(((EvalDataViewModelBase)this.Enabled.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.EnabledProperty = new DynamicProperty(((EvalDataViewModelBase)this.Enabled.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Enabled = this.Enabled.Value;
                }
            }
            else
            {
                model.Enabled = this.Enabled.Value;
            }

            foreach (var item in this.Sections)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Sections.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CycleConfigDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            model.Name = this.Name.Value;
            model.Enabled = this.Enabled.Value;
            if (this.Enabled.Formula != null)
            {
                model.EnabledProperty = new DynamicPropertyDataModel(((EvalDataViewModelBase)this.Enabled.Formula).ToDataModel());
            }

            foreach (var item in this.Sections)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Sections.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void NameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Name);
        }

        private void EnabledChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Enabled);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(CycleConfigDataModelBase dataModel = null);

        partial void Initialize(CycleConfigDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(CycleConfigBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref CycleConfigDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class CycleRefConfigDataViewModelBase : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private string referenceReferenceName;

        private CycleConfigDataViewModelBase referenceReference;

        public CycleRefConfigDataViewModelBase(IMediaShell mediaShell, CycleRefConfigDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            // Reference
            if (dataModel != null)
            {
                this.referenceReferenceName = dataModel.Reference;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected CycleRefConfigDataViewModelBase(IMediaShell mediaShell, CycleRefConfigDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.ReferenceName = dataViewModel.ReferenceName;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        [UserVisibleProperty("Common", OrderIndex = 0, GroupOrderIndex = 0)]
        public CycleConfigDataViewModelBase Reference
        {
            get
            {
                if (this.referenceReference == null)
                {
                    this.referenceReference = this.FindReference();
                    if (this.referenceReference != null)
                    {
                        this.referenceReference.PropertyChanged += this.ReferenceChanged;
                    }

                }

                return this.referenceReference;
            }
            set
            {
                if (this.Reference != null)
                {
                    this.Reference.PropertyChanged -= this.ReferenceChanged;
                }

                this.SetProperty(ref this.referenceReference, value, () => this.Reference);
                if (value != null)
                {
                    this.ReferenceName = value.Name.Value;
                    this.Reference.PropertyChanged += this.ReferenceChanged;
                }

            }
        }

        public string ReferenceName
        {
            get
            {
                return this.referenceReferenceName;
            }
            private set
            {
                this.SetProperty(ref this.referenceReferenceName, value, () => this.Reference);
            }
        }

        public CycleRefConfigBase Export(object parameters = null)
        {
            var cyclerefbase = (CycleRefConfigBase)this.CreateExportObject();
            this.DoExport(cyclerefbase, parameters);
            return cyclerefbase;
        }

        public CycleRefConfigDataModelBase ToDataModel()
        {
            var cyclerefbase = (CycleRefConfigDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(cyclerefbase);
            return cyclerefbase;
        }

        public override void ClearDirty()
        {
            if (this.Reference != null)
            {
                this.Reference.ClearDirty();
            }

            base.ClearDirty();
        }

        public abstract object Clone();

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is CycleRefConfigDataViewModelBase)
            {
                var that = (CycleRefConfigDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Reference != null)
                        {
                            result = result && this.Reference.EqualsViewModel(that.Reference);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected abstract object CreateExportObject();

        protected abstract object CreateDataModelObject();

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (CycleRefConfigBase)exportModel;
            model.Reference = this.ReferenceName;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CycleRefConfigDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            model.Reference = this.ReferenceName;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ReferenceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Reference != null)
            {
                this.ReferenceName = this.Reference.Name.Value;
            }

            this.RaisePropertyChanged(() => this.Reference);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(CycleRefConfigDataModelBase dataModel = null);

        partial void Initialize(CycleRefConfigDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(CycleRefConfigBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref CycleRefConfigDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class EventCycleConfigDataViewModel : EventCycleConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public EventCycleConfigDataViewModel(IMediaShell mediaShell, EventCycleConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected EventCycleConfigDataViewModel(IMediaShell mediaShell, EventCycleConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new EventCycleConfig Export(object parameters = null)
        {
            var eventcycle = (EventCycleConfig)this.CreateExportObject();
            this.DoExport(eventcycle, parameters);
            return eventcycle;
        }

        public new EventCycleConfigDataModel ToDataModel()
        {
            var eventcycle = (EventCycleConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(eventcycle);
            return eventcycle;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new EventCycleConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is EventCycleConfigDataViewModel)
            {
                var that = (EventCycleConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new EventCycleConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new EventCycleConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (EventCycleConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (EventCycleConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(EventCycleConfigDataModel dataModel = null);

        partial void Initialize(EventCycleConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(EventCycleConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref EventCycleConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class EventCycleConfigDataViewModelBase : CycleConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private GenericTriggerConfigDataViewModel trigger;

        public EventCycleConfigDataViewModelBase(IMediaShell mediaShell, EventCycleConfigDataModelBase dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.trigger = new GenericTriggerConfigDataViewModel(this.mediaShell);
            if (dataModel != null)
            {
                this.trigger = new GenericTriggerConfigDataViewModel(this.mediaShell, dataModel.Trigger);
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected EventCycleConfigDataViewModelBase(IMediaShell mediaShell, EventCycleConfigDataViewModelBase dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            var clonedTrigger = dataViewModel.Trigger;
            if (clonedTrigger != null)
            {
                this.Trigger = (GenericTriggerConfigDataViewModel)clonedTrigger.Clone();
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || (this.Trigger != null && this.Trigger.IsDirty);
            }
        }

        [UserVisibleProperty("Common", OrderIndex = 0, GroupOrderIndex = 0)]
        public GenericTriggerConfigDataViewModel Trigger
        {
            get
            {
                return this.trigger;
            }
            set
            {
                if (this.trigger != null)
                {
                    this.trigger.PropertyChanged -= this.TriggerChanged;
                }

                this.SetProperty(ref this.trigger, value, () => this.Trigger);
                if (value != null)
                {
                    this.trigger.PropertyChanged += this.TriggerChanged;
                }

            }
        }

        public new EventCycleConfigBase Export(object parameters = null)
        {
            var eventcyclebase = (EventCycleConfigBase)this.CreateExportObject();
            this.DoExport(eventcyclebase, parameters);
            return eventcyclebase;
        }

        public new EventCycleConfigDataModelBase ToDataModel()
        {
            var eventcyclebase = (EventCycleConfigDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(eventcyclebase);
            return eventcyclebase;
        }

        public override void ClearDirty()
        {
            if (this.Trigger != null)
            {
                this.Trigger.ClearDirty();
            }

            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is EventCycleConfigDataViewModelBase)
            {
                var that = (EventCycleConfigDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Trigger != null)
                        {
                            result = result && this.Trigger.EqualsViewModel(that.Trigger);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (EventCycleConfigBase)exportModel;
            base.DoExport(exportModel, exportParameters);
            if (this.Trigger != null)
            {
                model.Trigger = this.Trigger.Export(exportParameters);
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (EventCycleConfigDataModelBase)dataModel;
            base.ConvertToDataModel(model);
            if (this.Trigger != null)
            {
                model.Trigger = this.Trigger.ToDataModel();
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void TriggerChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TriggerChangedPartial(sender, e);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void TriggerChangedPartial(object sender, PropertyChangedEventArgs e);

        partial void Initialize(EventCycleConfigDataModelBase dataModel = null);

        partial void Initialize(EventCycleConfigDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(EventCycleConfigBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref EventCycleConfigDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class EventCycleRefConfigDataViewModel : CycleRefConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public EventCycleRefConfigDataViewModel(IMediaShell mediaShell, EventCycleRefConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected EventCycleRefConfigDataViewModel(IMediaShell mediaShell, EventCycleRefConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new EventCycleRefConfig Export(object parameters = null)
        {
            var eventcycleref = (EventCycleRefConfig)this.CreateExportObject();
            this.DoExport(eventcycleref, parameters);
            return eventcycleref;
        }

        public new EventCycleRefConfigDataModel ToDataModel()
        {
            var eventcycleref = (EventCycleRefConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(eventcycleref);
            return eventcycleref;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new EventCycleRefConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is EventCycleRefConfigDataViewModel)
            {
                var that = (EventCycleRefConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new EventCycleRefConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new EventCycleRefConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (EventCycleRefConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (EventCycleRefConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(EventCycleRefConfigDataModel dataModel = null);

        partial void Initialize(EventCycleRefConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(EventCycleRefConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref EventCycleRefConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class GenericTriggerConfigDataViewModel : DataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private ExtendedObservableCollection<GenericEvalDataViewModel> coordinates;

        public GenericTriggerConfigDataViewModel(IMediaShell mediaShell, GenericTriggerConfigDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Coordinates = new ExtendedObservableCollection<GenericEvalDataViewModel>();
            if (dataModel != null)
            {
                foreach (var item in dataModel.Coordinates)
                {
                    var convertedItem = new GenericEvalDataViewModel(mediaShell, item);
                    this.Coordinates.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected GenericTriggerConfigDataViewModel(IMediaShell mediaShell, GenericTriggerConfigDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Coordinates = new ExtendedObservableCollection<GenericEvalDataViewModel>();
            foreach (var item in dataViewModel.Coordinates)
            {
                GenericEvalDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (GenericEvalDataViewModel)item.Clone();
                }

                this.Coordinates.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Coordinates.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<GenericEvalDataViewModel> Coordinates
        {
            get
            {
                return this.coordinates;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.coordinates);
                this.SetProperty(ref this.coordinates, value, () => this.Coordinates);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public GenericTriggerConfig Export(object parameters = null)
        {
            var generictrigger = (GenericTriggerConfig)this.CreateExportObject();
            this.DoExport(generictrigger, parameters);
            return generictrigger;
        }

        public GenericTriggerConfigDataModel ToDataModel()
        {
            var generictrigger = (GenericTriggerConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(generictrigger);
            return generictrigger;
        }

        public override void ClearDirty()
        {
            if (this.Coordinates != null)
            {
                this.Coordinates.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new GenericTriggerConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is GenericTriggerConfigDataViewModel)
            {
                var that = (GenericTriggerConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Coordinates.Count == that.Coordinates.Count;
                        if (result)
                        {
                            foreach (var item in this.Coordinates)
                            {
                                var found = false;
                                foreach (var otherItem in that.Coordinates)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new GenericTriggerConfig();
        }

        protected object CreateDataModelObject()
        {
            return new GenericTriggerConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (GenericTriggerConfig)exportModel;
            foreach (var item in this.Coordinates)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Coordinates.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (GenericTriggerConfigDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            foreach (var item in this.Coordinates)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Coordinates.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(GenericTriggerConfigDataModel dataModel = null);

        partial void Initialize(GenericTriggerConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(GenericTriggerConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref GenericTriggerConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class MasterCycleConfigDataViewModel : CycleConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public MasterCycleConfigDataViewModel(IMediaShell mediaShell, MasterCycleConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected MasterCycleConfigDataViewModel(IMediaShell mediaShell, MasterCycleConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new MasterCycleConfig Export(object parameters = null)
        {
            var mastercycle = (MasterCycleConfig)this.CreateExportObject();
            this.DoExport(mastercycle, parameters);
            return mastercycle;
        }

        public new MasterCycleConfigDataModel ToDataModel()
        {
            var mastercycle = (MasterCycleConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(mastercycle);
            return mastercycle;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new MasterCycleConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is MasterCycleConfigDataViewModel)
            {
                var that = (MasterCycleConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new MasterCycleConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new MasterCycleConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (MasterCycleConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (MasterCycleConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(MasterCycleConfigDataModel dataModel = null);

        partial void Initialize(MasterCycleConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(MasterCycleConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref MasterCycleConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class MasterEventCycleConfigDataViewModel : EventCycleConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public MasterEventCycleConfigDataViewModel(IMediaShell mediaShell, MasterEventCycleConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected MasterEventCycleConfigDataViewModel(IMediaShell mediaShell, MasterEventCycleConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new MasterEventCycleConfig Export(object parameters = null)
        {
            var mastereventcycle = (MasterEventCycleConfig)this.CreateExportObject();
            this.DoExport(mastereventcycle, parameters);
            return mastereventcycle;
        }

        public new MasterEventCycleConfigDataModel ToDataModel()
        {
            var mastereventcycle = (MasterEventCycleConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(mastereventcycle);
            return mastereventcycle;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new MasterEventCycleConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is MasterEventCycleConfigDataViewModel)
            {
                var that = (MasterEventCycleConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new MasterEventCycleConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new MasterEventCycleConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (MasterEventCycleConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (MasterEventCycleConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(MasterEventCycleConfigDataModel dataModel = null);

        partial void Initialize(MasterEventCycleConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(MasterEventCycleConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref MasterEventCycleConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class StandardCycleConfigDataViewModel : CycleConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public StandardCycleConfigDataViewModel(IMediaShell mediaShell, StandardCycleConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected StandardCycleConfigDataViewModel(IMediaShell mediaShell, StandardCycleConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new StandardCycleConfig Export(object parameters = null)
        {
            var standardcycle = (StandardCycleConfig)this.CreateExportObject();
            this.DoExport(standardcycle, parameters);
            return standardcycle;
        }

        public new StandardCycleConfigDataModel ToDataModel()
        {
            var standardcycle = (StandardCycleConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(standardcycle);
            return standardcycle;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new StandardCycleConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is StandardCycleConfigDataViewModel)
            {
                var that = (StandardCycleConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new StandardCycleConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new StandardCycleConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (StandardCycleConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (StandardCycleConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(StandardCycleConfigDataModel dataModel = null);

        partial void Initialize(StandardCycleConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(StandardCycleConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref StandardCycleConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class StandardCycleRefConfigDataViewModel : CycleRefConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public StandardCycleRefConfigDataViewModel(IMediaShell mediaShell, StandardCycleRefConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected StandardCycleRefConfigDataViewModel(IMediaShell mediaShell, StandardCycleRefConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new StandardCycleRefConfig Export(object parameters = null)
        {
            var standardcycleref = (StandardCycleRefConfig)this.CreateExportObject();
            this.DoExport(standardcycleref, parameters);
            return standardcycleref;
        }

        public new StandardCycleRefConfigDataModel ToDataModel()
        {
            var standardcycleref = (StandardCycleRefConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(standardcycleref);
            return standardcycleref;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new StandardCycleRefConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is StandardCycleRefConfigDataViewModel)
            {
                var that = (StandardCycleRefConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new StandardCycleRefConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new StandardCycleRefConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (StandardCycleRefConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (StandardCycleRefConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(StandardCycleRefConfigDataModel dataModel = null);

        partial void Initialize(StandardCycleRefConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(StandardCycleRefConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref StandardCycleRefConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

}
namespace Gorba.Center.Media.Core.DataViewModels.Presentation.Section
{
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Presentation.Cycle;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Media.Core.Controllers;
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.ComponentModel;
    using System.Linq;
    public partial class ImageSectionConfigDataViewModel : SectionConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> filename;

        private DataValue<int> frame;

        public ImageSectionConfigDataViewModel(IMediaShell mediaShell, ImageSectionConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Filename = new DataValue<string>(string.Empty);
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.Frame = new DataValue<int>(default(int));
            this.Frame.PropertyChanged += this.FrameChanged;
            if (dataModel != null)
            {
                this.Filename.Value = dataModel.Filename;
                this.Frame.Value = dataModel.Frame;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected ImageSectionConfigDataViewModel(IMediaShell mediaShell, ImageSectionConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Filename = (DataValue<string>)dataViewModel.Filename.Clone();
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.Frame = (DataValue<int>)dataViewModel.Frame.Clone();
            this.Frame.PropertyChanged += this.FrameChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Filename.IsDirty || this.Frame.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 1)]
        public DataValue<string> Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filename);
                if (this.filename != null)
                {
                    this.filename.PropertyChanged -= this.FilenameChanged;
                }

                this.SetProperty(ref this.filename, value, () => this.Filename);
                if (value != null)
                {
                    this.filename.PropertyChanged += this.FilenameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 1, GroupOrderIndex = 1)]
        public DataValue<int> Frame
        {
            get
            {
                return this.frame;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.frame);
                if (this.frame != null)
                {
                    this.frame.PropertyChanged -= this.FrameChanged;
                }

                this.SetProperty(ref this.frame, value, () => this.Frame);
                if (value != null)
                {
                    this.frame.PropertyChanged += this.FrameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ImageSectionConfig Export(object parameters = null)
        {
            var imagesection = (ImageSectionConfig)this.CreateExportObject();
            this.DoExport(imagesection, parameters);
            return imagesection;
        }

        public new ImageSectionConfigDataModel ToDataModel()
        {
            var imagesection = (ImageSectionConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(imagesection);
            return imagesection;
        }

        public override void ClearDirty()
        {
            if (this.Filename != null)
            {
                this.Filename.ClearDirty();
            }

            if (this.Frame != null)
            {
                this.Frame.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new ImageSectionConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is ImageSectionConfigDataViewModel)
            {
                var that = (ImageSectionConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Filename.EqualsValue(that.Filename);
                        result = result && this.Frame.EqualsValue(that.Frame);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new ImageSectionConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new ImageSectionConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (ImageSectionConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Filename = this.Filename.Value;
            model.Frame = this.Frame.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (ImageSectionConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Filename = this.Filename.Value;
            model.Frame = this.Frame.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FilenameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Filename);
        }

        private void FrameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Frame);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(ImageSectionConfigDataModel dataModel = null);

        partial void Initialize(ImageSectionConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(ImageSectionConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref ImageSectionConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class MasterSectionConfigDataViewModel : SectionConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public MasterSectionConfigDataViewModel(IMediaShell mediaShell, MasterSectionConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected MasterSectionConfigDataViewModel(IMediaShell mediaShell, MasterSectionConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new MasterSectionConfig Export(object parameters = null)
        {
            var mastersection = (MasterSectionConfig)this.CreateExportObject();
            this.DoExport(mastersection, parameters);
            return mastersection;
        }

        public new MasterSectionConfigDataModel ToDataModel()
        {
            var mastersection = (MasterSectionConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(mastersection);
            return mastersection;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new MasterSectionConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is MasterSectionConfigDataViewModel)
            {
                var that = (MasterSectionConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new MasterSectionConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new MasterSectionConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (MasterSectionConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (MasterSectionConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(MasterSectionConfigDataModel dataModel = null);

        partial void Initialize(MasterSectionConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(MasterSectionConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref MasterSectionConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class MultiSectionConfigDataViewModel : SectionConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> language;

        private DataValue<int> table;

        private DataValue<int> rowsperpage;

        private DataValue<int> maxpages;

        private DataValue<PageMode> mode;

        public MultiSectionConfigDataViewModel(IMediaShell mediaShell, MultiSectionConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Language = new DataValue<int>(default(int));
            this.Language.PropertyChanged += this.LanguageChanged;
            this.Table = new DataValue<int>(default(int));
            this.Table.PropertyChanged += this.TableChanged;
            this.RowsPerPage = new DataValue<int>(default(int));
            this.RowsPerPage.PropertyChanged += this.RowsPerPageChanged;
            this.MaxPages = new DataValue<int>(-1);
            this.MaxPages.PropertyChanged += this.MaxPagesChanged;
            this.Mode = new DataValue<PageMode>(new PageMode());
            this.Mode.PropertyChanged += this.ModeChanged;
            if (dataModel != null)
            {
                this.Language.Value = dataModel.Language;
                this.Table.Value = dataModel.Table;
                this.RowsPerPage.Value = dataModel.RowsPerPage;
                this.MaxPages.Value = dataModel.MaxPages;
                this.Mode.Value = dataModel.Mode;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected MultiSectionConfigDataViewModel(IMediaShell mediaShell, MultiSectionConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Language = (DataValue<int>)dataViewModel.Language.Clone();
            this.Language.PropertyChanged += this.LanguageChanged;
            this.Table = (DataValue<int>)dataViewModel.Table.Clone();
            this.Table.PropertyChanged += this.TableChanged;
            this.RowsPerPage = (DataValue<int>)dataViewModel.RowsPerPage.Clone();
            this.RowsPerPage.PropertyChanged += this.RowsPerPageChanged;
            this.MaxPages = (DataValue<int>)dataViewModel.MaxPages.Clone();
            this.MaxPages.PropertyChanged += this.MaxPagesChanged;
            this.Mode = (DataValue<PageMode>)dataViewModel.Mode.Clone();
            this.Mode.PropertyChanged += this.ModeChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Language.IsDirty || this.Table.IsDirty || this.RowsPerPage.IsDirty || this.MaxPages.IsDirty || this.Mode.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 1, GroupOrderIndex = 1)]
        public DataValue<int> Language
        {
            get
            {
                return this.language;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.language);
                if (this.language != null)
                {
                    this.language.PropertyChanged -= this.LanguageChanged;
                }

                this.SetProperty(ref this.language, value, () => this.Language);
                if (value != null)
                {
                    this.language.PropertyChanged += this.LanguageChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 1)]
        public DataValue<int> Table
        {
            get
            {
                return this.table;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.table);
                if (this.table != null)
                {
                    this.table.PropertyChanged -= this.TableChanged;
                }

                this.SetProperty(ref this.table, value, () => this.Table);
                if (value != null)
                {
                    this.table.PropertyChanged += this.TableChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 4, GroupOrderIndex = 1)]
        public DataValue<int> RowsPerPage
        {
            get
            {
                return this.rowsperpage;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.rowsperpage);
                if (this.rowsperpage != null)
                {
                    this.rowsperpage.PropertyChanged -= this.RowsPerPageChanged;
                }

                this.SetProperty(ref this.rowsperpage, value, () => this.RowsPerPage);
                if (value != null)
                {
                    this.rowsperpage.PropertyChanged += this.RowsPerPageChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 3, GroupOrderIndex = 1)]
        public DataValue<int> MaxPages
        {
            get
            {
                return this.maxpages;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.maxpages);
                if (this.maxpages != null)
                {
                    this.maxpages.PropertyChanged -= this.MaxPagesChanged;
                }

                this.SetProperty(ref this.maxpages, value, () => this.MaxPages);
                if (value != null)
                {
                    this.maxpages.PropertyChanged += this.MaxPagesChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 2, GroupOrderIndex = 1)]
        public DataValue<PageMode> Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.mode);
                if (this.mode != null)
                {
                    this.mode.PropertyChanged -= this.ModeChanged;
                }

                this.SetProperty(ref this.mode, value, () => this.Mode);
                if (value != null)
                {
                    this.mode.PropertyChanged += this.ModeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new MultiSectionConfig Export(object parameters = null)
        {
            var multisection = (MultiSectionConfig)this.CreateExportObject();
            this.DoExport(multisection, parameters);
            return multisection;
        }

        public new MultiSectionConfigDataModel ToDataModel()
        {
            var multisection = (MultiSectionConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(multisection);
            return multisection;
        }

        public override void ClearDirty()
        {
            if (this.Language != null)
            {
                this.Language.ClearDirty();
            }

            if (this.Table != null)
            {
                this.Table.ClearDirty();
            }

            if (this.RowsPerPage != null)
            {
                this.RowsPerPage.ClearDirty();
            }

            if (this.MaxPages != null)
            {
                this.MaxPages.ClearDirty();
            }

            if (this.Mode != null)
            {
                this.Mode.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new MultiSectionConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is MultiSectionConfigDataViewModel)
            {
                var that = (MultiSectionConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Language.EqualsValue(that.Language);
                        result = result && this.Table.EqualsValue(that.Table);
                        result = result && this.RowsPerPage.EqualsValue(that.RowsPerPage);
                        result = result && this.MaxPages.EqualsValue(that.MaxPages);
                        result = result && this.Mode.EqualsValue(that.Mode);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new MultiSectionConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new MultiSectionConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (MultiSectionConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Language = this.Language.Value;
            model.Table = this.Table.Value;
            model.RowsPerPage = this.RowsPerPage.Value;
            model.MaxPages = this.MaxPages.Value;
            model.Mode = this.Mode.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (MultiSectionConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Language = this.Language.Value;
            model.Table = this.Table.Value;
            model.RowsPerPage = this.RowsPerPage.Value;
            model.MaxPages = this.MaxPages.Value;
            model.Mode = this.Mode.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void LanguageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Language);
        }

        private void TableChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Table);
        }

        private void RowsPerPageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.RowsPerPage);
        }

        private void MaxPagesChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.MaxPages);
        }

        private void ModeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Mode);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(MultiSectionConfigDataModel dataModel = null);

        partial void Initialize(MultiSectionConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(MultiSectionConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref MultiSectionConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class PoolSectionConfigDataViewModel : SectionConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<VideoEndMode> videoendmode;

        private DataValue<int> frame;

        private string poolReferenceName;

        private PoolConfigDataViewModel poolReference;

        public PoolSectionConfigDataViewModel(IMediaShell mediaShell, PoolSectionConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            // Pool
            this.VideoEndMode = new DataValue<VideoEndMode>(new VideoEndMode());
            this.VideoEndMode.PropertyChanged += this.VideoEndModeChanged;
            this.Frame = new DataValue<int>(default(int));
            this.Frame.PropertyChanged += this.FrameChanged;
            if (dataModel != null)
            {
                this.poolReferenceName = dataModel.Pool;
                this.VideoEndMode.Value = dataModel.VideoEndMode;
                this.Frame.Value = dataModel.Frame;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected PoolSectionConfigDataViewModel(IMediaShell mediaShell, PoolSectionConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.PoolName = dataViewModel.PoolName;
            this.VideoEndMode = (DataValue<VideoEndMode>)dataViewModel.VideoEndMode.Clone();
            this.VideoEndMode.PropertyChanged += this.VideoEndModeChanged;
            this.Frame = (DataValue<int>)dataViewModel.Frame.Clone();
            this.Frame.PropertyChanged += this.FrameChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.VideoEndMode.IsDirty || this.Frame.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 1)]
        public PoolConfigDataViewModel Pool
        {
            get
            {
                if (this.poolReference == null)
                {
                    this.poolReference = this.FindReference();
                    if (this.poolReference != null)
                    {
                        this.poolReference.PropertyChanged += this.PoolChanged;
                    }

                }

                return this.poolReference;
            }
            set
            {
                if (this.Pool != null)
                {
                    this.Pool.PropertyChanged -= this.PoolChanged;
                }

                this.SetProperty(ref this.poolReference, value, () => this.Pool);
                if (value != null)
                {
                    this.PoolName = value.Name.Value;
                    this.Pool.PropertyChanged += this.PoolChanged;
                }

            }
        }

        public string PoolName
        {
            get
            {
                return this.poolReferenceName;
            }
            private set
            {
                this.SetProperty(ref this.poolReferenceName, value, () => this.Pool);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 2, GroupOrderIndex = 1)]
        public DataValue<VideoEndMode> VideoEndMode
        {
            get
            {
                return this.videoendmode;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.videoendmode);
                if (this.videoendmode != null)
                {
                    this.videoendmode.PropertyChanged -= this.VideoEndModeChanged;
                }

                this.SetProperty(ref this.videoendmode, value, () => this.VideoEndMode);
                if (value != null)
                {
                    this.videoendmode.PropertyChanged += this.VideoEndModeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 1, GroupOrderIndex = 1)]
        public DataValue<int> Frame
        {
            get
            {
                return this.frame;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.frame);
                if (this.frame != null)
                {
                    this.frame.PropertyChanged -= this.FrameChanged;
                }

                this.SetProperty(ref this.frame, value, () => this.Frame);
                if (value != null)
                {
                    this.frame.PropertyChanged += this.FrameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new PoolSectionConfig Export(object parameters = null)
        {
            var poolsection = (PoolSectionConfig)this.CreateExportObject();
            this.DoExport(poolsection, parameters);
            return poolsection;
        }

        public new PoolSectionConfigDataModel ToDataModel()
        {
            var poolsection = (PoolSectionConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(poolsection);
            return poolsection;
        }

        public override void ClearDirty()
        {
            if (this.Pool != null)
            {
                this.Pool.ClearDirty();
            }

            if (this.VideoEndMode != null)
            {
                this.VideoEndMode.ClearDirty();
            }

            if (this.Frame != null)
            {
                this.Frame.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new PoolSectionConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is PoolSectionConfigDataViewModel)
            {
                var that = (PoolSectionConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Pool.EqualsViewModel(that.Pool);
                        result = result && this.VideoEndMode.EqualsValue(that.VideoEndMode);
                        result = result && this.Frame.EqualsValue(that.Frame);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new PoolSectionConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new PoolSectionConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (PoolSectionConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Pool = this.PoolName;
            model.VideoEndMode = this.VideoEndMode.Value;
            model.Frame = this.Frame.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (PoolSectionConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Pool = this.PoolName;
            model.VideoEndMode = this.VideoEndMode.Value;
            model.Frame = this.Frame.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void PoolChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Pool != null)
            {
                this.PoolName = this.Pool.Name.Value;
            }

            this.RaisePropertyChanged(() => this.Pool);
        }

        private void VideoEndModeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.VideoEndMode);
        }

        private void FrameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Frame);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(PoolSectionConfigDataModel dataModel = null);

        partial void Initialize(PoolSectionConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(PoolSectionConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref PoolSectionConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class SectionConfigDataViewModelBase : ReferenceTrackedDataViewModelBase, ICloneable
    {
        private readonly IMediaShell mediaShell;

        private DynamicDataValue<bool> enabled;

        private DataValue<TimeSpan> duration;

        private string layoutReferenceName;

        private LayoutConfigDataViewModelBase layoutReference;

        public SectionConfigDataViewModelBase(IMediaShell mediaShell, SectionConfigDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Enabled = new DynamicDataValue<bool>(true);
            this.Enabled.PropertyChanged += this.EnabledChanged;
            this.Duration = new DataValue<TimeSpan>(new TimeSpan());
            this.Duration.PropertyChanged += this.DurationChanged;
            // Layout
            if (dataModel != null)
            {
                if (dataModel.Enabled != null)
                {
                    this.Enabled.Value = dataModel.Enabled;
                }

                if (dataModel.EnabledProperty != null)
                {
                    this.Enabled.Formula = this.CreateEvalDataViewModel(dataModel.EnabledProperty.Evaluation);
                }

                this.Duration.Value = dataModel.Duration;
                this.layoutReferenceName = dataModel.Layout;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected SectionConfigDataViewModelBase(IMediaShell mediaShell, SectionConfigDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Enabled = (DynamicDataValue<bool>)dataViewModel.Enabled.Clone();
            this.Enabled.PropertyChanged += this.EnabledChanged;
            this.Duration = (DataValue<TimeSpan>)dataViewModel.Duration.Clone();
            this.Duration.PropertyChanged += this.DurationChanged;
            this.LayoutName = dataViewModel.LayoutName;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Enabled.IsDirty || this.Duration.IsDirty;
            }
        }

        [UserVisibleProperty("Common", OrderIndex = 0, GroupOrderIndex = 0)]
        public DynamicDataValue<bool> Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.enabled);
                if (this.enabled != null)
                {
                    this.enabled.PropertyChanged -= this.EnabledChanged;
                }

                this.SetProperty(ref this.enabled, value, () => this.Enabled);
                if (value != null)
                {
                    this.enabled.PropertyChanged += this.EnabledChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Common", OrderIndex = 1, GroupOrderIndex = 0)]
        public DataValue<TimeSpan> Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.duration);
                if (this.duration != null)
                {
                    this.duration.PropertyChanged -= this.DurationChanged;
                }

                this.SetProperty(ref this.duration, value, () => this.Duration);
                if (value != null)
                {
                    this.duration.PropertyChanged += this.DurationChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Common", OrderIndex = 2, GroupOrderIndex = 0)]
        public LayoutConfigDataViewModelBase Layout
        {
            get
            {
                if (this.layoutReference == null)
                {
                    this.layoutReference = this.FindReference();
                    if (this.layoutReference != null)
                    {
                        this.layoutReference.PropertyChanged += this.LayoutChanged;
                    }

                }

                return this.layoutReference;
            }
            set
            {
                var parentCycle = this.SearchParentCycle();
                if (this.Layout != null)
                {
                    this.Layout.PropertyChanged -= this.LayoutChanged;
                    var oldLayout = this.Layout as LayoutConfigDataViewModel;
                    if (oldLayout != null && parentCycle != null)
                    {
                        var reference = oldLayout.GetCycleSectionReference(this, parentCycle);
                        if (reference != null)
                        {
                            oldLayout.CycleSectionReferences.Remove(reference);
                        }

                    }

                }

                this.SetProperty(ref this.layoutReference, value, () => this.Layout);
                if (value != null)
                {
                    this.LayoutName = value.Name.Value;
                    this.Layout.PropertyChanged += this.LayoutChanged;
                    var newLayout = this.Layout as LayoutConfigDataViewModel;
                    if (newLayout != null && parentCycle != null)
                    {
                        var reference = new LayoutCycleSectionRefDataViewModel(parentCycle, this);
                        newLayout.CycleSectionReferences.Add(reference);
                    }

                }

            }
        }

        public string LayoutName
        {
            get
            {
                return this.layoutReferenceName;
            }
            private set
            {
                this.SetProperty(ref this.layoutReferenceName, value, () => this.Layout);
            }
        }

        public SectionConfigBase Export(object parameters = null)
        {
            var sectionbase = (SectionConfigBase)this.CreateExportObject();
            this.DoExport(sectionbase, parameters);
            return sectionbase;
        }

        public SectionConfigDataModelBase ToDataModel()
        {
            var sectionbase = (SectionConfigDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(sectionbase);
            return sectionbase;
        }

        public override void ClearDirty()
        {
            if (this.Enabled != null)
            {
                this.Enabled.ClearDirty();
            }

            if (this.Duration != null)
            {
                this.Duration.ClearDirty();
            }

            if (this.Layout != null)
            {
                this.Layout.ClearDirty();
            }

            base.ClearDirty();
        }

        public abstract object Clone();

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is SectionConfigDataViewModelBase)
            {
                var that = (SectionConfigDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Enabled.EqualsValue(that.Enabled);
                        result = result && this.Duration.EqualsValue(that.Duration);
                        result = result && this.Layout.EqualsViewModel(that.Layout);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected abstract object CreateExportObject();

        protected abstract object CreateDataModelObject();

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (SectionConfigBase)exportModel;
            if (this.Enabled.Formula != null)
            {
                var formulaController = ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.FormulaController;
                try
                {
                    var formulaString = ((EvalDataViewModelBase)this.Enabled.Formula).HumanReadable();
                    if (!formulaString.StartsWith("="))
                    {
                        formulaString = formulaString.Insert(0, "=");
                    }

                    formulaController.ParseFormula(formulaString);
                    var enabledEval = Enabled.Formula as CodeConversionEvalDataViewModel;
                    if (enabledEval != null)
                    {
                        if (this.CsvMappingCompatibilityRequired(exportParameters))
                        {
                            var csvMapping = new CsvMappingEval {
                                FileName = "codeconversion.csv",
                                DefaultValue = new DynamicProperty {
                                    Evaluation = new GenericEval {
                                        Column = 0,
                                        Language = 0,
                                        Table = 10,
                                        Row = 0
                                    }
                                }
                            };
                            var match0 = new MatchDynamicProperty {
                                Column = 0,
                                Evaluation = new GenericEval {
                                    Column = 1,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            var match1 = new MatchDynamicProperty {
                                Column = 1,
                                Evaluation = new GenericEval {
                                    Column = 0,
                                    Language = 0,
                                    Table = 10,
                                    Row = 0
                                }
                            };
                            csvMapping.Matches.Add(match0);
                            csvMapping.Matches.Add(match1);
                            csvMapping.OutputFormat = enabledEval.UseImage.Value ? "{2}" : "{3}";
                            model.EnabledProperty = new DynamicProperty(csvMapping);
                        }
                        else
                        {
                            model.EnabledProperty = new DynamicProperty(((EvalDataViewModelBase)this.Enabled.Formula).Export(exportParameters));
                        }

                    }
                    else
                    {
                        model.EnabledProperty = new DynamicProperty(((EvalDataViewModelBase)this.Enabled.Formula).Export(exportParameters));
                    }

                }
                catch
                {
                    model.Enabled = this.Enabled.Value;
                }
            }
            else
            {
                model.Enabled = this.Enabled.Value;
            }

            model.Duration = this.Duration.Value;
            model.Layout = this.LayoutName;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (SectionConfigDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            model.Enabled = this.Enabled.Value;
            if (this.Enabled.Formula != null)
            {
                model.EnabledProperty = new DynamicPropertyDataModel(((EvalDataViewModelBase)this.Enabled.Formula).ToDataModel());
            }

            model.Duration = this.Duration.Value;
            model.Layout = this.LayoutName;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void EnabledChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Enabled);
        }

        private void DurationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Duration);
        }

        private void LayoutChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Layout != null)
            {
                this.LayoutName = this.Layout.Name.Value;
            }

            this.RaisePropertyChanged(() => this.Layout);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(SectionConfigDataModelBase dataModel = null);

        partial void Initialize(SectionConfigDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(SectionConfigBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref SectionConfigDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class StandardSectionConfigDataViewModel : SectionConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public StandardSectionConfigDataViewModel(IMediaShell mediaShell, StandardSectionConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected StandardSectionConfigDataViewModel(IMediaShell mediaShell, StandardSectionConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new StandardSectionConfig Export(object parameters = null)
        {
            var standardsection = (StandardSectionConfig)this.CreateExportObject();
            this.DoExport(standardsection, parameters);
            return standardsection;
        }

        public new StandardSectionConfigDataModel ToDataModel()
        {
            var standardsection = (StandardSectionConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(standardsection);
            return standardsection;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new StandardSectionConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is StandardSectionConfigDataViewModel)
            {
                var that = (StandardSectionConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new StandardSectionConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new StandardSectionConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (StandardSectionConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (StandardSectionConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(StandardSectionConfigDataModel dataModel = null);

        partial void Initialize(StandardSectionConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(StandardSectionConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref StandardSectionConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class VideoSectionConfigDataViewModel : SectionConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> videouri;

        private DataValue<VideoEndMode> videoendmode;

        private DataValue<int> frame;

        public VideoSectionConfigDataViewModel(IMediaShell mediaShell, VideoSectionConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.VideoUri = new DataValue<string>(string.Empty);
            this.VideoUri.PropertyChanged += this.VideoUriChanged;
            this.VideoEndMode = new DataValue<VideoEndMode>(new VideoEndMode());
            this.VideoEndMode.PropertyChanged += this.VideoEndModeChanged;
            this.Frame = new DataValue<int>(default(int));
            this.Frame.PropertyChanged += this.FrameChanged;
            if (dataModel != null)
            {
                this.VideoUri.Value = dataModel.VideoUri;
                this.VideoEndMode.Value = dataModel.VideoEndMode;
                this.Frame.Value = dataModel.Frame;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected VideoSectionConfigDataViewModel(IMediaShell mediaShell, VideoSectionConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.VideoUri = (DataValue<string>)dataViewModel.VideoUri.Clone();
            this.VideoUri.PropertyChanged += this.VideoUriChanged;
            this.VideoEndMode = (DataValue<VideoEndMode>)dataViewModel.VideoEndMode.Clone();
            this.VideoEndMode.PropertyChanged += this.VideoEndModeChanged;
            this.Frame = (DataValue<int>)dataViewModel.Frame.Clone();
            this.Frame.PropertyChanged += this.FrameChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.VideoUri.IsDirty || this.VideoEndMode.IsDirty || this.Frame.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 1)]
        public DataValue<string> VideoUri
        {
            get
            {
                return this.videouri;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.videouri);
                if (this.videouri != null)
                {
                    this.videouri.PropertyChanged -= this.VideoUriChanged;
                }

                this.SetProperty(ref this.videouri, value, () => this.VideoUri);
                if (value != null)
                {
                    this.videouri.PropertyChanged += this.VideoUriChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 2, GroupOrderIndex = 1)]
        public DataValue<VideoEndMode> VideoEndMode
        {
            get
            {
                return this.videoendmode;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.videoendmode);
                if (this.videoendmode != null)
                {
                    this.videoendmode.PropertyChanged -= this.VideoEndModeChanged;
                }

                this.SetProperty(ref this.videoendmode, value, () => this.VideoEndMode);
                if (value != null)
                {
                    this.videoendmode.PropertyChanged += this.VideoEndModeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 1, GroupOrderIndex = 1)]
        public DataValue<int> Frame
        {
            get
            {
                return this.frame;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.frame);
                if (this.frame != null)
                {
                    this.frame.PropertyChanged -= this.FrameChanged;
                }

                this.SetProperty(ref this.frame, value, () => this.Frame);
                if (value != null)
                {
                    this.frame.PropertyChanged += this.FrameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new VideoSectionConfig Export(object parameters = null)
        {
            var videosection = (VideoSectionConfig)this.CreateExportObject();
            this.DoExport(videosection, parameters);
            return videosection;
        }

        public new VideoSectionConfigDataModel ToDataModel()
        {
            var videosection = (VideoSectionConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(videosection);
            return videosection;
        }

        public override void ClearDirty()
        {
            if (this.VideoUri != null)
            {
                this.VideoUri.ClearDirty();
            }

            if (this.VideoEndMode != null)
            {
                this.VideoEndMode.ClearDirty();
            }

            if (this.Frame != null)
            {
                this.Frame.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new VideoSectionConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is VideoSectionConfigDataViewModel)
            {
                var that = (VideoSectionConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.VideoUri.EqualsValue(that.VideoUri);
                        result = result && this.VideoEndMode.EqualsValue(that.VideoEndMode);
                        result = result && this.Frame.EqualsValue(that.Frame);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new VideoSectionConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new VideoSectionConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (VideoSectionConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.VideoUri = this.VideoUri.Value;
            model.VideoEndMode = this.VideoEndMode.Value;
            model.Frame = this.Frame.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (VideoSectionConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.VideoUri = this.VideoUri.Value;
            model.VideoEndMode = this.VideoEndMode.Value;
            model.Frame = this.Frame.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void VideoUriChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.VideoUri);
        }

        private void VideoEndModeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.VideoEndMode);
        }

        private void FrameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Frame);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(VideoSectionConfigDataModel dataModel = null);

        partial void Initialize(VideoSectionConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(VideoSectionConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref VideoSectionConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class WebmediaSectionConfigDataViewModel : SectionConfigDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> filename;

        private DataValue<VideoEndMode> videoendmode;

        public WebmediaSectionConfigDataViewModel(IMediaShell mediaShell, WebmediaSectionConfigDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Filename = new DataValue<string>(string.Empty);
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.VideoEndMode = new DataValue<VideoEndMode>(new VideoEndMode());
            this.VideoEndMode.PropertyChanged += this.VideoEndModeChanged;
            if (dataModel != null)
            {
                this.Filename.Value = dataModel.Filename;
                this.VideoEndMode.Value = dataModel.VideoEndMode;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected WebmediaSectionConfigDataViewModel(IMediaShell mediaShell, WebmediaSectionConfigDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Filename = (DataValue<string>)dataViewModel.Filename.Clone();
            this.Filename.PropertyChanged += this.FilenameChanged;
            this.VideoEndMode = (DataValue<VideoEndMode>)dataViewModel.VideoEndMode.Clone();
            this.VideoEndMode.PropertyChanged += this.VideoEndModeChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Filename.IsDirty || this.VideoEndMode.IsDirty;
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 1)]
        public DataValue<string> Filename
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filename);
                if (this.filename != null)
                {
                    this.filename.PropertyChanged -= this.FilenameChanged;
                }

                this.SetProperty(ref this.filename, value, () => this.Filename);
                if (value != null)
                {
                    this.filename.PropertyChanged += this.FilenameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Content", OrderIndex = 0, GroupOrderIndex = 1)]
        public DataValue<VideoEndMode> VideoEndMode
        {
            get
            {
                return this.videoendmode;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.videoendmode);
                if (this.videoendmode != null)
                {
                    this.videoendmode.PropertyChanged -= this.VideoEndModeChanged;
                }

                this.SetProperty(ref this.videoendmode, value, () => this.VideoEndMode);
                if (value != null)
                {
                    this.videoendmode.PropertyChanged += this.VideoEndModeChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new WebmediaSectionConfig Export(object parameters = null)
        {
            var webmediasection = (WebmediaSectionConfig)this.CreateExportObject();
            this.DoExport(webmediasection, parameters);
            return webmediasection;
        }

        public new WebmediaSectionConfigDataModel ToDataModel()
        {
            var webmediasection = (WebmediaSectionConfigDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(webmediasection);
            return webmediasection;
        }

        public override void ClearDirty()
        {
            if (this.Filename != null)
            {
                this.Filename.ClearDirty();
            }

            if (this.VideoEndMode != null)
            {
                this.VideoEndMode.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new WebmediaSectionConfigDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is WebmediaSectionConfigDataViewModel)
            {
                var that = (WebmediaSectionConfigDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Filename.EqualsValue(that.Filename);
                        result = result && this.VideoEndMode.EqualsValue(that.VideoEndMode);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new WebmediaSectionConfig();
        }

        protected override object CreateDataModelObject()
        {
            return new WebmediaSectionConfigDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (WebmediaSectionConfig)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Filename = this.Filename.Value;
            model.VideoEndMode = this.VideoEndMode.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (WebmediaSectionConfigDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Filename = this.Filename.Value;
            model.VideoEndMode = this.VideoEndMode.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FilenameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Filename);
        }

        private void VideoEndModeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.VideoEndMode);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(WebmediaSectionConfigDataModel dataModel = null);

        partial void Initialize(WebmediaSectionConfigDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(WebmediaSectionConfig model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref WebmediaSectionConfigDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

}
namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Compatibility;
    using Gorba.Center.Media.Core.DataViewModels.Eval;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Eval;
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.Models.Presentation;
    using Gorba.Center.Media.Core.Models.Presentation.Section;
    using Gorba.Center.Media.Core.Models.Presentation.Cycle;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Cycle;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Center.Common.Wpf.Framework;
    using Gorba.Center.Media.Core.Controllers;
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.ComponentModel;
    using System.Linq;
    public partial class AndEvalDataViewModel : CollectionEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public AndEvalDataViewModel(IMediaShell mediaShell, AndEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected AndEvalDataViewModel(IMediaShell mediaShell, AndEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new AndEval Export(object parameters = null)
        {
            var and = (AndEval)this.CreateExportObject();
            this.DoExport(and, parameters);
            return and;
        }

        public new AndEvalDataModel ToDataModel()
        {
            var and = (AndEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(and);
            return and;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new AndEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is AndEvalDataViewModel)
            {
                var that = (AndEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new AndEval();
        }

        protected override object CreateDataModelObject()
        {
            return new AndEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (AndEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (AndEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(AndEvalDataModel dataModel = null);

        partial void Initialize(AndEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(AndEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref AndEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class CollectionEvalDataViewModelBase : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private ExtendedObservableCollection<EvalDataViewModelBase> conditions;

        public CollectionEvalDataViewModelBase(IMediaShell mediaShell, CollectionEvalDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Conditions = new ExtendedObservableCollection<EvalDataViewModelBase>();
            if (dataModel != null)
            {
                foreach (var item in dataModel.Conditions)
                {
                    var typeName = item.GetType().Name.Replace("DataModel", "DataViewModel");
                    var assembly = this.GetType().Assembly;
                    var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                    if (type == null)
                    {
                        throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, item.GetType().Name));
                    }

                    var convertedItem = (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, item);
                    this.Conditions.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected CollectionEvalDataViewModelBase(IMediaShell mediaShell, CollectionEvalDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Conditions = new ExtendedObservableCollection<EvalDataViewModelBase>();
            foreach (var item in dataViewModel.Conditions)
            {
                EvalDataViewModelBase clonedItem = null;
                if (item != null)
                {
                    clonedItem = (EvalDataViewModelBase)item.Clone();
                }

                this.Conditions.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Conditions.IsDirty;
            }
        }

        public ExtendedObservableCollection<EvalDataViewModelBase> Conditions
        {
            get
            {
                return this.conditions;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.conditions);
                this.SetProperty(ref this.conditions, value, () => this.Conditions);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new CollectionEvalBase Export(object parameters = null)
        {
            var collectionbase = (CollectionEvalBase)this.CreateExportObject();
            this.DoExport(collectionbase, parameters);
            return collectionbase;
        }

        public new CollectionEvalDataModelBase ToDataModel()
        {
            var collectionbase = (CollectionEvalDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(collectionbase);
            return collectionbase;
        }

        public override void ClearDirty()
        {
            if (this.Conditions != null)
            {
                this.Conditions.ClearDirty();
            }

            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is CollectionEvalDataViewModelBase)
            {
                var that = (CollectionEvalDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Conditions.Count == that.Conditions.Count;
                        if (result)
                        {
                            foreach (var item in this.Conditions)
                            {
                                var found = false;
                                foreach (var otherItem in that.Conditions)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (CollectionEvalBase)exportModel;
            foreach (var item in this.Conditions)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Conditions.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CollectionEvalDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            foreach (var item in this.Conditions)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Conditions.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(CollectionEvalDataModelBase dataModel = null);

        partial void Initialize(CollectionEvalDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(CollectionEvalBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref CollectionEvalDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class ConstantEvalDataViewModel : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> value;

        public ConstantEvalDataViewModel(IMediaShell mediaShell, ConstantEvalDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Value = new DataValue<string>(string.Empty);
            this.Value.PropertyChanged += this.ValueChanged;
            if (dataModel != null)
            {
                this.Value.Value = dataModel.Value;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected ConstantEvalDataViewModel(IMediaShell mediaShell, ConstantEvalDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Value = (DataValue<string>)dataViewModel.Value.Clone();
            this.Value.PropertyChanged += this.ValueChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Value.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.value);
                if (this.value != null)
                {
                    this.value.PropertyChanged -= this.ValueChanged;
                }

                this.SetProperty(ref this.value, value, () => this.Value);
                if (value != null)
                {
                    this.value.PropertyChanged += this.ValueChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new ConstantEval Export(object parameters = null)
        {
            var constant = (ConstantEval)this.CreateExportObject();
            this.DoExport(constant, parameters);
            return constant;
        }

        public new ConstantEvalDataModel ToDataModel()
        {
            var constant = (ConstantEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(constant);
            return constant;
        }

        public override void ClearDirty()
        {
            if (this.Value != null)
            {
                this.Value.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new ConstantEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is ConstantEvalDataViewModel)
            {
                var that = (ConstantEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Value.EqualsValue(that.Value);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new ConstantEval();
        }

        protected override object CreateDataModelObject()
        {
            return new ConstantEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (ConstantEval)exportModel;
            model.Value = this.Value.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (ConstantEvalDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Value = this.Value.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Value);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(ConstantEvalDataModel dataModel = null);

        partial void Initialize(ConstantEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(ConstantEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref ConstantEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class ContainerEvalDataViewModelBase : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private EvalDataViewModelBase evaluation;

        public ContainerEvalDataViewModelBase(IMediaShell mediaShell, ContainerEvalDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.evaluation = this.CreateEvalDataViewModel(dataModel.Evaluation);
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected ContainerEvalDataViewModelBase(IMediaShell mediaShell, ContainerEvalDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            var clonedEvaluation = dataViewModel.Evaluation;
            if (clonedEvaluation != null)
            {
                this.Evaluation = (EvalDataViewModelBase)clonedEvaluation.Clone();
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || (this.Evaluation != null && this.Evaluation.IsDirty);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase Evaluation
        {
            get
            {
                return this.evaluation;
            }
            set
            {
                if (this.evaluation != null)
                {
                    this.evaluation.PropertyChanged -= this.EvaluationChanged;
                }

                this.SetProperty(ref this.evaluation, value, () => this.Evaluation);
                if (value != null)
                {
                    this.evaluation.PropertyChanged += this.EvaluationChanged;
                }

            }
        }

        public new ContainerEvalBase Export(object parameters = null)
        {
            var containerbase = (ContainerEvalBase)this.CreateExportObject();
            this.DoExport(containerbase, parameters);
            return containerbase;
        }

        public new ContainerEvalDataModelBase ToDataModel()
        {
            var containerbase = (ContainerEvalDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(containerbase);
            return containerbase;
        }

        public override void ClearDirty()
        {
            if (this.Evaluation != null)
            {
                this.Evaluation.ClearDirty();
            }

            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is ContainerEvalDataViewModelBase)
            {
                var that = (ContainerEvalDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Evaluation != null)
                        {
                            result = result && this.Evaluation.EqualsViewModel(that.Evaluation);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (ContainerEvalBase)exportModel;
            if (this.Evaluation != null)
            {
                model.Evaluation = this.Evaluation.Export(exportParameters);
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (ContainerEvalDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            if (this.Evaluation != null)
            {
                model.Evaluation = this.Evaluation.ToDataModel();
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void EvaluationChanged(object sender, PropertyChangedEventArgs e)
        {
            this.EvaluationChangedPartial(sender, e);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void EvaluationChangedPartial(object sender, PropertyChangedEventArgs e);

        partial void Initialize(ContainerEvalDataModelBase dataModel = null);

        partial void Initialize(ContainerEvalDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(ContainerEvalBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref ContainerEvalDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class CodeConversionEvalDataViewModel : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> filename;

        private DataValue<bool> useimage;

        public CodeConversionEvalDataViewModel(IMediaShell mediaShell, CodeConversionEvalDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.FileName = new DataValue<string>(string.Empty);
            this.FileName.PropertyChanged += this.FileNameChanged;
            this.UseImage = new DataValue<bool>(default(bool));
            this.UseImage.PropertyChanged += this.UseImageChanged;
            if (dataModel != null)
            {
                this.FileName.Value = dataModel.FileName;
                this.UseImage.Value = dataModel.UseImage;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected CodeConversionEvalDataViewModel(IMediaShell mediaShell, CodeConversionEvalDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.FileName = (DataValue<string>)dataViewModel.FileName.Clone();
            this.FileName.PropertyChanged += this.FileNameChanged;
            this.UseImage = (DataValue<bool>)dataViewModel.UseImage.Clone();
            this.UseImage.PropertyChanged += this.UseImageChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.FileName.IsDirty || this.UseImage.IsDirty;
            }
        }

        public DataValue<string> FileName
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filename);
                if (this.filename != null)
                {
                    this.filename.PropertyChanged -= this.FileNameChanged;
                }

                this.SetProperty(ref this.filename, value, () => this.FileName);
                if (value != null)
                {
                    this.filename.PropertyChanged += this.FileNameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> UseImage
        {
            get
            {
                return this.useimage;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.useimage);
                if (this.useimage != null)
                {
                    this.useimage.PropertyChanged -= this.UseImageChanged;
                }

                this.SetProperty(ref this.useimage, value, () => this.UseImage);
                if (value != null)
                {
                    this.useimage.PropertyChanged += this.UseImageChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new CodeConversionEval Export(object parameters = null)
        {
            var codeconversion = (CodeConversionEval)this.CreateExportObject();
            this.DoExport(codeconversion, parameters);
            return codeconversion;
        }

        public new CodeConversionEvalDataModel ToDataModel()
        {
            var codeconversion = (CodeConversionEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(codeconversion);
            return codeconversion;
        }

        public override void ClearDirty()
        {
            if (this.FileName != null)
            {
                this.FileName.ClearDirty();
            }

            if (this.UseImage != null)
            {
                this.UseImage.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new CodeConversionEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is CodeConversionEvalDataViewModel)
            {
                var that = (CodeConversionEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.FileName.EqualsValue(that.FileName);
                        result = result && this.UseImage.EqualsValue(that.UseImage);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new CodeConversionEval();
        }

        protected override object CreateDataModelObject()
        {
            return new CodeConversionEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (CodeConversionEval)exportModel;
            model.FileName = this.FileName.Value;
            model.UseImage = this.UseImage.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CodeConversionEvalDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.FileName = this.FileName.Value;
            model.UseImage = this.UseImage.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FileNameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FileName);
        }

        private void UseImageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.UseImage);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(CodeConversionEvalDataModel dataModel = null);

        partial void Initialize(CodeConversionEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(CodeConversionEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref CodeConversionEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class CsvMappingEvalDataViewModel : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> filename;

        private DataValue<string> outputformat;

        private EvalDataViewModelBase defaultvalue;

        private ExtendedObservableCollection<MatchDynamicPropertyDataViewModel> matches;

        public CsvMappingEvalDataViewModel(IMediaShell mediaShell, CsvMappingEvalDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.FileName = new DataValue<string>(string.Empty);
            this.FileName.PropertyChanged += this.FileNameChanged;
            this.OutputFormat = new DataValue<string>(string.Empty);
            this.OutputFormat.PropertyChanged += this.OutputFormatChanged;
            this.Matches = new ExtendedObservableCollection<MatchDynamicPropertyDataViewModel>();
            if (dataModel != null)
            {
                this.FileName.Value = dataModel.FileName;
                this.OutputFormat.Value = dataModel.OutputFormat;
                if (dataModel.DefaultValue != null && dataModel.DefaultValue.Evaluation != null)
                {
                    this.DefaultValue = this.CreateEvalDataViewModel(dataModel.DefaultValue.Evaluation);
                }

                foreach (var item in dataModel.Matches)
                {
                    var convertedItem = new MatchDynamicPropertyDataViewModel(mediaShell, item);
                    this.Matches.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected CsvMappingEvalDataViewModel(IMediaShell mediaShell, CsvMappingEvalDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.FileName = (DataValue<string>)dataViewModel.FileName.Clone();
            this.FileName.PropertyChanged += this.FileNameChanged;
            this.OutputFormat = (DataValue<string>)dataViewModel.OutputFormat.Clone();
            this.OutputFormat.PropertyChanged += this.OutputFormatChanged;
            if (dataViewModel.DefaultValue != null)
            {
                this.DefaultValue = (EvalDataViewModelBase)dataViewModel.DefaultValue.Clone();
            }

            this.Matches = new ExtendedObservableCollection<MatchDynamicPropertyDataViewModel>();
            foreach (var item in dataViewModel.Matches)
            {
                MatchDynamicPropertyDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (MatchDynamicPropertyDataViewModel)item.Clone();
                }

                this.Matches.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.FileName.IsDirty || this.OutputFormat.IsDirty || this.Matches.IsDirty || (this.DefaultValue != null && this.DefaultValue.IsDirty);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> FileName
        {
            get
            {
                return this.filename;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filename);
                if (this.filename != null)
                {
                    this.filename.PropertyChanged -= this.FileNameChanged;
                }

                this.SetProperty(ref this.filename, value, () => this.FileName);
                if (value != null)
                {
                    this.filename.PropertyChanged += this.FileNameChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> OutputFormat
        {
            get
            {
                return this.outputformat;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.outputformat);
                if (this.outputformat != null)
                {
                    this.outputformat.PropertyChanged -= this.OutputFormatChanged;
                }

                this.SetProperty(ref this.outputformat, value, () => this.OutputFormat);
                if (value != null)
                {
                    this.outputformat.PropertyChanged += this.OutputFormatChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase DefaultValue
        {
            get
            {
                return this.defaultvalue;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.defaultvalue);
                this.SetProperty(ref this.defaultvalue, value, () => this.DefaultValue);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<MatchDynamicPropertyDataViewModel> Matches
        {
            get
            {
                return this.matches;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.matches);
                this.SetProperty(ref this.matches, value, () => this.Matches);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new CsvMappingEval Export(object parameters = null)
        {
            var csvmapping = (CsvMappingEval)this.CreateExportObject();
            this.DoExport(csvmapping, parameters);
            return csvmapping;
        }

        public new CsvMappingEvalDataModel ToDataModel()
        {
            var csvmapping = (CsvMappingEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(csvmapping);
            return csvmapping;
        }

        public override void ClearDirty()
        {
            if (this.FileName != null)
            {
                this.FileName.ClearDirty();
            }

            if (this.OutputFormat != null)
            {
                this.OutputFormat.ClearDirty();
            }

            if (this.DefaultValue != null)
            {
                this.DefaultValue.ClearDirty();
            }

            if (this.Matches != null)
            {
                this.Matches.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new CsvMappingEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is CsvMappingEvalDataViewModel)
            {
                var that = (CsvMappingEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.FileName.EqualsValue(that.FileName);
                        result = result && this.OutputFormat.EqualsValue(that.OutputFormat);
                        if (this.DefaultValue != null)
                        {
                            result = result && this.DefaultValue.EqualsViewModel(that.DefaultValue);
                        }

                        result = result && this.Matches.Count == that.Matches.Count;
                        if (result)
                        {
                            foreach (var item in this.Matches)
                            {
                                var found = false;
                                foreach (var otherItem in that.Matches)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new CsvMappingEval();
        }

        protected override object CreateDataModelObject()
        {
            return new CsvMappingEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (CsvMappingEval)exportModel;
            model.FileName = this.FileName.Value;
            model.OutputFormat = this.OutputFormat.Value;
            if (this.DefaultValue != null)
            {
                var dynamicProperty = new DynamicProperty(this.DefaultValue.Export(exportParameters));
                model.DefaultValue = dynamicProperty;
            }

            foreach (var item in this.Matches)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Matches.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CsvMappingEvalDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.FileName = this.FileName.Value;
            model.OutputFormat = this.OutputFormat.Value;
            if (this.DefaultValue != null)
            {
                model.DefaultValue = new DynamicPropertyDataModel(this.DefaultValue.ToDataModel());
            }

            foreach (var item in this.Matches)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Matches.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FileNameChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FileName);
        }

        private void OutputFormatChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.OutputFormat);
        }

        private void DefaultValueChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.DefaultValue);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(CsvMappingEvalDataModel dataModel = null);

        partial void Initialize(CsvMappingEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(CsvMappingEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref CsvMappingEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class MatchDynamicPropertyDataViewModel : DataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> column;

        private EvalDataViewModelBase evaluation;

        public MatchDynamicPropertyDataViewModel(IMediaShell mediaShell, MatchDynamicPropertyDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Column = new DataValue<int>(default(int));
            this.Column.PropertyChanged += this.ColumnChanged;
            if (dataModel != null)
            {
                this.Column.Value = dataModel.Column;
                this.DisplayText = dataModel.DisplayText;
                if (dataModel.Evaluation != null)
                {
                    this.Evaluation = this.CreateEvalDataViewModel(dataModel.Evaluation.Evaluation);
                }

            }

            this.Initialize(dataModel);
        }

        protected MatchDynamicPropertyDataViewModel(IMediaShell mediaShell, MatchDynamicPropertyDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Column = (DataValue<int>)dataViewModel.Column.Clone();
            this.Column.PropertyChanged += this.ColumnChanged;
            if (dataViewModel.Evaluation != null)
            {
                this.Evaluation = (EvalDataViewModelBase)dataViewModel.Evaluation.Clone();
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Column.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Column
        {
            get
            {
                return this.column;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.column);
                if (this.column != null)
                {
                    this.column.PropertyChanged -= this.ColumnChanged;
                }

                this.SetProperty(ref this.column, value, () => this.Column);
                if (value != null)
                {
                    this.column.PropertyChanged += this.ColumnChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public EvalDataViewModelBase Evaluation
        {
            get
            {
                return this.evaluation;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.evaluation);
                this.SetProperty(ref this.evaluation, value, () => this.Evaluation);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public MatchDynamicProperty Export(object parameters = null)
        {
            var matchdynamicproperty = (MatchDynamicProperty)this.CreateExportObject();
            this.DoExport(matchdynamicproperty, parameters);
            return matchdynamicproperty;
        }

        public MatchDynamicPropertyDataModel ToDataModel()
        {
            var matchdynamicproperty = (MatchDynamicPropertyDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(matchdynamicproperty);
            return matchdynamicproperty;
        }

        public override void ClearDirty()
        {
            if (this.Column != null)
            {
                this.Column.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new MatchDynamicPropertyDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is MatchDynamicPropertyDataViewModel)
            {
                var that = (MatchDynamicPropertyDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Column.EqualsValue(that.Column);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new MatchDynamicProperty();
        }

        protected object CreateDataModelObject()
        {
            return new MatchDynamicPropertyDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (MatchDynamicProperty)exportModel;
            model.Column = this.Column.Value;
            model.Evaluation = this.Evaluation.Export(exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (MatchDynamicPropertyDataModel)dataModel;
            model.Column = this.Column.Value;
            model.Evaluation = new DynamicPropertyDataModel(this.Evaluation.ToDataModel());
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ColumnChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Column);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(MatchDynamicPropertyDataModel dataModel = null);

        partial void Initialize(MatchDynamicPropertyDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(MatchDynamicProperty model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref MatchDynamicPropertyDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class DateEvalDataViewModel : DateTimeEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<DateTime> begin;

        private DataValue<DateTime> end;

        public DateEvalDataViewModel(IMediaShell mediaShell, DateEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Begin = new DataValue<DateTime>(new DateTime());
            this.Begin.PropertyChanged += this.BeginChanged;
            this.End = new DataValue<DateTime>(new DateTime());
            this.End.PropertyChanged += this.EndChanged;
            if (dataModel != null)
            {
                this.Begin.Value = dataModel.Begin;
                this.End.Value = dataModel.End;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected DateEvalDataViewModel(IMediaShell mediaShell, DateEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Begin = (DataValue<DateTime>)dataViewModel.Begin.Clone();
            this.Begin.PropertyChanged += this.BeginChanged;
            this.End = (DataValue<DateTime>)dataViewModel.End.Clone();
            this.End.PropertyChanged += this.EndChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Begin.IsDirty || this.End.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<DateTime> Begin
        {
            get
            {
                return this.begin;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.begin);
                if (this.begin != null)
                {
                    this.begin.PropertyChanged -= this.BeginChanged;
                }

                this.SetProperty(ref this.begin, value, () => this.Begin);
                if (value != null)
                {
                    this.begin.PropertyChanged += this.BeginChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<DateTime> End
        {
            get
            {
                return this.end;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.end);
                if (this.end != null)
                {
                    this.end.PropertyChanged -= this.EndChanged;
                }

                this.SetProperty(ref this.end, value, () => this.End);
                if (value != null)
                {
                    this.end.PropertyChanged += this.EndChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new DateEval Export(object parameters = null)
        {
            var date = (DateEval)this.CreateExportObject();
            this.DoExport(date, parameters);
            return date;
        }

        public new DateEvalDataModel ToDataModel()
        {
            var date = (DateEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(date);
            return date;
        }

        public override void ClearDirty()
        {
            if (this.Begin != null)
            {
                this.Begin.ClearDirty();
            }

            if (this.End != null)
            {
                this.End.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new DateEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is DateEvalDataViewModel)
            {
                var that = (DateEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Begin.EqualsValue(that.Begin);
                        result = result && this.End.EqualsValue(that.End);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new DateEval();
        }

        protected override object CreateDataModelObject()
        {
            return new DateEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (DateEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Begin = this.Begin.Value;
            model.End = this.End.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (DateEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Begin = this.Begin.Value;
            model.End = this.End.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void BeginChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Begin);
        }

        private void EndChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.End);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(DateEvalDataModel dataModel = null);

        partial void Initialize(DateEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(DateEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref DateEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class DateTimeEvalDataViewModelBase : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public DateTimeEvalDataViewModelBase(IMediaShell mediaShell, DateTimeEvalDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected DateTimeEvalDataViewModelBase(IMediaShell mediaShell, DateTimeEvalDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new DateTimeEvalBase Export(object parameters = null)
        {
            var datetimebase = (DateTimeEvalBase)this.CreateExportObject();
            this.DoExport(datetimebase, parameters);
            return datetimebase;
        }

        public new DateTimeEvalDataModelBase ToDataModel()
        {
            var datetimebase = (DateTimeEvalDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(datetimebase);
            return datetimebase;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is DateTimeEvalDataViewModelBase)
            {
                var that = (DateTimeEvalDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (DateTimeEvalBase)exportModel;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (DateTimeEvalDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(DateTimeEvalDataModelBase dataModel = null);

        partial void Initialize(DateTimeEvalDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(DateTimeEvalBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref DateTimeEvalDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class DayOfWeekEvalDataViewModel : DateTimeEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<bool> monday;

        private DataValue<bool> tuesday;

        private DataValue<bool> wednesday;

        private DataValue<bool> thursday;

        private DataValue<bool> friday;

        private DataValue<bool> saturday;

        private DataValue<bool> sunday;

        public DayOfWeekEvalDataViewModel(IMediaShell mediaShell, DayOfWeekEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Monday = new DataValue<bool>(default(bool));
            this.Monday.PropertyChanged += this.MondayChanged;
            this.Tuesday = new DataValue<bool>(default(bool));
            this.Tuesday.PropertyChanged += this.TuesdayChanged;
            this.Wednesday = new DataValue<bool>(default(bool));
            this.Wednesday.PropertyChanged += this.WednesdayChanged;
            this.Thursday = new DataValue<bool>(default(bool));
            this.Thursday.PropertyChanged += this.ThursdayChanged;
            this.Friday = new DataValue<bool>(default(bool));
            this.Friday.PropertyChanged += this.FridayChanged;
            this.Saturday = new DataValue<bool>(default(bool));
            this.Saturday.PropertyChanged += this.SaturdayChanged;
            this.Sunday = new DataValue<bool>(default(bool));
            this.Sunday.PropertyChanged += this.SundayChanged;
            if (dataModel != null)
            {
                this.Monday.Value = dataModel.Monday;
                this.Tuesday.Value = dataModel.Tuesday;
                this.Wednesday.Value = dataModel.Wednesday;
                this.Thursday.Value = dataModel.Thursday;
                this.Friday.Value = dataModel.Friday;
                this.Saturday.Value = dataModel.Saturday;
                this.Sunday.Value = dataModel.Sunday;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected DayOfWeekEvalDataViewModel(IMediaShell mediaShell, DayOfWeekEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Monday = (DataValue<bool>)dataViewModel.Monday.Clone();
            this.Monday.PropertyChanged += this.MondayChanged;
            this.Tuesday = (DataValue<bool>)dataViewModel.Tuesday.Clone();
            this.Tuesday.PropertyChanged += this.TuesdayChanged;
            this.Wednesday = (DataValue<bool>)dataViewModel.Wednesday.Clone();
            this.Wednesday.PropertyChanged += this.WednesdayChanged;
            this.Thursday = (DataValue<bool>)dataViewModel.Thursday.Clone();
            this.Thursday.PropertyChanged += this.ThursdayChanged;
            this.Friday = (DataValue<bool>)dataViewModel.Friday.Clone();
            this.Friday.PropertyChanged += this.FridayChanged;
            this.Saturday = (DataValue<bool>)dataViewModel.Saturday.Clone();
            this.Saturday.PropertyChanged += this.SaturdayChanged;
            this.Sunday = (DataValue<bool>)dataViewModel.Sunday.Clone();
            this.Sunday.PropertyChanged += this.SundayChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Monday.IsDirty || this.Tuesday.IsDirty || this.Wednesday.IsDirty || this.Thursday.IsDirty || this.Friday.IsDirty || this.Saturday.IsDirty || this.Sunday.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> Monday
        {
            get
            {
                return this.monday;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.monday);
                if (this.monday != null)
                {
                    this.monday.PropertyChanged -= this.MondayChanged;
                }

                this.SetProperty(ref this.monday, value, () => this.Monday);
                if (value != null)
                {
                    this.monday.PropertyChanged += this.MondayChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> Tuesday
        {
            get
            {
                return this.tuesday;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.tuesday);
                if (this.tuesday != null)
                {
                    this.tuesday.PropertyChanged -= this.TuesdayChanged;
                }

                this.SetProperty(ref this.tuesday, value, () => this.Tuesday);
                if (value != null)
                {
                    this.tuesday.PropertyChanged += this.TuesdayChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> Wednesday
        {
            get
            {
                return this.wednesday;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.wednesday);
                if (this.wednesday != null)
                {
                    this.wednesday.PropertyChanged -= this.WednesdayChanged;
                }

                this.SetProperty(ref this.wednesday, value, () => this.Wednesday);
                if (value != null)
                {
                    this.wednesday.PropertyChanged += this.WednesdayChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> Thursday
        {
            get
            {
                return this.thursday;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.thursday);
                if (this.thursday != null)
                {
                    this.thursday.PropertyChanged -= this.ThursdayChanged;
                }

                this.SetProperty(ref this.thursday, value, () => this.Thursday);
                if (value != null)
                {
                    this.thursday.PropertyChanged += this.ThursdayChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> Friday
        {
            get
            {
                return this.friday;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.friday);
                if (this.friday != null)
                {
                    this.friday.PropertyChanged -= this.FridayChanged;
                }

                this.SetProperty(ref this.friday, value, () => this.Friday);
                if (value != null)
                {
                    this.friday.PropertyChanged += this.FridayChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> Saturday
        {
            get
            {
                return this.saturday;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.saturday);
                if (this.saturday != null)
                {
                    this.saturday.PropertyChanged -= this.SaturdayChanged;
                }

                this.SetProperty(ref this.saturday, value, () => this.Saturday);
                if (value != null)
                {
                    this.saturday.PropertyChanged += this.SaturdayChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> Sunday
        {
            get
            {
                return this.sunday;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.sunday);
                if (this.sunday != null)
                {
                    this.sunday.PropertyChanged -= this.SundayChanged;
                }

                this.SetProperty(ref this.sunday, value, () => this.Sunday);
                if (value != null)
                {
                    this.sunday.PropertyChanged += this.SundayChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new DayOfWeekEval Export(object parameters = null)
        {
            var dayofweek = (DayOfWeekEval)this.CreateExportObject();
            this.DoExport(dayofweek, parameters);
            return dayofweek;
        }

        public new DayOfWeekEvalDataModel ToDataModel()
        {
            var dayofweek = (DayOfWeekEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(dayofweek);
            return dayofweek;
        }

        public override void ClearDirty()
        {
            if (this.Monday != null)
            {
                this.Monday.ClearDirty();
            }

            if (this.Tuesday != null)
            {
                this.Tuesday.ClearDirty();
            }

            if (this.Wednesday != null)
            {
                this.Wednesday.ClearDirty();
            }

            if (this.Thursday != null)
            {
                this.Thursday.ClearDirty();
            }

            if (this.Friday != null)
            {
                this.Friday.ClearDirty();
            }

            if (this.Saturday != null)
            {
                this.Saturday.ClearDirty();
            }

            if (this.Sunday != null)
            {
                this.Sunday.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new DayOfWeekEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is DayOfWeekEvalDataViewModel)
            {
                var that = (DayOfWeekEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Monday.EqualsValue(that.Monday);
                        result = result && this.Tuesday.EqualsValue(that.Tuesday);
                        result = result && this.Wednesday.EqualsValue(that.Wednesday);
                        result = result && this.Thursday.EqualsValue(that.Thursday);
                        result = result && this.Friday.EqualsValue(that.Friday);
                        result = result && this.Saturday.EqualsValue(that.Saturday);
                        result = result && this.Sunday.EqualsValue(that.Sunday);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new DayOfWeekEval();
        }

        protected override object CreateDataModelObject()
        {
            return new DayOfWeekEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (DayOfWeekEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Monday = this.Monday.Value;
            model.Tuesday = this.Tuesday.Value;
            model.Wednesday = this.Wednesday.Value;
            model.Thursday = this.Thursday.Value;
            model.Friday = this.Friday.Value;
            model.Saturday = this.Saturday.Value;
            model.Sunday = this.Sunday.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (DayOfWeekEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Monday = this.Monday.Value;
            model.Tuesday = this.Tuesday.Value;
            model.Wednesday = this.Wednesday.Value;
            model.Thursday = this.Thursday.Value;
            model.Friday = this.Friday.Value;
            model.Saturday = this.Saturday.Value;
            model.Sunday = this.Sunday.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void MondayChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Monday);
        }

        private void TuesdayChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Tuesday);
        }

        private void WednesdayChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Wednesday);
        }

        private void ThursdayChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Thursday);
        }

        private void FridayChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Friday);
        }

        private void SaturdayChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Saturday);
        }

        private void SundayChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Sunday);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(DayOfWeekEvalDataModel dataModel = null);

        partial void Initialize(DayOfWeekEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(DayOfWeekEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref DayOfWeekEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class EvaluationEvalDataViewModel : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private string referenceReferenceName;

        private EvaluationConfigDataViewModel referenceReference;

        public EvaluationEvalDataViewModel(IMediaShell mediaShell, EvaluationEvalDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            // Reference
            if (dataModel != null)
            {
                this.referenceReferenceName = dataModel.Reference;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected EvaluationEvalDataViewModel(IMediaShell mediaShell, EvaluationEvalDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.ReferenceName = dataViewModel.ReferenceName;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvaluationConfigDataViewModel Reference
        {
            get
            {
                if (this.referenceReference == null)
                {
                    this.referenceReference = this.FindReference();
                    if (this.referenceReference != null)
                    {
                        this.referenceReference.PropertyChanged += this.ReferenceChanged;
                    }

                }

                return this.referenceReference;
            }
            set
            {
                if (this.Reference != null)
                {
                    this.Reference.PropertyChanged -= this.ReferenceChanged;
                }

                this.SetProperty(ref this.referenceReference, value, () => this.Reference);
                if (value != null)
                {
                    this.ReferenceName = value.Name.Value;
                    this.Reference.PropertyChanged += this.ReferenceChanged;
                }

            }
        }

        public string ReferenceName
        {
            get
            {
                return this.referenceReferenceName;
            }
            private set
            {
                this.SetProperty(ref this.referenceReferenceName, value, () => this.Reference);
            }
        }

        public new EvaluationEval Export(object parameters = null)
        {
            var evaluation = (EvaluationEval)this.CreateExportObject();
            this.DoExport(evaluation, parameters);
            return evaluation;
        }

        public new EvaluationEvalDataModel ToDataModel()
        {
            var evaluation = (EvaluationEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(evaluation);
            return evaluation;
        }

        public override void ClearDirty()
        {
            if (this.Reference != null)
            {
                this.Reference.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new EvaluationEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is EvaluationEvalDataViewModel)
            {
                var that = (EvaluationEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Reference != null)
                        {
                            result = result && this.Reference.EqualsViewModel(that.Reference);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new EvaluationEval();
        }

        protected override object CreateDataModelObject()
        {
            return new EvaluationEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (EvaluationEval)exportModel;
            model.Reference = this.ReferenceName;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (EvaluationEvalDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Reference = this.ReferenceName;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ReferenceChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Reference != null)
            {
                this.ReferenceName = this.Reference.Name.Value;
            }

            this.RaisePropertyChanged(() => this.Reference);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(EvaluationEvalDataModel dataModel = null);

        partial void Initialize(EvaluationEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(EvaluationEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref EvaluationEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class FormatEvalDataViewModel : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> format;

        private ExtendedObservableCollection<EvalDataViewModelBase> arguments;

        public FormatEvalDataViewModel(IMediaShell mediaShell, FormatEvalDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Format = new DataValue<string>(string.Empty);
            this.Format.PropertyChanged += this.FormatChanged;
            this.Arguments = new ExtendedObservableCollection<EvalDataViewModelBase>();
            if (dataModel != null)
            {
                this.Format.Value = dataModel.Format;
                foreach (var item in dataModel.Arguments)
                {
                    var typeName = item.GetType().Name.Replace("DataModel", "DataViewModel");
                    var assembly = this.GetType().Assembly;
                    var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                    if (type == null)
                    {
                        throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, item.GetType().Name));
                    }

                    var convertedItem = (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, item);
                    this.Arguments.Add(convertedItem);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected FormatEvalDataViewModel(IMediaShell mediaShell, FormatEvalDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Format = (DataValue<string>)dataViewModel.Format.Clone();
            this.Format.PropertyChanged += this.FormatChanged;
            this.Arguments = new ExtendedObservableCollection<EvalDataViewModelBase>();
            foreach (var item in dataViewModel.Arguments)
            {
                EvalDataViewModelBase clonedItem = null;
                if (item != null)
                {
                    clonedItem = (EvalDataViewModelBase)item.Clone();
                }

                this.Arguments.Add(clonedItem);
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Format.IsDirty || this.Arguments.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Format
        {
            get
            {
                return this.format;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.format);
                if (this.format != null)
                {
                    this.format.PropertyChanged -= this.FormatChanged;
                }

                this.SetProperty(ref this.format, value, () => this.Format);
                if (value != null)
                {
                    this.format.PropertyChanged += this.FormatChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public ExtendedObservableCollection<EvalDataViewModelBase> Arguments
        {
            get
            {
                return this.arguments;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.arguments);
                this.SetProperty(ref this.arguments, value, () => this.Arguments);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new FormatEval Export(object parameters = null)
        {
            var format = (FormatEval)this.CreateExportObject();
            this.DoExport(format, parameters);
            return format;
        }

        public new FormatEvalDataModel ToDataModel()
        {
            var format = (FormatEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(format);
            return format;
        }

        public override void ClearDirty()
        {
            if (this.Format != null)
            {
                this.Format.ClearDirty();
            }

            if (this.Arguments != null)
            {
                this.Arguments.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new FormatEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is FormatEvalDataViewModel)
            {
                var that = (FormatEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Format.EqualsValue(that.Format);
                        result = result && this.Arguments.Count == that.Arguments.Count;
                        if (result)
                        {
                            foreach (var item in this.Arguments)
                            {
                                var found = false;
                                foreach (var otherItem in that.Arguments)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new FormatEval();
        }

        protected override object CreateDataModelObject()
        {
            return new FormatEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (FormatEval)exportModel;
            model.Format = this.Format.Value;
            foreach (var item in this.Arguments)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Arguments.Add(convertedItem);
                }

            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (FormatEvalDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Format = this.Format.Value;
            foreach (var item in this.Arguments)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Arguments.Add(convertedItem);
                }

            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FormatChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Format);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(FormatEvalDataModel dataModel = null);

        partial void Initialize(FormatEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(FormatEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref FormatEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class GenericEvalDataViewModel : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> language;

        private DataValue<int> table;

        private DataValue<int> column;

        private DataValue<int> row;

        public GenericEvalDataViewModel(IMediaShell mediaShell, GenericEvalDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Language = new DataValue<int>(default(int));
            this.Language.PropertyChanged += this.LanguageChanged;
            this.Table = new DataValue<int>(default(int));
            this.Table.PropertyChanged += this.TableChanged;
            this.Column = new DataValue<int>(default(int));
            this.Column.PropertyChanged += this.ColumnChanged;
            this.Row = new DataValue<int>(default(int));
            this.Row.PropertyChanged += this.RowChanged;
            if (dataModel != null)
            {
                this.Language.Value = dataModel.Language;
                this.Table.Value = dataModel.Table;
                this.Column.Value = dataModel.Column;
                this.Row.Value = dataModel.Row;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected GenericEvalDataViewModel(IMediaShell mediaShell, GenericEvalDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Language = (DataValue<int>)dataViewModel.Language.Clone();
            this.Language.PropertyChanged += this.LanguageChanged;
            this.Table = (DataValue<int>)dataViewModel.Table.Clone();
            this.Table.PropertyChanged += this.TableChanged;
            this.Column = (DataValue<int>)dataViewModel.Column.Clone();
            this.Column.PropertyChanged += this.ColumnChanged;
            this.Row = (DataValue<int>)dataViewModel.Row.Clone();
            this.Row.PropertyChanged += this.RowChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Language.IsDirty || this.Table.IsDirty || this.Column.IsDirty || this.Row.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Language
        {
            get
            {
                return this.language;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.language);
                if (this.language != null)
                {
                    this.language.PropertyChanged -= this.LanguageChanged;
                }

                this.SetProperty(ref this.language, value, () => this.Language);
                if (value != null)
                {
                    this.language.PropertyChanged += this.LanguageChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Table
        {
            get
            {
                return this.table;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.table);
                if (this.table != null)
                {
                    this.table.PropertyChanged -= this.TableChanged;
                }

                this.SetProperty(ref this.table, value, () => this.Table);
                if (value != null)
                {
                    this.table.PropertyChanged += this.TableChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Column
        {
            get
            {
                return this.column;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.column);
                if (this.column != null)
                {
                    this.column.PropertyChanged -= this.ColumnChanged;
                }

                this.SetProperty(ref this.column, value, () => this.Column);
                if (value != null)
                {
                    this.column.PropertyChanged += this.ColumnChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Row
        {
            get
            {
                return this.row;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.row);
                if (this.row != null)
                {
                    this.row.PropertyChanged -= this.RowChanged;
                }

                this.SetProperty(ref this.row, value, () => this.Row);
                if (value != null)
                {
                    this.row.PropertyChanged += this.RowChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new GenericEval Export(object parameters = null)
        {
            var generic = (GenericEval)this.CreateExportObject();
            this.DoExport(generic, parameters);
            return generic;
        }

        public new GenericEvalDataModel ToDataModel()
        {
            var generic = (GenericEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(generic);
            return generic;
        }

        public override void ClearDirty()
        {
            if (this.Language != null)
            {
                this.Language.ClearDirty();
            }

            if (this.Table != null)
            {
                this.Table.ClearDirty();
            }

            if (this.Column != null)
            {
                this.Column.ClearDirty();
            }

            if (this.Row != null)
            {
                this.Row.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new GenericEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is GenericEvalDataViewModel)
            {
                var that = (GenericEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Language.EqualsValue(that.Language);
                        result = result && this.Table.EqualsValue(that.Table);
                        result = result && this.Column.EqualsValue(that.Column);
                        result = result && this.Row.EqualsValue(that.Row);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new GenericEval();
        }

        protected override object CreateDataModelObject()
        {
            return new GenericEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (GenericEval)exportModel;
            model.Language = this.Language.Value;
            model.Table = this.Table.Value;
            model.Column = this.Column.Value;
            model.Row = this.Row.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (GenericEvalDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            model.Language = this.Language.Value;
            model.Table = this.Table.Value;
            model.Column = this.Column.Value;
            model.Row = this.Row.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void LanguageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Language);
        }

        private void TableChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Table);
        }

        private void ColumnChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Column);
        }

        private void RowChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Row);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(GenericEvalDataModel dataModel = null);

        partial void Initialize(GenericEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(GenericEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref GenericEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class IfEvalDataViewModel : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private EvalDataViewModelBase condition;

        private EvalDataViewModelBase then;

        private EvalDataViewModelBase @else;

        public IfEvalDataViewModel(IMediaShell mediaShell, IfEvalDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                if (dataModel.Condition != null && dataModel.Condition.Evaluation != null)
                {
                    this.Condition = this.CreateEvalDataViewModel(dataModel.Condition.Evaluation);
                }

                if (dataModel.Then != null && dataModel.Then.Evaluation != null)
                {
                    this.Then = this.CreateEvalDataViewModel(dataModel.Then.Evaluation);
                }

                if (dataModel.Else != null && dataModel.Else.Evaluation != null)
                {
                    this.Else = this.CreateEvalDataViewModel(dataModel.Else.Evaluation);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected IfEvalDataViewModel(IMediaShell mediaShell, IfEvalDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            if (dataViewModel.Condition != null)
            {
                this.Condition = (EvalDataViewModelBase)dataViewModel.Condition.Clone();
            }

            if (dataViewModel.Then != null)
            {
                this.Then = (EvalDataViewModelBase)dataViewModel.Then.Clone();
            }

            if (dataViewModel.Else != null)
            {
                this.Else = (EvalDataViewModelBase)dataViewModel.Else.Clone();
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || (this.Condition != null && this.Condition.IsDirty) || (this.Then != null && this.Then.IsDirty) || (this.Else != null && this.Else.IsDirty);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.condition);
                this.SetProperty(ref this.condition, value, () => this.Condition);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase Then
        {
            get
            {
                return this.then;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.then);
                this.SetProperty(ref this.then, value, () => this.Then);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase Else
        {
            get
            {
                return this.@else;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.@else);
                this.SetProperty(ref this.@else, value, () => this.Else);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new IfEval Export(object parameters = null)
        {
            var @if = (IfEval)this.CreateExportObject();
            this.DoExport(@if, parameters);
            return @if;
        }

        public new IfEvalDataModel ToDataModel()
        {
            var @if = (IfEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(@if);
            return @if;
        }

        public override void ClearDirty()
        {
            if (this.Condition != null)
            {
                this.Condition.ClearDirty();
            }

            if (this.Then != null)
            {
                this.Then.ClearDirty();
            }

            if (this.Else != null)
            {
                this.Else.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new IfEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is IfEvalDataViewModel)
            {
                var that = (IfEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Condition != null)
                        {
                            result = result && this.Condition.EqualsViewModel(that.Condition);
                        }

                        if (this.Then != null)
                        {
                            result = result && this.Then.EqualsViewModel(that.Then);
                        }

                        if (this.Else != null)
                        {
                            result = result && this.Else.EqualsViewModel(that.Else);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new IfEval();
        }

        protected override object CreateDataModelObject()
        {
            return new IfEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (IfEval)exportModel;
            if (this.Condition != null)
            {
                var dynamicProperty = new DynamicProperty(this.Condition.Export(exportParameters));
                model.Condition = dynamicProperty;
            }

            if (this.Then != null)
            {
                var dynamicProperty = new DynamicProperty(this.Then.Export(exportParameters));
                model.Then = dynamicProperty;
            }

            if (this.Else != null)
            {
                var dynamicProperty = new DynamicProperty(this.Else.Export(exportParameters));
                model.Else = dynamicProperty;
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (IfEvalDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            if (this.Condition != null)
            {
                model.Condition = new DynamicPropertyDataModel(this.Condition.ToDataModel());
            }

            if (this.Then != null)
            {
                model.Then = new DynamicPropertyDataModel(this.Then.ToDataModel());
            }

            if (this.Else != null)
            {
                model.Else = new DynamicPropertyDataModel(this.Else.ToDataModel());
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ConditionChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.Condition);
        }

        private void ThenChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.Then);
        }

        private void ElseChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.Else);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(IfEvalDataModel dataModel = null);

        partial void Initialize(IfEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(IfEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref IfEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class IntegerCompareEvalDataViewModel : ContainerEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<int> begin;

        private DataValue<int> end;

        public IntegerCompareEvalDataViewModel(IMediaShell mediaShell, IntegerCompareEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Begin = new DataValue<int>(default(int));
            this.Begin.PropertyChanged += this.BeginChanged;
            this.End = new DataValue<int>(default(int));
            this.End.PropertyChanged += this.EndChanged;
            if (dataModel != null)
            {
                this.Begin.Value = dataModel.Begin;
                this.End.Value = dataModel.End;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected IntegerCompareEvalDataViewModel(IMediaShell mediaShell, IntegerCompareEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Begin = (DataValue<int>)dataViewModel.Begin.Clone();
            this.Begin.PropertyChanged += this.BeginChanged;
            this.End = (DataValue<int>)dataViewModel.End.Clone();
            this.End.PropertyChanged += this.EndChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Begin.IsDirty || this.End.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> Begin
        {
            get
            {
                return this.begin;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.begin);
                if (this.begin != null)
                {
                    this.begin.PropertyChanged -= this.BeginChanged;
                }

                this.SetProperty(ref this.begin, value, () => this.Begin);
                if (value != null)
                {
                    this.begin.PropertyChanged += this.BeginChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<int> End
        {
            get
            {
                return this.end;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.end);
                if (this.end != null)
                {
                    this.end.PropertyChanged -= this.EndChanged;
                }

                this.SetProperty(ref this.end, value, () => this.End);
                if (value != null)
                {
                    this.end.PropertyChanged += this.EndChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new IntegerCompareEval Export(object parameters = null)
        {
            var integercompare = (IntegerCompareEval)this.CreateExportObject();
            this.DoExport(integercompare, parameters);
            return integercompare;
        }

        public new IntegerCompareEvalDataModel ToDataModel()
        {
            var integercompare = (IntegerCompareEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(integercompare);
            return integercompare;
        }

        public override void ClearDirty()
        {
            if (this.Begin != null)
            {
                this.Begin.ClearDirty();
            }

            if (this.End != null)
            {
                this.End.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new IntegerCompareEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is IntegerCompareEvalDataViewModel)
            {
                var that = (IntegerCompareEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Begin.EqualsValue(that.Begin);
                        result = result && this.End.EqualsValue(that.End);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new IntegerCompareEval();
        }

        protected override object CreateDataModelObject()
        {
            return new IntegerCompareEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (IntegerCompareEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Begin = this.Begin.Value;
            model.End = this.End.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (IntegerCompareEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Begin = this.Begin.Value;
            model.End = this.End.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void BeginChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Begin);
        }

        private void EndChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.End);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(IntegerCompareEvalDataModel dataModel = null);

        partial void Initialize(IntegerCompareEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(IntegerCompareEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref IntegerCompareEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class NotEvalDataViewModel : ContainerEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public NotEvalDataViewModel(IMediaShell mediaShell, NotEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected NotEvalDataViewModel(IMediaShell mediaShell, NotEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new NotEval Export(object parameters = null)
        {
            var not = (NotEval)this.CreateExportObject();
            this.DoExport(not, parameters);
            return not;
        }

        public new NotEvalDataModel ToDataModel()
        {
            var not = (NotEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(not);
            return not;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new NotEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is NotEvalDataViewModel)
            {
                var that = (NotEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new NotEval();
        }

        protected override object CreateDataModelObject()
        {
            return new NotEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (NotEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (NotEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(NotEvalDataModel dataModel = null);

        partial void Initialize(NotEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(NotEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref NotEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class OrEvalDataViewModel : CollectionEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public OrEvalDataViewModel(IMediaShell mediaShell, OrEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected OrEvalDataViewModel(IMediaShell mediaShell, OrEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new OrEval Export(object parameters = null)
        {
            var or = (OrEval)this.CreateExportObject();
            this.DoExport(or, parameters);
            return or;
        }

        public new OrEvalDataModel ToDataModel()
        {
            var or = (OrEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(or);
            return or;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new OrEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is OrEvalDataViewModel)
            {
                var that = (OrEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new OrEval();
        }

        protected override object CreateDataModelObject()
        {
            return new OrEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (OrEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (OrEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(OrEvalDataModel dataModel = null);

        partial void Initialize(OrEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(OrEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref OrEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class RegexReplaceEvalDataViewModel : ContainerEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> pattern;

        private DataValue<string> replacement;

        public RegexReplaceEvalDataViewModel(IMediaShell mediaShell, RegexReplaceEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Pattern = new DataValue<string>(string.Empty);
            this.Pattern.PropertyChanged += this.PatternChanged;
            this.Replacement = new DataValue<string>(string.Empty);
            this.Replacement.PropertyChanged += this.ReplacementChanged;
            if (dataModel != null)
            {
                this.Pattern.Value = dataModel.Pattern;
                this.Replacement.Value = dataModel.Replacement;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected RegexReplaceEvalDataViewModel(IMediaShell mediaShell, RegexReplaceEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Pattern = (DataValue<string>)dataViewModel.Pattern.Clone();
            this.Pattern.PropertyChanged += this.PatternChanged;
            this.Replacement = (DataValue<string>)dataViewModel.Replacement.Clone();
            this.Replacement.PropertyChanged += this.ReplacementChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Pattern.IsDirty || this.Replacement.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Pattern
        {
            get
            {
                return this.pattern;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.pattern);
                if (this.pattern != null)
                {
                    this.pattern.PropertyChanged -= this.PatternChanged;
                }

                this.SetProperty(ref this.pattern, value, () => this.Pattern);
                if (value != null)
                {
                    this.pattern.PropertyChanged += this.PatternChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Replacement
        {
            get
            {
                return this.replacement;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.replacement);
                if (this.replacement != null)
                {
                    this.replacement.PropertyChanged -= this.ReplacementChanged;
                }

                this.SetProperty(ref this.replacement, value, () => this.Replacement);
                if (value != null)
                {
                    this.replacement.PropertyChanged += this.ReplacementChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new RegexReplaceEval Export(object parameters = null)
        {
            var regexreplace = (RegexReplaceEval)this.CreateExportObject();
            this.DoExport(regexreplace, parameters);
            return regexreplace;
        }

        public new RegexReplaceEvalDataModel ToDataModel()
        {
            var regexreplace = (RegexReplaceEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(regexreplace);
            return regexreplace;
        }

        public override void ClearDirty()
        {
            if (this.Pattern != null)
            {
                this.Pattern.ClearDirty();
            }

            if (this.Replacement != null)
            {
                this.Replacement.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new RegexReplaceEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is RegexReplaceEvalDataViewModel)
            {
                var that = (RegexReplaceEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Pattern.EqualsValue(that.Pattern);
                        result = result && this.Replacement.EqualsValue(that.Replacement);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new RegexReplaceEval();
        }

        protected override object CreateDataModelObject()
        {
            return new RegexReplaceEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (RegexReplaceEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Pattern = this.Pattern.Value;
            model.Replacement = this.Replacement.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (RegexReplaceEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Pattern = this.Pattern.Value;
            model.Replacement = this.Replacement.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void PatternChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Pattern);
        }

        private void ReplacementChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Replacement);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(RegexReplaceEvalDataModel dataModel = null);

        partial void Initialize(RegexReplaceEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(RegexReplaceEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref RegexReplaceEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class StringCompareEvalDataViewModel : ContainerEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> value;

        private DataValue<bool> ignorecase;

        public StringCompareEvalDataViewModel(IMediaShell mediaShell, StringCompareEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Value = new DataValue<string>(string.Empty);
            this.Value.PropertyChanged += this.ValueChanged;
            this.IgnoreCase = new DataValue<bool>(false);
            this.IgnoreCase.PropertyChanged += this.IgnoreCaseChanged;
            if (dataModel != null)
            {
                this.Value.Value = dataModel.Value;
                this.IgnoreCase.Value = dataModel.IgnoreCase;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected StringCompareEvalDataViewModel(IMediaShell mediaShell, StringCompareEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Value = (DataValue<string>)dataViewModel.Value.Clone();
            this.Value.PropertyChanged += this.ValueChanged;
            this.IgnoreCase = (DataValue<bool>)dataViewModel.IgnoreCase.Clone();
            this.IgnoreCase.PropertyChanged += this.IgnoreCaseChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Value.IsDirty || this.IgnoreCase.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.value);
                if (this.value != null)
                {
                    this.value.PropertyChanged -= this.ValueChanged;
                }

                this.SetProperty(ref this.value, value, () => this.Value);
                if (value != null)
                {
                    this.value.PropertyChanged += this.ValueChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<bool> IgnoreCase
        {
            get
            {
                return this.ignorecase;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.ignorecase);
                if (this.ignorecase != null)
                {
                    this.ignorecase.PropertyChanged -= this.IgnoreCaseChanged;
                }

                this.SetProperty(ref this.ignorecase, value, () => this.IgnoreCase);
                if (value != null)
                {
                    this.ignorecase.PropertyChanged += this.IgnoreCaseChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new StringCompareEval Export(object parameters = null)
        {
            var stringcompare = (StringCompareEval)this.CreateExportObject();
            this.DoExport(stringcompare, parameters);
            return stringcompare;
        }

        public new StringCompareEvalDataModel ToDataModel()
        {
            var stringcompare = (StringCompareEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(stringcompare);
            return stringcompare;
        }

        public override void ClearDirty()
        {
            if (this.Value != null)
            {
                this.Value.ClearDirty();
            }

            if (this.IgnoreCase != null)
            {
                this.IgnoreCase.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new StringCompareEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is StringCompareEvalDataViewModel)
            {
                var that = (StringCompareEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Value.EqualsValue(that.Value);
                        result = result && this.IgnoreCase.EqualsValue(that.IgnoreCase);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new StringCompareEval();
        }

        protected override object CreateDataModelObject()
        {
            return new StringCompareEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (StringCompareEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Value = this.Value.Value;
            model.IgnoreCase = this.IgnoreCase.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (StringCompareEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Value = this.Value.Value;
            model.IgnoreCase = this.IgnoreCase.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Value);
        }

        private void IgnoreCaseChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.IgnoreCase);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(StringCompareEvalDataModel dataModel = null);

        partial void Initialize(StringCompareEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(StringCompareEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref StringCompareEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class SwitchEvalDataViewModel : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private EvalDataViewModelBase value;

        private EvalDataViewModelBase @default;

        private ExtendedObservableCollection<CaseDynamicPropertyDataViewModel> cases;

        public SwitchEvalDataViewModel(IMediaShell mediaShell, SwitchEvalDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Cases = new ExtendedObservableCollection<CaseDynamicPropertyDataViewModel>();
            if (dataModel != null)
            {
                if (dataModel.Value != null && dataModel.Value.Evaluation != null)
                {
                    this.Value = this.CreateEvalDataViewModel(dataModel.Value.Evaluation);
                }

                foreach (var item in dataModel.Cases)
                {
                    var convertedItem = new CaseDynamicPropertyDataViewModel(mediaShell, item);
                    this.Cases.Add(convertedItem);
                }

                if (dataModel.Default != null && dataModel.Default.Evaluation != null)
                {
                    this.Default = this.CreateEvalDataViewModel(dataModel.Default.Evaluation);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected SwitchEvalDataViewModel(IMediaShell mediaShell, SwitchEvalDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            if (dataViewModel.Value != null)
            {
                this.Value = (EvalDataViewModelBase)dataViewModel.Value.Clone();
            }

            this.Cases = new ExtendedObservableCollection<CaseDynamicPropertyDataViewModel>();
            foreach (var item in dataViewModel.Cases)
            {
                CaseDynamicPropertyDataViewModel clonedItem = null;
                if (item != null)
                {
                    clonedItem = (CaseDynamicPropertyDataViewModel)item.Clone();
                }

                this.Cases.Add(clonedItem);
            }

            if (dataViewModel.Default != null)
            {
                this.Default = (EvalDataViewModelBase)dataViewModel.Default.Clone();
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Cases.IsDirty || (this.Value != null && this.Value.IsDirty) || (this.Default != null && this.Default.IsDirty);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.value);
                this.SetProperty(ref this.value, value, () => this.Value);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public ExtendedObservableCollection<CaseDynamicPropertyDataViewModel> Cases
        {
            get
            {
                return this.cases;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.cases);
                this.SetProperty(ref this.cases, value, () => this.Cases);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase Default
        {
            get
            {
                return this.@default;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.@default);
                this.SetProperty(ref this.@default, value, () => this.Default);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new SwitchEval Export(object parameters = null)
        {
            var @switch = (SwitchEval)this.CreateExportObject();
            this.DoExport(@switch, parameters);
            return @switch;
        }

        public new SwitchEvalDataModel ToDataModel()
        {
            var @switch = (SwitchEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(@switch);
            return @switch;
        }

        public override void ClearDirty()
        {
            if (this.Value != null)
            {
                this.Value.ClearDirty();
            }

            if (this.Cases != null)
            {
                this.Cases.ClearDirty();
            }

            if (this.Default != null)
            {
                this.Default.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new SwitchEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is SwitchEvalDataViewModel)
            {
                var that = (SwitchEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Value != null)
                        {
                            result = result && this.Value.EqualsViewModel(that.Value);
                        }

                        result = result && this.Cases.Count == that.Cases.Count;
                        if (result)
                        {
                            foreach (var item in this.Cases)
                            {
                                var found = false;
                                foreach (var otherItem in that.Cases)
                                {
                                    if (item != null && item.EqualsViewModel(otherItem))
                                    {
                                        found = true;
                                        break;
                                    }

                                }

                                result = result && found;
                            }

                        }

                        if (this.Default != null)
                        {
                            result = result && this.Default.EqualsViewModel(that.Default);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new SwitchEval();
        }

        protected override object CreateDataModelObject()
        {
            return new SwitchEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (SwitchEval)exportModel;
            if (this.Value != null)
            {
                var dynamicProperty = new DynamicProperty(this.Value.Export(exportParameters));
                model.Value = dynamicProperty;
            }

            foreach (var item in this.Cases)
            {
                if (item != null)
                {
                    var convertedItem = item.Export(exportParameters);
                    model.Cases.Add(convertedItem);
                }

            }

            if (this.Default != null)
            {
                var dynamicProperty = new DynamicProperty(this.Default.Export(exportParameters));
                model.Default = dynamicProperty;
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (SwitchEvalDataModel)dataModel;
            model.DisplayText = this.DisplayText;
            if (this.Value != null)
            {
                model.Value = new DynamicPropertyDataModel(this.Value.ToDataModel());
            }

            foreach (var item in this.Cases)
            {
                if (item != null)
                {
                    var convertedItem = item.ToDataModel();
                    model.Cases.Add(convertedItem);
                }

            }

            if (this.Default != null)
            {
                model.Default = new DynamicPropertyDataModel(this.Default.ToDataModel());
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.Value);
        }

        private void DefaultChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.Default);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(SwitchEvalDataModel dataModel = null);

        partial void Initialize(SwitchEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(SwitchEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref SwitchEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class CaseDynamicPropertyDataViewModel : DataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> value;

        private EvalDataViewModelBase evaluation;

        public CaseDynamicPropertyDataViewModel(IMediaShell mediaShell, CaseDynamicPropertyDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Value = new DataValue<string>(string.Empty);
            this.Value.PropertyChanged += this.ValueChanged;
            if (dataModel != null)
            {
                this.Value.Value = dataModel.Value;
                this.DisplayText = dataModel.DisplayText;
                if (dataModel.Evaluation != null)
                {
                    this.Evaluation = this.CreateEvalDataViewModel(dataModel.Evaluation.Evaluation);
                }

            }

            this.Initialize(dataModel);
        }

        protected CaseDynamicPropertyDataViewModel(IMediaShell mediaShell, CaseDynamicPropertyDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Value = (DataValue<string>)dataViewModel.Value.Clone();
            this.Value.PropertyChanged += this.ValueChanged;
            if (dataViewModel.Evaluation != null)
            {
                this.Evaluation = (EvalDataViewModelBase)dataViewModel.Evaluation.Clone();
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Value.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.value);
                if (this.value != null)
                {
                    this.value.PropertyChanged -= this.ValueChanged;
                }

                this.SetProperty(ref this.value, value, () => this.Value);
                if (value != null)
                {
                    this.value.PropertyChanged += this.ValueChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public EvalDataViewModelBase Evaluation
        {
            get
            {
                return this.evaluation;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.evaluation);
                this.SetProperty(ref this.evaluation, value, () => this.Evaluation);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public CaseDynamicProperty Export(object parameters = null)
        {
            var casedynamicproperty = (CaseDynamicProperty)this.CreateExportObject();
            this.DoExport(casedynamicproperty, parameters);
            return casedynamicproperty;
        }

        public CaseDynamicPropertyDataModel ToDataModel()
        {
            var casedynamicproperty = (CaseDynamicPropertyDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(casedynamicproperty);
            return casedynamicproperty;
        }

        public override void ClearDirty()
        {
            if (this.Value != null)
            {
                this.Value.ClearDirty();
            }

            base.ClearDirty();
        }

        public object Clone()
        {
            var clone = new CaseDynamicPropertyDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is CaseDynamicPropertyDataViewModel)
            {
                var that = (CaseDynamicPropertyDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Value.EqualsValue(that.Value);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected object CreateExportObject()
        {
            return new CaseDynamicProperty();
        }

        protected object CreateDataModelObject()
        {
            return new CaseDynamicPropertyDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (CaseDynamicProperty)exportModel;
            model.Value = this.Value.Value;
            model.Evaluation = this.Evaluation.Export(exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (CaseDynamicPropertyDataModel)dataModel;
            model.Value = this.Value.Value;
            model.Evaluation = new DynamicPropertyDataModel(this.Evaluation.ToDataModel());
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void ValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Value);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(CaseDynamicPropertyDataModel dataModel = null);

        partial void Initialize(CaseDynamicPropertyDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(CaseDynamicProperty model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref CaseDynamicPropertyDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class TextToImageEvalDataViewModel : ContainerEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<string> filepatterns;

        public TextToImageEvalDataViewModel(IMediaShell mediaShell, TextToImageEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.FilePatterns = new DataValue<string>(string.Empty);
            this.FilePatterns.PropertyChanged += this.FilePatternsChanged;
            if (dataModel != null)
            {
                this.FilePatterns.Value = dataModel.FilePatterns;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected TextToImageEvalDataViewModel(IMediaShell mediaShell, TextToImageEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.FilePatterns = (DataValue<string>)dataViewModel.FilePatterns.Clone();
            this.FilePatterns.PropertyChanged += this.FilePatternsChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.FilePatterns.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<string> FilePatterns
        {
            get
            {
                return this.filepatterns;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.filepatterns);
                if (this.filepatterns != null)
                {
                    this.filepatterns.PropertyChanged -= this.FilePatternsChanged;
                }

                this.SetProperty(ref this.filepatterns, value, () => this.FilePatterns);
                if (value != null)
                {
                    this.filepatterns.PropertyChanged += this.FilePatternsChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new TextToImageEval Export(object parameters = null)
        {
            var texttoimage = (TextToImageEval)this.CreateExportObject();
            this.DoExport(texttoimage, parameters);
            return texttoimage;
        }

        public new TextToImageEvalDataModel ToDataModel()
        {
            var texttoimage = (TextToImageEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(texttoimage);
            return texttoimage;
        }

        public override void ClearDirty()
        {
            if (this.FilePatterns != null)
            {
                this.FilePatterns.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new TextToImageEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is TextToImageEvalDataViewModel)
            {
                var that = (TextToImageEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.FilePatterns.EqualsValue(that.FilePatterns);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new TextToImageEval();
        }

        protected override object CreateDataModelObject()
        {
            return new TextToImageEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (TextToImageEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.FilePatterns = this.FilePatterns.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (TextToImageEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.FilePatterns = this.FilePatterns.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void FilePatternsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.FilePatterns);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(TextToImageEvalDataModel dataModel = null);

        partial void Initialize(TextToImageEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(TextToImageEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref TextToImageEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class TimeEvalDataViewModel : DateTimeEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<TimeSpan> begin;

        private DataValue<TimeSpan> end;

        public TimeEvalDataViewModel(IMediaShell mediaShell, TimeEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            this.Begin = new DataValue<TimeSpan>(TimeSpan.Zero);
            this.Begin.PropertyChanged += this.BeginChanged;
            this.End = new DataValue<TimeSpan>(TimeSpan.Zero);
            this.End.PropertyChanged += this.EndChanged;
            if (dataModel != null)
            {
                this.Begin.Value = dataModel.Begin;
                this.End.Value = dataModel.End;
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected TimeEvalDataViewModel(IMediaShell mediaShell, TimeEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Begin = (DataValue<TimeSpan>)dataViewModel.Begin.Clone();
            this.Begin.PropertyChanged += this.BeginChanged;
            this.End = (DataValue<TimeSpan>)dataViewModel.End.Clone();
            this.End.PropertyChanged += this.EndChanged;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || this.Begin.IsDirty || this.End.IsDirty;
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<TimeSpan> Begin
        {
            get
            {
                return this.begin;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.begin);
                if (this.begin != null)
                {
                    this.begin.PropertyChanged -= this.BeginChanged;
                }

                this.SetProperty(ref this.begin, value, () => this.Begin);
                if (value != null)
                {
                    this.begin.PropertyChanged += this.BeginChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public DataValue<TimeSpan> End
        {
            get
            {
                return this.end;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.end);
                if (this.end != null)
                {
                    this.end.PropertyChanged -= this.EndChanged;
                }

                this.SetProperty(ref this.end, value, () => this.End);
                if (value != null)
                {
                    this.end.PropertyChanged += this.EndChanged;
                }

                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new TimeEval Export(object parameters = null)
        {
            var time = (TimeEval)this.CreateExportObject();
            this.DoExport(time, parameters);
            return time;
        }

        public new TimeEvalDataModel ToDataModel()
        {
            var time = (TimeEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(time);
            return time;
        }

        public override void ClearDirty()
        {
            if (this.Begin != null)
            {
                this.Begin.ClearDirty();
            }

            if (this.End != null)
            {
                this.End.ClearDirty();
            }

            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new TimeEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is TimeEvalDataViewModel)
            {
                var that = (TimeEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        result = result && this.Begin.EqualsValue(that.Begin);
                        result = result && this.End.EqualsValue(that.End);
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new TimeEval();
        }

        protected override object CreateDataModelObject()
        {
            return new TimeEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (TimeEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            model.Begin = this.Begin.Value;
            model.End = this.End.Value;
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (TimeEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            model.Begin = this.Begin.Value;
            model.End = this.End.Value;
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void BeginChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Begin);
        }

        private void EndChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.End);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(TimeEvalDataModel dataModel = null);

        partial void Initialize(TimeEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(TimeEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref TimeEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public abstract partial class BinaryOperatorEvalDataViewModelBase : EvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private EvalDataViewModelBase left;

        private EvalDataViewModelBase right;

        public BinaryOperatorEvalDataViewModelBase(IMediaShell mediaShell, BinaryOperatorEvalDataModelBase dataModel = null)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                if (dataModel.Left != null && dataModel.Left.Evaluation != null)
                {
                    this.Left = this.CreateEvalDataViewModel(dataModel.Left.Evaluation);
                }

                if (dataModel.Right != null && dataModel.Right.Evaluation != null)
                {
                    this.Right = this.CreateEvalDataViewModel(dataModel.Right.Evaluation);
                }

                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected BinaryOperatorEvalDataViewModelBase(IMediaShell mediaShell, BinaryOperatorEvalDataViewModelBase dataViewModel)
        {
            this.mediaShell = mediaShell;
            if (dataViewModel.Left != null)
            {
                this.Left = (EvalDataViewModelBase)dataViewModel.Left.Clone();
            }

            if (dataViewModel.Right != null)
            {
                this.Right = (EvalDataViewModelBase)dataViewModel.Right.Clone();
            }

            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated || (this.Left != null && this.Left.IsDirty) || (this.Right != null && this.Right.IsDirty);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase Left
        {
            get
            {
                return this.left;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.left);
                this.SetProperty(ref this.left, value, () => this.Left);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        [UserVisibleProperty("Eval", OrderIndex = 0, GroupOrderIndex = 0)]
        public EvalDataViewModelBase Right
        {
            get
            {
                return this.right;
            }
            set
            {
                this.UnregisterIsDirtyPropertyChanged(this.right);
                this.SetProperty(ref this.right, value, () => this.Right);
                this.RegisterIsDirtyPropertyChanged(value);
            }
        }

        public new BinaryOperatorEvalBase Export(object parameters = null)
        {
            var binaryoperatorbase = (BinaryOperatorEvalBase)this.CreateExportObject();
            this.DoExport(binaryoperatorbase, parameters);
            return binaryoperatorbase;
        }

        public new BinaryOperatorEvalDataModelBase ToDataModel()
        {
            var binaryoperatorbase = (BinaryOperatorEvalDataModelBase)this.CreateDataModelObject();
            this.ConvertToDataModel(binaryoperatorbase);
            return binaryoperatorbase;
        }

        public override void ClearDirty()
        {
            if (this.Left != null)
            {
                this.Left.ClearDirty();
            }

            if (this.Right != null)
            {
                this.Right.ClearDirty();
            }

            base.ClearDirty();
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is BinaryOperatorEvalDataViewModelBase)
            {
                var that = (BinaryOperatorEvalDataViewModelBase)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                        if (this.Left != null)
                        {
                            result = result && this.Left.EqualsViewModel(that.Left);
                        }

                        if (this.Right != null)
                        {
                            result = result && this.Right.EqualsViewModel(that.Right);
                        }

                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (BinaryOperatorEvalBase)exportModel;
            if (this.Left != null)
            {
                var dynamicProperty = new DynamicProperty(this.Left.Export(exportParameters));
                model.Left = dynamicProperty;
            }

            if (this.Right != null)
            {
                var dynamicProperty = new DynamicProperty(this.Right.Export(exportParameters));
                model.Right = dynamicProperty;
            }

            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (BinaryOperatorEvalDataModelBase)dataModel;
            model.DisplayText = this.DisplayText;
            if (this.Left != null)
            {
                model.Left = new DynamicPropertyDataModel(this.Left.ToDataModel());
            }

            if (this.Right != null)
            {
                model.Right = new DynamicPropertyDataModel(this.Right.ToDataModel());
            }

            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private void LeftChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.Left);
        }

        private void RightChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(() => this.Right);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(BinaryOperatorEvalDataModelBase dataModel = null);

        partial void Initialize(BinaryOperatorEvalDataViewModelBase dataViewModel);

        partial void ExportNotGeneratedValues(BinaryOperatorEvalBase model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref BinaryOperatorEvalDataModelBase dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class EqualsEvalDataViewModel : BinaryOperatorEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public EqualsEvalDataViewModel(IMediaShell mediaShell, EqualsEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected EqualsEvalDataViewModel(IMediaShell mediaShell, EqualsEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new EqualsEval Export(object parameters = null)
        {
            var equals = (EqualsEval)this.CreateExportObject();
            this.DoExport(equals, parameters);
            return equals;
        }

        public new EqualsEvalDataModel ToDataModel()
        {
            var equals = (EqualsEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(equals);
            return equals;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new EqualsEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is EqualsEvalDataViewModel)
            {
                var that = (EqualsEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new EqualsEval();
        }

        protected override object CreateDataModelObject()
        {
            return new EqualsEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (EqualsEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (EqualsEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(EqualsEvalDataModel dataModel = null);

        partial void Initialize(EqualsEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(EqualsEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref EqualsEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class NotEqualsEvalDataViewModel : BinaryOperatorEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public NotEqualsEvalDataViewModel(IMediaShell mediaShell, NotEqualsEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected NotEqualsEvalDataViewModel(IMediaShell mediaShell, NotEqualsEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new NotEqualsEval Export(object parameters = null)
        {
            var notequals = (NotEqualsEval)this.CreateExportObject();
            this.DoExport(notequals, parameters);
            return notequals;
        }

        public new NotEqualsEvalDataModel ToDataModel()
        {
            var notequals = (NotEqualsEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(notequals);
            return notequals;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new NotEqualsEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is NotEqualsEvalDataViewModel)
            {
                var that = (NotEqualsEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new NotEqualsEval();
        }

        protected override object CreateDataModelObject()
        {
            return new NotEqualsEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (NotEqualsEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (NotEqualsEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(NotEqualsEvalDataModel dataModel = null);

        partial void Initialize(NotEqualsEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(NotEqualsEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref NotEqualsEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class GreaterThanEvalDataViewModel : BinaryOperatorEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public GreaterThanEvalDataViewModel(IMediaShell mediaShell, GreaterThanEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected GreaterThanEvalDataViewModel(IMediaShell mediaShell, GreaterThanEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new GreaterThanEval Export(object parameters = null)
        {
            var greaterthan = (GreaterThanEval)this.CreateExportObject();
            this.DoExport(greaterthan, parameters);
            return greaterthan;
        }

        public new GreaterThanEvalDataModel ToDataModel()
        {
            var greaterthan = (GreaterThanEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(greaterthan);
            return greaterthan;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new GreaterThanEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is GreaterThanEvalDataViewModel)
            {
                var that = (GreaterThanEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new GreaterThanEval();
        }

        protected override object CreateDataModelObject()
        {
            return new GreaterThanEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (GreaterThanEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (GreaterThanEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(GreaterThanEvalDataModel dataModel = null);

        partial void Initialize(GreaterThanEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(GreaterThanEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref GreaterThanEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class GreaterThanOrEqualEvalDataViewModel : BinaryOperatorEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public GreaterThanOrEqualEvalDataViewModel(IMediaShell mediaShell, GreaterThanOrEqualEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected GreaterThanOrEqualEvalDataViewModel(IMediaShell mediaShell, GreaterThanOrEqualEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new GreaterThanOrEqualEval Export(object parameters = null)
        {
            var greaterthanorequal = (GreaterThanOrEqualEval)this.CreateExportObject();
            this.DoExport(greaterthanorequal, parameters);
            return greaterthanorequal;
        }

        public new GreaterThanOrEqualEvalDataModel ToDataModel()
        {
            var greaterthanorequal = (GreaterThanOrEqualEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(greaterthanorequal);
            return greaterthanorequal;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new GreaterThanOrEqualEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is GreaterThanOrEqualEvalDataViewModel)
            {
                var that = (GreaterThanOrEqualEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new GreaterThanOrEqualEval();
        }

        protected override object CreateDataModelObject()
        {
            return new GreaterThanOrEqualEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (GreaterThanOrEqualEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (GreaterThanOrEqualEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(GreaterThanOrEqualEvalDataModel dataModel = null);

        partial void Initialize(GreaterThanOrEqualEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(GreaterThanOrEqualEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref GreaterThanOrEqualEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class LessThanEvalDataViewModel : BinaryOperatorEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public LessThanEvalDataViewModel(IMediaShell mediaShell, LessThanEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected LessThanEvalDataViewModel(IMediaShell mediaShell, LessThanEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new LessThanEval Export(object parameters = null)
        {
            var lessthan = (LessThanEval)this.CreateExportObject();
            this.DoExport(lessthan, parameters);
            return lessthan;
        }

        public new LessThanEvalDataModel ToDataModel()
        {
            var lessthan = (LessThanEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(lessthan);
            return lessthan;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new LessThanEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is LessThanEvalDataViewModel)
            {
                var that = (LessThanEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new LessThanEval();
        }

        protected override object CreateDataModelObject()
        {
            return new LessThanEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (LessThanEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (LessThanEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(LessThanEvalDataModel dataModel = null);

        partial void Initialize(LessThanEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(LessThanEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref LessThanEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public partial class LessThanOrEqualEvalDataViewModel : BinaryOperatorEvalDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        public LessThanOrEqualEvalDataViewModel(IMediaShell mediaShell, LessThanOrEqualEvalDataModel dataModel = null) 
            : base(mediaShell, dataModel)
        {
            this.mediaShell = mediaShell;
            if (dataModel != null)
            {
                this.DisplayText = dataModel.DisplayText;
            }

            this.Initialize(dataModel);
        }

        protected LessThanOrEqualEvalDataViewModel(IMediaShell mediaShell, LessThanOrEqualEvalDataViewModel dataViewModel) 
            : base(mediaShell, dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Initialize(dataViewModel);
        }

        public override bool IsDirty
        {
            get
            {
                bool isDirtyNotGenerated = false;
                this.IsDirtyNotGenerated(ref isDirtyNotGenerated);
                return base.IsDirty || isDirtyNotGenerated;
            }
        }

        public new LessThanOrEqualEval Export(object parameters = null)
        {
            var lessthanorequal = (LessThanOrEqualEval)this.CreateExportObject();
            this.DoExport(lessthanorequal, parameters);
            return lessthanorequal;
        }

        public new LessThanOrEqualEvalDataModel ToDataModel()
        {
            var lessthanorequal = (LessThanOrEqualEvalDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(lessthanorequal);
            return lessthanorequal;
        }

        public override void ClearDirty()
        {
            base.ClearDirty();
        }

        public override object Clone()
        {
            var clone = new LessThanOrEqualEvalDataViewModel(this.mediaShell, this);
            clone.ClonedFrom = this.GetHashCode();
            return clone;
        }

        public override bool EqualsViewModel(object obj)
        {
            bool result = base.EqualsViewModel(obj);
            if (obj is LessThanOrEqualEvalDataViewModel)
            {
                var that = (LessThanOrEqualEvalDataViewModel)obj;
                if (this != that)
                {
                    if (this == null || that == null)
                    {
                        result = false;
                    }
                    else
                    {
                    }

                }

            }
            else
            {
                result = false;
            }

            return result;
        }

        protected override object CreateExportObject()
        {
            return new LessThanOrEqualEval();
        }

        protected override object CreateDataModelObject()
        {
            return new LessThanOrEqualEvalDataModel();
        }

        protected override void DoExport(object exportModel, object exportParameters)
        {
            var model = (LessThanOrEqualEval)exportModel;
            base.DoExport(exportModel, exportParameters);
            this.ExportNotGeneratedValues(model, exportParameters);
        }

        protected override void ConvertToDataModel(object dataModel)
        {
            var model = (LessThanOrEqualEvalDataModel)dataModel;
            base.ConvertToDataModel(model);
            this.ConvertNotGeneratedToDataModel(ref model);
        }

        private EvalDataViewModelBase CreateEvalDataViewModel(EvalDataModelBase dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            var typeName = dataModel.GetType().Name.Replace("DataModel", "DataViewModel");
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for data model '{1} not found", typeName, dataModel));
            }

            return (EvalDataViewModelBase)Activator.CreateInstance(type, mediaShell, dataModel);
        }

        partial void Initialize(LessThanOrEqualEvalDataModel dataModel = null);

        partial void Initialize(LessThanOrEqualEvalDataViewModel dataViewModel);

        partial void ExportNotGeneratedValues(LessThanOrEqualEval model, object exportParameters);

        partial void ConvertNotGeneratedToDataModel(ref LessThanOrEqualEvalDataModel dataModel);

        partial void IsDirtyNotGenerated(ref bool isDirtyNotGenerated);

    }

    public enum EvaluationType
    {
        None
        ,
        And
        ,
        Constant
        ,
        CodeConversion
        ,
        CsvMapping
        ,
        Date
        ,
        DayOfWeek
        ,
        Evaluation
        ,
        Format
        ,
        Generic
        ,
        If
        ,
        IntegerCompare
        ,
        Not
        ,
        Or
        ,
        RegexReplace
        ,
        StringCompare
        ,
        Switch
        ,
        TextToImage
        ,
        Time
        ,
        Equals
        ,
        NotEquals
        ,
        GreaterThan
        ,
        GreaterThanOrEqual
        ,
        LessThan
        ,
        LessThanOrEqual

    }

}
