using Raylib_cs;
using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;

public class FilePicker
{
    private string currentDirectory;
    private string selectedFile;
    private int scrollIndex = 0;

    private const int itemHeight = 30;
    private const int visibleItems = 13;

    public bool IsOpen { get; private set; } = false;

    public FilePicker(string startDirectory = null)
    {
        currentDirectory = startDirectory ?? Directory.GetCurrentDirectory();
    }

    public void Open(string startDirectory = null)
    {
        currentDirectory = startDirectory ?? Directory.GetCurrentDirectory();
        selectedFile = null;
        scrollIndex = 0;
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public string GetSelectedFile()
    {
        string temp = selectedFile;
        selectedFile = null;
        return temp;
    }

    public void Update()
    {
        if (!IsOpen) return;

        int maxScroll = Math.Max(0, GetItemCount() - visibleItems);
        float wheel = Raylib.GetMouseWheelMove();
        scrollIndex -= (int)wheel;
        scrollIndex = Math.Clamp(scrollIndex, 0, maxScroll);
    }

    public void Draw()
    {
        if (!IsOpen) return;

        Raylib.DrawRectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), new Color(0, 0, 0, 160));

        Raylib.DrawRectangle(50, 50, 700, 500, Color.RayWhite);
        Raylib.DrawRectangleLines(50, 50, 700, 500, Color.Black);

        Raylib.DrawText("Select File", 60, 60, 20, Color.Black);
        Raylib.DrawText(currentDirectory, 60, 90, 20, Color.DarkGray);

        // Back button
        Rectangle backBtn = new Rectangle(620, 60, 100, 30);
        Raylib.DrawRectangleRec(backBtn, Color.LightGray);
        Raylib.DrawRectangleLinesEx(backBtn, 1, Color.Black);
        Raylib.DrawText("Back", 645, 67, 16, Color.Black);

        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), backBtn) &&
            Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            GoUpDirectory();
        }

        // File/folder list
        var items = GetDirectoryItems();
        int startY = 130;

        for (int i = 0; i < visibleItems && (i + scrollIndex) < items.Count; i++)
        {
            int idx = i + scrollIndex;
            string item = items[idx];
            int y = startY + i * itemHeight;

            Rectangle itemRect = new Rectangle(60, y, 660, itemHeight);
            bool hovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), itemRect);

            Color bgColor = hovered ? Color.LightGray : Color.Gray;
            Raylib.DrawRectangleRec(itemRect, bgColor);
            Raylib.DrawRectangleLinesEx(itemRect, 1, Color.Black);
            Raylib.DrawText(item, 70, y + 5, 20, Color.Black);

            if (hovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                HandleItemClick(item);
            }
        }

        // Optional: cancel button
        Rectangle cancelBtn = new Rectangle(580, 520, 80, 25);
        Raylib.DrawRectangleRec(cancelBtn, Color.Red);
        Raylib.DrawRectangleLinesEx(cancelBtn, 1, Color.Black);
        Raylib.DrawText("Cancel", 595, 525, 16, Color.White);

        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), cancelBtn) &&
            Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            Close();
        }
    }

    private List<string> GetDirectoryItems()
    {
        List<string> items = new List<string>();
        try
        {
            string[] dirs = Directory.GetDirectories(currentDirectory);
            string[] files = Directory.GetFiles(currentDirectory);

            foreach (var dir in dirs)
                items.Add("[DIR] " + Path.GetFileName(dir));
            foreach (var file in files)
                items.Add(Path.GetFileName(file));
        }
        catch
        {
            items.Add("[Error reading directory]");
        }

        return items;
    }

    private int GetItemCount()
    {
        return GetDirectoryItems().Count;
    }

    private void HandleItemClick(string item)
    {
        if (item.StartsWith("[DIR] "))
        {
            string dirName = item.Substring(6);
            string fullPath = Path.Combine(currentDirectory, dirName);
            if (Directory.Exists(fullPath))
            {
                currentDirectory = fullPath;
                scrollIndex = 0;
            }
        }
        else
        {
            selectedFile = Path.Combine(currentDirectory, item);
            IsOpen = false;
        }
    }

    private void GoUpDirectory()
    {
        try
        {
            string parent = Directory.GetParent(currentDirectory)?.FullName;
            if (!string.IsNullOrEmpty(parent))
            {
                currentDirectory = parent;
                scrollIndex = 0;
            }
        }
        catch { }
    }
}
