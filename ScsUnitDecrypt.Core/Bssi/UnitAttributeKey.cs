using System.Collections.Generic;
using System.Linq;

namespace ScsUnitDecrypt.Core.Bssi
{
    /// <summary>
    ///     Represent the name of an attribute and the type of the value, also contains extra data when value type is
    ///     <see cref="UnitAttributeType.OrdinalString" />
    /// </summary>
    internal class UnitAttributeKey
    {
        private readonly Dictionary<uint, string> _ordinalValues = new Dictionary<uint, string>();

        internal UnitAttributeKey(UnitAttributeType type, string name)
        {
            Type = type;
            Name = name;
        }

        internal UnitAttributeType Type { get; }

        internal string Name { get; }


        internal void AddOrdinalString(uint index, string value)
        {
            _ordinalValues.Add(index, value);
        }

        /// <summary>
        ///     Gets the string value for the given index.
        /// </summary>
        /// <param name="index">Index of the ordinal string</param>
        /// <returns>String for the given index or "null" (as a string, not literal)</returns>
        internal string GetOrdinalString(uint index)
        {
            return _ordinalValues.ContainsKey(index) ? _ordinalValues[index] : "null";
        }

        internal List<string> GetOrdinalStrings()
        {
            return _ordinalValues.Values.ToList();
        }
    }
}