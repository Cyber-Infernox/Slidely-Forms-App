Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Public Class FormEditSubmission
    Private submissions As List(Of Submission)
    Private currentIndex As Integer
    Private stopwatch As Stopwatch = New Stopwatch()

    Public Sub New(index As Integer)
        InitializeComponent()
        Me.currentIndex = index
    End Sub

    Private Async Sub FormEditSubmission_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await LoadSubmissionsAsync()
        DisplaySubmission(currentIndex)
    End Sub

    Private Sub btnToggleStopwatch_Click(sender As Object, e As EventArgs) Handles btnToggleStopwatch.Click
        If stopwatch.IsRunning Then
            stopwatch.Stop()
        Else
            stopwatch.Start()
        End If
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        txtStopwatch.Text = String.Format("{0:hh\:mm\:ss}", stopwatch.Elapsed)
    End Sub

    Private Async Function LoadSubmissionsAsync() As Task
        Using client As New HttpClient()
            Try
                Dim response As HttpResponseMessage = Await client.GetAsync("http://localhost:3000/read")
                response.EnsureSuccessStatusCode()
                Dim json As String = Await response.Content.ReadAsStringAsync()
                submissions = JsonConvert.DeserializeObject(Of List(Of Submission))(json)
            Catch ex As HttpRequestException
                MessageBox.Show("Error fetching submissions: " & ex.Message)
            End Try
        End Using
    End Function

    Private Sub DisplaySubmission(index As Integer)
        If index >= 0 AndAlso index < submissions.Count Then
            currentIndex = index
            Dim submission = submissions(index)
            txtName.Text = submission.Name
            txtEmail.Text = submission.Email
            txtPhone.Text = submission.Phone
            txtGithubLink.Text = submission.github_link
            txtStopwatch.Text = submission.stopwatch_time
        End If
    End Sub

    Private Async Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim name = txtName.Text
        Dim email = txtEmail.Text
        Dim phone = txtPhone.Text
        Dim githubLink = txtGithubLink.Text
        Dim stopwatchTime = txtStopwatch.Text

        Await UpdateSubmissionAsync(name, email, phone, githubLink, stopwatchTime)
    End Sub

    Private Async Function UpdateSubmissionAsync(name As String, email As String, phone As String, githubLink As String, stopwatchTime As String) As Task
        Using client As New HttpClient()
            Try
                Dim formData As New With {
                    .name = name,
                    .email = email,
                    .phone = phone,
                    .github_link = githubLink,
                    .stopwatch_time = stopwatchTime
                }
                Dim json As String = JsonConvert.SerializeObject(formData)
                Dim content As New StringContent(json, Encoding.UTF8, "application/json")
                Dim response As HttpResponseMessage = Await client.PutAsync($"http://localhost:3000/update?email={Uri.EscapeDataString(email)}", content)
                response.EnsureSuccessStatusCode()
                MessageBox.Show("Submission updated successfully!")

                Me.Close()
            Catch ex As HttpRequestException
                MessageBox.Show("Error updating submission: " & ex.Message)
            End Try
        End Using
    End Function

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.Control Or Keys.T
                btnToggleStopwatch.PerformClick()
                Return True
            Case Keys.Control Or Keys.S
                btnSave.PerformClick()
                Return True
        End Select
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

End Class