using DicomGrepCore.Enums;
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
            var lookupOption = new Option<LookupType?>(
                name: "--lookup",
                description: "[Lookup] lookup the DICOM Dictionary or UID"
                );
            lookupOption.AddAlias("-l");
            rootCommand.AddOption(lookupOption);
            #endregion

            #region search options
            var folderOption = new Option<string>(
                name: "--dir",
                description: "search in directory"
                );
            folderOption.AddAlias("-d");
            rootCommand.AddOption(folderOption);

            var fileTypeOption = new Option<string>(
                name: "--file-type",
                description: "search in file types"
                );
            fileTypeOption.SetDefaultValue("*.dcm");
            fileTypeOption.AddAlias("-f");
            rootCommand.AddOption(fileTypeOption);

            //-r, --recursive. As well as GREP
            var recursiveOption = new Option<bool>(
                name: "--recursive",
                description: "search in subdirectories"
                );
            recursiveOption.AddAlias("-r");
            rootCommand.AddOption(recursiveOption);

            var sopClassOption = new Option<string>(
                name: "--sop",
                description: "search by SOP Class UID"
                );
            rootCommand.AddOption(sopClassOption);

            //-t, --tag. DICOM Tag
            var tagOption = new Option<string>(
                name: "--tag",
                description: "search by DICOM Tag"
                );
            tagOption.AddAlias("-t");
            rootCommand.AddOption(tagOption);

            //-v, --value. DICOM Tag Value
            var valueOption = new Option<string>(
                name: "--value",
                description: "search by DICOM Tag Value"
                );
            valueOption.AddAlias("-v");
            rootCommand.AddOption(valueOption);

            var caseSensitiveOption = new Option<bool>(
                name: "--case-sensitive",
                description: "case sensitive (DICOM Tag)"
                );
            caseSensitiveOption.AddAlias("-c");
            rootCommand.AddOption(caseSensitiveOption);

            var wholdWordOption = new Option<bool>(
                name: "--whole-word",
                description: "whole word (DICOM Tag)"
                );
            wholdWordOption.AddAlias("-w");
            rootCommand.AddOption(wholdWordOption);

            var threadsOption = new Option<int>(
                name: "--threads",
                description: "number of threads"
                );
            threadsOption.SetDefaultValue(0);
            rootCommand.AddOption(threadsOption);

            var patternOption = new Option<MatchPatternEnum>(
                name: "--pattern",
                description: "plain text or regular expression pattern (DICOM Tag)"
                );
            patternOption.SetDefaultValue(MatchPatternEnum.Normal);
            rootCommand.AddOption(patternOption);


            #endregion search options


            rootCommand.SetHandler(
                (lookupOptionValue, folderOptionValue, recursiveOptionValue, ignoreCaseOptionValue, tagOptionValue, valueOptionValue) =>
                {
                    if (lookupOptionValue.HasValue)
                    {
                        PrintLookup(lookupOptionValue.Value);
                    }
                },
                lookupOption, folderOption, recursiveOption, caseSensitiveOption, tagOption, valueOption
                );


            return await rootCommand.InvokeAsync(args);
        }

        private static void PrintLookup(LookupType lookupType)
        {
            DictionaryService dictionaryService = new DictionaryService();
            dictionaryService.ReadAndAppendCustomDictionaries();
            switch (lookupType)
            {
                case LookupType.DICT:
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
                case LookupType.UID:
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

    enum LookupType
    {
        DICT = 0,
        UID
    }
}
