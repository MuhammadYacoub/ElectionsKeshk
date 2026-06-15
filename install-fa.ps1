$url = 'https://github.com/FortAwesome/Font-Awesome/releases/download/5.15.4/fontawesome-free-5.15.4-web.zip'
$zipPath = 'C:\Users\Keshk-Pc\Desktop\elections\wwwroot\lib\fa.zip'
$extractPath = 'C:\Users\Keshk-Pc\Desktop\elections\wwwroot\lib\fa-temp'
$destPath = 'C:\Users\Keshk-Pc\Desktop\elections\wwwroot\lib\fontawesome'

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

Write-Host "Downloading FontAwesome from GitHub..."
Invoke-WebRequest -Uri $url -OutFile $zipPath

Write-Host "Verifying download..."
if ((Get-Item $zipPath).length -lt 1MB) {
    Write-Host "Download failed. File is too small."
    exit 1
}

Write-Host "Extracting..."
Expand-Archive -Path $zipPath -DestinationPath $extractPath -Force

Write-Host "Copying files..."
if (Test-Path $destPath) { Remove-Item -Path $destPath -Recurse -Force }
New-Item -ItemType Directory -Force -Path $destPath

Copy-Item -Path "$extractPath\fontawesome-free-5.15.4-web\css" -Destination $destPath -Recurse -Force
Copy-Item -Path "$extractPath\fontawesome-free-5.15.4-web\webfonts" -Destination $destPath -Recurse -Force

Write-Host "Cleaning up..."
Remove-Item -Path $zipPath -Force
Remove-Item -Path $extractPath -Recurse -Force

Write-Host "FontAwesome Installation Complete!"
