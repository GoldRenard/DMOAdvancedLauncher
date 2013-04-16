// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using DMOLibrary;

namespace AdvancedLauncher
{
    public partial class NewsBlock : UserControl
    {
        NewsBlock_DC DContext;
        BackgroundWorker bw_load_news;

        private bool isLoading_ = false;

        private TwitterViewModel TwitterVM = new TwitterViewModel();
        private List<TwitterItemViewModel> twitter_statuses = new List<TwitterItemViewModel>();
        private delegate void DoAddTwit(List<UserStatus> status_list, BitmapImage bmp);
        private delegate void DoLoadTwitter(List<TwitterItemViewModel> statuses);

        private JoymaxViewModel JoymaxVM = new JoymaxViewModel();
        private List<JoymaxItemViewModel> joymax_news = new List<JoymaxItemViewModel>();
        private delegate void DoAddJoyNews(List<NewsItem> news);
        private delegate void DoLoadJoymax(List<JoymaxItemViewModel> news);

        public delegate void ChangedEventHandler(object sender, int tab_num);
        public event ChangedEventHandler TabChanged;
        protected virtual void OnChanged(int tab_num)
        {
            if (TabChanged != null)
                TabChanged(this, tab_num);
        }

        public string _username;
        public string twitter_username
        {
            get { return _username; }
            set
            {
                if (value != _username)
                {
                    _username = value;
                    TwitterVM.UnLoadData();
                }
            }

        }

        public NewsBlock()
        {
            InitializeComponent();
            DContext = new NewsBlock_DC();
            LayoutRoot.DataContext = DContext;
            Twitter_News.DataContext = TwitterVM;
            Joymax_News.DataContext = JoymaxVM;
        }

        public void ShowTab(int tab, bool isFast)
        {
            if (isLoading_)
                return;

            OnChanged(tab);

            //этот флаг нужен для того, чтобы не вызывать анимации загрузки, если ничего не было загружено
            bool isDownloaded = false;

            bw_load_news = new BackgroundWorker();
            bw_load_news.DoWork += (s1, e1) =>
            {
                if (tab == 1)
                {
                    if (!TwitterVM.IsDataLoaded)
                    {
                        isLoading(true);
                        isDownloaded = true;
                        GetTwitter_News(_username);
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoLoadTwitter((list) =>
                        {
                            TwitterVM.LoadData(list);
                        }), twitter_statuses);
                    }
                }
                else
                {
                    if (!JoymaxVM.IsDataLoaded)
                    {
                        isLoading(true);
                        isDownloaded = true;
                        GetJoymax_News();
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoLoadJoymax((list) =>
                        {
                            JoymaxVM.LoadData(list);
                        }), joymax_news);
                    }
                }
            };

