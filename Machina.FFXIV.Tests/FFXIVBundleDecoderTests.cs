﻿// Copyright © 2021 Ravahn - All Rights Reserved
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY. without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see<http://www.gnu.org/licenses/>.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Machina.FFXIV.Tests
{
    [TestClass]
    public class FFXIVBundleDecoderTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            TestInfrastructure.Listener.Messages.Clear();
        }


        [TestMethod]
        public void FFXIVBundle_StoreData_OneBundle_OneMessage()
        {
            byte[] data = Utility.HexStringToByteArray("5252A041FF5D46E27F2A644D7B99C475047C4E3F5F0100005C000000000001000101000000000000789C33606060D8BF405A40E487AA0033C3FE0A1106574606065586B5E1AF2381520CDDDC20F205F311CE33D3818C070009600B8E");

            FFXIVBundleDecoder sut = new FFXIVBundleDecoder();
            sut.StoreData(data);

            Assert.AreEqual(1, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);
        }

        [TestMethod]
        public void FFXIVBundle_StoreData_OneBundle_Uncompressed()
        {
            byte[] data = Utility.HexStringToByteArray("000000000000000000000000000000000000000000000000400000000000010000000000000000001800000000000000000000000800000014F82510AF57EB59");

            FFXIVBundleDecoder sut = new FFXIVBundleDecoder();
            sut.StoreData(data);

            Assert.AreEqual(1, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);
        }

        [TestMethod]
        public void FFXIVBundle_StoreData_OneBundle_TwoMessages()
        {
            byte[] data = Utility.HexStringToByteArray("5252A041FF5D46E27F2A644D7B99C475C07F4E3F5F01000073000000000002000101000000000000789C33606060F8DEA62C20F243558099617F8508832B2303832AC3BAF0D791402906D1890C0C07845E305BCA9C99EE95C4C060001403A9C5A5DEAB8381C1C6E50573AE0A443D0058D5177C");

            FFXIVBundleDecoder sut = new FFXIVBundleDecoder();
            sut.StoreData(data);

            Assert.AreEqual(2, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);
        }
        [TestMethod]
        public void FFXIVBundle_StoreData_TwoBundles()
        {
            byte[] data = Utility.HexStringToByteArray("00000000000000000000000000000000000000000000000040000000000001000001000000000000789C936040000E863C6E911FAA026BC35F47020015E504235252A041FF5D46E27F2A644D7B99C475DF7C4E3F5F01000060000000000001000101000000000000789C3360606048DBA02220F243558099617F8508832B2303832AC3DAF0D7914029860BF10C0C37F95E309FE43C333D682A030300148F0CDA");

            FFXIVBundleDecoder sut = new FFXIVBundleDecoder();
            sut.StoreData(data);

            Assert.AreEqual(2, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);
        }

        [TestMethod]
        public void FFXIVBundle_StoreData_PartialBundle()
        {
            byte[] data = Utility.HexStringToByteArray("5252A041FF5D46E27F2A644D7B99C475047C4E3F5F0100005C000000000001000101000000000000789C33606060D8BF405A40E487AA0033C3FE0A1106574606065586B5E1AF2381520CDDDC20F205F311CE33D3818C070009600B"); // 8E removed from end

            FFXIVBundleDecoder sut = new FFXIVBundleDecoder();
            sut.StoreData(data);

            Assert.AreEqual(0, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);

            Tuple<long, byte[]> result = sut.GetNextFFXIVMessage();
            Assert.IsNull(result);

            // now, add data
            sut.StoreData(new byte[] { 0x83 });
            Assert.AreEqual(1, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);
        }
        [TestMethod]
        public void FFXIVBundle_StoreData_FullAndPartialBundles()
        {
            byte[] data = Utility.HexStringToByteArray("5252A041FF5D46E27F2A644D7B99C475047C4E3F5F0100005C000000000001000101000000000000789C33606060D8BF405A40E487AA0033C3FE0A1106574606065586B5E1AF2381520CDDDC20F205F311CE33D3818C070009600B8E");
            byte[] data2 = Utility.HexStringToByteArray("5252A041FF5D46E27F2A644D7B99C475047C4E3F5F0100005C000000000001000101000000000000789C33606060D8BF405A40E487AA0033C3FE0A1106574606065586B5E1AF2381520CDDDC20F205F311CE33D3818C070009600B"); // 8E removed from end

            FFXIVBundleDecoder sut = new FFXIVBundleDecoder();
            sut.StoreData(data);
            sut.StoreData(data2);

            Assert.AreEqual(1, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);
        }

        [TestMethod]
        public void FFXIVBundle_StoreData_SplitBundle()
        {
            /// note: did not update
            byte[] data = Utility.HexStringToByteArray("5252A041FF5D46E27F2A644D7B99C47553C04E4C5F010000E100000000000D000101000000000000789C736060601010677048FC2821C0CCA0D12DC2E0C2C8C02028B06DF5BB48A01483110376F046284700443B00B12019FA0D4332E1FA8528D06F00C4F344D30410FAE550F4EF286570B0C92B0D539A1DD299585C9C4948FD7612D56FC3A2FE7B7D3A4EF51FEA191CFCF25603D59B76D34BFD8F251938D50BD583DCFF285C69F6F12E98FABFF7B370AA3F0C561F04343F0DEE5FC54BD938D51FCD06B9A70F68FE2BB8FA4DE5B938D53F6803993F294C75F61AB07A0038379D0F5252A041FF5D46E27F2A644D7B99C475ACC04E4C5F0100006303000000001F000101000000000000789CED565F485361143FF7DE55E8DAFAFCB3D494B554D2CA484B4AEAD236B51443122CAB976C84CB9516E443F4927BAC993D44DA9F5124582F42A884FD2507110AF950E643E283F3C58D8AD28ACAFED139BB53F76DAE44A22C3C70EEEFBBFB7EE79E73CFF976CEB50340641C982CA3F14C82F4BA58300B0051ACA3E5F51EDC0287A8EC4BB8B66C13E064B257B27F201440849BA048AB99EC610692831ACDF9CFE5FC6BA7B07952D376E6577622AC181283ECD2FDCADB15898176E3620FE1F179691095FD401B5729605E82BD825B08BA9F4E5EC8FFABE82A16CE7FA3A8EC830120AB0EA0937925C282440116C0883F08839938FFA27F132AF99EF49FC7F9A7C356ECE7F6E8A6B6FFBA38B07E299C7D8A24A002C4DEE83212AFD02A9389BBAA6CBDF339BE4726DE5CEEB305F8D773F6E59D2A903DF1BBD39CEDA7367A147E6F5378FEB55A407E4E59B2B37A82FFF26D4558FEA88FAFD995EC3CE118E71775A802F85BF878F602F46BBD124B03889B21BFB5C71A96FFA8846ABB79C6FC7421B89ECB39BE6E58394F630601B276D898FE21C0A7E64AE62A04B8A2B3B2EF26A51E99190043DD56F600FFC8712FCA992B5589A37F19C0D8D34AB60E5186FDAC2511A0FD5835CB45AC8CD9C7744B00BE0D1E640309009A0D2A161183E7B6F810F368953CDDD128F56B542B711E9FA79C9F8518E5C7DA03AC0963BCBBD4E6C3D3186B2CBED32D4423E26D4413E23DC43CC4FB88F9885D88DB11BB114B101F239622F620F6A2F6A1AEC4FB0CD4B5A8D953E6298DCBD37CB45163DF11304FD49701F3A421C43C4511629EB48498A74584FF699EA8FF53BCE1FA3F8609181A5C90F8BE702EC4EE0B04DAA9FD3DDD82FA3E12A0193BD9A58B0946EA0D831EFC55005F3B93C4A3E0520124F9B8F6E9B4344ED401B383163FEB7563C21ADF2C1B890C7D4E6A767DC3552178567DE6DE0964D907349F6842053DC21D663D27BF411C7876A93EFC2CE5EB6390B7CA343B95395A90E8C5335B2D50B147CC403349D04FACCFE2A5E0DDB37CEA811EFF94A567BF112304E2F866F2A4B81BF152B169C04C7C9ACAF5E7F5219CBFB49E1532579FD95F1FFAD6A05914EE5B69A8164CF2E1EB3B573993EA2C3535B63FC1E76708CF1F36127F357EEB1D71CC46FE0F8E203E0D5252A041FF5D46E27F2A644D7B99C475AEC04E4C5F010000E1000000000016000101000000000000789C33606060C8104E1048FC2821C0CCA0D12DC220C7C8C02028B06DF5BB48A014C3537B06079B3CDD30A5D9F95D89C5C5990664A89F279A8653FDD65290FA52A0FA904E62D46FC6A2FE7B7D3A4EF51FEA191CFCF25603D59B760F56F53F9664E0542F540FF2EFA370A5D9C7BB06837AC10150FFF3226EF58FF783D4EF09D59F7D9428F58FA8A0FEEFFD2C9CEA0F83DD1F048CDFB44E62D41FC2A27E53792E4EF5F7DB40EA2785A9CE5E4394FABB24AABF83A61E007B222F195252A041FF5D46E27F2A644D7B99C475D5C04E4C5F010000CF02000000000F000101000000000000789CDD555F48535118FFDD6DC6D2D4E3CCCC0AFB43D48A552E1FEAA1F23A5F42B08C84E8A53F04E6CAA017898270F49268126296160946516054060D263D0C4752B1A2C417E9A1ED454744D44B2594EB777636DDD56938EAA1BEF1DDDF77CFB9BFFB7DE777CF77");
            byte[] data2 = Utility.HexStringToByteArray("E601905708FDE897A5C20C7BCB62946B1C11DEDE4F0739852B26354FD006EC40D39A88D9BF5F2202404DC5C543ED2EE05E2C062E31BE158FEF27C5727CCA169C9E0C750F2FB973E4EF32A979869AAFF4EFE49F6BFD6DF1F573503BB7F6CFE7DFCE4B9621BFCB903F07A9ED773C1336844DD338F6B81B7995A6645EC23C339E33EAD26452F3725F4C72BE6AD4C5125003ED893587541D520B756F8E81D445DD5B8C6586A42E76BE44E69ECABFCE90BF6014F868AB17E3AB34386BDCA2986FFF7EBB4EF87703DD05B5624207C2CF6BC5812DC0CD61B7287100851F0E0B3FBFE1A360AD18590D8CBFAD13A5C41D38267A97038FCF9C122E625DFE1151B00CF8F9FE84785704646FB38885F980B3EAA418E307A9F45A842F1B18EA718BAB59AACEB319C08F25D017B1CA6F8DC7450F6BEC5FE98E61336BB58D014F883B895EE22EA28F584EEC27BA88CF8855C441E21EE20BE23EE24B62903E441FA66FE49883BE953E53A7F5069D32F87CA6EC1FEA942D31A0F633A8538E443DDE5FFFB94E5CE6349D2A0C3AE5416DE43E7A64967E33F28DFDC66542EEEB159B8C3C797E1A791348E6C971C4F376D3EFF0F9EB5D4565B13EF9DC278B8AB58F190D7848ED72CFFBF054E689713C292A4D6D66AD01E3D6F4F9D2AA1FB4B45A9B1DE1BDD56D56BDECC2EBC4FD487D86B558531A4E44A3D137FD776FC8D846EFEC748483D72E5B4B9CAD1DF2F9CDEA3008CD2B7192F99BDCAFD2E5FECB96A9FE03550FA761D146F953B1DCCBB25767DBCB96A4333D6191D1C10EC993BD9D0E4F9E05F3E5FD02BDC874ED5252A041FF5D46E27F2A644D7B99C475D9C04E4C5F010000A5020000000012000101000000000000789CED944B6813511486CFCDC4125A8D538D545CB412506B75611131E02899FA46EDC6127555BB300F6C8B3416970908A5A654A43E36C15A6B7153C1A8A50B5742B03E1A17B5DDA89BE8222D8A355DF8DEF89F99A4C98D16340855E8819FEFE6CE794CCE3D735D44E45F7E5C6D9A59A92A54DDE5A03A4154AE0E0F4E1FC323B212092AB0A9D4C865177875225054DCDD332D45C57D0EF98A8AFB3AE6FFE3B83EEC9655903B17F78DF2E348D30C843F7178A7B32045F22FAD17EC17763A45B409EC5DE1CD3BD74AE95CEFB4935B6B6DF738A30DE79A82C100FBCBF323FBA743E4AE6F1D84FF96AEACFF9701FF9CFEE521CEFFE68833FA2892F5AF3D74724EFF19C37F15F20FCFE697BF03D9FF5507FBF778D6446F19EF1FC69E5D9A475DF28F58CCE73CCFF67A9EC829E5C161A6880B1A82C7C53AA298CEF116A38BFDF14C3B938A81869DD9DF56B9DD49C81DFEA9DF72FD6E8BF9BD9083C47B0FD10B7BBF6252C4ADD44C2546FDD53AE7F81FEB57A31AD7CED55F27D55F84992CE5FA11A225CC8378CA14E6B930973261F7AB02EA75B0E26DA37A148C25BC2AEF3F448EDD934423E01EF009380A8D41E3D07AEC6D806AA1CDD07EE814F63BA04EE83C74014AA5F80E239A86D25009FC6C5010EBB249F3FB590C5E03B7827DA00BBC016E0307400DBC0DEE0563E03EF01E78001C029F4209E8393401D5607F237429F37F727DFA4EF97D52333DB0659A7B1387F12C91D8CE7D6EACB19110E67DAC501BF9142ADAB275D8F82E976747BECB1DDA0E8D67C59C179E1CA216518A27699DDFAB5954CEAE7D999C9CAF07ECFE38BEEBF563AFCA13D6FBA1CA98AF13D85F76D69CB52B58BF5BFB52679F8299FA97D6F3666DA985F3F98DF5BC199FCF0F3AFB8B455252A041FF5D46E27F2A644D7B99C475DBC04E4C5F01000063000000000002000101000000000000789C33606060F8B1244320F1A38400338346B708831C230383A0C0B6D5EF2281520C82F50C0E36798FC295661FEF4A2C2ECE3420513D00EE681BC35252A041FF5D46E27F2A644D7B99C4750DC14E4C5F0100006F010000000007000101000000000000789CB3606060E012677048FC2821C0CCA0D12DC2E0C4C8C02028B06DF5BB48A014833883F9C1B74C0C0CCC4036371303182CB8F6C3D102486F12CA17C0A58F9741F72148");

            FFXIVBundleDecoder sut = new FFXIVBundleDecoder();
            sut.StoreData(data);
            sut.StoreData(data2);

            Assert.AreEqual(101, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);
        }
        [TestMethod]
        public void FFXIVBundle_StoreData_BadMagicPlusOneBundle()
        {
            // changed '52' to '51' in first byte
            byte[] data = Utility.HexStringToByteArray("5152A041FF5D46E27F2A644D7B99C475047C4E3F5F0100005C000000000001000101000000000000789C33606060D8BF405A40E487AA0033C3FE0A1106574606065586B5E1AF2381520CDDDC20F205F311CE33D3818C070009600B8E");
            byte[] data2 = Utility.HexStringToByteArray("5252A041FF5D46E27F2A644D7B99C475047C4E3F5F0100005C000000000001000101000000000000789C33606060D8BF405A40E487AA0033C3FE0A1106574606065586B5E1AF2381520CDDDC20F205F311CE33D3818C070009600B8E");

            FFXIVBundleDecoder sut = new FFXIVBundleDecoder();
            sut.StoreData(data);
            sut.StoreData(data2);

            Assert.AreEqual(1, sut.Messages.Count);
            Assert.AreEqual(0, TestInfrastructure.Listener.Messages.Count);
        }

        [TestMethod]
        public void FFXIVBundle_BundleInMultipleIPPaylods()
        {
            // note: did not update
            byte[][] data = new byte[][]
            {
                Utility.HexStringToByteArray("450005DC73A740002B06FA797C969D34C0A80188D6D1F155348CDFF054FC6EE750101C9A79C400005252A041FF5D46E27F2A644D7B99C475064D503F5F0100003A0700000000AC000101000000000000789CB59B5B6C15551486F79CB69C43EF8542A13DA505B908D5846042A2A8208A4501411F0C101E90F8A0313E18299544946B0C25E2255154124CD42848890F88C6070DD1A4984ABC208998F060BCC420B65EE2030FA2CEEC7FCAB2B3D7393D73E69FD3341372321F6BD65E7BEDFFDF7BBAD118D37C695663F05B6116F6359B959E31193373EDC575FE57A6659C315F656B4C7F5DABF1BF30AB979B511FEFEDF93DFE25670A7C3696CC6FB3FCD7974500347EDEF2F3735DFE503B83DF6EF97DE3D38A7F9AE577B5A6C5EFB0FC9377BAFCCD334D6EA03329BFD3F29F5A8CFB0E6DCD668C59428C7FBAE5EF50F81B3A18FC1996BF7C959B9F62F1CF77F8AB1CFE0CFFDA5F575A3C53C278B6D71A1BCFD1E8FFECC7D338A7FCE715BE67F91F28FC62CF5B3A3F53307E0EBFC2F27F27F72BE15716CC0F27FF55967FF37569E5671CFA6177BCFC8C55CF013FEB5F83FA2C259EA9613C3BC27ABEA0F49FC639D9DCBC35E53DAFF051CF07BAB8F9143EEAF932B9DE848F7A7E54E127A937E1A39E9D3CDBF8B3043EEAF9F11B5CFEC93646FCA8E79A952E7FA47E1AFC1F7CAAAAE2F3B396FF83529F0F7644F99595F1F939CB7FA8DBE51B4AFD8CB7FCBDCAFAB8A28331BF6A2CBF77BA1B3FA73E6B2DBFFB6EDCB774CD258FA11F845F67F92FAAF133F2536FF90D778CBE3F98BFB3F28CF81B2CFFD86237FE93550C7EA3E56F51F4F973D526E7CCEBD8FC26CB1FBADDE51BCAF84EB0FCA9F7687C467F9B68F96FA9FA96513FCDF05F53D2CACF24CB9F1B530F94CE9F6CF9C76FC57D7FBFE9D9FC8CAC5FC9EBA7C5F237ABEB1723FE29967F5F6AEBFB54CB5FA4E49FD33FE1DFCF57BBF919E1AFF4022D177C325E7C3EFCFB2364FF2E7CF8F7BDA9AD8FF0D76B6BD2E2C35F7FADE4BF187F2CBD1DF0A7F9D71D25EAEDD6309E9DA1DEEE8BF4AB8C1FCF265F0F6C2B733E0A3FD4DB05D6D3E47CE8EDBE79A3EFF7687CE8EDC34A7EFC7FE6D6459F2B361F7A7B41AD9B1F93A0DE840FBD7D4EED278CFC406F0FB6B8F1E70C830FBDFD63446F67C27E15E43F6B9D66F0295D6F0B1F7AFB6097CBFFB99A31BED0DB6B6F73F36328E35B0DBF465EAF850F3DFFCD4237FE24EB91F0A1E78F2AFDE1CC0C46FD40CFF72AEBD1FA4EC6F842CF372A7A9E33BFA0E7DF55F512830F3D7F48D90F99D8CCE043CFF790F598F0A1E787C97E41F8D0F303A9CD2FE8F93EB29E113EF4FC8DA9F57FE8F94D643D2C7CE8F97F52E343CF1F27EB3DE143CF7F1C893FE01FAB60E41F7A3ECAC984E39B3C7EE8EDC1303F51BF969C8FF3B2098AFEE999C6881F7AFE606AFA0A7AFE17559F30C617E765ADE4FD0AE1E3BC6C68755AF9C179D92B31F7DBC7F23BADE179D9CE12FD4E5B18CFAED0EF3C4CDEFF173EFCCE6705F4F01365D683F0E1778E14D85F4ACE87DFB958E0FC25391F7EE78B152E9F133FFC4E7F01BF999C0FBF7342D163FDB3197CF89DCB755AFCF309F509BF73A6CE8D3F493F173EFCCEBFE4F55AF8F03BFB94FA0FF62B92E71F7EA78DDC6F850FBFB38EEC07850FBF33BDDEE58F8CEF6AEF2E2FF26D0C3EFC8E93E7847A43F8F03B8F297E8D337FE1779E89F8055E7DC2EFBC901A1F7EA78BEC47840FBFF39BE2A7F66419F987DF39955AFCF03B5BC87E53F8F03B3791F7E7850FBFB34FF1839F4F62E41F7EA7477D7F80C187DF6952F6433EAD65E4077EE77AA5BF2D08FDC27AA7BBC5E1E3FCE21C79BF4BF8F053C364BD2D7CF8A94F14BFC9E1C34FBDA6E8E7A67646FDC04F3DA08C2F277EF8A997147DF84735237EF8A989EAFACB581FE1A77E2AA2CFB5FDF0B1FC54C0BFCABFEE2AD14FE5C37876877EEA3CB91F0A1F7E2A7F8DFBBC26413D081F7E6A40992FBD2D0C3EFCD4770D11C0FFF2B3DF7BD67BDAF62CAF223E1F7EAA9FFC3EADF0E1A73645FA79D27E287CF8A9F794FC6C4BB05E081F7EEAFD22F9DFEEEDF476D9FC575654C4E6C34F7D5F849F2C7EF8A9ED219FE5D7840F3FB541799F2447A9"),
                Utility.HexStringToByteArray("450005DC73A840002B06FA787C969D34C0A80188D6D1F155348CE5A454FC6EE750101C9A5EEA00001FF8A977543FCEE0C34F1D50CF7F197CF8A999B7E03E7EFEE1A7DE50FCC247950C3EFCD420596F0B1F7EEA4FC52F70F2033F7546D92F3A1DE647A6DE95D73963F0E1A78E28F397931FF8A9130A7F6996C1879F9AACBCEF3A308EC1879F6A54DFA78DE6FF8ADC88C1879FDAAFAC5FDD09FE5E43F8F053CF2BEB97A18C2FFCD459F2F9A3F0E1A78615BDF76582F7A5850F3F7530667F1E4B4F06FC36FFBABB443DD91EC6B327D4935B0BECFF7C786D79CF2B7CE8C9FBC9FB93C2879E3C4CAE37E1434FFEAAEA7F937BB24CFF227CE8498793B09F0B1F7AD228FB039CFC404F9E56CE5F9618467EA027E729FBF386C2879EFC96BCFF207CE8C9AB15BFC3A91FE8C909CAF85E98CD881F7AF2AFB0AFB0F498F0A127372AF377A886C1879E7CB940BF4D9E7FE849A74F263CBF103EF4649B7A3EC5C80FF4E47AF2F988F0A1279785F12F3A3BFA7DAAE47CE8C97B8BE4A7C3CB7BED5EB97CE8C9538ADFE1E4077AB2BEC0DF4F255F7FA127F3E4F335E1434F1E52F2335CCF985FD093AF2AFCDEE6C2FCFF00C7F4BE7D5252A041FF5D46E27F2A644D7B99C4755F4D503F5F0100004B0700000000B2000101000000000000789CB59B7F68565518C7CFFB6EEF8FE974EFDAE6D4FDF475A65BBA068699A42E96B57473B6228C42E71F0B22218C4A5D5056E83434E99F8CFC31EA8F14050982B470FE110CFB0945BA408B1A882D420882F28F6ADD7BBE777BF6BEE7B9EFEEBBF3DC2BE38EBDF77CF6ECB9E79CEFF33DE7D8A3942ABFB528E57E15A8156F94AB8E885251D5B0F9F7C79D8F544D5CA9EF1233D5DEE2B9CAF940DD58AF32AFD32D2FA416279283CB942A71FEE12A2C1CFFB827307F9EE62F7D30131F71F8CE2DA97CAEE0FCF99AFF711BDAFDF34124AA54ABE6A716ABE42BAB6CF9559ABF26B4F8AB357FE7A62C8018BF46F33F0F2DFE5ACD3F98C577E3EF9D25C1AFD3FC3F1E082B3FF59A7F95C9CFB12A89FEB340F34FAF45BB13BB13BA7F227E8C2F3B7E1AF967F8BD75FEF1B718FC4E83BFD0B9EF2D0E164FAD17CF3EE7799DCF1559BFD01B8F7C34F9F0239AFF4DA5C94F3AF9DC33CDF745FCA8E61F79C8E42B8BFE46FC02CDBFE933DFDAC75FA8F98D255900873F505224107F0CF3557B667B77BC1C9D21117F5CF3CF2E082B3F09CDFF64B6C93F5321F17E939A7F861DEF12FC22CD8F64F54FDBF99CF83334BFA4C3CCCF78FEA91E88C5F2E7CFD4FCA176931F1579BFC59A3FE8BDDFEC7AC03E3FB334DF88D3CB8F3D7FB6E66F0B6DFE29D1FC32A6FFC8E427A5F97DEBCCF807EB24DE6FA9E63F717F58F9B94DF3C798F9617542825FA6F929667E96E93FE59A7FEE0E93DF582191FF0ACDFF35877E4DC72F107F8EE6372DF5E7DBC55F89FC307E61A440820FBFB34CB8DE263EFCCE2E61BF407CF89D2F428B1F7E27DDCDC52FD1FFE1777A99FA44267EF89D6B4C7E6E964AF41FCFEF7471F991881F7EE7D556932F33BEE077DE65EAB7E7EB25F8F03B8798FAAA2F87BE4CE5775C7EDAB9EF0BE877EABC78FA3DBF7328E5B5FB74DE443C367A4A7CF89DF329339FCAA23F101F7E6789A71752F512F1E1779AB2F4C8763C121F7EE7A9D0F203BFD3D762F265F203BFF366687CF89DE6FBD06EF2FBAD2E94E0C3EFB40BEB05F1E1776E30FCEE45127CF89D83AD5900873F5C25C187DFF993AD9724F8F03BEF30EB6F4A24FF9EDF11D653E2C3EFDCC5F8C1FE6A093EFCCE6B4CFC32F987DFD9CEF835193EFCCEC935267F635C820FBFB385997F2ECF90E0C3EF9C5B6BF22FC624F8F03B5F8636FFC3EF6CEC34F91B8A24F8F03BC7D9F52E093EFCCEA5D0F203BFD3CDF41F193EFCCE8F1B4CFE6814FC82891F4E2C47E5C187DFF988E99FF7D449C48F7AF5B93CF33355BDEAF2AB9D7B7FC07AB5DE8B67BF57AF3EC2F467B7BF3D5933399F13F63D0F3EEAD56AE17A92F8A8574F088F17E2A35EBDC58C9771FEE5C8F791E188CE4F3C96371FF5EAF25293AF2CC60BF151AF8E30E3E59978F6FB0D3E5E888F7A7517D39F3FAC94881FF5EA41E17A98F8A857AF30F5860C1FF5EA0EA61ED896"),
                Utility.HexStringToByteArray("450005DC73A940002B06FA777C969D34C0A80188D6D1F155348CEB5854FC6EE750101C9A1172000096E0A35EFD2147FFB4E3A35EBD255CAF121FF5EA72C68F8CCF3F767CD4AB1798F97CD8A2DE203EEAD59F42CB3FEAC9F34CFCDB2DF488F8A8F78E0BFB05E2A3DEFB9BE93FF78AF0B1BE64F4134B3D75F965CE7D7F403D5DE0C573C0D3D3CD3E7AEA7E4B4BFDC1E75BE2434FFF63F70BA69F4FE2434F1B99FE7CAA5C820F3D6DC9A9A75053A512F1FCF9D0D3B470BD417CE8E96E613D223EF4F47D663CCAF0A1A72B99F9B6D9623E243EF4F45F61BD203EF4F476E1FD6EE2434FAF32F5C0BA5A093EF4F445E1F501E2434F0798FEB32A22C1879EEE10F677C4879EBEDD6AF28F8ACC3F58FF1964F474A5C5FA12F1A1D771E17A80F858FFD9297C1E80F8A8078E09FB29E2A31E784FD8FFBAFC72E77E20A05EA7BD78AE7B7ABD9589E7AF39D3FF7B890FBD8E30FECBCD6757832D1F7A1DF5E1DBC70FBD6E16D653E243AFF730FE57860FBD5E1F1A1F7A7A44783E243EF4F42BE1F53AE2434F0F0BD733C4879EDEC9ECF7C9F0A1A7AB997A669FC57E16F18B7DF94B2CEA25E2434FE730E7977EB3586F273EF47440783E273EF4B489391F589994E0434F4F0AD7C3C4879E1E16DECF223EF4B48DA9272FCE97E0434FDF12DECF223EF6537E165E1F233EF6535E62EA99D106093EF6533A429BFFB19FF27068FCB921EB2FF66B5E0F4DBFB05FF319536F6FB558BF223EF66B8618BE4C7E703EEDEBD0F417E7D38C3AD0E13788F47F9C4F6B64CF6FA3FEA4A3ADC1D7A3888FF369D784FF7F07F1713EED31E6BCC7498BF3F9C4C7FAE1D1D6B0E2C77EDFA53CFDDA547E27ED9D4FBB1ED0EF2CF4E2699F05BF3324BCDF447CF89DFE74587CF89DFDA1F1E1473A99F3EAB9F853BD2F97EFEE9EBAF9E79FEFCE787ECCB9DC7DB9516F73AED578BE2DE3F997236363EEDD4DCBB70933BEA9DA3FEDB4DF12F1FBEBA66EFFA81371FA50EDA41924BFF6ABA3887FBABFFFD91CB19F3A7D6124487B3F44D0F651CBF60596ED0B2DDBFBBDBBA0EDE396ED996E3BD17E93D1BE2BA3FDDD67316EDDCFAF24DCF19381F8654C5FF48320BC32EF19770C4E6ECBFDCC1CCF1B0D9EFB15F3DE728FF1FC1663FCFBA423E735DE2E103FC79811E1FB0D0829BEDF8091E2FB0DA800FCFF01BC010D075252A041FF5D46E27F2A644D7B99C475044E503F5F01000045000000000001000101000000000000789CB360606010F9A12A00C2CC2036C3040664C0C8801D0000A01403635252A041FF5D46E27F2A644D7B99C475614D503F5F010000EF07000000002C000101000000000000789CED596B6C5455109EBBDB4ABB6DA531D51083718502152DD92EB865EB8395D55A04CA026B527CD4DED8DA56E8435AB544D4AA25362D286015300A8D3C24A8B1F199283FD02062D4A61A7FF8C0DD457C1401AD8FC4C68075BE7BCEDDBDBBB7DBDD82C44718727A66E67C3367CEDC33E79EBD541051CEE0A46C342BB93A72A85C21B2506ED991253C44434C946A656EB84626B960DFB889ED0516FA13764C1549F8B79CC58A2449070E8EC6FF1870292378B58639BB65F4FE292DD9E82334349AF8D3752B2569FFC74713BFEDF4C64F19E8936BAB7903DDB65AA121398FC7E4DF1BE5BF829F972B6E8C89ED7770AB1D619D0E937D51943DB68B5D3CFF10FEE499F0EF9111AF512B51B7251697A29870FF637A66FE2ECF3F1DC37F9BD46B4EA7770F8D5C37094989B59F19659FCAD2246E1F9C4554C9FD6E3EE38EF1117D7D3BD115BCB27E3E92B3A708578E4CA2658AB0F99ECFF2172C42FE388BE865D853A4C10FDA1425D2F2E45C17737BDA2A6C9F4811F893A1BD956F1D946CB7BE8E098A58C743DC7295C4F94BB77CF7E38238C7F9D28E8684F66E7AF3E0F0D6440B2835A1FD89C863D268ED58457B43EA6FC944F638ADF05AC5F9974A82375222FBED23A73894C87E8D7564FB9D26FBFE28FB73B9DDA05C4063630C8724C53A64BC027CECB4B1F878F6ADF4DED8072D667B9F29CEA762DE2F6D612C729D4253F9D94FA122838FF52B2F989DBF61D32C6759C66C8FC95F74DE1EB3BA46BC4424B25FCBF67BBEB868D1C9DAAF1FE1"),
            };

            IPDecoder sut1 = new IPDecoder(
                Utility.IPStringToUint("124.150.157.52"),
                Utility.IPStringToUint("192.168.1.136"),
                IPProtocol.TCP);
            TCPDecoder sut2 = new TCPDecoder(54993, 61781); // port may be 61781!

            FFXIVBundleDecoder sut3 = new FFXIVBundleDecoder();

            int messageCount = 0;
            for (int i = 0; i < data.Length; i++)
            {
                byte[] ippayload;
                sut1.FilterAndStoreData(data[i], data[i].Length);
                while ((ippayload = sut1.GetNextIPPayload()) != null)
                {
                    byte[] tcppayload;
                    sut2.FilterAndStoreData(ippayload);
                    while ((tcppayload = sut2.GetNextTCPDatagram()) != null)
                    {
                        sut3.StoreData(tcppayload);
                        while (sut3.GetNextFFXIVMessage() != null)
                        {
                            messageCount++;
                        }
                    }
                }
            }

            Assert.AreEqual(351, messageCount);
        }
    }

}
