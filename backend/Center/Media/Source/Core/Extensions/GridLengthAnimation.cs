// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridLengthAnimation.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The GridLengthAnimation.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    /// The GridLengthAnimation.
    /// </summary>
    public class GridLengthAnimation : AnimationTimeline
    {
        /// <summary>
        /// Dependency property for the From property
        /// </summary>
        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(
            "From",
            typeof(GridLength),
            typeof(GridLengthAnimation));

        /// <summary>
        /// Dependency property for the To property
        /// </summary>
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            "To",
            typeof(GridLength),
            typeof(GridLengthAnimation));

        /// <summary>
        /// Gets the Returns the type of object to animate
        /// </summary>
        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        /// <summary>
        /// Gets or sets the CLR Wrapper for the From dependency property
        /// </summary>
        public GridLength From
        {
            get
            {
                return (GridLength)this.GetValue(GridLengthAnimation.FromProperty);
            }

            set
            {
                this.SetValue(GridLengthAnimation.FromProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the CLR Wrapper for the To property
        /// </summary>
        public GridLength To
        {
            get
            {
                return (GridLength)this.GetValue(GridLengthAnimation.ToProperty);
            }

            set
            {
                this.SetValue(GridLengthAnimation.ToProperty, value);
            }
        }

        /// <summary>
        /// Animates the grid let set
        /// </summary>
        /// <param name="defaultOriginValue">The original value to animate</param>
        /// <param name="defaultDestinationValue">The final value</param>
        /// <param name="animationClock">The animation clock (timer)</param>
        /// <returns>Returns the new grid length to set</returns>
        public override object GetCurrentValue(
            object defaultOriginValue,
            object defaultDestinationValue, 
            AnimationClock animationClock)
        {
            double fromVal = ((GridLength)this.GetValue(GridLengthAnimation.FromProperty)).Value;
            
            // check that from was set from the caller
            if (Math.Abs(fromVal - 1) < 0.0001)
            {
                // set the from as the actual value
                fromVal = ((GridLength)defaultDestinationValue).Value;
            }

            double toVal = ((GridLength)this.GetValue(GridLengthAnimation.ToProperty)).Value;

            if (fromVal > toVal)
            {
                return new GridLength(
                    (1 - animationClock.CurrentProgress.Value) * ((fromVal - toVal) + toVal),
                    GridUnitType.Pixel);
            }

            return new GridLength(
                animationClock.CurrentProgress.Value * ((toVal - fromVal) + fromVal), 
                GridUnitType.Pixel);
        }

        /// <summary>
        /// Creates an instance of the animation object
        /// </summary>
        /// <returns>Returns the instance of the GridLengthAnimation</returns>
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }
    }
}