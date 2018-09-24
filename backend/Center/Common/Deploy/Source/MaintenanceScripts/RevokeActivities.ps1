<#
    .SYNOPSIS
    This script resets

    Subnet
    Station
    iqube
#>
param
(
    [Parameter(Mandatory = $true, Position = 1)] [string] $CommSMessagingServiceAddress
)

begin
{
    Import-Module CenterCmdlets
}

process
{
    $binding = New-NetTcpBindingProxyConfiguration -RemoteAddress $CommSMessagingServiceAddress -SecurityMode None
    $subnets = @(1, 2, 3, 8)
    foreach($subnet in $subnets)
    {
        for($i = 1; $i -le 255; $i ++)
        { 
            $address = "A:$($subnet).$($i).1";
            Write-Host "Sending Revoke message to '$($address)'"
            $message = New-RevokeMessage -Address $address -ActivityId 0
            Send-CommsMessage -Message $message -ProxyConfiguration $binding
        }
    }
}

end
{
}