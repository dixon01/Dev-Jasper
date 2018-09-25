// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyPair{TKey1,TKey2}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Define a class that could be used as key for dictionnaries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Utils
{
    /// <summary>
    /// Define a class that could be used as key for dictionaries.
    /// </summary>
    /// <typeparam name="TKey1">
    /// First type of the pair composing the key
    /// </typeparam>
    /// <typeparam name="TKey2">
    /// Second type of the pair composing the key
    /// </typeparam>
    public class KeyPair<TKey1, TKey2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyPair{TKey1,TKey2}"/> class.
        /// </summary>
        /// <param name="first">
        /// The first part of the key.
        /// </param>
        /// <param name="second">
        /// The second part of the key.
        /// </param>
        public KeyPair(TKey1 first, TKey2 second)
        {
            this.First = first;
            this.Second = second;
        }

        /// <summary>
        /// Gets Left key object.
        /// </summary>
        public TKey1 First { get; private set; }

        /// <summary>
        /// Gets Right.
        /// </summary>
        public TKey2 Second { get; private set; }

        /// <summary>
        /// Create a new KeyPair the the specified objects of the specified types.
        /// </summary>
        /// <param name="first">
        /// The first object of TKey1 type.
        /// </param>
        /// <param name="second">
        /// The second object of TKey2 type.
        /// </param>
        /// <typeparam name="TFirstKey">
        /// First type of the pair composing the key
        /// </typeparam>
        /// <typeparam name="TSecondKey">
        /// Second type of the pair composing the key
        /// </typeparam>
        /// <returns>
        /// Returns a new KeyPair composed with the two specified objects of type TKey1 and TKey2.
        /// </returns>
        public static KeyPair<TFirstKey, TSecondKey> Create<TFirstKey, TSecondKey>(TFirstKey first, TSecondKey second)
        {
            return new KeyPair<TFirstKey, TSecondKey>(first, second);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current
        /// <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>;
        /// otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.
        /// </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(KeyPair<TKey1, TKey2>))
            {
                return false;
            }

            return this.Equals((KeyPair<TKey1, TKey2>)obj);
        }

        /// <summary>
        /// Compare the two objects composing the key pair and returns true is they are equals else returns false.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// <b>true</b> if the two objects composing the key pair are equals otherwise <b>false</b>.
        /// </returns>
        public bool Equals(KeyPair<TKey1, TKey2> obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return object.Equals(obj.First, this.First) && object.Equals(obj.Second, this.Second);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.First.GetHashCode() * 397) ^ this.Second.GetHashCode();
            }
        }
    }
}
