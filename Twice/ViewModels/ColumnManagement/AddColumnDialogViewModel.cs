﻿using System.Collections.Generic;
using MaterialDesignThemes.Wpf;
using Twice.ViewModels.Columns.Definitions;
using Twice.ViewModels.Wizards;

namespace Twice.ViewModels.ColumnManagement
{
	internal interface IAddColumnDialogViewModel : IWizardViewModel
	{
	}

	internal class AddColumnDialogViewModel : WizardViewModel, IAddColumnDialogViewModel
	{
		public AddColumnDialogViewModel()
		{
			Pages.Add( 0, new ColumnTypeSelctorPage() );

			CurrentPage = Pages[0];
		}
	}

	internal class ColumnTypeItem
	{
		public ColumnTypeItem( ColumnType type, string name, string description, PackIconKind icon )
		{
			Type = type;
			Icon = icon;
			DisplayName = name;
			Description = description;
		}

		public string Description { get; }
		public string DisplayName { get; }
		public PackIconKind Icon { get; }
		public ColumnType Type { get; }
	}

	internal class ColumnTypeSelctorPage : WizardPageViewModel
	{
		public ColumnTypeSelctorPage()
		{
			ColumnTypes = new List<ColumnTypeItem>( ColumnTypeListFactory.GetItems() );
		}

		public ICollection<ColumnTypeItem> ColumnTypes { get; }

		public override int NextPageKey => _NextPageKey;

		private int _NextPageKey;
	}
}