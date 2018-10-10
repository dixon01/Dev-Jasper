// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="CheckSumUtil.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The check sum util.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Luminator.PeripheralDimmer.Interfaces;

    using NLog;

    /// <summary>The check sum util.</summary>
    public static class CheckSumUtil
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        /// <summary>The calculate check sum.</summary>
        /// <param name="model">The model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="byte"/>.</returns>
        /// <exception cref="SerializationException">Serialize fails.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="model"/> or <paramref name="func"/> is null.</exception>
        public static byte CalculateCheckSum<T>(T model) where T : class, IChecksum
        {
            // Exclude it in the checksum value from the calculation, Zero it first
            var prop = model.GetType().GetProperty("Checksum");
            if (prop != null)
            {
                prop.SetValue(model, (byte)0);
            }

            var bytes = model.ToBytes();
            var checksum = CheckSum(bytes);
            Debug.WriteLine("Calculate checksum on {0} total bytes = {1}, checksum = {2} 0x{3:X}", typeof(T), bytes.Length, checksum, checksum);
            if (prop != null)
            {
                prop.SetValue(model, checksum);
            }

            return checksum;
        }

        /// <summary>Get Check sum.</summary>
        /// <param name="array">The array.</param>
        /// <returns>The <see cref="byte"/>.</returns>
        public static byte CheckSum(byte[] array)
        {
            byte sum = 0;
            if (array != null)
            {
                // exclude the checksum which is the last byte in the array
                for (var i = 0; i < array.Length - 1; i++)
                {
                    sum += array[i];
                }
            }

            // Two's compliment:
            sum = (byte)((~sum) + 1);
            return sum;
        }

        /// <summary>The get static constant fields on the object.</summary>
        /// <param name="model">The model.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>The <see cref="List"/>Collection of FieldInfo</returns>
        public static List<FieldInfo> GetStaticConstantFields<T>(T model)
        {
            return
                typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                    .ToList();
        }

        /// <summary>Validate the byte array for a valid checksum where the last byte is the checksum.</summary>
        /// <param name="array">The array.</param>
        /// <returns>True if valid else false</returns>
        public static bool IsValidChecksum(byte[] array)
        {
            // array should include the checksum byte as the last
            byte sum = 0;
            if (array != null)
            {
                sum = array.Aggregate(sum, (current, t) => (byte)(current + t));
            }

            var valid = sum == 0;
            return valid;
        }

        #endregion

        #region Methods

        /* Our Goal routine to Match

            // Returns TRUE or FALSE:
            int pcputilsValidateChecksum (void *buffer, int length)
            {
            UCHAR       sum = 0;
            UCHAR*      puchar = (UCHAR*)buffer;
            int         i;

            // include chksum byte in loop:
            for (i = 0; i <= length; i ++)
            {
            sum += puchar [i];
            }

            return (sum == 0);
            }

        */

        /// <summary>Find the checksum prop on the model and it's value or Zero if not found</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns>CheckSum value or Zero if not found</returns>
        private static byte GetCheckSumValue<T>(T model) where T : class
        {
            var value = typeof(T).GetProperty("Checksum")?.GetValue(model);
            return (byte?)value ?? 0;
        }

        #endregion
    }
}