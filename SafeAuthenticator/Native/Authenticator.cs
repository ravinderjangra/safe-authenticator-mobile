﻿// Copyright 2020 MaidSafe.net limited.
//
// This SAFE Network Software is licensed to you under the MIT license <LICENSE-MIT
// http://opensource.org/licenses/MIT> or the Modified BSD license <LICENSE-BSD
// https://opensource.org/licenses/BSD-3-Clause>, at your option. This file may not be copied,
// modified, or distributed except according to those terms. Please review the Licences for the
// specific language governing permissions and limitations relating to use of the SAFE Network
// Software.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SafeAuthenticator.Helpers;
using Xamarin.Forms;

namespace SafeAuthenticator.Native
{
    public class Authenticator : IDisposable
    {
        private static readonly IAuthBindings AuthBindings = DependencyService.Get<IAuthBindings>();

        // ReSharper disable once UnassignedField.Global
        #pragma warning disable SA1401 // Fields should be private
        public static EventHandler Disconnected;
        #pragma warning restore SA1401 // Fields should be private
        private IntPtr _authPtr;
        private GCHandle _disconnectedHandle;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool IsDisconnected { get; set; }

        [PublicAPI]
        public static async Task AuthInitLoggingAsync(string outputFileName)
        {
            var fileList = new List<(string, string)>
                { ("log.toml", "log.toml") };

            var fileOps = DependencyService.Get<IFileOps>();
            var configDirPath = await AuthBindings.AuthConfigDirPathAsync();
            Debug.WriteLine($"Assets Should be available at: {configDirPath}");

            await fileOps.TransferAssetsAsync(fileList);

            Debug.WriteLine($"Assets Transferred: {fileOps.ConfigFilesPath}");
            await AuthBindings.AuthSetConfigDirPathAsync(fileOps.ConfigFilesPath);
            await AuthBindings.AuthInitLoggingAsync(outputFileName);
        }

        [PublicAPI]
        public static Task AuthSetConfigurationFilePathAsync(string configDirPath)
        {
            return AuthBindings.AuthSetConfigDirPathAsync(configDirPath);
        }

        [PublicAPI]
        public static Task<string> AuthGetConfigFilePathAsync()
        {
            return AuthBindings.AuthConfigDirPathAsync();
        }

        [PublicAPI]
        public static Task<Authenticator> CreateAccountAsync(string locator, string secret)
        {
            return Task.Run(
                () =>
                {
                    var authenticator = new Authenticator();
                    var tcs = new TaskCompletionSource<Authenticator>();
                    Action disconnect = () => { OnDisconnected(authenticator); };
                    Action<FfiResult, IntPtr, GCHandle> cb = (result, ptr, disconnectHandle) =>
                    {
                        if (result.ErrorCode != 0)
                        {
                            if (disconnectHandle.IsAllocated)
                            {
                                disconnectHandle.Free();
                            }

                            tcs.SetException(result.ToException());
                            return;
                        }

                        authenticator.Init(ptr, disconnectHandle);
                        tcs.SetResult(authenticator);
                    };
                    AuthBindings.CreateAccount(locator, secret, disconnect, cb);
                    return tcs.Task;
                });
        }

        [PublicAPI]
        public static Task<string> EncodeUnregisteredRespAsync(uint reqId, bool allow)
        {
            return AuthBindings.EncodeUnregisteredRespAsync(reqId, allow);
        }

        private Authenticator()
        {
            IsDisconnected = false;
            _authPtr = IntPtr.Zero;
        }

        public void Dispose()
        {
            FreeAuth();
            GC.SuppressFinalize(this);
        }

        [PublicAPI]
        public Task<List<AppAccess>> AuthAppsAccessingMutableDataAsync(byte[] name, ulong typeTag)
        {
            return AuthBindings.AuthAppsAccessingMutableDataAsync(_authPtr, name, typeTag);
        }

        [PublicAPI]
        public Task AuthFlushAppRevocationQueueAsync()
        {
            return AuthBindings.AuthFlushAppRevocationQueueAsync(_authPtr);
        }

