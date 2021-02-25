using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using MetaBrainz.MusicBrainz;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Aire_Logic_Technical_Test
{
    public enum WorkInclusionType
    {
        None,
        IncludeAll,
        IgnoreAll,
        CaseByCase
    }
    public class ExternalInterface
    {
        static List<string> songNames;
        static WorkInclusionType workInclusionType;

        static int numberOfSongs;
        /// <summary>
        /// Takes the name of an artist and returns a track list of their works.
        /// </summary>
        /// <returns></returns>
        public static List<string> QueryMusicBrainzArtist(int arrayIndex)
        {
            List<string> potentialArtists = new List<string>();
            songNames = new List<string>();            

            try
            {
                var q = new Query("Red Stapler", "19.99", "mailto:a.d.k.johnson@gmail.com");
                var artistInfo = q.FindArtists(Program.artists[arrayIndex], simple: true);

                Console.WriteLine("");
                Console.WriteLine("------------------------");
                Console.WriteLine("SEARCH RESULTS:");


                for (int i = 0; i < artistInfo.Results.Count; i++)
                {
                    Console.WriteLine("" + (i + 1) + "." + artistInfo.Results[i].Item.Name);
                    potentialArtists.Add(artistInfo.Results[i].Item.Name);
                }

                Console.WriteLine("------------------------");
                Console.WriteLine("PLEASE ENTER YOUR ARTIST'S REFERENCE NUMBER: ");

                bool selectionMade = false;
                int selection = 0;

                while (!selectionMade)
                {
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out selection))
                    {
                        if (selection > 0 & selection <= artistInfo.Results.Count)
                        {
                            Program.artists[arrayIndex] = potentialArtists[selection - 1];
                            selectionMade = true;
                        }
                        else
                        {
                            Console.WriteLine("ERROR: INPUT WAS NOT A VALID OPTION, PLEASE SELECT A NUMBER FROM THE LIST");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR: INPUT WAS NOT A VALID NUMBER");
                    }
                }

                if (selection > 0 & selection <= artistInfo.Results.Count)
                {
                    selection--;
                    var id = artistInfo.Results[selection].Item.Id;

                    var selectedArtist = q.LookupArtist(id, Include.Releases);
                    var releaseId = selectedArtist.Releases[0].Id;
                    var works = q.BrowseArtistWorks(id, limit: 100, offset: 0);

                    numberOfSongs = 0;

                    workInclusionType = WorkInclusionType.None;

                    while (works.Results.Count != 0)
                    {
                        foreach (var work in works.Results)
                        {
                            if (work.Type == "Song")
                            {
                                numberOfSongs++;
                                if (!songNames.Contains(work.Title))
                                {
                                    songNames.Add(work.Title);
                                }
                            }
                            else if (workInclusionType == WorkInclusionType.None)
                            {
                                IncludeAllDecisionInput(arrayIndex, work.Title);                                
                            }
                            else
                            {
                                switch (workInclusionType)
                                {
                                    case WorkInclusionType.IncludeAll:
                                        numberOfSongs++;

                                        if (!songNames.Contains(work.Title))
                                        {
                                            songNames.Add(work.Title);
                                        }

                                        break;
                                    case WorkInclusionType.CaseByCase:
                                        AddWorkWithoutSongTypeInput(arrayIndex, work.Title);
                                        break;
                                }
                            }
                        }

                        works = works.Next();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: CANNOT OBTAIN DATA:");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine(ex.StackTrace);
            }

            return songNames;
        }

        /// <summary>
        /// Displays a question relating to whether the works without a type of "song" should all be included
        /// in the search.
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <param name="title">Title of the work (work.title)</param>
        static void IncludeAllDecisionInput(int arrayIndex, string title)
        {
            Console.WriteLine(Program.artists[arrayIndex] + ": " + title + " IS NOT OF TYPE: SONG.");
            Console.WriteLine("DO YOU WANT TO INCLUDE ALL WORK INSTANCES IN THE SEARCH? (Y/N)");

            bool awaitingDecision = true;

            while (awaitingDecision)
            {
                string response = Console.ReadLine();

                if (response == "Y" | response == "y" | response == "yes" | response == "Yes")
                {
                    awaitingDecision = false;
                    workInclusionType = WorkInclusionType.IncludeAll;

                    numberOfSongs++;
                    if (!songNames.Contains(title))
                    {
                        songNames.Add(title);
                    }
                }
                else if (response != string.Empty)
                {
                    awaitingDecision = false;

                    IgnoreAllDecisionInput(arrayIndex, title);

                }
                else
                {
                    Console.WriteLine("NO RESPONSE DETECTED.  PLEASE RESPOND WITH EITHER: Y OR N");
                    Console.WriteLine("");
                }
            }
        }

        /// <summary>
        /// Displays a question relating to whether the works without a type of "song" should all be ignored
        /// from the search.
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <param name="title">Title of the work (work.title)</param>
        static void IgnoreAllDecisionInput(int arrayIndex, string title)
        {
            Console.WriteLine("");
            Console.WriteLine("DO YOU WANT TO IGNORE ALL WORK INSTANCES NOT OF TYPE SONG IN THE SEARCH? (Y/N)");
            Console.WriteLine("NOTE: CHOOSING NOT TO IGNORE ALL WILL RESULT IN SUCH WORK INSTANCES BEING BEING INSPECTED ON A CASE BY CASE BASIS.");

            bool awaitingIgnoreDecision = true;

            while (awaitingIgnoreDecision)
            {
                string ignoreResponse = Console.ReadLine();

                if (ignoreResponse == "Y" | ignoreResponse == "y" | ignoreResponse == "yes" | ignoreResponse == "Yes")
                {
                    awaitingIgnoreDecision = false;
                    workInclusionType = WorkInclusionType.IgnoreAll;
                }
                else if (ignoreResponse != string.Empty)
                {
                    awaitingIgnoreDecision = false;

                    Console.WriteLine("SONGS WILL BE BE INSPECTED ON A CASE BY CASE BASIS.");

                    workInclusionType = WorkInclusionType.CaseByCase;

                    AddWorkWithoutSongTypeInput(arrayIndex, title);

                }
                else
                {
                    Console.WriteLine("NO RESPONSE DETECTED.  PLEASE RESPOND WITH EITHER: Y OR N");
                    Console.WriteLine("");
                }
            }
        }

        /// <summary>
        /// Determines whether a work with a type of something other than "Song" is included in the search.  
        /// This is due to some songs not being input correctly in the MusicBrainz database.
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <param name="title">Title of the work (work.title)</param>
        static void AddWorkWithoutSongTypeInput(int arrayIndex, string title)
        {
            Console.WriteLine(Program.artists[arrayIndex] + ": " + title + " IS NOT OF TYPE: SONG.");
            Console.WriteLine("DO YOU WANT TO INCLUDE IT IN THE SEARCH? (Y/N)");

            bool awaitingDecision = true;

            while (awaitingDecision)
            {
                string response = Console.ReadLine();

                if (response == "Y" | response == "y" | response == "yes" | response == "Yes")
                {
                    awaitingDecision = false;
                    songNames.Add(title);
                    Console.WriteLine(Program.artists[arrayIndex] + ": " + title + " ADDED TO SONG LIST.");
                }
                else if (response != string.Empty)
                {
                    awaitingDecision = false;
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("NO RESPONSE DETECTED.  PLEASE RESPOND WITH EITHER: Y OR N");
                    Console.WriteLine("");
                }
            }
        }

        /// <summary>
        /// Sends a HTTPWebRequest to the Lyrics.ovh database for a particular song's lyrics information.
        /// </summary>
        /// <param name="artist">Name of the artist the wrote/performed the song</param>
        /// <param name="title">Title of the song being queried</param>
        /// <returns></returns>
        public Task GetSongLyrics(string artist, string title)
        {
            title = AlterSongTitleToAvoidIllegalCharacters(title);

            Song song = new Song();
            var encoding = ASCIIEncoding.ASCII;
            string url = "https://api.lyrics.ovh/v1/" + artist + "/" + title;
            string responseData = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 200000;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    responseData = reader.ReadToEnd();

                    dynamic json = JsonConvert.DeserializeObject(responseData);

                    song.artist = artist;
                    song.title = title;

                    if (json.lyrics != null)
                    {
                        song.lyrics = json.lyrics;
                    }
                }

                if (song.lyrics != null)
                {
                    Program.songs.Add(song);
                }
            }
            catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
            {
                Program.songsProcessed++;
                Console.WriteLine("SONGS PROCESSED: " + Program.songsProcessed);

                Program.numSongLyricsNotFound++;
                Program.songLyricsNotFound.Add(title);
                return null;
            }

            Program.songsProcessed++;
            Console.WriteLine("SONGS PROCESSED: " + Program.songsProcessed);

            return null;
        }

        /// <summary>
        /// Removes or replaces characters in a song title that would negatively affect the query string of an HTTPWebRequest.
        /// </summary>
        /// <param name="title">Title of the song being queried</param>
        /// <returns></returns>
        string AlterSongTitleToAvoidIllegalCharacters(string title)
        {
            if (title.Contains("/"))
            {
                title = title.Replace("/", " ");
            }

            if (title.Contains("&"))
            {
                title = title.Replace("&", "and");
            }

            if (title.Contains("%"))
            {
                title = title.Replace("%", "");
            }

            if (title.Contains("."))
            {
                title = title.Replace(".", "");
            }

            return title;
        }
    }
}
