@echo off
setlocal

REM Select the target folder in the full Explorer-style dialog, then click Open.
powershell.exe -NoProfile -ExecutionPolicy Bypass -STA -Command "Add-Type -AssemblyName System.Windows.Forms; $dialog = New-Object System.Windows.Forms.OpenFileDialog; $dialog.Title = 'Open the target folder, then click Open'; $dialog.CheckFileExists = $false; $dialog.CheckPathExists = $true; $dialog.ValidateNames = $false; $dialog.FileName = 'Select this folder'; $dialog.Filter = 'Folders|*.folder'; if ($dialog.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) { exit }; $target = [System.IO.Path]::GetDirectoryName($dialog.FileName); $folders = @('Scripts', 'Scripts\Components', 'Scripts\Components\Actors', 'Scripts\Components\UIs', 'Scripts\Objects', 'Scripts\Interfaces', 'Scripts\Enums'); foreach ($folder in $folders) { New-Item -ItemType Directory -Force -Path (Join-Path $target $folder) | Out-Null }; [System.Windows.Forms.MessageBox]::Show(('Created Scripts folder structure in:' + [Environment]::NewLine + $target), 'Complete', 'OK', 'Information') | Out-Null; Start-Process explorer.exe (Join-Path $target 'Scripts')"

endlocal
