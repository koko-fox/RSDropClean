namespace RSDropClean.Forms
{
  partial class DropCleanForm
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      openProcessListButton = new Button();
      dropCleanCheckBox = new CheckBox();
      itemListView = new ListView();
      index = new ColumnHeader();
      name = new ColumnHeader();
      SuspendLayout();
      // 
      // openProcessListButton
      // 
      openProcessListButton.AutoSize = true;
      openProcessListButton.Location = new Point(12, 12);
      openProcessListButton.Name = "openProcessListButton";
      openProcessListButton.Size = new Size(110, 25);
      openProcessListButton.TabIndex = 0;
      openProcessListButton.Text = "Open Process List";
      openProcessListButton.UseVisualStyleBackColor = true;
      // 
      // dropCleanCheckBox
      // 
      dropCleanCheckBox.AutoSize = true;
      dropCleanCheckBox.Location = new Point(128, 16);
      dropCleanCheckBox.Name = "dropCleanCheckBox";
      dropCleanCheckBox.Size = new Size(120, 19);
      dropCleanCheckBox.TabIndex = 1;
      dropCleanCheckBox.Text = "enable drop clean";
      dropCleanCheckBox.UseVisualStyleBackColor = true;
      // 
      // itemListView
      // 
      itemListView.CheckBoxes = true;
      itemListView.Columns.AddRange(new ColumnHeader[] { index, name });
      itemListView.Location = new Point(12, 43);
      itemListView.Name = "itemListView";
      itemListView.Size = new Size(460, 406);
      itemListView.TabIndex = 0;
      itemListView.UseCompatibleStateImageBehavior = false;
      itemListView.View = View.Details;
      // 
      // index
      // 
      index.Text = "Index";
      index.Width = 100;
      // 
      // name
      // 
      name.Text = "Name";
      name.Width = 300;
      // 
      // DropCleanForm
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(484, 461);
      Controls.Add(itemListView);
      Controls.Add(dropCleanCheckBox);
      Controls.Add(openProcessListButton);
      Name = "DropCleanForm";
      Text = "DropClean";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Button openProcessListButton;
    private CheckBox dropCleanCheckBox;
    private ListView itemListView;
    private ColumnHeader index;
    private ColumnHeader name;
  }
}
