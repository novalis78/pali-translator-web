using CST.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;

namespace PaliTranslatorWeb
{
   public class TranslateMain
    {
        private Stack<BackStackItem> backStack;
        private IContainer components = null;
        private bool dontLookup;
        private bool dontSelectFirst;
        private List<DictionaryWord> enWords;
        private List<DictionaryWord> hiWords;
        private List<DictionaryWord> lbWords;
        public List<string> wordAnalysisList;
        private int selectedIndex = 0;
        private string txtWord = "";
        public string resultHtml = "";

        public TranslateMain(string searchTerm)
        {
            string searchedWord = "";
            this.backStack = new Stack<BackStackItem>();
            this.DontLookup = true;
            this.lbWords = new List<DictionaryWord>();
            this.wordAnalysisList = new List<string>();
            txtWord = searchTerm;
        }

        

        private void cbDefinitionLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.backStack.Clear();
            if (!this.DontLookup)
            {
                this.LookupWord();
            }
        }


        private int CountCommonStartLetters(string str1, string str2)
        {
            if ((((str1 == null) || (str1.Length == 0)) || (str2 == null)) || (str2.Length == 0))
            {
                return 0;
            }
            int num = (str1.Length < str2.Length) ? str1.Length : str2.Length;
            int num2 = 0;
            num2 = 0;
            while (num2 < num)
            {
                if (str1[num2] != str2[num2])
                {
                    return num2;
                }
                num2++;
            }
            return num2;
        }

