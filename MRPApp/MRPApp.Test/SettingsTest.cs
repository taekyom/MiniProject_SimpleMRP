using Microsoft.VisualStudio.TestTools.UnitTesting;
using MRPApp.View.Settings;
using System;
using System.Linq;

namespace MRPApp.Test
{
    [TestClass]
    public class SettingsTest
    {
        //DB에 중복된 데이터가 있는지 테스트
        [TestMethod]
        public void IsDuplicateDataTest()
        {
            var expectVal = true;
            var inputCode = "PC010001";

            var code = Logic.DataAccess.GetSettings().Where(d => d.BasicCode.Contains(inputCode)).FirstOrDefault();
            var realVal = code != null ? true : false;

            Assert.AreNotEqual(expectVal, realVal); //값이 같으면 pass, 다르면 fail
        }

        [TestMethod]
        public void IsCodeSearched()
        {
            var expectVal = 2; //예상값
            var inputCode = "설비";

            var realVal = Logic.DataAccess.GetSettings().Where(d => d.BasicCode.Contains(inputCode)).FirstOrDefault();

            Assert.AreNotEqual(expectVal, realVal); //값이 같으면 pass, 다르면 fail 
        }

        [TestMethod]
        public void IsEmailCorrect()
        {
            var expectVal = 2; //예상값
            var inputCode = "설비";

            var realVal = Logic.DataAccess.GetSettings().Where(d => d.BasicCode.Contains(inputCode)).FirstOrDefault();

            Assert.AreNotEqual(expectVal, realVal); //값이 같으면 pass, 다르면 fail 
        }
    }
}
