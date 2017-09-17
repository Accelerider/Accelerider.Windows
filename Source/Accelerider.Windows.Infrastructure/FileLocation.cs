using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Accelerider.Windows.Infrastructure
{
    [DebuggerDisplay("{FullPath}")]
    public class FileLocation
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
        /// Gets a stirng representing the file name. 
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets a string representing the file extension. 
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// Initializes a instance of <see cref="FileLocation"/> with specified file full path.
        /// </summary>
        /// <param name="fileFullPath">A string representing the full path of the file. </param>
        public FileLocation(string fileFullPath)
        {
            var matchResult = RegexFileLocation.Match(fileFullPath);

            if (matchResult.Groups == null || matchResult.Groups.Count != 4) throw new ArgumentException($"The file path is not valid: {fileFullPath}", fileFullPath);

            FullPath = matchResult.Groups[0].Value;
            var temp = matchResult.Groups[1].Value;
            if (!string.IsNullOrEmpty(temp)) FolderPath = temp.Remove(temp.Length - 1); // Remove the "\" or "/" at the end. 
            FileName = matchResult.Groups[2].Value;
            FileExtension = matchResult.Groups[3].Value.ToLower();
        }

        public override string ToString() => FullPath;

        #region Equals
        public static bool operator ==(FileLocation left, FileLocation right) => left?.FullPath == right?.FullPath;

        public static bool operator !=(FileLocation left, FileLocation right) => !(left == right);

        protected bool Equals(FileLocation other) => string.Equals(FullPath, other.FullPath);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals(obj as FileLocation);
        }

        public override int GetHashCode() => FullPath != null ? FullPath.GetHashCode() : 0;
        #endregion

        public static implicit operator FileLocation(string filePath) => string.IsNullOrEmpty(filePath) ? null : new FileLocation(filePath);

        public static implicit operator string(FileLocation fileLocation) => fileLocation?.FullPath;
    }
}