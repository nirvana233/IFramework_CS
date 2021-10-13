using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

//############################################################################## 
//RSA ��ʽ���� 
//˵��KEY������XML����ʽ,���ص����ַ��� 
//����һ����Ҫ˵�������ü��ܷ�ʽ�� ���� ���Ƶģ��� 
//############################################################################## 
namespace IFramework.Utility.Encryption
{
#pragma warning disable CS1591 // ȱ�ٶԹ����ɼ����ͻ��Ա�� XML ע��
    public class RsaEncryption
    {

        /// <summary>
        /// RSA ����Կ���� ����˽Կ �͹�Կ 
        /// </summary>
        public void RSAKey(out string xmlKeys, out string xmlPublicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            xmlKeys = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        }


        public string Encrypt(string xmlPublicKey, string m_strEncryptString)
        {
            byte[] PlainTextBArray;
            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            PlainTextBArray = (new UnicodeEncoding()).GetBytes(m_strEncryptString);
            CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;

        }
        public string Encrypt(string xmlPublicKey, byte[] EncryptString)
        {
            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            CypherTextBArray = rsa.Encrypt(EncryptString, false);
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;
        }

        public string Decrypt(string xmlPrivateKey, string m_strDecryptString)
        {
            byte[] PlainTextBArray;
            byte[] DypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            PlainTextBArray = Convert.FromBase64String(m_strDecryptString);
            DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
            Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
            return Result;

        }
        public string Decrypt(string xmlPrivateKey, byte[] DecryptString)
        {
            byte[] DypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            DypherTextBArray = rsa.Decrypt(DecryptString, false);
            Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
            return Result;

        }

        //��ȡHash������ 
        public void GetHash(string m_strSource, ref byte[] HashData)
        {
            //���ַ�����ȡ��Hash���� 
            byte[] Buffer;
            HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
            Buffer = Encoding.GetEncoding("GB2312").GetBytes(m_strSource);
            HashData = MD5.ComputeHash(Buffer);
        }
        public void GetHash(string m_strSource, ref string strHashData)
        {
            //���ַ�����ȡ��Hash���� 
            byte[] Buffer;
            byte[] HashData;
            HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
            Buffer = Encoding.GetEncoding("GB2312").GetBytes(m_strSource);
            HashData = MD5.ComputeHash(Buffer);
            strHashData = Convert.ToBase64String(HashData);
        }
        public void GetHash(FileStream objFile, ref byte[] HashData)
        {
            //���ļ���ȡ��Hash���� 
            HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
            HashData = MD5.ComputeHash(objFile);
            objFile.Close();
        }
        public void GetHash(FileStream objFile, ref string strHashData)
        {
            //���ļ���ȡ��Hash���� 
            byte[] HashData;
            HashAlgorithm MD5 = HashAlgorithm.Create("MD5");
            HashData = MD5.ComputeHash(objFile);
            objFile.Close();
            strHashData = Convert.ToBase64String(HashData);
        }
        //RSAǩ�� 
        public void SignatureFormatter(string p_strKeyPrivate, byte[] HashbyteSignature, ref byte[] EncryptedSignatureData)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPrivate);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //����ǩ�����㷨ΪMD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //ִ��ǩ�� 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
        }
        public void SignatureFormatter(string p_strKeyPrivate, byte[] HashbyteSignature, ref string m_strEncryptedSignatureData)
        {
            byte[] EncryptedSignatureData;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPrivate);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //����ǩ�����㷨ΪMD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //ִ��ǩ�� 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
            m_strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);
        }
        public void SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref byte[] EncryptedSignatureData)
        {
            byte[] HashbyteSignature;
            HashbyteSignature = Convert.FromBase64String(m_strHashbyteSignature);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPrivate);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //����ǩ�����㷨ΪMD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //ִ��ǩ�� 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
        }
        public void SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref string m_strEncryptedSignatureData)
        {

            byte[] HashbyteSignature;
            byte[] EncryptedSignatureData;
            HashbyteSignature = Convert.FromBase64String(m_strHashbyteSignature);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPrivate);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //����ǩ�����㷨ΪMD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //ִ��ǩ�� 
            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);
            m_strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);
        }
        //RSA ǩ����֤ 
        public bool SignatureDeformatter(string p_strKeyPublic, byte[] HashbyteDeformatter, byte[] DeformatterData)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPublic);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
            RSADeformatter.SetHashAlgorithm("MD5");
            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                return true;
            return false;
        }
        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, byte[] DeformatterData)
        {
            byte[] HashbyteDeformatter;
            HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPublic);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
            RSADeformatter.SetHashAlgorithm("MD5");
            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                return true;
            return false;
        }
        public bool SignatureDeformatter(string p_strKeyPublic, byte[] HashbyteDeformatter, string p_strDeformatterData)
        {
            byte[] DeformatterData;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPublic);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
            RSADeformatter.SetHashAlgorithm("MD5");
            DeformatterData = Convert.FromBase64String(p_strDeformatterData);
            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                return true;
            return false;
        }
        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)
        {
            byte[] DeformatterData;
            byte[] HashbyteDeformatter;
            HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(p_strKeyPublic);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
            RSADeformatter.SetHashAlgorithm("MD5");
            DeformatterData = Convert.FromBase64String(p_strDeformatterData);
            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
                return true;
            return false;
        }
    }
#pragma warning restore CS1591 // ȱ�ٶԹ����ɼ����ͻ��Ա�� XML ע��

}