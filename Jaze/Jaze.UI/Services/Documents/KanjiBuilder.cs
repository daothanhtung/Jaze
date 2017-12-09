﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Jaze.UI.Models;
using Jaze.UI.Services.URI;

namespace Jaze.UI.Services.Documents
{
    public class KanjiBuilder : BuilderBase<KanjiModel>
    {
        private IUriService _uriService;

        public KanjiBuilder(IUriService uriService)
        {
            _uriService = uriService;
        }

        #region ----- Build Full Infomation -----

        public override FlowDocument Build(KanjiModel kanji)
        {
            if (kanji == null)
            {
                return null;
            }

            //start build
            FlowDocument document = new FlowDocument()
            {
                PagePadding = new Thickness(10)
            };
            document.Blocks.Add(BuildGeneralInfo(kanji));
            document.Blocks.Add(BuildWords(kanji.JaviModels));
            return document;
        }

        private Block BuildGeneralInfo(KanjiModel kanji)
        {
            Table table = new Table() { CellSpacing = 0 };

            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups[0].Rows.Add(new TableRow()
            {
                Cells =
                {
                    new TableCell(BuildAttributeSet(kanji)),
                    new TableCell(BuildKanjiStroke(kanji.Word, kanji.HanViet))
                }
            });

            table.RowGroups[0].Rows.Add(new TableRow()
            {
                Cells =
                {
                    new TableCell(BuildViMean(kanji.VieMeaning)),
                    new TableCell(BuildEnMean(kanji.EngMeaning))
                }
            });
            return table;
        }

