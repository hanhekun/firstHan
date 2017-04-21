using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;

namespace Aimer.SDK
{
    public class FittingRoomClient
    {
        string secret_code = "VDF81!$^&ASDNCC)(_):";
       

        public string GetRoomName()
        {
            return "AAAA";
        }

        public async Task<ScenResponse> GetScenesAsyc()
        {
            var Sign = md5(secret_code);
            
            using (HttpClient http=new HttpClient())
            {

               var data = await http.GetStringAsync("http://mobiletest.aimer.com.cn:8888/fittingroom/getscenes?sign=" + Sign);


               return JsonConvert.DeserializeObject<ScenResponse>(data);

            }

        }

        public async Task<GoodSceneResponse> GetGoodsScenesAsyc(string str)
        {
            var Sign = md5(secret_code);
     

            using (HttpClient http = new HttpClient())
            {

                var data = await http.GetStringAsync($"http://mobiletest.aimer.com.cn:8888/fittingroom/getgoodsscene?g_id={str}&sign=" + Sign);


                return JsonConvert.DeserializeObject<GoodSceneResponse>(data);

            }

        }


        private object md5(string key)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(secret_code, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var result = CryptographicBuffer.EncodeToHexString(hashed);
            return result;
        }

        private static string String2Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        
    }
}
