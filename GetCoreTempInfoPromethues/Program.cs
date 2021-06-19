using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using GetCoreTempInfoNET;
using System.Globalization;
using System.Threading;

namespace GetCoreTempInfoPromethues {
    class Program {
        static void Main(string[] args) {
            CoreTempInfo CTInfo = new CoreTempInfo();
            CTInfo.ReportError += new ErrorOccured(CTInfo_ReportError);

            bool bReadSuccess = CTInfo.GetData();
            if (!bReadSuccess)
                Thread.Sleep(15 * 1000);
            while (true) {
                bReadSuccess = CTInfo.GetData();
                if (bReadSuccess) {
                    uint index;
                    const string helpMsgtemp = "# HELP windows_coretemp_cpu_temp Metric coming from Core Temp\n";
                    const string typeMsgtemp = "# TYPE windows_coretemp_cpu_temp counter\n";
                    const string helpMsgLoad = "# HELP windows_coretemp_cpu_load Metric coming from Core Temp\n";
                    const string typeMsgLoad = "# TYPE windows_coretemp_cpu_load counter\n";
                    string path = @"C:\Program Files\windows_exporter\textfile_inputs\CoreTemp.prom";
                    string infoCodeTemp = "";
                    string infoCodeLoad = "";

                    for (uint i = 0; i < CTInfo.GetCPUCount; i++) {
                        for (uint g = 0; g < CTInfo.GetCoreCount; g++) {
                            index = g + (i * CTInfo.GetCoreCount);
                            infoCodeTemp += "windows_coretemp_cpu_temp{core=\"0," + index + "\"} " + Convert.ToString(CTInfo.GetTemp[index], new CultureInfo("en-US")).Replace(",", ".") + "\n";
                            infoCodeLoad += "windows_coretemp_cpu_load{core=\"0," + index + "\"} " + CTInfo.GetCoreLoad[index] + "\n";
                        }
                    }
                    byte[] info = new UTF8Encoding(true).GetBytes(
                        helpMsgtemp + typeMsgtemp + infoCodeTemp +
                        helpMsgLoad + typeMsgLoad + infoCodeLoad);
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
