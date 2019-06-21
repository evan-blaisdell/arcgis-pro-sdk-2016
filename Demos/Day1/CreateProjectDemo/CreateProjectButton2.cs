using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace CreateProjectDemo {
    internal class CreateProjectButton2 : Button {
        protected override async void OnClick() {
            var templates = await GetDefaultTemplatesAsync();
            var projectFolder = System.IO.Path.Combine(
                System.Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments),
                @"ArcGIS\Projects");

            CreateProjectSettings ps = new CreateProjectSettings() {
                Name = "MyProject",
                LocationPath = projectFolder,
                TemplatePath = templates[2]//2D "Map" template
            };
            int i = 1;
            //Create a Unique Name
            while (System.IO.Directory.Exists(System.IO.Path.Combine(
                ps.LocationPath, ps.Name))) {
                ps.Name = string.Format("MyProject{0}", i++);
            }
            var project = await Project.CreateAsync(ps);
        }

        public static string GetDefaultTemplateFolder() {
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string root = dir.Split(new string[] { @"\bin" }, StringSplitOptions.RemoveEmptyEntries)[0];
            return System.IO.Path.Combine(root, @"Resources\ProjectTemplates");
        }

        public static Task<List<string>> GetDefaultTemplatesAsync() {
            return Task.Run(() => {
                string templatesDir = GetDefaultTemplateFolder();
                return
                    Directory.GetFiles(templatesDir, "*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.EndsWith(".ppkx") || f.EndsWith(".aptx")).ToList();
            });
        }
    }
}
