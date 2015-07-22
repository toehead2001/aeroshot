/*  AeroShot - Transparent screenshot utility for Windows
	Copyright (C) 2015 toe_head2001
	Copyright (C) 2012 Caleb Joseph

	AeroShot is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	AeroShot is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>. */

namespace AeroShot
{
    using System;

    static class Switch
    {
        public static Switch<T> On<T>(T value) { return Create(true, value); }
        public static Switch<T> Off<T>(T value) { return Create(false, value); }
        public static Switch<T> Create<T>(bool on, T value) { return new Switch<T>(on, value); }
    }

    sealed class Switch<T>
    {
        public bool On  { get; private set; }
        public T Value  { get; private set; }

        public Switch(bool on, T value) { On = on; Value = value; }

        public T GetValueOrDefault() { return On ? Value : default(T); }

        public TResult Convert<TResult>(Converter<T, TResult> onSelector, TResult offValue = default(TResult))
        {
            return On ? onSelector(Value) : offValue;
        }

        public void WhenOnThen(Action<T> then) { if (On) then(Value); }

        public override string ToString() { return string.Format("{0}", Value); }
    }
}