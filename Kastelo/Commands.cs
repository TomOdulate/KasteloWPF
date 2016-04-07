using System.Windows.Input;

namespace Kastelo
{
    public static class Commands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
                (
                        "Exit",
                        "Exit",
                        typeof(Commands),
                        new InputGestureCollection(){new KeyGesture(Key.F4, ModifierKeys.Alt)}
                );
        public static readonly RoutedUICommand New = new RoutedUICommand
                (
                        "New Store",
                        "New",
                        typeof(Commands),
                        new InputGestureCollection() { new KeyGesture(Key.F3, ModifierKeys.Alt) }
                );
        public static readonly RoutedUICommand Open = new RoutedUICommand
                (
                        "Open",
                        "Open Store",
                        typeof(Commands),
                        new InputGestureCollection() { new KeyGesture(Key.F2, ModifierKeys.Alt) }
                );
        public static readonly RoutedUICommand Save = new RoutedUICommand
                (
                        "Save Store",
                        "Save",
                        typeof(Commands),
                        new InputGestureCollection() { new KeyGesture(Key.F5, ModifierKeys.Alt) }
                );
        public static readonly RoutedUICommand SetPassword = new RoutedUICommand
                (
                        "Set Password",
                        "SetPassword",
                        typeof(Commands),
                        new InputGestureCollection() { new KeyGesture(Key.F7, ModifierKeys.Alt) }
                );
        public static readonly RoutedUICommand ExportKey = new RoutedUICommand
                (
                        "Export encryption key",
                        "ExportKey",
                        typeof(Commands),
                        null
                );
        public static readonly RoutedUICommand ImportKey = new RoutedUICommand
                (
                        "Import encryption key",
                        "ImportKey",
                        typeof(Commands),
                        null
                );
        public static readonly RoutedUICommand CancelEdit = new RoutedUICommand
                (
                        "Cancel",
                        "CancelEdit",
                        typeof(Commands),
                        new InputGestureCollection() { new KeyGesture(Key.F8, ModifierKeys.Alt) }
                );
        public static readonly RoutedUICommand DoneEdit = new RoutedUICommand
                (
                        "Done",
                        "DoneEdit",
                        typeof(Commands),
                        new InputGestureCollection() { new KeyGesture(Key.F8, ModifierKeys.Alt) }
                );
        public static readonly RoutedUICommand GeneratePasswordButton = new RoutedUICommand
                (
                        "Generate Password",
                        "GeneratePasswordButton",
                        typeof(Commands),
                        new InputGestureCollection() { new KeyGesture(Key.F8, ModifierKeys.Alt) }
                );
        public static readonly RoutedUICommand AddApplication = new RoutedUICommand
                (
                        "Add Application",
                        "AddApplication",
                        typeof(Commands),
                        null
                );
        public static readonly RoutedUICommand AddCredential = new RoutedUICommand
                (
                        "Add Credential",
                        "AddCredential",
                        typeof(Commands),
                        null
                );
        public static readonly RoutedUICommand DeleteItem = new RoutedUICommand
                (
                        "",
                        "DeleteItem",
                        typeof(Commands),
                        null
                );
        public static readonly RoutedUICommand CopyPassword = new RoutedUICommand
                (
                        "Copy To Clipboard",
                        "CopyPassword",
                        typeof(Commands),
                        null
                );
        public static readonly RoutedUICommand ImportData = new RoutedUICommand
                (
                        "Import data",
                        "ImportData",
                        typeof(Commands),
                        null
                );
        public static readonly RoutedUICommand ExportData = new RoutedUICommand
                (
                        "Export data",
                        "ExportData",
                        typeof(Commands),
                        null
                );
    }
}