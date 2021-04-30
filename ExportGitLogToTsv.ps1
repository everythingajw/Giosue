# Giosue language interpreter
# The interpreter for the Giosue programming language.
# Copyright (C) 2021  Anthony Webster
# 
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
# 
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
# 
# You should have received a copy of the GNU General Public License along
# with this program; if not, write to the Free Software Foundation, Inc.,
# 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

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
