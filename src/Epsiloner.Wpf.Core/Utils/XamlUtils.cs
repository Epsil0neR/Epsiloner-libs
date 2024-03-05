using System;
using System.IO;
using System.Security;
using System.Xaml;
using System.Xml;
using XamlWriter = System.Windows.Markup.XamlWriter;

namespace Epsiloner.Wpf.Utils
{
    /// <summary>
    /// Provides useful functionality to work with XAML.
    /// </summary>
    public static class XamlUtils
    {
        /// <summary>
        /// Saves data as XAML information.
        /// The output can then be used to serialize data.
        /// </summary>
        /// <param name="data">Data to write to file.</param>
        /// <param name="path">The file to which you want to write.
        /// Creates a file at the specified path and writes to it in XML 1.0 text syntax.
        /// The outputFileName must be a file system path.</param>
        /// 
        /// <exception cref="ArgumentNullException">The url value is null or whitespace.</exception>
        /// <exception cref="ArgumentException">The path parameter contains invalid characters, is empty, or contains only white spaces.</exception>
        /// <exception cref="IOException">The network name is not known.</exception>\
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission to required directories.</exception>
        /// <exception cref="PathTooLongException">
        /// The specified path exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters
        /// and file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="NotSupportedException">path contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        /// <exception cref="SecurityException">The application is not running in full trust.</exception>
        public static void Save(object data, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var dir = Path.GetDirectoryName(path);
            Directory.CreateDirectory(dir);

            var settings = new XmlWriterSettings { Indent = true };
            using (var writer = XmlWriter.Create(path, settings))
            {
                XamlWriter.Save(data, writer);
            }
        }

        /// <summary>
        /// Loads data from file. If file at specified path contains different data type, but cannot read it - returns null.
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T">Data type to load.</typeparam>
        /// <returns>Loaded data.</returns>
        ///
        /// <exception cref="ArgumentException">path is a zero-length string, contains only white space,
        /// or contains one or more invalid characters as defined by System.IO.Path.InvalidPathChars.</exception>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="PathTooLongException">The specified path exceed the system-defined maximum length.
        /// For example, on Windows-based platforms, paths must be less than 248 characters,
        /// and file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
        /// <exception cref="UnauthorizedAccessException">path specified a directory.-or- The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file specified in path was not found.</exception>
        /// <exception cref="NotSupportedException">path is in an invalid format.</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        public static T Load<T>(string path) where T : class
        {
            using (var reader = File.OpenRead(path))
            {
                return XamlServices.Load(reader) as T;
            }
        }
    }
}
