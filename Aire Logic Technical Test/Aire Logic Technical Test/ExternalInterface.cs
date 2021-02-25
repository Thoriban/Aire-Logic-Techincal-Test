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
    public class ExternalInterface
    {
        /// <summary>
        /// Takes the name of an artist and returns a track list of their works.
        /// </summary>
        /// <returns></returns>
        public static List<string> QueryMusicBrainzArtist(int arrayIndex)
        {
            List<string> potentialArtists = new List<string>();
            List<string> songNames = new List<string>();

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

                    int numberOfSongs = 0;

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
                            else
                            {
                                Console.WriteLine(Program.artists[arrayIndex] + ": " + work.Title + " IS NOT OF TYPE: SONG.");
                                Console.WriteLine("DO YOU WANT TO INCLUDE IT IN THE SEARCH? (Y/N)");

                                bool awaitingDecision = true;

                                while (awaitingDecision)
                                {
                                    string response = Console.ReadLine();

                                    if (response == "Y" | response == "y" | response == "yes" | response == "Yes")
                                    {
                                        awaitingDecision = false;
                                        songNames.Add(work.Title);
                                        Console.WriteLine(Program.artists[arrayIndex] + ": " + work.Title + " ADDED TO SONG LIST.");
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
