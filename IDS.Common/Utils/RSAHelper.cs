using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace IDS.Common
{
    public class RsaHelper
    {

        #region key
        /// <summary>
        /// 将秘钥保存到Pem文件
        /// </summary>
        /// <param name="isPrivateKey"></param>
        /// <param name="buffer"></param>
        /// <param name="pemFileName"></param>
        /// <returns></returns>
        public static void WriteToPem(byte[] buffer, bool isPrivateKey, string pemFileName)
        {
            PemObject pemObject = new PemObject(isPrivateKey ? "RSA PRIVATE KEY" : "RSA PUBLIC KEY", buffer);
            if (File.Exists(pemFileName))
            {
                File.Delete(pemFileName);
            }
            using (var fileStream = new FileStream(pemFileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fileStream))
                {
                    var writer = new PemWriter(sw);
                    writer.WriteObject(pemObject);
                    sw.Flush();
                }
            }
        }
        /// <summary>
        /// 从Pem文件中读取秘钥
        /// </summary>
        /// <param name="pemFileName"></param>
        /// <returns></returns>
        public static byte[] ReadFromPem(string pemFileName)
        {
            using (var fileStream = new FileStream(pemFileName, FileMode.Open, FileAccess.Read))
            {
                using (var sw = new StreamReader(fileStream))
                {
                    var writer = new PemReader(sw);
                    return writer.ReadPemObject().Content;
                }
            }
        }

        /// <summary>
        /// 从xml中读取秘钥
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="isPrivateKey"></param>
        /// <param name="usePkcs8"></param>
        /// <returns></returns>
        public static byte[] ReadFromXml(string xml, bool isPrivateKey, bool usePkcs8)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xml);
                if (isPrivateKey)
                {
                    return usePkcs8 ? rsa.ExportPkcs8PrivateKey() : rsa.ExportRSAPrivateKey();
                }
                return usePkcs8 ? rsa.ExportSubjectPublicKeyInfo() : rsa.ExportRSAPublicKey();
            }
        }
        /// <summary>
        /// 将秘钥保存到xml中
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isPrivateKey"></param>
        /// <param name="usePkcs8"></param>
        /// <returns></returns>
        public static string WriteToXml(byte[] buffer, bool isPrivateKey, bool usePkcs8)
        {
            using (var rsa = CreateRSACryptoServiceProvider(buffer, isPrivateKey, usePkcs8))
            {
                return rsa.ToXmlString(isPrivateKey);
            }
        }

        /// <summary>
        /// 获取RSA非对称加密的Key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        public static void GenerateRsaKey(bool usePKCS8, out byte[] publicKey, out byte[] privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.KeySize = 1024;//1024位
                if (usePKCS8)
                {
                    //使用pkcs8填充方式导出
                    publicKey = rsa.ExportSubjectPublicKeyInfo();//公钥
                    privateKey = rsa.ExportPkcs8PrivateKey();//私钥
                }
                else
                {
                    //使用pkcs1填充方式导出
                    publicKey = rsa.ExportRSAPublicKey();//公钥
                    privateKey = rsa.ExportRSAPrivateKey();//私钥
                }
            }
        }

        public static void GenerateRsaKey(bool usePKCS8, out string publicKey, out string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                 byte[] pubkey = null;
                 byte[] prikey = null;
                 GenerateRsaKey(usePKCS8, out pubkey, out prikey);

                publicKey =Convert.ToBase64String(pubkey);
                privateKey = Convert.ToBase64String(prikey);

            }
        }
        /// <summary>
        /// 从Pfx文件获取RSA非对称加密的Key
        /// </summary>
        /// <param name="pfxFileName"></param>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        public static void ReadFromPfx(string pfxFileName, string password, out byte[] publicKey, out byte[] privateKey)
        {
            X509Certificate2 x509Certificate2 = new X509Certificate2(pfxFileName, password, X509KeyStorageFlags.Exportable);
            publicKey = x509Certificate2.GetRSAPublicKey().ExportRSAPublicKey();
            privateKey = x509Certificate2.GetRSAPrivateKey().ExportRSAPrivateKey();
        }
        /// <summary>
        /// 从Crt文件中读取公钥
        /// </summary>
        /// <param name="crtFileName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] ReadPublicKeyFromCrt(string crtFileName, string password)
        {
            X509Certificate2 x509Certificate2 = new X509Certificate2(crtFileName, password, X509KeyStorageFlags.Exportable);
            var publicKey = x509Certificate2.GetRSAPublicKey().ExportRSAPublicKey();
            return publicKey;
        }
        #endregion

        #region Pkcs1 and Pkcs8

        /// <summary>
        /// Pkcs1转Pkcs8
        /// </summary>
        /// <param name="isPrivateKey"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] Pkcs1ToPkcs8(bool isPrivateKey, byte[] buffer)
        {
           // System.Security.Cryptography.
            using (var rsa = new RSACryptoServiceProvider())
            {
                if (isPrivateKey)
                {
                    rsa.ImportRSAPrivateKey(buffer, out _);
                    return rsa.ExportPkcs8PrivateKey();
                }
                else
                {
                    rsa.ImportRSAPublicKey(buffer, out _);
                    return rsa.ExportSubjectPublicKeyInfo();
                }
            }
        }
        /// <summary>
        /// Pkcs8转Pkcs1
        /// </summary>
        /// <param name="isPrivateKey"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static byte[] Pkcs8ToPkcs1(bool isPrivateKey, byte[] buffer)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                if (isPrivateKey)
                {
                    rsa.ImportPkcs8PrivateKey(buffer, out _);
                    return rsa.ExportRSAPrivateKey();
                }
                else
                {
                    rsa.ImportSubjectPublicKeyInfo(buffer, out _);
                    return rsa.ExportRSAPublicKey();
                }
            }
        }

        #endregion

        #region RSA

        /// <summary>
        /// 获取一个RSACryptoServiceProvider
        /// </summary>
        /// <param name="isPrivateKey"></param>
        /// <param name="buffer"></param>
        /// <param name="usePkcs8"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider CreateRSACryptoServiceProvider(byte[] buffer, bool isPrivateKey, bool usePkcs8 = false)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            if (isPrivateKey)
            {
                if (usePkcs8)
                    rsa.ImportPkcs8PrivateKey(buffer, out _);
                else
                    rsa.ImportRSAPrivateKey(buffer, out _);
            }
            else
            {
                if (usePkcs8)
                    rsa.ImportSubjectPublicKeyInfo(buffer, out _);
                else
                    rsa.ImportRSAPublicKey(buffer, out _);
            }
            return rsa;
        }

        /// <summary>
        /// RSA公钥加密
        /// </summary>
        /// <param name="value">待加密的明文</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="usePkcs8">是否使用pkcs8填充</param>
        /// <returns></returns>
        public static string RsaEncrypt(string value, byte[] publicKey, bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;

            using (RSACryptoServiceProvider rsa = CreateRSACryptoServiceProvider(publicKey, false, usePkcs8))
            {
                var buffer = Encoding.UTF8.GetBytes(value);
                buffer = rsa.Encrypt(buffer, false);
                return Convert.ToBase64String(buffer);
                //使用hex格式输出数据
                //StringBuilder result = new StringBuilder();
                //foreach (byte b in buffer)
                //{
                //    result.AppendFormat("{0:x2}", b);
                //}
                //var result = Encoding.Default.GetString(buffer);
                // Convert.FromBase64String
                //return result.ToString();
                //或者使用下面的输出
                //return BitConverter.ToString(buffer).Replace("-", "").ToLower();
            }
        }


        /// <summary>
        /// RSA公钥加密
        /// </summary>
        /// <param name="value">待加密的明文</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="usePkcs8">是否使用pkcs8填充</param>
        /// <returns></returns>
        public static string RsaEncryptHex(string value, byte[] publicKey, bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;

            using (RSACryptoServiceProvider rsa = CreateRSACryptoServiceProvider(publicKey, false, usePkcs8))
            {
                var buffer = Encoding.UTF8.GetBytes(value);
                buffer = rsa.Encrypt(buffer, false);

                //使用hex格式输出数据
                StringBuilder result = new StringBuilder();
                foreach (byte b in buffer)
                {
                    result.AppendFormat("{0:x2}", b);
                }
                return result.ToString();
                //或者使用下面的输出
                //return BitConverter.ToString(buffer).Replace("-", "").ToLower();
            }
        }



        public static string Encrypt(string value, string publicKey, bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;
            byte[] pub = Convert.FromBase64String(publicKey);
            return RsaEncrypt(value, pub, usePkcs8);
        }

        /// <summary>
        /// RSA私钥解密
        /// </summary>
        /// <param name="value">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="usePkcs8">是否使用pkcs8填充</param>
        /// <returns></returns>
        public static string RsaDecrypt(string value, byte[] privateKey, bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;

            using (RSACryptoServiceProvider rsa = CreateRSACryptoServiceProvider(privateKey, true, usePkcs8))
            {
                ////转换hex格式数据为byte数组
                //var buffer = new byte[value.Length / 2];
                //for (var i = 0; i < buffer.Length; i++)
                //{
                //    buffer[i] = (byte)Convert.ToInt32(value.Substring(i * 2, 2), 16);
                //}
                var val = Convert.FromBase64String(value);
                var buffer = rsa.Decrypt(val, false);            
                return Encoding.UTF8.GetString(buffer);
            }
        }


        /// <summary>
        /// RSA私钥解密
        /// </summary>
        /// <param name="value">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <param name="usePkcs8">是否使用pkcs8填充</param>
        /// <returns></returns>
        public static string RsaDecryptHex(string value, byte[] privateKey, bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;

            using (RSACryptoServiceProvider rsa = CreateRSACryptoServiceProvider(privateKey, true, usePkcs8))
            {
                //转换hex格式数据为byte数组
                var buffer = new byte[value.Length / 2];
                for (var i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)Convert.ToInt32(value.Substring(i * 2, 2), 16);
                }
                buffer = rsa.Decrypt(buffer, false);
                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static string Decrypt(string value, string privateKey, bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;
            byte[] pri = Convert.FromBase64String(privateKey);
            return RsaDecrypt(value, pri, usePkcs8);
        }
        /// <summary>
        /// RSA私钥生成签名
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="halg">签名hash算法：SHA，SHA1，MD5，SHA256，SHA384，SHA512</param>
        /// <param name="usePkcs8">是否使用pkcs8填充</param>
        /// <returns></returns>
        public static string Sign(string value, byte[] privateKey, string halg = "MD5", bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;

            using (RSACryptoServiceProvider rsa = CreateRSACryptoServiceProvider(privateKey, true, usePkcs8))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                buffer = rsa.SignData(buffer, HashAlgorithm.Create(halg));

                //使用hex格式输出数据
                StringBuilder result = new StringBuilder();
                foreach (byte b in buffer)
                {
                    result.AppendFormat("{0:x2}", b);
                }
                return result.ToString();
                //或者使用下面的输出
                //return BitConverter.ToString(buffer).Replace("-", "").ToLower();
            }
        }
        /// <summary>
        /// RSA公钥验证签名
        /// </summary>
        /// <param name="value">原始值</param>
        /// <param name="publicKey">公钥</param>
        /// <param name="signature">签名</param>
        /// <param name="halg">签名hash算法：SHA，SHA1，MD5，SHA256，SHA384，SHA512</param>
        /// <param name="usePkcs8">是否使用pkcs8填充</param>
        /// <returns></returns>
        public static bool Verify(string value, byte[] publicKey, string signature, string halg = "MD5", bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return false;

            using (RSACryptoServiceProvider rsa = CreateRSACryptoServiceProvider(publicKey, false, usePkcs8))
            {
                //转换hex格式数据为byte数组
                var buffer = new byte[signature.Length / 2];
                for (var i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (byte)Convert.ToInt32(signature.Substring(i * 2, 2), 16);
                }
                return rsa.VerifyData(Encoding.UTF8.GetBytes(value), HashAlgorithm.Create(halg), buffer);
            }
        }
        #endregion
    }
}