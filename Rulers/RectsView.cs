using System;
using System.Linq;
using System.Collections.Generic;

using AppKit;
using Foundation;
using CoreGraphics;

namespace Rulers
{
	
	/// <summary>
	/// RectsView is the ruler view's client in this test app. It tries to handle most ruler operations.
	/// </summary>
	public partial class RectsView : AppKit.NSView
	{
		// First time control variable
		static bool beenHere = false;
		
		// Our display list of ColorRect objects
		List<ColorRect> rects;
		ColorRect selectedItem = null;
		
		
		// make sure these stay global
		//  problems arise when they are defined locally.
		NSNumber[] upArray;
		NSNumber[] downArray;
		
		// Our ruler marker images
		NSImage leftImage;
		NSImage rightImage;
		NSImage topImage;
		NSImage bottomImage;
		
		
		// String labels used for the object representation of our markers
		static NSString STR_LEFT_OBJ 	= new NSString("Left Edge");
		static NSString STR_RIGHT_OBJ 	= new NSString("Right Edge");
		static NSString STR_TOP_OBJ 	= new NSString("Top Edge");
		static NSString STR_BOTTOM_OBJ 	= new NSString("Bottom Edge");
		
		const string STR_LEFT 	= "Left Edge";
		const string STR_RIGHT 	= "Right Edge";
		const string STR_TOP 	= "Top Edge";
		const string STR_BOTTOM = "Bottom Edge";
		
		// Random class used to generate our colors
		Random rand = new Random();
		
		static float MIN_SIZE = 5.0f;
		
		// Zoom Factors
		const float ZOOM_IN_FACTOR = 2.0f;
		const float ZOOM_OUT_FACTOR = 1.0f / ZOOM_IN_FACTOR;
		
		#region Constructors

		// Called when created from unmanaged code
		public RectsView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public RectsView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{			
			if (beenHere) return;
			
			beenHere = true;
			
			// Moved from the initWithFrame constructor because it was giving a 
			// not initialized correctly message.
			CGRect aRect;
			ColorRect firstRect;
			
			SetBoundsOrigin (new CGPoint (-108.0f,-108.0f));
			
			rects = new List<ColorRect> ();
			selectedItem = null;
			
			aRect = new CGRect (30.0f, 45.0f, 57.0f, 118.0f);
			firstRect = new ColorRect (aRect, NSColor.Blue);
			rects.Add (firstRect);                          

			//-------------------------------------------------------------------
			
			NSBundle mainBundle;
			string path;
			
			mainBundle = NSBundle.MainBundle;
			path = mainBundle.PathForResource ("EdgeMarkerLeft","tiff");
			leftImage = new NSImage(path);
			
			path = mainBundle.PathForResource ("EdgeMarkerRight","tiff");
			rightImage = new NSImage(path);
			
			path = mainBundle.PathForResource ("EdgeMarkerTop","tiff");
			topImage = new NSImage(path);

			path = mainBundle.PathForResource ("EdgeMarkerBottom","tiff");
			bottomImage = new NSImage (path);
			
			upArray = new NSNumber[] { NSNumber.FromDouble (2.0) };
			downArray = new NSNumber[] { NSNumber.FromDouble (0.5),
										NSNumber.FromDouble (0.2)};
			
			// Setup our custom ruler units
			NSRulerView.RegisterUnit ("MyCustomRulerUnits", "mcru", 100.0f, upArray, downArray);

		}
		
		#endregion
		
		#region Class Overrides
		
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			
			NSScrollView scrollView = EnclosingScrollView;
			
			if (scrollView == null) 
				return;
			
			scrollView.HasHorizontalRuler = true;
			scrollView.HasVerticalRuler = true;

			setRulerOffsets ();
			updateRulers ();
			
			scrollView.RulersVisible = true;
			
		}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override bool IsFlipped {
			get {
				return true;
			}
		}
		
		public override void DrawRect (CGRect dirtyRect)
		{
			NSColor.White.Set ();
			NSGraphics.RectFill (dirtyRect);
			
			foreach (ColorRect thisRect in rects){
				if (thisRect.Frame.IntersectsWith (dirtyRect))
					thisRect.DrawRect (dirtyRect, thisRect == selectedItem);
			}
			
			// draw a little cross in our view
			NSColor.Black.Set ();
			NSBezierPath.StrokeLine (new CGPoint (-10.0f,0.0f), new CGPoint (10.0f,0.0f));
			NSBezierPath.StrokeLine (new CGPoint (0.0f,-10.0f), new CGPoint (0.0f,10.0f));
		}		
		
