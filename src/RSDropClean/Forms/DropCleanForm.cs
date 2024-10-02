using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace RSDropClean.Forms;

public partial class DropCleanForm : Form
{
  private Process? targetProcess = null;
  private DropCleaner? dropCleaner = null;
  private DropCleanConfig config = new();
  private List<RSItem> items = [];
  private List<RSItemCategory>? itemCategories = [];
  private List<ListViewItem> allListViewItems = [];
  private string gameWindowTitle = string.Empty;

  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  private static extern bool SetWindowText(IntPtr hWnd, string lpString);

  public DropCleanForm()
  {
    InitializeComponent();

    FormClosed += (sender, e) =>
    {
      if (dropCleaner != null)
      {
        dropCleaner.Stop();
        dropCleaner.Close();
      }

      if (targetProcess != null)
      {
        var hWnd = targetProcess.MainWindowHandle;
        if (hWnd != IntPtr.Zero)
        {
          SetWindowText(hWnd, gameWindowTitle);
        }
      }
    };

    openProcessListButton.Click += (sender, e) =>
    {
      using var processListForm = new ProcessListForm();
      processListForm.OnProcessSelected += process =>
      {
        if (targetProcess != null)
        {
          var hWnd = targetProcess.MainWindowHandle;
          if (hWnd != IntPtr.Zero)
          {
            SetWindowText(hWnd, gameWindowTitle);
          }
        }

        targetProcess = process;
        if (targetProcess != null)
        {
          gameWindowTitle = process.MainWindowTitle;

          var hWnd = targetProcess.MainWindowHandle;
          if (hWnd != IntPtr.Zero)
          {
            SetWindowText(hWnd, gameWindowTitle + " -attached");
          }
        }
      };
      processListForm.ShowDialog();
    };

    dropCleanCheckBox.CheckedChanged += (sender, e) =>
    {
      if (dropCleaner != null)
      {
        dropCleaner.Stop();
        dropCleaner.Close();
        dropCleaner = null;
      }

      if (dropCleanCheckBox.Checked)
      {
        if (targetProcess == null)
          return;

        dropCleaner = new DropCleaner(targetProcess, config, 1000);
        dropCleaner.Start();
      }
    };

    itemListView.ItemChecked += (sender, e) =>
    {
      if (!e.Item.Focused)
        return;

      var item = e.Item;
      var willBeChecked = item.Checked;

      if (int.TryParse(item.SubItems[0].Text, out int id))
      {
        config.SetWillBeRemoved(id, willBeChecked);
      }
    };

    LoadItemData();
    SetupCategoryFilterDropDown();
    config.Load();

    foreach (var item in items)
    {
      var listItem = new ListViewItem(item.Id.ToString());
      listItem.SubItems.Add(item.Name);
      listItem.Checked = config.IsRemoveMarkdItem(item.Id);
      listItem.Tag = item;
      allListViewItems.Add(listItem);
    }

    itemListView.BeginUpdate();
    itemListView.Items.AddRange(allListViewItems.ToArray());
    itemListView.EndUpdate();
  }

  private void LoadItemData()
  {
    byte[] key = [0xB6, 0x98, 0x71, 0x4D, 0x72, 0x9E, 0x62, 0x6E, 0x7A, 0x72, 0xD7, 0xD3, 0xE6, 0x73, 0xA2, 0xB6, 0xB7, 0xCE, 0x56, 0xBC, 0x2A, 0x98, 0xC2, 0xD7, 0x6A, 0x58, 0x1C, 0xD7, 0xAE, 0x16, 0xAD, 0xB4, 0x5C, 0x96, 0xB1, 0x9B, 0x86, 0x96, 0x1E, 0xD8, 0xA9, 0xE1, 0x5A, 0xFD, 0xA2, 0xE1, 0xB1, 0x99, 0xF5, 0xBD, 0xD0, 0xED, 0xB6, 0xED, 0x6E, 0x75, 0xDC, 0xF0, 0xC8, 0xAA, 0xFE, 0x56, 0x68, 0x16, 0x66, 0x5C, 0xF6, 0xC4, 0x62, 0x73, 0x4E];
    var encryptor = new XorEncryptor(key);
    var shiftJisEncoding = Encoding.GetEncoding("shift-jis");
    byte[] buffer = [];

    try
    {
      buffer = File.ReadAllBytes(@"./Data/Scenario/Red Stone/item.dat");
      var itemCountBuffer = buffer[4..12];
      itemCountBuffer = encryptor.ApplyXor(itemCountBuffer);
      int itemCount = BitConverter.ToInt32(itemCountBuffer);
      int itemDataSize = 0x01AA;

      for (int i = 0; i < itemCount; i++)
      {
        var startAddr = i * itemDataSize + 0xC;
        int endAddr = startAddr + itemDataSize;
        var itemDataBuffer = buffer[startAddr..endAddr];
        itemDataBuffer = encryptor.ApplyXor(itemDataBuffer);

        var id = BitConverter.ToUInt16(itemDataBuffer[0..2]);
        var name = shiftJisEncoding.GetString(itemDataBuffer[0x04..0x36]);
        var category = BitConverter.ToUInt16(itemDataBuffer[0x4c..0x4e]);
        var rsItem = new RSItem(id, name, category);
        items.Add(rsItem);
      }
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.ToString(), "error");
    }
  }

  private void SetupCategoryFilterDropDown()
  {
    var json = File.ReadAllText("./itemCategories.json");
    itemCategories = JsonSerializer.Deserialize<List<RSItemCategory>>(json);

    categorySelectDropDown.Items.Add("All Items");

    if (itemCategories != null)
    {
      var categories = itemCategories.Select(category => category.Name).ToArray();
      categorySelectDropDown.BeginUpdate();
      categorySelectDropDown.Items.AddRange(categories);
      categorySelectDropDown.EndUpdate();
    }

    categorySelectDropDown.SelectedIndexChanged += (sender, e) =>
    {
      if (categorySelectDropDown.SelectedItem == null)
        return;

      if (categorySelectDropDown.SelectedIndex == 0)
      {
        itemListView.BeginUpdate();
        itemListView.Items.Clear();
        itemListView.Items.AddRange(allListViewItems.ToArray());
        itemListView.EndUpdate();
      }
      else
      {
        var selectedCategoryName = categorySelectDropDown.SelectedItem.ToString();
        if (selectedCategoryName != null)
        {
          FilterItemListView(selectedCategoryName);
        }
      }
    };
  }

  private void FilterItemListView(string categoryName)
  {
    itemListView.Items.Clear();

    var category = itemCategories?.FirstOrDefault(category => category.Name == categoryName);
    if (category == null)
      return;

    var filteredItems =
    allListViewItems
    .Where(item =>
    {
      if (item.Tag == null)
        return false;

      return ((RSItem)item.Tag).Category == category.Index;
    })
    .ToArray();

    itemListView.BeginUpdate();
    itemListView.Items.AddRange(filteredItems);
    itemListView.EndUpdate();
  }
}
