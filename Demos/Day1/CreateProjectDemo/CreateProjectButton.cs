using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace CreateProjectDemo {
    internal class CreateProjectButton : Button {
        protected override async void OnClick() {
            //Settings used to create a new project
            CreateProjectSettings ps = new CreateProjectSettings();
            //Sets the path and name of the project that will be created
            ps.LocationPath = @"C:\ProSDKWorkshop\Data\Projects";
            ps.Name = "MyProject";
            int i = 1;
            //Create a Unique Name
            while (System.IO.Directory.Exists(System.IO.Path.Combine(
                ps.LocationPath, ps.Name))) {
                ps.Name = string.Format("MyProject{0}", i++);
            }
            //Create the new project
            await Project.CreateAsync(ps);
        }
    }
}
