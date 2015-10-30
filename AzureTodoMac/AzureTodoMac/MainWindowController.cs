using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AppKit;
using Foundation;

namespace AzureTodo
{
	public partial class MainWindowController : NSWindowController
	{
		#region Private Variables
		TodoItemManager manager;
		#endregion

		#region Computed Properties
		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
		#endregion

		#region Constructors
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}
		#endregion

		#region Override Methods
		public override async void WindowDidLoad ()
		{
			base.WindowDidLoad ();
			if (string.IsNullOrEmpty (Constants.ApplicationURL)) {
				// No, inform user
				ShowAlert ();
			} else {
				manager = new TodoItemManager ();
				await Reload ();
			}
		}
		#endregion

		#region Storyboard Actions
		/// <summary>
		/// Adds the new todo item.
		/// </summary>
		/// <param name="sender">Sender.</param>
		async partial void AddNewTodoItem (NSObject sender)
		{
			if (string.IsNullOrEmpty (Constants.ApplicationURL)) {
				// No, inform user
				ShowAlert ();
			} else {
				// Create a new item and save it to Azure
				var newTodo = new TodoItem {
					Name = newTodoItemName.StringValue
				};
				await manager.SaveTodoItemAsync (newTodo);

				// Update the interface
				await Reload ();
				newTodoItemName.StringValue = string.Empty;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Reloads all of the records from Azure and displays them in the table.
		/// </summary>
		public async Task Reload ()
		{
			var items = await manager.GetTodoItemsAsync ();
			Console.WriteLine ("items retrieved: {0}", items.Count);
			todoTable.DataSource = new TableDataSource (items);
			todoTable.Delegate = new TableDelegate (this);
		}

		/// <summary>
		/// Deletes the given record from Azure.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public async Task Delete (string id)
		{
			await manager.DeleteTodoItemAsync (new TodoItem { ID = id });
			await Reload ();
		}
		#endregion

		void ShowAlert ()
		{
			// No, inform user
			var alert = new NSAlert {
				AlertStyle = NSAlertStyle.Warning,
				InformativeText = "Before this example can be successfully run, you need to provide your developer information used to access Azure.",
				MessageText = "Azure Not Configured"
			};
			alert.RunSheetModal (Window);
		}
	}
}