		public override void MouseDown (NSEvent theEvent)
		{
			ColorRect oldSelectedItem = selectedItem;
			ColorRect thisRect;
			CGPoint mouseLoc = CGPoint.Empty;
			CGPoint mouseOffset = CGPoint.Empty;
			NSEventMask eventMask;
			bool dragged = false;
			bool timerOn = false;
			
			NSEvent autoScrollEvent = null;
			
			selectedItem = null;
			
			if (!Window.MakeFirstResponder (this)) 
				return;
			
			mouseLoc = ConvertPointFromView (theEvent.LocationInWindow, null);
			
			// we go from last to first
			for (int x = rects.Count - 1;x >= 0; x--){
				thisRect = rects[x];
				
				if (IsMouseInRect (mouseLoc,thisRect.Frame)){
					selectedItem = thisRect;
					break;
				}
			}
			
			if (oldSelectedItem != selectedItem){
				if (oldSelectedItem != null)
					SetNeedsDisplayInRect (oldSelectedItem.Frame);
				if (selectedItem != null)
					SetNeedsDisplayInRect (selectedItem.Frame);
			 	
				updateRulers ();
			}
			
			if (selectedItem == null || selectedItem.IsLocked) 
				return;
			
			mouseOffset.X = mouseLoc.X - selectedItem.Frame.Location.X;
			mouseOffset.Y = mouseLoc.Y - selectedItem.Frame.Location.Y;
			
			eventMask = NSEventMask.LeftMouseDragged | NSEventMask.LeftMouseUp
						| NSEventMask.Periodic;
			
			while (theEvent != null) {
				CGRect visibleRect = VisibleRect ();
				
				switch (theEvent.Type){					
				case NSEventType.Periodic:
					if (autoScrollEvent != null)
						Autoscroll(autoScrollEvent);
					moveSelectedItemWithEvent (autoScrollEvent, mouseOffset);
					break;
					
				case NSEventType.LeftMouseDragged:
					if (!dragged)
						drawRulerLinesWithRect (selectedItem.Frame);
					
					dragged = true;
					mouseLoc = ConvertPointFromView (theEvent.LocationInWindow, null);
					
					moveSelectedItemWithEvent (theEvent, mouseOffset);
					
					if (!IsMouseInRect (mouseLoc, visibleRect)){
						if (!timerOn){
							NSEvent.StartPeriodicEventsAfterDelay (0.1,0.1);
							timerOn = true;
						}
						
						autoScrollEvent = theEvent;
						break;						
					} else {
						if (timerOn){
 							NSEvent.StopPeriodicEvents ();
							timerOn = false;
							autoScrollEvent = null;
						}
						
					}
					DisplayIfNeeded ();
					break;
					
				case NSEventType.LeftMouseUp:
					if (timerOn){
						NSEvent.StopPeriodicEvents ();
						timerOn = false;
						autoScrollEvent = null;
					}
					if (dragged)
						eraseRulerLinesWithRect (selectedItem.Frame);
					updateRulers ();
					return;

				default:
					break;
				}
				
				theEvent = this.Window.NextEventMatchingMask (eventMask);
			}
		}
		
		#endregion
		
		#region Actions
		
		partial void lockSelectedItem (NSObject sender)
		{
			if (selectedItem == null) 
				return;
			
			selectedItem.IsLocked = !selectedItem.IsLocked;
				SetNeedsDisplayInRect(selectedItem.Frame);
		}
		
		partial void zoomIn (NSObject sender)
		{
			CGRect tempRect;
			CGRect oldBounds;
			NSScrollView scrollView = this.EnclosingScrollView;
			
			oldBounds = Bounds;
			
			tempRect = Frame;
			tempRect.Size = setRectWidth (tempRect, ZOOM_IN_FACTOR * tempRect.Width);
			tempRect.Size = setRectHeight (tempRect, ZOOM_IN_FACTOR * tempRect.Height);
			
			Frame = tempRect;
			
			SetBoundsSize(oldBounds.Size);
			SetBoundsOrigin(oldBounds.Location);
			
			if (scrollView != null)
				scrollView.NeedsDisplay = true;
			else
				Superview.NeedsDisplay = true;
		}
		
