using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace VueInAndroidWebView {
	[Activity( Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true )]
	public class MainActivity : AppCompatActivity {

		WebView wv { get { return FindViewById<WebView>( Resource.Id.wv ); } }

		protected override void OnCreate( Bundle savedInstanceState ) {
			base.OnCreate( savedInstanceState );
			Xamarin.Essentials.Platform.Init( this, savedInstanceState );
			SetContentView( Resource.Layout.activity_main );

			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>( Resource.Id.toolbar );
			SetSupportActionBar( toolbar );

			FloatingActionButton fab = FindViewById<FloatingActionButton>( Resource.Id.fab );
			fab.Click += FabOnClick;

			var myChromeClient = new WebChromeClient();
			wv.SetWebChromeClient( myChromeClient );

			var myWebClient = new MyWebViewClient( Assets );
			wv.SetWebViewClient( myWebClient );

			wv.Settings.JavaScriptEnabled = true;
			wv.Settings.DomStorageEnabled = true;
			wv.Settings.AllowContentAccess = true;
			wv.Settings.AllowFileAccess = true;
			wv.Settings.AllowFileAccessFromFileURLs = true;
			wv.CanGoBack();
			wv.CanGoForward();
			
			global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled( true );


			if ( savedInstanceState != null ) {
				wv.RestoreState( savedInstanceState );
			} else {
				// wv.LoadDataWithBaseURL( "file:///android_asset/", new StreamReader( Assets.Open( "index2.html" ) ).ReadToEnd(), "text/html", "UTF-8", null );
				wv.LoadUrl( "file:///android_asset/index.html" );
			}
		}

		public override bool OnCreateOptionsMenu( IMenu menu ) {
			MenuInflater.Inflate( Resource.Menu.menu_main, menu );
			return true;
		}

		public override bool OnOptionsItemSelected( IMenuItem item ) {
			int id = item.ItemId;
			if ( id == Resource.Id.action_settings ) {
				return true;
			}

			return base.OnOptionsItemSelected( item );
		}

		private void FabOnClick( object sender, EventArgs eventArgs ) {
			wv.Reload();
		}
		public override void OnRequestPermissionsResult( int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults ) {
			Xamarin.Essentials.Platform.OnRequestPermissionsResult( requestCode, permissions, grantResults );

			base.OnRequestPermissionsResult( requestCode, permissions, grantResults );
		}
	}


	public class MyWebChromeClient : WebChromeClient { // WebViewClient

		private AssetManager m_Assets;

		public MyWebChromeClient( AssetManager assets ) {
			m_Assets = assets;
		}

		public override bool OnConsoleMessage( ConsoleMessage consoleMessage ) {
			Log.Debug( "WebView", consoleMessage.Message() );
			return base.OnConsoleMessage( consoleMessage );
		}

		public override void OnProgressChanged( WebView view, int newProgress ) {
			Log.Debug( "WebView", "Progress: " + newProgress );
			base.OnProgressChanged( view, newProgress );
		}
	}


	public class MyWebViewClient : WebViewClient {

		private AssetManager m_Assets;

		public MyWebViewClient( AssetManager assets ) {
			m_Assets = assets;
		}

		public static string GetMimeType( String url ) {
			String type = null;
			String extension = url.Split( '.' ).Last();
			if ( extension != null ) {
				if ( extension == "js" ) {
					return "text/javascript";
				} else if ( extension == "woff" ) {
					return "application/font-woff";
				} else if ( extension == "woff2" ) {
					return "application/font-woff2";
				} else if ( extension == "ttf" ) {
					return "application/x-font-ttf";
				} else if ( extension == "eot" ) {
					return "application/vnd.ms-fontobject";
				} else if ( extension == "svg" ) {
					return "image/svg+xml";
				}

				type = "text/html";
			}

			return type;
		}

		public override WebResourceResponse ShouldInterceptRequest( WebView view, IWebResourceRequest request ) {
			try {
				// request.RequestHeaders.Add(/* collection of Key Value Pairs */ );
				if ( request.Url != null && request.Url.Path != null && request.Url.Path.StartsWith( "/" ) && !request.Url.Path.EndsWith( "html" ) ) {
					var mimeType = GetMimeType( request.Url.Path );
					var fileUrl = request.Url.Path.Replace( "file:///android_asset/", "" ).Replace( "file:///", "" ).Replace( "/android_asset", "" ).Trim( '/' );
					var fileContents = new StreamReader( m_Assets.Open( fileUrl ) ).ReadToEnd();
					return new WebResourceResponse( mimeType, "UTF-8", 200, "OK", new Dictionary<string, string>(),
						new MemoryStream( System.Text.Encoding.UTF8.GetBytes( fileContents ) ) );
				}
			} catch(Exception err) {
				Log.Debug( "WebView", err.ToString() );
			}

			return base.ShouldInterceptRequest( view, request );
		}
	}
}

