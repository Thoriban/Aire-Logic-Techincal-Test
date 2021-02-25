using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestNumberOfWordsCounter()
        {
            int result = Aire_Logic_Technical_Test.LyricsManipulation.CalculateNumberOfWords("Testing one two three\n\n\n\n  This IS a Test, \r\r\r                 Testing");

            Assert.AreEqual(9, result);
        }

        [TestMethod]
        public void TestAverageCalculation()
        {
            List<int> testData = new List<int>();
            testData.Add(1);
            testData.Add(2);
            testData.Add(3);
            testData.Add(4);
            testData.Add(5);

            float result = Aire_Logic_Technical_Test.LyricsManipulation.CalculateAverageWordCount(testData);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestComparingAverageWordCounts()
        {
            float value0 = 205.5f;
            float value1 = 215.5f;

            float result = Aire_Logic_Technical_Test.LyricsManipulation.CompareAverageWordCounts(value0, value1);

            Assert.AreEqual(1, result);

            value0 = 205.5f;
            value1 = 115.5f;

            result = Aire_Logic_Technical_Test.LyricsManipulation.CompareAverageWordCounts(value0, value1);

            Assert.AreEqual(0, result);
            
            value0 = 15.5f;
            value1 = 15.5f;

            result = Aire_Logic_Technical_Test.LyricsManipulation.CompareAverageWordCounts(value0, value1);

            Assert.AreEqual(-1, result);
        }
    }
}
