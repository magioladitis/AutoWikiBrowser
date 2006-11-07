using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace WikiFunctions.AWBSettings
{

    //mother class
    [Serializable, XmlRoot("AutoWikiBrowserPreferences")]
    public class UserPrefs
    {
        public UserPrefs() { }

        [XmlAttribute]
        public string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public ProjectEnum Project = ProjectEnum.wikipedia;
        public LangCodeEnum LanguageCode = LangCodeEnum.en;
        public string CustomProject = "";

        public ListPrefs List = new ListPrefs();
        public FaRPrefs FindAndReplace = new FaRPrefs();
        public EditPrefs Editprefs = new EditPrefs();
        public GeneralPrefs General = new GeneralPrefs();
        public SkipPrefs Skipoptions = new SkipPrefs();
        public ModulePrefs Module = new ModulePrefs();

        public List<PluginPrefs> Plugin = new List<PluginPrefs>();
    }

    //find and replace prefs
    [Serializable]
    public class FaRPrefs
    {
        public bool Enabled = false;
        public bool IgnoreSomeText = false;
        public bool AppendSummary = true;
        public List<WikiFunctions.Parse.Replacement> Replacements = new List<WikiFunctions.Parse.Replacement>();

        public List<WikiFunctions.MWB.IRule> AdvancedReps = new List<WikiFunctions.MWB.IRule>();
    }

    [Serializable]
    public class ListPrefs
    {
        public string ListSource = "";
        public WikiFunctions.Lists.SourceType Source = WikiFunctions.Lists.SourceType.Category;
        public List<Article> ArticleList = new List<Article>();
    }

    //the basic settings
    [Serializable]
    public class EditPrefs
    {
        public bool GeneralFixes = true;
        public bool Tagger = true;
        public bool Unicodify = true;

        public int Recategorisation = 0;
        public string NewCategory = "";

        public int ReImage = 0;
        public string ImageFind = "";
        public string Replace = "";

        public bool AppendText = false;
        public bool Append = true;
        public string Text = "";

        public int AutoDelay = 10;
        public bool QuickSave = false;
        public bool SuppressTag = false;

        public bool RegexTypoFix = false;
    }

    //skip options
    [Serializable]
    public class SkipPrefs
    {
        public bool SkipNonexistent = true;
        public bool SkipWhenNoChanges = false;

        public bool SkipDoes = false;
        public bool SkipDoesNot = false;

        public string SkipDoesText = "";
        public string SkipDoesNotText = "";

        public bool Regex = false;
        public bool CaseSensitive = false;

        public bool SkipNoFindAndReplace = false;
        public bool SkipNoRegexTypoFix = false;

        public string GeneralSkip = "";
    }

    [Serializable]
    public class GeneralPrefs
    {
        public string SelectedSummary = "Clean up";
        public List<string> Summaries = new List<string>();

        public string[] PasteMore = new string[10] { "", "", "", "", "", "", "", "", "", "" };

        public string FindText = "";
        public bool FindRegex = false;
        public bool FindCaseSensitive = false;

        public bool WordWrap = true;
        public bool ToolBarEnabled = false;
        public bool BypassRedirect = true;
        public bool NoAutoChanges = false;
        public bool Preview = false;
        public bool Minor = false;
        public bool Watch = false;
        public bool TimerEnabled = false;
        public bool SortInterwikiOrder = true;
        public bool AddIgnoredToLog = false;

        public bool EnhancedDiff = true;
        public bool ScrollDown = true;
        public int DiffFontSize = 150;
        public int TextBoxSize = 10;
        public string TextBoxFont = "Courier New";
        public bool LowThreadPriority = false;
        public bool FlashAndBeep = true;
    }

    [Serializable]
    public class ModulePrefs
    {
        public bool Enabled = false;
        public int Language = 0;
        public string Code = "";
    }

    [Serializable]
    public class PluginPrefs
    {
        public string Name = "";
        public object[] PluginSettings = null;
    }
}