        [PublicAPI]
        public Task AuthReconnectAsync()
        {
            return AuthBindings.AuthReconnectAsync(_authPtr);
        }

        [PublicAPI]
        public Task<List<RegisteredApp>> AuthRegisteredAppsAsync()
        {
            return AuthBindings.AuthRegisteredAppsAsync(_authPtr);
        }

        [PublicAPI]
        public Task<string> AuthRevokeAppAsync(string appId)
        {
            return AuthBindings.AuthRevokeAppAsync(_authPtr, appId);
        }

        [PublicAPI]
        public Task<List<AppExchangeInfo>> AuthRevokedAppsAsync()
        {
            return AuthBindings.AuthRevokedAppsAsync(_authPtr);
        }

        [PublicAPI]
        public Task AuthRmRevokedAppAsync(string appId)
        {
            return AuthBindings.AuthRmRevokedAppAsync(_authPtr, appId);
        }

        [PublicAPI]
        public Task<IpcReq> DecodeIpcMessageAsync(string msg)
        {
            return AuthBindings.DecodeIpcMessage(_authPtr, msg);
        }

        [PublicAPI]
        public Task<string> EncodeAuthRespAsync(AuthIpcReq authIpcReq, bool allow)
        {
            return AuthBindings.EncodeAuthRespAsync(_authPtr, ref authIpcReq.AuthReq, authIpcReq.ReqId, allow);
        }

        [PublicAPI]
        public Task<string> EncodeContainersRespAsync(ContainersIpcReq req, bool allow)
        {
            return AuthBindings.EncodeContainersRespAsync(_authPtr, ref req.ContainersReq, req.ReqId, allow);
        }

        [PublicAPI]
        public Task<string> EncodeShareMdataRespAsync(ShareMDataIpcReq req, bool allow)
        {
            return AuthBindings.EncodeShareMDataRespAsync(_authPtr, ref req.ShareMDataReq, req.ReqId, allow);
        }

        ~Authenticator()
        {
            FreeAuth();
        }

        private void FreeAuth()
        {
            if (_disconnectedHandle.IsAllocated)
            {
                _disconnectedHandle.Free();
            }

            if (_authPtr == IntPtr.Zero)
            {
                return;
            }

            IsDisconnected = false;
            AuthBindings.AuthFree(_authPtr);
            _authPtr = IntPtr.Zero;
        }

        private void Init(IntPtr authPtr, GCHandle disconnectedHandle)
        {
            _authPtr = authPtr;
            _disconnectedHandle = disconnectedHandle;
            IsDisconnected = false;
        }

        [PublicAPI]
        public static bool IsMockBuild()
        {
            return AuthBindings.AuthIsMock();
        }

        [PublicAPI]
        public static Task<Authenticator> LoginAsync(string locator, string secret)
        {
            return Task.Run(
                () =>
                {
                    var authenticator = new Authenticator();
                    var tcs = new TaskCompletionSource<Authenticator>();
                    Action disconnect = () => { OnDisconnected(authenticator); };
                    Action<FfiResult, IntPtr, GCHandle> cb = (result, ptr, disconnectHandle) =>
                    {
                        if (result.ErrorCode != 0)
                        {
                            if (disconnectHandle.IsAllocated)
                            {
                                disconnectHandle.Free();
                            }

                            tcs.SetException(result.ToException());
                            return;
                        }

                        authenticator.Init(ptr, disconnectHandle);
                        tcs.SetResult(authenticator);
                    };
                    AuthBindings.Login(locator, secret, disconnect, cb);
                    return tcs.Task;
                });
        }

        private static void OnDisconnected(Authenticator authenticator)
        {
            authenticator.IsDisconnected = true;
            Disconnected?.Invoke(authenticator, EventArgs.Empty);
        }

        [PublicAPI]
        public static Task<IpcReq> UnRegisteredDecodeIpcMsgAsync(string msg)
        {
            return AuthBindings.UnRegisteredDecodeIpcMsgAsync(msg);
        }
    }
}
