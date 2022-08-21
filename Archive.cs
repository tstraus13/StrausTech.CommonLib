using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace CommonLib;
/// <summary>
/// Class to create file archives and/or compress files into
/// Zip, Tar, Tar.Gz, Gz, and Bz2
/// </summary>
public static class Archive
{
    /// <summary>
    /// Compress and Archive a file using Zip
    /// </summary>
    /// <param name="inputPath">The file path you want to archive</param>
    /// <param name="outputPath">The Zip file path you want to create</param>
    /// <param name="compressionLevel">The compression level</param>
    public static void ZipFile(string inputPath, string outputPath, int compressionLevel = 3)
    {
        FileInfo outFileInfo = new FileInfo(outputPath);
        FileInfo inFileInfo = new FileInfo(inputPath);

        // Create the output directory if it does not exist
        if (!Directory.Exists(outFileInfo.Directory.FullName))
            Directory.CreateDirectory(outFileInfo.Directory.FullName);

        using FileStream fsOut = File.Create(outputPath);
        using ZipOutputStream zipStream = new ZipOutputStream(fsOut);
        zipStream.SetLevel(compressionLevel);

        ZipEntry newEntry = new ZipEntry(inFileInfo.Name)
        {
            DateTime = inFileInfo.LastWriteTime//DateTime.UtcNow;
        };

        zipStream.PutNextEntry(newEntry);

        byte[] buffer = new byte[4096];
        using FileStream streamReader = File.OpenRead(inputPath);
        StreamUtils.Copy(streamReader, zipStream, buffer);

        zipStream.CloseEntry();
        zipStream.IsStreamOwner = true;
        zipStream.Close();
    }

    /// <summary>
    /// Extract a Zip Archive to a directory
    /// </summary>
    /// <param name="zipFileName">The name/path of the zip file</param>
    /// <param name="targetDir">The location you would like to place the extracted file</param>
    /// <param name="password">Optional password used to encrypt the zip archive</param>
    public static void ExtractZipFile(string zipFileName, string targetDir, string password = null)
    {
        if (!Path.IsPathFullyQualified(targetDir))
            return;

        using (Stream fs = File.OpenRead(zipFileName))
        using (ZipFile zf = new ZipFile(fs))
        {

            if (!string.IsNullOrEmpty(password))
            {
                // AES encrypted entries are handled automatically
                zf.Password = password;
            }

            foreach (ZipEntry zipEntry in zf)
            {
                if (!zipEntry.IsFile)
                {
                    // Ignore directories
                    continue;
                }
                string entryFileName = zipEntry.Name;
                // to remove the folder from the entry:
                //entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here
                // to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.

                // Manipulate the output filename here as desired.
                var fullZipToPath = Path.Combine(Tools.PathRemoveTrailingSlash(targetDir), entryFileName);
                var directoryName = Path.GetDirectoryName(fullZipToPath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                // 4K is optimum
                var buffer = new byte[4096];

                // Unzip file in buffered chunks. This is just as fast as unpacking
                // to a buffer the full size of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (var zipStream = zf.GetInputStream(zipEntry))
                using (Stream fsOutput = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, fsOutput, buffer);
                }
            }
        }
    }

    /// <summary>
    /// Compress and Archive a file using Tar
    /// </summary>
    /// <param name="inputPath">The file path you want to archive</param>
    /// <param name="outputPath">The Tar file path you want to create</param>
    public static void TarFile(string inputPath, string outputPath)
    {
        FileInfo outFileInfo = new FileInfo(outputPath);
        FileInfo inFileInfo = new FileInfo(inputPath);

        // Create the output directory if it does not exist
        if (!Directory.Exists(outFileInfo.Directory.FullName))
            Directory.CreateDirectory(outFileInfo.Directory.FullName);

        using FileStream fsOut = File.Create(outputPath);
        using TarOutputStream tarStream = new TarOutputStream(fsOut, Encoding.Default);
        using FileStream streamReader = File.OpenRead(inputPath);
        TarEntry newEntry = TarEntry.CreateTarEntry(inFileInfo.Name);
        newEntry.Size = streamReader.Length;
        tarStream.PutNextEntry(newEntry);

        byte[] buffer = new byte[4096];

        StreamUtils.Copy(streamReader, tarStream, buffer);

        tarStream.CloseEntry();
        tarStream.IsStreamOwner = true;
        tarStream.Close();
    }

