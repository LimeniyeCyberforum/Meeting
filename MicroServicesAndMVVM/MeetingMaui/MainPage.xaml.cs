using Grpc.Core;
using Grpc.Net.Client;
#if ANDROID
using Grpc.Net.Client.Web;
#endif
using GrpcCommon;
using MeetingCommon.Abstractions;
using MeetingCommon.DataTypes;
using MeetingGrpcClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MeetingMaui;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private readonly MeetingServiceAbstract _meetingServiceAbstract;
    private UserDto _currentUser;

    private string _message = "Kek";
    public string Message { get => _message; set => SetPropertyValue(ref _message, value); }

    public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>()
    {
        new Message(Guid.NewGuid(), "Hello world!", DateTime.UtcNow)
    };


    public static string IPAddress = DeviceInfo.Platform == DevicePlatform.Android
    ? "10.0.2.2"
    : "localhost";

    public static string BaseUri = $"https://{IPAddress}:7129";



    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
		AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

		try
		{
			//            var httpClientHandler = new SocketsHttpHandler();
			//            var grpcChannelOptions = new GrpcChannelOptions
			//            {
			//#if ANDROID
			//                HttpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpClientHandler)),
			//#endif
			//                Credentials = new SslCredentials()
			//            };
			//            var grpcChannel =  GrpcChannel.ForAddress(BaseUri, grpcChannelOptions);

			//            var meetingClient = new Meeting.MeetingClient(grpcChannel);


			var httpClientHandler = new SocketsHttpHandler();
			HttpClient httpClient = new HttpClient();

#if ANDROID
			httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpClientHandler));
