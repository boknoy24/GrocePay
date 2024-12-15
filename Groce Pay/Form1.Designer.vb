<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.LoginButton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.myEmail = New System.Windows.Forms.TextBox()
        Me.myPassword = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'LoginButton
        '
        Me.LoginButton.ForeColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.LoginButton.Location = New System.Drawing.Point(141, 134)
        Me.LoginButton.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.LoginButton.Name = "LoginButton"
        Me.LoginButton.Size = New System.Drawing.Size(137, 78)
        Me.LoginButton.TabIndex = 0
        Me.LoginButton.Text = "Login"
        Me.LoginButton.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(39, 26)
        Me.Label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(57, 24)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Email"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 78)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(92, 24)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Password"
        '
        'myEmail
        '
        Me.myEmail.Location = New System.Drawing.Point(127, 20)
        Me.myEmail.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.myEmail.Name = "myEmail"
        Me.myEmail.Size = New System.Drawing.Size(174, 34)
        Me.myEmail.TabIndex = 3
        '
        'myPassword
        '
        Me.myPassword.Location = New System.Drawing.Point(127, 78)
        Me.myPassword.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.myPassword.Name = "myPassword"
        Me.myPassword.Size = New System.Drawing.Size(174, 34)
        Me.myPassword.TabIndex = 6
        Me.myPassword.UseSystemPasswordChar = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(11.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(394, 236)
        Me.Controls.Add(Me.myPassword)
        Me.Controls.Add(Me.myEmail)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.LoginButton)
        Me.Font = New System.Drawing.Font("Gadugi", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.Text = "GrocePay System"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LoginButton As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents myEmail As TextBox
    Friend WithEvents myPassword As TextBox
End Class
