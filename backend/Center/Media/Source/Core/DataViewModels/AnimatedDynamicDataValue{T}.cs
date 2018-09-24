// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimatedDynamicDataValue{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnimatedDynamicDataValue type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Eval;

    /// <summary>
    /// Defines a <see cref="DataValue&lt;T&gt;"/> specific for dynamic properties.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class AnimatedDynamicDataValue<T> : DynamicDataValue<T>, IAnimatedDataValue
    {
        private AnimationDataViewModelBase animation;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedDynamicDataValue{T}"/> class.
        /// </summary>
        /// <param name="animation">
        /// The animation.
        /// </param>
        /// <param name="evaluation">
        /// The evaluation.
        /// </param>
        public AnimatedDynamicDataValue(AnimationDataViewModelBase animation, EvalDataViewModelBase evaluation = null)
        {
            this.Animation = animation;
            if (evaluation != null)
            {
                this.Formula = evaluation;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedDynamicDataValue{T}"/> class.
        /// </summary>
        /// <param name="evaluation">
        /// The evaluation formula.
        /// </param>
        public AnimatedDynamicDataValue(EvalDataViewModelBase evaluation)
            : base(evaluation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedDynamicDataValue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public AnimatedDynamicDataValue(T value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedDynamicDataValue&lt;T&gt;"/> class.
        /// </summary>
        public AnimatedDynamicDataValue()
        {
        }

        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        public AnimationDataViewModelBase Animation
        {
            get
            {
                return this.animation;
            }

            set
            {
                this.SetProperty(ref this.animation, value, () => this.Animation);
                this.MakeDirty();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value or the Formula is dirty.
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || (this.Formula != null && this.Formula.IsDirty)
                       || (this.Animation != null && this.Animation.IsDirty);
            }
        }

        /// <summary>
        /// Clears the dirty flag of the value and formula.
        /// </summary>
        public override void ClearDirty()
        {
            if (this.Formula != null)
            {
                this.Formula.ClearDirty();
            }

            if (this.Animation != null)
            {
                this.Animation.ClearDirty();
            }

            base.ClearDirty();
        }

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The cloned object.
        /// </returns>
        public override object Clone()
        {
            var clone = new AnimatedDynamicDataValue<T> { Value = this.Value, };
            if (this.Formula != null)
            {
                var eval = (EvalDataViewModelBase)this.Formula;
                clone.Formula = (EvalDataViewModelBase)eval.Clone();
                clone.RestoreFormulaReferences();
            }

            if (this.Animation != null)
            {
                var a = (AnimationDataViewModel)this.Animation;
                clone.Animation = (AnimationDataViewModelBase)a.Clone();
            }

            return clone;
        }

        /// <summary>
        /// Compares the current object with another.
        /// </summary>
        /// <param name="that">the other data Value</param>
        /// <returns>
        /// true if they are equal
        /// </returns>
        public override bool EqualsValue(DataValue<T> that)
        {
            var result = base.EqualsValue(that);

            if (result && this.Animation != null)
            {
                result = this.Animation.EqualsViewModel(((AnimatedDynamicDataValue<T>)that).Animation);
            }

            return result;
        }
    }
}