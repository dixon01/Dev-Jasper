Get-PSSnapin -Registered SqlServerCmdletSnapin100 | Add-PSSnapin -ErrorAction SilentlyContinue

$query = "SELECT * FROM [Entries] WHERE [SessionId] = 1 ORDER BY [EntryId], [TickCount] ASC"
$values = Invoke-Sqlcmd -ServerInstance "." -Database "PerformanceLogger"`
    -Username "PerformanceLogger" -Password "Gorba_Performance" -Query $query

$groups = $values | Group-Object "EntryId"

foreach($group in $groups)
{
    $ticks = $group.Group[1].TickCount  - $group.Group[0].TickCount 
    Write-Host "Message with Id $($group.Name) in $($ticks) ticks"
}