#endif
			var grpcChannel = GrpcChannel.ForAddress(BaseUri,
				new GrpcChannelOptions
				{
					HttpClient = httpClient,
					Credentials = new SslCredentials()
				});



			_meetingServiceAbstract = new MeetingService(new Meeting.MeetingClient(grpcChannel));
			_meetingServiceAbstract.Connect("limeniye_mobile");

			//_meetingServiceAbstract.ConnectionStateChanged += OnConnectionStateChanged;
            //_meetingServiceAbstract.Connect("limeniye_mobile");
        }
        catch(Exception ex)
        {
			/*
             * {Grpc.Core.RpcException: Status(StatusCode="Internal", Detail="Error starting gRPC call. HttpRequestException: The SSL connection could not be established, see inner exception. AuthenticationException: Authentication failed, see inner exception. SslException: Exception of type 'Interop+AndroidCrypto+SslException' was thrown.", DebugException="System.Net.Http.HttpRequestException: The SSL connection could not be established, see inner exception.
 ---> System.Security.Authentication.AuthenticationException: Authentication failed, see inner exception.
 ---> Interop+AndroidCrypto+SslException: Exception of type 'Interop+AndroidCrypto+SslException' was thrown.
   --- End of inner exception stack trace ---
   at System.Net.Security.SslStream.<ForceAuthenticationAsync>d__173`1[[System.Net.Security.AsyncReadWriteAdapter, System.Net.Security, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]].MoveNext()
   at System.Net.Http.ConnectHelper.EstablishSslConnectionAsync(SslClientAuthenticationOptions sslOptions, HttpRequestMessage request, Boolean async, Stream stream, CancellationToken cancellationToken)
   --- End of inner exception stack trace ---
   at System.Net.Http.ConnectHelper.EstablishSslConnectionAsync(SslClientAuthenticationOptions sslOptions, HttpRequestMessage request, Boolean async, Stream stream, CancellationToken cancellationToken)
   at System.Net.Http.HttpConnectionPool.ConnectAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
   at System.Net.Http.HttpConnectionPool.AddHttp2ConnectionAsync(HttpRequestMessage request)
   at System.Threading.Tasks.TaskCompletionSourceWithCancellation`1.<WaitWithCancellationAsync>d__1[[System.Net.Http.Http2Connection, System.Net.Http, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]].MoveNext()
   at System.Net.Http.HttpConnectionPool.GetHttp2ConnectionAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
   at System.Net.Http.HttpConnectionPool.SendWithVersionDetectionAndRetryAsync(HttpRequestMessage request, Boolean async, Boolean doRequestAuth, CancellationToken cancellationToken)
   at System.Net.Http.RedirectHandler.SendAsync(HttpRequestMessage request, Boolean async, CancellationToken cancellationToken)
   at Grpc.Net.Client.Balancer.Internal.BalancerHttpHandler.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Grpc.Net.Client/Balancer/Internal/BalancerHttpHandler.cs:line 134
   at Grpc.Net.Client.Internal.GrpcCall`2.<RunCall>d__73[[GrpcCommon.ConnectRequest, GrpcCommon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[GrpcCommon.ConnectResponse, GrpcCommon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].MoveNext() in /_/src/Grpc.Net.Client/Internal/GrpcCall.cs:line 493")
   at Grpc.Net.Client.Internal.HttpClientCallInvoker.BlockingUnaryCall[ConnectRequest,ConnectResponse](Method`2 method, String host, CallOptions options, ConnectRequest request) in /_/src/Grpc.Net.Client/Internal/HttpClientCallInvoker.cs:line 116
   at Grpc.Core.Interceptors.InterceptingCallInvoker.<BlockingUnaryCall>b__3_0[ConnectRequest,ConnectResponse](ConnectRequest req, ClientInterceptorContext`2 ctx) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:line 51
   at Grpc.Core.ClientBase.ClientBaseConfiguration.ClientBaseConfigurationInterceptor.BlockingUnaryCall[ConnectRequest,ConnectResponse](ConnectRequest request, ClientInterceptorContext`2 context, BlockingUnaryCallContinuation`2 continuation) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/ClientBase.cs:line 174
   at Grpc.Core.Interceptors.InterceptingCallInvoker.BlockingUnaryCall[ConnectRequest,ConnectResponse](Method`2 method, String host, CallOptions options, ConnectRequest request) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:line 48
   at GrpcCommon.Meeting.MeetingClient.Connect(ConnectRequest request, CallOptions options) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:line 178
   at GrpcCommon.Meeting.MeetingClient.Connect(ConnectRequest request, Metadata headers, Nullable`1 deadline, CancellationToken cancellationToken) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:line 174
   at MeetingGrpcClient.MeetingService.Connect(String username) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingGrpcClient\MeetingService.cs:line 102
   at MeetingMaui.MainPage..ctor() in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingMaui\MainPage.xaml.cs:line 59}
            */





			/* localhost
             * {Grpc.Core.RpcException: Status(StatusCode="Unavailable", Detail="Error connecting to subchannel.", DebugException="System.Net.Sockets.SocketException (111): Connection refused
   at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.ThrowException(SocketError error, CancellationToken cancellationToken)
   at System.Net.Sockets.Socket.AwaitableSocketAsyncEventArgs.System.Threading.Tasks.Sources.IValueTaskSource.GetResult(Int16 token)
   at System.Net.Sockets.Socket.<ConnectAsync>g__WaitForConnectWithCancellation|277_0(AwaitableSocketAsyncEventArgs saea, ValueTask connectTask, CancellationToken cancellationToken)
   at Grpc.Net.Client.Balancer.Internal.SocketConnectivitySubchannelTransport.TryConnectAsync(CancellationToken cancellationToken) in /_/src/Grpc.Net.Client/Balancer/Internal/SocketConnectivitySubchannelTransport.cs:line 138")
   at Grpc.Net.Client.Balancer.Internal.ConnectionManager.PickAsync(PickContext context, Boolean waitForReady, CancellationToken cancellationToken) in /_/src/Grpc.Net.Client/Balancer/Internal/ConnectionManager.cs:line 352
   at Grpc.Net.Client.Balancer.Internal.BalancerHttpHandler.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Grpc.Net.Client/Balancer/Internal/BalancerHttpHandler.cs:line 87
   at Grpc.Net.Client.Internal.GrpcCall`2.<RunCall>d__73[[GrpcCommon.ConnectRequest, GrpcCommon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[GrpcCommon.ConnectResponse, GrpcCommon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].MoveNext() in /_/src/Grpc.Net.Client/Internal/GrpcCall.cs:line 493
   at Grpc.Net.Client.Internal.HttpClientCallInvoker.BlockingUnaryCall[ConnectRequest,ConnectResponse](Method`2 method, String host, CallOptions options, ConnectRequest request) in /_/src/Grpc.Net.Client/Internal/HttpClientCallInvoker.cs:line 116
   at Grpc.Core.Interceptors.InterceptingCallInvoker.<BlockingUnaryCall>b__3_0[ConnectRequest,ConnectResponse](ConnectRequest req, ClientInterceptorContext`2 ctx) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:line 51
   at Grpc.Core.ClientBase.ClientBaseConfiguration.ClientBaseConfigurationInterceptor.BlockingUnaryCall[ConnectRequest,ConnectResponse](ConnectRequest request, ClientInterceptorContext`2 context, BlockingUnaryCallContinuation`2 continuation) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/ClientBase.cs:line 174
   at Grpc.Core.Interceptors.InterceptingCallInvoker.BlockingUnaryCall[ConnectRequest,ConnectResponse](Method`2 method, String host, CallOptions options, ConnectRequest request) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:line 48
   at GrpcCommon.Meeting.MeetingClient.Connect(ConnectRequest request, CallOptions options) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:line 178
   at GrpcCommon.Meeting.MeetingClient.Connect(ConnectRequest request, Metadata headers, Nullable`1 deadline, CancellationToken cancellationToken) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:line 174
   at MeetingGrpcClient.MeetingService.Connect(String username) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingGrpcClient\MeetingService.cs:line 102
   at MeetingMaui.MainPage..ctor() in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingMaui\MainPage.xaml.cs:line 59}
             */


			/*
             * 
             * {Grpc.Core.RpcException: Status(StatusCode="Internal", Detail="Error starting gRPC call. WebException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found. SSLHandshakeException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found. CertificateException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found. CertPathValidatorException: Trust anchor for certification path not found.", DebugException="System.Net.WebException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
 ---> Javax.Net.Ssl.SSLHandshakeException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
 ---> Java.Security.Cert.CertificateException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
 ---> Java.Security.Cert.CertPathValidatorException: Trust anchor for certification path not found.

  --- End of managed Java.Security.Cert.CertPathValidatorException stack trace ---
java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
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

  --- End of managed Java.Security.Cert.CertPathValidatorException stack trace ---
java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
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

   --- End of inner exception stack trace ---

  --- End of managed Java.Security.Cert.CertificateException stack trace ---
java.security.cert.CertificateException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
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
Caused by: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
	... 33 more

  --- End of managed Java.Security.Cert.CertificateException stack trace ---
java.security.cert.CertificateException: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
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
Caused by: java.security.cert.CertPathValidatorException: Trust anchor for certification path not found.
	... 33 more

   --- End of inner exception stack trace ---
   at Java.Interop.JniEnvironment.InstanceMethods.CallVoidMethod(JniObjectReference instance, JniMethodInfo method, JniArgumentValue* args) in /Users/runner/work/1/s/xamarin-android/external/Java.Interop/src/Java.Interop/Java.Interop/JniEnvironment.g.cs:line 11884
   at Java.Interop.JniPeerMembers.JniInstanceMethods.InvokeAbstractVoidMethod(String encodedMember, IJavaPeerable self, JniArgumentValue* parameters) in /Users/runner/work/1/s/xamarin-android/external/Java.Interop/src/Java.Interop/Java.Interop/JniPeerMembers.JniInstanceMethods_Invoke.cs:line 17
   at Javax.Net.Ssl.HttpsURLConnectionInvoker.Connect() in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/obj/Release/net6.0/android-32/mcw/Javax.Net.Ssl.HttpsURLConnection.cs:line 433
   at Xamarin.Android.Net.AndroidMessageHandler.<>c__DisplayClass125_0.<ConnectAsync>b__0() in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs:line 442
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.<>c.<.cctor>b__272_0(Object obj)
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
--- End of stack trace from previous location ---
   at Xamarin.Android.Net.AndroidMessageHandler.DoProcessRequest(HttpRequestMessage request, URL javaUrl, HttpURLConnection httpConnection, CancellationToken cancellationToken, RequestRedirectionState redirectState) in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs:line 494
   at Xamarin.Android.Net.AndroidMessageHandler.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs:line 367
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

   --- End of inner exception stack trace ---
   at Xamarin.Android.Net.AndroidMessageHandler.SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) in /Users/runner/work/1/s/xamarin-android/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs:line 381
   at Grpc.Net.Client.Web.GrpcWebHandler.SendAsyncCore(HttpRequestMessage request, CancellationToken cancellationToken) in /_/src/Grpc.Net.Client.Web/GrpcWebHandler.cs:line 166
   at System.Net.Http.HttpClient.<SendAsync>g__Core|83_0(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationTokenSource cts, Boolean disposeCts, CancellationTokenSource pendingRequestsCts, CancellationToken originalCancellationToken)
   at Grpc.Net.Client.Internal.GrpcCall`2.<RunCall>d__73[[GrpcCommon.ConnectRequest, GrpcCommon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[GrpcCommon.ConnectResponse, GrpcCommon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]].MoveNext() in /_/src/Grpc.Net.Client/Internal/GrpcCall.cs:line 493")
   at Grpc.Net.Client.Internal.HttpClientCallInvoker.BlockingUnaryCall[ConnectRequest,ConnectResponse](Method`2 method, String host, CallOptions options, ConnectRequest request) in /_/src/Grpc.Net.Client/Internal/HttpClientCallInvoker.cs:line 116
   at Grpc.Core.Interceptors.InterceptingCallInvoker.<BlockingUnaryCall>b__3_0[ConnectRequest,ConnectResponse](ConnectRequest req, ClientInterceptorContext`2 ctx) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:line 51
   at Grpc.Core.ClientBase.ClientBaseConfiguration.ClientBaseConfigurationInterceptor.BlockingUnaryCall[ConnectRequest,ConnectResponse](ConnectRequest request, ClientInterceptorContext`2 context, BlockingUnaryCallContinuation`2 continuation) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/ClientBase.cs:line 174
   at Grpc.Core.Interceptors.InterceptingCallInvoker.BlockingUnaryCall[ConnectRequest,ConnectResponse](Method`2 method, String host, CallOptions options, ConnectRequest request) in /var/local/git/grpc/src/csharp/Grpc.Core.Api/Interceptors/InterceptingCallInvoker.cs:line 48
   at GrpcCommon.Meeting.MeetingClient.Connect(ConnectRequest request, CallOptions options) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:line 178
   at GrpcCommon.Meeting.MeetingClient.Connect(ConnectRequest request, Metadata headers, Nullable`1 deadline, CancellationToken cancellationToken) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\GrpcCommon\obj\Debug\netstandard2.0\MeetingGrpc.cs:line 174
   at MeetingGrpcClient.MeetingService.Connect(String username) in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingGrpcClient\MeetingService.cs:line 102
   at MeetingMaui.MainPage..ctor() in C:\MicroServicesAndMVVMEducationalRepositoryByLimeniye\MicroServicesAndMVVM\MeetingMaui\MainPage.xaml.cs:line 80}
             */
		}

	}

    private void OnConnectionStateChanged(object sender, (ConnectionAction Action, MeetingCommon.DataTypes.UserDto User) e)
    {
        _meetingServiceAbstract.MessageService.MessagesChanged += OnMessagesChanged;
        _meetingServiceAbstract.MessageService.ChatSubscribeAsync();
    }

    private void OnMessagesChanged(object sender, Common.EventArgs.NotifyDictionaryChangedEventArgs<Guid, MeetingCommon.DataTypes.MessageDto> e)
    {

    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        var newMessage = new Message(Guid.NewGuid(), Message, null);
        Message = "";
        Messages.Add(newMessage);
        await _meetingServiceAbstract.MessageService.SendMessageAsync(newMessage.Id, _currentUser.Guid, newMessage.Text);
    }

#region INPC
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (value == null ? field != null : !value.Equals(field))
        {
            field = value;

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
            return true;
        }
        return false;
    }
#endregion
}

public class Message
{
    public Guid Id { get; }
    public string Text { get; }
    public DateTime? DateTime { get; }

    public Message(Guid id, string text, DateTime? dateTime)
    {
        Id = id;
        Text = text;
        DateTime = dateTime;
    }
}

