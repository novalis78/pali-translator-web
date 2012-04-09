using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PaliTranslatorWeb
{
    public partial class Translated : System.Web.UI.Page
    {
        public string input = "";
        public string resultHtml = "";
        public List<string> wordAnalysisList = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            System.Text.StringBuilder displayValues = new System.Text.StringBuilder();
            System.Collections.Specialized.NameValueCollection
            postedValues = Request.Form;
            string s = Request["s"];

            string input = postedValues["inputs"];
            if (String.IsNullOrEmpty(input))
            {
                input = s;
            }

            TranslateMain tm = new TranslateMain(input);
            resultHtml = tm.TranslateText(input);
            wordAnalysisList = tm.wordAnalysisList;

        }
    }
}
