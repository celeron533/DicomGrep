using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.RegularExpressions;

namespace DicomGrepCli
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("DicomGrep Command Line Interface");


            #region lookup options
            // once see the lookup options, do not execute any search
            var lookupOption = new Option<LookupIn>(
                name: "--lookup",
                description: "lookup the Dictionary or SOPClass UID"
                );
            lookupOption.AddAlias("-l");
            rootCommand.AddOption(lookupOption);
            #endregion

            #region search options
            var folderOption = new Option<string>(
                name: "--dir",
                description: "search in this directory"
                );
            rootCommand.AddOption(folderOption);

            //-r, --recursive. As well as GREP
            var recursiveOption = new Option<bool>(
                name: "--recursive",
                description: "search in subdirectories"
                );
            recursiveOption.AddAlias("-r");
            rootCommand.AddOption(recursiveOption);

            //-i, --ignore-case. As well as GREP
            var ignoreCaseOption = new Option<bool>(
                name: "--ignore-case",
                description: "ignore case"
                );
            ignoreCaseOption.AddAlias("-i");
            rootCommand.AddOption(ignoreCaseOption);

            //-F, --fixed-strings. As well as GREP
            //-e PATTERNS, --regexp = PATTERNS. As well as GREP
            //-c, --count. As well as GREP

            //-t, --tag. DICOM Tag
            var tagOption = new Option<string>(
                name: "--tag",
                description: "DICOM Tag"
                );
            tagOption.AddAlias("-t");
            rootCommand.AddOption(tagOption);

            //-v, --value. DICOM Tag Value
            var valueOption = new Option<string>(
                name: "--value",
                description: "DICOM Tag Value"
                );
            valueOption.AddAlias("-v");
            rootCommand.AddOption(valueOption);


            #endregion search options


            return await rootCommand.InvokeAsync(args);
        }


    }

    enum LookupIn
    {
        DICT = 0,
        SOP
    }
}