    /// <summary>
    /// Compress and Archive a file using Tar in conjuction with GZip
    /// </summary>
    /// <param name="inputPath">The file path you want to archive</param>
    /// <param name="outputPath">The Tar.Gz file path you want to create</param>
    /// <param name="compressionLevel">The compression level</param>
    public static void TarGzFile(string inputPath, string outputPath, int compressionLevel = 3)
    {
        FileInfo outFileInfo = new FileInfo(outputPath);
        FileInfo inFileInfo = new FileInfo(inputPath);

        // Create the output directory if it does not exist
        if (!Directory.Exists(outFileInfo.Directory.FullName))
            Directory.CreateDirectory(outFileInfo.Directory.FullName);

        using FileStream fsOut = File.Create(outputPath);
        using GZipOutputStream gzStream = new GZipOutputStream(fsOut);
        using TarOutputStream tarStream = new TarOutputStream(gzStream, Encoding.Default);
        gzStream.SetLevel(compressionLevel);

        using FileStream streamReader = File.OpenRead(inputPath);
        TarEntry newEntry = TarEntry.CreateTarEntry(inFileInfo.Name);
        newEntry.Size = streamReader.Length;
        tarStream.PutNextEntry(newEntry);

        byte[] buffer = new byte[4096];

        StreamUtils.Copy(streamReader, tarStream, buffer);

        tarStream.CloseEntry();
        tarStream.IsStreamOwner = true;
        tarStream.Close();

        gzStream.IsStreamOwner = true;
        gzStream.Close();
    }

    /// <summary>
    /// Compress and Archive a file using GZip
    /// </summary>
    /// <param name="inputPath">The file path you want to archive</param>
    /// <param name="outputPath">The GZip file path you want to create</param>
    /// <param name="compressionLevel">The compression level</param>
    public static void GZipFile(string inputPath, string outputPath, int compressionLevel = 3)
    {
        FileInfo outFileInfo = new FileInfo(outputPath);
        FileInfo inFileInfo = new FileInfo(inputPath);

        // Create the output directory if it does not exist
        if (!Directory.Exists(outFileInfo.Directory.FullName))
            Directory.CreateDirectory(outFileInfo.Directory.FullName);

        using FileStream fsOut = File.Create(outputPath);
        using GZipOutputStream gzStream = new GZipOutputStream(fsOut);

        gzStream.SetLevel(compressionLevel);
        byte[] buffer = new byte[4096];
        using FileStream streamReader = File.OpenRead(inFileInfo.FullName);
        StreamUtils.Copy(streamReader, gzStream, buffer);

        gzStream.IsStreamOwner = true;
        gzStream.Close();
    }

    /// <summary>
    /// Compress and Archive a file using BZip2
    /// </summary>
    /// <param name="inputPath">The file path you want to archive</param>
    /// <param name="outputPath">The BZip2 file path you want to create</param>
    public static void BZip2File(string inputPath, string outputPath)
    {
        FileInfo outFileInfo = new FileInfo(outputPath);
        FileInfo inFileInfo = new FileInfo(inputPath);

        // Create the output directory if it does not exist
        if (!Directory.Exists(outFileInfo.Directory.FullName))
            Directory.CreateDirectory(outFileInfo.Directory.FullName);

        using FileStream fsOut = File.Create(outputPath);
        using BZip2OutputStream bz2Stream = new BZip2OutputStream(fsOut);

        byte[] buffer = new byte[4096];
        using FileStream streamReader = File.OpenRead(inFileInfo.FullName);
        StreamUtils.Copy(streamReader, bz2Stream, buffer);

        bz2Stream.IsStreamOwner = true;
        bz2Stream.Close();
    }
}