using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using ServiceAPI.Models.TransactionRequest;
using ServiceAPI.Models.TransactionResponse;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace ServiceAPI.Utils
{
    public class Utils
    {
        private static string EncryptABS(ref int c)
        {
            checked
            {
                if (c > 255)
                {
                    c -= 255;
                }
                else if (c < 0)
                {
                    c += 255;
                }

                return Conversions.ToString(c);
            }
        }

        public static string TBTEncrypt(string strText)
        {
            Random random = new Random();
            int num = random.Next(103000, 999179);
            int num2 = random.Next(101000, 744999);
            int num3 = Conversions.ToInteger(Strings.Left(Conversions.ToString(num2), 3));
            string text = Conversions.ToString(num) + Conversions.ToString(num2);
            string text2 = "";
            checked
            {
                int num4 = Strings.Len(strText) - 1;
                for (int i = 0; i <= num4; i++)
                {
                    int num5 = Strings.Asc(Strings.Mid(strText, i + 1, 1));
                    num5 += Strings.Asc(Strings.Mid(text, unchecked(i % Strings.Len(text)) + 1, 1));
                    text2 += Conversions.ToString(Strings.Asc(Strings.Chr(Conversions.ToInteger(EncryptABS(ref num5)))) + num3);
                }

                random = null;
                return Conversions.ToString(num) + text2 + Conversions.ToString(num2);
            }
        }

        public static string TBTDecrypt(string strText0)
        {
            checked
            {
                string result;
                try
                {
                    string text = "";
                    string text2 = "";
                    int num = Conversions.ToInteger(Strings.Left(strText0, 6));
                    int num2 = Conversions.ToInteger(Strings.Right(strText0, 6));
                    int num3 = Conversions.ToInteger(Strings.Left(Conversions.ToString(num2), 3));
                    string text3 = Conversions.ToString(num) + Conversions.ToString(num2);
                    int num4 = (int)Math.Round((double)Strings.Len(strText0) / 3.0 - 2.0);
                    for (int i = 3; i <= num4; i++)
                    {
                        text2 += Conversions.ToString(Strings.Chr((int)Math.Round(Conversions.ToDouble(Strings.Mid(strText0, 1 + (i - 1) * 3, 3)) - (double)num3)));
                    }

                    int num5 = Strings.Len(text2) - 1;
                    for (int i = 0; i <= num5; i++)
                    {
                        int num6 = Strings.Asc(Strings.Mid(text2, i + 1, 1));
                        num6 -= Strings.Asc(Strings.Mid(text3, unchecked(i % Strings.Len(text3)) + 1, 1));
                        text += Conversions.ToString(Strings.Chr(Conversions.ToInteger(EncryptABS(ref num6))));
                    }

                    result = text;
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Exception ex2 = ex;
                    result = "";
                    ProjectData.ClearProjectError();
                }

                return result;
            }
        } 

        public static string StrDec(ref string s)
        {
            string text = "";
            string text2 = s;
            int num = Conversions.ToInteger(Strings.Right(text2, 2));
            checked
            {
                int num2 = Conversions.ToInteger(Strings.Mid(text2, Strings.Len(text2) - 3, 2));
                short num3 = (short)Math.Round(20.0 + Conversions.ToDouble(Strings.Left(Conversions.ToString(num2), 1)) * Conversions.ToDouble(Strings.Right(Conversions.ToString(num2), 1)));
                int num4 = ((num2 > 66) ? 1 : ((num2 <= 33) ? 3 : 2));
                int num5 = Conversions.ToInteger(Strings.Mid(text2, Strings.Len(text2) - 3 - num4, num4));
                num5 ^= num;
                for (int i = 1; i <= num5; i++)
                {
                    text += Conversions.ToString(Strings.Chr((int)Math.Round(Conversions.ToDouble(Strings.Mid(text2, 4 + (i - 1) * 3, 3)) - (double)num3)));
                }
                return text;
            }
        }

        public static string AyXorStr(string targetString, string maskValue)
        {
            if (Operators.CompareString(maskValue, "", TextCompare: false) == 0)
            {
                return targetString;
            }

            int num = 0;
            StringBuilder stringBuilder = new StringBuilder();
            char[] array = targetString.ToCharArray();
            foreach (char c in array)
            {
                stringBuilder.Append(Strings.ChrW(c ^ Strings.AscW(maskValue.Substring(num, 1))));
                num = checked(num + 1) % maskValue.Length;
            }

            string result = stringBuilder.ToString();
            stringBuilder = null;
            return result;
        }

        public static string AyCompress(ref string Str1)
        {
            MemoryStream memoryStream = new MemoryStream();
            GZipStream stream = new GZipStream(memoryStream, CompressionMode.Compress);
            StreamWriter streamWriter = new StreamWriter(stream);
            streamWriter.Write(Str1);
            streamWriter.Close();
            byte[] array = memoryStream.ToArray();
            return Convert.ToBase64String(array, 0, array.Length);
        }

        public static string AyDeCompress(ref string Str2)
        {
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(Str2));
            GZipStream stream2 = new GZipStream(stream, CompressionMode.Decompress);
            StreamReader streamReader = new StreamReader(stream2);
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            return result;
        }

        public static CommmandResponseTo CheckErrorTo(string errcode, string Key)
        {
            CommmandResponseTo cmdResponse = new();
            switch (errcode)
            {
                case "06":
                    cmdResponse.description = " Kiểm tra " + Key + " danh mục khách ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "07":
                    cmdResponse.description = " Kiểm tra " + Key + " danh mục kho hàng ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "08":
                    cmdResponse.description = " Kiểm tra " + Key + " danh mục hàng hoá ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "09":
                    cmdResponse.description = " Kiểm tra " + Key + " Lệnh xuất kho xăng dầu ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "10":
                    cmdResponse.description = " Kiểm tra " + Key + " Danh mục đơn vị tính ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "11":
                    cmdResponse.description = " Kiểm tra " + Key + " Danh mục đơn vị tính ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "12":
                    cmdResponse.description = " Kiểm tra " + Key + " Danh mục phương tiện ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "13":
                    cmdResponse.description = " Kiểm tra " + Key + " Danh mục nguồn hàng ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "14":
                    cmdResponse.description = " Kiểm tra " + Key + " Danh mục Cửa hàng ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "15":
                    cmdResponse.description = " Kiểm tra " + Key + " Danh mục user tạo ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "16":
                    cmdResponse.description = " Kiểm tra lệnh " + Key + " không tồn tại ";
                    cmdResponse.errorCode = errcode;
                    break;
                case "17":
                    cmdResponse.description = " Kiểm tra phương thức xuất hàng " + Key + " không tồn tại ";
                    cmdResponse.errorCode = errcode;
                    break; 
                case "20":
                    cmdResponse.description = " Kiểm tra phương tiện " + Key + " đã tồn tại ";
                    cmdResponse.errorCode = errcode;
                    break;
                default:
                    cmdResponse.description = "Lỗi cấu trúc Json liên hệ admin để xử lý!";
                    cmdResponse.errorCode = errcode;
                    break;
            }
            return cmdResponse;
        }

        public static CommmandResponseGo CheckErrorGo(string errcode)
        {
            CommmandResponseGo cmdResponse = new();
            switch (errcode)
            {
                case "06":
                    cmdResponse.description = "Lỗi cấu trúc Json liên hệ admin để xử lý!";
                    cmdResponse.errorCode = errcode;
                    break;
                default:
                    cmdResponse.description = "Lỗi cấu trúc Json liên hệ admin để xử lý!";
                    cmdResponse.errorCode = errcode;
                    break;
            }
            return cmdResponse;
        }
    }
}
