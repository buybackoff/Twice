﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Twice.ViewModels;

namespace Twice.Behaviors
{
	internal class ScrollToEnd : Behavior<ItemsControl>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
		}

		private static void OnControllerChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			var b = d as ScrollToEnd;
			b?.OnControllerChanged( (IScrollController)e.NewValue, (IScrollController)e.OldValue );
		}

		private void Controller_ScrollRequested( object sender, EventArgs e )
		{
			int index = ScrollBottom
				? AssociatedObject.ItemContainerGenerator.Items.Count - 1
				: 0;

			var element = AssociatedObject.ItemContainerGenerator.ContainerFromIndex( index ) as FrameworkElement;
			element?.BringIntoView();
		}

		private void OnControllerChanged( IScrollController newController, IScrollController oldController )
		{
			if( oldController != null )
			{
				oldController.ScrollRequested -= Controller_ScrollRequested;
			}

			if( newController != null )
			{
				newController.ScrollRequested += Controller_ScrollRequested;
			}
		}

		public static readonly DependencyProperty ControllerProperty =
			DependencyProperty.Register( "Controller", typeof(IScrollController), typeof(ScrollToEnd),
				new PropertyMetadata( null, OnControllerChanged ) );

		public static readonly DependencyProperty ScrollBottomProperty =
			DependencyProperty.Register( "ScrollBottom", typeof(bool), typeof(ScrollToEnd), new PropertyMetadata( true ) );

		public IScrollController Controller
		{
			get { return (IScrollController)GetValue( ControllerProperty ); }
			set { SetValue( ControllerProperty, value ); }
		}

		public bool ScrollBottom
		{
			get { return (bool)GetValue( ScrollBottomProperty ); }
			set { SetValue( ScrollBottomProperty, value ); }
		}
	}
}