// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;

namespace StoryboardTables
{
	public partial class PeekViewController : UIViewController
	{
		public PeekViewController (IntPtr handle) : base (handle)
		{
			View.BackgroundColor = UIColor.White;
		}

		Task current {get;set;}

		// this will be called before the view is displayed 
		public void SetTodo (Task todo) {
			current = todo;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			Name.Text = current.Name;
			Notes.Text = current.Notes;
			if (current.Done)
				Done.Image = UIImage.FromBundle ("checkbox");
			else
				Done.Image = UIImage.FromBundle ("box");
			
		}
	}
}
