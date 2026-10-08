using System;
using NativeFileDialogNET;

public static class FileDialog
{
    public static string? OpenFile(
        string? filter = null,
        string? defaultPath = null)
    {
        using var dialog = new NativeFileDialog()
            .SelectFile();

        if (!string.IsNullOrEmpty(filter))
            dialog.AddFilter("Files", filter);

        var result = dialog.Open(out string? path, defaultPath);

        return result == DialogResult.Okay ? path : null;
    }

    public static string? OpenFolder(
        string? defaultPath = null)
    {
        using var dialog = new NativeFileDialog()
            .SelectFolder();

        var result = dialog.Open(out string? path, defaultPath);

        return result == DialogResult.Okay ? path : null;
    }

    public static string? SaveFile(
        string? filter = null,
        string? defaultPath = null)
    {
        using var dialog = new NativeFileDialog()
            .SaveFile();

        if (!string.IsNullOrEmpty(filter))
            dialog.AddFilter("Files", filter);

        var result = dialog.Open(out string? path, defaultPath);

        return result == DialogResult.Okay ? path : null;
    }

    public static string? SaveFolder( // same as OpenFolder
        string? defaultPath = null)
    {
        using var dialog = new NativeFileDialog()
            .SelectFolder();

        var result = dialog.Open(out string? path, defaultPath);

        return result == DialogResult.Okay ? path : null;
    }
}
