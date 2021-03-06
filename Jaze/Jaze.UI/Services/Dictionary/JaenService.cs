﻿using System.Collections.Generic;
using System.Linq;
using Jaze.Domain;
using Jaze.UI.Models;
using Jaze.UI.Util;
using Newtonsoft.Json;

namespace Jaze.UI.Services
{
    public class JaenService : ServiceBase<JaenModel>
    {
        //public static IEnumerable<JaEn> Search(SearchArgs searchArgs)
        //{
        //    var key = searchArgs.SearchKey;

        //    if (string.IsNullOrWhiteSpace(key))
        //    {
        //        //return GetAll();
        //        return null;
        //    }

        //    key = key.Contains("-") ? StringUtil.ConvertRomaji2Katakana(key) : StringUtil.ConvertRomaji2Hiragana(key);

        //    switch (searchArgs.Option)
        //    {
        //        case SearchOption.Exact:
        //            return SearchExact(key);
        //        case SearchOption.StartWith:
        //            return SearchStartWith(key);
        //        case SearchOption.EndWith:
        //            return SearchEndWith(key);
        //        case SearchOption.Contain:
        //            return SearchContain(key);
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        //private static IEnumerable<JaEn> SearchContain(string key)
        //{
        //    var context = JazeDatabaseContext.Context;
        //    return context.JaEns.Where(o => o.Word.Contains(key) || o.Kana.Contains(key)).ToArray();
        //}

        //private static IEnumerable<JaEn> SearchEndWith(string key)
        //{
        //    var context = JazeDatabaseContext.Context;
        //    return context.JaEns.Where(o => o.Word.EndsWith(key) || o.Kana.EndsWith(key)).ToArray();
        //}

        //private static IEnumerable<JaEn> SearchStartWith(string key)
        //{
        //    var context = JazeDatabaseContext.Context;
        //    return context.JaEns.Where(o => o.Word.StartsWith(key) || o.Kana.StartsWith(key)).ToArray();
        //}

        //private static IEnumerable<JaEn> SearchExact(string key)
        //{
        //    var context = JazeDatabaseContext.Context;
        //    //at stat
        //    var keyStart = key + " ";
        //    //at middle
        //    var keyMiddle = " " + key + " ";
        //    //at end
        //    var keyEnd = " " + key;
        //    return context.JaEns.Where(o => o.Word == key || o.Kana == key || o.Kana.StartsWith(keyStart) || o.Kana.Contains(keyMiddle) || o.Kana.EndsWith(keyEnd)).ToArray();
        //}

        //private static IEnumerable<JaEn> GetAll()
        //{
        //    var context = JazeDatabaseContext.Context;
        //    return context.JaEns.ToArray();
        //}
        public override JaenModel Get(int id)
        {
            using (var db = new JazeDatabaseContext())
            {
                var entity = db.JaEns.Find(id);
                if (entity != null)
                {
                    return JaenModel.Create(entity);
                }

                return null;
            }
        }

        public override List<JaenModel> Search(SearchArgs searchArgs)
        {
            var key = searchArgs.SearchKey;

            if (string.IsNullOrWhiteSpace(key))
            {
                return new List<JaenModel>();
            }
            key = key.Contains("-") ? StringUtil.ConvertRomaji2Katakana(key) : StringUtil.ConvertRomaji2Hiragana(key);
            searchArgs.SearchKey = key;
            return base.Search(searchArgs);
        }

        public override List<JaenModel> SearchExact(string key)
        {
            using (var db = new JazeDatabaseContext())
            {
                //at stat
                var keyStart = key + " ";
                //at middle
                var keyMiddle = " " + key + " ";
                //at end
                var keyEnd = " " + key;
                return db.JaEns.Where(o => o.Word == key || o.Kana == key || o.Kana.StartsWith(keyStart) || o.Kana.Contains(keyMiddle) || o.Kana.EndsWith(keyEnd))
                    .ToList().Select(entity => JaenModel.Create(entity)).ToList();
            }
        }

        public override List<JaenModel> SearchStartWith(string key)
        {
            using (var db = new JazeDatabaseContext())
            {
                return db.JaEns.Where(o => o.Word.EndsWith(key) || o.Kana.EndsWith(key)).ToList().Select(entity => JaenModel.Create(entity)).ToList();
            }
        }

        public override List<JaenModel> SearchEndWith(string key)
        {
            using (var db = new JazeDatabaseContext())
            {
                return db.JaEns.Where(o => o.Word.EndsWith(key) || o.Kana.EndsWith(key)).ToList().Select(entity => JaenModel.Create(entity)).ToList();
            }
        }

        public override List<JaenModel> SearchContain(string key)
        {
            using (var db = new JazeDatabaseContext())
            {
                return db.JaEns.Where(o => o.Word.Contains(key) || o.Kana.Contains(key)).ToList().Select(entity => JaenModel.Create(entity)).ToList();
            }
        }

        public override List<JaenModel> GetAll()
        {
            using (var db = new JazeDatabaseContext())
            {
                return db.JaEns.ToList().Select(entity => JaenModel.Create(entity)).ToList();
            }
        }

        public override void LoadFull(JaenModel model)
        {
            if (!model.IsLoadFull)
            {
                using (var db = new JazeDatabaseContext())
                {
                    model.Means = JsonConvert.DeserializeObject<List<WordMean>>(model.MeanText);
                    foreach (var mean in model.Means)
                    {
                        if (mean.ExampleIds != null)
                        {
                            mean.Examples = new List<ExampleModel>();
                            foreach (var exampleId in mean.ExampleIds)
                            {
                                var example = db.JaEnExamples.Find(exampleId);
                                if (example != null)
                                {
                                    mean.Examples.Add(ExampleModel.Create(example));
                                }
                            }
                        }
                    }
                    model.IsLoadFull = true;
                }
            }
        }
    }
}