		partial void zoomOut (NSObject sender)
		{
			CGRect tempRect;
			CGRect oldBounds;
			NSScrollView scrollView = EnclosingScrollView;
			
			oldBounds = Bounds;
			
			tempRect = Frame;
			tempRect.Size = setRectWidth (tempRect, ZOOM_OUT_FACTOR * tempRect.Width);
			tempRect.Size = setRectHeight (tempRect, ZOOM_OUT_FACTOR * tempRect.Height);
			
			Frame = tempRect;
			
			SetBoundsSize (oldBounds.Size);
			SetBoundsOrigin (oldBounds.Location);
			
			if (scrollView != null)
				scrollView.NeedsDisplay = true;
			else
				Superview.NeedsDisplay = true;
		}
		
		
		
		/// <summary>
		/// 
		/// slips a larger view between the enclosing NSClipView and the
		/// receiver, and adjusts the ruler origin to lie at the same point in the
		/// receiver. Apps that tile pages differently might want to do this when
		/// an NSView representing a page is moved.
		/// 
		/// </summary>
		/// <param name="sender">
		/// A <see cref="NSObject"/>
		/// </param>		

		partial void nestle (NSObject sender)
		{
			NSScrollView enclosingScrollView = EnclosingScrollView;
			
			if (enclosingScrollView == null) 
				return;
			
			if (Superview is NestleView)
				enclosingScrollView.DocumentView = this;
			else {
				CGRect nFrame, rFrame;
				NestleView nestleView;
				
				rFrame = Frame;
				nFrame = new CGRect(0.0f, 0.0f, rFrame.Width + 64.0f, rFrame.Height + 64.0f);
				
				nestleView = new NestleView (nFrame);

				nestleView.AddSubview (this);
				rFrame.Location = new CGPoint (32.0f, 32.0f);
				Frame = rFrame;
				enclosingScrollView.DocumentView = nestleView;
				
			}
			
			Window.MakeFirstResponder (this);
			setRulerOffsets ();
			updateRulers ();
			enclosingScrollView.NeedsDisplay = true;
		}
		#endregion
		
		#region Worker Methods
		
		private void moveSelectedItemWithEvent (NSEvent theEvent, CGPoint mouseOffset)
		{
			CGRect oldRect, newRect, bounds;
			CGPoint mouseLoc;
			
			mouseLoc = ConvertPointFromView (theEvent.LocationInWindow, null);
			
			bounds = Bounds;
			oldRect = newRect = selectedItem.Frame;
			newRect.Location = new CGPoint (mouseLoc.X - mouseOffset.X, mouseLoc.Y - mouseOffset.Y);
			
			if (MinX (newRect) < MinX (bounds))
				newRect.Location = setOriginX (newRect, MinX (bounds));
			if (MaxX (newRect) > MaxX (bounds))
				newRect.Location = setOriginX (newRect, MaxX (bounds) - newRect.Width);
			if (MinY (newRect) < MinY (bounds))
				newRect.Location = setOriginY (newRect, MinY (bounds));
			if (MaxY (newRect) > MaxY (bounds))
				newRect.Location = setOriginY (newRect, MaxY (bounds) - newRect.Height);
			
			selectedItem.Frame = newRect;
			updateRulerLinesWithOldRect (oldRect,newRect);
			SetNeedsDisplayInRect (oldRect);
			SetNeedsDisplayInRect (newRect);
		}
		
		private void updateRulerLinesWithOldRect (CGRect oldRect, CGRect newRect)
		{
			NSScrollView scrollView = EnclosingScrollView;
			NSRulerView horizRuler;
			NSRulerView vertRuler;
			CGRect convOldRect;
			CGRect convNewRect;
			
			if (scrollView == null) 
				return;
			
			horizRuler = scrollView.HorizontalRulerView;
			vertRuler = scrollView.VerticalRulerView;
			
			if (horizRuler != null){
				convOldRect = ConvertRectToView (oldRect,horizRuler);
				convNewRect = ConvertRectToView (newRect,horizRuler);
				
				horizRuler.MoveRulerline (MinX (convOldRect), MinX (convNewRect));
				horizRuler.MoveRulerline (MaxX (convOldRect), MaxX (convNewRect));
			}
			
			if (vertRuler != null){
				convOldRect = ConvertRectToView (oldRect, vertRuler);
				convNewRect = ConvertRectToView (newRect, vertRuler);
				
				vertRuler.MoveRulerline (MinY (convOldRect), MinY (convNewRect));
				vertRuler.MoveRulerline (MaxY (convOldRect), MaxY (convNewRect));
			}
		}
		
