﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Twice.Behaviors
{
	internal class ManualToggleButton : Behavior<Button>
	{
		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.Click += AssociatedObject_Click;
		}

		private void AssociatedObject_Click( object sender, RoutedEventArgs e )
		{
			IsChecked = !IsChecked;
		}

		public bool IsChecked
		{
			get { return (bool)GetValue( IsCheckedProperty ); }
			set { SetValue( IsCheckedProperty, value ); }
		}

		public static readonly DependencyProperty IsCheckedProperty =
			DependencyProperty.Register( "IsChecked", typeof( bool ), typeof( ManualToggleButton ), new PropertyMetadata( false ) );
	}
}