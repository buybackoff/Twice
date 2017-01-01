using System;
using GalaSoft.MvvmLight;
using LinqToTwitter;
using Twice.Models.Proxy;
using Twice.Models.Twitter;
using Twice.Models.Twitter.Entities;

namespace Twice.ViewModels.Twitter
{
	internal class UserViewModel : ObservableObject
	{
		public UserViewModel( User user )
		{
			Model = user;

			ProfileImageUrlHttps = user.ProfileImageUrlHttps;
			ProfileImageUrlHttpsOrig = user.ProfileImageUrlHttps?.Replace( "_normal", "" );
			ProfileImageUrlHttpsMini = user.ProfileImageUrlHttps?.Replace( "_normal", "_mini" );
			ProfileImageUrlHttpsBig = user.ProfileImageUrlHttps?.Replace( "_normal", "_bigger" );
			if( !string.IsNullOrEmpty( ProfileImageUrlHttpsBig ) )
			{
				BigProfileImageUrl = MediaProxyServer.BuildUrl( ProfileImageUrlHttpsBig );
			}

			ScreenName = Constants.Twitter.Mention + Model.GetScreenName();
			Url = Uri.IsWellFormedUriString( user.Url, UriKind.Absolute ) ? new Uri( user.Url ) : user.GetUserUrl();
			DisplayUrl = Url.AbsoluteUri;
		}

		public UserViewModel( UserEx user )
			: this( (User)user )
		{
			if( Uri.IsWellFormedUriString( user.UrlDisplay, UriKind.Absolute ) )
			{
				Url = new Uri( user.UrlDisplay );
			}

			DisplayUrl = user.UrlDisplay ?? Url.AbsoluteUri;
		}

		public bool IsProtected => Model.Protected;
		public bool IsVerified => Model.Verified;
		public User Model { get; }
		public UserEx ModelEx => Model as UserEx;
		public string ProfileImageUrlHttps { get; }
		public string ProfileImageUrlHttpsBig { get; }
		public Uri BigProfileImageUrl { get; }
		public string ProfileImageUrlHttpsMini { get; }
		public string ProfileImageUrlHttpsOrig { get; }
		public string ScreenName { get; }
		public Uri Url { get; }
		public string DisplayUrl { get; }
		public ulong UserId => Model.GetUserId();
	}
}