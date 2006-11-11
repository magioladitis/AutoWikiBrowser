<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AssessmentComments
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.WebControl1 = New WikiFunctions.Browser.WebControl
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.ReferencesButton = New System.Windows.Forms.Button
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.CitationsButton = New System.Windows.Forms.Button
        Me.PhotoButton = New System.Windows.Forms.Button
        Me.CleanupButton = New System.Windows.Forms.Button
        Me.ExpansionButton = New System.Windows.Forms.Button
        Me.CopyeditButton = New System.Windows.Forms.Button
        Me.btnSave = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Label2 = New System.Windows.Forms.Label
        Me.SkipButton = New System.Windows.Forms.Button
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'WebControl1
        '
        Me.WebControl1.AllowNavigation = False
        Me.WebControl1.Busy = False
        Me.WebControl1.DiffFontSize = 150
        Me.WebControl1.EnhanceDiffEnabled = True
        Me.WebControl1.Location = New System.Drawing.Point(0, 0)
        Me.WebControl1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebControl1.Name = "WebControl1"
        Me.WebControl1.ScriptErrorsSuppressed = True
        Me.WebControl1.ScrollDown = True
        Me.WebControl1.Size = New System.Drawing.Size(807, 450)
        Me.WebControl1.TabIndex = 0
        Me.WebControl1.WebBrowserShortcutsEnabled = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.My.Resources.Resources.WP1
        Me.PictureBox1.Location = New System.Drawing.Point(12, 460)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(64, 61)
        Me.PictureBox1.TabIndex = 3
        Me.PictureBox1.TabStop = False
        '
        'ReferencesButton
        '
        Me.ReferencesButton.Location = New System.Drawing.Point(119, 460)
        Me.ReferencesButton.Name = "ReferencesButton"
        Me.ReferencesButton.Size = New System.Drawing.Size(75, 23)
        Me.ReferencesButton.TabIndex = 4
        Me.ReferencesButton.Text = "References"
        Me.ToolTip1.SetToolTip(Me.ReferencesButton, "Article needs references")
        Me.ReferencesButton.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 561)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(808, 22)
        Me.StatusStrip1.TabIndex = 5
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
        '
        'CitationsButton
        '
        Me.CitationsButton.Location = New System.Drawing.Point(119, 489)
        Me.CitationsButton.Name = "CitationsButton"
        Me.CitationsButton.Size = New System.Drawing.Size(75, 23)
        Me.CitationsButton.TabIndex = 6
        Me.CitationsButton.Text = "Citations"
        Me.ToolTip1.SetToolTip(Me.CitationsButton, "Article needs citations")
        Me.CitationsButton.UseVisualStyleBackColor = True
        '
        'PhotoButton
        '
        Me.PhotoButton.Location = New System.Drawing.Point(119, 518)
        Me.PhotoButton.Name = "PhotoButton"
        Me.PhotoButton.Size = New System.Drawing.Size(75, 23)
        Me.PhotoButton.TabIndex = 7
        Me.PhotoButton.Text = "Photo"
        Me.ToolTip1.SetToolTip(Me.PhotoButton, "Article needs a photo")
        Me.PhotoButton.UseVisualStyleBackColor = True
        '
        'CleanupButton
        '
        Me.CleanupButton.Location = New System.Drawing.Point(200, 460)
        Me.CleanupButton.Name = "CleanupButton"
        Me.CleanupButton.Size = New System.Drawing.Size(75, 23)
        Me.CleanupButton.TabIndex = 8
        Me.CleanupButton.Text = "Cleanup"
        Me.ToolTip1.SetToolTip(Me.CleanupButton, "Article needs cleanup")
        Me.CleanupButton.UseVisualStyleBackColor = True
        '
        'ExpansionButton
        '
        Me.ExpansionButton.Location = New System.Drawing.Point(200, 489)
        Me.ExpansionButton.Name = "ExpansionButton"
        Me.ExpansionButton.Size = New System.Drawing.Size(75, 23)
        Me.ExpansionButton.TabIndex = 9
        Me.ExpansionButton.Text = "Expansion"
        Me.ToolTip1.SetToolTip(Me.ExpansionButton, "Article is not comprehensive")
        Me.ExpansionButton.UseVisualStyleBackColor = True
        '
        'CopyeditButton
        '
        Me.CopyeditButton.Location = New System.Drawing.Point(200, 518)
        Me.CopyeditButton.Name = "CopyeditButton"
        Me.CopyeditButton.Size = New System.Drawing.Size(75, 23)
        Me.CopyeditButton.TabIndex = 10
        Me.CopyeditButton.Text = "Copyedit"
        Me.ToolTip1.SetToolTip(Me.CopyeditButton, "Article needs copyediting")
        Me.CopyeditButton.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Enabled = False
        Me.btnSave.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSave.Location = New System.Drawing.Point(681, 460)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(102, 32)
        Me.btnSave.TabIndex = 31
        Me.btnSave.Tag = "Save the Comments"
        Me.btnSave.Text = "Save"
        Me.ToolTip1.SetToolTip(Me.btnSave, "Save the Comments")
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(305, 460)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(230, 39)
        Me.Label1.TabIndex = 32
        Me.Label1.Text = "Need other boilerplate text items?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Need customised text items to save in setting" & _
            "s?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Let me know."
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(507, 502)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(276, 39)
        Me.Label2.TabIndex = 33
        Me.Label2.Text = "Please do NOT click the save button inside the" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "above browser window. Click the s" & _
            "ave button" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "above this label instead."
        '
        'SkipButton
        '
        Me.SkipButton.Location = New System.Drawing.Point(590, 460)
        Me.SkipButton.Name = "SkipButton"
        Me.SkipButton.Size = New System.Drawing.Size(75, 23)
        Me.SkipButton.TabIndex = 34
        Me.SkipButton.Text = "Skip"
        Me.SkipButton.UseVisualStyleBackColor = True
        '
        'AssessmentComments
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(808, 583)
        Me.Controls.Add(Me.SkipButton)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.CopyeditButton)
        Me.Controls.Add(Me.ExpansionButton)
        Me.Controls.Add(Me.CleanupButton)
        Me.Controls.Add(Me.PhotoButton)
        Me.Controls.Add(Me.CitationsButton)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ReferencesButton)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.WebControl1)
        Me.Name = "AssessmentComments"
        Me.ShowIcon = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Assessment Comments"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents WebControl1 As WikiFunctions.Browser.WebControl
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents ReferencesButton As System.Windows.Forms.Button
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents CitationsButton As System.Windows.Forms.Button
    Friend WithEvents PhotoButton As System.Windows.Forms.Button
    Friend WithEvents CleanupButton As System.Windows.Forms.Button
    Friend WithEvents ExpansionButton As System.Windows.Forms.Button
    Friend WithEvents CopyeditButton As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents SkipButton As System.Windows.Forms.Button
End Class
