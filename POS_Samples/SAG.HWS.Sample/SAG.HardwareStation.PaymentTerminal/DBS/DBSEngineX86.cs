using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SAG.HardwareStation.PaymentTerminal.DBS
{
    public class DBSEngineX86
    {
        #region import 32 bit Process
        const string SPECTRA_ECRDLL_x86 = "DBSDLL/x86/ECR32XP.dll";

        [DllImport(SPECTRA_ECRDLL_x86)]
        static extern short OpenConnection(string a, string b);

        [DllImport(SPECTRA_ECRDLL_x86)]
        static extern short CloseConnection();

        [DllImport(SPECTRA_ECRDLL_x86)]
        static extern int SendComm(byte[] buf, int len);

        [DllImport(SPECTRA_ECRDLL_x86)]
        static extern int ReceiveComm(byte[] buf);

        [DllImport(SPECTRA_ECRDLL_x86)]
        static extern short ReceiveStatus();
        #endregion

        public static DataContract.DBSResponse Run(string comPort, string ecr_msg)
        {
            DataContract.DBSResponse dbsResponse = new DataContract.DBSResponse();

            int bytesSent;
            byte[] enc_ecr_msg;
            int len = ecr_msg.Length;

            try
            {
                if (OpenConnection(comPort, "9600,n,8,1") == 1)
                {
                    //Message Convertion
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    enc_ecr_msg = encoding.GetBytes(ecr_msg);

                    //Send Message
                    bytesSent = SendComm(enc_ecr_msg, len);

                    if (bytesSent <= 0)
                    {
                        dbsResponse.ErrorMessage = "[FAIL]SendComm Fail";
                    }
                    else
                    {
                        byte[] rxBuf = new byte[1024];
                        int rxlen;

                        UTF8Encoding enc = new UTF8Encoding();

                        while (true)
                        {
                            string s;
                            if (ReceiveStatus() != 1)
                            {
                                System.Threading.Thread.Sleep(1);
                                continue;
                            }
                            if ((rxlen = ReceiveComm(rxBuf)) <= 0)
                            {
                                System.Threading.Thread.Sleep(1);
                                continue;
                            }
                            s = enc.GetString(rxBuf);
                            s = s.Substring(0, rxlen);
                            if (rxBuf[0] == 'P')
                            {
                                System.Diagnostics.Debug.WriteLine("[ OK ]Receive Polling Message len = {0}", rxlen);
                                System.Diagnostics.Debug.WriteLine(s);
                            }
                            else if (rxBuf[0] == enc_ecr_msg[0])
                            {
                                System.Diagnostics.Debug.WriteLine("[ OK ]Receive Correct Response len = {0}", rxlen);
                                System.Diagnostics.Debug.WriteLine(s);

                                // translate the response
                                dbsResponse = ECREngine.ConvertResponseData(s);

                                break;
                            }
                            else
                            {
                                dbsResponse.ErrorMessage = string.Format($"[FAIL]Receive Invalid Response len = {rxlen}");

                                System.Diagnostics.Debug.WriteLine("[FAIL]Receive Invalid Response len = {0}", rxlen);
                                System.Diagnostics.Debug.WriteLine(s);
                                break;
                            }
                        }
                    }

                    CloseConnection();
                    Console.WriteLine("[ OK ] CloseConnection");

                    //dbsResponse.ErrorMessage += "\r\n Data to send (Hex) : " + BitConverter.ToString(enc_ecr_msg);
                }
                else
                {
                    dbsResponse.ErrorMessage = "[FAIL]Open COM Port Fail";
                    System.Diagnostics.Debug.WriteLine("[FAIL]Open COM Port Fail");
                }
            }
            catch (Exception ex)
            {
                dbsResponse.ErrorMessage = ex.ToString();
            }
            finally
            {
                CloseConnection();
            }
            return dbsResponse;

        }
    }
}
