public override NSView GetView (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item) {
	// Cast item
	var product = item as Product;

	// This pattern allows you reuse existing views when they are no-longer in use.
	// If the returned view is null, you instance up a new view
	// If a non-null view is returned, you modify it enough to reflect the new data
	NSTableCellView view = (NSTableCellView)outlineView.MakeView (tableColumn.Title, this);
	if (view == null) {
		view = new NSTableCellView ();
		if (tableColumn.Title == "Product") {
			view.ImageView = new NSImageView (new CGRect (0, 0, 16, 16));
			view.AddSubview (view.ImageView);
			view.TextField = new NSTextField (new CGRect (20, 0, 400, 16));
		} else {
			view.TextField = new NSTextField (new CGRect (0, 0, 400, 16));
		}
		view.TextField.AutoresizingMask = NSViewResizingMask.WidthSizable;
		view.AddSubview (view.TextField);
		view.Identifier = tableColumn.Title;
		view.TextField.BackgroundColor = NSColor.Clear;
		view.TextField.Bordered = false;
		view.TextField.Selectable = false;
		view.TextField.Editable = !product.IsProductGroup;
	}

	// Tag view
	view.TextField.Tag = outlineView.RowForItem (item);

	// Allow for edit
	view.TextField.EditingEnded += (sender, e) => {

		// Grab product
		var prod = outlineView.ItemAtRow(view.Tag) as Product;

		// Take action based on type
		switch(view.Identifier) {
		case "Product":
			prod.Title = view.TextField.StringValue;
			break;
		case "Details":
			prod.Description = view.TextField.StringValue;
			break; 
		}
	};

	// Setup view based on the column selected
	switch (tableColumn.Title) {
	case "Product":
		if (product.IsProductGroup) { 
			view.ImageView.Image = NSImage.ImageNamed("tags.png");
		} else {
			view.ImageView.Image = NSImage.ImageNamed("tag.png");
		}
		view.TextField.StringValue = product.Title;
		break;
	case "Details":
		view.TextField.StringValue = product.Description;
		break;
	}

	return view;
}