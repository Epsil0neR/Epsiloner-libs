using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Epsiloner.Wpf.Utils;

namespace Epsiloner.Wpf.KeyBinding
{
    /// <inheritdoc />
    public class KeyBindingConfigs : List<KeyBindingConfig>
    {
        /// <summary>
        /// Saves key binding configs as XAML information.
        /// The output can then be used to serialize key binding configs.
        /// </summary>
        /// <param name="path">The file to which you want to write.
        /// Creates a file at the specified path and writes to it in XML 1.0 text syntax.
        /// The outputFileName must be a file system path.</param>
        /// <remarks>Wrap for <see cref="XamlUtils.Save"/> method.</remarks>
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
        public void Save(string path)
        {
            XamlUtils.Save(this, path);
        }

        /// <summary>
        /// Loads key binding configs from file. If file at specified path contains different data, but can read it - returns null.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Loaded configs.</returns>
        /// <remarks>Wrap for <see cref="XamlUtils.Load{T}"/> method.</remarks>
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
        public static KeyBindingConfigs Load(string path)
        {
            return XamlUtils.Load<KeyBindingConfigs>(path);
        }
    }
}