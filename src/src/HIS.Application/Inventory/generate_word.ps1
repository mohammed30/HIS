$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Add()
$selection = $word.Selection

# Set title
$selection.Style = "Title"
$selection.ParagraphFormat.Alignment = 1 # Center
$selection.TypeText("HIS System Implementation Task List")
$selection.TypeParagraph()

# Read the content and add to doc
$content = Get-Content "c:\Users\Mohammed\.gemini\antigravity\brain\180b0a22-8868-468f-a8ff-96bb58686e85\task.md" -Encoding UTF8

foreach ($line in $content) {
    if ($line.StartsWith("# ")) {
        $selection.Style = "Heading 1"
        $selection.TypeText($line.Substring(2))
        $selection.TypeParagraph()
    }
    elseif ($line.StartsWith("## ")) {
        $selection.Style = "Heading 2"
        $selection.TypeText($line.Substring(3))
        $selection.TypeParagraph()
    }
    elseif ($line.StartsWith("- [x]") -or $line.StartsWith("- [ ]")) {
        $selection.Style = "List Paragraph"
        $selection.TypeText($line)
        $selection.TypeParagraph()
    }
    elseif ($line.Trim() -ne "") {
        $selection.Style = "Normal"
        $selection.TypeText($line)
        $selection.TypeParagraph()
    }
}

$outputPath = "c:\Users\Mohammed\Desktop\HIS_Task_List.docx"
$doc.SaveAs($outputPath)
$doc.Close()
$word.Quit()
Write-Host "Word document created at: $outputPath"
