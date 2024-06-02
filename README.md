# FolderShield

## Overview

FolderShield is a Windows-only application that provides automatic file encryption within a specified directory. This project is built using the .NET Framework and utilizes the Rfc2898DeriveBytes method for password-based encryption, combined with the AES algorithm for securing files.

**Note:** You may need to add this program to your antivirus's exception list to ensure it functions correctly. Currently, it has only been tested with .docx, .txt, and .pptx files.

## Features

- **GUI for Password Entry**: Easily set up the encryption password via a user-friendly graphical interface.
- **Directory Selection**: Choose the directory you want to monitor and encrypt files in.
- **Automatic Background Operation**: The encryption service runs as a Windows background service. Once the directory is selected and the password is set, any file edited and saved in the directory will be automatically encrypted.
- **Seamless File Access**: Double-clicking an encrypted file will decrypt it, allowing for immediate access.
- **Persistent Encryption**: Files remain encrypted if the service is stopped, and can only be decrypted when the service is restarted with the correct password.

## How It Works

1. **Setup**: Launch the application and use the GUI to:
   - Enter a password for encryption.
   - Select the directory to monitor.
   - Start the Windows service.
2. **File Encryption**: Any file within the selected directory that is edited and saved will be automatically encrypted.
3. **File Decryption**: Double-click an encrypted file to decrypt it temporarily.
4. **Service Control**: Stop the service to keep files encrypted. Files can only be accessed again by restarting the service with the same password.

## Installation

In the [Releases](https://github.com/KamilFicerman/FolderShield/releases/tag/v1.0.0) section, you will find the installer for FolderShield. Download and install the application easily from there.

## Demonstration

Below are video demonstrations showcasing the functionality of the application.
Installation            |  Demo                       
:-------------------------:|:-------------------------:
<img src="https://github.com/KamilFicerman/FolderShield/blob/master/gifs/installation.gif" width="200" height="500" />  |  <img src="https://github.com/KamilFicerman/FolderShield/blob/master/gifs/demonstration.gif" width="200" height="500" />
