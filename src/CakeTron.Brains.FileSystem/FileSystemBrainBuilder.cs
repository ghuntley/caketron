using System;
using System.IO;
using CakeTron.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CakeTron.Brains.FileSystem
{
    public static class FileSystemBrainBuilder
    {
        public static RobotBuilder UseFileSystemBrain(this RobotBuilder builder, string root)
        {
            if (string.IsNullOrWhiteSpace(root))
            {
                throw new ArgumentNullException(nameof(root));
            }

            // Configuration
            var configuration = new FileSystemBrainConfiguration { Root = new DirectoryInfo(root) };
            builder.Services.AddSingleton(configuration);

            // Provider
            builder.Services.AddSingleton<IBrainProvider, FileSystemBrain>();

            return builder;
        }
    }
}
