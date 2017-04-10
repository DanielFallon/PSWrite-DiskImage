function Get-TabCompletionDisks {
    $disks = Get-WmiObject Win32_DiskDrive | ForEach-Object {
        $_.DeviceID
    }
    return $disks
}
function Write-DiskImage {
    [CmdletBinding()]
    Param(
        [System.IO.FileInfo]
        $DiskImage
    )
    DynamicParam{
        # Disk Param
        New-DynamicParam -Name Disk -ValidateSet (Get-TabCompletionDisks) -Mandatory -Position 1

    }
    Begin{
        Get-BinaryContent -FilePath $DiskImage | Set-BinaryContent -FilePath $Disk
    }
}

Export-ModuleMember -Function Write-DiskImage