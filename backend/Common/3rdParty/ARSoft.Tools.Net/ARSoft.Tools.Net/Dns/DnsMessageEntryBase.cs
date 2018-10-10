﻿#region Copyright and License
// Copyright 2010..2012 Alexander Reinert
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ARSoft.Tools.Net.Dns
{
	/// <summary>
	///   Base class for a dns name identity
	/// </summary>
	public abstract class DnsMessageEntryBase
	{
		/// <summary>
		///   Domain name
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		///   Type of the record
		/// </summary>
		public RecordType RecordType { get; internal set; }

		/// <summary>
		///   Class of the record (without the <see cref="RecordClassFlag"/> flag)
		/// </summary>
        public RecordClass RecordClass { get; internal set; }

        /// <summary>
        ///   Flag inside the record class
        /// </summary>
        internal bool RecordClassFlag { get; set; }

		internal abstract int MaximumLength { get; }

		/// <summary>
		///   Returns the textual representation
		/// </summary>
		/// <returns> Textual representation </returns>
		public override string ToString()
		{
			return Name + " " + RecordType + " " + RecordClass;
		}
	}
}