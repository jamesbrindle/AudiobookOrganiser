using System;
using System.ComponentModel;
using System.Reflection;

namespace FfMpeg.AudibleActivationBytesHelper.Lib
{
    [Obfuscation(Exclude = true)]
    public enum EConvFormat { mp3, mp4 }

    [Obfuscation(Exclude = true)]
    public enum EConvMode { single, chapters, splitChapters, splitTime }

    [Obfuscation(Exclude = true)]
    public enum EInteractionCustomCallback { none, noActivationCode }

    //  Track file naming pattern
    //<author> - <book title> - <track>
    //<track> - <book title> - <author>
    //<book title> - <track>
    //<track> - <book title>
    [Obfuscation(Exclude = true)]
    public enum EFileNaming
    {
        track,
        track_book,
        track_book_author,
        author_book_track,
        book_track,
    }

    //Track title tag naming pattern
    //<author> - <book title> - <track>
    //<track> - <book title> - <author>
    //<book title> - <track>
    //<track> - <book title>
    [Obfuscation(Exclude = true)]
    public enum ETitleNaming
    {
        track,
        track_book,
        track_book_author,
        book_author,
        author_book,
        author_book_track,
        book_track,
    }

    //Track numbering pattern (for track file and track title)
    //<track>
    //<chapter>.<track>
    [Obfuscation(Exclude = true)]
    public enum ETrackNumbering
    {
        track,
        chapter_a_track,
        track_b_chapter_c,
        track_b_nTrks_c,
        chapter_a_track_b_nTrks_c,
        track_b_chapter_c_b_nTrks_c,
    }

    [Obfuscation(Exclude = true)]
    public enum ELongTitle { no, book_series, series_book, as_is }

    [Obfuscation(Exclude = true)]
    public enum EGeneralNaming
    {
        source,
        standard,
        custom,
    }

    [Obfuscation(Exclude = true)]
    public enum EGeneralNamingEx
    {
        source,
        standard,
        custom,
        _nofolders
    }

    [Obfuscation(Exclude = true)]
    public enum EFlatFolderNaming
    {
        author_book,
        book_author,
    }

    [Obfuscation(Exclude = true)]
    [Flags]
    [TypeConverter]
    public enum EAaxCopyMode
    {
        no = 0,
        // book_first = 1,
        // book_with_author = 2,
        // folders_by_author = 4,
        // folders = 8,
        flat__author_book = 10,
        flat__book_author = 11,
        author__book = 12,
        author__author_book = 14,
        author__book_author = 15,
    }

    [Obfuscation(Exclude = true)]
    public enum ENamedChapters
    {
        no,
        yes,
        yesAlwaysWithNumbers
    }

    [Obfuscation(Exclude = true)]
    public enum EOnlineUpdate
    {
        no,
        promptForDownload,
        promptForInstall
    }

    [Obfuscation(Exclude = true)]
    public enum EVerifyAdjustChapters
    {
        no = 0,
        splitChapterMode = 1,
        bothChapterModes = 2,
    }

    [Obfuscation(Exclude = true)]
    public enum EVerifyAdjustChapterMarks
    {
        no = 0,
        splitChapterOrTimeMode = 1,
        allModes = 2,
    }

    [Obfuscation(Exclude = true)]
    public enum EFixAACEncoding
    {
        no = 0,
        withIntermediateCopy = 1,
        allModes = 2,
    }

    [Obfuscation(Exclude = true)]
    enum ETrackDuration
    {
        Min = 3,
        MaxSplitChapter = 15,
        MaxTimeSplit = 90
    }

    [Obfuscation(Exclude = true)]
    public enum EReducedBitRate
    {
        off,
        _128k,
        _96k,
        _64k,
        _48k,
        _32k,
        _24k
    }

    [Obfuscation(Exclude = true)]
    public enum EPreferEmbeddedChapterTimes
    {
        no,
        ifSilent,
        always
    }

    [Obfuscation(Exclude = true)]
    public enum ERoleTag
    {
        none,
        artist,
        albumArtist,
        composer,
        conductor
    }

    [Obfuscation(Exclude = true)]
    public enum ERoleTagAssignment
    {
        none = 0,
        author = 1,
        author__narrator__ = 2, // DEPRECATED
        author_narrator = 3, // => 2
        __narrator__ = 4, // DEPRECATED
        narrator = 5 // => 3
    }

    [Obfuscation(Exclude = true)]
    public enum EOutFolderConflict
    {
        ask,
        overwrite,
        new_folder,
        skip
    }
}

namespace ReAlPDFc.Loader.AudibleActivationBytesHelper.Diagn
{

    /// <summary>
    /// Flags to control dump output
    /// </summary>
    [Flags]
    [Obfuscation(Exclude = true)]
    public enum EDumpFlags
    {
        none = 0,

        /// <summary>
        /// Add a counter to each item in an enumeration
        /// </summary>
        withItmCnt = 1,

        /// <summary>
        /// Include properties with <c>null</c> values 
        /// </summary>
        inclNullVals = 2,

        /// <summary>
        /// Include property description, <see cref="DescriptionAttribute"/> 
        /// </summary>
        inclDesc = 4,

        /// <summary>
        /// Description above property, if included. Behind property by default. 
        /// </summary>
        descOnTop = 8,

        /// <summary>
        /// Include type description, <see cref="DescriptionAttribute"/>
        /// </summary>
        inclTypeDesc = 16,

        /// <summary>
        /// Include description in enumerations 
        /// </summary>
        inclDescInEnum = 32,

        /// <summary>
        /// Inherit attributes defined for properities in base interfaces, <see cref="TreeDecomposition{T}"/> for recognized attributes 
        /// </summary>
        inherInterfaceAttribs = 64,

        /// <summary>
        /// Group properties by implemented interfaces and their hierarchy
        /// </summary>
        byInterface = 128,

        /// <summary>
        /// Include grouping by interface for types further down the hierarchy 
        /// </summary>
        byInterfaceNestedTypes = 256,
    }
}