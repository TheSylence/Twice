using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Twice.Tests
{
	[ExcludeFromCodeCoverage]
	internal class PropertyChangedTester
	{
		public PropertyChangedTester( INotifyPropertyChanged obj, bool includeInherited = false, ITypeResolver typeResolver = null )
		{
			Object = obj;
			IncludeInherited = includeInherited;
			TypeResolver = typeResolver;
		}

		public void Test( params string[] propsToSkip )
		{
			Test( (IEnumerable<string>)propsToSkip );
		}

		public void Test( IEnumerable<string> propsToSkip = null )
		{
			Errors.Clear();

			var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			if( !IncludeInherited )
			{
				bindingFlags |= BindingFlags.DeclaredOnly;
			}

			var properties =
				Object.GetType()
				.GetProperties( bindingFlags )
				.Where( p => p.CanRead && p.CanWrite && p.SetMethod.IsPublic )
				.ToArray();
			if( !properties.Any() )
			{
				Errors.Add( "No properties found" );
			}

			List<string> receivedChangeNames = new List<string>();

			Object.PropertyChanged += ( s, e ) => receivedChangeNames.Add( e.PropertyName );

			var toSkip = propsToSkip ?? Enumerable.Empty<string>();
			foreach( var prop in properties.Where( p => !toSkip.Contains( p.Name ) ) )
			{
				receivedChangeNames.Clear();

				prop.SetValue( Object, GetDefaultValue( prop.PropertyType ) );
				prop.SetValue( Object, GetNonDefaultValue( prop.PropertyType ) );

				if( !receivedChangeNames.Contains( prop.Name ) )
				{
					Errors.Add( $"Property {prop.Name} does not raise PropertyChangedEvent when being changed" );
					continue;
				}

				receivedChangeNames.Clear();

				prop.SetValue( Object, prop.GetValue( Object ) );
				if( receivedChangeNames.Contains( prop.Name ) )
				{
					Errors.Add( $"Property {prop.Name} raises PropertyChangedEvent when same value is set" );
				}
			}
		}

		public void Verify()
		{
			Assert.IsFalse( Errors.Any(), string.Join( Environment.NewLine, Errors ) );
		}

		private static object GetDefaultValue( Type type )
		{
			if( type.IsValueType )
			{
				return Activator.CreateInstance( type );
			}

			return null;
		}

		private object GetNonDefaultValue( Type type )
		{
			var typeMap = new Dictionary<Type, object>
			{
				{typeof( short ), 1},
				{typeof( ushort ), 1},
				{typeof( int ), 1},
				{typeof( uint ), 1},
				{typeof( long ), 1},
				{typeof( ulong ), 1},
				{typeof( double ), 1},
				{typeof( float ), 1},
				{typeof( decimal ), 1},
				{typeof( short? ), (short?)1},
				{typeof( ushort? ), (ushort?)1},
				{typeof( int? ), (int?)1},
				{typeof( uint? ), (uint?)1},
				{typeof( long? ), (long?)1},
				{typeof( ulong? ), (ulong?)1},
				{typeof( double? ), (double?)1},
				{typeof( float? ), (float?)1},
				{typeof( decimal? ), (decimal?)1},
				{typeof( string ), string.Empty},
				{typeof( DateTime ), DateTime.Now},
				{typeof( bool ), true},
				{typeof(CultureInfo), CultureInfo.CurrentUICulture}
			};

			object v;
			if( typeMap.TryGetValue( type, out v ) )
			{
				return v;
			}

			if( type.IsEnum )
			{
				return Enum.GetValues( type ).Cast<object>().Last();
			}

			var constructor = type.GetConstructor( Type.EmptyTypes );
			if( constructor != null )
			{
				return constructor.Invoke( null );
			}

			if( TypeResolver!=null)
			{
				return TypeResolver.Resolve( type );
			}

			throw new InvalidOperationException( $"{type.Name} - Don't known how to create non default value" );
		}

		private readonly List<string> Errors = new List<string>();
		private readonly INotifyPropertyChanged Object;
		private readonly bool IncludeInherited;
		private readonly ITypeResolver TypeResolver;
	}
}