using DicomGrepCore.Services;
using FellowOakDicom;
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
            var lookupOption = new Option<LookupIn?>(
                name: "--lookup",
                description: "lookup the DICOM Dictionary or UID"
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
            ignoreCaseOption.SetDefaultValue(true);
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


            rootCommand.SetHandler(
                (lookupOptionValue, folderOptionValue, recursiveOptionValue, ignoreCaseOptionValue, tagOptionValue, valueOptionValue) =>
                {
                    if (lookupOptionValue.HasValue)
                    {
                        PrintLookup(lookupOptionValue.Value);
                    }
                },
                lookupOption, folderOption, recursiveOption, ignoreCaseOption, tagOption, valueOption
                );


            return await rootCommand.InvokeAsync(args);
        }

        private static void PrintLookup(LookupIn lookupIn)
        {
            DictionaryService dictionaryService = new DictionaryService();
            dictionaryService.ReadAndAppendCustomDictionaries();
            switch (lookupIn)
            {
                case LookupIn.DICT:
                    //dictionaryService.GetAllDicomTagDefs();
                    //dictionaryService.GetAllPrivateTagDefs();
                    var publicTags = dictionaryService.GetAllDicomTagDefs().OrderBy(entry => entry.Tag.Group).ThenBy(entry => entry.Tag.Element).ToList();
                    var privateTags = dictionaryService.GetAllPrivateTagDefs().OrderBy(entry => entry.Tag.PrivateCreator).ThenBy(entry => entry.Tag.Group).ThenBy(entry => entry.Tag.Element).ToList();
                    Console.WriteLine("Tag|Name|Group|Element|IsRetired|IsPrivate|PrivateCreator");
                    foreach (var tag in publicTags.Concat(privateTags))
                    {
                        Console.WriteLine($"{tag.Tag}|{tag.Name}|{tag.Tag.Group}|{tag.Tag.Element}|{tag.IsRetired}|{tag.Tag.IsPrivate}|{tag.Tag.PrivateCreator}");
                    }
                    Console.WriteLine($"Total {publicTags.Count + privateTags.Count} tags. Public: {publicTags.Count}, Private: {privateTags.Count}");
                    break;
                case LookupIn.UID:
                    var allUids = dictionaryService.GetAllDicomUIDDefs().ToList();
                    Console.WriteLine("UID|Name|Type|IsRetired|IsImageStorage|IsVolumeStorage|StorageCategory");
                    foreach (var uid in allUids)
                    {
                        Console.WriteLine($"{uid.UID}|{uid.Name}|{uid.Type}|{uid.IsRetired}|{uid.IsImageStorage}|{uid.IsVolumeStorage}|{uid.StorageCategory}");
                    }
                    Console.WriteLine($"Total {allUids.Count} UIDs.");
                    break;
                default:
                    break;
            }
        }
    }

    enum LookupIn
    {
        DICT = 0,
        UID
    }
}
