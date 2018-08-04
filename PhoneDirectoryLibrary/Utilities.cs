using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public static class Utilities
    {
        public static string PadValue(string value, int width)
        {
            int valueLength = value.Length;

            if (valueLength > width)
            {
                throw new ArgumentOutOfRangeException($"Cannot create a column of size {valueLength}. Column width is {width}.");
            }

            return value.PadRight(width, ' ');
        }

        /// <summary>
        /// Abstracted this out because it was showing up in multiple places
        /// </summary>
        /// <param name="theDict">Dictionary to which we are adding</param>
        /// <param name="fieldName">The header name for the field</param>
        /// <param name="fieldValue">The value of this instance of the field</param>
        /// <param name="columnWidths">The maximum width for the current column</param>
        public static void AddToDict(ref Dictionary<string, string> theDict, string fieldName, string fieldValue, Dictionary<string, int> columnWidths)
        {
            theDict.Add(PadValue(fieldName, columnWidths[fieldName]), PadValue(fieldValue, columnWidths[fieldName]));
        }
    }
}