		private void eraseRulerLinesWithRect (CGRect aRect)
		{
			NSScrollView scrollView = EnclosingScrollView;
			NSRulerView horizRuler;
			NSRulerView vertRuler;
			
			if (scrollView == null) 
				return;
			
			horizRuler = scrollView.HorizontalRulerView;
			vertRuler = scrollView.VerticalRulerView;
			
			if (horizRuler != null)
				horizRuler.NeedsDisplay = true;
			
			if (vertRuler != null)
				vertRuler.NeedsDisplay = true;
		}
		
		private void drawRulerLinesWithRect(CGRect aRect)
		{
			NSScrollView scrollView = EnclosingScrollView;
			NSRulerView horizRuler;
			NSRulerView vertRuler;
			CGRect convRect;
			
			if (scrollView == null) 
				return;
			
			horizRuler = scrollView.HorizontalRulerView;
			vertRuler = scrollView.VerticalRulerView;
			
			if (horizRuler != null){
				convRect = ConvertRectToView (aRect, horizRuler);
				
				horizRuler.MoveRulerline (-1.0f, MinX (convRect));
				horizRuler.MoveRulerline (-1.0f, MaxX (convRect));
			}
			
			if (vertRuler != null){
				convRect = this.ConvertRectToView (aRect, vertRuler);
				
				vertRuler.MoveRulerline (-1.0f, MinY (convRect));
				vertRuler.MoveRulerline (-1.0f, MaxY (convRect));
			}
		}
		
		private void setRulerOffsets()
		{
			NSScrollView scrollView = EnclosingScrollView;
			NSRulerView horizRuler = null;
			NSRulerView vertRuler;
			NSView docView;
			NSView clientView;
			CGPoint zero = CGPoint.Empty;
			
			docView = (NSView)scrollView.DocumentView;
			clientView = this;
			
			if (scrollView == null) 
				return;
			
			horizRuler = scrollView.HorizontalRulerView;
			vertRuler = scrollView.VerticalRulerView;
			
			zero = docView.ConvertPointFromView (clientView.Bounds.Location,clientView);
			
			horizRuler.OriginOffset = zero.X - docView.Bounds.Location.X;
			vertRuler.OriginOffset = zero.Y - docView.Bounds.Location.Y;
		}
		
		private void updateRulers()
		{
			updateHorizontalRuler();
			updateVerticalRuler();
		}
		
		private void updateHorizontalRuler() 
		{
			NSScrollView scrollView;
			NSRulerView horizRuler = null;
			NSRulerMarker leftMarker;
			NSRulerMarker rightMarker;
			
			scrollView = EnclosingScrollView;
			if (scrollView == null) 
				return;
			
			horizRuler = scrollView.HorizontalRulerView;
			if (horizRuler == null) 
				return;
			
			if (horizRuler.ClientView != this){
				horizRuler.ClientView = this;
				horizRuler.MeasurementUnits = "MyCustomRulerUnits";
			}
			
			if (selectedItem == null){
				horizRuler.Markers = null;
				return;
			}
			
			leftMarker = new NSRulerMarker (horizRuler, MinX(selectedItem.Frame), 
			                                leftImage, CGPoint.Empty);
			rightMarker = new NSRulerMarker (horizRuler, MaxX(selectedItem.Frame),
			                                 rightImage, new CGPoint(7.0f,0.0f));
			horizRuler.Markers = new NSRulerMarker[] { leftMarker, rightMarker };
			
			leftMarker.Removable = true;
			rightMarker.Removable = true;
			
			leftMarker.RepresentedObject = STR_LEFT_OBJ;
			rightMarker.RepresentedObject = STR_RIGHT_OBJ;
		}

