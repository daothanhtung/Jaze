﻿using System;
using System.Collections.Generic;
using System.Linq;
using Jaze.Domain;
using Jaze.Domain.Entities;

namespace Jaze.Search
{
    internal class HanVietSearcher
    {
        public static IEnumerable<HanViet> Search(SearchArgs searchArgs)
        {
            var key = searchArgs.SearchKey;
            
            if (string.IsNullOrWhiteSpace(key))
            {
                //return GetAll();
                return null;
            }

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

        private static IEnumerable<HanViet> SearchContain(string key)
        {
            var context = JazeDatabaseContext.Context;
            return context.HanViets.Where(hv => hv.Word.Contains(key) || hv.Reading.Contains(key)).ToArray();
        }

        private static IEnumerable<HanViet> SearchEndWith(string key)
        {
            var context = JazeDatabaseContext.Context;
            return context.HanViets.Where(hv => hv.Word.EndsWith(key) || hv.Reading.EndsWith(key)).ToArray();
        }

        private static IEnumerable<HanViet> SearchStartWith(string key)
        {
            var context = JazeDatabaseContext.Context;
            return context.HanViets.Where(hv => hv.Word.StartsWith(key) || hv.Reading.StartsWith(key)).ToArray();
        }

        private static IEnumerable<HanViet> SearchExact(string key)
        {
            var context = JazeDatabaseContext.Context;
            //at stat
            var keyStart = key + ", ";
            //at middle
            var keyMiddle = ", " + key + ",";
            //at end
            var keyEnd = ", " + key;
            return context.HanViets.Where(hv => hv.Word == key || hv.Reading == key || hv.Reading.StartsWith(keyStart) || hv.Reading.Contains(keyMiddle) || hv.Reading.EndsWith(keyEnd)).ToArray();
        }

        private static IEnumerable<HanViet> GetAll()
        {
            var context = JazeDatabaseContext.Context;
            return context.HanViets.ToArray();
        }
    }
}
