MusicBee to Google Music Playlist Sync
======================================

This is a [MusicBee](http://getmusicbee.com) plugin  designed to allow users to synchronise their local playlists to Google Play and a .NET API for the Google Play Music website.

Warning
-------

This plugin is in *ALPHA*. It has not been fully tested, and could do any number of things to your Google Music account. While I have used it to some extent on my own account without issues, there are many, many edge cases that I may have missed. Proceed with caution! *Don't use this if you value your Google Music playlists.* It may delete all of them.

Plugin
------

### Installation

To just use the plugin, download the built DLLs from the "Plugin" directory. Place both included DLLs into the MusicBee\\Plugins directory

### Usage

In MusicBee, Preferences -> Plugins, enable and click "Configure". 

Enter your login details in the box, select to Remember Login and click "Login". The text below the email box will indicate if you've been successful.

The plugin attempts to fetch your Google Music library and playlists after logging in. If you have a big library, this might take a while. Once fetched, Google Music playlists will be shown in the right hand box.

To synchronise your MusicBee playlists to Google Music, select the playlists you'd like to synchronise from the left hand list, ensure the "To Google Music" radio button is selected and click "Synchronise Selected". To synchronise from Google Music to your computer, select the remote playlists you'd like to copy to MusicBee. Ensure that the "From Google Music" radio button is selected and click "Synchronise Selected" Check the "Sync on Startup" box to synchronise every time you start MusicBee. This will synchronise with whatever settings you set before you close the window, and is only one way (based on the radio button selection).

### How it Works

On logging in the plugin fetches a full list of songs and playlists tied to your Google Music account. It goes through each local playlist you wish to synchronise and looks for a playlist with the same name on Google Music. If it finds one, it deletes it and then creates a new playlist with the local playlist's name. For each song in the local playlist, the plugin attempts to match it with a remote song. Songs it can find remote matches for are placed into the new playlist in order. It silently skips those it can't match.

When synchronising Remote to local, it checks for a local playlist with the same name as the remote one. If it exists, then it deletes the .m3u file that playlist is contained in, and recreates it as a blank text file. If it doesn't exist, it creates a new playlist with that name. It then attempts to match each song in the remote playlist with a song in the local library. If it finds a match, it adds it to the new playlist, otherwise it skips that song silently.

### Limitations

1. Error handling is awful. If something goes wrong in communicating with Google, it probably won't tell you in a meaningful way.

2. It deletes the contents of existing playlists with the same name. Without another level of complexity it's not clear what the best solution to this is. The remote playlist appears to have a "lastModified" tag attached to it, so it may be possible to use that and local playlist monitoring to determine which is the newer playlist, but it would probably be quite a lot of work.

3. It's fairly buggy. My MusicBee plugin abilities are limited!

C#/.NET API For Google Play Music
----------------------------------

The plugin uses an incomplete implementation of an API for Google Play Music, [GooglePlayMusicApi](https://github.com/mitchhymel/GooglePlayMusicAPI), which is a port of the Python library, [gmusicapi](https://github.com/simon-weber/Unofficial-Google-Music-API)