            bw_load_news.RunWorkerCompleted += (s1, e1) =>
            {
                //Скрываем старую панель
                Storyboard sb = new Storyboard();
                DoubleAnimation dbl_anim = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(isFast ? 10 : 150)));
                Storyboard.SetTarget(dbl_anim, (tab == 1) ? Joymax_News : Twitter_News);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Completed += (s, e) =>
                {
                    if (isDownloaded)
                        isLoading(false);
                    if (tab == 1)
                    {
                        Twitter_News.Visibility = Visibility.Visible;
                        Joymax_News.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Joymax_News.Visibility = Visibility.Visible;
                        Twitter_News.Visibility = Visibility.Collapsed;
                    }
                    sb = new Storyboard();
                    dbl_anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(150)));
                    Storyboard.SetTarget(dbl_anim, (tab == 1) ? Twitter_News : Joymax_News);
                    Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                    sb.Children.Add(dbl_anim);
                    sb.Begin();
                };
                sb.Begin();
            };
            bw_load_news.RunWorkerAsync();
        }

        #region Twitter statuses

        private Regex URLRegex = new Regex("((https?|s?ftp|ssh)\\:\\/\\/[^\"\\s\\<\\>]*[^.,;'\">\\:\\s\\<\\>\\)\\]\\!])", RegexOptions.Compiled);
        private Regex HashRegex = new Regex(@"(\B#\w*[a-zA-Z]+\w*)", RegexOptions.Compiled);
        private Regex TUserRegex = new Regex(@"(\B@\w*[a-zA-Z]+\w*)", RegexOptions.Compiled);
        private static string LINK_REPL = "%DMOAL:LINK%", HASHTAG_REPL = "%DMOAL:HASHTAG%", USER_REPL = "%DMOAL:TWUSER%";

        public struct UserStatus
        {
            public string UserName;
            public string UserScreenName;
            public string ProfileImageUrl;
            public string RetweetImageUrl;
            public string Status;
            public string StatusId;
            public BitmapImage ProfileImageBitmap;
            public DateTime StatusDate;
        }

        public struct ProfileImage
        {
            public string url;
            public BitmapImage bitmap;
        }

        enum TwitterTextType
        {
            Text,
            Link,
            HashTag,
            UserName
        }

        struct TwitterTextPart
        {
            public TwitterTextType Type;
            public string Data;
        }

        //получение статусов
        public void GetTwitter_News(string username)
        {
            WebClient wc = new WebClient();
            wc.Proxy = (IWebProxy)null;
            Uri link = new Uri(string.Format("http://api.twitter.com/1/statuses/user_timeline.xml?screen_name={0}&include_rts=true", username));
            twitter_statuses.Clear();
            string result;
            try
            {
                result = wc.DownloadString(link);
            }
            catch (Exception e)
            {
                twitter_statuses.Add(new TwitterItemViewModel { Title = LanguageProvider.strings.NEWS_CANT_GET_TWITLIST + ": " + e.Message + " [ERRCODE 3]" });
                return;
            }

            if (result.IndexOf("errors") == -1)
            {
                XDocument document;
                try { document = XDocument.Parse(result, LoadOptions.None); }
                catch
                {
                    twitter_statuses.Add(new TwitterItemViewModel { Title = LanguageProvider.strings.NEWS_CANT_GET_TWITLIST + " [ERRCODE 1]" });
                    return;
                }

                List<UserStatus> statuses = new List<UserStatus>();
                List<ProfileImage> prof_images = new List<ProfileImage>();
                List<ProfileImage> cur_image;
                ProfileImage p_image;
                foreach (XElement XStatus in document.Root.Descendants("status"))
                {
                    UserStatus status = new UserStatus();
                    status.UserName = XStatus.Element("user").Element("name").Value;
                    status.UserScreenName = XStatus.Element("user").Element("screen_name").Value;
                    status.ProfileImageUrl = XStatus.Element("user").Element("profile_image_url").Value;
                    try { status.RetweetImageUrl = XStatus.Element("retweeted_status").Element("user").Element("profile_image_url").Value; }
                    catch { };
                    status.Status = XStatus.Element("text").Value;
                    status.StatusId = XStatus.Element("id").Value;
                    status.StatusDate = ParseDateTime(XStatus.Element("created_at").Value);

                    string profile_image;
                    if (status.RetweetImageUrl != null)
                        profile_image = status.RetweetImageUrl;
                    else
                        profile_image = status.ProfileImageUrl;

                    cur_image = prof_images.FindAll(i => (i.url == profile_image));
                    if (cur_image.Count == 0)
                    {
                        p_image = new ProfileImage();
                        p_image.url = profile_image;
                        p_image.bitmap = GetImage(profile_image);
                      if (p_image.bitmap != null)
                        prof_images.Add(p_image);
                    }
                    else
                        p_image = cur_image[0];

                    status.ProfileImageBitmap = p_image.bitmap;

                    statuses.Add(status);
                }

                foreach (UserStatus status in statuses)
                    twitter_statuses.Add(new TwitterItemViewModel { Title = status.Status, Date = status.StatusDate.ToLongDateString() + " " + status.StatusDate.ToShortTimeString(), Image = status.ProfileImageBitmap });
            }
            else
            {
                twitter_statuses.Add(new TwitterItemViewModel { Title = LanguageProvider.strings.NEWS_CANT_GET_TWITLIST + " [ERRCODE 2]" });
                return;
            }
        }

        public BitmapImage GetImage(string url)
        {
            WebClient wc = new WebClient();
            wc.Proxy = (IWebProxy)null;

            Uri uri = new Uri(url);
            byte[] image_bytes;
            try { image_bytes = wc.DownloadData(uri); }
            catch { return null; }

            MemoryStream img_stream = new MemoryStream(image_bytes, 0, image_bytes.Length);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = img_stream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        //парсинг строки времени
        public static DateTime ParseDateTime(string date)
        {
            string dayOfWeek = date.Substring(0, 3).Trim();
            string month = date.Substring(4, 3).Trim();
            string dayInMonth = date.Substring(8, 2).Trim();
            string time = date.Substring(11, 9).Trim();
            string offset = date.Substring(20, 5).Trim();
            string year = date.Substring(25, 5).Trim();
            string dateTime = string.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);
            DateTime ret = DateTime.Parse(dateTime);
            return ret;
        }

        //подмена всех ссылок контролом гипер-ссылки
        private void TextBlock_Parse(object sender, RoutedEventArgs e)
        {
            TextBlock tb = ((TextBlock)sender);

            if (tb.Text.Contains(LINK_REPL) || tb.Text.Contains(HASHTAG_REPL))
                return;

            string FormatString = URLRegex.Replace(tb.Text, LINK_REPL);
            FormatString = HashRegex.Replace(FormatString, HASHTAG_REPL);
            FormatString = TUserRegex.Replace(FormatString, USER_REPL);

            //Collecting links, hashtags and users in lists
            int LinkCounter = 0;
            List<string> links = new List<string>();
            foreach (Match match in URLRegex.Matches(tb.Text))
                links.Add(match.Groups[0].Value);

            int HashCounter = 0;
            List<string> tags = new List<string>();
            foreach (Match match in HashRegex.Matches(tb.Text))
                tags.Add(match.Groups[0].Value);

            int UserCounter = 0;
            List<string> users = new List<string>();
            foreach (Match match in TUserRegex.Matches(tb.Text))
                users.Add(match.Groups[0].Value);

            //Creating list of splitted text by links
            List<TwitterTextPart> Parts = GetTwitterParts(ref links, ref LinkCounter, FormatString, LINK_REPL, TwitterTextType.Link, false);

            //Creating list of splitted text (from older list) by hashtags
            List<TwitterTextPart> TempParts = new List<TwitterTextPart>();
            foreach (TwitterTextPart part in Parts)
            {
                if (part.Type == TwitterTextType.Text)
                {
                    List<TwitterTextPart> HashParts = GetTwitterParts(ref tags, ref HashCounter, part.Data, HASHTAG_REPL, TwitterTextType.HashTag, false);
                    TempParts.AddRange(HashParts);
                }
                else
                    TempParts.Add(part);
            }
            Parts = TempParts;

            //Creating list of splitted text (from older list) by usernames
            TempParts = new List<TwitterTextPart>();
            foreach (TwitterTextPart part in Parts)
            {
                if (part.Type == TwitterTextType.Text)
                {
                    List<TwitterTextPart> HashParts = GetTwitterParts(ref users, ref UserCounter, part.Data, USER_REPL, TwitterTextType.UserName, false);
                    TempParts.AddRange(HashParts);
                }
                else
                    TempParts.Add(part);
            }
            Parts = TempParts;

            tb.Inlines.Clear();
            foreach (TwitterTextPart part in Parts)
            {
                switch (part.Type)
                {
                    case TwitterTextType.Text:
                        {
                            tb.Inlines.Add(part.Data);
                            break;
                        }
                    case TwitterTextType.Link:
                        {
                            Hyperlink hyperLink = new Hyperlink() { NavigateUri = new Uri(part.Data) };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.Style = (Style)FindResource("BlueHyperLink");
                            hyperLink.RequestNavigate += Hyperlink_RequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                    case TwitterTextType.HashTag:
                        {
                            Hyperlink hyperLink = new Hyperlink() { NavigateUri = new Uri(string.Format("https://twitter.com/search?q=%23{0}&src=hash", part.Data.Substring(1))) };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.Style = (Style)FindResource("BlueHyperLink");
                            hyperLink.RequestNavigate += Hyperlink_RequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                    case TwitterTextType.UserName:
                        {
                            Hyperlink hyperLink = new Hyperlink() { NavigateUri = new Uri(string.Format("https://twitter.com/{0}/", part.Data.Substring(1))) };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.Style = (Style)FindResource("BlueHyperLink");
                            hyperLink.RequestNavigate += Hyperlink_RequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                }
            }

        }

        private List<TwitterTextPart> GetTwitterParts(ref List<string> DataArr, ref int DataCounter, string FormatString, string SplitText, TwitterTextType NonStringType, bool IsReturnNull)
        {
            List<TwitterTextPart> parts = new List<TwitterTextPart>();

            string[] link_splited = FormatString.Split(new string[] { SplitText }, StringSplitOptions.None);
            if (link_splited.Length > 1)
            {
                bool IsFirstAdded = false;
                foreach (string part in link_splited)
                {
                    if (IsFirstAdded)
                        parts.Add(new TwitterTextPart { Type = NonStringType, Data = DataArr[DataCounter++] });
                    parts.Add(new TwitterTextPart { Type = TwitterTextType.Text, Data = part });
                    IsFirstAdded = true;
                }
                return parts;
            }
            else
            {
                if (IsReturnNull)
                    return null;
                parts.Add(new TwitterTextPart { Data = FormatString, Type = TwitterTextType.Text });
                return parts;
            }
        }


        #endregion

        #region joymax news

        private void GetJoymax_News()
        {
            List<NewsItem> news = App.DMOProfile.GetNewsProfile().GetNews();

            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoAddJoyNews((s) =>
            {
                Rect viewbox;
                string mode;
                foreach (NewsItem n in news)
                {
                    mode = n.mode;
                    if (mode == "NOTICE")
                    {
                        viewbox = new Rect(215, 54, 90, 18);
                        mode = LanguageProvider.strings.NEWS_TYPE_NOTICE;
                    }
                    else if (mode == "EVENT")
                    {
                        viewbox = new Rect(215, 36, 90, 18);
                        mode = LanguageProvider.strings.NEWS_TYPE_EVENT;
                    }
                    else if (mode == "PATCH")
                    {
                        viewbox = new Rect(215, 0, 90, 18);
                        mode = LanguageProvider.strings.NEWS_TYPE_PATCH;
                    }
                    else
                        viewbox = new Rect(215, 0, 90, 18);
                    joymax_news.Add(new JoymaxItemViewModel { Title = n.subj, Content = n.content, Date = n.date, Type = mode, Link = n.url, ImgVB = viewbox });
                }
            }), news);
        }

        #endregion

        #region Interface processing

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Utils.OpenSiteNoDecode(e.Uri.ToString());
        }

        private void isLoading(bool state)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate()
            {
                if (state)
                {
                    LoaderIcon.IsEnabled = state;
                    LoaderIcon.Visibility = Visibility.Visible;
                }
                Storyboard sb = new Storyboard();
                DoubleAnimation dbl_anim = new DoubleAnimation();
                dbl_anim.From = state ? 0 : 1;
                dbl_anim.To = state ? 1 : 0;
                dbl_anim.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                Storyboard.SetTarget(dbl_anim, LoaderIcon);
                Storyboard.SetTargetProperty(dbl_anim, new PropertyPath(OpacityProperty));
                sb.Children.Add(dbl_anim);
                sb.Completed += (s, e) => {
                    LoaderIcon.Visibility = state ? Visibility.Visible : Visibility.Collapsed;
                    LoaderIcon.IsEnabled = state;
                };
                sb.Begin();
                isLoading_ = state;
            }));
        }

        private void button_news_twitter_Click(object sender, RoutedEventArgs e)
        {
            ShowTab(1, false);
        }

        private void button_news_joymax_Click(object sender, RoutedEventArgs e)
        {
            ShowTab(2, false);
        }

        //Добавляет хэндлер колеса мыши
        private void NewsScroll_Loaded_1(object sender, RoutedEventArgs e)
        {
            Twitter_News.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
            Joymax_News.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
        }

        //Прокручивание контента по колесу мыши
        private void MyMouseWheelH(object sender, RoutedEventArgs e)
        {
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            double x = (double)eargs.Delta;
            double y = NewsScroll.VerticalOffset;
            NewsScroll.ScrollToVerticalOffset(y - x);
        }

        #endregion
    }

    public class IntConverter : System.Windows.Data.IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
                return ((double)value - 18);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
