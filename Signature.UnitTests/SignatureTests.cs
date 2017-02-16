using NUnit.Framework;
using System;


namespace Signature.UnitTests
{
    [TestFixture]
    class SignatureTests
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


    }
}
