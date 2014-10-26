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
        private BackgroundWorker bwLoadTwitter = new BackgroundWorker();
        private BackgroundWorker bwLoadJoymax = new BackgroundWorker();
        private Storyboard ShowTwitter = new Storyboard();
        private Storyboard HideTwitter = new Storyboard();
        private Storyboard ShowJoymax = new Storyboard();
        private Storyboard HideJoymax = new Storyboard();
        private Storyboard AnimShow = new Storyboard();
        private Storyboard AnimHide = new Storyboard();

        private int AnimSpeed = 100;

        private bool _IsLoading = false;
        public bool IsLoading {
            get {
                return _IsLoading;
            }
        }

        private TwitterViewModel TwitterVM = new TwitterViewModel();
        private List<TwitterItemViewModel> TwitterStatuses = new List<TwitterItemViewModel>();
        private delegate void DoAddTwit(List<UserStatus> statusList, BitmapImage bmp);
        private delegate void DoLoadTwitter(List<TwitterItemViewModel> statuses);

        private JoymaxViewModel JoymaxVM = new JoymaxViewModel();
        private List<JoymaxItemViewModel> JoymaxNews = new List<JoymaxItemViewModel>();
        private delegate void DoAddJoyNews(List<NewsItem> news);
        private delegate void DoLoadJoymax(List<JoymaxItemViewModel> news);

        public delegate void ChangedEventHandler(object sender, byte tabNum);
        public event ChangedEventHandler TabChanged;
        protected virtual void OnChanged(byte tabNum) {
            if (TabChanged != null) {
                TabChanged(this, tabNum);
            }
        }

        private string _jsonUrlLoaded;
        private string _jsonUrl;

        public NewsBlock() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                LauncherEnv.Settings.ProfileChanged += ReloadNews;
                LanguageEnv.Languagechanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
                TwitterNewsList.DataContext = TwitterVM;
                JoymaxNewsList.DataContext = JoymaxVM;

                //Init animations for News 
                DoubleAnimation dShowJoymax = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dShowJoymax, JoymaxNewsList);
                Storyboard.SetTargetProperty(dShowJoymax, new PropertyPath(OpacityProperty));
                DoubleAnimation dHideJoymax = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dHideJoymax, JoymaxNewsList);
                Storyboard.SetTargetProperty(dHideJoymax, new PropertyPath(OpacityProperty));
                DoubleAnimation dShowTwitter = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dShowTwitter, TwitterNewsList);
                Storyboard.SetTargetProperty(dShowTwitter, new PropertyPath(OpacityProperty));
                DoubleAnimation dHideTwitter = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dHideTwitter, TwitterNewsList);
                Storyboard.SetTargetProperty(dHideTwitter, new PropertyPath(OpacityProperty));

                ShowJoymax.Children.Add(dShowJoymax);
                HideTwitter.Children.Add(dHideTwitter);
                HideTwitter.Completed += (s, e) => {
                    IsLoadingAnim(false);
                    TwitterNewsList.Visibility = Visibility.Collapsed;
                    JoymaxNewsList.Visibility = Visibility.Visible;
                    ShowJoymax.Begin();
                };

                ShowTwitter.Children.Add(dShowTwitter);
                HideJoymax.Children.Add(dHideJoymax);
                HideJoymax.Completed += (s, e) => {
                    IsLoadingAnim(false);
                    TwitterNewsList.Visibility = Visibility.Visible;
                    JoymaxNewsList.Visibility = Visibility.Collapsed;
                    ShowTwitter.Begin();
                };

                //Animations for loading circle
                DoubleAnimation dblAnimShow = new DoubleAnimation();
                dblAnimShow.To = 1;
                dblAnimShow.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                DoubleAnimation dblAnimHide = new DoubleAnimation();
                dblAnimHide.To = 0;
                dblAnimHide.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                Storyboard.SetTarget(dblAnimShow, LoaderIcon);
                Storyboard.SetTarget(dblAnimHide, LoaderIcon);
                Storyboard.SetTargetProperty(dblAnimShow, new PropertyPath(OpacityProperty));
                Storyboard.SetTargetProperty(dblAnimHide, new PropertyPath(OpacityProperty));
                AnimShow.Children.Add(dblAnimShow);
                AnimHide.Children.Add(dblAnimHide);
                AnimHide.Completed += (s, e) => {
                    LoaderIcon.Visibility = Visibility.Collapsed; LoaderIcon.IsEnabled = false;
                };

                bwLoadTwitter.RunWorkerCompleted += (s, e) => {
                    HideJoymax.Begin();
                };
                bwLoadTwitter.DoWork += (s1, e1) => {
                    if (!TwitterVM.IsDataLoaded || _jsonUrlLoaded != _jsonUrl) {
                        IsLoadingAnim(true);
                        GetTwitterNewsAPI11(_jsonUrl);
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoLoadTwitter((list) => {
                            TwitterVM.UnLoadData();
                            TwitterVM.LoadData(list);
                        }), TwitterStatuses);
                        _jsonUrlLoaded = _jsonUrl;
                    }
                };

                bwLoadJoymax.RunWorkerCompleted += (s, e) => {
                    HideTwitter.Begin();
                };
                bwLoadJoymax.DoWork += (s1, e1) => {
                    if (!JoymaxVM.IsDataLoaded) {
                        IsLoadingAnim(true);
                        GetJoymaxNews();
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoLoadJoymax((list) => {
                            if (list != null)
                                JoymaxVM.LoadData(list);
                        }), JoymaxNews);
                    }
                };
                ReloadNews();
            }
        }


        private void ReloadNews() {
            if (_jsonUrl != LauncherEnv.Settings.CurrentProfile.News.TwitterUrl) {
                _jsonUrl = LauncherEnv.Settings.CurrentProfile.News.TwitterUrl;
            }
            if (LauncherEnv.Settings.CurrentProfile.DMOProfile.IsNewsAvailable) {
                ShowTab(LauncherEnv.Settings.CurrentProfile.News.FirstTab);
            } else {
                ShowTab(0);
            }
        }

        public void OnShowTwitter(object sender, RoutedEventArgs e) {
            ShowTab(0);
        }

        public void OnShowJoymax(object sender, RoutedEventArgs e) {
            ShowTab(1);
        }

        public void ShowTab(byte tab) {
            if (IsLoading) {
                return;
            }
            OnChanged(tab);
            if (tab == 0) {
                bwLoadTwitter.RunWorkerAsync();
            } else {
                bwLoadJoymax.RunWorkerAsync();
            }
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

        private enum TwitterTextType {
            Text,
            Link,
            HashTag,
            UserName
        }

        private struct TwitterTextPart {
            public TwitterTextType Type;
            public string Data;
        }

        public void GetTwitterNewsAPI11(string url) {
            WebClient wc = new WebClient();
            wc.Proxy = (IWebProxy)null;
            Uri link = new Uri(url);
            TwitterStatuses.Clear();
            string response;
            try {
                response = wc.DownloadString(link);
            } catch (Exception e) {
                TwitterStatuses.Add(new TwitterItemViewModel {
                    Title = LanguageEnv.Strings.NewsTwitterError + ": " + e.Message + " [ERRCODE 3 - Remote Error]"
                });
                return;
            }

            JArray tList;
            try {
                tList = JArray.Parse(System.Web.HttpUtility.HtmlDecode(response));
            } catch {
                TwitterStatuses.Add(new TwitterItemViewModel {
                    Title = LanguageEnv.Strings.NewsTwitterError + " [ERRCODE 1 - Parse Error]"
                });
                return;
            }

            List<UserStatus> statuses = new List<UserStatus>();
            List<ProfileImage> profileImages = new List<ProfileImage>();
            List<ProfileImage> currentImage;
            ProfileImage profileImage;
            for (int i = 0; i < tList.Count(); i++) {
                JObject tweet = JObject.Parse(tList[i].ToString());
                UserStatus status = new UserStatus();

                status.UserName = tweet["user"]["name"].ToString();
                status.UserScreenName = tweet["user"]["screen_name"].ToString();
                status.ProfileImageUrl = tweet["user"]["profile_image_url"].ToString();
                try {
                    status.RetweetImageUrl = tweet["retweeted_status"]["user"]["profile_image_url"].ToString();
                } catch {
                };
                status.Status = tweet["text"].ToString();
                status.StatusId = tweet["id"].ToString();
                status.StatusDate = ParseDateTime(tweet["created_at"].ToString());

                string profile_image;
                if (status.RetweetImageUrl != null) {
                    profile_image = status.RetweetImageUrl;
                } else {
                    profile_image = status.ProfileImageUrl;
                }

                currentImage = profileImages.FindAll(im => (im.url == profile_image));
                if (currentImage.Count == 0) {
                    profileImage = new ProfileImage();
                    profileImage.url = profile_image;
                    profileImage.bitmap = GetImage(profile_image);
                    if (profileImage.bitmap != null) {
                        profileImages.Add(profileImage);
                    }
                } else {
                    profileImage = currentImage[0];
                }
                status.ProfileImageBitmap = profileImage.bitmap;
                statuses.Add(status);
            }
            foreach (UserStatus status in statuses) {
                TwitterStatuses.Add(new TwitterItemViewModel {
                    Title = status.Status,
                    Date = status.StatusDate.ToLongDateString()
                        + " " + status.StatusDate.ToShortTimeString(),
                    Image = status.ProfileImageBitmap
                });
            }
        }

        private BitmapImage GetImage(string url) {
            WebClient wc = new WebClient();
            wc.Proxy = (IWebProxy)null;

            Uri uri = new Uri(url);
            byte[] image_bytes;
            try {
                image_bytes = wc.DownloadData(uri);
            } catch {
                return null;
            }

            MemoryStream img_stream = new MemoryStream(image_bytes, 0, image_bytes.Length);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = img_stream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        //парсинг строки времени
        private static DateTime ParseDateTime(string date) {
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
        private void ParseTextBlock(object sender, RoutedEventArgs e) {
            TextBlock tb = ((TextBlock)sender);

            if (tb.Text.Contains(LINK_REPL) || tb.Text.Contains(HASHTAG_REPL)) {
                return;
            }

            string FormatString = URLRegex.Replace(tb.Text, LINK_REPL);
            FormatString = HashRegex.Replace(FormatString, HASHTAG_REPL);
            FormatString = TUserRegex.Replace(FormatString, USER_REPL);

            //Collecting links, hashtags and users in lists
            int LinkCounter = 0;
            List<string> links = new List<string>();
            foreach (Match match in URLRegex.Matches(tb.Text)) {
                links.Add(match.Groups[0].Value);
            }

            int HashCounter = 0;
            List<string> tags = new List<string>();
            foreach (Match match in HashRegex.Matches(tb.Text)) {
                tags.Add(match.Groups[0].Value);
            }

            int UserCounter = 0;
            List<string> users = new List<string>();
            foreach (Match match in TUserRegex.Matches(tb.Text)) {
                users.Add(match.Groups[0].Value);
            }

            //Creating list of splitted text by links
            List<TwitterTextPart> Parts = GetTwitterParts(ref links, ref LinkCounter, FormatString, LINK_REPL, TwitterTextType.Link, false);

            //Creating list of splitted text (from older list) by hashtags
            List<TwitterTextPart> TempParts = new List<TwitterTextPart>();
            foreach (TwitterTextPart part in Parts) {
                if (part.Type == TwitterTextType.Text) {
                    List<TwitterTextPart> HashParts = GetTwitterParts(ref tags, ref HashCounter, part.Data, HASHTAG_REPL, TwitterTextType.HashTag, false);
                    TempParts.AddRange(HashParts);
                } else {
                    TempParts.Add(part);
                }
            }
            Parts = TempParts;

            //Creating list of splitted text (from older list) by usernames
            TempParts = new List<TwitterTextPart>();
            foreach (TwitterTextPart part in Parts) {
                if (part.Type == TwitterTextType.Text) {
                    List<TwitterTextPart> HashParts = GetTwitterParts(ref users, ref UserCounter, part.Data, USER_REPL, TwitterTextType.UserName, false);
                    TempParts.AddRange(HashParts);
                } else {
                    TempParts.Add(part);
                }
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
                            Hyperlink hyperLink = new Hyperlink() {
                                NavigateUri = new Uri(part.Data)
                            };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.Style = (Style)FindResource("BlueHyperLink");
                            hyperLink.RequestNavigate += OnRequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                    case TwitterTextType.HashTag: {
                            Hyperlink hyperLink = new Hyperlink() {
                                NavigateUri = new Uri(string.Format("https://twitter.com/search?q=%23{0}&src=hash", part.Data.Substring(1)))
                            };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.Style = (Style)FindResource("BlueHyperLink");
                            hyperLink.RequestNavigate += OnRequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                    case TwitterTextType.UserName: {
                            Hyperlink hyperLink = new Hyperlink() {
                                NavigateUri = new Uri(string.Format("https://twitter.com/{0}/", part.Data.Substring(1)))
                            };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.Style = (Style)FindResource("BlueHyperLink");
                            hyperLink.RequestNavigate += OnRequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                }
            }

        }

        private List<TwitterTextPart> GetTwitterParts(ref List<string> DataArr, ref int DataCounter, string FormatString, string SplitText, TwitterTextType NonStringType, bool IsReturnNull) {
            List<TwitterTextPart> parts = new List<TwitterTextPart>();

            string[] linkSplitted = FormatString.Split(new string[] { SplitText }, StringSplitOptions.None);
            if (linkSplitted.Length > 1) {
                bool IsFirstAdded = false;
                foreach (string part in linkSplitted) {
                    if (IsFirstAdded) {
                        parts.Add(new TwitterTextPart {
                            Type = NonStringType,
                            Data = DataArr[DataCounter++]
                        });
                    }
                    parts.Add(new TwitterTextPart {
                        Type = TwitterTextType.Text,
                        Data = part
                    });
                    IsFirstAdded = true;
                }
                return parts;
            } else {
                if (IsReturnNull) {
                    return null;
                }
                parts.Add(new TwitterTextPart {
                    Data = FormatString,
                    Type = TwitterTextType.Text
                });
                return parts;
            }
        }

        #endregion

        #region Joymax news

        private void GetJoymaxNews() {
            List<NewsItem> news = Environment.Containers.Profile.GetJoymaxProfile().NewsProfile.GetNews();
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoAddJoyNews((s) => {
                Rect viewbox;
                string mode;
                foreach (NewsItem n in news) {
                    mode = n.Mode;
                    if (mode == "NOTICE") {
                        viewbox = new Rect(215, 54, 90, 18);
                        mode = LanguageEnv.Strings.NewsType_Notice;
                    } else if (mode == "EVENT") {
                        viewbox = new Rect(215, 36, 90, 18);
                        mode = LanguageEnv.Strings.NewsType_Event;
                    } else if (mode == "PATCH") {
                        viewbox = new Rect(215, 0, 90, 18);
                        mode = LanguageEnv.Strings.NewsType_Patch;
                    } else {
                        viewbox = new Rect(215, 0, 90, 18);
                    }
                    JoymaxNews.Add(new JoymaxItemViewModel {
                        Title = n.Subject,
                        Content = n.Content,
                        Date = n.Date,
                        Type = mode,
                        Link = n.Url,
                        ImgVB = viewbox
                    });
                }
            }), news);
        }

        #endregion

        #region Interface processing

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            Utils.OpenSiteNoDecode(e.Uri.ToString());
        }

        private void IsLoadingAnim(bool state) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                _IsLoading = state;
                if (_IsLoading) {
                    LoaderIcon.IsEnabled = true;
                    LoaderIcon.Visibility = Visibility.Visible;
                    AnimShow.Begin();
                } else
                    AnimHide.Begin();
            }));
        }

        //Добавляет хэндлер колеса мыши
        private void OnNewsScrollLoaded(object sender, RoutedEventArgs e) {
            TwitterNewsList.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
            JoymaxNewsList.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
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
