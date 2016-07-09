﻿using Jaze.Model;

namespace Jaze.Search
{
    public class SearchArg
    {
        public SearchArg(string searchKey, SearchOption option)
        {
            SearchKey = searchKey;
            Option = option;
        }

        public SearchArg(string searchKey, SearchOption option, DictionaryType dictionary)
        {
            SearchKey = searchKey;
            Option = option;
            Dictionary = dictionary;
        }

        public string SearchKey { get; set; }
        public SearchOption Option { get; set; }
        public DictionaryType Dictionary { get; set; }
    }

    public enum SearchOption
    {
        Exact,
        StartWith,
        EndWith,
        Contain
    }
}
