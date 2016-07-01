﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ninject;
using Ninject.Modules;
using Twice.Models.Media;

namespace Twice.Injections
{
	[ExcludeFromCodeCoverage]
	internal class Kernel : StandardKernel
	{
		public Kernel()
			: base( InjectionModules.ToArray() )
		{
			MigrateAppData();

			MediaExtractorRepository.Default.AddExtractor( new InstragramExtractor() );
			MediaExtractorRepository.Default.AddExtractor( new YoutubeExtractor() );
		}

		[Conditional( "DEBUG" )]
		private static void MigrateAppData()
		{
#if !DEBUG
			var localAppDataFolder = Constants.IO.AppDataFolder;
			var roamingAppDataFolder = Constants.IO.RoamingAppDataFolder;

			if( Directory.Exists( localAppDataFolder ) )
			{
				foreach( var file in Directory.GetFiles( localAppDataFolder ) )
				{
					var targetFile = Path.GetFileName( file );
					if( targetFile == null )
					{
						continue;
					}

					File.Move( file, Path.Combine( roamingAppDataFolder, targetFile ) );
				}

				Directory.Delete( localAppDataFolder );
			}
#endif
		}

		private static IEnumerable<INinjectModule> InjectionModules
		{
			get
			{
				yield return new ModelInjectionModule();
				yield return new ViewModelInjectionModule();
				yield return new ServiceInjectionModule();
				yield return new UtilitiyInjectionModule();
			}
		}
	}
}