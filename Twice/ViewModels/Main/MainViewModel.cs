﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Twice.Messages;
using Twice.Models.Twitter;
using Twice.ViewModels.Columns;
using Twice.ViewModels.Columns.Definitions;
using Twice.Views;

namespace Twice.ViewModels.Main
{
	internal class MainViewModel : ViewModelBaseEx, IMainViewModel
	{
		public MainViewModel( ITwitterContextList list, IStatusMuter muter, INotifier notifier )
		{
			Notifier = notifier;
			var context = list.Contexts.First();

			var columnList = new ColumnDefintionList( Constants.IO.ColumnDefintionFileName );
			var columns = columnList.Load();
			if( !columns.Any() )
			{
				columns = columnList.DefaultColumns( context.UserId );
			}

			var factory = new ColumnFactory( list, muter );

			var constructed = columns.Select( c => factory.Construct( c ) );
			constructed = constructed.Where( c => c != null );
			

			Columns = new ObservableCollection<IColumnViewModel>( constructed );
			foreach( var col in Columns )
			{
				col.NewStatus += Col_NewStatus;
			}
		}

		public async Task OnLoad( object data )
		{
			foreach( var col in Columns )
			{
				await col.Load();
			}
		}

		private void Col_NewStatus( object sender, StatusEventArgs e )
		{
			ColumnNotifications columnSettings = new ColumnNotifications
			{
				Popup = true,
				Sound = true,
				Toast = true
			};

			Notifier.OnStatus( e.Status, columnSettings );
		}

		private async void ExecuteAccountsCommand()
		{
			await ViewServiceRepository.ShowAccounts();
		}

		private async void ExecuteInfoCommand()
		{
			await ViewServiceRepository.ShowInfo();
		}

		private async void ExecuteAddColumnCommand()
		{
			await ViewServiceRepository.ShowAddColumnDialog();
		}

		private void ExecuteNewTweetCommand()
		{
			MessengerInstance.Send( new FlyoutMessage( FlyoutNames.TweetComposer, FlyoutAction.Open ) );
		}

		private async void ExecuteSettingsCommand()
		{
			await ViewServiceRepository.ShowSettings();
		}

		public ICommand AccountsCommand => _AccountsCommand ?? ( _AccountsCommand = new RelayCommand( ExecuteAccountsCommand ) );

		public ICollection<IColumnViewModel> Columns { get; }

		public ICommand InfoCommand => _InfoCommand ?? ( _InfoCommand = new RelayCommand( ExecuteInfoCommand ) );

		public ICommand AddColumnCommand => _ManageColumnsCommand ?? ( _ManageColumnsCommand = new RelayCommand( ExecuteAddColumnCommand ) );

		public ICommand NewTweetCommand => _NewTweetCommand ?? ( _NewTweetCommand = new RelayCommand( ExecuteNewTweetCommand ) );

		public ICommand SettingsCommand => _SettingsCommand ?? ( _SettingsCommand = new RelayCommand( ExecuteSettingsCommand ) );

		private readonly INotifier Notifier;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _AccountsCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _InfoCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _ManageColumnsCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _NewTweetCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _SettingsCommand;
	}
}