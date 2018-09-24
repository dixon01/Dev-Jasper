

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Configuration.Infomedia.Eval;

namespace Gorba.Motion.Infomedia.Core.Presentation.Evaluation
{
	public static class EvaluatorFactory
	{
		public static EvaluatorBase CreateEvaluator(EvalBase eval, IPresentationContext context)
		{
			if (eval == null)
			{
				return null;
			}

			var reference = eval as EvaluationEval;
			if (reference != null)
			{
				var config = context.Config.Config.Evaluations.Find(e => e.Name == reference.Reference);
				if (config == null)
				{
					throw new KeyNotFoundException("Couldn't find referenced evaluation: " + reference.Reference);
				}

				// create the evaluator with the referenced evaluation
				return CreateEvaluator(config.Evaluation, context);
			}


			var evalAnd = eval as AndEval;
			if (evalAnd != null)
			{
				return new AndEvaluator(evalAnd, context);
			}
		
			var evalConstant = eval as ConstantEval;
			if (evalConstant != null)
			{
				return new ConstantEvaluator(evalConstant, context);
			}
		
			var evalCodeConversion = eval as CodeConversionEval;
			if (evalCodeConversion != null)
			{
				return new CodeConversionEvaluator(evalCodeConversion, context);
			}
		
			var evalCsvMapping = eval as CsvMappingEval;
			if (evalCsvMapping != null)
			{
				return new CsvMappingEvaluator(evalCsvMapping, context);
			}
		
			var evalDate = eval as DateEval;
			if (evalDate != null)
			{
				return new DateEvaluator(evalDate, context);
			}
		
			var evalDayOfWeek = eval as DayOfWeekEval;
			if (evalDayOfWeek != null)
			{
				return new DayOfWeekEvaluator(evalDayOfWeek, context);
			}
		
			var evalFormat = eval as FormatEval;
			if (evalFormat != null)
			{
				return new FormatEvaluator(evalFormat, context);
			}
		
			var evalGeneric = eval as GenericEval;
			if (evalGeneric != null)
			{
				return new GenericEvaluator(evalGeneric, context);
			}
		
			var evalIf = eval as IfEval;
			if (evalIf != null)
			{
				return new IfEvaluator(evalIf, context);
			}
		
			var evalIntegerCompare = eval as IntegerCompareEval;
			if (evalIntegerCompare != null)
			{
				return new IntegerCompareEvaluator(evalIntegerCompare, context);
			}
		
			var evalNot = eval as NotEval;
			if (evalNot != null)
			{
				return new NotEvaluator(evalNot, context);
			}
		
			var evalOr = eval as OrEval;
			if (evalOr != null)
			{
				return new OrEvaluator(evalOr, context);
			}
		
			var evalRegexReplace = eval as RegexReplaceEval;
			if (evalRegexReplace != null)
			{
				return new RegexReplaceEvaluator(evalRegexReplace, context);
			}
		
			var evalStringCompare = eval as StringCompareEval;
			if (evalStringCompare != null)
			{
				return new StringCompareEvaluator(evalStringCompare, context);
			}
		
			var evalSwitch = eval as SwitchEval;
			if (evalSwitch != null)
			{
				return new SwitchEvaluator(evalSwitch, context);
			}
		
			var evalTextToImage = eval as TextToImageEval;
			if (evalTextToImage != null)
			{
				return new TextToImageEvaluator(evalTextToImage, context);
			}
		
			var evalTime = eval as TimeEval;
			if (evalTime != null)
			{
				return new TimeEvaluator(evalTime, context);
			}
		
			var evalEquals = eval as EqualsEval;
			if (evalEquals != null)
			{
				return new EqualsEvaluator(evalEquals, context);
			}
		
			var evalNotEquals = eval as NotEqualsEval;
			if (evalNotEquals != null)
			{
				return new NotEqualsEvaluator(evalNotEquals, context);
			}
		
			var evalGreaterThan = eval as GreaterThanEval;
			if (evalGreaterThan != null)
			{
				return new GreaterThanEvaluator(evalGreaterThan, context);
			}
		
			var evalGreaterThanOrEqual = eval as GreaterThanOrEqualEval;
			if (evalGreaterThanOrEqual != null)
			{
				return new GreaterThanOrEqualEvaluator(evalGreaterThanOrEqual, context);
			}
		
			var evalLessThan = eval as LessThanEval;
			if (evalLessThan != null)
			{
				return new LessThanEvaluator(evalLessThan, context);
			}
		
			var evalLessThanOrEqual = eval as LessThanOrEqualEval;
			if (evalLessThanOrEqual != null)
			{
				return new LessThanOrEqualEvaluator(evalLessThanOrEqual, context);
			}
			
			throw new NotSupportedException(string.Format("Unknown evaluation: {0}", eval.GetType().Name));
		}
	}

	
	public sealed partial class AndEvaluator : CollectionEvaluatorBase
	{
		public AndEvaluator(AndEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.AndEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public AndEval AndEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public abstract partial class CollectionEvaluatorBase : EvaluatorBase
	{
		public CollectionEvaluatorBase(CollectionEvalBase eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.EvalsConditions = new List<EvaluatorBase>();
				
			this.CollectionEvalBase = eval;
	
			foreach (var item in eval.Conditions)
			{
				var evalConditions = EvaluatorFactory.CreateEvaluator(item, context);
				evalConditions.ValueChanged += this.HandlerOnValueChanged;
				this.EvalsConditions.Add(evalConditions);
			}
			
			this.Initialize();
		}

		public CollectionEvalBase CollectionEvalBase { get ; private set; }

	
		public List<EvaluatorBase> EvalsConditions { get; private set; }
					
		public override void Dispose()
		{
			this.Deinitialize();
			foreach (var evalConditions in this.EvalsConditions)
			{
				evalConditions.ValueChanged -= this.HandlerOnValueChanged;
				evalConditions.Dispose();
			}
			
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		private void HandlerOnValueChanged(object sender, EventArgs e)
		{
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class ConstantEvaluator : EvaluatorBase
	{
		public ConstantEvaluator(ConstantEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.ConstantEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public ConstantEval ConstantEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public abstract partial class ContainerEvaluatorBase : EvaluatorBase
	{
		public ContainerEvaluatorBase(ContainerEvalBase eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.ContainerEvalBase = eval;
	
			this.EvalEvaluation = EvaluatorFactory.CreateEvaluator(eval.Evaluation, context);
			this.EvalEvaluation.ValueChanged += this.HandlerOnValueChanged;
			
			this.Initialize();
		}

		public ContainerEvalBase ContainerEvalBase { get ; private set; }

	
		public EvaluatorBase EvalEvaluation { get; private set; }
					
		public override void Dispose()
		{
			this.Deinitialize();			
			this.EvalEvaluation.ValueChanged -= this.HandlerOnValueChanged;
			this.EvalEvaluation.Dispose();
			
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		private void HandlerOnValueChanged(object sender, EventArgs e)
		{
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class CodeConversionEvaluator : EvaluatorBase
	{
		public CodeConversionEvaluator(CodeConversionEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.CodeConversionEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public CodeConversionEval CodeConversionEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class CsvMappingEvaluator : EvaluatorBase
	{
		public CsvMappingEvaluator(CsvMappingEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.HandlersMatches = new List<DynamicPropertyHandler>();
				
			this.CsvMappingEval = eval;
	
			this.HandlerDefaultValue = new DynamicPropertyHandler(eval.DefaultValue, string.Empty, context);
			this.HandlerDefaultValue.ValueChanged += this.HandlerOnValueChanged;
						
			foreach (var item in eval.Matches)
			{
				var handler = new DynamicPropertyHandler(item, string.Empty, context);
				handler.ValueChanged += this.HandlerOnValueChanged;
				this.HandlersMatches.Add(handler);
			}
			
			this.Initialize();
			this.InitializeValue();
		}

		public CsvMappingEval CsvMappingEval { get ; private set; }

	
		public DynamicPropertyHandler HandlerDefaultValue { get; private set; }
			
		public List<DynamicPropertyHandler> HandlersMatches { get; private set; }
					
		public override void Dispose()
		{
			this.Deinitialize();
			this.HandlerDefaultValue.ValueChanged -= this.HandlerOnValueChanged;
			this.HandlerDefaultValue.Dispose();
						
			foreach (var handler in this.HandlersMatches)
			{
				handler.ValueChanged -= this.HandlerOnValueChanged;
				handler.Dispose();
			}
			
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		private void HandlerOnValueChanged(object sender, EventArgs e)
		{
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class DateEvaluator : DateTimeEvaluatorBase
	{
		public DateEvaluator(DateEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.DateEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public DateEval DateEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public abstract partial class DateTimeEvaluatorBase : EvaluatorBase
	{
		public DateTimeEvaluatorBase(DateTimeEvalBase eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.DateTimeEvalBase = eval;
	
			this.Initialize();
		}

		public DateTimeEvalBase DateTimeEvalBase { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class DayOfWeekEvaluator : DateTimeEvaluatorBase
	{
		public DayOfWeekEvaluator(DayOfWeekEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.DayOfWeekEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public DayOfWeekEval DayOfWeekEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class FormatEvaluator : EvaluatorBase
	{
		public FormatEvaluator(FormatEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.EvalsArguments = new List<EvaluatorBase>();
				
			this.FormatEval = eval;
	
			foreach (var item in eval.Arguments)
			{
				var evalArguments = EvaluatorFactory.CreateEvaluator(item, context);
				evalArguments.ValueChanged += this.HandlerOnValueChanged;
				this.EvalsArguments.Add(evalArguments);
			}
			
			this.Initialize();
			this.InitializeValue();
		}

		public FormatEval FormatEval { get ; private set; }

	
		public List<EvaluatorBase> EvalsArguments { get; private set; }
					
		public override void Dispose()
		{
			this.Deinitialize();
			foreach (var evalArguments in this.EvalsArguments)
			{
				evalArguments.ValueChanged -= this.HandlerOnValueChanged;
				evalArguments.Dispose();
			}
			
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		private void HandlerOnValueChanged(object sender, EventArgs e)
		{
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class GenericEvaluator : EvaluatorBase
	{
		public GenericEvaluator(GenericEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.GenericEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public GenericEval GenericEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class IfEvaluator : EvaluatorBase
	{
		public IfEvaluator(IfEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.IfEval = eval;
	
			this.HandlerCondition = new DynamicPropertyHandler(eval.Condition, string.Empty, context);
			this.HandlerCondition.ValueChanged += this.HandlerOnValueChanged;
			
			this.HandlerThen = new DynamicPropertyHandler(eval.Then, string.Empty, context);
			this.HandlerThen.ValueChanged += this.HandlerOnValueChanged;
			
			this.HandlerElse = new DynamicPropertyHandler(eval.Else, string.Empty, context);
			this.HandlerElse.ValueChanged += this.HandlerOnValueChanged;
			
			this.Initialize();
			this.InitializeValue();
		}

		public IfEval IfEval { get ; private set; }

	
		public DynamicPropertyHandler HandlerCondition { get; private set; }
			
		public DynamicPropertyHandler HandlerThen { get; private set; }
			
		public DynamicPropertyHandler HandlerElse { get; private set; }
					
		public override void Dispose()
		{
			this.Deinitialize();
			this.HandlerCondition.ValueChanged -= this.HandlerOnValueChanged;
			this.HandlerCondition.Dispose();
			
			this.HandlerThen.ValueChanged -= this.HandlerOnValueChanged;
			this.HandlerThen.Dispose();
			
			this.HandlerElse.ValueChanged -= this.HandlerOnValueChanged;
			this.HandlerElse.Dispose();
			
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		private void HandlerOnValueChanged(object sender, EventArgs e)
		{
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class IntegerCompareEvaluator : ContainerEvaluatorBase
	{
		public IntegerCompareEvaluator(IntegerCompareEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.IntegerCompareEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public IntegerCompareEval IntegerCompareEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class NotEvaluator : ContainerEvaluatorBase
	{
		public NotEvaluator(NotEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.NotEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public NotEval NotEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class OrEvaluator : CollectionEvaluatorBase
	{
		public OrEvaluator(OrEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.OrEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public OrEval OrEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class RegexReplaceEvaluator : ContainerEvaluatorBase
	{
		public RegexReplaceEvaluator(RegexReplaceEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.RegexReplaceEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public RegexReplaceEval RegexReplaceEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class StringCompareEvaluator : ContainerEvaluatorBase
	{
		public StringCompareEvaluator(StringCompareEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.StringCompareEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public StringCompareEval StringCompareEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class SwitchEvaluator : EvaluatorBase
	{
		public SwitchEvaluator(SwitchEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.HandlersCases = new List<DynamicPropertyHandler>();
				
			this.SwitchEval = eval;
	
			this.HandlerValue = new DynamicPropertyHandler(eval.Value, string.Empty, context);
			this.HandlerValue.ValueChanged += this.HandlerOnValueChanged;
						
			foreach (var item in eval.Cases)
			{
				var handler = new DynamicPropertyHandler(item, string.Empty, context);
				handler.ValueChanged += this.HandlerOnValueChanged;
				this.HandlersCases.Add(handler);
			}
			
			this.HandlerDefault = new DynamicPropertyHandler(eval.Default, string.Empty, context);
			this.HandlerDefault.ValueChanged += this.HandlerOnValueChanged;
			
			this.Initialize();
			this.InitializeValue();
		}

		public SwitchEval SwitchEval { get ; private set; }

	
		public DynamicPropertyHandler HandlerValue { get; private set; }
			
		public List<DynamicPropertyHandler> HandlersCases { get; private set; }
			
		public DynamicPropertyHandler HandlerDefault { get; private set; }
					
		public override void Dispose()
		{
			this.Deinitialize();
			this.HandlerValue.ValueChanged -= this.HandlerOnValueChanged;
			this.HandlerValue.Dispose();
						
			foreach (var handler in this.HandlersCases)
			{
				handler.ValueChanged -= this.HandlerOnValueChanged;
				handler.Dispose();
			}
			
			this.HandlerDefault.ValueChanged -= this.HandlerOnValueChanged;
			this.HandlerDefault.Dispose();
			
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		private void HandlerOnValueChanged(object sender, EventArgs e)
		{
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class TextToImageEvaluator : ContainerEvaluatorBase
	{
		public TextToImageEvaluator(TextToImageEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.TextToImageEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public TextToImageEval TextToImageEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class TimeEvaluator : DateTimeEvaluatorBase
	{
		public TimeEvaluator(TimeEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.TimeEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public TimeEval TimeEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public abstract partial class BinaryOperatorEvaluatorBase : EvaluatorBase
	{
		public BinaryOperatorEvaluatorBase(BinaryOperatorEvalBase eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.BinaryOperatorEvalBase = eval;
	
			this.HandlerLeft = new DynamicPropertyHandler(eval.Left, string.Empty, context);
			this.HandlerLeft.ValueChanged += this.HandlerOnValueChanged;
			
			this.HandlerRight = new DynamicPropertyHandler(eval.Right, string.Empty, context);
			this.HandlerRight.ValueChanged += this.HandlerOnValueChanged;
			
			this.Initialize();
		}

		public BinaryOperatorEvalBase BinaryOperatorEvalBase { get ; private set; }

	
		public DynamicPropertyHandler HandlerLeft { get; private set; }
			
		public DynamicPropertyHandler HandlerRight { get; private set; }
					
		public override void Dispose()
		{
			this.Deinitialize();
			this.HandlerLeft.ValueChanged -= this.HandlerOnValueChanged;
			this.HandlerLeft.Dispose();
			
			this.HandlerRight.ValueChanged -= this.HandlerOnValueChanged;
			this.HandlerRight.Dispose();
			
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		private void HandlerOnValueChanged(object sender, EventArgs e)
		{
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class EqualsEvaluator : BinaryOperatorEvaluatorBase
	{
		public EqualsEvaluator(EqualsEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.EqualsEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public EqualsEval EqualsEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class NotEqualsEvaluator : BinaryOperatorEvaluatorBase
	{
		public NotEqualsEvaluator(NotEqualsEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.NotEqualsEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public NotEqualsEval NotEqualsEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class GreaterThanEvaluator : BinaryOperatorEvaluatorBase
	{
		public GreaterThanEvaluator(GreaterThanEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.GreaterThanEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public GreaterThanEval GreaterThanEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class GreaterThanOrEqualEvaluator : BinaryOperatorEvaluatorBase
	{
		public GreaterThanOrEqualEvaluator(GreaterThanOrEqualEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.GreaterThanOrEqualEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public GreaterThanOrEqualEval GreaterThanOrEqualEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class LessThanEvaluator : BinaryOperatorEvaluatorBase
	{
		public LessThanEvaluator(LessThanEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.LessThanEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public LessThanEval LessThanEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
	public sealed partial class LessThanOrEqualEvaluator : BinaryOperatorEvaluatorBase
	{
		public LessThanOrEqualEvaluator(LessThanOrEqualEval eval, IPresentationContext context)
			: base(eval, context)
		{
		
			this.LessThanOrEqualEval = eval;
	
			this.Initialize();
			this.InitializeValue();
		}

		public LessThanOrEqualEval LessThanOrEqualEval { get ; private set; }

			
		public override void Dispose()
		{
			this.Deinitialize();
			base.Dispose();
		}

		protected override void InitializeValue()
		{
			base.InitializeValue();
			this.UpdateValue();
		}
		
		partial void Initialize();

		partial void UpdateValue();

		partial void Deinitialize();
	}
	
}