		private void updateVerticalRuler() 
		{
			
			NSScrollView scrollView;
			NSRulerView vertRuler = null;
			CGPoint thePoint; 	/* Just a temporary scratch variable */
			nfloat location;
			NSRulerMarker topMarker;
			NSRulerMarker bottomMarker;
			
			scrollView = EnclosingScrollView;
			if (scrollView == null) 
				return;
			
			vertRuler = scrollView.VerticalRulerView;
			if (vertRuler == null) 
				return;
			
			if (vertRuler.ClientView != this){
				vertRuler.ClientView = this;
				vertRuler.MeasurementUnits = "MyCustomRulerUnits";
			}
			
			if (selectedItem == null){
				vertRuler.Markers = null;
				return;
			}
			
			if (IsFlipped)
				location = MaxY (selectedItem.Frame);
			else
				location = MinY (selectedItem.Frame);	
			
			thePoint = new CGPoint (8.0f, 1.0f);
			bottomMarker = new NSRulerMarker (vertRuler, location, bottomImage, thePoint);
			bottomMarker.Removable = true;
			bottomMarker.RepresentedObject = STR_BOTTOM_OBJ;
			
			if (this.IsFlipped)
				location = MinY (selectedItem.Frame);
			else
				location = MaxY (selectedItem.Frame);	

			thePoint = new CGPoint (8.0f, 8.0f);
			
			topMarker = new NSRulerMarker (vertRuler, location, topImage, thePoint);
			topMarker.Removable = true;
			topMarker.RepresentedObject = STR_TOP_OBJ;

			vertRuler.Markers = new NSRulerMarker[] { bottomMarker,topMarker };
		}
		
		private void selectRect (ColorRect aColorRect)
		{
			if (selectedItem == aColorRect) 
				return;
			
			if (selectedItem != null)
				SetNeedsDisplayInRect (selectedItem.Frame);
			
			if (aColorRect == null) 
				selectedItem = null;
			else 
				if (rects.Contains (aColorRect))
					selectedItem = aColorRect;
			
			updateRulers ();
			
			if (selectedItem != null)
				SetNeedsDisplayInRect (selectedItem.Frame);
		}
		
		private void updateSelectedRectFromRulers() 
		{
			NSRulerView horizRuler;
			NSRulerView vertRuler;
			NSRulerMarker[] markers;
			nfloat m1Loc, m2Loc;
			CGRect newRect = CGRect.Empty;
			
			if (selectedItem == null) 
				return;
			
			horizRuler = EnclosingScrollView.HorizontalRulerView;
			markers = horizRuler.Markers;
			
			if (markers.Count() != 2) 
				return;
			
			m1Loc = markers [0].MarkerLocation;
			m2Loc = markers [1].MarkerLocation;
			
			if (m1Loc < m2Loc){
				newRect.Location = setOriginX(newRect, m1Loc);
				newRect.Size = setRectWidth(newRect,m2Loc - m1Loc);
			} else {
				newRect.Location = setOriginX (newRect, m2Loc);
				newRect.Size = setRectWidth (newRect,m1Loc - m2Loc);
			}
			
			vertRuler = EnclosingScrollView.VerticalRulerView;
			markers = vertRuler.Markers;
			
			if (markers.Length != 2) 
				return;
			
			m1Loc = markers [0].MarkerLocation;
			m2Loc = markers [1].MarkerLocation;
			
			if (m1Loc < m2Loc) {
				newRect.Location = setOriginY (newRect, m1Loc);
				newRect.Size = setRectHeight (newRect, m2Loc - m1Loc);
			} else {
				newRect.Location = setOriginY (newRect, m2Loc);
				newRect.Size = setRectHeight (newRect, m1Loc - m2Loc);
			}
			
			SetNeedsDisplayInRect (selectedItem.Frame);
			selectedItem.Frame = newRect;
			SetNeedsDisplayInRect (newRect);
		}
		
		#endregion
		
		#region Static Rectangle Manipulation Methods
		
		static nfloat MinX (CGRect rect)
		{
			return (nfloat)Math.Min (rect.X, rect.Right);	
		}
		
		static nfloat MaxX (CGRect rect)
		{
			return (nfloat)Math.Max (rect.X, rect.Right);	
		}

		static nfloat MinY (CGRect rect)
		{
			return (nfloat)Math.Min (rect.Y, rect.Bottom);	
		}

		static nfloat MaxY (CGRect rect)
		{
			return (nfloat)Math.Max (rect.Y, rect.Bottom);	
		}
		
		static CGSize setRectWidth (CGRect rect, nfloat width)
		{
			CGSize size = rect.Size;
			size.Width = width;
			return size;
		}
		
