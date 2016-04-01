﻿using System.Diagnostics.CodeAnalysis;
using Akavache;
using Ninject.Modules;
using Twice.Models.Cache;
using Twice.Models.Configuration;
using Twice.Models.Twitter;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class ModelInjectionModule : NinjectModule
	{
		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		public override void Load()
		{
			Bind<IDataCache>().To<DataCache>().InSingletonScope();
			Bind<IBlobCache>().ToProvider<BlobCacheProvider>().InSingletonScope();
			Bind<ISecureBlobCache>().ToProvider<SecureBlobCacheProvider>().InSingletonScope();

			Bind<ITwitterContextList>().ToProvider<TwitterContextListProvider>().InSingletonScope();
			Bind<IConfig>().ToProvider<ConfigurationProvider>().InSingletonScope();
			Bind<IStatusMuter>().To<StatusMuter>();
		}
	}
}