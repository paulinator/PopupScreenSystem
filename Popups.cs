using System;
using System.Collections.Generic;

namespace PThomann.Utilities.PopupScreenSystem
{
	/// <summary>
	/// Manages the PopupScreenSystem popups.
	/// Contains methods to display and hide popups,
	/// as well as a very basic navigation system
	/// that is easily included in an application
	/// by calling Popups.BackPress(e) from any 
	/// appropriate OnBackPress event.
	/// </summary>
	public static class Popups
	{
		static Dictionary<object, CustomScreenBase> dict = new Dictionary<object, CustomScreenBase>();
		static List<CustomScreenBase> popupStack = new List<CustomScreenBase>();
		static CustomScreenBase current;
		/// <summary>
		/// Retrieves the currently displaying popup, if any.
		/// </summary>
		public static CustomScreenBase Current { get { return current; } }

		private static Microsoft.Phone.Controls.PageOrientation pageOrientation;
		/// <summary>
		/// The current page orientation.
		/// </summary>
		public static Microsoft.Phone.Controls.PageOrientation PageOrientation
		{
			get { return pageOrientation; }
		}

		/// <summary>
		/// Show an arbitrary CustomScreens popup that has been prepared in code.
		/// </summary>
		/// <param name="popup">the popup</param>
		/// <param name="mode">the mode for popup navigation</param>
		public static void Show(CustomScreenBase popup, ShowMode mode)
		{
			CustomScreenBase tmp = current;
			current = popup;
			
			switch (mode)
			{
				case ShowMode.Next:
					current.LastScreen = tmp;
					break;
				case ShowMode.OnTop:
					if (tmp != null)
						popupStack.Add(tmp);
					break;
				case ShowMode.Replace:
					current.LastScreen = tmp.LastScreen;
					break;
				default:
					break;
			}
			if (tmp != null)
				tmp.Hide();
			current.Show(false);
		}
		/// <summary>
		/// Show THE instance of a CustomScreen&lt;T&gt; (stored by T, create if necessary), 
		/// initializing it with appropriate CustomScreenParameters&lt;T&gt;.
		/// </summary>
		/// <typeparam name="T">the CustomScreen-derived Type</typeparam>
		/// <param name="parameters">the parameters for that Type</param>
		/// <param name="mode">the mode for popup navigation</param>
		public static void Show<T>(CustomScreenParameters<T> parameters, ShowMode mode) where T : CustomScreen<T>
		{
			CustomScreenBase tmp = current;
			if (parameters.UniqueScope == null || !dict.ContainsKey(parameters.UniqueScope))
			{
				// if that screen uses the uniquescope feature, this happens automatically.
				current = (T)(typeof(T).GetConstructors()[0].Invoke(parameters.ConstructorParameters));
			}
			else
				current = dict[parameters.UniqueScope];
			
			switch (mode)
			{
				case ShowMode.Next:
					current.LastScreen = tmp;
					break;
				case ShowMode.OnTop:
					if (tmp != null)
						popupStack.Add(tmp);
					break;
				case ShowMode.Replace:
					current.LastScreen = tmp.LastScreen;
					break;
				default:
					break;
		}
			if (tmp != null)
				tmp.Hide();
			((T)current).Show(parameters);
		}
		/// <summary>
		/// Shows the single instance of CustomScreens for the uniqueScope if there is one.
		/// Will not create one if there is none!
		/// </summary>
		/// <param name="mode">the mode for back navigation</param>
		/// <param name="uniqueScope">the scope in which that Screen is unique</param>
		/// <returns>true if the Popup for this scope was found, false otherwise.</returns>
		public static bool Show(ShowMode mode, object uniqueScope)
		{
			if (dict.ContainsKey(uniqueScope))
			{
				Show(dict[uniqueScope], mode);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Show a MessageScreen, something similar to a MessageBox.
		/// </summary>
		/// <param name="title">the Title</param>
		/// <param name="message">the message</param>
		/// <param name="dialogButtons">what buttons to show</param>
		/// <param name="closeAction">the Action to perform when the message is closed.</param>
		public static MessageScreen ShowMessage(string title, string message, CustomScreenDialogButtons dialogButtons, Action closeAction)
		{
			MessageScreen ms = new MessageScreen(title, message);
			ms.Buttons = ms.MakeDialogButtons(dialogButtons).ToArray();
			ms.CloseAction = closeAction;
			Popups.Show(ms, ShowMode.OnTop);
			return ms;
		}

		/// <summary>
		/// Go back as if the back button had been pressed.
		/// </summary>
		public static void Back()
		{
			BackPress(new System.ComponentModel.CancelEventArgs());
		}
	
		/// <summary>
		/// Exit all popups.
		/// </summary>
		public static void HideAll()
		{
			if (current != null)
				current.Hide();
			current = null;
			popupStack.Clear();
		}

		/// <summary>
		/// Go back because the back button has been pressed.
		/// </summary>
		/// <param name="e">Cancellable</param>
		public static void BackPress(System.ComponentModel.CancelEventArgs e)
		{
			var screen = current;
			if (current == null)
				return; // no action required, no popups open, let page navigation do its work
			current.DoBackpress(e);
			if (e.Cancel)
				return;
			e.Cancel = true;

			if (current != screen)	// occurs if Popups are hidden / shown from within a CloseAction.
			{
				popupStack.Remove(screen);
				screen.Hide();
				return;
			}

			current.Hide();
			if (current.LastScreen != null)
			{
				current = current.LastScreen;
				current.Show(true);
			}
			else if (popupStack.Count > 0)
			{
				current = popupStack[popupStack.Count - 1];
				popupStack.RemoveAt(popupStack.Count - 1);
				current.Show(true);
			}
			else
				current = null;
		}

		/// <summary>
		/// Go one layer back, returning to the state 
		/// before the last Show(ShowMode.OnTop) 
		/// (or before all Show()s if OnTop was never used)
		/// </summary>
		public static void BackDown()
		{
			System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
			current.DoBackpress(e);
			if (e.Cancel)
				return;
			if (current != null)
				current.Hide();
			if (popupStack.Count > 0)
			{
				current = popupStack[popupStack.Count - 1];
				popupStack.RemoveAt(popupStack.Count - 1);
				current.Show(true);
			}
			else
				current = null;
		}

		/// <summary>
		/// Stores the current PageOrientation and propagates the change to CustomScreens.
		/// 
		/// You should call this from the Page_OrientationChanged of every page that uses the PopupScreenSystem.
		/// </summary>
		/// <param name="orient"></param>
		public static void OrientationChanged(Microsoft.Phone.Controls.PageOrientation orient)
		{
			pageOrientation = orient;
			if (current != null)
				current.PageOrientation = orient;
			foreach (CustomScreenBase cs in popupStack)
				cs.PageOrientation = orient;
		}

		/// <summary>
		/// Assigns a screen as THE global popup for the specified scope.
		/// </summary>
		/// <param name="popup">the screen. If you wish to delete an entry for a scope, pass null for screen and true for force.</param>
		/// <param name="scope">the scope</param>
		/// <param name="force">whether to overwrite existing assignment for this scope</param>
		/// <returns>false</returns>
		internal static bool SetPopupUniqueScope(CustomScreenBase popup, object scope, bool force)
		{
			if (scope == null)
				return false;
			if (dict.ContainsKey(scope))
			{
				if (force)
				{
					if (popup == null)
						dict.Remove(scope);
					else
						dict[scope] = popup;
				}
				else
					return false;
			}
			else if (popup == null)
				return false;
			else
				dict.Add(scope, popup);
			return true;
		}
	}
}
