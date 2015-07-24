#region The MIT License (MIT)
//
// Copyright (c) 2015 Atif Aziz. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

namespace Gini
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    partial class Ini
    {
        public static IEnumerable<string> Format<TSection, TEntry>(
            IEnumerable<TSection> sections,
            Func<TSection, string> sectionNameSelector,
            Func<TSection, IEnumerable<TEntry>> entriesSelector,
            Func<TEntry, KeyValuePair<string, string>> entrySelector)
        {
            if (sections == null) throw new ArgumentNullException("sections");
            if (sectionNameSelector == null) throw new ArgumentNullException("sectionNameSelector");
            if (entriesSelector == null) throw new ArgumentNullException("entriesSelector");
            if (entrySelector == null) throw new ArgumentNullException("entrySelector");

            return
                from section in sections
                select new
                {
                    Name = sectionNameSelector(section),
                    Entries = entriesSelector(section),
                }
                into section
                from lines in new[]
                {
                    new[] { !string.IsNullOrEmpty(section.Name) ? "[" + section.Name + "]" : null },
                    from entry in section.Entries
                    select entrySelector(entry) into entry
                    select entry.Key + "=" + entry.Value,
                    new[] { string.Empty },
                }
                from line in lines
                where line != null
                select line;
        }
    }
}