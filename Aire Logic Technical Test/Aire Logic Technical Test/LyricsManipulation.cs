using System;
using System.Collections.Generic;
using System.Text;

namespace Aire_Logic_Technical_Test
{
    public class LyricsManipulation
    {
        public static int CalculateNumberOfWords(string lyrics)
        {
            string normalisedLyrics = lyrics.Replace("\n", " ");
            normalisedLyrics = normalisedLyrics.Replace("\r", " ");

            while (normalisedLyrics.Contains("  "))
            {
                normalisedLyrics = normalisedLyrics.Replace("  ", " ");
            }

            return normalisedLyrics.Split(" ").Length;
        }

        public static float CalculateAverageWordCount(List<int> songWordCounts)
        {
            float totalCount = 0;

            foreach (int count in songWordCounts)
            {
                totalCount += count;
            }

            return (totalCount / songWordCounts.Count);
        }

        public static int CompareAverageWordCounts(float average0, float average1)
        {

            if (average0 > average1)
            {
                return 0;
            }
            else if (average1 > average0)
            {
                return 1;
            }

            return -1;
        }
    }
}
