using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Deployment.Application;

//dependency : System.Deployment.dll

namespace ChromaZ_WPF
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string version = ClickOnceVersion;
            if (null == version)
            {
                version = AssemblyVersion;
            }

            tb_update.Text = version;
        }
        public string ClickOnceVersion
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    tb_log.Text += "NetworkDeployed : \n";
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
                tb_log.Text += "LocalDeployed : \n";
                return null;
            }
        }
        //In AssemblyInfo.cs [assembly: AssemblyVersion("...")]
        public string AssemblyVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        void ad_UpdateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //Do something after the update. Maybe restart the application in order for the update to take effect.
            System.Diagnostics.Trace.WriteLine("[info] new version is updated. You should restart the app.");//the app is goling down for restart. ");
            //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            MessageBoxResult ret = MessageBox.Show("new version is updated. You should restart the app.", "Update Complete", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (ret == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
            
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                System.Deployment.Application.UpdateCheckInfo info = null;
                try
                {
                    ad.UpdateCompleted += new System.ComponentModel.AsyncCompletedEventHandler(ad_UpdateCompleted);
                    //ad_UpdateCompleted is a private method which handles what happens after the update is done
                    //tb_log.Text += "MinimumRequiredVersion" + info.MinimumRequiredVersion;
                    //tb_log.Text += "ActivationUri" + ad.ActivationUri;
                    tb_log.Text += "UpdateLocation" + ad.UpdateLocation;

                    info = ad.CheckForDetailedUpdate();

                    tb_log.Text += "AvailableVersion" + info.AvailableVersion;
                    tb_log.Text += "UpdateAvailable " + info.UpdateAvailable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    if (info.UpdateAvailable)
                    {
                        //You can create a dialog or message that prompts the user that there's an update. Make sure the user knows that your are updating the application.
                        ad.UpdateAsync();//Updates the application asynchronously
                    }
                }
            }
        }
    }
}