        public string DisplayMeaning(string meaning)
        {
            string str = "";
            str = "body { font-family:Tahoma; font-size:9.75pt; border-top:0; }";
            meaning = Regex.Replace(meaning, "<see>(.+?)</see>", "<a onclick=\"window.external.SeeAlso('$1')\" href=\"#\">$1</a>");
            string str2 = "<html><head><style type=\"text/css\">" + str + "</style></head><body><p>" + meaning + "</p>";
            if (this.backStack.Count > 0)
            {
                string str3 = ScriptConverter.Convert(this.backStack.Peek().Word, Script.Ipe, AppState.Inst.CurrentScript);
                string str4 = str2;
                str2 = str4 + "<p>Back to <a onclick=\"window.external.SeeAlsoBack('" + str3 + "')\" href=\"#\">" + str3 + "</a></p>";
            }
            str2 = str2 + "</body></html>";
            return str2;
        }

       

        
        private void LoadEnglishDictionary()
        {
            this.enWords = new List<DictionaryWord>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                string path = HttpContext.Current.Server.MapPath("/");
                FileInfo[] files = new DirectoryInfo(path + "//translate//Reference//" + Config.Inst.EnglishDictionaryDirectory).GetFiles();
                foreach (FileInfo info2 in files)
                {
                    string str;
                    bool flag;
                    StreamReader reader = new StreamReader(info2.FullName);
                    goto Label_0123;
                Label_0068:
                    str = reader.ReadLine();
                    if (str == null)
                    {
                        goto Label_012B;
                    }
                    string str2 = reader.ReadLine();
                    if (str2 == null)
                    {
                        goto Label_012B;
                    }
                    if ((((str != null) && (str.Length != 0)) && (str2 != null)) && (str2.Length != 0))
                    {
                        str = Any2Ipe.Convert(str);
                        if (dictionary.ContainsKey(str))
                        {
                            string str3 = dictionary[str];
                            str3 = str3 + "</p><hr/<p>" + str2;
                            dictionary.Remove(str);
                            dictionary.Add(str, str3);
                        }
                        else
                        {
                            dictionary.Add(str, str2);
                        }
                    }
                Label_0123:
                    flag = true;
                    goto Label_0068;
                Label_012B:
                    reader.Close();
                }
                foreach (string str in dictionary.Keys)
                {
                    this.enWords.Add(new DictionaryWord(str, dictionary[str]));
                }
                this.enWords.Sort(new DictionaryWordComparer());
            }
            catch (Exception exception)
            {
                this.enWords = null;
                Console.WriteLine("Error reading Pali-English dictionary: " + exception.ToString(), "Error");
            }
        }


        private void LoadHindiDictionary()
        {
            this.hiWords = new List<DictionaryWord>();
            try
            {
                string str;
                bool flag;
                string path = HttpContext.Current.Server.MapPath("/");
                StreamReader reader = new StreamReader(path + "//" + Config.Inst.PaliHindiDictionaryFile);
                goto Label_00A0;
            Label_0038:
                str = reader.ReadLine();
                if (str == null)
                {
                    goto Label_00A4;
                }
                string meaning = reader.ReadLine();
                if (meaning == null)
                {
                    goto Label_00A4;
                }
                if ((((str != null) && (str.Length != 0)) && (meaning != null)) && (meaning.Length != 0))
                {
                    str = Any2Ipe.Convert(str);
                    this.hiWords.Add(new DictionaryWord(str, meaning));
                }
            Label_00A0:
                flag = true;
                goto Label_0038;
            Label_00A4:
                this.hiWords.Sort(new DictionaryWordComparer());
                reader.Close();
            }
            catch (Exception)
            {
                this.hiWords = null;
            }
        }

        public void LookupWord()
        {
           // this.btnClose.Enabled = false;
            this.LoadEnglishDictionary();
            this.Search();
            //this.btnClose.Enabled = true;
        }

       

        private void Search()
        {
            List<DictionaryWord> enWords = null;
            enWords = this.enWords;
            this.lbWords.Clear();
            
            if (enWords != null)
            {
                string text = txtWord;
                if ((text == null) || (text.Length == 0))
                {
                    this.DisplayMeaning("");
                }
                else
                {
                    text = Any2Ipe.Convert(text);
                    int num = enWords.BinarySearch(new DictionaryWord(text, ""), new DictionaryWordComparer());
                    if (num < 0)
                    {
                        num = ~num;
                        int num2 = num;
                        DictionaryWord word = null;
                        DictionaryWord word2 = null;
                        int num3 = 0;
                        int num4 = 0;
                        if (((num - 1) >= 0) && ((num - 1) < enWords.Count))
                        {
                            word = enWords[num - 1];
                            num3 = this.CountCommonStartLetters(text, word.Word);
                        }
                        if ((num >= 0) && (num < enWords.Count))
                        {
                            word2 = enWords[num];
                            num4 = this.CountCommonStartLetters(text, word2.Word);
                        }

                        if ((num3 >= num4) && (num3 > 0))
                        {
                            Stack<DictionaryWord> stack = new Stack<DictionaryWord>();
                            num--;
                            DictionaryWord item = null;
                            while ((num >= 0) && (num < enWords.Count))
                            {
                                item = enWords[num];
                                if (this.CountCommonStartLetters(text, item.Word) != num3)
                                {
                                    break;
                                }
                                stack.Push(item);
                                num--;
                            }
                            while (stack.Count > 0)
                            {
                                item = stack.Pop();
                                this.lbWords.Add(item);
                            }
                        }
                        if ((num4 >= num3) && (num4 > 0))
                        {
                            for (num = num2; num < enWords.Count; num++)
                            {
                                if (this.CountCommonStartLetters(enWords[num].Word, text) != num4)
                                {
                                    break;
                                }
                                this.lbWords.Add(enWords[num]);
                            }
                        }
                        if (this.lbWords.Count == 0)
                        {
                            this.DisplayMeaning("");
                        }
                        else if (!((this.lbWords.Count <= 0) || this.DontSelectFirst))
                        {
                            this.selectedIndex = 0;
                        }
                    }
                    else
                    {
                        this.lbWords.Add(enWords[num]);
                        num++;
                        while (num < enWords.Count)
                        {
                            if (!enWords[num].Word.StartsWith(text))
                            {
                                break;
                            }
                            this.lbWords.Add(enWords[num]);
                            num++;
                        }
                        if (!((this.lbWords.Count <= 0) || this.DontSelectFirst))
                        {
                            this.selectedIndex = 0;
                        }
                    }
                }
            }
        }

        public void SeeAlso(string word)
        {
            this.backStack.Push(new BackStackItem(this.txtWord, ((DictionaryWord)this.lbWords[this.selectedIndex]).Word, this.selectedIndex));
            this.txtWord = word;
        }

        public void SeeAlsoBack(string word)
        {
            BackStackItem item = this.backStack.Pop();
            this.txtWord = item.UserText;
            if (item.SelectedIndex < this.lbWords.Count)
            {
                this.selectedIndex = item.SelectedIndex;
            }
        }

       

        public bool DontLookup
        {
            get
            {
                return this.dontLookup;
            }
            set
            {
                this.dontLookup = value;
            }
        }

        public bool DontSelectFirst
        {
            get
            {
                return this.dontSelectFirst;
            }
            set
            {
                this.dontSelectFirst = value;
            }
        }

        public string TranslateText(string tobetranslated)
        {
            this.wordAnalysisList.Clear();
            string inputText = tobetranslated;
            string outputText = "";
            string str2 = "";
            resultHtml = "<head></head><body></body>";

            if (String.IsNullOrEmpty(tobetranslated))
                return "";

            string[] words = inputText.Split(' ');

            foreach (string item in words)
            {
                this.txtWord = item;
                
                DictionaryWord word = null;
                string wordDef = item;
                this.backStack.Clear();
                //test
                this.LookupWord();
                if (this.lbWords == null)
                    outputText += " --- ";
                else
                {
                    if (this.lbWords.Count <= 0)
                    {
                        continue;
                    }
                    word = (DictionaryWord)this.lbWords[0];
                    this.DisplayMeaning(word.Meaning);
                    if (item.EndsWith("ti"))
                        wordDef += " (VB) ";
                    else if (item.EndsWith("o"))
                        wordDef += " (N|NOM) ";
                    else if (item.EndsWith("aṃ"))
                        wordDef += " (N|ACC) ";
                    else if (item.EndsWith("esu"))
                        wordDef += " (N|LOC) ";
                    else if (item.EndsWith("tu"))
                        wordDef += " (VB|IMP) ";

                    this.wordAnalysisList.Add("<b>" + wordDef + "</b>" + " => " + word.Meaning + "<br />");
                    outputText += word.Meaning + " ";
                    str2 += "<head><style>.interlinear_term {margin-top: 1ex; border-left: 1px solid grey; float: left; margin-bottom: 2ex;}</style></head><body></body><div class=\"interlinear_term\"><table><tbody><tr><td><br></td><td><br></td><td><br></td><td><br><b><term>" + item + "</term></b>";
                    str2 += "</td></tr></tbody></table><table><tbody><tr><td colspan=\"4\">" + word.Meaning + "</td></tr></tbody></table></div>";
                }
            }
            //translatedText.Text = outputText;
            resultHtml = str2;
            return resultHtml;
           
        }
    }
}




