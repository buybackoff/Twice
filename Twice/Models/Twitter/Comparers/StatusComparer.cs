using LinqToTwitter;
using System.Collections.Generic;

namespace Twice.Models.Twitter.Comparers
{
	internal class StatusComparer : IEqualityComparer<Status>
	{
		public bool Equals( Status x, Status y )
		{
			return x.GetStatusId() == y.GetStatusId();
		}

		public int GetHashCode( Status obj )
		{
			return obj.GetStatusId().GetHashCode();
		}
	}
}