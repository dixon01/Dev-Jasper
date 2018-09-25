// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CallMethodAction.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CallMethodAction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Interactivity;

    using NLog;

    /// <summary>
    /// Trigger action used to invoke a method.
    /// </summary>
    public class CallMethodAction : TriggerAction<DependencyObject>
    {
        /// <summary>
        /// The object containing the method to execute.
        /// </summary>
        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register(
            "TargetObject",
            typeof(object),
            typeof(CallMethodAction),
            new PropertyMetadata(CallMethodAction.OnTargetObjectChanged));

        /// <summary>
        /// The name of the method to execute.
        /// </summary>
        public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register(
            "MethodName",
            typeof(string),
            typeof(CallMethodAction),
            new PropertyMetadata(CallMethodAction.OnMethodNameChanged));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private List<CallMethodAction.MethodDescriptor> methodDescriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallMethodAction"/> class.
        /// </summary>
        public CallMethodAction()
        {
            this.methodDescriptors = new List<CallMethodAction.MethodDescriptor>();
        }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        public string MethodName
        {
            get
            {
                return (string)this.GetValue(CallMethodAction.MethodNameProperty);
            }

            set
            {
                this.SetValue(CallMethodAction.MethodNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the target object.
        /// </summary>
        /// <value>
        /// The target object.
        /// </value>
        public object TargetObject
        {
            get
            {
                return this.GetValue(CallMethodAction.TargetObjectProperty);
            }

            set
            {
                this.SetValue(CallMethodAction.TargetObjectProperty, value);
            }
        }

        private object Target
        {
            get
            {
                return this.TargetObject ?? this.AssociatedObject;
            }
        }

        /// <summary>
        /// Invokes the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject == null)
            {
                return;
            }

            var methodDescriptor = this.FindBestMethod(parameter);
            if (methodDescriptor == null)
            {
                if (this.TargetObject == null)
                {
                    return;
                }

                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Error while invoking the method",
                        new object[] { this.MethodName, this.TargetObject.GetType().Name }));
            }

            var parameters = methodDescriptor.Parameters;
            if (parameters.Length == 0)
            {
                methodDescriptor.MethodInfo.Invoke(this.Target, null);
                return;
            }

            if (parameters.Length != 2 || this.AssociatedObject == null || parameter == null
                || !parameters[0].ParameterType.IsInstanceOfType(this.AssociatedObject)
                || !parameters[1].ParameterType.IsInstanceOfType(parameter))
            {
                return;
            }

            methodDescriptor.MethodInfo.Invoke(this.Target, new[] { this.AssociatedObject, parameter });
        }

        /// <summary>
        /// Called when attached.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.UpdateMethodInfo();
        }

        /// <summary>
        /// Called when detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            this.methodDescriptors.Clear();
            base.OnDetaching();
        }

        private static bool AreMethodParamsValid(ParameterInfo[] methodParams)
        {
            if (methodParams.Length == 2)
            {
                if (methodParams[0].ParameterType != typeof(object))
                {
                    return false;
                }

                if (!typeof(EventArgs).IsAssignableFrom(methodParams[1].ParameterType))
                {
                    return false;
                }
            }
            else
            {
                if (methodParams.Length != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static void OnMethodNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var callMethodAction = (CallMethodAction)sender;
            callMethodAction.UpdateMethodInfo();
        }

        private static void OnTargetObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var callMethodAction = (CallMethodAction)sender;
            callMethodAction.UpdateMethodInfo();
        }

        private CallMethodAction.MethodDescriptor FindBestMethod(object parameter)
        {
            if (parameter != null)
            {
                var type = parameter.GetType();
                Logger.Trace("Parameter of type '{0}'", type);
            }

            return
                this.methodDescriptors.FirstOrDefault(
                    methodDescriptor =>
                    !methodDescriptor.HasParameters
                    || (parameter != null && methodDescriptor.SecondParameterType.IsInstanceOfType(parameter)));
        }

        private void UpdateMethodInfo()
        {
            this.methodDescriptors.Clear();
            if (this.Target == null || string.IsNullOrEmpty(this.MethodName))
            {
                return;
            }

            var type = this.Target.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (var methodInfo in methods)
            {
                if (!this.IsMethodValid(methodInfo))
                {
                    continue;
                }

                var parameters = methodInfo.GetParameters();
                if (CallMethodAction.AreMethodParamsValid(parameters))
                {
                    this.methodDescriptors.Add(new CallMethodAction.MethodDescriptor(methodInfo, parameters));
                }
            }

            this.methodDescriptors =
                this.methodDescriptors.OrderByDescending(
                    delegate(CallMethodAction.MethodDescriptor methodDescriptor)
                        {
                            var num = 0;
                            if (methodDescriptor.HasParameters)
                            {
                                for (var type2 = methodDescriptor.SecondParameterType;
                                     type2 != typeof(EventArgs);
                                     type2 = type2.BaseType)
                                {
                                    num++;
                                }
                            }

                            return methodDescriptor.ParameterCount + num;
                        }).ToList();
        }

        private bool IsMethodValid(MethodInfo method)
        {
            return string.Equals(method.Name, this.MethodName, StringComparison.Ordinal)
                   && method.ReturnType == typeof(void);
        }

        private class MethodDescriptor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MethodDescriptor"/> class.
            /// </summary>
            /// <param name="methodInfo">The method info.</param>
            /// <param name="methodParams">The method parameters.</param>
            public MethodDescriptor(MethodInfo methodInfo, ParameterInfo[] methodParams)
            {
                this.MethodInfo = methodInfo;
                this.Parameters = methodParams;
            }

            public MethodInfo MethodInfo { get; private set; }

            public bool HasParameters
            {
                get
                {
                    return this.Parameters.Length > 0;
                }
            }

            public int ParameterCount
            {
                get
                {
                    return this.Parameters.Length;
                }
            }

            public ParameterInfo[] Parameters { get; private set; }

            public Type SecondParameterType
            {
                get
                {
                    return this.Parameters.Length >= 2 ? this.Parameters[1].ParameterType : null;
                }
            }
        }
    }
}