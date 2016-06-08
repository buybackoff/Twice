using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using LinqToTwitter;

namespace Twice.ViewModels.Twitter
{
	internal abstract class ColumnItem : ObservableObject
	{
		public abstract DateTime CreatedAt { get; }
		public abstract Entities Entities { get; }
		public abstract ulong Id { get; }

		public bool IsLoading
		{
			[DebuggerStepThrough] get { return _IsLoading; }
			set
			{
				if( _IsLoading == value )
				{
					return;
				}

				_IsLoading = value;
				RaisePropertyChanged();
			}
		}

		public abstract string Text { get; }
		public UserViewModel User { get; protected set; }

		[DebuggerBrowsable( DebuggerBrowsableState.Never )] private bool _IsLoading;
	}
}