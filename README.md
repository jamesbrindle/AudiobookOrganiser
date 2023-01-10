# AudiobookOrganiser

This a command line utility that 'organises' an audiobook collection to a particular file structure, as well as adding missing meta data and downloading from Audible using audible-cli and converting to M4b.

## Likely Won't Serve Your Needs Out of the Box

I create this simple console app just for myself, to organise my fairly large library of audio books, that, before organisating had a variety of weird and wonderful naming conversions, audio formats and missing audiobooks that were in my Audible library but not local, and missing meta data... Because I got bored.

This application wasn't written for the intention of allowing a multidude of 'options' to cater for anyone elses preferences, nor to create some sor to sort of fan base. If you wish to customise for your preferences, it shouldn't be hard to do so...

I thought it worthwhile though, to make this repo public, because within the code base it does offer quite a lot that some may find very useful, and that took me a while to research and implement from past projects I've worked on... Which include

1. A fairly decent ffmpeg wrapper written in C# (which, I largely got from somewhere else on github, and altered / added to it).
2. Ability to unencrypt both Audible .aax and .aaxc files, using a variety of methods, including RCrack, looking up registry settings for the appearance of 'activation_bytes' (needed for unencrypting), should you have the 'Audible' or 'Audible Manager' app installed or audible-cli activated.
3. ffmpeg built with libfdk_aac.
4. Use of [MediaInfo.dll](https://en.wikipedia.org/wiki/MediaInfo#:~:text=MediaInfo%20is%20a%20free%2C%20cross,and%20K%2DLite%20Codec%20Pack.) with a C# wrapper to store basic meta data implemented.

## Features

### Organise an Existing Library

Iterate through a directory of audiobook files and move and rename them into a particular folder structure:

* \<Aurthor>\<Series?>\<Series-Part?>. <Title-Short> <(Book Series Part)> <Year> (<Narrated - Narrator>).mp4

**For Example**

* <Libary Path>\Arthur C. Clarke\Space Odyssey\1. 2001 (Book 1) - 2008 (Narrated - Dick Hill).m4b

## Add MetaData

It ideally needs the meta data to be present within the file, in order to properly move and rename them according the naming convention. It will attempt 3 different methods to identify meta data fields:

1. It will see if you are using [Reader](https://readarr.com/) which holds a database of your books and their file paths... So it can lookup the author, series, title etc.
**Readarr must be installed.**

2. It will use a books.json file in your [OpenAudible](https://openaudible.org/) data folder if you have one, which will contain the meta data.
**OpenAudible must be installed.**

3. If using the audible-sync arugument, to download and organise from Audible (using [audible-cli](https://github.com/mkb79/audible-cli)) when executing the 'download all books' command, it can reference from the library.json file for the meta data.
**audible-cli must be installed and Activated**

### More About Audible-Cli

This application will indeed download all your books from Audible and convert them to M4b. I have this run app run periodically on a scheduled task to do just this, so once I've purchased a book from Audible, within about half an hour I have the book locally in my library, properly organised and named and converted from either AAX or AAXC audible encrypted format to non-encrypted, M4b format, complete with chapter information.

You must run the [audible-cli](https://github.com/mkb79/audible-cli) CLI once and activate it for it to work. Refer to the website, the commands are simple enough. When running first time run:

```
audible quickstart
```

To get your profile set up and activator with Audible (**Important note, use 'browser' login, rather than username and password legacy loging, when asked).**

To use audible-cli for your own purposes outside of this app, you can refer to the list of commands availble by using:

```
audible -h
```

#### Schedule Task 

See 'SilentRunAudiobookOrganiser.vb' to run this CLI as a scheduled task, hidden under your user context (or you can create a service under Windows).

## Command Line Arguments

There only exists 1

```
AudiobookOrganiser.exe -audible-cli
```

This will perform a 1-way sink from Audible only. It will only refresh from the 'last sync' (not redownloading everything), so it's very quick.

If you **don't** include any arguments, it will perform ane existing library sync only, which, depending on the size of your library and if anything needs converting, can take a while.

I personaly opt to run -audible-sync regularly on a scheduled task, and run without arguments manually.

## Modifying Config

Modify the file 'App.Config' (pre-build file) or 'AudiobookOrganiser.exe.config' (built / bin folder file) to suit your needs.

## DISCLAIMER

Backup your colleciton before running. I won't be hold responsible for messing your library up.









