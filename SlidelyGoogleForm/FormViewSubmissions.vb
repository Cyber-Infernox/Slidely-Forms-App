Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Logging
Imports System.Text

Public Class FormViewSubmissions
    Private submissions As List(Of Submission)
    Private currentIndex As Integer

    Private Async Sub FormViewSubmissions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Fetch submissions from backend
        Await LoadSubmissionsAsync()
        DisplaySubmission(0)
    End Sub

    Private Async Function LoadSubmissionsAsync() As Task
        Using client As New HttpClient()
            Try
                Dim response As HttpResponseMessage = Await client.GetAsync("http://localhost:3000/read")
                response.EnsureSuccessStatusCode()
                Dim json As String = Await response.Content.ReadAsStringAsync()
                Debug.Print(json)
                submissions = JsonConvert.DeserializeObject(Of List(Of Submission))(json)
                Console.WriteLine(submissions(0).ToString)
            Catch ex As HttpRequestException
                MessageBox.Show("Error fetching submissions: " & ex.Message)
            End Try
        End Using
    End Function

    Private Sub DisplaySubmission(index As Integer)
        If index >= 0 AndAlso index < submissions.Count Then
            currentIndex = index
            Dim submission = submissions(index)
            Console.WriteLine(submission.ToString())
            txtName.Text = submission.Name
            txtEmail.Text = submission.Email
            txtPhone.Text = submission.Phone
            txtGithubLink.Text = submission.github_link
            txtStopwatch.Text = submission.stopwatch_time
        End If
    End Sub

    Private Sub btnPrevious_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        If currentIndex > 0 Then
            DisplaySubmission(currentIndex - 1)
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        If currentIndex < submissions.Count - 1 Then
            DisplaySubmission(currentIndex + 1)
        End If
    End Sub

    Private Async Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If currentIndex >= 0 AndAlso currentIndex < submissions.Count Then
            Dim submission = submissions(currentIndex)
            Dim confirmResult = MessageBox.Show("Are you sure you want to delete this submission?", "Confirm Delete", MessageBoxButtons.YesNo)
            If confirmResult = DialogResult.Yes Then
                Await DeleteSubmissionAsync(submission.Email)
                submissions.RemoveAt(currentIndex)
                If currentIndex >= submissions.Count Then
                    currentIndex = submissions.Count - 1
                End If
                If currentIndex >= 0 Then
                    DisplaySubmission(currentIndex)
                Else
                    ClearSubmissionDetails()
                End If
            End If
        End If
    End Sub

    Private Async Function DeleteSubmissionAsync(email As String) As Task
        Using client As New HttpClient()
            Try
                Dim request = New HttpRequestMessage(HttpMethod.Delete, "http://localhost:3000/submissions")
                Dim content = JsonConvert.SerializeObject(New With {.email = email})
                request.Content = New StringContent(content, Encoding.UTF8, "application/json")

                Dim response As HttpResponseMessage = Await client.SendAsync(request)
                response.EnsureSuccessStatusCode()
            Catch ex As HttpRequestException
                MessageBox.Show("Error deleting submission: " & ex.Message)
            End Try
        End Using
    End Function

    Private Sub ClearSubmissionDetails()
        txtName.Text = ""
        txtEmail.Text = ""
        txtPhone.Text = ""
        txtGithubLink.Text = ""
        txtStopwatch.Text = ""
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        Dim editForm As New FormEditSubmission(currentIndex)
        editForm.ShowDialog()
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.Control Or Keys.P
                btnPrevious.PerformClick()
                Return True
            Case Keys.Control Or Keys.E
                btnEdit.PerformClick()
                Return True
            Case Keys.Control Or Keys.N
                btnNext.PerformClick()
                Return True
            Case Keys.Control Or Keys.D
                btnDelete.PerformClick()
                Return True
        End Select
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
End Class
