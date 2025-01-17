using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Medicare.Main;
using Medicare.Setup;
using Medicare.Utility;


public partial class CTPReportWindow : Form
{
    public static List<string> reportList = new List<string>();
    public static string reportText = string.Empty;
    public static string passFailReport = string.Empty;

    public CTPReportWindow()
    {
        InitializeComponent();
        //reportTextBox.Text = reportText;
        foreach (var s in reportList)
            reportTextBox.Text += s + "\n";
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    public static void savePassFailReport(bool isFail)
    {
        var port = CTPMain.selectedRFPort == 0 ? "RF1" : "RF2";
        var fileName = DateTime.Now.ToString("yyyyddMM") + "_" + DateTime.Now.ToString("HH_mm_ss") + "_" +
                       port + ".txt";
        try
        {
            string dir = string.Empty;

            if (!isFail)
            {
                dir = CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] +
                      @"\Result\Report\Pass\";
            }
            else
            {
                dir = CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] +
                      @"\Result\Report\Fail\";
            }

            File.AppendAllText(dir + fileName, passFailReport, Encoding.UTF8);
            Util.addLog("A new pass/fail result file : " + dir + fileName, CTPMain.selectedRFPort);
            passFailReport = string.Empty;
        }
        catch (Exception e)
        {
            Util.openPopupOk("Pass/Fail Result File : " + fileName + ".\nError - " +
                             e.Message);
            Util.addLog("Pass/Fail Result File : " + fileName + ".\nError - " + e.Message,
                CTPMain.selectedRFPort);
        }
    }
}
