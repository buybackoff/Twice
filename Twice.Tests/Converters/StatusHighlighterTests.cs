﻿using LinqToTwitter;
using LitJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Documents;
using Twice.Converters;
using Twice.Models.Configuration;
using Twice.ViewModels.Twitter;

namespace Twice.Tests.Converters
{
	[TestClass, ExcludeFromCodeCoverage]
	public class StatusHighlighterTests
	{
		[TestMethod, TestCategory( "Converters" )]
		public void ConvertBackThrowsException()
		{
			// Arrange
			var conv = new StatusHighlighter();

			// Act
			var ex = ExceptionAssert.Catch<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void DifferentCasingInEntitiesIsHandledCorrectly()
		{
			// Arrange
			var json = File.ReadAllText( "Data/tweet_casedentities.json" );
			var data = JsonMapper.ToObject( json );
			var status = new Status( data );
			var vm = new StatusViewModel( status, null, null, null );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Visual ).Returns( new VisualConfig { InlineMedia = false } );

			var conv = new StatusHighlighter( config.Object );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );
			var links = inlines.OfType<Hyperlink>().ToArray();

			// Assert
			Assert.AreEqual( 5, links.Length );

			Assert.AreEqual( "@spurs", ( (Run)links[0].Inlines.First() ).Text );
			Assert.AreEqual( "@okcthunder", ( (Run)links[1].Inlines.First() ).Text );
			Assert.AreEqual( "#PhantomCam", ( (Run)links[2].Inlines.First() ).Text );
			Assert.AreEqual( "#SPURSvTHUNDER", ( (Run)links[3].Inlines.First() ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void DoubleEntitiesAreOnlyCreatedOnce()
		{
			// Arrange
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Text = "@hello";
			status.Entities.UserMentionEntities.Add( new UserMentionEntity
			{
				Start = 0,
				ScreenName = "hello",
				End = 6
			} );
			status.Entities.UserMentionEntities.Add( new UserMentionEntity
			{
				Start = 0,
				ScreenName = "hello",
				End = 6
			} );

			var vm = new StatusViewModel( status, null, null, null );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );
			var links = inlines.OfType<Hyperlink>().ToArray();

			// Assert
			Assert.AreEqual( 1, links.Length );
			Assert.AreEqual( "@hello", ( (Run)links[0].Inlines.First() ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EmojiDoesNotBreakEntities()
		{
			// Arrange
			var json = File.ReadAllText( "Data/tweet_emoji.json" );
			var data = JsonMapper.ToObject( json );
			var status = new Status( data );
			var vm = new StatusViewModel( status, null, null, null );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Visual ).Returns( new VisualConfig { InlineMedia = false } );

			var conv = new StatusHighlighter( config.Object );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );

			// Assert
			Assert.IsInstanceOfType( inlines[0], typeof( Run ) );
			Assert.AreEqual( "Jetzt seid Ihr gefragt: Stimmt für Euren Pokalhelden ab! 🏆⚽ ", ( (Run)inlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[1], typeof( Hyperlink ) ); // #FCB
			var linkInlines = ( (Hyperlink)inlines[1] ).Inlines.ToArray();
			Assert.AreEqual( "#FCB", ( (Run)linkInlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[2], typeof( Run ) );
			Assert.AreEqual( " ", ( (Run)inlines[2] ).Text );

			Assert.IsInstanceOfType( inlines[3], typeof( Hyperlink ) ); // #BVB
			linkInlines = ( (Hyperlink)inlines[3] ).Inlines.ToArray();
			Assert.AreEqual( "#BVB", ( (Run)linkInlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[4], typeof( Run ) );
			Assert.AreEqual( " ", ( (Run)inlines[4] ).Text );

			Assert.IsInstanceOfType( inlines[5], typeof( Hyperlink ) ); // #WalkofFame
			linkInlines = ( (Hyperlink)inlines[5] ).Inlines.ToArray();
			Assert.AreEqual( "#WalkofFame", ( (Run)linkInlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[6], typeof( Run ) );
			Assert.AreEqual( " ", ( (Run)inlines[6] ).Text );

			Assert.IsInstanceOfType( inlines[7], typeof( Hyperlink ) ); // Link
			linkInlines = ( (Hyperlink)inlines[7] ).Inlines.ToArray();
			Assert.AreEqual( "on.sport1.de/20ZXKa8", ( (Run)linkInlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[8], typeof( Run ) );
			Assert.AreEqual( " ", ( (Run)inlines[8] ).Text );

			Assert.IsInstanceOfType( inlines[9], typeof( Hyperlink ) ); // Image
			linkInlines = ( (Hyperlink)inlines[9] ).Inlines.ToArray();
			Assert.AreEqual( "pic.twitter.com/JPRZdM31ha", ( (Run)linkInlines[0] ).Text );

			Assert.AreEqual( 10, inlines.Length );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EntitiesAtEndAreCorrectlyEmbedded()
		{
			// Arrange
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities.UserMentionEntities.Add( new UserMentionEntity
			{
				ScreenName = "Testi",
				Name = "Test name",
				Start = "This is a test ".Length,
				End = "This is a test @Testi".Length
			} );
			status.Text = "This is a test @Testi";
			var vm = new StatusViewModel( status, null, null, null );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );

			// Assert
			Assert.AreEqual( 2, inlines.Length );

			Assert.IsInstanceOfType( inlines[1], typeof( Hyperlink ) );
			var linkInlines = ( (Hyperlink)inlines[1] ).Inlines.ToArray();
			Assert.AreEqual( "@Testi", ( (Run)linkInlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[0], typeof( Run ) );
			Assert.AreEqual( "This is a test ", ( (Run)inlines[0] ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void EntitiesAtStartAreCorrectlyEmbedded()
		{
			// Arrange
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Entities.UserMentionEntities.Add( new UserMentionEntity
			{
				ScreenName = "Testi",
				Name = "Test name",
				Start = 0,
				End = "Testi".Length + 1
			} );
			status.Text = "@Testi This is a test";
			var vm = new StatusViewModel( status, null, null, null );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );

			// Assert
			Assert.AreEqual( 2, inlines.Length );

			Assert.IsInstanceOfType( inlines[0], typeof( Hyperlink ) );
			var linkInlines = ( (Hyperlink)inlines[0] ).Inlines.ToArray();
			Assert.AreEqual( "@Testi", ( (Run)linkInlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[1], typeof( Run ) );
			Assert.AreEqual( " This is a test", ( (Run)inlines[1] ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void HtmlCharsAreDecodedCorrectly()
		{
			// Arrange
			const string content = "<test>";
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Text = WebUtility.HtmlEncode( content );
			var vm = new StatusViewModel( status, null, null, null );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );

			// Assert
			Assert.AreEqual( 1, inlines.Length );
			Assert.IsInstanceOfType( inlines[0], typeof( Run ) );
			Assert.AreEqual( content, ( (Run)inlines[0] ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void ItemWithNullEntitiesDoesNotCrash()
		{
			// Arrange
			var conv = new StatusHighlighter();
			var status = new Status
			{
				User = DummyGenerator.CreateDummyUser()
			};
			var vm = new StatusViewModel( status, null, null, null );

			// Act
			var baseNull = ExceptionAssert.Catch<Exception>( () => conv.Convert( vm, null, null, null ) );

			status.Entities = new Entities();
			var allNull = ExceptionAssert.Catch<Exception>( () => conv.Convert( vm, null, null, null ) );

			status.Entities.HashTagEntities = new List<HashTagEntity>();
			status.Entities.MediaEntities = new List<MediaEntity>();
			status.Entities.SymbolEntities = new List<SymbolEntity>();
			status.Entities.UrlEntities = new List<UrlEntity>();
			status.Entities.UserMentionEntities = new List<UserMentionEntity>();

			var noneNull = ExceptionAssert.Catch<Exception>( () => conv.Convert( vm, null, null, null ) );

			// Assert
			Assert.IsNull( baseNull, baseNull?.ToString() );
			Assert.IsNull( allNull );
			Assert.IsNull( noneNull );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NonStandardMentionSignIsHandledCorrectly()
		{
			// Arrange
			var json = File.ReadAllText( "Data/tweet_unicodemention.json" );
			var data = JsonMapper.ToObject( json );
			var status = new Status( data );
			var vm = new StatusViewModel( status, null, null, null );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Visual ).Returns( new VisualConfig { InlineMedia = true } );

			var conv = new StatusHighlighter( config.Object );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );
			var links = inlines.OfType<Hyperlink>().ToArray();

			// Assert
			Assert.AreEqual( 3, inlines.Length );

			Assert.AreEqual( 1, links.Length );
			Assert.AreEqual( "@BVB", ( (Run)links[0].Inlines.First() ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void NonStatusThrowsException()
		{
			// Arrange
			var conv = new StatusHighlighter();

			// Act
			var ex = ExceptionAssert.Catch<ArgumentException>( () => conv.Convert( string.Empty, null, null, null ) );

			// Assert
			Assert.IsNotNull( ex );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void SimpleTextStatusResultsInSingeInline()
		{
			// Arrange
			var conv = new StatusHighlighter();
			var status = DummyGenerator.CreateDummyStatus();
			status.Text = "Hello World";
			var vm = new StatusViewModel( status, null, null, null );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );

			// Assert
			Assert.AreEqual( 1, inlines.Length );
			Assert.IsInstanceOfType( inlines[0], typeof( Run ) );
			Assert.AreEqual( status.Text, ( (Run)inlines[0] ).Text );
		}

		[TestMethod, TestCategory( "Converters" )]
		public void TextOfExtendedTweetIsCorrect()
		{
			// Arrange
			var json = File.ReadAllText( "Data/tweet_extended.json" );
			var data = JsonMapper.ToObject( json );
			var status = new Status( data );
			var vm = new StatusViewModel( status, null, null, null );
			var config = new Mock<IConfig>();
			config.SetupGet( c => c.Visual ).Returns( new VisualConfig { InlineMedia = false } );

			var conv = new StatusHighlighter( config.Object );

			// Act
			var inlines = (Inline[])conv.Convert( vm, null, null, null );

			// Assert
			Assert.IsInstanceOfType( inlines[0], typeof( Run ) );
			Assert.AreEqual( "Das ist mal wieder ein ", ( (Run)inlines[0] ).Text );

			Assert.IsInstanceOfType( inlines[2], typeof( Run ) );
			Assert.AreEqual( ". Und jetzt muss ich sogar irgendwie dieses mal die 140 Zeichen vollbekommen. Aber wie in einem ", ( (Run)inlines[2] ).Text );

			Assert.IsInstanceOfType( inlines[4], typeof( Run ) );
			Assert.AreEqual( "? Fragen :o ", ( (Run)inlines[4] ).Text );
		}
	}
}