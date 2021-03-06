﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Twice.Models.Configuration;
using Twice.Utilities.Ui;
using Twice.ViewModels.Settings;

namespace Twice.Tests.ViewModels.Settings
{
	[TestClass, ExcludeFromCodeCoverage]
	public class GeneralSettingsTests
	{
		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void EnglishIsUsedAsFallbackLanguageWhenOtherLanguageWasNotFound()
		{
			// Arrange
			var cfg = new GeneralConfig { Language = "es-ES" };
			var cfgMock = new Mock<IConfig>();
			cfgMock.SetupGet( c => c.General ).Returns( cfg );

			var languageProvider = new Mock<ILanguageProvider>();
			languageProvider.SetupGet( l => l.AvailableLanguages ).Returns( new[]
			{
				CultureInfo.CreateSpecificCulture( "de-DE" ),
				CultureInfo.CreateSpecificCulture( "en-US" )
			} );

			// Act
			var vm = new GeneralSettings( cfgMock.Object, languageProvider.Object );

			// Assert
			Assert.AreEqual( "en-US", vm.SelectedLanguage.Name );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void InvariantCultureIsNotDisplayed()
		{
			// Arrange
			var cfg = new GeneralConfig { Language = "es-ES" };
			var cfgMock = new Mock<IConfig>();
			cfgMock.SetupGet( c => c.General ).Returns( cfg );

			var languageProvider = new Mock<ILanguageProvider>();
			languageProvider.SetupGet( l => l.AvailableLanguages ).Returns( new[]
			{
				CultureInfo.CreateSpecificCulture( "en-US" ),
				CultureInfo.CreateSpecificCulture( "" )
			} );

			// Act
			var vm = new GeneralSettings( cfgMock.Object, languageProvider.Object );

			// Assert
			Assert.AreEqual( 1, vm.AvailableLanguages.Count );
			Assert.AreEqual( "en-US", vm.AvailableLanguages.First().Name );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void NeutralLanguagesAreRemovedIfSpecificVersionExists()
		{
			// Arrange
			var cfg = new GeneralConfig { Language = "es-ES" };
			var cfgMock = new Mock<IConfig>();
			cfgMock.SetupGet( c => c.General ).Returns( cfg );

			var languageProvider = new Mock<ILanguageProvider>();
			languageProvider.SetupGet( l => l.AvailableLanguages ).Returns( new[]
			{
				CultureInfo.CreateSpecificCulture( "en-US" ),
				CultureInfo.CreateSpecificCulture( "de-DE" ),
				CultureInfo.CreateSpecificCulture( "en-US" ).Parent,
				CultureInfo.CreateSpecificCulture( "en-GB" ),
				CultureInfo.CreateSpecificCulture( "de-DE" ).Parent
			} );

			// Act
			var vm = new GeneralSettings( cfgMock.Object, languageProvider.Object );

			// Assert
			var names = vm.AvailableLanguages.Select( l => l.Name ).ToArray();
			CollectionAssert.AreEquivalent( new[] { "de-DE", "en-US", "en-GB" }, names, string.Join( "; ", names ) );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void PropertyChangedIsImplementedCorrectly()
		{
			// Arrange
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var lng = new Mock<ILanguageProvider>();
			var vm = new GeneralSettings( cfg.Object, lng.Object );
			var tester = new PropertyChangedTester( vm );

			// Act
			tester.Test();

			// Assert
			tester.Verify();
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SavedValuesAreAppliedDuringConstruction()
		{
			// Arrange
			var cfg = new GeneralConfig
			{
				CheckForUpdates = false,
				IncludePrereleaseUpdates = true,
				Language = "de-DE",
				RealtimeStreaming = false,
				FilterSensitiveTweets = true
			};

			var cfgMock = new Mock<IConfig>();
			cfgMock.SetupGet( c => c.General ).Returns( cfg );

			var languageProvider = new Mock<ILanguageProvider>();
			languageProvider.SetupGet( l => l.AvailableLanguages ).Returns( new[] { CultureInfo.CreateSpecificCulture( "de-DE" ) } );

			// Act
			var vm = new GeneralSettings( cfgMock.Object, languageProvider.Object );

			// Assert
			Assert.AreEqual( cfg.CheckForUpdates, vm.CheckForUpdates );
			Assert.AreEqual( cfg.IncludePrereleaseUpdates, vm.IncludePrereleaseUpdates );
			Assert.AreEqual( cfg.RealtimeStreaming, vm.RealtimeStreaming );
			Assert.AreEqual( cfg.Language, vm.SelectedLanguage.Name );
			Assert.AreEqual( cfg.FilterSensitiveTweets, vm.FilterSensitiveTweets );
		}

		[TestMethod, TestCategory( "ViewModels.Settings" )]
		public void SettingsAreCorrectlySaved()
		{
			// Arrange
			var cfg = new Mock<IConfig>();
			cfg.SetupGet( c => c.General ).Returns( new GeneralConfig() );
			var lng = new Mock<ILanguageProvider>();
			var vm = new GeneralSettings( cfg.Object, lng.Object )
			{
				CheckForUpdates = false,
				IncludePrereleaseUpdates = true,
				RealtimeStreaming = false,
				SelectedLanguage = CultureInfo.CreateSpecificCulture( "es-ES" ),
				FilterSensitiveTweets = true
			};

			var saved = new Mock<IConfig>();
			var general = new GeneralConfig();
			saved.SetupGet( s => s.General ).Returns( general );

			// Act
			vm.SaveTo( saved.Object );

			// Assert
			Assert.AreEqual( vm.CheckForUpdates, general.CheckForUpdates );
			Assert.AreEqual( vm.IncludePrereleaseUpdates, general.IncludePrereleaseUpdates );
			Assert.AreEqual( vm.RealtimeStreaming, general.RealtimeStreaming );
			Assert.AreEqual( vm.SelectedLanguage.Name, general.Language );
			Assert.AreEqual( vm.FilterSensitiveTweets, general.FilterSensitiveTweets );
		}
	}
}