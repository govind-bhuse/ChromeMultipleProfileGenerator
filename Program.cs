using System;
using System.IO;
using System.Windows.Forms;

namespace ChromeProfileGeneratorEnglish;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}

public sealed class MainForm : Form
{
    private readonly NumericUpDown startNumber = new() { Minimum = 1, Maximum = 999999, Value = 1 };
    private readonly TextBox prefix = new() { Text = "Chrome-" };
    private readonly NumericUpDown amount = new() { Minimum = 1, Maximum = 10000, Value = 10 };
    private readonly Label preview = new() { AutoSize = true };
    private readonly Button generate = new() { Text = "Generate Profiles", AutoSize = true };
    private readonly string outputFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Chrome Profiles");

    public MainForm()
    {
        Text = "Chrome Profile Generator";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = true;
        ClientSize = new System.Drawing.Size(390, 270);
        Font = new System.Drawing.Font("Segoe UI", 9F);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 2,
            RowCount = 6,
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 6; i++)
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

        AddRow(layout, 0, "Starting number:", startNumber);
        AddRow(layout, 1, "Profile prefix:", prefix);
        AddRow(layout, 2, "Number of profiles:", amount);
        AddRow(layout, 3, "First profile name:", preview);
        layout.Controls.Add(new Label { Text = "Output folder:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
        var folderLabel = new Label { Text = @"Desktop\Chrome Profiles", AutoSize = true, Anchor = AnchorStyles.Left };
        layout.Controls.Add(folderLabel, 1, 4);

        generate.Text = "Generate Profiles";
        generate.Width = 160;
        generate.Height = 36;
        generate.Anchor = AnchorStyles.None;
        layout.Controls.Add(generate, 0, 5);
        layout.SetColumnSpan(generate, 2);
        Controls.Add(layout);

        startNumber.ValueChanged += (_, _) => UpdatePreview();
        prefix.TextChanged += (_, _) => UpdatePreview();
        UpdatePreview();
        generate.Click += (_, _) => GenerateShortcuts();
    }

    private static void AddRow(TableLayoutPanel panel, int row, string caption, Control control)
    {
        panel.Controls.Add(new Label { Text = caption, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        control.Dock = DockStyle.Fill;
        if (control is NumericUpDown n) n.TextAlign = HorizontalAlignment.Left;
        panel.Controls.Add(control, 1, row);
    }

    private void UpdatePreview()
    {
        preview.Text = $"{prefix.Text}{(int)startNumber.Value:000}";
    }

    private static string? FindChrome()
    {
        string[] candidates =
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Google\Chrome\Application\chrome.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Google\Chrome\Application\chrome.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\Application\chrome.exe"),
        };
        foreach (var candidate in candidates)
            if (File.Exists(candidate)) return candidate;
        return null;
    }

    private void GenerateShortcuts()
    {
        string? chromePath = FindChrome();
        if (chromePath is null)
        {
            MessageBox.Show("Google Chrome was not found. Please install Chrome and try again.",
                "Chrome not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        string p = prefix.Text.Trim();
        if (string.IsNullOrWhiteSpace(p) || p.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            p.Contains('\\') || p.Contains('/'))
        {
            MessageBox.Show("Please enter a valid profile prefix.", "Invalid prefix",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int first = (int)startNumber.Value;
        int count = (int)amount.Value;
        if ((long)first + count - 1 > 999999)
        {
            MessageBox.Show("The profile numbering exceeds the supported limit.", "Invalid range",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Directory.CreateDirectory(outputFolder);
            Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType is null)
                throw new InvalidOperationException("Windows Script Host is unavailable.");

            dynamic shell = Activator.CreateInstance(shellType)!;
            for (int i = 0; i < count; i++)
            {
                int number = first + i;
                string profileName = $"{p}{number:000}";
                string shortcutPath = Path.Combine(outputFolder, profileName + ".lnk");
                string chromeProfileDirectory = $"HENG_{profileName}";

                dynamic shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = chromePath;
                shortcut.Arguments = $"--profile-directory=\"{chromeProfileDirectory}\"";
                shortcut.WorkingDirectory = Path.GetDirectoryName(chromePath);
                shortcut.Description = $"Google Chrome profile {profileName}";
                shortcut.IconLocation = chromePath + ",0";
                shortcut.Save();
            }

            MessageBox.Show($"{count} Chrome shortcuts were created successfully.\n\nLocation:\n{outputFolder}\n\nChrome creates/initializes each profile the first time its shortcut is opened.",
                "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("The shortcuts could not be created.\n\n" + ex.Message,
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
