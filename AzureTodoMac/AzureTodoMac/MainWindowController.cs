using System;

using Foundation;
using AppKit;
using AzureTodo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureTodo
{
	public partial class MainWindowController : NSWindowController
	{
		#region Private Variables
		private TodoItemManager manager;
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
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public override async void WindowDidLoad ()
		{
			base.WindowDidLoad ();
			manager = new TodoItemManager ();
			Reload ();
		}
		#endregion

		#region Storyboard Actions
		/// <summary>
		/// Adds the new todo item.
		/// </summary>
		/// <param name="sender">Sender.</param>
		async partial void addNewTodoItem (Foundation.NSObject sender) {

			// Create a new item and save it to Azure
			var newTodo = new TodoItem {
				Name = newTodoItemName.StringValue
			};
			await manager.SaveTodoItemAsync (newTodo);

			// Update the interface
			await Reload ();
			newTodoItemName.StringValue = "";
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Reloads all of the records from Azure and displays them in the table.
		/// </summary>
		public async Task Reload ()
		{
			var items = await manager.GetTodoItemsAsync ();
			Console.WriteLine ("items retrieved: " + items.Count);
			todoTable.DataSource = new TableDataSource(items);
			todoTable.Delegate = new TableDelegate (this);
		}

		/// <summary>
		/// Deletes the given record from Azure.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public async Task Delete (string id){
			await manager.DeleteTodoItemAsync(new TodoItem {ID = id});
			Reload ();
		}
		#endregion
	}
}
