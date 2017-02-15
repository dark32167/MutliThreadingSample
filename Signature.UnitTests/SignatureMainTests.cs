using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace Signature.UnitTests
{
    [TestFixture]
    public class SignatureMainTests
    {
        [TestCase("")]
        [TestCase("1")]
        [TestCase(@"ControlFilesForTesting\ExistsButEmptyFile.txt")]
        public void IsFilePathCorrect_InvalidExtensions_ReturnsFalse(string filePath)
        {
            //получение пути к контрольным файлам, так как по умолчанию обращение идет в другую дирректорию
            filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + filePath;

            bool result = SignatureMain.IsFilePathCorrect(filePath);            

            Assert.False(result);
        }

        [TestCase(@"ControlFilesForTesting\CorrectFile.txt")]
        [TestCase(@"ControlFilesForTesting\correctfile.txt")]
        [TestCase(@"ControlFilesForTesting\correctfile.TXT")]
        public void IsFilePathCorrect_ValidExtensions_ReturnsTrue(string filePath)
        {
            //получение пути к контрольным файлам, так как по умолчанию обращение идет в другую дирректорию
            filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + filePath;

            bool result = SignatureMain.IsFilePathCorrect(filePath);

            Assert.True(result);
        }
    }
}
