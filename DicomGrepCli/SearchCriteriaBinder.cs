using DicomGrepCore.Entities;
using DicomGrepCore.Enums;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrepCli
{
    internal class SearchCriteriaBinder : BinderBase<SearchCriteria>
    {

        private readonly Option<string> _folderOption;
        private readonly Option<string> _fileTypeOption;
        private readonly Option<bool> _recursiveOption;
        private readonly Option<string> _sopClassOption;
        private readonly Option<string> _tagOption;
        private readonly Option<string> _valueOption;
        private readonly Option<bool> _caseSensitiveOption;
        private readonly Option<bool> _wholeWordOption;
        private readonly Option<int> _threadsOption;
        private readonly Option<MatchPatternEnum> _patternOption;


        public SearchCriteriaBinder(Option<string> folderOption, Option<string> fileTypeOption,
            Option<bool> recursiveOption, Option<string> sopClassOption, Option<string> tagOption,
            Option<string> valueOption, Option<bool> caseSensitiveOption, Option<bool> wholeWordOption,
            Option<int> threadsOption, Option<MatchPatternEnum> patternOption)
        {
            _folderOption = folderOption;
            _fileTypeOption = fileTypeOption;
            _recursiveOption = recursiveOption;
            _sopClassOption = sopClassOption;
            _tagOption = tagOption;
            _valueOption = valueOption;
            _caseSensitiveOption = caseSensitiveOption;
            _wholeWordOption = wholeWordOption;
            _threadsOption = threadsOption;
            _patternOption = patternOption;
        }

        protected override SearchCriteria GetBoundValue(BindingContext bindingContext)
        {
            return new SearchCriteria
            {
                SearchPath = bindingContext.ParseResult.GetValueForOption(_folderOption),
                FileTypes = bindingContext.ParseResult.GetValueForOption(_fileTypeOption),
                IncludeSubfolders = bindingContext.ParseResult.GetValueForOption(_recursiveOption),
                SearchSopClassUid = bindingContext.ParseResult.GetValueForOption(_sopClassOption),
                SearchTag = bindingContext.ParseResult.GetValueForOption(_tagOption),
                SearchText = bindingContext.ParseResult.GetValueForOption(_valueOption),
                CaseSensitive = bindingContext.ParseResult.GetValueForOption(_caseSensitiveOption),
                WholeWord = bindingContext.ParseResult.GetValueForOption(_wholeWordOption),
                SearchThreads = bindingContext.ParseResult.GetValueForOption(_threadsOption),
                MatchPattern = bindingContext.ParseResult.GetValueForOption(_patternOption)
            };
        }
    }
}