		static CGSize setRectHeight (CGRect rect, nfloat height)
		{
			CGSize size = rect.Size;
			size.Height = height;
			return size;
		}
		
		static CGPoint setOriginX (CGRect rect, nfloat newOrigX)
		{
			CGPoint orig = rect.Location;
			orig.X = newOrigX;
			return orig;
		}
		
		static CGPoint setOriginY (CGRect rect, nfloat newOrigY)
		{
			CGPoint orig = rect.Location;
			orig.Y = newOrigY;
			return orig;
		}
		
		
		#endregion
		
		#region NSRulerView Delegate Client Methods
		
		/*
		 * NSRulerView Delegate Client Methods
		 */

		[Export("rulerView:shouldMoveMarker:")]
		public bool rulerViewShouldMoveMarker (NSRulerView aRulerView, NSRulerMarker aMarker)
		{
			if (selectedItem == null || selectedItem.IsLocked) return false;
			
			return true;
		}
		
		[Export("rulerView:willMoveMarker:toLocation:")]
		public nfloat rulerViewWillMoveMarker(NSRulerView aRulerView, NSRulerMarker aMarker, nfloat location)
		{
			NSEvent currentEvent;
			bool shifted;
			CGRect rect, dirtyRect;
			NSString theEdge = (NSString)aMarker.RepresentedObject;
			
			if (selectedItem == null) 
				return location;
			
			rect = selectedItem.Frame;
			dirtyRect = rect;
			dirtyRect.Size = setRectWidth (dirtyRect,rect.Width + 2.0f);  // fudge to counter hilite prob
			dirtyRect.Size = setRectHeight (dirtyRect, rect.Height + 2.0f);
			
			SetNeedsDisplayInRect (dirtyRect);
			
			currentEvent = NSApplication.SharedApplication.CurrentEvent;
			var eventFlags = currentEvent.ModifierFlags;
			
			shifted = (eventFlags & NSEventModifierMask.ShiftKeyMask) == NSEventModifierMask.ShiftKeyMask;
			
			if (!shifted) {
				switch (theEdge.ToString()){
				case STR_LEFT:
					if (location > MaxX(rect) - MIN_SIZE)
						location = MaxX(rect) - MIN_SIZE;
					
					rect.Size = setRectWidth(rect, MaxX(rect) - location);
					rect.Location = setOriginX(rect,location);
					break;

				case STR_RIGHT:
					if (location < MinX(rect) + MIN_SIZE)
						location = MinX(rect) + MIN_SIZE;
					
					rect.Size = setRectWidth(rect, location - MinX(rect));
					break;	

				case STR_TOP:
					if (this.IsFlipped) {
						if (location > MaxY(rect) - MIN_SIZE)
							location = MaxY(rect) - MIN_SIZE;
					
						rect.Size = setRectHeight(rect, MaxY(rect) - location);
						rect.Location = setOriginY(rect,location);
					} else {
						if (location < MinY(rect) + MIN_SIZE)
							location = MinY(rect) + MIN_SIZE;
					
						rect.Size = setRectHeight(rect, location - MinY(rect));
					}
					break;						

				case STR_BOTTOM:
					if (this.IsFlipped) {
						if (location < MinY(rect) + MIN_SIZE)
							location = MinY(rect) + MIN_SIZE;
					
						rect.Size = setRectHeight(rect, location - MinY(rect));
					} else {
						if (location > MaxY(rect) - MIN_SIZE)
							location = MaxY(rect) - MIN_SIZE;
					
						rect.Size = setRectHeight(rect, MaxY(rect) - location);
						rect.Location = setOriginY(rect,location);
					}
					break;						
				}
			} else {
				NSRulerMarker [] markers = aRulerView.Markers;
				NSRulerMarker otherMarker;
				
				otherMarker = markers [0];
				
				if (otherMarker == aMarker)
					otherMarker = markers [1];
				
				switch (theEdge.ToString ())
				{
				case STR_LEFT:
					rect.Location = setOriginX (rect, location);
					otherMarker.MarkerLocation = MaxX (rect);
					break;
				case STR_RIGHT:
					rect.Location = setOriginX (rect, location - rect.Width);
					otherMarker.MarkerLocation = MinX (rect);
					break;
				case STR_TOP:
					if (this.IsFlipped) 
					{
						rect.Location = setOriginY (rect,location);
						otherMarker.MarkerLocation = MaxY (rect);
					} else {
						rect.Location = setOriginY (rect, location - rect.Height);
						otherMarker.MarkerLocation = MinY (rect);
					}
					break;						
				case STR_BOTTOM:
					if (this.IsFlipped)  {
						rect.Location = setOriginY (rect,location - rect.Height);
						otherMarker.MarkerLocation = MinY (rect);
					} else {
						rect.Location = setOriginY (rect,location);
						otherMarker.MarkerLocation = MaxY (rect);
					}
					break;								
				}
			}
			
			selectedItem.Frame = rect;
			SetNeedsDisplayInRect (rect);
			
			return location;	
		}
		
