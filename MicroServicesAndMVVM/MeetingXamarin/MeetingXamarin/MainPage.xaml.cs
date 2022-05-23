using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using GrpcCommon;
using MeetingCommon.Abstractions;
using MeetingGrpcClient;
using System;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MeetingXamarin
{
    public partial class MainPage : ContentPage
    {
        private readonly MeetingServiceAbstract _meetingServiceAbstract;


        public static string IPAddress = DeviceInfo.Platform == DevicePlatform.Android
        ? "10.0.2.2"
		: "localhost";

        public static string BaseUri = $"https://{IPAddress}:7129";

        public MainPage()
        {
            InitializeComponent();

            //android:networkSecurityConfig="@network_security_config"

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            try
            {
                GrpcChannel ToAuthChannel(HttpClient httpClient, string baseUri) =>
                    GrpcChannel.ForAddress(baseUri,
                        new GrpcChannelOptions
                        {
                            HttpClient = httpClient,
                            Credentials = ChannelCredentials.SecureSsl
                        });


                var httpClientHandler = new HttpClientHandler();

                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (cert.Issuer.Equals("CN=localhost"))
                        return true;
                    return errors == System.Net.Security.SslPolicyErrors.None;
                };

                var grpcChannel = GrpcChannel.ForAddress(BaseUri, new GrpcChannelOptions { HttpHandler = httpClientHandler });


				var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, httpClientHandler);
				var gpTest = new HttpClient(handler);


				_meetingServiceAbstract = new MeetingService(new Meeting.MeetingClient(ToAuthChannel(gpTest, BaseUri)));
                _meetingServiceAbstract.Connect("limeniye_mobile");
            }
            catch(Exception ex)
            {

				/*
				 * {Grpc.Core.RpcException: Status(StatusCode="Internal", Detail="Error starting gRPC call. WebException: Failed to connect to localhost/127.0.0.1:7129 ConnectException: Failed to connect to localhost/127.0.0.1:7129", DebugException="System.Net.WebException: Failed to connect to localhost/127.0.0.1:7129 ---> Java.Net.ConnectException: Failed to connect to localhost/127.0.0.1:7129
  at Java.Interop.JniEnvironment+InstanceMethods.CallVoidMethod (Java.Interop.JniObjectReference instance, Java.Interop.JniMethodInfo method, Java.Interop.JniArgumentValue* args) [0x00068] in /Users/runner/work/1/s/xamarin-android/external/Java.Interop/src/Java.Interop/Java.Interop/JniEnvironment.g.cs:11884 
  at Java.Interop.JniPeerMembers+JniInstanceMethods.InvokeAbstractVoidMethod (System.String encodedMember, Java.Interop.IJavaPeerable self, Java.Interop.JniArgumentValue* parameters) [0x0000e] in /Users/runner/work/1/s/xamarin-android/external/Java.Interop/src/Java.Interop/Java.Interop/JniPeerMembers.JniInstanceMethods_Invoke.cs:17 
  at Javax.Net.Ssl.HttpsURLConnectionInvoker.Connect () [0x00000] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/obj/Release/monoandroid10/android-31/mcw/Javax.Net.Ssl.HttpsURLConnection.cs:433 
  at Xamarin.Android.Net.AndroidClientHandler+<>c__DisplayClass44_0.<ConnectAsync>b__0 () [0x0007d] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidClientHandler.Legacy.cs:356 
  at System.Threading.Tasks.Task.InnerInvoke () [0x0000f] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corert/src/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs:2476 
  at System.Threading.Tasks.Task.Execute () [0x00000] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corert/src/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs:2319 
--- End of stack trace from previous location where exception was thrown ---

  at Xamarin.Android.Net.AndroidClientHandler.DoProcessRequest (System.Net.Http.HttpRequestMessage request, Java.Net.URL javaUrl, Java.Net.HttpURLConnection httpConnection, System.Threading.CancellationToken cancellationToken, Xamarin.Android.Net.AndroidClientHandler+RequestRedirectionState redirectState) [0x000e4] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidClientHandler.Legacy.cs:405 
   --- End of inner exception stack trace ---
  at Xamarin.Android.Net.AndroidClientHandler.DoProcessRequest (System.Net.Http.HttpRequestMessage request, Java.Net.URL javaUrl, Java.Net.HttpURLConnection httpConnection, System.Threading.CancellationToken cancellationToken, Xamarin.Android.Net.AndroidClientHandler+RequestRedirectionState redirectState) [0x0016e] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidClientHandler.Legacy.cs:412 
  at Xamarin.Android.Net.AndroidClientHandler.SendAsync (System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) [0x00286] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidClientHandler.Legacy.cs:287 
  at System.Net.Http.HttpClient.FinishSendAsyncUnbuffered (System.Threading.Tasks.Task`1[TResult] sendTask, System.Net.Http.HttpRequestMessage request, System.Threading.CancellationTokenSource cts, System.Boolean disposeCts) [0x000b3] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/HttpClient.cs:531 
  at Grpc.Net.Client.Internal.GrpcCall`2[TRequest,TResponse].RunCall (System.Net.Http.HttpRequestMessage request, System.Nullable`1[T] timeout) [0x00215] in /_/src/Grpc.Net.Client/Internal/GrpcCall.cs:493 ")
  at Grpc.Net.Client.Internal.HttpClientCallInvoker.BlockingUnaryCall[TRequest,TResponse] (Grpc.Core.Method`2[TRequest,TResponse] method, System.String host, Grpc.Core.CallOptions options, TRequest request) [0x0000b] in /_/src/Grpc.Net.Client/Internal/HttpClientCallInvoker.cs:116 
  at Grpc.Core.Interceptors.InterceptingCallInvoker.<BlockingUnaryCall>b__3_0[TRequest,TResponse] (TRequest req, Grpc.Core.Interceptors.ClientInterceptorContext`2[TRequest,TResponse] ctx) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:51 
  at Grpc.Core.ClientBase+ClientBaseConfiguration+ClientBaseConfigurationInterceptor.BlockingUnaryCall[TRequest,TResponse] (TRequest request, Grpc.Core.Interceptors.ClientInterceptorContext`2[TRequest,TResponse] context, Grpc.Core.Interceptors.Interceptor+BlockingUnaryCallContinuation`2[TRequest,TResponse] continuation) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/ClientBase.cs:174 
  at Grpc.Core.Interceptors.InterceptingCallInvoker.BlockingUnaryCall[TRequest,TResponse] (Grpc.Core.Method`2[TRequest,TResponse] method, System.String host, Grpc.Core.CallOptions options, TRequest request) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:48 
  at GrpcCommon.Meeting+MeetingClient.Connect (GrpcCommon.ConnectRequest request, Grpc.Core.CallOptions options) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:178 
  at GrpcCommon.Meeting+MeetingClient.Connect (GrpcCommon.ConnectRequest request, Grpc.Core.Metadata headers, System.Nullable`1[T] deadline, System.Threading.CancellationToken cancellationToken) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:173 
  at MeetingGrpcClient.MeetingService.Connect (System.String username) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingGrpcClient\MeetingService.cs:88 
  at MeetingXamarin.MainPage..ctor () [0x0006a] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingXamarin\MeetingXamarin\MainPage.xaml.cs:57 }
				 */


				/*
                 * {Grpc.Core.RpcException: Status(StatusCode="Internal", Detail="Error starting gRPC call. SSLHandshakeException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found. CertificateException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found. CertPathValidatorException: Trust anchor for certification path not found.", DebugException="Javax.Net.Ssl.SSLHandshakeException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found. ---> Java.Security.Cert.CertificateException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found. ---> Java.Security.Cert.CertPathValidatorException: Trust anchor for certification path not found.
   --- End of inner exception stack trace ---
   --- End of inner exception stack trace ---
  at Java.Interop.JniEnvironment+InstanceMethods.CallVoidMethod (Java.Interop.JniObjectReference instance, Java.Interop.JniMethodInfo method, Java.Interop.JniArgumentValue* args) [0x00068] in /Users/runner/work/1/s/xamarin-android/external/Java.Interop/src/Java.Interop/Java.Interop/JniEnvironment.g.cs:11884 
  at Java.Interop.JniPeerMembers+JniInstanceMethods.InvokeAbstractVoidMethod (System.String encodedMember, Java.Interop.IJavaPeerable self, Java.Interop.JniArgumentValue* parameters) [0x0000e] in /Users/runner/work/1/s/xamarin-android/external/Java.Interop/src/Java.Interop/Java.Interop/JniPeerMembers.JniInstanceMethods_Invoke.cs:17 
  at Javax.Net.Ssl.HttpsURLConnectionInvoker.Connect () [0x00000] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/obj/Release/monoandroid10/android-31/mcw/Javax.Net.Ssl.HttpsURLConnection.cs:433 
  at Xamarin.Android.Net.AndroidClientHandler+<>c__DisplayClass44_0.<ConnectAsync>b__0 () [0x0007d] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidClientHandler.Legacy.cs:356 
  at System.Threading.Tasks.Task.InnerInvoke () [0x0000f] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corert/src/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs:2476 
  at System.Threading.Tasks.Task.Execute () [0x00000] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corert/src/System.Private.CoreLib/src/System/Threading/Tasks/Task.cs:2319 
--- End of stack trace from previous location where exception was thrown ---

  at Xamarin.Android.Net.AndroidClientHandler.DoProcessRequest (System.Net.Http.HttpRequestMessage request, Java.Net.URL javaUrl, Java.Net.HttpURLConnection httpConnection, System.Threading.CancellationToken cancellationToken, Xamarin.Android.Net.AndroidClientHandler+RequestRedirectionState redirectState) [0x000e4] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidClientHandler.Legacy.cs:405 
  at Xamarin.Android.Net.AndroidClientHandler.SendAsync (System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) [0x00286] in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidClientHandler.Legacy.cs:287 
  at System.Net.Http.HttpClient.FinishSendAsyncUnbuffered (System.Threading.Tasks.Task`1[TResult] sendTask, System.Net.Http.HttpRequestMessage request, System.Threading.CancellationTokenSource cts, System.Boolean disposeCts) [0x000b3] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/HttpClient.cs:531 
  at Grpc.Net.Client.Internal.GrpcCall`2[TRequest,TResponse].RunCall (System.Net.Http.HttpRequestMessage request, System.Nullable`1[T] timeout) [0x00215] in /_/src/Grpc.Net.Client/Internal/GrpcCall.cs:493 
  --- End of managed Javax.Net.Ssl.SSLHandshakeException stack trace ---
javax.net.ssl.SSLHandshakeException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
	at com.android.org.conscrypt.SSLUtils.toSSLHandshakeException(SSLUtils.java:363)
	at com.android.org.conscrypt.ConscryptEngine.convertException(ConscryptEngine.java:1134)
	at com.android.org.conscrypt.ConscryptEngine.readPlaintextData(ConscryptEngine.java:1089)
	at com.android.org.conscrypt.ConscryptEngine.unwrap(ConscryptEngine.java:876)
	at com.android.org.conscrypt.ConscryptEngine.unwrap(ConscryptEngine.java:747)
	at com.android.org.conscrypt.ConscryptEngine.unwrap(ConscryptEngine.java:712)
	at com.android.org.conscrypt.ConscryptEngineSocket$SSLInputStream.processDataFromSocket(ConscryptEngineSocket.java:858)
	at com.android.org.conscrypt.ConscryptEngineSocket$SSLInputStream.access$100(ConscryptEngineSocket.java:731)
	at com.android.org.conscrypt.ConscryptEngineSocket.doHandshake(ConscryptEngineSocket.java:241)
	at com.android.org.conscrypt.ConscryptEngineSocket.startHandshake(ConscryptEngineSocket.java:220)
	at com.android.okhttp.internal.io.RealConnection.connectTls(RealConnection.java:196)
	at com.android.okhttp.internal.io.RealConnection.connectSocket(RealConnection.java:153)
	at com.android.okhttp.internal.io.RealConnection.connect(RealConnection.java:116)
	at com.android.okhttp.internal.http.StreamAllocation.findConnection(StreamAllocation.java:186)
	at com.android.okhttp.internal.http.StreamAllocation.findHealthyConnection(StreamAllocation.java:128)
	at com.android.okhttp.internal.http.StreamAllocation.newStream(StreamAllocation.java:97)
	at com.android.okhttp.internal.http.HttpEngine.connect(HttpEngine.java:289)
	at com.android.okhttp.internal.http.HttpEngine.sendRequest(HttpEngine.java:232)
	at com.android.okhttp.internal.huc.HttpURLConnectionImpl.execute(HttpURLConnectionImpl.java:465)
	at com.android.okhttp.internal.huc.HttpURLConnectionImpl.connect(HttpURLConnectionImpl.java:131)
	at com.android.okhttp.internal.huc.DelegatingHttpsURLConnection.connect(DelegatingHttpsURLConnection.java:90)
	at com.android.okhttp.internal.huc.HttpsURLConnectionImpl.connect(HttpsURLConnectionImpl.java:30)
Caused by: java.security.cert.CertificateException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
	at com.android.org.conscrypt.TrustManagerImpl.verifyChain(TrustManagerImpl.java:672)
	at com.android.org.conscrypt.TrustManagerImpl.checkTrustedRecursive(TrustManagerImpl.java:549)
	at com.android.org.conscrypt.TrustManagerImpl.checkTrusted(TrustManagerImpl.java:505)
	at com.android.org.conscrypt.TrustManagerImpl.checkTrusted(TrustManagerImpl.java:425)
	at com.android.org.conscrypt.TrustManagerImpl.getTrustedChainForServer(TrustManagerImpl.java:353)
	at android.security.net.config.NetworkSecurityTrustManager.checkServerTrusted(NetworkSecurityTrustManager.java:94)
	at android.security.net.config.RootTrustManager.checkServerTrusted(RootTrustManager.java:90)
	at com.android.org.conscrypt.ConscryptEngineSocket$2.checkServerTrusted(ConscryptEngineSocket.java:163)
	at com.android.org.conscrypt.Platform.checkServerTrusted(Platform.java:255)
	at com.android.org.conscrypt.ConscryptEngine.verifyCertificateChain(ConscryptEngine.java:1638)
	at com.android.org.conscrypt.NativeCrypto.ENGINE_SSL_read_direct(Native Method)
	at com.android.org.conscrypt.NativeSsl.readDirectByteBuffer(NativeSsl.java:569)
	at com.android.org.conscrypt.ConscryptEngine.readPlaintextDataDirect(ConscryptEngine.java:1095)
	at com.android.org.conscrypt.ConscryptEngine.readPlaintextData(ConscryptEngine.java:1079)
	... 19 more
Caused by: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
	... 33 more
")
  at Grpc.Net.Client.Internal.HttpClientCallInvoker.BlockingUnaryCall[TRequest,TResponse] (Grpc.Core.Method`2[TRequest,TResponse] method, System.String host, Grpc.Core.CallOptions options, TRequest request) [0x0000b] in /_/src/Grpc.Net.Client/Internal/HttpClientCallInvoker.cs:116 
  at Grpc.Core.Interceptors.InterceptingCallInvoker.<BlockingUnaryCall>b__3_0[TRequest,TResponse] (TRequest req, Grpc.Core.Interceptors.ClientInterceptorContext`2[TRequest,TResponse] ctx) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:51 
  at Grpc.Core.ClientBase+ClientBaseConfiguration+ClientBaseConfigurationInterceptor.BlockingUnaryCall[TRequest,TResponse] (TRequest request, Grpc.Core.Interceptors.ClientInterceptorContext`2[TRequest,TResponse] context, Grpc.Core.Interceptors.Interceptor+BlockingUnaryCallContinuation`2[TRequest,TResponse] continuation) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/ClientBase.cs:174 
  at Grpc.Core.Interceptors.InterceptingCallInvoker.BlockingUnaryCall[TRequest,TResponse] (Grpc.Core.Method`2[TRequest,TResponse] method, System.String host, Grpc.Core.CallOptions options, TRequest request) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:48 
  at GrpcCommon.Meeting+MeetingClient.Connect (GrpcCommon.ConnectRequest request, Grpc.Core.CallOptions options) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:178 
  at GrpcCommon.Meeting+MeetingClient.Connect (GrpcCommon.ConnectRequest request, Grpc.Core.Metadata headers, System.Nullable`1[T] deadline, System.Threading.CancellationToken cancellationToken) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:173 
  at MeetingGrpcClient.MeetingService.Connect (System.String username) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingGrpcClient\MeetingService.cs:88 
  at MeetingXamarin.MainPage..ctor () [0x0006a] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingXamarin\MeetingXamarin\MainPage.xaml.cs:57 }
                 */



				/*
                 * {Grpc.Core.RpcException: Status(StatusCode="Internal", Detail="Content-Type 'application/grpc-web' is not supported.")
  at Grpc.Net.Client.Internal.HttpClientCallInvoker.BlockingUnaryCall[TRequest,TResponse] (Grpc.Core.Method`2[TRequest,TResponse] method, System.String host, Grpc.Core.CallOptions options, TRequest request) [0x0000b] in /_/src/Grpc.Net.Client/Internal/HttpClientCallInvoker.cs:116 
  at Grpc.Core.Interceptors.InterceptingCallInvoker.<BlockingUnaryCall>b__3_0[TRequest,TResponse] (TRequest req, Grpc.Core.Interceptors.ClientInterceptorContext`2[TRequest,TResponse] ctx) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:51 
  at Grpc.Core.ClientBase+ClientBaseConfiguration+ClientBaseConfigurationInterceptor.BlockingUnaryCall[TRequest,TResponse] (TRequest request, Grpc.Core.Interceptors.ClientInterceptorContext`2[TRequest,TResponse] context, Grpc.Core.Interceptors.Interceptor+BlockingUnaryCallContinuation`2[TRequest,TResponse] continuation) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/ClientBase.cs:174 
  at Grpc.Core.Interceptors.InterceptingCallInvoker.BlockingUnaryCall[TRequest,TResponse] (Grpc.Core.Method`2[TRequest,TResponse] method, System.String host, Grpc.Core.CallOptions options, TRequest request) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:48 
  at GrpcCommon.Meeting+MeetingClient.Connect (GrpcCommon.ConnectRequest request, Grpc.Core.CallOptions options) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:178 
  at GrpcCommon.Meeting+MeetingClient.Connect (GrpcCommon.ConnectRequest request, Grpc.Core.Metadata headers, System.Nullable`1[T] deadline, System.Threading.CancellationToken cancellationToken) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:173 
  at MeetingGrpcClient.MeetingService.Connect (System.String username) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingGrpcClient\MeetingService.cs:88 
  at MeetingXamarin.MainPage..ctor () [0x00071] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingXamarin\MeetingXamarin\MainPage.xaml.cs:57 }
                 */



				/*
                 * {Grpc.Core.RpcException: Status(StatusCode="Unavailable", Detail="Error starting gRPC call. HttpRequestException: The SSL connection could not be established, see inner exception. IOException: Remote prematurely closed connection.", DebugException="System.Net.Http.HttpRequestException: The SSL connection could not be established, see inner exception. ---> System.IO.IOException: Remote prematurely closed connection.
  at Mono.Net.Security.AsyncProtocolRequest.ProcessOperation (System.Threading.CancellationToken cancellationToken) [0x000e3] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/mcs/class/System/Mono.Net.Security/AsyncProtocolRequest.cs:211 
  at Mono.Net.Security.AsyncProtocolRequest.StartOperation (System.Threading.CancellationToken cancellationToken) [0x00046] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/mcs/class/System/Mono.Net.Security/AsyncProtocolRequest.cs:187 
  at Mono.Net.Security.MobileAuthenticatedStream.ProcessAuthentication (System.Boolean runSynchronously, Mono.Net.Security.MonoSslAuthenticationOptions options, System.Threading.CancellationToken cancellationToken) [0x0025c] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/mcs/class/System/Mono.Net.Security/MobileAuthenticatedStream.cs:310 
  at System.Net.Http.ConnectHelper.EstablishSslConnectionAsyncCore (System.IO.Stream stream, System.Net.Security.SslClientAuthenticationOptions sslOptions, System.Threading.CancellationToken cancellationToken) [0x0007b] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/ConnectHelper.cs:165 
   --- End of inner exception stack trace ---
  at System.Net.Http.ConnectHelper.EstablishSslConnectionAsyncCore (System.IO.Stream stream, System.Net.Security.SslClientAuthenticationOptions sslOptions, System.Threading.CancellationToken cancellationToken) [0x000f6] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/ConnectHelper.cs:176 
  at System.Threading.Tasks.ValueTask`1[TResult].get_Result () [0x0001b] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/Common/src/CoreLib/System/Threading/Tasks/ValueTask.cs:813 
  at System.Net.Http.HttpConnectionPool.CreateConnectionAsync (System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) [0x002d8] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/HttpConnectionPool.cs:408 
  at System.Threading.Tasks.ValueTask`1[TResult].get_Result () [0x0001b] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/Common/src/CoreLib/System/Threading/Tasks/ValueTask.cs:813 
  at System.Net.Http.HttpConnectionPool.WaitForCreatedConnectionAsync (System.Threading.Tasks.ValueTask`1[TResult] creationTask) [0x000a2] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/HttpConnectionPool.cs:543 
  at System.Threading.Tasks.ValueTask`1[TResult].get_Result () [0x0001b] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/Common/src/CoreLib/System/Threading/Tasks/ValueTask.cs:813 
  at System.Net.Http.HttpConnectionPool.SendWithRetryAsync (System.Net.Http.HttpRequestMessage request, System.Boolean doRequestAuth, System.Threading.CancellationToken cancellationToken) [0x0003f] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/HttpConnectionPool.cs:284 
  at System.Net.Http.RedirectHandler.SendAsync (System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) [0x00070] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/RedirectHandler.cs:32 
  at Grpc.Net.Client.Web.GrpcWebHandler.SendAsyncCore (System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) [0x000c5] in /_/src/Grpc.Net.Client.Web/GrpcWebHandler.cs:166 
  at System.Net.Http.HttpClient.FinishSendAsyncUnbuffered (System.Threading.Tasks.Task`1[TResult] sendTask, System.Net.Http.HttpRequestMessage request, System.Threading.CancellationTokenSource cts, System.Boolean disposeCts) [0x000b3] in /Users/builder/jenkins/workspace/archive-mono/2020-02/android/release/external/corefx/src/System.Net.Http/src/System/Net/Http/HttpClient.cs:531 
  at Grpc.Net.Client.Internal.GrpcCall`2[TRequest,TResponse].RunCall (System.Net.Http.HttpRequestMessage request, System.Nullable`1[T] timeout) [0x00215] in /_/src/Grpc.Net.Client/Internal/GrpcCall.cs:493 ")
  at Grpc.Net.Client.Internal.HttpClientCallInvoker.BlockingUnaryCall[TRequest,TResponse] (Grpc.Core.Method`2[TRequest,TResponse] method, System.String host, Grpc.Core.CallOptions options, TRequest request) [0x0000b] in /_/src/Grpc.Net.Client/Internal/HttpClientCallInvoker.cs:116 
  at Grpc.Core.Interceptors.InterceptingCallInvoker.<BlockingUnaryCall>b__3_0[TRequest,TResponse] (TRequest req, Grpc.Core.Interceptors.ClientInterceptorContext`2[TRequest,TResponse] ctx) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:51 
  at Grpc.Core.ClientBase+ClientBaseConfiguration+ClientBaseConfigurationInterceptor.BlockingUnaryCall[TRequest,TResponse] (TRequest request, Grpc.Core.Interceptors.ClientInterceptorContext`2[TRequest,TResponse] context, Grpc.Core.Interceptors.Interceptor+BlockingUnaryCallContinuation`2[TRequest,TResponse] continuation) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/ClientBase.cs:174 
  at Grpc.Core.Interceptors.InterceptingCallInvoker.BlockingUnaryCall[TRequest,TResponse] (Grpc.Core.Method`2[TRequest,TResponse] method, System.String host, Grpc.Core.CallOptions options, TRequest request) [0x00000] in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:48 
  at GrpcCommon.Meeting+MeetingClient.Connect (GrpcCommon.ConnectRequest request, Grpc.Core.CallOptions options) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:178 
  at GrpcCommon.Meeting+MeetingClient.Connect (GrpcCommon.ConnectRequest request, Grpc.Core.Metadata headers, System.Nullable`1[T] deadline, System.Threading.CancellationToken cancellationToken) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:173 
  at MeetingGrpcClient.MeetingService.Connect (System.String username) [0x00001] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingGrpcClient\MeetingService.cs:88 
  at MeetingXamarin.MainPage..ctor () [0x00065] in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingXamarin\MeetingXamarin\MainPage.xaml.cs:56 }
                 */
			}
		}
    }
}
