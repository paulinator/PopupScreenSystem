using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PThomann.Utilities.PopupScreenSystem
{
	/// <summary>
	/// Modes for Popups.Show()
	/// </summary>
	public enum ShowMode
	{
		/// <summary>
		/// Replace the current popup by the new one, 
		/// with history forgetting it completely.
		/// </summary>
		Replace = 0,
		/// <summary>
		/// Replace the current popup by the new one, 
		/// with history remembering it as the previous page on the same layer.
		/// </summary>
		Next,
		/// <summary>
		/// Replace the current popup by the new one,
		/// with history remembering it as the last page on the previous layer.
		/// </summary>
		OnTop
	}

	/// <summary>
	/// The result of a MessageScreen (may be encountered in different screens too).
	/// </summary>
	public enum CustomScreenDialogResult
	{
		/// <summary>
		/// The user clicked Cancel or hit the back button.
		/// </summary>
		Cancel = 0,
		/// <summary>
		/// The user explicitly said NO.
		/// </summary>
		Deny,
		/// <summary>
		/// The user explicitly said YES.
		/// </summary>
		Accept
	}

	/// <summary>
	/// Identifies standard combinations of message screen buttons.
	/// </summary>
	public enum CustomScreenDialogButtons
	{
		/// <summary>
		/// No Buttons, Result always Cancel
		/// </summary>
		None = 0,
		/// <summary>
		/// Cancel Button, Result always Cancel
		/// </summary>
		Cancel = 1,
		/// <summary>
		/// OK Button, Result Cancel or Accept
		/// </summary>
		OK = 2,
		/// <summary>
		/// OK and Cancel Buttons, Result Cancel or Accept
		/// </summary>
		OkCancel = 3,
		/// <summary>
		/// Yes and No Buttons, Result Cancel, Deny or Accept
		/// </summary>
		YesNo = 4,
		/// <summary>
		/// Cancel, Yes and No Buttons, Result Cancel, Deny or Accept
		/// </summary>
		YesNoCancel = 5
	}
}
