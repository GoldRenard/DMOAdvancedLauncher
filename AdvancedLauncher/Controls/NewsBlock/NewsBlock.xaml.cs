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

using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;
using DMOLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvancedLauncher.Controls {
    public partial class NewsBlock : UserControl {
        BackgroundWorker bwLoadTwitter = new BackgroundWorker();
        BackgroundWorker bwLoadJoymax = new BackgroundWorker();
        Storyboard sbShowTwitter = new Storyboard();
        Storyboard sbHideTwitter = new Storyboard();
        Storyboard sbShowJoymax = new Storyboard();
        Storyboard sbHideJoymax = new Storyboard();
        Storyboard sbAnimShow = new Storyboard();
        Storyboard sbAnimHide = new Storyboard();

        private int AnimSpeed = 100;

        private bool _IsLoading = false;
        public bool IsLoading { get { return _IsLoading; } }

        private TwitterViewModel TwitterVM = new TwitterViewModel();
        private List<TwitterItemViewModel> twitter_statuses = new List<TwitterItemViewModel>();
        private delegate void DoAddTwit(List<UserStatus> status_list, BitmapImage bmp);
        private delegate void DoLoadTwitter(List<TwitterItemViewModel> statuses);

        private JoymaxViewModel JoymaxVM = new JoymaxViewModel();
        private List<JoymaxItemViewModel> joymax_news = new List<JoymaxItemViewModel>();
        private delegate void DoAddJoyNews(List<NewsItem> news);
        private delegate void DoLoadJoymax(List<JoymaxItemViewModel> news);

        public delegate void ChangedEventHandler(object sender, byte tab_num);
        public event ChangedEventHandler TabChanged;
        protected virtual void OnChanged(byte tab_num) {
            if (TabChanged != null)
                TabChanged(this, tab_num);
        }

        private string _json_url_loaded;
        private string _json_url;

        public NewsBlock() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                LauncherEnv.Settings.ProfileChanged += ReloadNews;
                LanguageEnv.Languagechanged += delegate() { this.DataContext = LanguageEnv.Strings; };
                Twitter_News.DataContext = TwitterVM;
                Joymax_News.DataContext = JoymaxVM;

                //Init animations for News 
                DoubleAnimation dShowJoymax = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dShowJoymax, Joymax_News);
                Storyboard.SetTargetProperty(dShowJoymax, new PropertyPath(OpacityProperty));
                DoubleAnimation dHideJoymax = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dHideJoymax, Joymax_News);
                Storyboard.SetTargetProperty(dHideJoymax, new PropertyPath(OpacityProperty));
                DoubleAnimation dShowTwitter = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dShowTwitter, Twitter_News);
                Storyboard.SetTargetProperty(dShowTwitter, new PropertyPath(OpacityProperty));
                DoubleAnimation dHideTwitter = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dHideTwitter, Twitter_News);
                Storyboard.SetTargetProperty(dHideTwitter, new PropertyPath(OpacityProperty));

                sbShowJoymax.Children.Add(dShowJoymax);
                sbHideTwitter.Children.Add(dHideTwitter);
                sbHideTwitter.Completed += (s, e) => {
                    IsLoadingAnim(false);
                    Twitter_News.Visibility = Visibility.Collapsed;
                    Joymax_News.Visibility = Visibility.Visible;
                    sbShowJoymax.Begin();
                };

                sbShowTwitter.Children.Add(dShowTwitter);
                sbHideJoymax.Children.Add(dHideJoymax);
                sbHideJoymax.Completed += (s, e) => {
                    IsLoadingAnim(false);
                    Twitter_News.Visibility = Visibility.Visible;
                    Joymax_News.Visibility = Visibility.Collapsed;
                    sbShowTwitter.Begin();
                };

                //Animations for loading circle
                DoubleAnimation dbl_anim_show = new DoubleAnimation();
                dbl_anim_show.To = 1;
                dbl_anim_show.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                DoubleAnimation dbl_anim_hide = new DoubleAnimation();
                dbl_anim_hide.To = 0;
                dbl_anim_hide.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                Storyboard.SetTarget(dbl_anim_show, LoaderIcon);
                Storyboard.SetTarget(dbl_anim_hide, LoaderIcon);
                Storyboard.SetTargetProperty(dbl_anim_show, new PropertyPath(OpacityProperty));
                Storyboard.SetTargetProperty(dbl_anim_hide, new PropertyPath(OpacityProperty));
                sbAnimShow.Children.Add(dbl_anim_show);
                sbAnimHide.Children.Add(dbl_anim_hide);
                sbAnimHide.Completed += (s, e) => { LoaderIcon.Visibility = Visibility.Collapsed; LoaderIcon.IsEnabled = false; };

                bwLoadTwitter.RunWorkerCompleted += (s, e) => { sbHideJoymax.Begin(); };
                bwLoadTwitter.DoWork += (s1, e1) => {
                    if (!TwitterVM.IsDataLoaded || _json_url_loaded != _json_url) {
                        IsLoadingAnim(true);
                        GetTwitter_News_API11(_json_url);
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoLoadTwitter((list) => {
                            TwitterVM.UnLoadData();
                            TwitterVM.LoadData(list);
                        }), twitter_statuses);
                        _json_url_loaded = _json_url;
                    }
                };

                bwLoadJoymax.RunWorkerCompleted += (s, e) => { sbHideTwitter.Begin(); };
                bwLoadJoymax.DoWork += (s1, e1) => {
                    if (!JoymaxVM.IsDataLoaded) {
                        IsLoadingAnim(true);
                        GetJoymax_News();
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoLoadJoymax((list) => {
                            if (list != null)
                                JoymaxVM.LoadData(list);
                        }), joymax_news);
                    }
                };

                ReloadNews();
            }
        }


        void ReloadNews() {
            if (_json_url != LauncherEnv.Settings.pCurrent.News.TwitterUrl)
                _json_url = LauncherEnv.Settings.pCurrent.News.TwitterUrl;
            if (LauncherEnv.Settings.pCurrent.DMOProfile.IsNewsAvailable)
                ShowTab(LauncherEnv.Settings.pCurrent.News.FirstTab);
            else
                ShowTab(0);
        }

        public void ShowTwitter(object sender, RoutedEventArgs e) {
            ShowTab(0);
        }

        public void ShowJoymax(object sender, RoutedEventArgs e) {
            ShowTab(1);
        }

        public void ShowTab(byte tab) {
            if (IsLoading)
                return;
            OnChanged(tab);
            if (tab == 0)
                bwLoadTwitter.RunWorkerAsync();
            else
                bwLoadJoymax.RunWorkerAsync();
        }

        #region Twitter statuses

        private Regex URLRegex = new Regex("((https?|s?ftp|ssh)\\:\\/\\/[^\"\\s\\<\\>]*[^.,;'\">\\:\\s\\<\\>\\)\\]\\!])", RegexOptions.Compiled);
        private Regex HashRegex = new Regex(@"(\B#\w*[a-zA-Z]+\w*)", RegexOptions.Compiled);
        private Regex TUserRegex = new Regex(@"(\B@\w*[a-zA-Z]+\w*)", RegexOptions.Compiled);
        private static string LINK_REPL = "%DMOAL:LINK%", HASHTAG_REPL = "%DMOAL:HASHTAG%", USER_REPL = "%DMOAL:TWUSER%";

        public struct UserStatus {
            public string UserName;
            public string UserScreenName;
            public string ProfileImageUrl;
            public string RetweetImageUrl;
            public string Status;
            public string StatusId;
            public BitmapImage ProfileImageBitmap;
            public DateTime StatusDate;
        }

        public struct ProfileImage {
            public string url;
            public BitmapImage bitmap;
        }

        enum TwitterTextType {
            Text,
            Link,
            HashTag,
            UserName
        }

        struct TwitterTextPart {
            public TwitterTextType Type;
            public string Data;
        }

        public void GetTwitter_News_API11(string url) {
            WebClient wc = new WebClient();
            wc.Proxy = (IWebProxy)null;
            Uri link = new Uri(url);
            twitter_statuses.Clear();
            string response;
            try {
                response = wc.DownloadString(link);
            } catch (Exception e) {
                twitter_statuses.Add(new TwitterItemViewModel { Title = LanguageEnv.Strings.NewsTwitterError + ": " + e.Message + " [ERRCODE 3 - Remote Error]" });
                return;
            }

            JArray tList;
            try {
                tList = JArray.Parse(System.Web.HttpUtility.HtmlDecode(response));
            } catch {
                twitter_statuses.Add(new TwitterItemViewModel { Title = LanguageEnv.Strings.NewsTwitterError + " [ERRCODE 1 - Parse Error]" });
                return;
            }

            List<UserStatus> statuses = new List<UserStatus>();
            List<ProfileImage> prof_images = new List<ProfileImage>();
            List<ProfileImage> cur_image;
            ProfileImage p_image;
            for (int i = 0; i < tList.Count(); i++) {
                JObject tweet = JObject.Parse(tList[i].ToString());
                UserStatus status = new UserStatus();

                status.UserName = tweet["user"]["name"].ToString();
                status.UserScreenName = tweet["user"]["screen_name"].ToString();
                status.ProfileImageUrl = tweet["user"]["profile_image_url"].ToString();
                try { status.RetweetImageUrl = tweet["retweeted_status"]["user"]["profile_image_url"].ToString(); } catch { };
                status.Status = tweet["text"].ToString();
                status.StatusId = tweet["id"].ToString();
                status.StatusDate = ParseDateTime(tweet["created_at"].ToString());

                string profile_image;
                if (status.RetweetImageUrl != null)
                    profile_image = status.RetweetImageUrl;
                else
                    profile_image = status.ProfileImageUrl;

                cur_image = prof_images.FindAll(im => (im.url == profile_image));
                if (cur_image.Count == 0) {
                    p_image = new ProfileImage();
                    p_image.url = profile_image;
                    p_image.bitmap = GetImage(profile_image);
                    if (p_image.bitmap != null)
                        prof_images.Add(p_image);
                } else
                    p_image = cur_image[0];

                status.ProfileImageBitmap = p_image.bitmap;

                statuses.Add(status);
            }
            foreach (UserStatus status in statuses)
                twitter_statuses.Add(new TwitterItemViewModel { Title = status.Status, Date = status.StatusDate.ToLongDateString() + " " + status.StatusDate.ToShortTimeString(), Image = status.ProfileImageBitmap });
        }

        public BitmapImage GetImage(string url) {
            WebClient wc = new WebClient();
            wc.Proxy = (IWebProxy)null;

            Uri uri = new Uri(url);
            byte[] image_bytes;
            try { image_bytes = wc.DownloadData(uri); } catch { return null; }

            MemoryStream img_stream = new MemoryStream(image_bytes, 0, image_bytes.Length);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = img_stream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        //парсинг строки времени
        public static DateTime ParseDateTime(string date) {
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
        private void TextBlock_Parse(object sender, RoutedEventArgs e) {
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
            foreach (TwitterTextPart part in Parts) {
                if (part.Type == TwitterTextType.Text) {
                    List<TwitterTextPart> HashParts = GetTwitterParts(ref tags, ref HashCounter, part.Data, HASHTAG_REPL, TwitterTextType.HashTag, false);
                    TempParts.AddRange(HashParts);
                } else
                    TempParts.Add(part);
            }
            Parts = TempParts;

            //Creating list of splitted text (from older list) by usernames
            TempParts = new List<TwitterTextPart>();
            foreach (TwitterTextPart part in Parts) {
                if (part.Type == TwitterTextType.Text) {
                    List<TwitterTextPart> HashParts = GetTwitterParts(ref users, ref UserCounter, part.Data, USER_REPL, TwitterTextType.UserName, false);
                    TempParts.AddRange(HashParts);
                } else
                    TempParts.Add(part);
            }
            Parts = TempParts;

            tb.Inlines.Clear();
            foreach (TwitterTextPart part in Parts) {
                switch (part.Type) {
                    case TwitterTextType.Text: {
                            tb.Inlines.Add(part.Data);
                            break;
                        }
                    case TwitterTextType.Link: {
                            Hyperlink hyperLink = new Hyperlink() { NavigateUri = new Uri(part.Data) };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.Style = (Style)FindResource("BlueHyperLink");
                            hyperLink.RequestNavigate += Hyperlink_RequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                    case TwitterTextType.HashTag: {
                            Hyperlink hyperLink = new Hyperlink() { NavigateUri = new Uri(string.Format("https://twitter.com/search?q=%23{0}&src=hash", part.Data.Substring(1))) };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.Style = (Style)FindResource("BlueHyperLink");
                            hyperLink.RequestNavigate += Hyperlink_RequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                    case TwitterTextType.UserName: {
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

        private List<TwitterTextPart> GetTwitterParts(ref List<string> DataArr, ref int DataCounter, string FormatString, string SplitText, TwitterTextType NonStringType, bool IsReturnNull) {
            List<TwitterTextPart> parts = new List<TwitterTextPart>();

            string[] link_splited = FormatString.Split(new string[] { SplitText }, StringSplitOptions.None);
            if (link_splited.Length > 1) {
                bool IsFirstAdded = false;
                foreach (string part in link_splited) {
                    if (IsFirstAdded)
                        parts.Add(new TwitterTextPart { Type = NonStringType, Data = DataArr[DataCounter++] });
                    parts.Add(new TwitterTextPart { Type = TwitterTextType.Text, Data = part });
                    IsFirstAdded = true;
                }
                return parts;
            } else {
                if (IsReturnNull)
                    return null;
                parts.Add(new TwitterTextPart { Data = FormatString, Type = TwitterTextType.Text });
                return parts;
            }
        }

        #endregion

        #region Joymax news

        private void GetJoymax_News() {
            List<NewsItem> news = Environment.Containers.Profile.GetJoymaxProfile().NewsProfile.GetNews();
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoAddJoyNews((s) => {
                Rect viewbox;
                string mode;
                foreach (NewsItem n in news) {
                    mode = n.mode;
                    if (mode == "NOTICE") {
                        viewbox = new Rect(215, 54, 90, 18);
                        mode = LanguageEnv.Strings.NewsType_Notice;
                    } else if (mode == "EVENT") {
                        viewbox = new Rect(215, 36, 90, 18);
                        mode = LanguageEnv.Strings.NewsType_Event;
                    } else if (mode == "PATCH") {
                        viewbox = new Rect(215, 0, 90, 18);
                        mode = LanguageEnv.Strings.NewsType_Patch;
                    } else
                        viewbox = new Rect(215, 0, 90, 18);
                    joymax_news.Add(new JoymaxItemViewModel { Title = n.subj, Content = n.content, Date = n.date, Type = mode, Link = n.url, ImgVB = viewbox });
                }
            }), news);
        }

        #endregion

        #region Interface processing

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            Utils.OpenSiteNoDecode(e.Uri.ToString());
        }

        private void IsLoadingAnim(bool state) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                _IsLoading = state;
                if (_IsLoading) {
                    LoaderIcon.IsEnabled = true;
                    LoaderIcon.Visibility = Visibility.Visible;
                    sbAnimShow.Begin();
                } else
                    sbAnimHide.Begin();
            }));
        }

        //Добавляет хэндлер колеса мыши
        private void NewsScroll_Loaded_1(object sender, RoutedEventArgs e) {
            Twitter_News.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
            Joymax_News.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
        }

        //Прокручивание контента по колесу мыши
        private void MyMouseWheelH(object sender, RoutedEventArgs e) {
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            double x = (double)eargs.Delta;
            double y = NewsScroll.VerticalOffset;
            NewsScroll.ScrollToVerticalOffset(y - x);
        }

        #endregion
    }
}
