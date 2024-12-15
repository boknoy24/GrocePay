Imports System.Security.Cryptography
Imports System.Text
Imports MySql.Data.MySqlClient

Public Class Form1
    Dim conn As New MySqlConnection("Server=localhost;Database=grocepay;Uid=root;Pwd=;")

    Private Sub LoginButton_Click(sender As Object, e As EventArgs) Handles LoginButton.Click
        Dim email As String = myEmail.Text.Trim()
        Dim password As String = myPassword.Text.Trim()
        Dim hashedPassword As String = HashPassword(password)

        Dim query As String = "SELECT role FROM users WHERE email = @Email AND password = @Password"

        Try
            conn.Open()
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Email", email)
                cmd.Parameters.AddWithValue("@Password", hashedPassword)

                Dim role As Object = cmd.ExecuteScalar()

                If role IsNot Nothing Then
                    Select Case role.ToString().ToLower()
                        Case "admin"
                            MessageBox.Show("Login Successful!", "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            Dim adminForm As New AdminDB()
                            adminForm.Show()
                            Me.Hide()
                        Case "employee"
                            MessageBox.Show("Login Successful!", "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            Dim employeeForm As New EmployeeDB(email)
                            employeeForm.Show()
                            Me.Hide()
                        Case Else
                            MessageBox.Show("Unknown role.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Select
                Else
                    MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub

    Public Function HashPassword(password As String) As String
        Using sha256 As New SHA256Managed()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function
End Class
