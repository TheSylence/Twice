using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Twice.ViewModels.Validation
{
	internal abstract class ValidationViewModel : ViewModelBaseEx, INotifyDataErrorInfo, IPropertyValidatorContainer, IValidationViewModel
	{
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		void IPropertyValidatorContainer.AddValidator<TProperty>( string propertyName, PropertyValidator<TProperty> validator )
		{
			List<PropertyValidatorBase> validatorList;
			if( !ValidationMap.TryGetValue( propertyName, out validatorList ) )
			{
				validatorList = new List<PropertyValidatorBase>();
				ValidationMap.Add( propertyName, validatorList );
			}

			validatorList.Add( validator );
		}

		public void ClearValidationErrors()
		{
			foreach( var kvp in ValidationMap )
			{
				foreach( var binder in kvp.Value )
				{
					binder.Clear();
				}

				RaiseErrorsChanged( kvp.Key );
			}

			RaiseErrorsChanged( null );
		}

		public void ClearValidationRules()
		{
			ValidationMap.Clear();
		}

		public IEnumerable GetErrors( string propertyName )
		{
			if( string.IsNullOrEmpty( propertyName ) || !ValidationMap.ContainsKey( propertyName ) )
			{
				return Enumerable.Empty<string>();
			}

			return ValidationMap[propertyName].Where( p => p.HasError ).Select( p => p.Error );
		}

		public IValidationSetup<TProperty> ManualValidate<TProperty>( Expression<Func<TProperty>> propertyExpression )
		{
			if( propertyExpression == null )
			{
				throw new ArgumentNullException( nameof( propertyExpression ) );
			}

			string propertyName = ( propertyExpression.Body as MemberExpression )?.Member.Name;
			VerifyPropertyName( propertyName );

			CachePropertyGetter( propertyName );
			return new ValidationSetup<TProperty>( this, propertyName, true );
		}

		public override void RaisePropertyChanged( [CallerMemberName] string propertyName = null )
		{
			ClearValidationErrors( propertyName );
			ValidateProperty( propertyName );

			// ReSharper disable once ExplicitCallerInfoArgument
			base.RaisePropertyChanged( propertyName );
		}

		protected void RaiseErrorsChanged( string propertyName )
		{
			ErrorsChanged?.Invoke( this, new DataErrorsChangedEventArgs( propertyName ) );
		}

		protected IValidationSetup<TProperty> Validate<TProperty>( Expression<Func<TProperty>> propertyExpression )
		{
			if( propertyExpression == null )
			{
				throw new ArgumentNullException( nameof( propertyExpression ) );
			}

			string propertyName = ( propertyExpression.Body as MemberExpression )?.Member.Name;
			VerifyPropertyName( propertyName );

			CachePropertyGetter( propertyName );
			return new ValidationSetup<TProperty>( this, propertyName, false );
		}

		protected void ValidateAll()
		{
			foreach( var kvp in ValidationMap )
			{
				var value = GetPropertyValue( kvp.Key );
				kvp.Value.ForEach( v => v.Update( value ) );

				RaiseErrorsChanged( kvp.Key );
			}
		}

		private void CachePropertyGetter( string propertyName )
		{
			if( Getters.ContainsKey( propertyName ) )
			{
				return;
			}

			var propInfo = GetType().GetProperty( propertyName );
			Getters.Add( propertyName, obj => propInfo.GetValue( obj ) );
		}

		private void ClearValidationErrors( string propertyName )
		{
			List<PropertyValidatorBase> validatorList;
			if( !ValidationMap.TryGetValue( propertyName, out validatorList ) )
			{
				return;
			}

			foreach( var v in validatorList )
			{
				v.Clear();
			}

			RaiseErrorsChanged( propertyName );
		}

		private object GetPropertyValue( string propertyName )
		{
			return Getters[propertyName]( this );
		}

		private void ValidateProperty( string propertyName )
		{
			List<PropertyValidatorBase> validatorList;
			if( !ValidationMap.TryGetValue( propertyName, out validatorList ) )
			{
				return;
			}

			var value = GetPropertyValue( propertyName );

			validatorList.ForEach( b =>
			{
				if( !b.IsManual )
				{
					b.Update( value );
				}
			} );
			RaiseErrorsChanged( propertyName );
		}

		public string AllErrors
		{
			get
			{
				var lines =
					ValidationMap.Values.Where( v => v.Any( vv => vv.HasError ) ).SelectMany( v => v ).Select( v => v.Error ).Where(
						x => x != null ).ToArray();
				return lines.Any()
					? string.Join( Environment.NewLine, lines )
					: null;
			}
		}

		public bool HasErrors => ValidationMap.Values.Any( prop => prop.Any( validator => validator.HasError ) );
		private readonly Dictionary<string, Func<object, object>> Getters = new Dictionary<string, Func<object, object>>();

		private readonly Dictionary<string, List<PropertyValidatorBase>> ValidationMap =
			new Dictionary<string, List<PropertyValidatorBase>>();
	}
}