		[Export("rulerView:didMoveMarker:")]
		public void rulerViewDidMoveMarker (NSRulerView aRulerView, NSRulerMarker aMarker)
		{
			updateSelectedRectFromRulers ();
		}
		
		[Export("rulerView:shouldRemoveMarker:")]
		public bool rulerViewShouldRemoveMarker (NSRulerView aRulerView, NSRulerMarker aMarker)
		{
			if (selectedItem != null && !selectedItem.IsLocked) 
				return true;
			
			return false;
		}

		[Export("rulerView:didRemoveMarker:")]
		public void rulerViewDidRemoveMarker (NSRulerView aRulerView, NSRulerMarker aMarker)
		{
			if (selectedItem == null) 
				return;
			
			SetNeedsDisplayInRect (selectedItem.Frame);
			rects.Remove (selectedItem);
			selectedItem = null;
			updateRulers ();
		}
	
		[Export("rulerView:shouldAddMarker:")]
		public bool rulerViewShouldAddMarker (NSRulerView aRulerView, NSRulerMarker aMarker)
		{
			return true;	
		}
		
		[Export("rulerView:willAddMarker:toLocation:")]
		public float rulerViewWillAddMarker (NSRulerView aRulerView, NSRulerMarker aMarker, float location)
		{
			return location;	
		}
			
		[Export("rulerView:didAddMarker:")]
		public void rulerViewDidAddMarker (NSRulerView aRulerView, NSRulerMarker aMarker)
		{
			nfloat theOtherCoord;
			CGRect newRect;
			NSColor newColor;
			ColorRect newColorRect;
			
			var visibleRect = VisibleRect();
			
			aMarker.Removable = true;
			
			if (aRulerView.Orientation == NSRulerOrientation.Horizontal){
				theOtherCoord = MaxY (visibleRect) - 165.0f;
				newRect = new CGRect (aMarker.MarkerLocation, theOtherCoord, 115.0f, 115.0f);
			} else {
				if (IsFlipped) {
					theOtherCoord = MinX (visibleRect) + 50.0f;
					newRect = new CGRect(theOtherCoord, aMarker.MarkerLocation, 115.0f, 115.0f);
				} else {
					theOtherCoord = MinX(visibleRect) + 50.0f;
					newRect = new CGRect (theOtherCoord, aMarker.MarkerLocation - 115.0f, 115.0f, 115.0f);
				}
			}
				
			newColor = NSColor.FromDeviceRgba ((float)rand.NextDouble (),
			                                        (float)rand.NextDouble (),
			                                        (float)rand.NextDouble (),
			                                        1.0f);
			newColorRect = new ColorRect (newRect, newColor);
			rects.Add (newColorRect);
			selectRect (newColorRect);
		}		
		
		[Export("rulerView:handleMouseDown:")]
		public void rulerViewDidRemoveMarker (NSRulerView aRulerView, NSEvent theEvent)
		{
			NSRulerMarker newMarker;
			
			if (aRulerView.Orientation == NSRulerOrientation.Horizontal)
				newMarker = new NSRulerMarker (aRulerView, 0.0f, leftImage, CGPoint.Empty);
			else
				newMarker = new NSRulerMarker (aRulerView, 0.0f, topImage, new CGPoint (8.0f,8.0f));
			
			aRulerView.TrackMarker (newMarker, theEvent);
		}	
		
		[Export("rulerView:willSetClientView:")]
		public void rulerViewWillSetClientView (NSRulerView aRulerView, NSView newClient)
		{
			return;
		}		
		
		#endregion
	}
}

