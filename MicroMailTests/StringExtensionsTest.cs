using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroMail.Infrastructure.Extensions;

namespace MicroMailTests
{
    [TestClass]
    public class StringExtensionsTest
    {
        private const string HeaderTestBody = "Test-Header-Name: Test header value; test-attribute-name=\"Test header attribute value.\"";
        private const string HeaderTestName = "test-header-name";
        private const string HeaderTestEtalon = "Test header value";
        private const string HeaderAttributeTestName = "test-attribute-name";
        private const string HeaderAttributeTestEtalon = "Test header attribute value.";

        private const string DecodeBase64WordTestBody = "QmFzZTY0IGRlY29kZSB0ZXN0LiDQotC10YHRgiDQtNC70Y8g0LrQuNGA0LjQu9C40YbRli4=";
        private const string DecodeQuotedWordTestBody = "Base64 decode test. =D0=A2=D0=B5=D1=81=D1=82 =D0=B4=D0=BB=D1=8F =D0=BA=D0=B8=D1=80=D0=B8=D0=BB=D0=B8=D1=86=D1=96.";
        private const string DecodeWordTemplate = "=?{0}?{1}?{2}?=";
        private const string DecodeCharset = "utf-8";
        private const string Base64Marker = "B";
        private const string QuotedPrintableMarker = "Q";
        private const string DecodeWordTestEtalon = "Base64 decode test. Тест для кирилиці.";
        
        [TestMethod]
        public void GetHeaderValueTest()
        {
            Assert.AreEqual(HeaderTestBody.GetHeaderValue(HeaderTestName), HeaderTestEtalon);
            Assert.AreEqual(HeaderTestBody.GetHeaderValue(HeaderTestName, HeaderAttributeTestName), HeaderAttributeTestEtalon);
        }

        [TestMethod]
        public void DecodeBase64Test()
        {
            Assert.AreEqual(DecodeBase64WordTestBody.DecodeBase64(DecodeCharset), DecodeWordTestEtalon);
        }

        [TestMethod]
        public void DecodeQuotedPrintableTest()
        {
            Assert.AreEqual(DecodeQuotedWordTestBody.DecodeQuotedPrintable(DecodeCharset), DecodeWordTestEtalon);
        }

        [TestMethod]
        public void DecodeEncodedWordTest()
        {
            var base64Word = string.Format(DecodeWordTemplate, DecodeCharset, Base64Marker, DecodeBase64WordTestBody);
            var quotedWord = string.Format(DecodeWordTemplate, DecodeCharset, QuotedPrintableMarker, DecodeQuotedWordTestBody);
            Assert.AreEqual(base64Word.DecodeEncodedWord(), DecodeWordTestEtalon);
            Assert.AreEqual(quotedWord.DecodeEncodedWord(), DecodeWordTestEtalon);
        }

        [TestMethod]
        public void ParseDateHeaderTest()
        {
            const string dateParsingTestHeader = "Thu, 12 Sep 2013 10:58:34 +0300";
            var dateParsingEtalonValue = new DateTime(2013, 9, 12, 10, 58, 34);
            Assert.AreEqual(dateParsingTestHeader.ParseDateString(), dateParsingEtalonValue);
        }

        [TestMethod]
        public void ReplaceAllNewLinesTest()
        {
            const string testString = "\r\nTest \rvalue.\n";
            const string defaultEtalonString = "Test value.";
            const string replacementEtalonString = "[replacement]Test [replacement]value.[replacement]";
            const string replacementString = "[replacement]";
            Assert.AreEqual(testString.ReplaceAllNewLines(), defaultEtalonString);
            Assert.AreEqual(testString.ReplaceAllNewLines(replacementString), replacementEtalonString);
        }

        [TestMethod]
        public void EscapeRegexpSpecialChars()
        {
            const string testString = "\\.+*?^$[](){}/|'#";
            const string etalonString = "\\\\\\.\\+\\*\\?\\^\\$\\[\\]\\(\\)\\{\\}\\/\\|\\'\\#";
            Assert.AreEqual(testString.EscapeRegexpSpecialChars(), etalonString);
        }

    }
}
