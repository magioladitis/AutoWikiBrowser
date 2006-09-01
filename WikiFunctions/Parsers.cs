﻿/*
WikiFunctions
Copyright (C) 2006 Martin Richards

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections;
using System.Web;

[assembly: CLSCompliant(true)]
namespace WikiFunctions.Parse
{
    /// <summary>
    /// Provides functions for editting wiki text, such as formatting and re-categorisation.
    /// </summary>
    public class Parsers
    {
        #region constructor etc.
        public Parsers()
        {//default constructor
            metaDataSorter = new MetaDataSorter(this);
            MakeRegexes();
        }

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="StubWordCount">The number of maximum number of words for a stub.</param>
        public Parsers(int StubWordCount, bool AddHumanKey)
        {
            metaDataSorter = new MetaDataSorter(this);
            StubMaxWordCount = StubWordCount;
            addCatKey = AddHumanKey;
            MakeRegexes();
        }

        private void MakeRegexes()
        {
            //look bad if changed
            RegexUnicode.Add(new Regex("&(ndash|mdash|minus|times|lt|gt|#160|nbsp|thinsp|shy|[Pp]rime);", RegexOptions.Compiled), "&amp;$1;");
            //IE6 does like these
            RegexUnicode.Add(new Regex("&#(705|803|596|620|699|700|8652|9408|9848);", RegexOptions.Compiled), "&amp;#$1;");
            //Phoenician alphabet
            RegexUnicode.Add(new Regex("&#(x109[0-9A-Z]{2});", RegexOptions.Compiled), "&amp;#$1;");
            //Blackboard bold and more
            RegexUnicode.Add(new Regex("&#((?:277|119|84|x1D|x100)[A-Z0-9a-z]{2,3});", RegexOptions.Compiled), "&amp;#$1;");
            //Cuneiform script
            RegexUnicode.Add(new Regex("&#(x12[A-Za-z0-9]{3});", RegexOptions.Compiled), "&amp;#$1;");
            //interfere with wiki syntax
            RegexUnicode.Add(new Regex("&#(126|x5D|x5B|x7c|0?9[13]|0?12[345]|0?0?3[92]);", RegexOptions.Compiled), "&amp;#$1;");
            //not entity, but still wrong
            RegexUnicode.Add(new Regex("(cm| m|mm|km|mi)<sup>2</sup>", RegexOptions.Compiled), "$1²");
            RegexUnicode.Add(new Regex("(cm| m|mm|km|mi)<sup>3</sup>", RegexOptions.Compiled), "$1³");

            RegexTagger.Add(new Regex("\\{\\{(template:)?(wikify(\\|.*?)?|wfy|wiki)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Wikify-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Clean ?up|CU|Clean|Tidy)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Cleanup-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?Linkless\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Linkless-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Uncategori[sz]ed|Uncat|Classify|Category needed|Catneeded|categori[zs]e|nocats?)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncat-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            RegexConversion.Add(new Regex("\\{\\{(Dab|Disamb|Disambiguation)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Disambig}}");
            RegexConversion.Add(new Regex("\\{\\{(2cc|2LAdisambig|2LCdisambig|2LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{2CC}}");
            RegexConversion.Add(new Regex("\\{\\{(3cc|3LW|Tla-dab|TLA-disambig|TLAdisambig|3LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{3CC}}");
            RegexConversion.Add(new Regex("\\{\\{(4cc|4LW|4LA|4LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{4CC}}");
            RegexConversion.Add(new Regex("\\{\\{(Prettytable|Prettytable100|Pt)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{subst:Prettytable}}");
            RegexConversion.Add(new Regex("\\{\\{(?:[Tt]emplate:)?(PAGENAMEE?\\}\\}|[Ll]ived\\||[Bb]io-cats\\|)", RegexOptions.Compiled), "{{subst:$1");

            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|([0-9]{4})\|([0-9]{4})\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:$1 births|$3]]\r\n[[Category:$2 deaths|$3]]");
            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|\|([0-9]{4})\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:Year of birth unknown|$2]]\r\n[[Category:$1 deaths|$2]]");
            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|([0-9]{4})\|\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:$1 births|$2]]\r\n[[Category:Year of death unknown|$2]]");
        }

        Dictionary<Regex, string> RegexUnicode = new Dictionary<Regex, string>();
        Dictionary<Regex, string> RegexConversion = new Dictionary<Regex, string>();
        Dictionary<Regex, string> RegexTagger = new Dictionary<Regex, string>();

        MetaDataSorter metaDataSorter;
        string testText = "";
        int StubMaxWordCount = 500;

        /// <summary>
        /// Sort interwiki link order
        /// </summary>
        public bool sortInterwikiOrder
        {
            get { return boolInterwikiOrder; }
            set { boolInterwikiOrder = value; }
        }
        private bool boolInterwikiOrder = true;

        /// <summary>
        /// The interwiki link order to use
        /// </summary>
        public InterWikiOrderEnum InterWikiOrder
        {
            set { metaDataSorter.InterWikiOrder = value; }
            get { return metaDataSorter.InterWikiOrder; }
        }

        /// <summary>
        /// When set to true, adds key to categories (for people only) when parsed
        /// </summary>
        public bool addCatKey
        {
            get { return boolAddCatKey; }
            set { boolAddCatKey = value; }
        }
        private bool boolAddCatKey = false;

        #endregion

        #region General Parse

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="sortWikis">True, sort interwiki order per pywiki bots, false keep current order.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string articleText, string articleTitle)
        {
            return metaDataSorter.Sort(articleText, articleTitle);
        }

        readonly Regex regexHeadings0 = new Regex("(== ?)(see also:?|related topics:?|related articles:?|internal links:?|also see:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings1 = new Regex("(== ?)(external links:?|external sites:?|outside links|web ?links:?|exterior links:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings2 = new Regex("(== ?)(external link:?|external site:?|web ?link:?|exterior link:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings3 = new Regex("(== ?)(reference:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings4 = new Regex("(== ?)(source:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings5 = new Regex("(== ?)(further readings?:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        readonly Regex regexHeadings6 = new Regex("(== ?)(Early|Personal|Adult) Life( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings7 = new Regex("(== ?)Early Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = FixHeadings(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string articleText)
        {
            if (!Regex.IsMatch(articleText, "= ?See also ?="))
                articleText = regexHeadings0.Replace(articleText, "$1See also$3");

            articleText = regexHeadings1.Replace(articleText, "$1External links$3");
            articleText = regexHeadings2.Replace(articleText, "$1External link$3");
            articleText = regexHeadings3.Replace(articleText, "$1Reference$3");
            articleText = regexHeadings4.Replace(articleText, "$1Source$3");
            articleText = regexHeadings5.Replace(articleText, "$1Further reading$3");
            articleText = regexHeadings6.Replace(articleText, "$1$2 life$3");
            articleText = regexHeadings7.Replace(articleText, "$1Early career$2");

            return articleText;
        }

        /// <summary>
        /// Applies removes some excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string articleText)
        {
            articleText = Regex.Replace(articleText, "\r\n(\r\n)+", "\r\n\r\n");

            articleText = Regex.Replace(articleText, "== ? ?\r\n\r\n==", "==\r\n==");
            articleText = articleText.Replace("\r\n\r\n(* ?\\[?http)", "\r\n$1");

            articleText = Regex.Replace(articleText.Trim(), "----+$", "");
            articleText = Regex.Replace(articleText.Trim(), "<br ?/?>$", "", RegexOptions.IgnoreCase);

            return articleText.Trim();
        }

        /// <summary>
        /// Applies removes all excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string RemoveAllWhiteSpace(string articleText)
        {//removes all whitespace
            articleText = articleText.Replace("\t", " ");
            articleText = RemoveWhiteSpace(articleText);

            articleText = articleText.Replace("\r\n\r\n*", "\r\n*");

            articleText = Regex.Replace(articleText, "  +", " ");
            articleText = Regex.Replace(articleText, " \r\n", "\r\n");

            articleText = Regex.Replace(articleText, "==\r\n\r\n", "==\r\n");

            //fix bullet points
            articleText = Regex.Replace(articleText, "^([\\*#]+) ", "$1", RegexOptions.Multiline);
            articleText = Regex.Replace(articleText, "^([\\*#]+)", "$1 ", RegexOptions.Multiline);

            //fix heading space
            articleText = Regex.Replace(articleText, "^(={1,4}) ?(.*?) ?(={1,4})$", "$1$2$3", RegexOptions.Multiline);

            //fix dash spacing
            articleText = Regex.Replace(articleText, " ?(–|—|&#15[01];|&[nm]dash;|&#821[12];|&#x201[34];) ?", "$1");
            articleText = Regex.Replace(articleText, "(—|&#151;|&mdash;|&#8212;|&#x2014;|–|&#150;|&ndash;|&#8211;|&#x2013;)", " $1 ");

            return articleText.Trim();
        }

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixSyntax(string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = FixSyntax(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        readonly Regex SyntaxRegex1 = new Regex("\\[\\[http:\\/\\/([^][]*?)\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex2 = new Regex("\\[http:\\/\\/([^][]*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex3 = new Regex("\\[\\[http:\\/\\/(.*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex4 = new Regex("\\[\\[([^][]*?)\\]([^][][^\\]])", RegexOptions.Compiled);
        readonly Regex SyntaxRegex5 = new Regex("([^][])\\[([^][]*?)\\]\\]([^\\]])", RegexOptions.Compiled);

        readonly Regex SyntaxRegex6 = new Regex("\\[?\\[image:(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex7 = new Regex("\\[\\[ (.*)?\\]\\]", RegexOptions.Compiled);
        readonly Regex SyntaxRegex8 = new Regex("\\[\\[([A-Za-z]*) \\]\\]", RegexOptions.Compiled);
        readonly Regex SyntaxRegex9 = new Regex("\\[\\[(.*)?_#(.*)\\]\\]", RegexOptions.Compiled);

        readonly Regex SyntaxRegex10 = new Regex("(\\{\\{[\\s]*)[Tt]emplate:(.*?\\}\\})", RegexOptions.Singleline | RegexOptions.Compiled);
        readonly Regex SyntaxRegex11 = new Regex("^((#|\\*).*?)<br ?/?>\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        readonly Regex SyntaxRegex12 = new Regex("<i>(.*?)</i>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex13 = new Regex("<b>(.*?)</b>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixSyntax(string articleText)
        {
            //replace html with wiki syntax
            if (!Regex.IsMatch(articleText, "'</?[ib]>|</?[ib]>'", RegexOptions.IgnoreCase))
            {
                articleText = SyntaxRegex12.Replace(articleText, "''$1''");
                articleText = SyntaxRegex13.Replace(articleText, "'''$1'''");
            }
            articleText = Regex.Replace(articleText, "^<hr>|^----+", "----", RegexOptions.Multiline);

            //remove appearance of double line break
            articleText = Regex.Replace(articleText, "(^==?[^=]*==?)\r\n(\r\n)?----+", "$1", RegexOptions.Multiline);

            //remove unnecessary namespace
            articleText = SyntaxRegex10.Replace(articleText, "$1$2");

            //remove <br> from lists
            articleText = SyntaxRegex11.Replace(articleText, "$1\r\n");

            //can cause problems
            //articleText = Regex.Replace(articleText, "^<[Hh]2>(.*?)</[Hh]2>", "==$1==", RegexOptions.Multiline);
            //articleText = Regex.Replace(articleText, "^<[Hh]3>(.*?)</[Hh]3>", "===$1===", RegexOptions.Multiline);
            //articleText = Regex.Replace(articleText, "^<[Hh]4>(.*?)</[Hh]4>", "====$1====", RegexOptions.Multiline);

            //fix uneven bracketing on links
            if (!Regex.IsMatch(articleText, "\\[\\[[Ii]mage:[^]]*http"))
            {
                articleText = SyntaxRegex1.Replace(articleText, "[http://$1]");
                articleText = SyntaxRegex2.Replace(articleText, "[http://$1]");
                articleText = SyntaxRegex3.Replace(articleText, "[http://$1]");
                articleText = SyntaxRegex4.Replace(articleText, "[[$1]]$2");
                articleText = SyntaxRegex5.Replace(articleText, "$1[[$2]]$3");
            }

            //repair bad external links
            articleText = SyntaxRegex6.Replace(articleText, "[$1]");

            //repair bad internal links
            articleText = SyntaxRegex7.Replace(articleText, "[[$1]]");
            articleText = SyntaxRegex8.Replace(articleText, "[[$1]]");
            articleText = SyntaxRegex9.Replace(articleText, "[[$1#$2]]");

            articleText = Regex.Replace(articleText, "ISBN: ?([0-9])", "ISBN $1");

            return articleText.Trim();
        }

        /// <summary>
        /// Fixes link syntax
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixLinks(string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = FixLinks(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        readonly Regex RegexLink = new Regex("\\[\\[.*?\\]\\]", RegexOptions.Compiled);

        /// <summary>
        /// Fixes link syntax
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixLinks(string articleText)
        {
            string x = "";
            string y = "";

            string cat = "[[" + Variables.Namespaces[14];

            foreach (Match m in RegexLink.Matches(articleText))
            {
                x = m.Value;
                y = "";

                if (!x.StartsWith("[[Image:") && !x.StartsWith("[[image:") && !x.StartsWith("[[_") && !x.Contains("|_"))
                    y = x.Replace("_", " ");
                else
                    y = x;

                y = y.Replace("+", "%2B");
                y = HttpUtility.UrlDecode(y);

                if (y.StartsWith(cat))
                    y = y.Replace("|]]", "| ]]");
                else
                    y = Regex.Replace(y, " ?\\| ?", "|");

                articleText = articleText.Replace(x, y);
            }

            return articleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = BulletExternalLinks(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string articleText)
        {
            int intStart = 0;
            string articleTextSubstring = "";

            Match m = Regex.Match(articleText, "= ? ?external links? ? ?=", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

            if (!m.Success)
                return articleText;

            intStart = m.Index;

            articleTextSubstring = articleText.Substring(intStart);
            articleText = articleText.Substring(0, intStart);
            articleTextSubstring = Regex.Replace(articleTextSubstring, "(\r\n)?(\r\n)(\\[?http)", "$2* $3");
            articleText += articleTextSubstring;

            return articleText;
        }

        public string FixCategories(string articleText)
        {//Fix common spacing/capitalisation errors in categories

            Regex catregex = new Regex("\\[\\[ ?" + Variables.NamespacesCaseInsensitive[14].Replace(":", " ?:") + " ?(.*?)\\]\\]");
            string cat = "[[" + Variables.Namespaces[14];
            string x = "";

            foreach (Match m in catregex.Matches(articleText))
            {
                x = cat + m.Groups[1].Value.Replace("_", " ") + "]]";
                articleText = articleText.Replace(m.Value, x);
            }

            return articleText;
        }

        #endregion

        #region other functions

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = Unicodify(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string articleText)
        {            
            articleText = Regex.Replace(articleText, "&#150;|&#8211;|&#x2013;", "&ndash;");
            articleText = Regex.Replace(articleText, "&#151;|&#8212;|&#x2014;", "&mdash;");
            articleText = articleText.Replace(" &amp; ", " & ");
            articleText = articleText.Replace("&amp;", "&amp;amp;");

            foreach (KeyValuePair<Regex, string> k in RegexUnicode)
            {
                articleText = k.Key.Replace(articleText, k.Value);
            }
            try
            {
                articleText = HttpUtility.HtmlDecode(articleText);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return articleText;
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the title, if it isnt already
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string articleText, string articleTitle, ref bool NoChange)
        {
            testText = articleText;
            articleText = BoldTitle(articleText, articleTitle);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the title, if it isnt already
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string articleText, string articleTitle)
        {
            //ignore date articles
            if (Regex.IsMatch(articleTitle, "^(January|February|March|April|May|June|July|August|September|October|November|December) [0-9]{1,2}$"))
                return articleText;

            string escTitle = Regex.Escape(articleTitle);

            //remove self links first
            Regex tregex = new Regex("\\[\\[(" + Tools.caseInsensitive(escTitle) + ")\\]\\]");
            if (!articleText.Contains("'''"))
            {
                articleText = tregex.Replace(articleText, "'''$1'''", 1);
            }
            else
            {
                articleText = articleText.Replace("[[" + articleTitle + "]]", articleTitle);
                articleText = articleText.Replace("[[" + Tools.TurnFirstToLower(articleTitle) + "]]", Tools.TurnFirstToLower(articleTitle));
            }

            escTitle = Regex.Replace(articleTitle, " \\(.*?\\)$", "");
            escTitle = Regex.Escape(escTitle);

            if (Regex.IsMatch(articleText, "^(\\[\\[|\\{|\\*|:)") || Regex.IsMatch(articleText, "''' ?" + escTitle + " ?'''", RegexOptions.IgnoreCase))
                return articleText;

            Regex regexBold = new Regex("([^\\[]|^)(" + escTitle + ")([ ,.:;])", RegexOptions.IgnoreCase);

            string strSecondHalf = "";
            if (articleText.Length > 80)
            {
                strSecondHalf = articleText.Substring(80);
                articleText = articleText.Substring(0, 80);
            }

            if (articleText.Contains("'''"))
                return articleText + strSecondHalf;

            articleText = regexBold.Replace(articleText, "$1'''$2'''$3", 1);

            return articleText + strSecondHalf;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string ReImager(string OldImage, string NewImage, string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = ReplaceImage(OldImage, NewImage, articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <returns>The new article text.</returns>
        public string ReplaceImage(string OldImage, string NewImage, string articleText)
        {
            //remove image prefix
            OldImage = Regex.Replace(OldImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            NewImage = Regex.Replace(NewImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");

            OldImage = Regex.Escape(OldImage).Replace("\\ ", "[ _]");

            OldImage = Variables.NamespacesCaseInsensitive[6] + Tools.caseInsensitive(OldImage);
            NewImage = Variables.Namespaces[6] + NewImage;

            articleText = Regex.Replace(articleText, OldImage, NewImage);

            return articleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="Image">The image to remove.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string Image, string articleText, bool CommentOut, string Comment)
        {
            //remove image prefix
            Image = Regex.Replace(Image, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            Image = Regex.Escape(Image).Replace("\\ ", "[ _]");
            Image = Tools.caseInsensitive(Image);

            Regex r = new Regex("\\[\\[" + Variables.NamespacesCaseInsensitive[6] + Image + ".*\\]\\]");
            MatchCollection n = r.Matches(articleText);

            if (n.Count > 0)
            {
                foreach (Match m in n)
                {
                    string match = m.Value;

                    int i = 0;
                    int j = 0;

                    foreach (char c in match)
                    {
                        if (c == '[')
                            j++;
                        else if (c == ']')
                            j--;

                        i++;

                        if (j == 0)
                        {
                            if (match.Length > i)
                                match = match.Remove(i);

                            Regex t = new Regex(Regex.Escape(match));

                            if (CommentOut)
                                articleText = t.Replace(articleText, "<!-- " + Comment + " " + match + " -->", 1, m.Index);
                            else
                                articleText = t.Replace(articleText, "", 1);

                            break;
                        }

                    }
                }
            }
            else
            {
                r = new Regex("(" + Variables.NamespacesCaseInsensitive[6] + ")?" + Image);
                n = r.Matches(articleText);

                foreach (Match m in n)
                {
                    Regex t = new Regex(Regex.Escape(m.Value));

                    if (CommentOut)
                        articleText = t.Replace(articleText, "<!-- " + Comment + " $0 -->", 1, m.Index);
                    else
                        articleText = t.Replace(articleText, "", 1, m.Index);
                }
            }

            return articleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldImage">The image to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string Image, string articleText, bool CommentOut, string Comment, ref bool NoChange)
        {
            testText = articleText;
            articleText = RemoveImage(Image, articleText, CommentOut, Comment);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string articleText, string articleTitle, ref bool NoChange)
        {
            testText = articleText;
            articleText = AddCategory(NewCategory, articleText, articleTitle);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string articleText, string articleTitle)
        {
            if (Regex.IsMatch(articleText, "\\[\\[ ?[Cc]ategory ?: ?" + Regex.Escape(NewCategory)))
                return articleText;

            string cat = "\r\n[[" + Variables.Namespaces[14] + NewCategory + "]]";
            cat = Tools.ApplyKeyWords(articleTitle, cat);

            if (articleTitle.StartsWith(Variables.Namespaces[10]))
                articleText += "<noinclude>" + cat + "\r\n</noinclude>";
            else
                articleText += cat;

            return articleText;
        }

        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldCategory">The old category to replace.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The re-categorised article text.</returns>
        public string ReCategoriser(string OldCategory, string NewCategory, string articleText, ref bool NoChange)
        {
            //remove category prefix
            OldCategory = Regex.Replace(OldCategory, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);
            NewCategory = Regex.Replace(NewCategory, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);

            //format categories properly
            articleText = FixCategories(articleText);

            testText = articleText;

            if (Regex.IsMatch(articleText, "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + Tools.caseInsensitive(NewCategory)))
            {
                articleText = RemoveCategory(OldCategory, articleText);
            }
            else
            {
                OldCategory = Variables.Namespaces[14] + OldCategory + "( ?\\|| ?\\]\\])";
                NewCategory = Variables.Namespaces[14] + NewCategory + "$1";

                articleText = Regex.Replace(articleText, OldCategory, NewCategory, RegexOptions.IgnoreCase);
            }

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The article text without the old category.</returns>
        public string RemoveCategory(string strOldCat, string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = RemoveCategory(strOldCat, articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public string RemoveCategory(string strOldCat, string articleText)
        {
            //format categories properly
            articleText = FixCategories(articleText);

            strOldCat = Tools.caseInsensitive(strOldCat);

            strOldCat = "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + " ?" + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])(\r\n)?";
            articleText = Regex.Replace(articleText, strOldCat, "");

            return articleText;
        }

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = LinkSimplifier(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        readonly Regex LinkSimplierRegex = new Regex("\\[\\[([^[]*?)\\|([^[]*?)\\]\\]", RegexOptions.Compiled);

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string articleText)
        {
            string n = "";
            string a = "";
            string b = "";
            string k = "";

            foreach (Match m in LinkSimplierRegex.Matches(articleText))
            {
                n = m.Value;
                a = m.Groups[1].Value;
                b = m.Groups[2].Value;

                if (a == b || Tools.TurnFirstToLower(a) == b)
                {
                    k = LinkSimplierRegex.Replace(n, "[[$2]]");
                    articleText = articleText.Replace(n, k);
                }
                else if (a + "s" == b || Tools.TurnFirstToLower(a) + "s" == b)
                {
                    k = LinkSimplierRegex.Replace(n, "$2");
                    k = "[[" + k.Substring(0, k.Length - 1) + "]]s";
                    articleText = articleText.Replace(n, k);
                }
            }

            return articleText;
        }

        public string LivingPeople(string articleText, ref bool NoChange)
        {
            NoChange = true;
            testText = articleText;

            if (Regex.IsMatch(articleText, "\\[\\[ ?Category ?:[ _]?([0-9]{1,2}[ _]century[ _]deaths|[0-9s]{4,5}[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _]missing|Possibly[ _]living[ _]people)", RegexOptions.IgnoreCase))
                return articleText;

            Match m = Regex.Match(articleText, "\\[\\[ ?Category ?:[ _]?([0-9]{4})[ _]births(\\|.*?)?\\]\\]", RegexOptions.IgnoreCase);

            if (!m.Success)
                return articleText;

            string birthCat = m.Value;
            int birthYear = int.Parse(m.Groups[1].Value);
            string catKey = "";

            if (birthYear < 1910)
                return articleText;

            if (birthCat.Contains("|"))
                catKey = Regex.Match(birthCat, "\\|.*?\\]\\]").Value;
            else
                catKey = "]]";

            articleText += "[[Category:Living people" + catKey;

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string articleText, ref bool NoChange)
        {
            testText = articleText;
            articleText = Conversions(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string articleText)
        {
            foreach (KeyValuePair<Regex, string> k in RegexConversion)
            {
                articleText = k.Key.Replace(articleText, k.Value);
            }

            return articleText;
        }

        /// <summary>
        /// Removes unnecessary introductory headers 
        /// </summary>
        public string RemoveBadHeaders(string articleText, string articleTitle, ref bool NoChange)
        {
            testText = articleText;
            articleText = RemoveBadHeaders(articleText, articleTitle);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        Regex RegexBadHeader = new Regex("^(={1,4} ?(about|description|overview|definition|general information|background|intro|introduction|summary|bio|biography) ?={1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// Removes unnecessary introductory headers 
        /// </summary>
        public string RemoveBadHeaders(string articleText, string articleTitle)
        {
            articleText = Regex.Replace(articleText, "^={1,4} ?" + Regex.Escape(articleTitle) + " ?={1,4}", "", RegexOptions.IgnoreCase);
            articleText = RegexBadHeader.Replace(articleText, "");

            return articleText.Trim();
        }

        /// <summary>
        /// Subst'd some user talk templates
        /// </summary>
        /// <param name="TalPageText">The wiki text of the talk page.</param>
        /// <returns>The new text.</returns>
        public string SubstUserTemplates(string TalkPageText)
        {
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(test[n0-6]?[ab]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(test[n0-6]?[ab]?-n\\|.*?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(3RR[0-5]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(spam[0-5][ab]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(spam[0-5]?-n\\|.*?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(welcome[0-6]|welcomeip|anon|welcome-anon)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            return TalkPageText;
        }

        readonly Regex RegexStub = new Regex("\\{\\{.*?[Ss]tub\\}\\}", RegexOptions.Compiled);

        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        public string Tagger(string articleText, string articleTitle, ref bool NoChange, ref string Summary)
        {
            testText = articleText;
            articleText = Tagger(articleText, articleTitle, ref Summary);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// adds/removes
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitlet">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public string Tagger(string articleText, string articleTitle, ref string Summary)
        {
            if (Tools.IsRedirect(articleText))
                return articleText;

            double Length = articleText.Length + 1;
            int words = Tools.WordCount(articleText);
            double LinkCount = 1;
            double Ratio = 0;

            //update by-date tags
            foreach (KeyValuePair<Regex, string> k in RegexTagger)
            {
                articleText = k.Key.Replace(articleText, k.Value);
            }

            //remove stub tags from long articles
            if (words > StubMaxWordCount && RegexStub.IsMatch(articleText))
            {
                MatchEvaluator stubEvaluator = new MatchEvaluator(stubChecker);
                articleText = RegexStub.Replace(articleText, stubEvaluator);

                return articleText.Trim();
            }            

            if (Regex.IsMatch(articleText, @"\{\{.*?\}\}"))
            {
                return articleText;
            }

            LinkCount = Tools.LinkCount(articleText);

            Ratio = LinkCount / Length;

            if (LinkCount < 4 && (Ratio < 0.0025))
            {
                articleText = "{{Wikify-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + articleText;
                Summary += " and added wikify tag";

                return articleText;
            }
            else if (articleText.Length <= 300 && !RegexStub.IsMatch(articleText))
            {
                articleText = articleText + "\r\n\r\n\r\n{{stub}}";
                Summary += " and added stub tag";

                return articleText;
            }
            else if(words > 8 && !Regex.IsMatch(articleText, @"\[\[ ?category", RegexOptions.IgnoreCase))
            {
                articleText += "\r\n\r\n{{Uncat-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                Summary += " and added uncategorised tag";

                return articleText;
            }

            return articleText;
        }

        private string stubChecker(Match m)
        {// Replace each Regex cc match with the number of the occurrence.
            if (Regex.IsMatch(m.Value, "\\{\\{[Ss]ect"))
                return m.Value;
            else
                return "";
        }

        #endregion

        #region unused

        /// <summary>
        /// Fixes minor problems, such as abbreviations and miscapitalisations
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public string MinorThings(string articleText)
        {
            articleText = Regex.Replace(articleText, "[Aa]\\.[Kk]\\.[Aa]\\.?", "also known as");

            articleText = articleText.Replace("e.g.", "for example");
            articleText = articleText.Replace("i.e.", "that is");

            MatchCollection ma = Regex.Matches(articleText, "(monday|tuesday|wednesday|thursday|friday|saturday|sunday|january|february|april|june|july|august|september|october|november|december)");
            if (ma.Count > 0)
            {
                foreach (Match m in ma)
                    articleText = articleText.Replace(m.Groups[1].Value, Tools.TurnFirstToUpper(m.Groups[1].Value));
            }

            return articleText;
        }

        //[http://en.wikipedia.org/wiki/Dog] to [[Dog]]
        //private string ExtToInternalLinks(string articleText)
        //{
        //    foreach (Match m in Regex.Matches(articleText, "\\[http://en\\.wikipedia\\.org/wiki/.*?\\]"))
        //    {
        //        string a = HttpUtility.UrlDecode(m.ToString());

        //        if (a.Contains(" "))
        //        {
        //            int intP;
        //            //string a = n;
        //            intP = a.IndexOf(" ");

        //            string b = a.Substring(intP);
        //            a = a.Remove(intP);
        //            b = b.TrimStart();
        //            a = a.Replace("_", " ");

        //            articleText = articleText.Replace(m.ToString(), a);
        //        }
        //    }

        //    articleText = Regex.Replace(articleText, "\\[http://en\\.wikipedia\\.org/wiki/(.*?)\\]", "[[$1]]");
        //    return articleText;
        //}

        #endregion
    }
}
