﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twice.Models.Cache;
using Twice.Models.Twitter;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.ViewModels.Twitter
{
	[TestClass]
	public class ComposeTweetViewModelTests
	{
		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public async Task DefaultAccountIsPreselectedForTweeting()
		{
			// Arrange
			var defAcc = new Mock<IContextEntry>();
			defAcc.SetupGet( a => a.IsDefault ).Returns( true );
			defAcc.SetupGet( a => a.ProfileImageUrl ).Returns( new Uri( "http://example.com" ) );
			var otherAcc = new Mock<IContextEntry>();
			otherAcc.SetupGet( a => a.IsDefault ).Returns( false );
			otherAcc.SetupGet( a => a.ProfileImageUrl ).Returns( new Uri( "http://example.com" ) );

			var cache = new Mock<IDataCache>();
			var contextList = new Mock<ITwitterContextList>();
			contextList.SetupGet( c => c.Contexts ).Returns( new[] {otherAcc.Object, defAcc.Object} );

			var vm = new ComposeTweetViewModel( cache.Object )
			{
				ContextList = contextList.Object
			};

			// Act
			await vm.Reset();

			// Assert
			var usedAccount = vm.Accounts.SingleOrDefault( a => a.Use );
			Assert.IsNotNull( usedAccount );

			var notUsedAccount = vm.Accounts.SingleOrDefault( a => !a.Use );
			Assert.IsNotNull( notUsedAccount );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void MediaCanBeRemoved()
		{
			// Arrange
			var cache = new Mock<IDataCache>();
			var vm = new ComposeTweetViewModel( cache.Object );
			vm.AttachedMedias.Add( new MediaItem( 123, new byte[] {} ) );

			// Act
			vm.DeleteMediaCommand.Execute( 111ul );
			bool wrongId = vm.AttachedMedias.Any( m => m.MediaId == 123 );
			vm.DeleteMediaCommand.Execute( 123ul );
			bool correctId = vm.AttachedMedias.Any( m => m.MediaId == 123 );

			// Assert
			Assert.IsTrue( wrongId );
			Assert.IsFalse( correctId );
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void MimeTypeForFileIsDetectedCorrectly()
		{
			// Arrange
			var testCases = new Dictionary<string, string>
			{
				{"file.jpg", "application/octet-stream"},
				{"file.png", "image/png"},
				{"file.bmp", "image/bmp"},
				{"file.gif", "image/gif"},
				{"file.png.gif", "image/gif"},
				{"file", "application/octet-stream"},
				{"name.exe", "application/octet-stream"},
				{"name", "application/octet-stream"}
			};

			// Act
			var results = testCases.ToDictionary( kvp => kvp.Key, kvp => ComposeTweetViewModel.GetMimeType( kvp.Key ) );

			// Assert
			foreach( var kvp in results )
			{
				Assert.AreEqual( testCases[kvp.Key], kvp.Value );
			}
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void PropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var cache = new Mock<IDataCache>();
			var obj = new ComposeTweetViewModel( cache.Object );
			var tester = new PropertyChangedTester( obj );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Twitter" )]
		public void TweetCommandIsCorrectlyDisabled()
		{
			// Arrange
			var cache = new Mock<IDataCache>();
			var vm = new ComposeTweetViewModel( cache.Object );

			bool requiresConfirmation = true;
			var contextEntry = new Mock<IContextEntry>();
			contextEntry.SetupGet( c => c.ProfileImageUrl ).Returns( new Uri( "http://example.com" ) );
			contextEntry.SetupGet( c => c.RequiresConfirmation ).Returns( () => requiresConfirmation );

			// Act
			var noData = vm.SendTweetCommand.CanExecute( null );

			vm.Text = "\r\n\t\t ";
			var onlyWhitespace = vm.SendTweetCommand.CanExecute( null );

			vm.Text = "test";
			vm.Accounts.Add( new AccountEntry( contextEntry.Object ) );
			var noUsedAccounts = vm.SendTweetCommand.CanExecute( null );

			vm.Accounts.First().Use = true;
			var noConfirmationSet = vm.SendTweetCommand.CanExecute( null );

			requiresConfirmation = false;
			vm.Text = new string( 'x', 141 );
			var tooLong = vm.SendTweetCommand.CanExecute( null );

			vm.Text = "test";
			var ok = vm.SendTweetCommand.CanExecute( null );

			// Assert
			Assert.IsFalse( noData );
			Assert.IsFalse( onlyWhitespace );
			Assert.IsFalse( noUsedAccounts );
			Assert.IsFalse( noConfirmationSet );
			Assert.IsFalse( tooLong );
			Assert.IsTrue( ok );
		}
	}
}