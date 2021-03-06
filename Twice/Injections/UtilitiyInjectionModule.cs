﻿using System.Diagnostics.CodeAnalysis;
using Ninject.Modules;
using Twice.Utilities;
using Twice.Utilities.Os;
using Twice.Utilities.Ui;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class UtilitiyInjectionModule : NinjectModule
	{
		/// <summary>
		///     Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IColorProvider>().To<ColorProvider>();
			Bind<ILanguageProvider>().To<LanguageProvider>();
			Bind<IDateProvider>().To<DateProvider>().InSingletonScope();
			Bind<IDispatcher>().To<DispatcherHelperWrapper>().InSingletonScope();
			Bind<IFileSystem>().To<FileSystem>();
			Bind<ITimerFactory>().To<TimerFactory>();
			Bind<IAppUpdaterFactory>().To<AppUpdaterFactory>();
			Bind<IProcessStarter>().To<ProcessStarter>();
			Bind<ISerializer>().To<Serializer>();
			Bind<IClipboard>().To<ClipboardWrapper>();
		}
	}
}