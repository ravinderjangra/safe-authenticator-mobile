﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Rg.Plugins.Popup.Extensions;
using SafeAuthenticator.Helpers;
using SafeAuthenticator.Models;
using SafeAuthenticator.Native;
using SafeAuthenticator.Services;
using SafeAuthenticator.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthService))]

namespace SafeAuthenticator.Services
{
    public class AuthService : ObservableObject, IDisposable
    {
        private const string AutoReconnectPropKey = "AutoReconnect";
        private readonly SemaphoreSlim _reconnectSemaphore = new SemaphoreSlim(1, 1);
        private Authenticator _authenticator;
        private bool _isLogInitialised;
        private string _secret;
        private string _password;

        public string AuthenticationReq { get; set; }

        internal bool IsLogInitialised
        {
            get => _isLogInitialised;
            private set => SetProperty(ref _isLogInitialised, value);
        }

        private CredentialCacheService CredentialCache { get; }

        internal bool AuthReconnect
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey(AutoReconnectPropKey))
                {
                    Application.Current.Properties[AutoReconnectPropKey] = false;
                    return false;
                }

                return (bool)Application.Current.Properties[AutoReconnectPropKey];
            }

            set
            {
                if (value)
                {
                    StoreCredentials();
                }
                else
                {
                    CredentialCache.Delete();
                }
                Application.Current.Properties[AutoReconnectPropKey] = value;
            }
        }

        public AuthService()
        {
            _isLogInitialised = false;
            CredentialCache = new CredentialCacheService();
            Connectivity.ConnectivityChanged += InternetConnectivityChanged;

            Authenticator.Disconnected += OnNetworkDisconnected;
            Task.Run(async () => await InitLoggingAsync());
        }

        public void Dispose()
        {
            // ReSharper disable once DelegateSubtraction
            Authenticator.Disconnected -= OnNetworkDisconnected;
            FreeState();
            GC.SuppressFinalize(this);
        }

        internal async Task CheckAndReconnect()
        {
            await _reconnectSemaphore.WaitAsync();
            try
            {
                if (_authenticator == null)
                {
                    if (AuthReconnect)
                    {
                        var (location, password) = await CredentialCache.Retrieve();
                        using (UserDialogs.Instance.Loading("Reconnecting to Network"))
                        {
                            await LoginAsync(location, password);
                            MessagingCenter.Send(this, MessengerConstants.NavHomePage);
                        }
                    }
                }
                else if (_authenticator.IsDisconnected)
                {
                    using (UserDialogs.Instance.Loading("Reconnecting to Network"))
                    {
                        await LoginAsync(_secret, _password);
                    }
                }
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to Reconnect: {ex.Message}", "OK");
                FreeState();
                MessagingCenter.Send(this, MessengerConstants.ResetAppViews);
            }
            finally
            {
                _reconnectSemaphore.Release(1);
            }
        }

        internal async Task CreateAccountAsync(string location, string password, string invitation)
        {
            _authenticator = await Authenticator.CreateAccountAsync(location, password, invitation);
            _secret = location;
            _password = password;
        }

        internal async Task<string> RevokeAppAsync(string appId)
        {
            return await _authenticator.AuthRevokeAppAsync(appId);
        }

        ~AuthService()
        {
            FreeState();
        }

        public void FreeState()
        {
            if (_authenticator != null)
            {
                _authenticator.Dispose();
                _authenticator = null;
            }
        }

        internal async Task<(int, int)> GetAccountInfoAsync()
        {
            var acctInfo = await _authenticator.AuthAccountInfoAsync();
            return (Convert.ToInt32(acctInfo.MutationsDone),
                Convert.ToInt32(acctInfo.MutationsDone + acctInfo.MutationsAvailable));
        }

        internal async Task<List<RegisteredAppModel>> GetRegisteredAppsAsync()
        {
            var appList = await _authenticator.AuthRegisteredAppsAsync();
            return appList.Select(app => new RegisteredAppModel(app.AppInfo, app.Containers)).ToList();
        }

        public async Task HandleUrlActivationAsync(string encodedUri)
        {
            try
            {
                if (_authenticator == null && !AuthReconnect)
                {
                    if (await HandleUnregisteredAppRequest(encodedUri))
                        return;
                    AuthenticationReq = encodedUri;
                    await Application.Current.MainPage.DisplayAlert("Login", "An application is requesting access, login to view details", "OK");
                    return;
                }

                await CheckAndReconnect();
                if (await HandleUnregisteredAppRequest(encodedUri))
                    return;

                var encodedReq = UrlFormat.GetRequestData(encodedUri);
                var decodeResult = await _authenticator.DecodeIpcMessageAsync(encodedReq);
                var decodedType = decodeResult.GetType();
                if (decodedType == typeof(IpcReqError))
                {
                    var error = decodeResult as IpcReqError;
                    await Application.Current.MainPage.DisplayAlert("Auth Request", $"Error: {error?.Description}", "Ok");
                }
                else
                {
                    var requestPage = new RequestDetailPage(decodeResult);
                    requestPage.CompleteRequest += async (s, e) =>
                    {
                        var args = e as ResponseEventArgs;
                        await SendResponseBack(encodedUri, decodeResult, args.Response);
                    };

                    MessagingCenter.Send(this, MessengerConstants.NavHomePage);
                    await Application.Current.MainPage.Navigation.PushPopupAsync(requestPage);
                }
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task SendResponseBack(string encodedUri, IpcReq req, bool isGranted)
        {
            try
            {
                string encodedRsp;
                var formattedRsp = string.Empty;
                var requestType = req.GetType();
                if (requestType == typeof(UnregisteredIpcReq))
                {
                    var uauthReq = req as UnregisteredIpcReq;
                    encodedRsp = await Authenticator.EncodeUnregisteredRespAsync(uauthReq.ReqId, isGranted);
                    var appIdFromUrl = UrlFormat.GetAppId(encodedUri);
                    formattedRsp = UrlFormat.Format(appIdFromUrl, encodedRsp, false);
                }
                else if (requestType == typeof(AuthIpcReq))
                {
                    var authReq = req as AuthIpcReq;
                    encodedRsp = await _authenticator.EncodeAuthRespAsync(authReq, isGranted);
                    formattedRsp = UrlFormat.Format(authReq?.AuthReq.App.Id, encodedRsp, false);
                }
                else if (requestType == typeof(ContainersIpcReq))
                {
                    var containerReq = req as ContainersIpcReq;
                    encodedRsp = await _authenticator.EncodeContainersRespAsync(containerReq, isGranted);
                    formattedRsp = UrlFormat.Format(containerReq?.ContainersReq.App.Id, encodedRsp, false);
                }
                else if (requestType == typeof(ShareMDataIpcReq))
                {
                    var mDataShareReq = req as ShareMDataIpcReq;
                    if (!isGranted)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "SharedMData request denied", "OK");
                        return;
                    }
                    encodedRsp = await _authenticator.EncodeShareMdataRespAsync(mDataShareReq, isGranted);
                    formattedRsp = UrlFormat.Format(mDataShareReq?.ShareMDataReq.App.Id, encodedRsp, false);
                }

                Debug.WriteLine($"Encoded Rsp to app: {formattedRsp}");
                Device.BeginInvokeOnMainThread(() => { Device.OpenUri(new Uri(formattedRsp)); });
            }
            catch (FfiException ex)
            {
                var errorMessage = Utilities.GetErrorMessage(ex);
                await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task<bool> HandleUnregisteredAppRequest(string encodedUri)
        {
            var encodedReq = UrlFormat.GetRequestData(encodedUri);
            var udecodeResult = await Authenticator.UnRegisteredDecodeIpcMsgAsync(encodedReq);
            if (udecodeResult.GetType() == typeof(UnregisteredIpcReq))
            {
                var requestPage = new RequestDetailPage(udecodeResult);
                requestPage.CompleteRequest += async (s, e) =>
                {
                    var args = e as ResponseEventArgs;
                    await SendResponseBack(encodedUri, udecodeResult, args.Response);
                };

                await Application.Current.MainPage.Navigation.PushPopupAsync(requestPage);
                return true;
            }
            return false;
        }

        private async Task InitLoggingAsync()
        {
            await Authenticator.AuthInitLoggingAsync(null);

            Debug.WriteLine("Rust Logging Initialised.");
            IsLogInitialised = true;
        }

        internal async Task LoginAsync(string location, string password)
        {
            _authenticator = await Authenticator.LoginAsync(location, password);
            _secret = location;
            _password = password;
        }

        internal async Task LogoutAsync()
        {
            await Task.Run(() =>
            {
                FreeState();
                CredentialCache.Delete();
            });
        }

        private void InternetConnectivityChanged(object obj, EventArgs args)
        {
            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    {
                        return;
                    }
                    await CheckAndReconnect();
                });
        }

        private void OnNetworkDisconnected(object obj, EventArgs args)
        {
            Debug.WriteLine("Network Observer Fired");

            if (obj == null || _authenticator == null || obj as Authenticator != _authenticator)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(
                async () =>
                {
                    if (App.IsBackgrounded)
                    {
                        return;
                    }

                    await CheckAndReconnect();
                });
        }

        private async void StoreCredentials()
        {
            await CredentialCache.Store(_secret, _password);
        }
    }
}
