// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialAsync.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PartialAsync type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper class that allows to execute asynchronous methods with partial methods.
    /// </summary>
    public static class PartialAsync
    {
        /// <summary>
        /// An asynchronous partial method that can run a simple task.
        /// </summary>
        /// <param name="asyncMethod">
        /// The async method to be called asynchronously.
        /// </param>
        public delegate void PartialAsyncAction(ref Func<Task> asyncMethod);

        /// <summary>
        /// An asynchronous partial method that can run a simple task.
        /// </summary>
        /// <param name="asyncMethod">
        /// The async method to be called asynchronously.
        /// </param>
        /// <typeparam name="T">
        /// The type of argument passed to the async method.
        /// </typeparam>
        public delegate void PartialAsyncAction<T>(ref Func<T, Task> asyncMethod);

        /// <summary>
        /// An asynchronous partial method that can run a simple task.
        /// </summary>
        /// <param name="asyncMethod">
        /// The async method to be called asynchronously.
        /// </param>
        /// <typeparam name="T1">
        /// The type of the first argument passed to the async method.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument passed to the async method.
        /// </typeparam>
        public delegate void PartialAsyncAction<T1, T2>(ref Func<T1, T2, Task> asyncMethod);

        /// <summary>
        /// An asynchronous partial method that can run a simple task.
        /// </summary>
        /// <param name="asyncMethod">
        /// The async method to be called asynchronously.
        /// </param>
        /// <typeparam name="TArg">
        /// The type of argument passed to the async method.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The type of argument returned from the async method.
        /// </typeparam>
        public delegate void PartialAsyncFunc<TArg, TReturn>(ref Func<TArg, Task<TReturn>> asyncMethod);

        /// <summary>
        /// An asynchronous partial method that can run a simple task.
        /// </summary>
        /// <param name="asyncMethod">
        /// The async method to be called asynchronously.
        /// </param>
        /// <typeparam name="TArg1">
        /// The type of the first argument passed to the async method.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument passed to the async method.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The type of argument returned from the async method.
        /// </typeparam>
        public delegate void PartialAsyncFunc<TArg1, TArg2, TReturn>(
            ref Func<TArg1, TArg2, Task<TReturn>> asyncMethod);

        /// <summary>
        /// Runs an asynchronous partial method, if one is returned by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">
        /// The partial method used to get an async method.
        /// </param>
        public static async void RunAsync(PartialAsyncAction method)
        {
            Func<Task> asyncMethod = null;
            method(ref asyncMethod);
            if (asyncMethod != null)
            {
                await asyncMethod();
            }
        }

        /// <summary>
        /// Runs an asynchronous partial method, if one is returned by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">
        /// The partial method used to get an async method.
        /// </param>
        /// <param name="arg">
        /// The argument passed to the async method.
        /// </param>
        /// <returns>
        /// The async task result.
        /// </returns>
        /// <typeparam name="T">
        /// The type of argument passed to the async method.
        /// </typeparam>
        public static async Task RunAsync<T>(PartialAsyncAction<T> method, T arg)
        {
            Func<T, Task> asyncMethod = null;
            method(ref asyncMethod);
            if (asyncMethod != null)
            {
                await asyncMethod(arg);
            }
        }

        /// <summary>
        /// Runs an asynchronous partial method, if one is returned by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">
        /// The partial method used to get an async method.
        /// </param>
        /// <param name="arg1">
        /// The first argument passed to the async method.
        /// </param>
        /// <param name="arg2">
        /// The second argument passed to the async method.
        /// </param>
        /// <returns>
        /// The async task result.
        /// </returns>
        /// <typeparam name="T1">
        /// The type of the first argument passed to the async method.
        /// </typeparam>
        /// <typeparam name="T2">
        /// The type of the second argument passed to the async method.
        /// </typeparam>
        public static async Task RunAsync<T1, T2>(PartialAsyncAction<T1, T2> method, T1 arg1, T2 arg2)
        {
            Func<T1, T2, Task> asyncMethod = null;
            method(ref asyncMethod);
            if (asyncMethod != null)
            {
                await asyncMethod(arg1, arg2);
            }
        }

        /// <summary>
        /// Runs an asynchronous partial method, if one is returned by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">
        /// The partial method used to get an async method.
        /// </param>
        /// <param name="arg">
        /// The argument passed to the async method.
        /// </param>
        /// <param name="defaultReturn">
        /// The default return value if the <paramref name="method"/> didn't return an asynchronous method.
        /// </param>
        /// <returns>
        /// The async task result.
        /// </returns>
        /// <typeparam name="TArg">
        /// The type of argument passed to the async method.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The type of argument returned from the async method.
        /// </typeparam>
        public static async Task<TReturn> RunAsync<TArg, TReturn>(
            PartialAsyncFunc<TArg, TReturn> method, TArg arg, TReturn defaultReturn = default(TReturn))
        {
            Func<TArg, Task<TReturn>> asyncMethod = null;
            method(ref asyncMethod);
            if (asyncMethod != null)
            {
                return await asyncMethod(arg);
            }

            return defaultReturn;
        }

        /// <summary>
        /// Runs an asynchronous partial method, if one is returned by the <paramref name="method"/>.
        /// </summary>
        /// <param name="method">
        /// The partial method used to get an async method.
        /// </param>
        /// <param name="arg1">
        /// The first argument passed to the async method.
        /// </param>
        /// <param name="arg2">
        /// The second argument passed to the async method.
        /// </param>
        /// <param name="defaultReturn">
        /// The default return value if the <paramref name="method"/> didn't return an asynchronous method.
        /// </param>
        /// <returns>
        /// The async task result.
        /// </returns>
        /// <typeparam name="TArg1">
        /// The type of the first argument passed to the async method.
        /// </typeparam>
        /// <typeparam name="TArg2">
        /// The type of the second argument passed to the async method.
        /// </typeparam>
        /// <typeparam name="TReturn">
        /// The type of argument returned from the async method.
        /// </typeparam>
        public static async Task<TReturn> RunAsync<TArg1, TArg2, TReturn>(
            PartialAsyncFunc<TArg1, TArg2, TReturn> method,
            TArg1 arg1,
            TArg2 arg2,
            TReturn defaultReturn = default(TReturn))
        {
            Func<TArg1, TArg2, Task<TReturn>> asyncMethod = null;
            method(ref asyncMethod);
            if (asyncMethod != null)
            {
                return await asyncMethod(arg1, arg2);
            }

            return defaultReturn;
        }
    }
}
