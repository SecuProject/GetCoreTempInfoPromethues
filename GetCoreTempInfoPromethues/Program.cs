using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using GetCoreTempInfoNET;
using System.Threading;

namespace GetCoreTempInfoPromethues {
    class Program {

        static string TemplateMsg(string type) {
            string templateMsg = 
                "# HELP windows_coretemp_" + type + " Metric coming from Core Temp\n" +
                "# TYPE windows_coretemp_" + type + " counter\n";
            return templateMsg;
        }

        static void Main(string[] args) {
            CoreTempInfo CTInfo = new CoreTempInfo();
            CTInfo.ReportError += new ErrorOccured(CTInfo_ReportError);

            while (true) {
                bool bReadSuccess = CTInfo.GetData();

                if (bReadSuccess) {
                    uint index;

                    string path = @"C:\Program Files\windows_exporter\textfile_inputs\CoreTemp.prom";
                    string infoCodeTemp = "";
                    string infoCodeLoad = "";
                    string infoTjMax = "";

                    for (uint i = 0; i < CTInfo.GetCPUCount; i++) {
                        infoTjMax += "windows_coretemp_TjMax{core=\"" + i + "\"} " + CTInfo.GetTjMax[i] + "\n";
                        for (uint g = 0; g < CTInfo.GetCoreCount; g++) {
                            index = g + (i * CTInfo.GetCoreCount);
                            infoCodeTemp += "windows_coretemp_cpu_temp{core=\""+ i + "," + index + "\"} " + Convert.ToString(CTInfo.GetTemp[index]).Replace(",", ".") + "\n";
                            infoCodeLoad += "windows_coretemp_cpu_load{core=\"" + i + "," + index + "\"} " + CTInfo.GetCoreLoad[index] + "\n";
                        }
                    }
                    byte[] info = new UTF8Encoding(true).GetBytes(
                        TemplateMsg("TjMax") + infoTjMax +
                        TemplateMsg("cpu_temp") + infoCodeTemp +
                        TemplateMsg("cpu_load") + infoCodeLoad);
                    using (FileStream fs = File.Create(path)) {
                        fs.Write(info, 0, info.Length);
                    }
                } else {
                    string title = "Internal error";
                    string message = "name: " + CTInfo.GetLastError + "\n" +
                        "value: " + (int)CTInfo.GetLastError + "\n" +
                        "message: " + CTInfo.GetErrorMessage(CTInfo.GetLastError);
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                Thread.Sleep(15 * 1000);
            }
        }
        static void CTInfo_ReportError(ErrorCodes ErrCode, string ErrMsg) {
            string title = "Internal error";
            string message = "message: " + ErrMsg;

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
