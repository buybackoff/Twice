﻿using Ninject;
using System.Diagnostics.CodeAnalysis;
using Twice.ViewModels.Accounts;
using Twice.ViewModels.ColumnManagement;
using Twice.ViewModels.Dialogs;
using Twice.ViewModels.Info;
using Twice.ViewModels.Main;
using Twice.ViewModels.Profile;
using Twice.ViewModels.Settings;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels
{
	[ExcludeFromCodeCoverage]
	internal class ViewModelLocator
	{
		public ViewModelLocator()
		{
			Kernel = App.Kernel;
		}

		public IColumnTypeSelectionDialogViewModel AccountColumns => Kernel.Get<IColumnTypeSelectionDialogViewModel>();
		public IAccountsDialogViewModel Accounts => Kernel.Get<IAccountsDialogViewModel>();
		public IAddColumnDialogViewModel AddColumn => Kernel.Get<IAddColumnDialogViewModel>();
		public IComposeMessageViewModel ComposeMessage => Kernel.Get<IComposeMessageViewModel>();
		public IComposeTweetViewModel ComposeTweet => Kernel.Get<IComposeTweetViewModel>();
		public IConfirmDialogViewModel Confirm => Kernel.Get<IConfirmDialogViewModel>();
		public IDialogHostViewModel DialogWindowHost => Kernel.Get<IDialogHostViewModel>();
		public IImageDialogViewModel Image => Kernel.Get<IImageDialogViewModel>();
		public IInfoDialogViewModel Info => Kernel.Get<IInfoDialogViewModel>();
		public IMainViewModel Main => Kernel.Get<IMainViewModel>();
		public IMessageDetailsViewModel MessageDetails => Kernel.Get<IMessageDetailsViewModel>();
		public IProfileDialogViewModel Profile => Kernel.Get<IProfileDialogViewModel>();
		public IRetweetDialogViewModel Retweet => Kernel.Get<IRetweetDialogViewModel>();
		public ISearchDialogViewModel Search => Kernel.Get<SearchDialogViewModel>();
		public ISettingsDialogViewModel Settings => Kernel.Get<ISettingsDialogViewModel>();
		public ITextInputDialogViewModel TextInput => Kernel.Get<ITextInputDialogViewModel>();
		public ITweetDetailsViewModel TweetDetails => Kernel.Get<ITweetDetailsViewModel>();
		private readonly IKernel Kernel;
	}
}