[CmdletBinding()]
param(
    [string]
    $OutputPath
)

if ($null -eq $OutputPath) {
    throw "Output path is null"
}

if ([string]::IsNullOrEmpty($OutputPath.Trim())) {
    throw "Output path is empty"
}
    
# git log --pretty=format:'%H%x09%cn%x09%ce%x09%cI%x09%s' | select -First 10

$columns = @(
    [PSCustomObject]@{ 
        ColumnName = 'Commit hash'; 
        FormatSpecifier = '%H'
    },
    [PSCustomObject]@{ 
        ColumnName = 'Commit author name'; 
        FormatSpecifier = '%cn'
    },
    [PSCustomObject]@{ 
        ColumnName = 'Commit author email'; 
        FormatSpecifier = '%ce'
    },
    [PSCustomObject]@{ 
        ColumnName = 'Commit time'; 
        FormatSpecifier = '%cI'
    },
    [PSCustomObject]@{ 
        ColumnName = 'Commit subject'; 
        FormatSpecifier = '%s'
    } 
)

$columnSeparatorFormatSpecifier = '%x09'
$columnHeaderFormatSpecifiers = $columns | ForEach-Object { $_.FormatSpecifier }
$columnHeaderNameSeparator = "`t"
$columnHeaderNames = $columns | ForEach-Object { $_.ColumnName }

Write-Host "Collecting commit data"
$gitLog = git log --pretty=format:$($columnHeaderFormatSpecifiers -join $columnSeparatorFormatSpecifier)

Write-Host "Writing headers to data file"
Set-Content -Path $OutputPath -Value ($columnHeaderNames -join $columnHeaderNameSeparator)

Write-Host "Writing commit data to file"
Add-Content -Path $OutputPath -Value $gitLog

Write-Host "Done"
