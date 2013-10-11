MusicBee to Google Music Playlist Sync
======================================

This is a [MusicBee](http://getmusicbee.com) plugin  designed to allow users to synchronise their local playlists to Google Play. 

Warning
-------

This plugin is in *ALPHA*. It has not been fully tested, and could do any number of things to your Google Music account. While I have used it to some extent on my own account without issues, there are many, many edge cases that I may have missed. Proceed with caution! *Don't use this if you value your Google Music playlists.* It may delete all of them.

Installation
------------

To just use the plugin, download the zip file in the Plugin directory. All 3 DLLs are needed to function.

Usage
-----

In MusicBee, Preferences -> Plugins, enable and click "Configure". 

Enter your login details in the box, select to Remember Login and click "Login". The text below the email box will indicate if you've been successful.

The plugin attempts to fetch your Google Music library and playlists after logging in. If you have a big library, this might take a while. Once fetched, Google Music playlists will be shown in the right hand box.

To synchronise your MusicBee playlists to Google Music, select the playlists you'd like to synchronise from the left hand list and click "Synchronise to Google Music". Check the "Sync on Startup" box to synchronise every time you start MusicBee.

How it Works
------------

On logging in the plugin fetches a full list of songs and playlists tied to your Google Music account. It goes through each local playlist you wish to synchronise and looks for a playlist with the same name on Google Music. If it finds one, it deletes it and then creates a new playlist with the local playlist's name. For each song in the local playlist, the plugin attempts to match it with a remote song. Songs it can find remote matches for are placed into the new playlist in order. It silently skips those it can't match.

Limitations
-----------

1. Error handling is awful. If something goes wrong in communicating with Google, it probably won't tell you in a meaningful way.

2. Currently sync is one way only. There's no reason it can't be too ways - in fact full playlist details are already downloaded from Google Music. My knowledge of the MusicBee API is limited, but the next step is probably allowing downloading of playlists.

3. It deletes the contents of existing playlists with the same name. Without another level of complexity it's not clear what the best solution to this is. The remote playlist appears to have a "lastModified" tag attached to it, so it may be possible to use that and local playlist monitoring to determine which is the newer playlist, but it would probably be quite a lot of work.