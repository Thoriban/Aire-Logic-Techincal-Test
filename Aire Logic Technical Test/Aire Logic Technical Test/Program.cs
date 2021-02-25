// Produce a program that, when given the name of an artist, will produce the average number of words in their songs
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Aire_Logic_Technical_Test
{
    public enum SearchType
    {
        AverageWordCount,
        CompareAverageWordCount
    }

    public class Program
    {
        public static string[] artists;

        public static SearchType searchType;

        public static float[] averageWordCounts;

        static List<int> songWordCounts;
        public static List<Song> songs;
        public static int songsProcessed;
        static List<Task> tasks;

        public static int numSongLyricsNotFound;
        public static List<string> songLyricsNotFound;

        public static bool runProgram;

        static void Main(string[] args)
        {
            runProgram = true;

            while (runProgram)
            {
                InitialiseVariables();

                UserInterface.DisplayIntrodutction();
                
                GetArtistAndLyricInformation();

                UserInterface.DetetmineResultOutputFormat();

                UserInterface.DisplayProgramRestartText();
            }
        }

        /// <summary>
        /// Initialises all variables used within the program.
        /// </summary>
        static void InitialiseVariables()
        {
            runProgram = false;
            numSongLyricsNotFound = 0;
            songs = new List<Song>();            
            tasks = new List<Task>();            
            songWordCounts = new List<int>();            
            songLyricsNotFound = new List<string>();
        }

        /// <summary>
        /// Runs through the API requests to gather first the arist's information from MusicBrainz and then the Lyrics for each song from lyrics.ovh
        /// </summary>
        static void GetArtistAndLyricInformation()
        {
            ExternalInterface externalInterface = new ExternalInterface();

            // Add option for the various pieces of data that can be examined here

            for (int i = 0; i < artists.Length; i++)
            {
                List<string> songTitles = ExternalInterface.QueryMusicBrainzArtist(i);

                //Average word count in a song
                Console.WriteLine("");
                Console.WriteLine("--------------------------");
                Console.WriteLine("COLLECTING LYRICS FOR " + artists[i] + ".  NUMBER OF SONGS TO PROCESS: " + songTitles.Count);
                Console.WriteLine("--------------------------");
                Console.WriteLine("");

                songsProcessed = 0;

                //API calls to the Lyrics.ovh webservice have been placed in Asynchronous tasks due to the lengthy amount of time each call takes.
                foreach (string songTitle in songTitles)
                {
                    Task getLyrics = Task.Run(() => externalInterface.GetSongLyrics(artists[i], songTitle));
                    tasks.Add(getLyrics);

                    Thread.Sleep(500);
                }

                try
                {
                    Task.WaitAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine("--------------------------");
                    Console.WriteLine(numSongLyricsNotFound + " SONGS FAILED TO RETURN LYRICS FROM THE API DUE TO A 502 ERROR.  THE FOLLOWING SONGS MAY NOT BE AVAILABLE IN THE TARGETTED DATABASE:");

                    foreach (string songTitle in songLyricsNotFound)
                    {
                        Console.WriteLine(songTitle);
                    }

                    Console.WriteLine("--------------------------");
                    Console.WriteLine("");
                }

                Console.WriteLine("");
                Console.WriteLine("--------------------------");
                Console.WriteLine("ALL AVAILABLE LYRICS HAVE NOW BEEN OBTAINED.");
                Console.WriteLine("--------------------------");
                Console.WriteLine("");

                CalculateWordCountAverage(i);
            }
        }
                
        /// <summary>
        /// Calculate the Word Count of each song in the artist's song list and determine the average number of words.
        /// </summary>
        /// <param name="arrayIndex"></param>
        public static void CalculateWordCountAverage(int arrayIndex)
        {
            foreach (Song song in songs)
            {
                if (song.lyrics != null)
                {
                    songWordCounts.Add(LyricsManipulation.CalculateNumberOfWords(song.lyrics));
                }
            }

            averageWordCounts[arrayIndex] = LyricsManipulation.CalculateAverageWordCount(songWordCounts);
        }
    }
}
