using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LSLib.LS;
using LSLib.LS.Enums;

namespace Divine.CLI
{
    internal class CommandLinePackageProcessor
    {
        private static readonly CommandLineArguments Args = Program.argv;

        public static void Create()
        {
            CreatePackageResource();
        }

        public static void ListFiles(Func<AbstractFileInfo, bool> filter = null)
        {
            if (CommandLineActions.SourcePath == null)
            {
                CommandLineLogger.LogFatal("Cannot list package without source path", 1);
            }
            else
            {
                ListPackageFiles(CommandLineActions.SourcePath, filter);
            }
        }

        public static void ExtractSingleFile()
        {
            ExtractSingleFile(CommandLineActions.SourcePath, CommandLineActions.DestinationPath, CommandLineActions.PackagedFilePath);
        }

        private static void ExtractSingleFile(string packagePath, string destinationPath, string packagedPath)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                CommandLineLogger.LogFatal($"Failed to list package: {e.Message}", 2);
                CommandLineLogger.LogTrace($"{e.StackTrace}");
            }
        }

        private static void ListPackageFiles(string packagePath, Func<AbstractFileInfo, bool> filter = null)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                CommandLineLogger.LogFatal($"Failed to list package: {e.Message}", 2);
                CommandLineLogger.LogTrace($"{e.StackTrace}");
            }
        }

        public static void Extract(Func<AbstractFileInfo, bool> filter = null)
        {
            if (CommandLineActions.SourcePath == null)
            {
                CommandLineLogger.LogFatal("Cannot extract package without source path", 1);
            }
            else
            {
                string extractionPath = GetExtractionPath(CommandLineActions.SourcePath, CommandLineActions.DestinationPath);

                CommandLineLogger.LogInfo($"Extracting package: {CommandLineActions.SourcePath}");

                ExtractPackageResource(CommandLineActions.SourcePath, extractionPath, filter);
            }
        }

        public static void BatchExtract(Func<AbstractFileInfo, bool> filter = null)
        {
            string[] files = Directory.GetFiles(CommandLineActions.SourcePath, $"*.{Args.InputFormat}");

            foreach (string file in files)
            {
                string extractionPath = GetExtractionPath(file, CommandLineActions.DestinationPath);

                CommandLineLogger.LogInfo($"Extracting package: {file}");

                ExtractPackageResource(file, extractionPath, filter);
            }
        }

        private static string GetExtractionPath(string sourcePath, string destinationPath)
        {
            return Args.UsePackageName ? Path.Combine(destinationPath, Path.GetFileNameWithoutExtension(sourcePath) ?? throw new InvalidOperationException()) : CommandLineActions.DestinationPath;
        }

        private static void CreatePackageResource(string file = "")
        {
            if (string.IsNullOrEmpty(file))
            {
                file = CommandLineActions.DestinationPath;
                CommandLineLogger.LogDebug($"Using destination path: {file}");
            }

            var options = new PackageCreationOptions();
            options.Version = CommandLineActions.PackageVersion;

            Dictionary<string, object> compressionOptions = CommandLineArguments.GetCompressionOptions(Path.GetExtension(file)?.ToLower() == ".lsv" ? "zlib" : Args.CompressionMethod, options.Version);

            options.Compression = (CompressionMethod)compressionOptions["Compression"];
            options.FastCompression = (bool)compressionOptions["FastCompression"];

            var fast = options.FastCompression ? "Fast" : "Normal";
            CommandLineLogger.LogDebug($"Using compression method: {options.Compression.ToString()} ({fast})");

            var packager = new Packager();
            packager.CreatePackage(file, CommandLineActions.SourcePath, options);

            CommandLineLogger.LogInfo("Package created successfully.");
        }

        private static void ExtractPackageResource(string file = "", string folder = "", Func<AbstractFileInfo, bool> filter = null)
        {
            if (string.IsNullOrEmpty(file))
            {
                file = CommandLineActions.SourcePath;
                CommandLineLogger.LogDebug($"Using source path: {file}");
            }

            try
            {
                var packager = new Packager();

                string extractionPath = GetExtractionPath(folder, CommandLineActions.DestinationPath);

                CommandLineLogger.LogDebug($"Using extraction path: {extractionPath}");

                packager.UncompressPackage(file, extractionPath, filter);

                CommandLineLogger.LogInfo($"Extracted package to: {extractionPath}");
            }
            catch (Exception e)
            {
                CommandLineLogger.LogFatal($"Failed to extract package: {e.Message}", 2);
                CommandLineLogger.LogTrace($"{e.StackTrace}");
            }
        }
    }
}
