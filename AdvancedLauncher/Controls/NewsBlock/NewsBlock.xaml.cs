// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AdvancedLauncher.Environment;
using AdvancedLauncher.Service;
using DMOLibrary;
using DMOLibrary.Profiles;
using Newtonsoft.Json.Linq;

namespace AdvancedLauncher.Controls {

    public partial class NewsBlock : UserControl {
        private BackgroundWorker bwLoadTwitter = new BackgroundWorker();
        private BackgroundWorker bwLoadJoymax = new BackgroundWorker();

        private Storyboard ShowTwitter = new Storyboard();
        private Storyboard ShowJoymax = new Storyboard();

        private int AnimSpeed = 300;

        private TwitterViewModel TwitterVM = new TwitterViewModel();
        private List<TwitterItemViewModel> TwitterStatuses = new List<TwitterItemViewModel>();

        private delegate void DoAddTwit(List<UserStatus> statusList, BitmapImage bmp);

        private delegate void DoLoadTwitter(List<TwitterItemViewModel> statuses);

        private JoymaxViewModel JoymaxVM = new JoymaxViewModel();
        private List<JoymaxItemViewModel> JoymaxNews = new List<JoymaxItemViewModel>();

        private delegate void DoAddJoyNews(List<DMONewsProfile.NewsItem> news);

        private delegate void DoLoadJoymax(List<JoymaxItemViewModel> news);

        private string _jsonUrlLoaded;
        private string _jsonUrl;

        public NewsBlock() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                LauncherEnv.Settings.ProfileChanged += ReloadNews;
                LanguageEnv.LanguageChanged += delegate() {
                    this.DataContext = LanguageEnv.Strings;
                };
                TwitterNewsList.DataContext = TwitterVM;
                JoymaxNewsList.DataContext = JoymaxVM;

                //Init animations for News
                DoubleAnimation dShowJoymax = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dShowJoymax, JoymaxNewsList);
                Storyboard.SetTargetProperty(dShowJoymax, new PropertyPath(OpacityProperty));
                DoubleAnimation dShowTwitter = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(AnimSpeed)));
                Storyboard.SetTarget(dShowTwitter, TwitterNewsList);
                Storyboard.SetTargetProperty(dShowTwitter, new PropertyPath(OpacityProperty));

                ShowJoymax.Children.Add(dShowJoymax);
                ShowTwitter.Children.Add(dShowTwitter);

                bwLoadTwitter.RunWorkerCompleted += (s, e) => {
                    ShowTwitter.Begin();
                    IsTwitterLoadingAnim(false);
                };

                bwLoadTwitter.DoWork += (s1, e1) => {
                    if (!TwitterVM.IsDataLoaded || _jsonUrlLoaded != _jsonUrl) {
                        IsTwitterLoadingAnim(true);
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => {
                            TwitterVM.UnLoadData();
                        }));
                        GetTwitterNewsAPI11(_jsonUrl);
                        this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoLoadTwitter((list) => {
                            TwitterVM.LoadData(list);
                        }), TwitterStatuses);
                        _jsonUrlLoaded = _jsonUrl;
                    }
                };

                bwLoadJoymax.RunWorkerCompleted += (s, e) => {
                    ShowJoymax.Begin();
                    IsJoymaxLoadingAnim(false);
                };
                bwLoadJoymax.DoWork += (s1, e1) => {
                    if (!JoymaxVM.IsDataLoaded) {
                        IsJoymaxLoadingAnim(true);
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (this.IsLoaded) {
                ShowTab(((TabControl)sender).SelectedIndex);
            }
        }

        private void ReloadNews() {
            if (_jsonUrl != LauncherEnv.Settings.CurrentProfile.News.TwitterUrl) {
                _jsonUrl = LauncherEnv.Settings.CurrentProfile.News.TwitterUrl;
            }

            bool newsSupported = LauncherEnv.Settings.CurrentProfile.DMOProfile.IsNewsAvailable;
            NavJoymax.Visibility = newsSupported ? Visibility.Visible : Visibility.Hidden;
            NavTwitter.Visibility = newsSupported ? Visibility.Visible : Visibility.Hidden;
            byte index = newsSupported ? LauncherEnv.Settings.CurrentProfile.News.FirstTab : (byte)0;
            NewsTabControl.SelectedIndex = index;
            ShowTab(index);
        }

        public void ShowTab(int tab) {
            if (tab < 0) {
                return;
            }
            if (tab == 0 && !bwLoadTwitter.IsBusy) {
                bwLoadTwitter.RunWorkerAsync();
            } else if (!bwLoadJoymax.IsBusy) {
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
            Uri link = new Uri(url);
            TwitterStatuses.Clear();
            string response;
            using (WebClient wc = new WebClientEx()) {
                try {
                    response = wc.DownloadString(link);
                } catch (Exception e) {
                    TwitterStatuses.Add(new TwitterItemViewModel {
                        Title = LanguageEnv.Strings.NewsTwitterError + ": " + e.Message + " [ERRCODE 3 - Remote Error]"
                    });
                    return;
                }
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
                    status.UserScreenName = tweet["retweeted_status"]["user"]["name"].ToString();
                    status.UserName = tweet["retweeted_status"]["user"]["screen_name"].ToString();
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
                    Image = status.ProfileImageBitmap,
                    StatusLink = "https://twitter.com/statuses/" + status.StatusId,
                    UserLink = "https://twitter.com/" + status.UserScreenName,
                    UserName = status.UserName
                });
            }
        }

        private BitmapImage GetImage(string url) {
            using (WebClient wc = new WebClientEx()) {
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
                            hyperLink.RequestNavigate += OnRequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                    case TwitterTextType.HashTag: {
                            Hyperlink hyperLink = new Hyperlink() {
                                NavigateUri = new Uri(string.Format("https://twitter.com/search?q=%23{0}&src=hash", part.Data.Substring(1)))
                            };
                            hyperLink.Inlines.Add(part.Data);
                            hyperLink.RequestNavigate += OnRequestNavigate;
                            tb.Inlines.Add(hyperLink);
                            break;
                        }
                    case TwitterTextType.UserName: {
                            Hyperlink hyperLink = new Hyperlink() {
                                NavigateUri = new Uri(string.Format("https://twitter.com/{0}/", part.Data.Substring(1)))
                            };
                            hyperLink.Inlines.Add(part.Data);
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

        #endregion Twitter statuses

        #region Joymax news

        private void GetJoymaxNews() {
            if (LauncherEnv.Settings.CurrentProfile.DMOProfile.NewsProfile != null) {
                List<DMONewsProfile.NewsItem> news = LauncherEnv.Settings.CurrentProfile.DMOProfile.NewsProfile.GetNews();
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DoAddJoyNews((s) => {
                    Rect viewbox;
                    string mode;
                    foreach (DMONewsProfile.NewsItem n in news) {
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
        }

        #endregion Joymax news

        #region Interface processing

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            Utils.OpenSiteNoDecode(e.Uri.ToString());
        }

        private void IsTwitterLoadingAnim(bool state) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                TwitterProgressRing.IsActive = state;
            }));
        }

        private void IsJoymaxLoadingAnim(bool state) {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate() {
                JoymaxProgressRing.IsActive = state;
            }));
        }

        #endregion Interface processing
    }
}