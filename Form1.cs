using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Baixar_Videos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> listId = new List<string>();
            var Videos = YMDCore.GetViedosFromList("https://www.youtube.com/playlist?list=PLxGs0X3jGqmBJblKjFCMzz9N8kivOaThH&disable_polymer=true"); // <- URL da lista de reprodução
            
            foreach (KeyValuePair<string, string> item in Videos)
            {
                string Vid_ID = item.Key;
                string Vid_Title = item.Value;
                listId.Add(Vid_ID);
                this.checkedListBox1.Items.Add(Vid_Title);                
            }
            this.Text = this.checkedListBox1.Items.Count.ToString();
        }
    }
    public static class YMDCore
    {
        static public Dictionary<string, string> GetViedosFromList(string Url)
        {
            var RetDic = new Dictionary<string, string>();

            string RetStr = HTTPGet(String.Format("https://api.youtubemultidownloader.com/playlist?url={0}&nextPageToken=", Url));

            JObject JO = JObject.Parse(RetStr);
            JObject Videos = JObject.Parse(JO["items"].ToString());

            foreach (var v in Videos.Children())
            {
                string Video_ID = v.First["id"].ToString();
                string Video_Title = v.First["title"].ToString();
                RetDic.Add(Video_ID, Video_Title);
            }

            return RetDic;
        }

        static public string GetVideoUrl(string VideoID)
        {
            string RetStr = HTTPGet(String.Format("https://api.youtubemultidownloader.com/video?id={0}", VideoID));
            JObject JO = JObject.Parse(RetStr);

            return JO["result"]["22"].ToString();
        }

        static private string HTTPGet(string Url)
        {
            HttpWebRequest wb = (HttpWebRequest)WebRequest.Create(Url);
            wb.Method = "GET";
            wb.KeepAlive = true;
            wb.Proxy = null;
            wb.Referer = "https://youtubemultidownloader.com/playlist.html";
            wb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";
            return new StreamReader(wb.GetResponse().GetResponseStream()).ReadToEnd();
        }
    }
}