// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace StoryboardTables
{
	public partial class CollectionController : UICollectionViewController, IUIViewControllerPreviewingDelegate
	{
		public CollectionController (IntPtr handle) : base (handle)
		{
			Title = NSBundle.MainBundle.LocalizedString ("Todo", "");
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			AddButton.Clicked += (sender, e) => {
				CreateTask ();
			};
			// 3D Touch
			if (TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available) {
				RegisterForPreviewingWithDelegate (this, CollectionView);
			}
		}
		List<Task> tasks;
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			tasks = AppDelegate.Current.TaskMgr.GetTasks ().ToList ();

			// bind every time, to reflect deletion in the Detail view
			Collection.Source = new AllCollectionSource(tasks.ToArray ());
			Collection.AllowsSelection = true;
			Collection.DelaysContentTouches = false;
		}



		/// <summary>
		/// Prepares for segue.
		/// </summary>
		/// <remarks>
		/// The prepareForSegue method is invoked whenever a segue is about to take place. 
		/// The new view controller has been loaded from the storyboard at this point but 
		/// it’s not visible yet, and we can use this opportunity to send data to it.
		/// </remarks>
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "todosegue") { // set in Storyboard
				var tvc = segue.DestinationViewController as DetailViewController;
				if (tvc != null) {
					var source = Collection.Source as AllCollectionSource;
					var rowPath = Collection.IndexPathForCell (sender as UICollectionViewCell);
					var item = source.GetItem(rowPath.Row);
					tvc.Delegate = this;
					tvc.SetTodo(item);
				}
			}
		}

		public void CreateTask ()
		{
			// StackView
			var detail = Storyboard.InstantiateViewController("detailvc") as DetailViewController;
			detail.Delegate = this;
			detail.SetTodo (new Task());
			NavigationController.PushViewController (detail, true);

			// Could to this instead of the above, but need to create 'new Task()' in PrepareForSegue()
			//this.PerformSegue ("TaskSegue", this);
		}
		public void SaveTask (Task task) {
			Console.WriteLine("Save "+task.Name);

			AppDelegate.Current.TaskMgr.SaveTask(task);

			SpotlightHelper.Index (task);

		}
		public void DeleteTask (Task task) {
			Console.WriteLine("Delete "+task.Name);
			if (task.Id >= 0) {
				AppDelegate.Current.TaskMgr.DeleteTask (task.Id);
				SpotlightHelper.Delete (task);
			}
		}

		#region 3DTouch Peek
		public UIViewController GetViewControllerForPreview (IUIViewControllerPreviewing previewingContext, CGPoint location)
		{
			// Obtain the index path and the cell that was pressed.
			var indexPath = CollectionView.IndexPathForItemAtPoint (location);

			Console.WriteLine ("ForPreview " + location.ToString() + " " + indexPath);

			if (indexPath == null)
				return null;

			var cell = CollectionView.CellForItem (indexPath);

			if (cell == null)
				return null;


			// Create a detail view controller and set its properties.
			var peekViewController = (PeekViewController)Storyboard.InstantiateViewController ("peekvc");
			if (peekViewController == null)
				return null;

			var peekAt = tasks [indexPath.Row];
			peekViewController.SetTodo (peekAt);
			peekViewController.PreferredContentSize = new CGSize (0, 160);
			previewingContext.SourceRect = cell.Frame;
			return peekViewController;
		}

		public void CommitViewController (IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit)
		{
			Console.WriteLine ("CommitViewContoller");

			var sv = (UICollectionView)previewingContext.SourceView;
			var si = sv.GetIndexPathsForSelectedItems ();

			var x = previewingContext.SourceRect.X + (previewingContext.SourceRect.Width / 2);
			var y = previewingContext.SourceRect.Y + (previewingContext.SourceRect.Height / 2);

			var indexPath = CollectionView.IndexPathForItemAtPoint (new CGPoint(x,y));
			var popAt = tasks [indexPath.Row];


			var detailViewController = (DetailViewController)Storyboard.InstantiateViewController ("detailvc");
			if (detailViewController == null)
				return;

			detailViewController.SetTodo (popAt);

			//viewControllerToCommit = peekViewController;
			ShowViewController (detailViewController, this);
		}
		#endregion
	}
}
