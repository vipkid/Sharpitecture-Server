using System;
using System.Collections.Generic;

namespace Sharpitecture.Chatting
{
    public static class LineWrapper
    {
        public const int MAX_TEXT_LENGTH = 64;

        public static IEnumerable<string> WrapLines(string input)
        {
            while (input.Contains("  ")) input = input.Replace("  ", " ");
            Colour startCol = c.White;
            string tempLine = string.Empty;
            string[] parts = input.Split(' ');
            string curPart;

            for (int i = 0; i < parts.Length; i++)
            {
                curPart = parts[i];
                int colIndex = -1;

                colIndex = Math.Max(curPart.LastIndexOf('&'), curPart.LastIndexOf('%'));

                if (colIndex != -1)
                {
                    string colString = LookForColourCode(ref curPart, colIndex);
                    if (colString != string.Empty)
                    {
                        Colour tmp = startCol;
                        startCol = ColourDefinitions.FindColour(colString);
                        if (startCol.Equals(default(Colour))) startCol = tmp;
                    }
                }

                if (curPart.Length > 60) tempLine += " ";
                if (tempLine.Length + curPart.Length + 1 > MAX_TEXT_LENGTH)
                {
                    yield return tempLine.Trim(' ');
                    tempLine = "> &" + startCol.code;
                }

                bool skipAppend = false;
                while (curPart.Length > 64)
                {
                    int remaining = 64 - tempLine.Length;
                    tempLine += curPart.Substring(0, remaining);
                    curPart = curPart.Remove(0, remaining);
                    yield return tempLine.Trim(' ');
                    tempLine = string.Empty; //for potential links
                    skipAppend = string.IsNullOrEmpty(curPart);
                }

                if (skipAppend)
                {
                    if (tempLine.Length == 64)
                    {
                        yield return tempLine.Trim(' ');
                        tempLine = "> &" + startCol.code;
                    }
                    else
                    {
                        tempLine += " ";
                    }
                    continue;
                }

                tempLine += curPart + " ";
            }

            if (!string.IsNullOrEmpty(tempLine.Trim(' ')))
                yield return tempLine.Trim(' ');
        }

        static string LookForColourCode(ref string part, int index)
        {
            if (part.Length == index + 1)
                return string.Empty;
            char c = part[index + 1];
            if (!ColourDefinitions.IsValidColourCode(c))
                return string.Empty;
            return "&" + c;
        }
    }
}
