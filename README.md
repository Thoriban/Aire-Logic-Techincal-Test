# Aire-Logic-Techincal-Test
Technical Test for stage 2 of the Aire Logic interview process

The Aire Logic Technical Test Programme is a c# console application that takes name of one or multiple artists
and returns the average number of words in the artist's songs.  If multiple artists have been selected
the program will also compare the artist's averages and declare which has the higher average word count.


## Installation
Extract the Zip file to your desired location. Navigate to ../AireLogic Technical Test/bin/Release/netcoreapp3.1/publish/ and
run the Aire Logic Techincal Test.exe executable file.


## Using the Application
Upon starting the application, you will be greeted with an option to decide whether to find the average wordcount of a particular
artist or compare the average word count of two artists.  This requires a numerical input of either 1 or 2. 

From here the programme will request the name of the artist(s) you wish to run the search for.  

After entering the artist you will be given the top 25 matches to your inputted name.  Enter the number next to the artist that 
best matches the artist you wanted to find.

The programme will then start looking for the songs created by the artist you specified.  If a work in the database is not correctly
typed as a song, the programme will ask if you want to include all works, ignore all works that dont have the type "song" or go through
those works on a case by case basis. 

Once all the song titles have been added, the programme will run a series of asynchronous tasks to query the lyrics.ovh api to collect
the songs lyrics.  If a song is not in the lyrics.ovh it will be recorded as missing and printed to the console once all of the other 
song lyrics have been collected.

If the comparison option has been selected, the system will start processing the second artist.

The programme will then print out the average word count for the artist (and in the case of the comparison it will show the artist with the highest word count).

Finally the programme will give you the option of restarting the application or exiting.

## Roadmap
To further develop this program, I would add a dataset for the artist information gained from the MusicBrainz API
and request more information using mutliple queries to allow for the collection of album data in order to compare 
release schedules between artists at varying points throughout their career's.

Additionally, I would replace the command line side of the application with a web interface that would display the
search software and allow for the visualisation of the artist's data in the form of charts relating to the average 
word count of songs for a given time frame, comparing an artist's works across certain periods of their career.
