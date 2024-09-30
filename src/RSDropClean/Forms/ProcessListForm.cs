using System.Diagnostics;

namespace RSDropClean.Forms
{
  public partial class ProcessListForm : Form
  {
    public event Action<Process> OnProcessSelected = delegate { };

    public ProcessListForm()
    {
      InitializeComponent();

      Text = "Process List";
      Size = new Size(400, 300);

      var processListView = new ListView
      {
        Dock = DockStyle.Fill,
        View = View.Details,
        FullRowSelect = true
      };

      processListView.Columns.Add("Process Id", 100, HorizontalAlignment.Left);
      processListView.Columns.Add("Process Name", 150, HorizontalAlignment.Left);
      processListView.Columns.Add("Memory(KB)", 150, HorizontalAlignment.Left);

      var processName = @"Red Stone";
      var processes = Process.GetProcessesByName(processName);
      foreach (var process in processes)
      {
        var item = new ListViewItem(process.Id.ToString());
        item.SubItems.Add(process.ProcessName);
        item.SubItems.Add((process.WorkingSet64 / 1024).ToString());
        processListView.Items.Add(item);
      }

      Controls.Add(processListView);

      var selectButton = new Button
      {
        Text = "Open Process",
        Dock = DockStyle.Bottom,
      };

      selectButton.Click += (sender, e) =>
      {
        if (processListView.SelectedItems.Count <= 0)
          return;

        var selectedProcess = processes[processListView.SelectedItems[0].Index];
        OnProcessSelected(selectedProcess);
        Close();
      };

      Controls.Add(selectButton);
    }
  }
}
