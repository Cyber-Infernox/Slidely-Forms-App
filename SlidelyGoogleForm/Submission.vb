Public Class Submission
    Public Property Name As String
    Public Property Email As String
    Public Property Phone As String
    Public Property github_link As String
    Public Property stopwatch_time As String

    ' Override ToString method to provide a custom string representation
    Public Overrides Function ToString() As String
        Return String.Format("Name: {0}, Email: {1}, Phone: {2}, GithubLink: {3}, StopwatchTime: {4}",
                              Name, Email, Phone, github_link, stopwatch_time)
    End Function
End Class