        private Block BuildWords(List<JaviModel> javiModels)
        {
            if (javiModels == null || javiModels.Count == 0)
            {
                return new Section();
            }
            Table table = new Table() { CellSpacing = 0 };
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });
            table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn { Width = GridLength.Auto });
            table.RowGroups.Add(new TableRowGroup());

            foreach (var javiModel in javiModels)
            {
                table.RowGroups[0].Rows.Add(new TableRow()
                {
                    Cells =
                    {
                        new TableCell(new Paragraph(new Run(javiModel.Word))),
                        new TableCell(new Paragraph(new Run(javiModel.Kana))),
                        new TableCell(BuildJaViMean(javiModel.Means))
                    }
                });
            }
            foreach (var row in table.RowGroups[0].Rows)
            {
                foreach (var cell in row.Cells)
                {
                    cell.BorderThickness = new Thickness(1);
                    cell.BorderBrush = Brushes.Black;
                    cell.Padding = new Thickness(1.5);
                }
            }
            return table;
        }

        private Block BuildJaViMean(List<WordMean> means)
        {
            if (means == null || means.Count == 0)
            {
                return new Section();
            }
            var paragraph = new Paragraph();
            foreach (var mean in means)
            {
                paragraph.Inlines.Add(new Run("- " + mean.Mean));
                paragraph.Inlines.Add(new LineBreak());
            }
            return paragraph;
        }

        private Block BuildAttributeSet(KanjiModel kanji)
        {
            List list = new List()
            {
                MarkerStyle = TextMarkerStyle.Box
            };

            //add list attribute
            list.ListItems.Add(BuildAttribute("Bộ thủ: ", $"{kanji.Radical.Word}({kanji.Radical.HanViet})"));
            list.ListItems.Add(BuildAttribute("Cách viết khác: ", kanji.Variant));
            list.ListItems.Add(BuildAttribute("Onyomi: ", kanji.Onyomi));
            list.ListItems.Add(BuildAttribute("Kunyomi: ", kanji.Kunyomi));
            //list.ListItems.Add(BuildKunAttribute(kanji.Kunyomi));
            list.ListItems.Add(BuildAttribute("Số nét: ", "" + kanji.Stroke));
            list.ListItems.Add(BuildAttribute("Độ phổ biến: ",
                kanji.Frequence == int.MaxValue ? "?/2500" : "" + kanji.Frequence + "/2500"));
            list.ListItems.Add(BuildAttribute("Trình độ: ", kanji.Level.ToString()));
            list.ListItems.Add(BuildAttribute("Thành phần: ", kanji.Component));
            list.ListItems.Add(BuildAttribute("Gần giống: ", kanji.Similar));
            list.ListItems.Add(BuildAttributeParts("Cấu tạo: ", kanji.Parts));
            return list;
        }

        private ListItem BuildAttributeParts(string name, List<PartModel> parts)
        {
            ListItem item = new ListItem();
            var partsStr = string.Join("", parts.Select(part => part.Word).ToArray());
            item.Blocks.Add(new Paragraph()
            {
                Inlines =
                {
                    new Run(name)
                    {
                        Foreground = Brushes.Gray,
                        FontSize = 14
                    },
                    new Hyperlink(new Run(partsStr))
                    {
                        NavigateUri = _uriService.Create(Definitions.UriAction.ShowParts, partsStr)
                    }
                }
            });

            return item;
        }

        private ListItem BuildAttribute(string name, string contain)
        {
            ListItem item = new ListItem();
            item.Blocks.Add(new Paragraph()
            {
                Inlines =
                {
                    new Run(name)
                    {
                        Foreground = Brushes.Gray,
                        FontSize = 14
                    },
                    new Run(contain)
                }
            });

            return item;
        }

        //private ListItem BuildKunAttribute(string contain)
        //{
        //    ListItem item = new ListItem();
        //    var paragragh = new Paragraph()
        //    {
        //        Inlines =
        //        {
        //            new Run("Kunyomi: ")
        //            {
        //                Foreground = Brushes.Gray,
        //                FontSize = 14
        //            }
        //        }
        //    };

        //    //split contain
        //    if (!string.IsNullOrWhiteSpace(contain))
        //    {
        //        var kuns = Regex.Split(contain, "、 ");
        //        foreach (var s in kuns)
        //        {
        //            var arr = s.Split('.');
        //            if (arr.Length == 2)
        //            {
        //                paragragh.Inlines.Add(new Run(arr[0])
        //                {
        //                    Foreground = Brushes.Blue,
        //                    FontSize = 15,
        //                    FontWeight = FontWeights.Bold
        //                });
        //                paragragh.Inlines.Add(new Run(arr[1])
        //                {
        //                    FontSize = 15
        //                });
        //            }
        //            else
        //            {
        //                paragragh.Inlines.Add(new Run(arr[0])
        //                {
        //                    FontSize = 15
        //                });
        //            }
        //            paragragh.Inlines.Add(new Run("、 ")
        //            {
        //                FontSize = 15
        //            });
        //        }
        //    }

        //    item.Blocks.Add(paragragh);
        //    return item;
        //}

        private Block BuildKanjiStroke(string kanji, string hanviet)
        {
            Paragraph paragraph = new Paragraph()
            {
                TextAlignment = TextAlignment.Center,
                Inlines = { new Run(kanji)
                {
                    FontSize = 150,
                    FontFamily = new FontFamily("KanjiStrokeOrders")
                } ,
                new LineBreak(),
                new Run(hanviet)
                }
            };
            return paragraph;
        }

        private Block BuildViMean(string viMean)
        {
            if (string.IsNullOrWhiteSpace(viMean))
            {
                return new Section();
            }
            Section section = new Section();
            //title
            section.Blocks.Add(new Paragraph(new Run("Nghĩa tiếng Việt")));
            section.Blocks.Add(BuildListViMean(viMean));
            return section;
        }

        public static Block BuildListViMean(string viMean)
        {
            if (string.IsNullOrWhiteSpace(viMean))
            {
                return new Section();
            }
            List list = new List()
            {
                MarkerStyle = TextMarkerStyle.Square,
                MarkerOffset = 5,
                Margin = new Thickness(0),
                Padding = new Thickness(20, 0, 0, 0)
            };

            //build list hv + mean
            var dic = AnalyseVimean(viMean);
            foreach (var key in dic.Keys)
            {
                ListItem item = new ListItem();
                item.Blocks.Add(new Paragraph(new Run(key)));

                //add list mean for each key
                List subList = new List();

                foreach (var mean in dic[key])
                {
                    subList.ListItems.Add(new ListItem()
                    {
                        Blocks = { new Paragraph(new Run(mean)) }
                    });
                }
                item.Blocks.Add(subList);
                list.ListItems.Add(item);
            }
            return list;
        }

        private static Dictionary<string, List<string>> AnalyseVimean(string vimean)
        {
            var result = new Dictionary<string, List<string>>();
            var matches = Regex.Matches(vimean, "{(.*?)}");

            foreach (Match match in matches)
            {
                var s = match.Groups[1].Value;
                //TODO get Han Viet + list mean
                //var m = Regex.Match(s, "^\\((.*(?)\\)(.*)$");
                //string hv = m.Groups[1].Value;
                //string means = m.Groups[2].Value;
                int pos = s.IndexOf(")");
                string hv = s.Substring(1, pos - 1);
                string means = s.Substring(pos + 1);
                var arr = Regex.Split(means, "##").ToList();
                result.Add(hv, arr);
            }
            return result;
        }

        private Block BuildEnMean(string enMean)
        {
            if (string.IsNullOrWhiteSpace(enMean))
            {
                return null;
            }
            Section section = new Section();
            section.Blocks.Add(new Paragraph(new Run("English meaning")));
            List list = new List();
            ////
            foreach (var item in Regex.Split(enMean, "##"))
            {
                list.ListItems.Add(new ListItem(new Paragraph(new Run(item))));
            }
            ///////
            section.Blocks.Add(list);
            return section;
        }

        #endregion ----- Build Full Infomation -----

        #region Build Quick View

        public override FlowDocument BuildLite(KanjiModel model)
        {
            return Build(model);
        }

        #endregion Build Quick View
    }
}