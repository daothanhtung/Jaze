﻿using System;
using System.Collections.Generic;
using System.Linq;
using Jaze.Domain;
using Jaze.Domain.Entities;
using Jaze.Util;

namespace Jaze.Search
{
    static class KanjiSearcher
    {
        public static IEnumerable<Kanji> Search(SearchArgs searchArgs)
        {
            var key = searchArgs.SearchKey;
            //
            if (string.IsNullOrWhiteSpace(key))
            {
                return GetAll();
            }
            //if search key contain multi kanji
            var arr = StringUtil.FilterCharsInString(key, CharSet.Kanji);
            if (arr.Count>0)
            {
                return LoadKanji(arr);
            }
            //if search key is vietnamese sentence
            if (key.Contains(" "))
            {
                return SearchVietNameseSentence(key);
            }
            //other case
            switch (searchArgs.Option)
            {
                case SearchOption.Exact:
                    return SearchExact(key);
                case SearchOption.StartWith:
                    return SearchStartWith(key);
                case SearchOption.EndWith:
                    return SearchEndWith(key);
                case SearchOption.Contain:
                    return SearchContain(key);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<Kanji> SearchVietNameseSentence(string key)
        {
            var arr = key.Split(' ');
            var context = JazeDatabaseContext.Context;
            var list = new List<Kanji>();
            foreach (var s in arr)
            {
                var kanjis = context.Kanjis.Where(k => k.HanViet == s);
                foreach (var kanji in kanjis)
                {
                    if (!list.Contains(kanji))
                    {
                        list.Add(kanji);
                    }
                }
            }
            return list;
        }

        private static IEnumerable<Kanji> LoadKanji(IList<char> arr)
        {
            var context = JazeDatabaseContext.Context;
            return arr.Select(c => "" + c).Select(s => context.Kanjis.FirstOrDefault(kanji => kanji.Word == s)).ToList();
        }

        private static IEnumerable<Kanji> SearchContain(string key)
        {
            var context = JazeDatabaseContext.Context;
            return context.Kanjis.Where(kanji => kanji.HanViet.Contains(key)).ToArray();
        }

        private static IEnumerable<Kanji> SearchEndWith(string key)
        {
            var context = JazeDatabaseContext.Context;
            return context.Kanjis.Where(kanji => kanji.HanViet.EndsWith(key)).ToArray();
        }

        private static IEnumerable<Kanji> SearchStartWith(string key)
        {
            var context = JazeDatabaseContext.Context;
            return context.Kanjis.Where(kanji => kanji.HanViet.StartsWith(key)).ToArray();
        }

        private static IEnumerable<Kanji> SearchExact(string key)
        {
            var context = JazeDatabaseContext.Context;
            //at stat
            var keyStart = key + ",";
            //at middle
            var keyMiddle = "," + key + ",";
            //at end
            var keyEnd = "," + key;
            return context.Kanjis.Where(kanji => kanji.HanViet == key || kanji.HanViet.StartsWith(keyStart) || kanji.HanViet.Contains(keyMiddle) || kanji.HanViet.EndsWith(keyEnd)).ToArray();
        }

        private static IEnumerable<Kanji> GetAll()
        {
            var context = JazeDatabaseContext.Context;
            return context.Kanjis.ToArray();
        }
    }
}
