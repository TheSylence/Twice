using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Twice.Models.Columns
{
	internal class ColumnDefinition
	{
		public ColumnDefinition( ColumnType type )
		{
			Type = type;
			Width = 300;
			Notifications = new ColumnNotifications();

			SourceAccounts = new ulong[0];
			TargetAccounts = new ulong[0];
		}

		public override bool Equals( object obj )
		{
			var other = obj as ColumnDefinition;
			if( other?.Type != Type )
			{
				return false;
			}

			return SourceAccounts.Compare( other.SourceAccounts ) &&
					TargetAccounts.Compare( other.TargetAccounts );
		}

		[SuppressMessage( "ReSharper", "NonReadonlyMemberInGetHashCode" )]
		public override int GetHashCode()
		{
			int hash = 17;
			unchecked
			{
				hash = hash * 23 + Type.GetHashCode();
				hash = SourceAccounts.OrderBy( x => x ).Aggregate( hash, ( current, acc ) => current * 23 + acc.GetHashCode() );
				hash = TargetAccounts.OrderBy( x => x ).Aggregate( hash, ( current, acc ) => current * 23 + acc.GetHashCode() );
			}
			return hash;
		}

		public Guid Id { get; set; }
		public ColumnNotifications Notifications { get; set; }
		public ulong[] SourceAccounts { get; set; }
		public ulong[] TargetAccounts { get; set; }
		public ColumnType Type { get; set; }
		public int Width { get; set; }
	}
}