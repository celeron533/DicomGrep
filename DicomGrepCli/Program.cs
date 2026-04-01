using DicomGrepCore.Entities;
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
            Option<LookupType?> lookupOption = new("--lookup", "-l")
            {
                Description = "[Lookup] lookup the DICOM Dictionary or UID",
            };

            rootCommand.Options.Add(lookupOption);
            #endregion

            #region search options
            Option<string> folderOption = new Option<string>("--dir", "-d")
            {
                Description = "search in directory",
                DefaultValueFactory = folder => Environment.CurrentDirectory
            };
            rootCommand.Options.Add(folderOption);

            Option<string> fileTypeOption = new Option<string>("--file-type", "-f")
            {
                Description = "search in file types",
                DefaultValueFactory = fileType => "*.dcm"
            };
            rootCommand.Options.Add(fileTypeOption);

            //-r, --recursive. As well as GREP
            Option<bool> recursiveOption = new Option<bool>("--recursive", "-r")
            {
                Description = "search in subdirectories"
            };
            rootCommand.Options.Add(recursiveOption);

            Option<string> sopClassOption = new Option<string>("--sop")
            {
                Description = "search by SOP Class UID",
                DefaultValueFactory = sop => string.Empty
            };
            rootCommand.Options.Add(sopClassOption);

            //-t, --tag. DICOM Tag
            Option<string> tagOption = new Option<string>("--tag", "-t")
            {
                Description = "search by DICOM Tag",
                DefaultValueFactory = tag => string.Empty
            };
            rootCommand.Options.Add(tagOption);

            //-v, --value. DICOM Tag Value
            Option<string> valueOption = new Option<string>("--value", "-v")
            {
                Description = "search by DICOM Tag Value",
                DefaultValueFactory = value => string.Empty
            };
            rootCommand.Options.Add(valueOption);

            Option<bool> caseSensitiveOption = new Option<bool>("--case-sensitive", "-c")
            {
                Description = "case sensitive (DICOM Tag)"
            };
            rootCommand.Options.Add(caseSensitiveOption);

            Option<bool> wholeWordOption = new Option<bool>("--whole-word", "-w")
            {
                Description = "whole word (DICOM Tag)"
            };
            rootCommand.Options.Add(wholeWordOption);

            Option<int> threadsOption = new Option<int>("--threads", "-t")
            {
                Description = "number of threads",
                DefaultValueFactory = threads => 0
            };
            rootCommand.Options.Add(threadsOption);

            Option<MatchPatternEnum> patternOption = new("--pattern")
            { 
                Description = "plain text or regular expression pattern (DICOM Tag)",
                DefaultValueFactory = pattern => MatchPatternEnum.Normal
            };
            rootCommand.Options.Add(patternOption);


            #endregion search options

            rootCommand.SetAction(parseResult=>
            {
                Console.Out.WriteLine("DicomGrep CLI - DicomGrep Command Line Interface");

                DoRootCommand(
                    parseResult.GetValue(lookupOption),
                    new SearchCriteria
                    {
                        SearchPath = parseResult.GetRequiredValue(folderOption),
                        FileTypes = parseResult.GetRequiredValue(fileTypeOption),
                        IncludeSubfolders = parseResult.GetValue(recursiveOption),
                        SearchSopClassUid = parseResult.GetRequiredValue(sopClassOption),
                        SearchTag = parseResult.GetRequiredValue(tagOption),
                        SearchText = parseResult.GetRequiredValue(valueOption),
                        CaseSensitive = parseResult.GetValue(caseSensitiveOption),
                        WholeWord = parseResult.GetValue(wholeWordOption),
                        SearchThreads = parseResult.GetValue(threadsOption),
                        MatchPattern = parseResult.GetValue(patternOption)
                    });
            });




            return await rootCommand.Parse(args).InvokeAsync();
        }

        private static void DoRootCommand(LookupType? lookupOptionValue, SearchCriteria criteria)
        {
            if (lookupOptionValue.HasValue)
            {
                PrintLookup(lookupOptionValue.Value);
            }
            else
            {
                Console.WriteLine(criteria.ToString("n1"));
                DoSearch(criteria);
            }
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

        private static void DoSearch(SearchCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.SearchPath))
            {
                Console.WriteLine("Please specify the directory.");
                return;
            }

            var tokenSource = new CancellationTokenSource();//dummy
            SearchService searchService = new SearchService();
            searchService.FileListCompleted += (sender, e) =>
            {
                Console.WriteLine($"> Search in {e.Count} files...");
            };
            searchService.OnCompletDicomFile += (sender, e) =>
            {
                if (e.IsMatch)
                {
                    Console.WriteLine(e.Filename);
                }
            };
            searchService.OnSearchComplete += (sender, e) =>
            {
                Console.WriteLine($"> Search completed.");
            };


            searchService.Search(criteria, tokenSource);
        }
    }

    enum LookupType
    {
        DICT = 0,
        UID
    }
}
