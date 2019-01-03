using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure
{
    [DebuggerDisplay("{" + nameof(FullPath) + "}")]
    public class FileLocator : IEquatable<FileLocator>
    {
        /// <summary>
        /// Gets a regular expression for splitting the file full path string.
        /// In the right case, the group will have four elements:
        /// [0]: FullPath
        /// [1]: FolderName
        /// [2]: FileName
        /// [3]: FileExtension
        /// </summary>
        private static readonly Regex RegexFileLocation = new Regex(@"^([\\/]?(?:\w:)?(?:[^\\/]+?[\\/])*?)([^\\/]+?(?:\.(\w+?))?)?$", RegexOptions.Compiled);


        /// <summary>
        /// Gets a string representing the full path of the file. 
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Gets a string representing the folder where the file is located. 
        /// </summary>
        public string FolderPath { get; }

        /// <summary>
        /// Gets a string representing the file name. 
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets a string representing the file extension. 
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// Initializes a instance of <see cref="FileLocator"/> with specified file full path.
        /// </summary>
        /// <param name="fileFullPath">A string representing the full path of the file. </param>
        public FileLocator(string fileFullPath)
        {
            var matchResult = RegexFileLocation.Match(fileFullPath);

            if (matchResult.Groups == null || matchResult.Groups.Count != 4)
                throw new ArgumentException($"The file path is not valid: {fileFullPath}", fileFullPath);

            FullPath = matchResult.Groups[0].Value;
            var temp = matchResult.Groups[1].Value;
            if (!string.IsNullOrEmpty(temp)) FolderPath = temp.Remove(temp.Length - 1); // Remove the "\" or "/" at the end. 
            FileName = matchResult.Groups[2].Value;
            FileExtension = matchResult.Groups[3].Value.ToLower();
        }

        public override string ToString() => FullPath;

        #region Implements Equals

        public bool Equals(FileLocator other) => !Equals(other, null) && string.Equals(FullPath, other.FullPath);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || Equals(obj as FileLocator);

        public static bool operator ==(FileLocator left, FileLocator right)
        {
            if ((object)left == null || (object)right == null)
                return Equals(left, right);

            return left.Equals(right);
        }

        public static bool operator !=(FileLocator left, FileLocator right) => !(left == right);

        public override int GetHashCode() => FullPath != null ? FullPath.GetHashCode() : 0;

        #endregion

        public static implicit operator FileLocator(string filePath) => string.IsNullOrEmpty(filePath) ? null : new FileLocator(filePath);

        public static implicit operator string(FileLocator fileLocation) => fileLocation?.FullPath;
    }
}