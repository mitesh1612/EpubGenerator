using System;
using System.Collections.Generic;
using System.Text;

namespace EpubCreatorFromHtml
{
    public class StringHelpers
    {
        public static string ReplaceSpacesWithUnderscores(string stringValue)
        {
            var replacedString = stringValue.Replace(' ', '_');
            return replacedString;
        }
    }
}
