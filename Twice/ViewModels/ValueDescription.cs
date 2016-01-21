using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Twice.Attributes;
using Twice.Resources;

namespace Twice.ViewModels
{
	internal class ValueDescription<TValue>
	{
		public ValueDescription( TValue value, string description )
		{
			Value = value;
			Name = description;
		}

		private ValueDescription( TValue value )
		{
			Value = value;

			Name = GetName();
		}

		public static IEnumerable<ValueDescription<TValue>> GetValues( bool includeNone = false, params TValue[] toSkip )
		{
			if( !typeof( TValue ).IsEnum )
			{
				yield break;
			}

			var skip = toSkip ?? Enumerable.Empty<TValue>();

			foreach( TValue value in Enum.GetValues( typeof( TValue ) ).Cast<TValue>().Except( skip ) )
			{
				var intValue = ( (IConvertible)value ).ToInt32( CultureInfo.InvariantCulture );
				if( !includeNone && intValue == 0 )
				{
					continue;
				}

				yield return new ValueDescription<TValue>( value );
			}
		}

		public static implicit operator TValue( ValueDescription<TValue> value )
		{
			return value.Value;
		}

		public static bool operator !=( ValueDescription<TValue> a, ValueDescription<TValue> b )
		{
			return !( a == b );
		}

		public static bool operator ==( ValueDescription<TValue> a, ValueDescription<TValue> b )
		{
			if( ReferenceEquals( a, b ) )
			{
				return true;
			}

			// ReSharper disable RedundantCast.0
			if( ( (object)a == null ) || ( (object)b == null ) )

				// ReSharper restore RedundantCast.0
			{
				return false;
			}

			return a.Equals( b );
		}

		public override bool Equals( object obj )
		{
			var other = obj as ValueDescription<TValue>;
			if( other == null )
			{
				return false;
			}

			return Value.Equals( other.Value );
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		private string GetName()
		{
			if( !typeof( TValue ).IsEnum )
			{
				return null;
			}

			var convertable = (IConvertible)Value;

			var typeInfo = Value.GetType();
			var field = typeInfo.GetField( convertable.ToString( CultureInfo.InvariantCulture ) );
			var keyAttr = field.GetCustomAttribute<LocalizeKeyAttribute>();

			if( keyAttr == null )
			{
				return Strings.ResourceManager.GetString( $"{typeInfo.Name}_{Value}" );
			}

			var key = keyAttr.Key ?? convertable.ToString( CultureInfo.InvariantCulture );
			return Strings.ResourceManager.GetString( key );
		}

		// ReSharper disable once MemberCanBePrivate.Global
		// ReSharper disable once UnusedAutoPropertyAccessor.Global
		public string Name { get; }

		// ReSharper disable once MemberCanBePrivate.Global
		public TValue Value { get; }
	}
}