using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using AppKit;

namespace AzureTodo
{
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public class TodoItemManager
	{
		#region Private Variables
		private IMobileServiceTable<TodoItem> todoTable;
		private MobileServiceClient client;
		#endregion

		#region Constructors
		public TodoItemManager()
		{

			// Has the application been configured with the developer's
			// Azure information?
			if (Constants.ApplicationURL == "") {
				// No, inform user
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Critical,
					InformativeText = "Before this example can be successfully run, you need to provide your developer information used to access Azure.",
					MessageText = "Azure Not Configured",
				};
				alert.RunModal ();
			} else {

				// Establish a link to Azure
				client = new MobileServiceClient (
					Constants.ApplicationURL,
					Constants.ApplicationKey);

				// Read any existing todo items from the Azure client
				this.todoTable = client.GetTable<TodoItem> ();
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the todo item async.
		/// </summary>
		/// <returns>The todo item async.</returns>
		/// <param name="id">The ID of the item to get.</param>
		public async Task<TodoItem> GetTodoItemAsync(string id)
		{
			try
			{
				return await todoTable.LookupAsync(id);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"INVALID {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"ERROR {0}", e.Message);
			}
			return null;
		}

		/// <summary>
		/// Gets all todo items async.
		/// </summary>
		/// <returns>The todo items async.</returns>
		public async Task<List<TodoItem>> GetTodoItemsAsync()
		{
			try
			{
				return new List<TodoItem>(await todoTable.ReadAsync());
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"INVALID {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"ERROR {0}", e.Message);
			}
			return null;
		}

		/// <summary>
		/// Saves the todo item to Azure storage async.
		/// </summary>
		/// <returns>The todo item async.</returns>
		/// <param name="item">The Item to save.</param>
		public async Task SaveTodoItemAsync(TodoItem item)
		{
			if (item.ID == null) {
				await todoTable.InsertAsync (item);
			} else {
				await todoTable.UpdateAsync (item);
			}
		}

		/// <summary>
		/// Deletes the todo item from Azure storage async.
		/// </summary>
		/// <returns>The todo item async.</returns>
		/// <param name="item">The Item to delete.</param>
		public async Task DeleteTodoItemAsync(TodoItem item)
		{
			try
			{
				await todoTable.DeleteAsync(item);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"INVALID {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"ERROR {0}", e.Message);
			}
		}
		#endregion
	}
}

