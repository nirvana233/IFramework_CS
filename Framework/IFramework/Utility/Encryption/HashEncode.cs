using System.Security.Cryptography;
using System.Text;
namespace IFramework.Utility.Encryption
{
#pragma warning disable CS1591 // ȱ�ٶԹ����ɼ����ͻ��Ա�� XML ע��
    public class HashEncode
    {

        /// �õ������ϣ�����ַ���
        public static string GetSecurity()
        {
            return HashEncoding(GetRandomValue());
        }
        /// �õ�һ�������ֵ
        public static string GetRandomValue()
        {
            System.Random Seed = new System.Random();
            return Seed.Next(1, int.MaxValue).ToString();
        }
        /// ��ϣ����һ���ַ���
        public static string HashEncoding(string Security)
        {
            byte[] Value;
            UnicodeEncoding Code = new UnicodeEncoding();
            byte[] Message = Code.GetBytes(Security);
            SHA512Managed Arithmetic = new SHA512Managed();
            Value = Arithmetic.ComputeHash(Message);
            Security = "";
            foreach (byte o in Value)
            {
                Security += (int)o + "O";
            }
            return Security;
        }
    }
#pragma warning restore CS1591 // ȱ�ٶԹ����ɼ����ͻ��Ա�� XML ע��
}