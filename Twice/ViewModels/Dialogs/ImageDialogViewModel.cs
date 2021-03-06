﻿using GalaSoft.MvvmLight.CommandWpf;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Twice.Resources;
using Twice.Utilities.Os;
using Twice.ViewModels.Twitter;

namespace Twice.ViewModels.Dialogs
{
	internal class ImageDialogViewModel : DialogViewModel, IImageDialogViewModel
	{
		public ImageDialogViewModel()
		{
			Images = new ObservableCollection<ImageEntry>();
			Title = Strings.ImageViewer;
		}

		public void SetImages( IEnumerable<Uri> images )
		{
			Images.Clear();
			foreach( var url in images )
			{
				Images.Add( new ImageEntry( url, false ) );
			}

			Center();
		}

		public void SetImages( IEnumerable<StatusMediaViewModel> images )
		{
			Images.Clear();
			foreach( var url in images )
			{
				Images.Add( new ImageEntry( url ) );
			}

			Center();
		}

		private void ExecuteCopyToClipboardCommand()
		{
			Clipboard.SetText( SelectedImage.DisplayUrl.AbsoluteUri );
		}

		private void ExecuteOpenImageCommand()
		{
			ProcessStarter.Start( SelectedImage.DisplayUrl.AbsoluteUri );
		}

		[Inject]
		public IClipboard Clipboard { get; set; }

		public ICommand CopyToClipboardCommand
					=> _CopyToClipboardCommand ?? ( _CopyToClipboardCommand = new RelayCommand( ExecuteCopyToClipboardCommand ) );

		public ICollection<ImageEntry> Images { get; }

		public ICommand OpenImageCommand
			=> _OpenImageCommand ?? ( _OpenImageCommand = new RelayCommand( ExecuteOpenImageCommand ) );

		public ImageEntry SelectedImage
		{
			[DebuggerStepThrough]
			get { return _SelectedImage; }
			set
			{
				if( _SelectedImage == value )
				{
					return;
				}

				_SelectedImage = value;
				RaisePropertyChanged();
			}
		}

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _CopyToClipboardCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private RelayCommand _OpenImageCommand;

		[DebuggerBrowsable( DebuggerBrowsableState.Never )]
		private ImageEntry _SelectedImage;
	}
}