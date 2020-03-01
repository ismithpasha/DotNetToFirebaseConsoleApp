using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using FcmSharp;
using FcmSharp.Model.Options;
using FcmSharp.Model.Topics;
using FcmSharp.Requests.Topics;
using FcmSharp.Settings;
using FirebaseDotNet.Database;
using static System.Console;

namespace FirebaseConsoleDotNet
{
   

    /// <summary>  
    /// Examples  
    /// </summary>  
    public class Program
    {
        /// <summary>  
        /// Main Method  
        /// </summary>  
        public static void Main()
        {
            UpdateData();

        }

        public static void UpdateData()
        {
            #region One
            //  Instanciating with base URL
            FirebaseDB firebaseDB = new FirebaseDB("https://your_app_id.firebaseio.com/");
            // Referring to Node with name "Teams"  
            FirebaseDB firebaseDBTeams = firebaseDB.Node("Teams");

            var data = @"{  
                                'Team-Awesome': {  
                                    'Members': {  
                                        'M1': {  
                                            'City': 'Dhaka',  
                                            'Name': 'Motahar Vai'  
                                            },  
                                        'M2': {  
                                            'City': 'Comilla',  
                                            'Name': 'Pasha vai'  
                                            },  
                                        'M3': {  
                                            'City': 'Kushtia',  
                                            'Name': 'Saikat Vai'  
                                            }  
                                       }  
                                   }  
                              }";

            WriteLine("GET Request");
            FirebaseResponse getResponse = firebaseDBTeams.Get();
            WriteLine(getResponse.Success);
            if (getResponse.Success)
                WriteLine(getResponse.JSONContent);
            WriteLine();

            WriteLine("PUT Request");
            FirebaseResponse putResponse = firebaseDBTeams.Put(data);
            WriteLine(putResponse.Success);
            WriteLine();

            WriteLine("POST Request");
            FirebaseResponse postResponse = firebaseDBTeams.Post(data);
            WriteLine(postResponse.Success);
            WriteLine();

            WriteLine("PATCH Request");
            FirebaseResponse patchResponse = firebaseDBTeams
                // Use of NodePath to refer path lnager than a single Node  
                .NodePath("Team-Awesome/Members/M2")
                .Patch("{\"Name\":\"Faysal Ahmed\"}");
            WriteLine(patchResponse.Success);
            WriteLine();

            WriteLine("PATCH Request");
            FirebaseResponse patchResponse2 = firebaseDB.Node("Users")
                // Use of NodePath to refer path lnager than a single Node  
                .NodePath("")
                .Patch("{\"Name\":\"Faysal Ahmed\"}");
            WriteLine(patchResponse.Success);
            WriteLine();
            
            WriteLine("DELETE Request");
            FirebaseResponse deleteResponse = firebaseDBTeams.Delete();
            // FirebaseResponse deleteResponse = firebaseDBTeams.NodePath("Team-Awesome/Members/M1").Delete();
            WriteLine(deleteResponse.Success);
            WriteLine();

            WriteLine(firebaseDBTeams.ToString());
            ReadLine();

            #endregion One
        }

        public static void  SendNotification()
        {
            //RegisterId you got from Android Developer.
            string deviceId = "ccylPX-QY4s:APA91bG20bQkbSV6jJQ_FANY3Jhwc0K5s43HHZIPLeKF6eLVU0WTyLaplje4_Q_cMpLq1veZJ_bFCVoZBba_1lSuhaCbdCgTc2oefCTHSWAF7TBL_Gk263AK_8DtqF_DceuR6qf7bWB-";
           
            string message = "Good Morning, Everything is on progress.";
            string tickerText = "Patient Registration";
            string contentTitle = "Alert";
            string postData =
            "{ \"registration_ids\": [ \"" + deviceId + "\" ], " +
              "\"notification\": {\"title\":\"" + contentTitle + "\", " +
                         "\"body\": \"" + message + "\"}}";

             string postData1 =
                 "{ \"registration_ids\": [ \"" + deviceId + "\" ], " +
                     "\"notification\": {\"title\":\"" + contentTitle + "\", " +
                         "\"body\": \"" + message + "\"," +
                         "\"click_action\":\"MainActivity\" }, " + 
                          
                         "\"data\" : {" +
                         "\"Nick\" : \"Mario\"," +
                         "\"Room\" : \"PortugalVSDenmark\" } }";


            // string postData =
            // "{ \"registration_ids\": [ \"" + deviceId + "\" ], " +
            //  "\"data\": {\"tickerText\":\"" + tickerText + "\", " +
            //             "\"contentTitle\":\"" + contentTitle + "\", " +
            //             "\"message\": \"" + message + "\"}}";

            string response = SendGCMNotification("AAAAkFr6lUo:APA91bH7d0J3vAQtSACGazxy3jPYPUltx4ktOjrocj7uq9rHrJAJbKsEeVujbNjq2CQSn3PvMgBLVCBBdM8u6NJuuJmupRuLw0gTbxjhgVbrpJevWYAfxkUydfFELgcTyDw6VoZv6Twa", postData);
          //  WriteLine("GET response");
            WriteLine(response);

        }

        private static string SendGCMNotification(string apiKey, string postData, string postDataContentType = "application/json")
        {
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);

            //  
            //  MESSAGE CONTENT  
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //  
            //  CREATE REQUEST  
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            Request.Method = "POST";
            //  Request.KeepAlive = false;  

            Request.ContentType = postDataContentType;
            Request.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            Request.ContentLength = byteArray.Length;

            Stream dataStream = Request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //  
            //  SEND MESSAGE  
            try
            {
                WebResponse Response = Request.GetResponse();

                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
                {
                    var text = "Unauthorized - need new token";
                    return text;
                }
                else if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    var text = "Response from web service isn't OK";
                    return text;
                }

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = Reader.ReadToEnd();
                Reader.Close();

                return responseLine;
            }
            catch (Exception e)
            {
            }
            return "error";
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        // =======
    }
}