using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace CLW.Model
{
    public class WatcherModel
    {

        public struct LoadingPresetResult
        {
            public bool Success { get; set; }
            public Exception Error { get; set; }
            public WatcherModel Result { get; set; }
            public bool UserCanceled { get; set; }

        }


        /// <summary>
        /// the ofiicailly useed loeading methode
        /// </summary>
        public async static Task<LoadingPresetResult> FromPresetFile(string PresetPath)
        {
            var newWM = new WatcherModel();
            try
            {
                IEnumerable<System.Xml.Linq.XAttribute> LWAttribs;
                newWM.CoreCustomLW = XMLLW.LoadXMLPreset(File.ReadAllText(PresetPath), out LWAttribs);

                if (File.Exists(newWM.CoreCustomLW.ReferenceFilePath))
                {
                    newWM.CoreCustomLW.InitialReferenceContent = File.ReadAllText(newWM.CoreCustomLW.ReferenceFilePath);

                    if (string.IsNullOrEmpty(newWM.CoreCustomLW.InitialReferenceContent))
                    {
                        MessageBox.Show($"Empty ref file! FBHD will attemp to fetch content from {newWM.CoreCustomLW.Href} and override the file:\n {newWM.CoreCustomLW.ReferenceFilePath}  ");
                        var userConfirmation = MessageBox.Show($"Empty ref file! FBHD will attemp to fetch content from {newWM.CoreCustomLW.Href} and override the file:\n {newWM.CoreCustomLW.ReferenceFilePath}  "
                            , "Empty ref file", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (userConfirmation == MessageBoxResult.Cancel)
                        {
                            return new LoadingPresetResult() {UserCanceled = true };
                        }
                        var data = await Services.WebClientMi.Native.getNewInstance().GetTextAdvanced (newWM.CoreCustomLW.Href);
                        if (data.Success)
                        {
                            File.WriteAllText(newWM.CoreCustomLW.ReferenceFilePath, data.Text);
                            newWM.CoreCustomLW.InitialReferenceContent = data.Text;
                        }
                        else
                        {
                            MessageBox.Show($"couldnt fetch the data, try later\n errorcoe:{data.ClientExitCode}");
                            throw new Exception("couldn't connect") ;// //done//todo: shold inform the caller that things went wrong
                        }
                    }
                }
                else
                {
                    var userConfirmation = MessageBox.Show($@"Reference file does not exist at {newWM.CoreCustomLW.ReferenceFilePath}{Environment.NewLine}
                FBHD will attempt to create one with initial data from {newWM.CoreCustomLW.Href}", "Missing ref", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (userConfirmation == MessageBoxResult.Cancel)
                    {
                        return new LoadingPresetResult() { UserCanceled = true };
                    }
                    var data = await Services.WebClientMi.Native.getNewInstance().GetTextAdvanced (newWM.CoreCustomLW.Href);
                    if (data.Success)
                    {
                        // create the directory first if it does not exist
                        if (Directory.Exists(Path.GetDirectoryName(newWM.CoreCustomLW.ReferenceFilePath)) == false)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(newWM.CoreCustomLW.ReferenceFilePath));
                        }
                        File.WriteAllText(newWM.CoreCustomLW.ReferenceFilePath, data.Text);
                        newWM.CoreCustomLW.InitialReferenceContent = data.Text;
                    }
                    else
                    {
                        MessageBox.Show($"couldnt fetch the data, try later\n errorcoe:{data.ClientExitCode}");
                        return new LoadingPresetResult() { UserCanceled = true };
                    }
                }
                var maybeColorAttrib = LWAttribs.FirstOrDefault((att) => att.Name.LocalName.ToLower() == "color");
                newWM.Color = ColorConverter.ConvertFromString(maybeColorAttrib.Value.ToString()) as Color?;

                return new LoadingPresetResult() { Success = true, Result = newWM };


            }




            catch (Exception err)
            {
                return new LoadingPresetResult() { Error = err };

            }
           

        }





        public Color? Color { get; set; }
        public CustomLW CoreCustomLW { get; set; }
        public Guid guid { get; internal set; }
    }
}
