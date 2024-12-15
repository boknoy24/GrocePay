Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Public Class AdminDB
    Dim conn As New MySqlConnection("Server=localhost;Database=grocepay;Uid=root;Pwd=;")

    ' Save Employee Data
    Private Sub mySave_Click(sender As Object, e As EventArgs) Handles mySave.Click
        Dim name As String = myEmployeeName.Text.Trim()
        Dim email As String = myEmployeeEmail.Text.Trim()
        Dim password As String = myEmployeePassword.Text.Trim()
        Dim hashedPassword As String = HashPassword(password)
        Dim role As String = "employee"

        ' Step 1: Validate Email Format
        If Not IsValidEmail(email) Then
            MessageBox.Show("Invalid email format.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Step 2: Check if Email Already Exists
        If IsEmailExists(email) Then
            MessageBox.Show("This email is already registered.", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            ' Open connection if it's closed
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If

            ' Step 3: Insert into Users Table
            Dim userQuery As String = "INSERT INTO users (email, password, role) VALUES (@Email, @Password, @Role)"
            Using userCmd As New MySqlCommand(userQuery, conn)
                userCmd.Parameters.AddWithValue("@Email", email)
                userCmd.Parameters.AddWithValue("@Password", hashedPassword)
                userCmd.Parameters.AddWithValue("@Role", role)
                userCmd.ExecuteNonQuery()
            End Using

            ' Retrieve the last inserted User ID
            Dim userId As Integer
            Using cmd As New MySqlCommand("SELECT LAST_INSERT_ID()", conn)
                userId = Convert.ToInt32(cmd.ExecuteScalar())
            End Using

            ' Step 4: Insert into Employees Table
            Dim empQuery As String = "INSERT INTO employees (user_id, name, time_in, total_sales, time_out, date) VALUES (@UserId, @Name, NULL, NULL, NULL, NOW())"
            Using empCmd As New MySqlCommand(empQuery, conn)
                empCmd.Parameters.AddWithValue("@UserId", userId)
                empCmd.Parameters.AddWithValue("@Name", name)
                empCmd.ExecuteNonQuery()
            End Using

            MessageBox.Show("Employee registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Refresh DataGridView
            LoadEmployeeData()

        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Ensure connection is closed after operation is complete
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    ' Function to Hash Password
    Private Function HashPassword(password As String) As String
        Using sha256 As New SHA256Managed()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function

    ' Function to Validate Email Format
    Private Function IsValidEmail(email As String) As Boolean
        Dim emailPattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
        Return Regex.IsMatch(email, emailPattern)
    End Function

    ' Function to Check if Email Already Exists in Database
    Private Function IsEmailExists(email As String) As Boolean
        Dim query As String = "SELECT COUNT(*) FROM users WHERE email = @Email"
        Using cmd As New MySqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@Email", email)
            ' Ensure connection is open before executing the query
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            Return count > 0
        End Using
    End Function

    ' Load Employee Data into DataGridView
    Private Sub LoadEmployeeData()
        Try
            ' Ensure the connection is open before executing the query
            If conn.State = ConnectionState.Closed Then
                conn.Open()
            End If

            Dim query As String = "SELECT e.name AS 'Name', u.email AS 'Email', u.role AS 'Role', e.time_in AS 'Time In', e.total_sales AS 'Total Sales', e.time_out AS 'Time Out', e.date AS 'Date' FROM employees e JOIN users u ON e.user_id = u.id"
            Using cmd As New MySqlCommand(query, conn)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()

                DataGridView1.Rows.Clear()

                While reader.Read()
                    DataGridView1.Rows.Add(
                        reader("Name").ToString(),
                        reader("Email").ToString(),
                        reader("Time In").ToString(),
                        reader("Total Sales").ToString(),
                        reader("Time Out").ToString(),
                        reader("Date").ToString()
                    )
                End While

                reader.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Ensure connection is closed after operation is complete
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub

    ' Form Load Event
    Private Sub AdminDB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadEmployeeData()
    End Sub

    ' Logout Event
    Private Sub myLogout_Click(sender As Object, e As EventArgs) Handles myLogout.Click
        Me.Close()
        Form1.Show()
    End Sub
End Class
