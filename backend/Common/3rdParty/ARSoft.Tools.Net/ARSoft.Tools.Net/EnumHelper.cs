#region Copyright and License
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

namespace ARSoft.Tools.Net
{
	using System.Reflection;

	internal static class EnumHelper<T>
		where T : struct
	{
		private static readonly Dictionary<T, string> _names;
		private static readonly Dictionary<string, T> _values;

		static EnumHelper()
		{
#if WindowsCE
			var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
			var names = new List<string>(fields.Length);
			var values = new List<T>(fields.Length);

			foreach (var field in fields)
			{
				names.Add(field.Name);
				values.Add((T)field.GetValue(null));
			}
#else
			string[] names = Enum.GetNames(typeof(T));
			T[] values = (T[])Enum.GetValues(typeof(T));
#endif

			_names = new Dictionary<T, string>(names.Count());
			_values = new Dictionary<string, T>(names.Count() * 2);

			for (int i = 0, max = names.Count(); i < max; i++)
			{
				_names[values[i]] = names[i];
				_values[names[i]] = values[i];
				_values[names[i].ToLower()] = values[i];
			}
		}

		public static bool TryParse(string s, bool ignoreCase, out T value)
		{
			if (String.IsNullOrEmpty(s))
			{
				value = default(T);
				return false;
			}

			return _values.TryGetValue((ignoreCase ? s.ToLower() : s), out value);
		}

		public static string ToString(T value)
		{
			string res;
			return _names.TryGetValue(value, out res) ? res : Convert.ToInt64(value).ToString();
		}

		public static Dictionary<T, string> Names
		{
			get { return _names; }
		}

		internal static T Parse(string s, bool ignoreCase, T defaultValue)
		{
			T res;
			return TryParse(s, ignoreCase, out res) ? res : defaultValue;
		}
	}
}