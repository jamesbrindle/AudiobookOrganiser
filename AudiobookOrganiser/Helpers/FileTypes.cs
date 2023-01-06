using System.Linq;

namespace AudiobookOrganiser.Helpers
{
    public static class FileTypes
    {
        public static string[] Image
        {
            get
            {
                string[] ext = new string[] { ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".bmp", ".raw", ".gif", ".eps", ".emf", ".svg", ".ico" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Image_All
        {
            get
            {
                string[] ext = new string[] { ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".bmp", ".raw", ".gif", ".svg", ".emf",
                                              ".threefr",".threeg2",".threegp",".a",".aai",".ai",".art",".arw",".avs",".b",".bgr",".bgra",".bgro",".brf",".c",".cal",".cals",".canvas",
                                              ".caption",".cin",".cip",".clip",".clipboard",".cmyk",".cmyka",".cr2",".cr3",".crw",".cube",".cur",".cut",".dcm",".dcr",".dcraw",".dcx",
                                              ".dds",".dfont",".dib",".dng",".dpx",".dxt1",".dxt5",".epdf",".epi",".eps",".epsi",".ept",".erf",".exr",".fax",".fits",".flif",".fractal",
                                              ".fts",".g",".g3",".g4",".gif87",".gradient",".gray",".graya",".group4",".hald",".hdr",".heic",".histogram",".hrz",".icb",".ico",".icon",
                                              ".iiq",".inline",".ipl",".isobrl",".isobrl6",".j2c",".j2k",".jng",".jnx",".jp2",".jpc",".jpe",".jpm",".jps",".jpt",".k",".k25",".kdc",".m",
                                              ".mac",".map",".mask",".mat",".matte",".mef",".miff",".mng",".mono",".mpc",".mrw",".msl",".mtv",".nef",".nrw",".null",".o",".orf",
                                              ".otb",".otf",".pal",".palm",".pam",".pango",".pattern",".pbm",".pcd",".pcds",".pcl",".pct",".pcx",".pdb",".pdfa",".pef",".pes",".pfa",".pfb",
                                              ".pfm",".pgm",".pgx",".picon",".pict",".pix",".pjpeg",".plasma",".pocketmod",".ppm",".ps",".ps2",".ps3",".psb",".psd",".ptif",".pwp",".r",
                                              ".radialgradient",".raf",".ras",".raw",".rgb",".rgb565",".rgba",".rgbo",".rgf",".rla",".rle",".rmf",".rw2",".scr",".screenshot",".sct",".sgi",
                                              ".shtml",".six",".sixel",".sparsecolor",".sr2",".srf",".stegano",".sun",".svg",".svgz",".tga",".thumbnail",".tile",".tim",".tm2",".ttc",".ubrl",
                                              ".ubrl6",".uil",".uyvy",".vda",".vicar",".viff",".vips",".vst",".webp",".wbmp",".wmf",".wpg",".x3f",".xbm",".xc",".xcf",".xpm",".xps",".xv",
                                              ".y",".ycbcr",".ycbcra",".yuv",".pjpeg", ".jjiff" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Image_Basic
        {
            get
            {
                string[] ext = new string[] { ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".bmp", ".raw", ".gif", ".svg", ".emf", ".ico" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Image_Complex
        {
            get
            {
                string[] ext = new string[] { ".threefr",".threeg2",".threegp",".a",".aai",".ai",".art",".arw",".avs",".b",".bgr",".bgra",".bgro",".brf",".c",".cal",".cals",".canvas",
                                              ".caption",".cin",".cip",".clip",".clipboard",".cmyk",".cmyka",".cr2",".cr3",".crw",".cube",".cur",".cut",".dcm",".dcr",".dcraw",".dcx",
                                              ".dds",".dfont",".dib",".dng",".dpx",".dxt1",".dxt5",".epdf",".epi",".eps",".epsi",".ept",".erf",".exr",".fax",".fits",".flif",".fractal",
                                              ".fts",".g",".g3",".g4",".gif87",".gradient",".gray",".graya",".group4",".hald",".hdr",".heic",".histogram",".hrz",".icb",".ico",".icon",
                                              ".iiq",".inline",".ipl",".isobrl",".isobrl6",".j2c",".j2k",".jng",".jnx",".jp2",".jpc",".jpe",".jpm",".jps",".jpt",".k",".k25",".kdc",".m",
                                              ".mac",".map",".mask",".mat",".matte",".mef",".miff",".mng",".mono", ".mpc",".mrw",".msl",".mtv",".nef",".nrw",".null",".o",".orf",
                                              ".otb",".otf",".pal",".palm",".pam",".pango",".pattern",".pbm",".pcd",".pcds",".pcl",".pct",".pcx",".pdb",".pdfa",".pef",".pes",".pfa",".pfb",
                                              ".pfm",".pgm",".pgx",".picon",".pict",".pix",".pjpeg",".plasma",".pocketmod",".ppm",".ps",".ps2",".ps3",".psb",".psd",".ptif",".pwp",".r",
                                              ".radialgradient",".raf",".ras",".raw",".rgb",".rgb565",".rgba",".rgbo",".rgf",".rla",".rle",".rmf",".rw2",".scr",".screenshot",".sct",".sgi",
                                              ".shtml",".six",".sixel",".sparsecolor",".sr2",".srf",".stegano",".sun",".svg",".svgz",".tga",".thumbnail",".tile",".tim",".tm2",".ttc",".ubrl",
                                              ".ubrl6",".uil",".uyvy",".vda",".vicar",".viff",".vips",".vst",".webp",".wbmp",".wmf",".wpg",".x3f",".xbm",".xc",".xcf",".xpm",".xps",".xv",
                                              ".y",".ycbcr",".ycbcra",".yuv",".pjpeg", ".jjiff" };

                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Video
        {
            get
            {
                string[] ext = new string[] { ".ts", ".swf", ".mov", ".mp4", ".m4v", ".3gp", ".3g2", ".flv", ".f4v", ".avi", ".mpgeg", ".mpg", ".wmv", ".asf", ".ram", ".mkv", ".webm" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Audio
        {
            get
            {
                string[] ext = new string[] { ".aax", ".aaxc", ".aac", ".mp3", ".wav", ".aif", ".aiff", ".mpa", ".m4a", ".m4b", ".wma", ".3gp", ".acc", ".aa", ".act", ".aiff", ".alac", ".amr", ".ape",
                                              ".au", ".awb", ".dss", ".dvf", ".flac", ".ivs", ".m4b", ".m4p", ".mmf", ".mpc", ".msv", ".nsf", ".oof", ".oga", ".mogg",
                                              ".opus", ".ra", ".rm", ".raw", ".rf64", "sln", ".tta", ".voc", ".vox", ".wv", "8svx", ".cda" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Html
        {
            get
            {
                string[] ext = new string[] { ".html", ".htm" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Adobe
        {
            get
            {
                string[] ext = new string[] { ".psd", ".ai", ".indd", ".ps", ".eps", ".prn", ".pdf" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Pdf
        {
            get
            {
                string[] ext = new string[] { ".pdf" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] OpenOffice
        {
            get
            {
                string[] ext = new string[] { ".odt", ".odp", ".ods", ".odg", ".odf", ".sxw", ".sxi", ".sxc", ".sxd", ".stw" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Text
        {
            get
            {
                string[] ext = new string[] { ".txt" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Csv
        {
            get
            {
                string[] ext = new string[] { ".csv", ".tsv" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Data
        {
            get
            {
                string[] ext = new string[] {   ".fmat", ".roadtrip", ".quicken2017", ".h12", ".vmt", ".u10", ".zmc", ".bld", ".clp", ".ebuild", ".vcs", ".prs", ".potm", ".hst",
                                                ".ppsm", ".xlc", ".dockzip", ".kpr", ".nitf", ".aifb", ".dvo", ".sqr", ".quicken2015", ".ali", ".hdf", ".mdl", ".nbp", ".dif", ".tdb", ".txd", ".not",
                                                ".fcpevent", ".tax2019", ".edi", ".aw", ".cub", ".mls", ".jasper", ".xem", ".egp", ".bgt", ".kpf", ".nrl", ".bin", ".h13", "0.001", ".quickendata", ".dat",
                                                ".cel", ".rox", ".prj", ".adx", ".net", ".menc", ".pptx", ".ppt", ".fsc", ".mmc", ".uwl", ".livereg", ".prdx", ".tmx", ".xmlper", ".ta9", ".mbg", ".mox",
                                                ".h17", ".wjr", ".tax2010", ".drl", ".topc", ".tt18", ".lix", ".imt", ".rte", ".itmsp", ".t08", ".i5z", ".xft", ".met", ".tt12", ".hl", ".qmtf", ".twh",
                                                ".pj2", ".dm2", ".dii", ".vsx", ".styk", ".hyv", ".sq", ".mosaic", ".pro6plx", ".capt", ".tax2018", ".otln", ".plw", ".ald", ".kdc", ".tbl", ".ldif", ".vok",
                                                ".xdna", ".ncorx", ".liveupdate", ".t12", ".aby", ".dvdproj", ".qb2017", ".ckt", ".gedcom", ".mpkt", ".seo", ".mdf", ".contact", ".ggb", ".photoslibrary",
                                                ".adt", ".pcb", ".blg", ".cdx", ".exx", ".bdic", ".pka", ".sub", ".jef", ".ip", ".wdf", ".vdf", ".tax2016", ".opju", ".snag", ".abcd", ".tar", ".tcc", ".fil",
                                                ".trk", ".rbt", ".enl", ".lib", ".pps", ".ova", ".jph", ".wpc", ".ral", ".ink", ".q08", ".wgt", ".bci", ".rgo", ".qrp", ".pkt", ".fdb", ".grf", ".pdx", ".rfa",
                                                ".zap", ".oo3", ".obj", ".opx", ".mno", ".vxml", ".rfo", ".ndx", ".rvt", ".bvp", ".l6t", ".grade", ".usr", ".aam", ".bgl", ".keychain", ".ppsx", ".pfc",
                                                ".lbl", ".wab", ".fox", ".xpt", ".sav", ".pdb", ".phb", ".tsv", ".qpb", ".q09", ".lms", ".tra", ".xml", ".mdl", ".mwf", ".ofc", ".fdb", ".iif", ".btm",
                                                ".xlf", ".dam", ".pkb", ".pcr", ".emlxpart", ".fcs", ".ttk", ".dsz", ".np", ".tax2009", ".jnt", ".odp", ".ptf", ".inp", ".csv", ".rsc", ".xlt", ".rpt",
                                                ".idx", ".poi", ".ppf", ".cma", ".notebook", ".sdf", ".acc", ".mmp", ".mai", ".qdf", ".efx", ".mat", ".3dr", ".ofx", ".vcf", ".key", ".one", ".myi", ".bcm",
                                                ".slp", ".jdb", ".vdb", ".gwk", ".tdl", ".ovf", ".oeaccount", ".sen", ".ii", ".sds", ".pks", ".mpx", ".flp", ".mnc", ".hda", ".emb", ".xpj", ".dfproj",
                                                ".upoi", ".t18", ".sc45", ".wtb", ".npl", ".kpz", ".ev", ".pdx", ".lvm", ".t07", ".t11", ".aft", ".mth", ".itl", ".crtx", ".box", ".xsl", ".sdf", ".stm",
                                                ".in", ".oft", ".rp", ".mpp", ".gs", ".slx", ".xfd", ".paf", ".xrdml", ".celtx", ".otp", ".xpg", ".uccapilog", ".iba", ".dcmd", ".mjk", ".ptn", ".gcw",
                                                ".twb", ".anme", ".rcg", ".svf", ".tax2015", ".inx", ".abp", ".ptz", ".lcm", ".trd", ".gbr", ".qb2011", ".tax2011", ".t13", ".grk", ".tax2017", ".lp7",
                                                ".lgi", ".trs", ".adcp", ".gdt", ".qb2013", ".out", ".pst", ".pds", ".cap", ".sps", ".idx", ".vce", ".mcdx", ".rte", ".rdb", ".jrprint", ".pptm", ".m",
                                                ".fob", ".gpi", ".pmo", ".ffwp", ".potx", ".cdf", ".prj", ".ged", ".tst", ".tbk", ".dsb", ".mdm", ".sdp", ".vcd", ".4dv", ".lsf", ".windowslivecontact",
                                                ".mmap", ".qb2014", ".sta", ".ima", ".qvw", ".ab3", ".enex", ".dcm", ".exp", ".xmcd", ".jrxml", ".gan", ".gtp", ".scd", ".pxl", ".cna", ".xrp", ".wgt",
                                                ".pd4", ".pod", ".grr", ".te3", ".fpsl", ".lmx", ".rou", ".mdsx", ".dbd", ".t16", ".bjo", ".t05", ".t06", ".rodz", ".mmf", ".kth", ".pd5", ".tdm", ".vrd",
                                                ".gno", ".tdt", ".phm", ".grv", ".moho", ".sca", ".tef", ".fmc", ".tt10", ".ddcx", ".id2", ".tkfl", ".dcf", ".hcu", ".sar", ".flo", ".t10", ".ppf", ".mdc",
                                                ".npt", ".tet", ".rnq", ".wea", ".sqd", ".das", ".pdas", ".ndk", ".itx", ".ulf", ".mw", ".pkh", ".spv", ".mdj", ".tcx", ".vi", ".mcd", ".cdx", ".exif",
                                                ".isf", ".kap", ".qfx", ".mdx", ".fxp", ".tpf", ".esx", ".stykz", ".ctf", ".tax2013", ".ftw", ".xfdf", ".tb", ".er1", ".dal", ".dsy", ".qif", ".mws",
                                                ".kismac", ".rpp", ".ptb", ".cvd", ".clm", ".snapfireshow", ".mph", ".wb3", ".pcapng", ".qbw", ".kid", ".vdx", ".mbx", ".qb2012", ".tt13", ".trs", ".sgml",
                                                ".mtw", ".ddc", ".brw", ".pjm", ".dmsp", ".mmp", ".t09", ".epw", ".xslt", ".fop", ".mex", ".vtx", ".csa", ".kpx", ".lgh", ".fro", ".hml", ".gc", ".spub",
                                                ".cvn", ".opx", ".xdb", ".pxf", ".zdc", ".wnk", ".sle", ".lsl", ".ixb", ".shw", ".rod", ".swk", ".qb2009", ".ulz", ".tfa", ".tpb", ".mbg", ".blb", ".dwi",
                                                ".wb2", ".kpp", ".odx", ".ond", ".hcc", ".lix", ".igc", ".omp", ".txf", ".itm", ".ivt", ".huh", ".qmbl", ".fhc", ".stp" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Database
        {
            get
            {
                string[] ext = new string[] { ".te", ".temx", ".accdt", ".accdc", ".teacher", ".ddl", ".gdb", ".eco", ".sqlitedb", ".pdb", ".trm", ".accft", ".fic", ".sqlite",
                                              ".dtsx", ".db", ".sqlite3", ".db.crypt8", ".nyf", ".itdb", ".mdf", ".4dl", ".v12", ".db.crypt", ".marshal", ".daschema", ".udl",
                                              ".alf", ".dbc", ".daconnections", ".crypt9", ".mar", ".his", ".p97", ".gdb", ".db", ".db.crypt12", ".accde", ".pdm", ".abs", ".wmdb",
                                              ".db3", ".fp3", ".crypt6", ".sql", ".pan", ".fp7", ".sdf", ".crypt12", ".sis", ".xmlff", ".db-wal", ".dp1", ".oqy", ".mdb", ".usr",
                                              ".adf", ".dbs", ".crypt7", ".trc", ".cdb", ".fpt", ".wdb", ".dlis", ".dbf", ".crypt8", ".ask", ".hdb", ".qvd", ".btr", ".flexolibrary",
                                              ".dcb", ".$er", ".frm", ".accdb", ".dxl", ".adp", ".xld", ".dsn", ".rpd", ".sdb", ".sdb", ".fdb", ".mwb", ".db-journal", ".odb", ".rodx",
                                              ".cpd", ".nnt", ".cdb", ".crypt5", ".sdc", ".mav", ".orx", ".dbx", ".fdb", ".rod", ".scx", ".grdb", ".abcddb", ".nsf", ".sdb", ".4dd",
                                              ".accdr", ".adb", ".myd", ".edb", ".cdb", ".sdb", ".mpd", ".ndf", ".kdb", ".maq", ".lwx", ".lgc", ".fmp", ".ib", ".nwdb", ".ihx", ".udb",
                                              ".pdb", ".fmp12", ".vvv", ".ora", ".trc", ".odb", ".vis", ".qry", ".db-shm", ".cma", ".jet", ".mdn", ".mdbhtml", ".accdw", ".db2",
                                              ".dacpac", ".rctd", ".dbv", ".nrmlib", ".kexi", ".ckp", ".fmpsl", ".spq", ".maw", ".epim", ".itw", ".pnz", ".dbt", ".ade", ".tps", ".idb",
                                              ".nv", ".dsk", ".sas7bdat", ".tmd", ".ecx", ".sbf", ".cat", ".maf", ".rsd", ".mud", ".rbf", ".mdt", ".dcx", ".adb", ".nv2", ".dqy", ".^^^",
                                              ".mas", ".fp5", ".fm5", ".dad", ".jtx", ".dct", ".owc", ".p96", ".ns2", ".erx", ".exb", ".kexis", ".rod", ".adn", ".abx", ".vpd", ".mrg",
                                              ".gwi", ".ns3", ".kexic", ".xdb", ".edb", ".mfd", ".dadiagrams", ".ns4", ".wrk", ".fcd", ".fp4", ".fol" };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Developer
        {
            get
            {
                string[] ext = new string[] { ".adb", ".ads", ".ahk", ".applescript", ".as", ".au3", ".bat", ".bas", ".btm", ".class", ".cljs", ".cmd", ".coffee", ".c", ".cpp", ".cs",
                                              ".ino", ".egg", ".egt", ".erb", ".go", ".hta", ".ibi", ".ici", ".ijs", ".ipynb", ".itcl", ".js", ".jsfl", ".kt", ".lua", ".m", ".mrc",
                                              ".ncf", ".nuc", ".nud", ".nut", ".o", ".pde", ".php", ".ph", ".mm", ".dtd", ".mk", ".bas", ".pl", ".pm", ".ps1", ".ps1xml", ".psc1",
                                              ".psd1", ".psm1", ".py", ".pyc", ".pyo", ".r", ".rb", ".rdp", ".red", ".rs", ".sb2/sb3", ".scpt", ".scptd", ".sdl", ".sh", ".syjs",
                                              ".sypy", ".tcl", ".tns", ".ts", ".vbs", ".xpl", ".ebuild", ".html", ".htm", ".cshtml", ".sql", ".vb", ".swift", ".json", ".java", ".md",
                                              ".yaml", ".yml", ".config", ".ml", ".fs", ".xsd", ".y", ".gs", ".rpy", ".w32", ".nk", ".hh", ".pb", ".xaml", ".cxx", ".appxbundle", ".lgo",
                                              ".ypr", ".sb", ".rbxl", ".cs", ".sb2", ".class", ".appx", ".ipr", ".ino", ".py", ".cpp", ".arsc", ".rbxm", ".c", ".lua", ".pjx", ".in",
                                              ".pyd", ".octest", ".awk", ".nxc", ".smali", ".sc", ".vcproj", ".java", ".vbp", ".md", ".mfa", ".b", ".sh", ".hs", ".cc", ".sln", ".dex",
                                              ".csproj", ".pas", ".cd", ".gitattributes", ".po", ".swc", ".patch", ".config", ".mf", ".ocx", ".tt", ".gmk", ".res", ".resources", ".yml",
                                              ".gs", ".capx", ".res", ".ml", ".proto", ".pbg", ".ise", ".resx", ".fs", ".hpp", ".qpr", ".xsd", ".cod", ".y", ".bbc", ".fbp", ".r",
                                              ".vb", ".gm81", ".swd", ".nib", ".asm", ".def", ".o", ".bluej", ".vdproj", ".so", ".suo", ".targets", ".gmx", ".f", ".agi", ".pwn", ".ccs",
                                              ".entitlements", ".ctp", ".ane", ".erb", ".idb", ".ph", ".ssi", ".pyw", ".bet", ".mm", ".ex", ".s", ".appxupload", ".dtd", ".scc", ".ap_",
                                              ".mk", ".bas", ".pl", ".trx", ".4db", ".xt", ".rc", ".h", ".ymp", ".pbj", ".omo", ".lisp", ".ipr", ".rpy", ".ilk", ".fxml", ".d", ".swift",
                                              ".asc", ".apa", ".au3", ".tk", ".w32", ".bpl", ".vcxproj", ".as3proj", ".markdown", ".myapp", ".nk", ".rul", ".rexx", ".hh", ".ftl", ".fpm",
                                              ".twig", ".l", ".diff", ".cxp", ".hbs", ".pb", ".dox", ".mo", ".abc", ".s19", ".as", ".sc", ".ads", ".csp", ".gsproj", ".pbxuser", ".yaml",
                                              ".jspf", ".am4", ".gm6", ".dcp", ".dbproj", ".vhd", ".w", ".asm", ".rdlc", ".ipch", ".fxc", ".testsettings", ".msix", ".mrt", ".aps", ".asi",
                                              ".livecode", ".ui", ".rb", ".am7", ".sym", ".cp", ".framework", ".vbx", ".pas", ".vm", ".xaml", ".ptl", ".dmd", ".idl", ".cbp", ".xcworkspace",
                                              ".jsfl", ".kdevprj", ".gld", ".plc", ".svn-base", ".v", ".cu", ".wiq", ".as2proj", ".jic", ".t", ".i", ".pika", ".autoplay", ".csx", ".vbg",
                                              ".m", ".iml", ".workspace", ".rsrc", ".alb", ".ism", ".dpr", ".vbproj", ".gem", ".m", ".xpp", ".wdgt", ".pbxproj", ".xcconfig", ".pbxbtree",
                                              ".dsgm", ".ltb", ".xamlx", ".wdl", ".bsc", ".dbml", ".storyboard", ".tlh", ".pbk", ".dproj", ".am6", ".clw", ".sma", ".wdp", ".erl", ".fsscript",
                                              ".pro", ".haml", ".nuspec", ".oca", ".nvv", ".lds", ".for", ".mss", ".wdw", ".bdsproj", ".df1", ".lbs", ".rbc", ".wsc", ".dgml", ".has",
                                              ".iconset", ".sltng", ".xcdatamodeld", ".gitignore", ".cxx", ".v", ".wsp", ".pri", ".m4", ".inc", ".pcp", ".sas", ".dpl", ".bb", ".mak", ".sud",
                                              ".cls", ".lnt", ".mcp", ".cdf", ".pl1", ".nbc", ".vssscc", ".ppc", ".pkgdef", ".ftn", ".gch", ".hal", ".sup", ".uml", ".hxx", ".wxs", ".msha",
                                              ".inl", ".edml", ".v11.suo", ".fsproj", ".ctxt", ".mshc", ".xoml", ".xq", ".xojo_xml_project", ".forth", ".nls", ".exp", ".nsi", ".jpr", ".cp",
                                              ".pyx", ".pl", ".a2w", ".dgsl", ".vdp", ".wxl", ".ist", ".dm1", ".pm", ".asvf", ".pli", ".owl", ".nw", ".ccn", ".vsmacros", ".edmx", ".mer",
                                              ".nsh", ".dob", ".mv", ".resw", ".csi", ".a", ".f90", ".kpl", ".playground", ".hpf", ".src", ".pxd", ".ss", ".pot", ".pri", ".vsz", ".neko",
                                              ".nqc", ".dcuil", ".fxpl", ".refresh", ".mshi", ".ncb", ".wdgtproj", ".xqm", ".jcp", ".testrunconfig", ".lxsproj", ".xql", ".iwb", ".pkgundef",
                                              ".wixmst", ".wixout", ".bbproject", ".wixlib", ".textfactory", ".wixpdb", ".tmlanguage", ".tld", ".lbi", ".wixobj", ".gameproj", ".rodl", ".mdzip",
                                              ".lucidsnippet", ".ned", ".eql", ".p3d", ".src.rpm", ".xcsnapshots", ".pdm", ".ged", ".addin", ".tpu", ".scriptterminology",
                                              ".xojo_binary_project", ".snippet", ".sqlproj", ".wpw", ".xojo_menu", ".dpkw", ".fbz7", ".kts", ".mom", ".licx", ".gmo", ".dec", ".vspscc",
                                              ".ple", ".rbp", ".prg", ".gszip", ".am5", ".bs2", ".vtm", ".fxl", ".iws", ".tur", ".fsproj", ".slogo", ".fgl", ".kdevelop", ".gemspec", ".gs3",
                                              ".rss", ".fsx", ".vbz", ".ccp", ".bcp", ".cob", ".tli", ".cbl", ".xcappdata", ".gorm", ".vspx", ".resjson", ".rise", ".fxcproj", ".vc", ".psc",
                                              ".ent", ".tcl", ".caf", ".mod", ".c", ".vdm", ".tns", ".tds", ".xcarchive", ".brx", ".idt", ".dba", ".licenses", ".lproj", ".inform", ".vgc",
                                              ".rkt", ".4th", ".ctl", ".dsp", ".xcodeproj", ".xib", ".acd", ".pch", ".rnc", ".greenfoot", ".bbprojectd", ".spec", ".r", ".dpk", ".mpr", ".psm1",
                                              ".rbw", ".vsmdi", ".vtv", ".lsproj", ".vsmproj", ".vtml", ".xojo_project", ".prg", ".groupproj", ".bpg", ".sdef", ".gfar", ".rav", ".csn", ".dcproj",
                                              ".pod", ".cvsrc", ".p", ".tmproj", ".wixproj", ".lhs", ".vsp", ".exw", ".odl", ".dfm", ".csi", ".pde", ".msp", ".cfc", ".sbproj", ".nfm", ".scriptsuite",
                                              ".dcu", ".lit", ".ssc", ".deviceids", ".magik", ".dbo", ".wixmsp", ".dbpro", ".dba", ".fsi", ".vcp", ".wxi", ".artproj", ".ccgame", ".jpx", ".groovy",
                                              ".xquery", ".tu", ".gmd", ".r", };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public static string[] Compressed
        {
            get
            {
                string[] ext = new string[] {   ".s00", ".comppkg.hauptwerk.rar", ".ice", ".arduboy", ".mpkg", ".pup", ".rar", ".gz2", ".rte", ".deb", ".vip", ".gzip", ".xapk",
                                                ".tbz", ".pkg.tar.xz", ".lemon", ".sy_", ".sit", ".b6z", ".7z", ".bndl", ".dl_", ".dz", ".pkg", ".bz2", ".r00", ".uha", ".zpi", ".jar.pack", ".fzpz",
                                                ".pbi", ".wa", ".mint", ".gza", ".sqx", ".qda", ".sifz", ".r2", ".rp9", ".dar", ".lzm", ".7z.002", ".smpf", ".hbe", ".pf", ".ecs", ".tar.lzma", ".zip",
                                                ".par", ".b1", ".cbr", ".pak", ".cbz", ".rev", ".tar.xz", ".jsonlz4", ".ita", ".nex", ".kgb", ".taz", ".rpm", ".pit", ".pwa", ".npk", ".ark", ".f",
                                                ".sfg", ".xip", ".pcv", ".spd", ".cdz", ".bh", ".gz", ".a02", ".pea", ".c00", ".tx_", ".piz", ".bundle", ".s7z", ".car", ".7z.001", ".hki", ".f3z",
                                                ".apz", ".lzma", ".tar.lz", ".zix", ".ar", ".cxarchive", ".sfx", ".alz", ".opk", ".ari", ".zl", ".czip", ".z03", ".ctz", ".arc", ".r01", ".bz", "0",
                                                ".tar.bz2", ".ipk", ".dd", ".snb", ".war", ".c01", ".sitx", ".zipx", ".z", ".gmz", ".a01", ".ace", ".xx", ".xz", ".tbz2", ".fdp", ".r03", ".shar",
                                                ".sdn", ".voca", ".lpkg", ".rnc", ".zst", ".s01", ".s02", ".c10", ".ctx", ".oz", ".ufs.uzip", ".ba", ".pa", ".oar", ".gca", ".cb7", ".mbz", ".cbt",
                                                ".p19", ".package", ".tgz", ".sdc", ".sea", ".r30", ".arj", ".pup", ".snappy", ".spt", ".par2", ".rz", ".archiver", ".tar.gz", ".mzp", ".a00", ".sfs",
                                                ".zfsendtotarget", ".xez", ".sh", ".gzi", ".r0", ".lhzd", ".pack.gz", ".edz", ".jhh", ".fp8", ".paq8p", ".yz", "0", ".lz", ".rk", ".z02", ".lbr",
                                                ".zsplit", ".jgz", ".whl", ".xar", ".hyp", ".shr", ".pet", ".j", ".tcx", ".bzip2", ".iadproj", ".lzh", ".srep", ".zi", ".zoo", ".pax", ".z01", ".fzbz",
                                                ".lqr", ".warc", ".paq8f", ".zz", ".ize", ".nz", ".wdz", ".vmcz", ".agg", ".hki1", ".vsi", ".lzo", ".lnx", ".hki3", ".dgc", ".bza", ".efw", ".asr",
                                                ".ipg", ".libzip", ".egg", ".cpt", ".cba", ".z04", ".mzp", ".zw", ".md", ".xmcdz", ".uc2", ".r04", ".tz", ".ain", ".isx", ".arh", ".epi", ".hbc",
                                                ".txz", ".cpgz", ".wastickers", ".lha", ".layout", ".rss", ".zap", ".spm", ".b64", ".tg", ".wux", ".hpk", ".xzm", ".c02", ".vpk", ".sar", ".pae",
                                                ".yz1", ".uzip", ".bzip", ".hpkg", ".jic", ".xef", ".paq6", ".tar.z", ".waff", ".puz", ".zi_", ".bdoc", ".r02", ".mou", ".tlz", ".kz", ".trs",
                                                ".dist", ".sbx", ".r21", ".sen", ".vem", ".tlzma", ".r1", ".cp9", ".hbc2", ".ha", ".psz", ".jex", ".tar.gz2", ".comppkg_hauptwerk_rar", ".stproj",
                                                ".pvmz", ".wlb", ".hki2", ".tzst", ".spl", ".vwi", ".s09", ".prs", ".p7z", ".p01", ".apex", ".paq8l", ".lzx", ".paq7", ".vfs", ".daf", ".ish",
                                                ".boo", ".si", ".paq8", ".y", ".sbx", ".pim", ".wot", ".shk", ".sqz", ".ecsbx", ".gar", };
                string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                return ext.Concat(extNoDot).ToArray();
            }
        }

        public class MicrosoftOffice
        {
            public static string[] Word
            {
                get
                {
                    string[] ext = new string[] { ".doc", ".dot", ".wbk", ".docx", ".docm", ".dotx", ".dotm", ".docb", ".rtf", ".wpd", ".odt" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }

            }

            public static string[] Excel
            {
                get
                {
                    string[] ext = new string[] { ".xls", ".xlt", ".xlm", ".xlsx", ".xlsm", ".xltx", ".xltm", ".xlsb", ".xla", ".xlam", ".xll", ".xlw", ".ods" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }

            }

            public static string[] PowerPoint
            {
                get
                {
                    string[] ext = new string[] { ".ppt", ".pot", ".pps", ".pptx", ".pptm", ".potx", ".potm", ".ppsx", ".ppsm", ".sldx", ".sldm", ".odp" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }

            }

            public static string[] Access
            {
                get
                {
                    string[] ext = new string[] { ".accdb ", ".accde", ".accdt ", ".accdr", };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Visio
            {
                get
                {
                    string[] ext = new string[] { ".vsd ", ".vsdx" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Project
            {
                get
                {
                    string[] ext = new string[] { ".mpp ", ".mpt" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Publisher
            {
                get
                {
                    string[] ext = new string[] { ".pub" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Outlook
            {
                get
                {
                    string[] ext = new string[] { ".msg", ".eml", ".ics", ".vcf" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }

            public static string[] Xps
            {
                get
                {
                    string[] ext = new string[] { ".xps" };
                    string[] extNoDot = ext.Select(x => x.Replace(".", "")).ToArray();

                    return ext.Concat(extNoDot).ToArray();
                }
            }
        }
    }
}
