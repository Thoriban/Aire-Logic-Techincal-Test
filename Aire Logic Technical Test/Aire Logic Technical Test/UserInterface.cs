using System;
using System.Collections.Generic;
using System.Text;

namespace Aire_Logic_Technical_Test
{
    class UserInterface
    {
        /// <summary>
        /// Displays the introduction text to the Console and takes in the type of search to be made.
        /// </summary>
        public static void DisplayIntrodutction()
        {
            Console.WriteLine("------------------------");
            Console.WriteLine("WELCOME TO ANDREW JOHNSON'S AIRE LOGIC TECHNICAL TEST MUSIC DATABASE SEARCH PROGRAMME.");
            Console.WriteLine("");
            Console.WriteLine("PLEASE ENTER A NUMBER FROM THE LIST OF OPTIONS BELOW TO BEGIN YOUR SEARCH: ");
            Console.WriteLine("");
            Console.WriteLine("1. FIND THE AVERAGE WORDCOUNT OF SONGS MADE BY A PARTICULAR ARIST.");
            Console.WriteLine("2. COMPARE THE AVERAGE WORDCOUNT OF SONGS MADE BY A TWO ARISTS.");
            Console.WriteLine("");

            bool validResponse = false;

            while (!validResponse)
            {
                string response = Console.ReadLine();
                int value;

                if (int.TryParse(response, out value))
                {
                    switch (value)
                    {
                        case 1:
                            validResponse = true;
                            Program.searchType = SearchType.AverageWordCount;

                            Program.artists = new string[1];
                            Program.averageWordCounts = new float[1];
                            Program.artists[0] = GetArtistName("");
                            break;
                        case 2:
                            validResponse = true;
                            Program.searchType = SearchType.CompareAverageWordCount;

                            Program.artists = new string[2];
                            Program.averageWordCounts = new float[2];
                            Program.artists[0] = GetArtistName("FIRST ");
                            Program.artists[1] = GetArtistName("SECOND ");
                            break;
                        default:
                            Console.WriteLine("");
                            Console.WriteLine("INVALID VALUE ENTERED.  PLEASE ENTER A VALID NUMBER FROM THE LIST OF OPTIONS ABOVE.");
                            Console.WriteLine("");
                            Console.WriteLine("PLEASE ENTER THE NAME OF THE ARTIST YOU WOULD LIKE TO SEARCH FOR: ");
                            Console.WriteLine("");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("ENTERED INFORMATION WAS NOT A NUMBER.  PLEASE ENTER A VALID NUMBER FROM THE LIST OF OPTIONS ABOVE.");
                    Console.WriteLine("");
                }
            }

        }

        /// <summary>
        /// Requests the name of the artist to search for.  returns the a valid name to the main program
        /// </summary>
        /// <param name="artistIndex"></param>
        /// <returns></returns>
        public static string GetArtistName(string artistIndex)
        {
            bool validInput = false;
            string artistName = string.Empty;
            while (!validInput)
            {
                Console.WriteLine("PLEASE ENTER THE NAME OF THE " + artistIndex + "ARTIST YOU WOULD LIKE TO SEARCH FOR: ");
                Console.WriteLine("");

                artistName = Console.ReadLine();

                if (artistName != string.Empty)
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("INVALID NAME ENTERED.  NAME MUST BE AT LEAST 1 CHARACTER LONG.");
                    Console.WriteLine("");
                }
            }

            return artistName;
        }

        /// <summary>
        /// Displays the average word count for the current artist.
        /// </summary>
        /// <param name="arrayIndex"></param>
        public static void DisplayWordAverages(int arrayIndex)
        {
            Console.WriteLine("");
            Console.WriteLine("------------------------");
            Console.WriteLine("CALCULATING AVERAGE WORD COUNT OF SONGS MADE BY " + Program.artists[arrayIndex] + ".");
            Console.WriteLine("");

            DisplaySearchFinalResult("AVERAGE WORD COUNT", Program.averageWordCounts[arrayIndex], arrayIndex);
        }

        /// <summary>
        /// Outputs the results of the search type to the console
        /// </summary>
        public static void DetetmineResultOutputFormat()
        {
            switch (Program.searchType)
            {
                case SearchType.AverageWordCount:
                    DisplayWordAverages(0);
                    break;
                case SearchType.CompareAverageWordCount:
                    DisplayWordAverages(0);
                    DisplayWordAverages(1);

                    int result = LyricsManipulation.CompareAverageWordCounts(Program.averageWordCounts[0], Program.averageWordCounts[1]);

                    switch (result)
                    {
                        case 0:
                            Console.WriteLine("");
                            Console.WriteLine("------------------------");
                            Console.WriteLine(Program.artists[0] + " HAS THE GREATER AVERAGE WORD COUNT THAN " + Program.artists[1] + ".");
                            Console.WriteLine("------------------------");
                            Console.WriteLine("");
                            break;
                        case 1:
                            Console.WriteLine("");
                            Console.WriteLine("------------------------");
                            Console.WriteLine(Program.artists[1] + " HAS THE GREATER AVERAGE WORD COUNT THAN " + Program.artists[0] + ".");
                            Console.WriteLine("------------------------");
                            Console.WriteLine("");
                            break;
                        default:
                            Console.WriteLine("");
                            Console.WriteLine("------------------------");
                            Console.WriteLine(Program.artists[0] + " AND " + Program.artists[1] + " HAVE THE EXACT SAME WORD COUNT AVERAGE AT " + Program.averageWordCounts[0] + " WORDS PER SONG.");
                            Console.WriteLine("------------------------");
                            Console.WriteLine("");
                            break;
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Displays the final search result of a given artist to the console.
        /// </summary>
        /// <param name="searchTypeText">Text displayed to the console that identifies the result type</param>
        /// <param name="result">The numerical value associated with the search type</param>
        /// <param name="arrayIndex"></param>
        public static void DisplaySearchFinalResult(string searchTypeText, float result, int arrayIndex)
        {
            Console.WriteLine("ARTIST: " + Program.artists[arrayIndex]);
            Console.WriteLine(searchTypeText + ": " + result);
            Console.WriteLine("");
            Console.WriteLine("------------------------");
        }

        /// <summary>
        /// Displays the search end card to the console and asks if the user would like to make a new search.
        /// On a Y, y, Yes, or yes response the program restarts.
        /// Any other input of at least 1 character long results in the program ending.
        /// </summary>
        public static void DisplayProgramRestartText()
        {
            Console.WriteLine("WOULD YOU LIKE TO MAKE A NEW SEARCH? (Y/N)");

            bool awaitingDecision = true;

            while (awaitingDecision)
            {
                string response = Console.ReadLine();

                if (response == "Y" | response == "y" | response == "yes" | response == "Yes")
                {
                    Program.runProgram = true;
                    awaitingDecision = false;
                    Console.WriteLine("");
                }
                else if (response != string.Empty)
                {
                    awaitingDecision = false;
                    Console.WriteLine("");
                    Console.WriteLine("THANK YOU FOR YOUR TIME, ENDING PROGRAM NOW.");
                    Console.WriteLine("------------------------");
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("NO RESPONSE DETECTED.  PLEASE RESPOND WITH EITHER: Y OR N");
                    Console.WriteLine("");
                }
            }
        }
    }
}
