using AudiobookOrganiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiobookOrganiser.Helpers
{
    internal class LibraryPathHelper
    {
        public static string DetermineLibraryPath(MetaData metaData)
        {
            if (string.IsNullOrWhiteSpace(metaData.Genre))
                return Program.LibraryRootPaths.Where(l => l == "Fiction").FirstOrDefault();

            if (metaData.Genre.StringContainsIn(
                "comedy",
                "stand-up",
                "stand up",
                "humour"))

            {
                return Program.LibraryRootPaths.Where(l => l.StringContainsIn("Comedy")).FirstOrDefault();
            }

            else if (metaData.Genre.StringContainsIn(
                "fiction",
                "mystery",
                "triller",
                "suspense",
                "crime",
                "romance",
                "classics",
                "fantasy"))
            {
                return Program.LibraryRootPaths.Where(l => l.Contains("\\Fiction")).FirstOrDefault();
            }

            return Program.LibraryRootPaths.Where(l => l.StringContainsIn("Non-Fiction")).FirstOrDefault();
        }
    }
}
