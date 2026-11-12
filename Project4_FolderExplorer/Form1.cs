using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Project4_FolderExplorer;

public class Form1 : Form
{
    TreeView tree = new();
    ListView list = new();

    public Form1()
    {
        Text = "Folder Explorer";
        Size = new Size(1000, 650);
        StartPosition = FormStartPosition.CenterScreen;

        tree.Dock = DockStyle.Left;
        tree.Width = 350;
        tree.BeforeExpand += Tree_BeforeExpand;
        tree.AfterSelect += Tree_AfterSelect;

        list.Dock = DockStyle.Fill;
        list.View = View.Details;
        list.FullRowSelect = true;
        list.Columns.Add("Name", 300);
        list.Columns.Add("Size", 150);
        list.Columns.Add("Date Modified", 200);

        Controls.Add(list);
        Controls.Add(tree);

        LoadDrives();
    }

    void LoadDrives()
    {
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (!drive.IsReady) continue;
            TreeNode node = new(drive.Name);
            node.Tag = drive.RootDirectory.FullName;
            node.Nodes.Add("...");
            tree.Nodes.Add(node);
        }
    }

    void Tree_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
    {
        if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "...")
        {
            e.Node.Nodes.Clear();
            AddDirectories(e.Node, e.Node.Tag?.ToString() ?? "");
        }
    }

    void AddDirectories(TreeNode parent, string path)
    {
        try
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                try
                {
                    TreeNode n = new(Path.GetFileName(dir));
                    n.Tag = dir;
                    n.Nodes.Add("...");
                    parent.Nodes.Add(n);
                }
                catch { }
            }
        }
        catch { }
    }

    void Tree_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        string path = e.Node.Tag?.ToString() ?? "";
        list.Items.Clear();

        try
        {
            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo info = new(file);
                ListViewItem item = new(info.Name);
                item.SubItems.Add($"{info.Length} bytes");
                item.SubItems.Add(info.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss"));
                list.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Cannot access folder: " + ex.Message);
        }
    }
}
