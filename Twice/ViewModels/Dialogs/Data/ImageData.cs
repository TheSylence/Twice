using System;
using System.Collections.Generic;
using System.Linq;
using Twice.ViewModels.Twitter;
using Twice.Views.Dialogs;

namespace Twice.ViewModels.Dialogs.Data
{
	internal class ImageData : DialogData
	{
		public ImageData( IList<StatusMediaViewModel> imageSet, StatusMediaViewModel selectedImage )
			: base( typeof( ImageDialog ), typeof( IImageDialogViewModel ) )
		{
			ImageSetMedias = new List<StatusMediaViewModel>( imageSet );
			SelectedImageMedia = selectedImage;
		}

		public ImageData( IList<Uri> imageSet, Uri selectedImage )
			: base( typeof( ImageDialog ), typeof( IImageDialogViewModel ) )
		{
			ImageSetUrls = new List<Uri>( imageSet );
			SelectedImageUrl = selectedImage;
		}

		public override bool Equals( DialogData obj )
		{
			var other = obj as ImageData;
			if( other == null )
			{
				return false;
			}

			if( ImageSetUrls != null && other.ImageSetUrls != null )
			{
				return SelectedImageUrl.Equals( other.SelectedImageUrl )
					&& ImageSetUrls.Compare( other.ImageSetUrls );
			}

			if( ImageSetMedias != null && other.ImageSetMedias != null )
			{
				return SelectedImageMedia.Equals( other.SelectedImageMedia )
					&& ImageSetMedias.Compare( other.ImageSetMedias );
			}

			return false;
		}

		public override object GetResult( object viewModel )
		{
			return null;
		}

		public override void Setup( object viewModel )
		{
			var vm = CastViewModel<IImageDialogViewModel>( viewModel );

			if( ImageSetUrls != null )
			{
				vm.SetImages( ImageSetUrls );
				vm.SelectedImage = vm.Images.FirstOrDefault( img => img.ImageUrl == SelectedImageUrl )
								   ?? vm.Images.FirstOrDefault();
			}
			else
			{
				vm.SetImages( ImageSetMedias );
				vm.SelectedImage = vm.Images.FirstOrDefault( img => img.ImageUrl == SelectedImageMedia.Url )
									?? vm.Images.FirstOrDefault();
			}
		}

		private List<StatusMediaViewModel> ImageSetMedias;
		private List<Uri> ImageSetUrls;
		private StatusMediaViewModel SelectedImageMedia;
		private Uri SelectedImageUrl;
	}
}